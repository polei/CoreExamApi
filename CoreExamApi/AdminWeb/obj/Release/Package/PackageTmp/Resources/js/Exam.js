function goTo(url, menutype) {
    if (menutype == '6') {
        alertConfirm({
            msg: '你确定已经提交所有试卷并且退出系统吗？',
            fn1: function () {
                window.location.href = "LoginOut.aspx";
            },
            fn2: function () { }
        });
    } else {
        window.location.href = url;
    }
}

function ShowDailog(amsg) {

    alertMessage({ msg: amsg });
}

function ExamOver() {
    alertMessage({ msg: '本次考试已结束！' })
    window.location.href = "Login.aspx";
}
function DownLoadTemplate(Name, Path) {
    /// <summary>下载模板</summary>
    /// <param name="Name" type="String">附件名称</param>
    /// <param name="Path" type="String">附件路径</param>

    window.location.href = "/Scripts/PublicPackages/DownLoad.ashx?Path=" + encodeURIComponent(Path) + "&Name=" + encodeURIComponent(Name) + "&r=" + Math.random();
}

function DownLoadResouse(path) {
    window.open(path);
}
