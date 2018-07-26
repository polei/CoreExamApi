//得到窗口句柄
function getParentDialogWindow() {
    var parentPage = window.parent;
    var win = window;
    while (parentPage != win) {
        parentPage = parentPage.parent;
        win = win.parent;
    }
    return win;
}

//取得最后一个弹出层索引
function getLastDialogWindowIndex() {
    var parentObj = getParentObj();
    return parentObj.find("div.window").length - 1;
}

function getParentObj() {
    return $(window.parent.document);
}

//弹出层确定按钮操作子页面提交,btnid子页面提交按钮的id
function childrenPage(dialogObj, btnid) {
    dialogObj.find("iframe").contents().find("#" + btnid).click();
}

//子页面调用弹出层外按钮事件
//function parentBtn(btnid) {
//    getParentObj.find("#dlg-buttons").find("#saveBtn").click(function () {
//        $("#" + btnid).click();
//    });
//}

//移除HTML
function removeDialogHtml(dialogObj) {
    dialogObj.parent().next().next().remove();
    dialogObj.parent().next().remove();
    //dialogObj.parent().remove();
}

//设置窗口回调函数参数
function setWindowCallBackArgs(argStr) {
    getParentObj().find("div.easyui-dialog:last").attr("args", argStr);
}

//始终为最外框架弹出层
function parentdialog(title, width, height, url, button, callback) {
    var win = getParentDialogWindow();
    win._openWindow(title, width, height, url, button, callback);
}

function openWindow(title, width, height, url, button, callback) {
    var win = getParentDialogWindow();
    win._openWindow(title, width, height, url, button, callback);
}

//父页面弹出层,title弹出层的标题，width弹出层的宽度，height弹出层的高度，url弹出层子页面的地址，button为"",表示不需要定义按钮，不然为一个json对象，text是按钮的名称，targetid为操作按钮的id
function _openWindow(title, width, height, url, button, callback) {
    if (!callback) callback = function (args) { }; //回调函数
    var width = width == "screenwidth" ? $(window).width() : width; //WINDOW宽度
    var height = height == "screenheight" ? $(window).height() : height; //WINDOW高度
    var bindButton = "";

    //构造DIALOG对象
    var Dialog = $("<div>", {
        "class": "easyui-dialog",
        "closed": true,
        "html": "<div class=\"loading\"><img src=\"/images/load.gif\" />页面加载中...</div><iframe src='' name='dialogWindow' width='100%' marginwidth='0' height='100%' marginheight='0' scrolling='no' frameborder='0'></iframe>"
    });

    //操作按钮
    if (button != "" || button != false) {
        var bindButtonstr = "[";
        $.each(button, function (i) {
            bindButtonstr += "{ text:'" + button[i].text + "', handler: function () { childrenPage(Dialog, '" + button[i].targetid + "') }},"
        });
        bindButtonstr += "{ text: '取消', id:'dialog-btn-cancel', handler: function () { Dialog.dialog('close'); } }]";
        bindButton = eval("(" + bindButtonstr + ")");
    }
    else {
        bindButtonstr = "[{ text: '关闭', id:'dialog-btn-cancel', handler: function () { Dialog.dialog('close'); } }]";
        bindButton = eval("(" + bindButtonstr + ")");
    }

    var iframeObj = Dialog.find("iframe");
    var loadingObj = Dialog.find(".loading");
    iframeObj.hide(); //隐藏iframe
    loadingObj.show(); //显示加载图标

    Dialog.dialog({
        modal: true,
        closed: false,
        width: width,
        height: height,
        title: title,
        buttons: bindButton,
        onClose: function () {
            var args = {};
            try {

                var argsStr = Dialog.attr("args");
                if (argsStr != "" && argsStr != undefined) {
                    args = eval('(' + argsStr + ')');
                    //清除参数
                    Dialog.attr("args", "");
                }

                Dialog.find("iframe").each(function () {
                    var iframeObj = $(this);
                    iframeObj.attr("src", "about:blank");
                    //iframeObj.contentWindow.document.write('');
                    //iframeObj.parentNode.removeChild(iframeObj);
                });
            } catch (err) {
                alert(err);
            } finally {
                //移除HTML
                removeDialogHtml(Dialog);

                //回调函数
                callback(args);

                Dialog.dialog('destroy');
            }
        },
        onDestroy: function () {
            //alert("onDestroy");
        }
    });

    iframeObj.attr("src", url);
    //页面加载完成
    iframeObj.load(function () {
        var mainheight = Dialog.find(".dialog-content").outerHeight(true);
        $(this).contents().find("#main").height(mainheight);
        //$(this).contents().find("#pmain").height(mainheight);
        
        $(this).show(); //显示iframe
        loadingObj.hide(); //隐藏加载图标
    });
}

