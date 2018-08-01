using AdminWeb.Models;
using AdminWeb.Services;
using AdminWeb.ViewModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PMSoft.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AdminWeb.Controllers
{
    [Authorize(Users = "admin")]
    public class MainController : AdminController
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly IExamService _examService;
        public MainController(ILogger logger
            , IUserService userService
            , IExamService examService)
        {
            _userService = userService;
            _examService = examService;
            _logger = logger;
        }

        #region page
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserPageList()
        {
            return View();
        }

        public ActionResult UserScoreList()
        {
            return View();
        }

        public ActionResult ProblemList()
        {
            return View();
        }

        public ActionResult ExamSetting()
        {
            return View();
        }
        #endregion

        public async Task<ActionResult> GetUserList(int page, int rows)
        {
            var userList = await _userService.GetUserList(page, rows);
            var json = new { total = await _userService.GetUserCount(), rows = userList };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetProblemList(int page, int rows, int? problemType)
        {
            try
            {
                var problemList = await _examService.GetProblemList(page, rows, problemType);
                var json = new { total = await _examService.GetProblemCount(problemType), rows = problemList };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _logger.Error("获取问题列表出错{0}", ex.InnerException);
            }
            return HttpNotFound();
        }

        public async Task<ActionResult> GetScoreType(int? problemType)
        {
            var score = await _examService.GetScoreType(problemType);
            return Json(score, JsonRequestBehavior.AllowGet);
        }
        

        public async Task<ActionResult> GetScoreList(int page, int rows, string SearchValue)
        {
            try
            {
                var userScoreList = await _examService.GetScoreList(page, rows, SearchValue);
                var json = new { total = await _examService.GetScoreCount(SearchValue), rows = userScoreList };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _logger.Error("获取考试列表出错{0}", ex.InnerException);
            }
            return HttpNotFound();
        }

        /// <summary>
        /// 考前发题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Issue()
        {
            var json = new mJsonResult();
            try
            {
                if (await _examService.Issue())
                {
                    json.success = true;
                    json.msg = "发放成功！";
                }
                else
                {
                    json.msg = "发题失败！";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("考前发题错误{0}", ex.Message);
                json.msg = "发题出现异常！";
            }
            return Json(json);
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveBaseSetting(BaseSetting model)
        {
            var json = new mJsonResult();
            try
            {
                if (await _examService.SaveBaseSetting(model))
                {
                    json.success = true;
                    json.msg = "保存成功！";
                }
                else
                {
                    json.msg = "保存失败！";
                }
            }
            catch (Exception ex)
            {
                _logger.Error("保存配置出现错误{0}", ex.Message);
                json.msg = "保存异常！";
            }
            return Json(json);
        }

        #region 用户导入
        public async Task<ActionResult> ImportUserExcel()
        {
            var json = new mJsonResult();
            string filePath = "";
            System.Web.HttpFileCollection _file = System.Web.HttpContext.Current.Request.Files;
            if (_file.Count > 0)
            {
                //文件大小   
                long size = _file[0].ContentLength;
                //文件类型   
                string type = _file[0].ContentType;
                //文件名   
                string name = _file[0].FileName;
                //文件格式   
                string _tp = System.IO.Path.GetExtension(name);

                if (_tp.ToLower() == ".xls" || _tp.ToLower() == ".xlsx")
                {
                    //获取文件流   
                    System.IO.Stream stream = _file[0].InputStream;
                    //保存文件   
                    string saveName = DateTime.Now.ToString("yyyyMMddHHmmss") + _tp;
                    string directory = Server.MapPath("/UpLoadFiles/");
                    //判断目录是否存在  

                    string path = directory + saveName;
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path);
                    }
                    _file[0].SaveAs(path);
                    filePath = path;
                }
            }
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    List<User> list = new List<User>();
                    list.AddRange(ImportUsers(filePath));
                    if (await _userService.InsertUserList(list))
                    {
                        json.success = true;
                        json.msg = "导入成功";
                    }
                    else
                    {
                        json.msg = "导入失败";
                    }

                }
                catch (Exception ex)
                {
                    _logger.Error("导入问题" + ex.Message);
                    json.msg = "导入失败！";
                }
            }
            else
            {
                json.success = false;
                json.msg = "请选择文件";
            }
            return Json(json, "text/html");
        }

        public List<User> ImportUsers(string filePath)
        {
            List<User> list = new List<User>();
            DataTable dt = new DataTable();
            dt = ImportExcelFile(filePath, 0);
            if (dt != null)
            {
                if (dt.Columns.Contains("姓名") && dt.Columns.Contains("手机号码")
                     && dt.Columns.Contains("序号") && dt.Columns.Contains("单位名称"))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        User model = new User();
                        model.UserName = dr["手机号码"].ToString();
                        if (string.IsNullOrEmpty(model.UserName))
                        {
                            break;
                        }
                        model.ID = Guid.NewGuid();
                        model.OrderNumber = Convert.ToInt16(dr["序号"]);
                        model.TrueName = dr["姓名"].ToString();
                        model.CompanyName = dr["单位名称"].ToString();
                        model.CreateDate = DateTime.Now;
                        list.Add(model);
                    }

                }
            }
            return list;
        }

        /// <summary>
        /// Excel导入
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public DataTable ImportExcelFile(string filePath, int index)
        {
            HSSFWorkbook hssfworkbook;
            #region//初始化信息
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            #endregion

            //using (NPOI.SS.UserModel.ISheet sheet = hssfworkbook.GetSheetAt(0))
            //{

            NPOI.SS.UserModel.ISheet sheet = hssfworkbook.GetSheetAt(index);
            DataTable table = new DataTable();
            IRow headerRow = sheet.GetRow(0);//第一行为标题行
            int cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells
            int rowCount = sheet.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1

            //handling header.
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }
            for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();

                if (row != null)
                {
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                            dataRow[j] = GetCellValue(row.GetCell(j));
                    }
                }

                table.Rows.Add(dataRow);
            }
            return table;
            //}

        }

        /// <summary>
        /// 根据Excel列类型获取列的值
        /// </summary>
        /// <param name="cell">Excel列</param>
        /// <returns></returns>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.BLANK:
                    return string.Empty;
                case CellType.BOOLEAN:
                    return cell.BooleanCellValue.ToString();
                case CellType.ERROR:
                    return cell.ErrorCellValue.ToString();
                case CellType.NUMERIC:
                case CellType.Unknown:
                default:
                    return cell.ToString();//This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number
                case CellType.STRING:
                    return cell.StringCellValue;
                case CellType.FORMULA:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
        #endregion

        #region 问题导入
        [HttpPost]
        public async Task<ActionResult> ImportExcel()
        {
            var json = new mJsonResult();
            string filePath = "";
            System.Web.HttpFileCollection _file = System.Web.HttpContext.Current.Request.Files;
            if (_file.Count > 0)
            {
                //文件大小   
                long size = _file[0].ContentLength;
                //文件类型   
                string type = _file[0].ContentType;
                //文件名   
                string name = _file[0].FileName;
                //文件格式   
                string _tp = System.IO.Path.GetExtension(name);

                if (_tp.ToLower() == ".xls" || _tp.ToLower() == ".xlsx")
                {
                    //获取文件流   
                    System.IO.Stream stream = _file[0].InputStream;
                    //保存文件   
                    string saveName = DateTime.Now.ToString("yyyyMMddHHmmss") + _tp;
                    string directory = Server.MapPath("/UpLoadFiles/");
                    //判断目录是否存在  

                    string path = directory + saveName;
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path);
                    }
                    _file[0].SaveAs(path);
                    filePath = path;
                }
            }
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                try
                {
                    List<Problem> list = new List<Problem>();
                    list.AddRange(ImportSelect(filePath, 0));
                    list.AddRange(ImportSelect(filePath, 1));
                    list.AddRange(ImportSelect(filePath, 2));
                    if (await _examService.InsertProblemList(list))
                    {
                        json.success = true;
                        json.msg = "导入成功";
                    }
                    else
                    {
                        json.msg = "导入失败";
                    }
                    json.success = true;
                    json.msg = "导入成功";
                }
                catch (Exception ex)
                {
                    _logger.Error("导入问题" + ex.Message);
                    json.msg = "导入失败！";
                }
            }
            else
            {
                json.success = false;
                json.msg = "请选择文件";
            }
            return Json(json, "text/html");
        }

        /// <summary>
        /// 题目导入
        /// </summary>
        public List<Problem> ImportSelect(string filePath, int index)
        {
            List<Problem> list = new List<Problem>();
            DataTable dt = new DataTable();
            dt = ImportExcelFile(filePath, index);
            if (dt != null)
            {
                if (dt.Columns.Contains("题号")
                    && dt.Columns.Contains("题目") && dt.Columns.Contains("A")
                    && dt.Columns.Contains("B") && dt.Columns.Contains("C")
                    && dt.Columns.Contains("D") && dt.Columns.Contains("E")
                    && dt.Columns.Contains("答案") && dt.Columns.Contains("分值"))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Problem model = new Problem();
                        model.ID = Guid.NewGuid();
                        model.ProblemName = dr["题目"].ToString();
                        if (string.IsNullOrEmpty(model.ProblemName))
                        {
                            break;
                        }
                        model.ProblemType = index + 1;
                        model.Score = dr["分值"] != null ? Convert.ToDecimal(dr["分值"]) : 0;
                        model.QuestionNumber = dr["题号"] != null ? Convert.ToInt16(dr["题号"]) : 0;
                        if (index == 0)
                        {
                            model.ProblemSubType = dr["组别"] != null ? Convert.ToInt16(dr["组别"]) : 0;
                        }
                        model.ProblemFeatures = GetProblemFeatures(dr);
                        model.Answer = dr["答案"].ToString().Replace("\n", "").Replace(" ", "")
                            .Replace("\t", "").Replace("\r", "");
                        list.Add(model);
                    }

                }
            }
            return list;
        }

        /// <summary>
        /// 获取题目
        /// </summary>
        /// <returns></returns>
        public string GetProblemFeatures(DataRow dr)
        {
            string reStr = string.Empty;
            if (dr["A"].ToString() != "")
            {
                reStr += "A、" + dr["A"].ToString() + "$";
            }
            if (dr["B"].ToString() != "")
            {
                reStr += "B、" + dr["B"].ToString() + "$";
            }
            if (dr["C"].ToString() != "")
            {
                reStr += "C、" + dr["C"].ToString() + "$";
            }
            if (dr["D"].ToString() != "")
            {
                reStr += "D、" + dr["D"].ToString() + "$";
            }
            if (dr["E"].ToString() != "")
            {
                reStr += "E、" + dr["E"].ToString() + "$";
            }
            return reStr;
        }
        #endregion

        #region 成绩导出
        public async Task<FileResult> ExportExcel(string SearchValue)
        {
            //获取list数据
            var list = await _examService.GetScoreList(1, 10000, SearchValue);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("姓名");
            row1.CreateCell(1).SetCellValue("总分");
            row1.CreateCell(2).SetCellValue("排名");
            row1.CreateCell(3).SetCellValue("争分夺秒 ");
            row1.CreateCell(4).SetCellValue("一比高下");
            row1.CreateCell(5).SetCellValue("狭路相逢");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].TrueName);
                rowtemp.CreateCell(1).SetCellValue(list[i].TotalScores?.ToString());
                rowtemp.CreateCell(2).SetCellValue(list[i].rownum);
                rowtemp.CreateCell(3).SetCellValue(list[i].TypeScores1?.ToString());
                rowtemp.CreateCell(4).SetCellValue(list[i].TypeScores2?.ToString());
                rowtemp.CreateCell(5).SetCellValue(list[i].TypeScores3?.ToString());
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "考生成绩.xls");

        }
        #endregion
    }
}