//本页面弹出层
function pageDialog(divid, title, width, height, button) {
    if (button == true) {
        $("#" + divid).dialog({
            buttons: "#dlg-buttons"
        });
    }
    else {
        $("#" + divid).dialog({
            buttons: null
        });
    }
    $("#" + divid).dialog({
        modal: true,
        closed: false,
        width: width,
        height: height,
        title: title
    });
}

//========================================================================
//关闭当前窗口脚本函数
//args:{ func: '', args: ''}
//func:脚本函数
//args:参数
//========================================================================
function closeWindow(args) {
    var win = getParentDialogWindow();
    if (args.timeout == undefined) {
        args.timeout = 0;
    }
    setTimeout(function () {
        win._closeWindow(args);
        win._closeMessage();

        //win._closeLoading();
    }, args.timeout);
}
function _closeWindow(args) {
    var parentObj = getParentObj();
    var btnCancelObj = parentObj.find(".dialog-button:last #dialog-btn-cancel");
    btnCancelObj.click();
}

//========================================================================
//根据索引关闭窗口脚本函数
//args:{ func: '', args: '' }
//func:脚本函数
//args:参数
//========================================================================
function closeIndexWindow(args) {
    var win = getParentDialogWindow();
    if (args.timeout == undefined) {
        args.timeout = 0;
    }
    setTimeout(function () {
        if (typeof args.func == "function")
        {
            args.func();
        }

        win._closeIndexWindow(args);
        win._closeMessage();

        //win._closeLoading();
    }, args.timeout);
}
function _closeIndexWindow(args) {
    var parentObj = getParentObj();
    var btnCancelObj = parentObj.find(".dialog-button:eq(" + args.index + ") #dialog-btn-cancel");
    btnCancelObj.click();
}

//========================================================================
//调用父窗口脚本函数
//args:{ func: '', args: ''}
//func:脚本函数
//args:参数
//示例：invokeParentWindowFunc({ func: 'doUpdate', args: { title: '名称' } })
//========================================================================
function invokeParentWindowFunc(args) {
    var win = getParentDialogWindow();
    win._invokeParentWindowFunc(args);
}
function _invokeParentWindowFunc(args) {
    var index = getLastDialogWindowIndex()-1;
    var parentObj = getParentObj();
    var iframeObj = parentObj.find("iframe[name='dialogWindow']")[index];
    try {
        if (iframeObj == null) {
            throw "窗口不存在";
        }
        var func = eval("iframeObj.contentWindow." + args.func);
        if (typeof func == 'function') {
            func(args.args);
        } else {
            throw "父页面函数[" + args.func + "]不存在";
        }
    }
    catch (err) {
        alert(err);
    }
}

//========================================================================
//调用指定窗口脚本函数
//args:{ func: '', args: ''}
//func:脚本函数
//args:参数-index:第几个打开的窗口，第一个则index为0
//示例：invokeIndexWindowFunc({ func: 'doUpdate', args: { title: '名称', index:2 } })
//add by zc 2014-8-8
//========================================================================
function invokeIndexWindowFunc(args) {
    var index = args.args.index;
    var parentObj = getParentObj();
    var iframeObj = parentObj.find("iframe[name='dialogWindow']")[index];
    try {
        if (iframeObj == null) {
            throw "窗口不存在";
        }
        var func = eval("iframeObj.contentWindow." + args.func);
        if (typeof func == 'function') {
            func(args.args);
        } else {
            throw "页面函数[" + args.func + "]不存在";
        }
    }
    catch (err) {
        alert(err);
    }
}

//========================================================================
//调用MainIframe页面脚本函数
//args：{ func: '', args: ''}
//func：脚本函数
//args：参数
//示例：invokeMainIframeFunc({ func: 'doUpdate', args: { title: '名称' } })
//========================================================================
function invokeMainIframeFunc(args) {
    var win = getParentDialogWindow();
    win._invokeMainIframeFunc(args);
}
function _invokeMainIframeFunc(args) {
    var parentObj = getParentObj();
    var iframe = parentObj.find("#mainWin")[0];
    try {
        if (iframe == null) {
            throw "mainFrame窗口不存在";
        }
        var func = eval("iframe.contentWindow." + args.func);
        if (typeof func == 'function') {
            func(args.args);
        } else {
            throw "mainFrame窗口页面函数[" + args.func + "]不存在";
        }
    }
    catch (err) {
        alert(err);
    }
}


//========================================================================
//提示消息
//args:{ msg: '', timeout: 5000 }
//示例：alertMessage({ msg: '保存失败' })
//========================================================================
function alertMessage(args) {
    var win = getParentDialogWindow();
    win._bindMessage(args);
}
function _bindMessage(args) {
    if (args.title == undefined) {
        args.title = "提示";
    }
    $.messager.alert(args.title, args.msg, 'info');

    if (args.timeout != undefined) {
        setTimeout(function () {
            _closeMessage();
        }, args.timeout);
    }
}
function closeMessage() {
    var win = getParentDialogWindow();
    win._closeMessage();
}
function _closeMessage() {
    $(".messager-window a.panel-tool-close").click();
}


//========================================================================
//错误消息
//args:{ msg: ''}
//示例：alertError({ msg: '保存失败' })
//========================================================================
function alertError(args) {
    var win = getParentDialogWindow();
    win._bindError(args);
}
function _bindError(args) {
    if (args.title == undefined) {
        args.title = "错误";
    }
    $.messager.alert(args.title, args.msg, 'error');
}



//========================================================================
//警告消息
//args:{ msg: ''}
//示例：alertWarning({ msg: '删除后数据不可恢复' })
//========================================================================
function alertWarning(args) {
    var win = getParentDialogWindow();
    win._bindWarning(args);
}
function _bindWarning(args) {
    if (args.title == undefined) {
        args.title = "警告";
    }
    $.messager.alert(args.title, args.msg, 'warning');
}



//========================================================================
//选择提示
//args:{ msg: '', fn1: null, fn2: null}
//示例：alertConfirm({ msg: '是否删除', fn1: function(){ //... }, fn2: function(){ //... } })
//========================================================================
function alertConfirm(args) {
    var win = getParentDialogWindow();
    win._bindConfirm(args);
}
function _bindConfirm(args) {
    if (args.title == undefined) {
        args.title = "选择";
    }
    $.messager.confirm(args.title, args.msg, function (r) {
        if (r) {
            if (args.fn1 != null && typeof args.fn2 != 'undefined') {
                args.fn1();
            }
        } else {
            if (args.fn2 != null && typeof args.fn2 != 'undefined') {
                args.fn2();
            }
        }
    });
}

//========================================================================
//加载提示框
//args:{ title: '', msg: ''}
//示例：openLoading({ title: '加载', msg: '数据加载中，请稍等...' })
//========================================================================
function openLoading(args) {
    var win = getParentDialogWindow();
    args.msg = "<table><tr><td><img src='/images/loading.gif' align='absmiddle'> " + args.msg + "</td></tr></table>";
    //win._openLoading(args);
    win._bindMessage(args);
}
function _openLoading(args) {
    if (args.title == undefined) {
        args.title = "提示";
    }
    $.messager.show({
        modal: true,
        title: args.title,
        msg: args.msg,
        showType: null,
        style: {
            right: '',
            bottom: ''
            
        }
    });
}
//关闭
function closeLoading() {
    var win = getParentDialogWindow();
    win._closeLoading();
}
function _closeLoading() {
    //var closeObj = $("a.panel-tool-close:last");
    var closeObj = $(".messager-window a.panel-tool-close");
    closeObj.click();
}



//========================================================================
//进度提示框
//args:{ title: '', msg: '', timeout: 5000 }
//time:毫秒
//========================================================================
function openProgress(args) {
    var win = getParentDialogWindow();
    win._bindProgress(args);
}
function _bindProgress(args) {
    if (args.title == undefined) {
        args.title = "进度";
    }
    $.messager.progress(args);

    if (args.timeout != undefined) {
        setTimeout(function () {
            _closeProgress();
        }, args.timeout);
    }
}
function closeProgress() {
    var win = getParentDialogWindow();
    win._closeProgress();
}
function _closeProgress() {
    $.messager.progress('close');
}



//========================================================================
//显示消息框
//args:{ msg: '', showType: null, timeout: 5000 }
//示例：openMessage({ title: '提示', msg: '保存成功', showType: null, timeout: 2000 })
//========================================================================
//function openMessage(args) {
//    var win = getParentDialogWindow();
//    win._openMessage(args);
//}
//function _openMessage(args) {
//    if (args.title == undefined) {
//        args.title = "提示";
//    }
//    $.messager.show({
//        title: args.title,        
//        msg: args.msg,
//        showType: args.showType,
//        timeout: args.timeout,
//        style: {
//            right: '',
//            bottom: ''
//        }
//    });
//}
////关闭
//function closeMessage() {
//    var win = getParentDialogWindow();
//    win._closeMessage();
//}
//function _closeMessage() {
//    $("a.panel-tool-close").click();
//}