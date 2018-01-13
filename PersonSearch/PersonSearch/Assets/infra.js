/// <reference path="../jquery-1.7.2.min.js" />
var _menuClicked;
var _showLoader = false;

var plmPublisher = {
    subscribers: {
        defaultsub: []	/* subscribers */
    },
    subscribe: function (fnPtr, type) {
        type = type || 'defaultsub';
        if (typeof this.subscribers[type] === "undefined") {
            this.subscribers[type] = [];
        }
        this.subscribers[type].push(fnPtr);
    },
    unsubscribe: function (fnPtr, type) {
        this.loopsubscribers('unsubscribe', fnPtr, type);
    },
    publish: function (data, type) {
        this.loopsubscribers('publish', data, type);
    },
    loopsubscribers: function (action, arg, type) {
        var pubtype = type || 'defaultsub',
            subscribers = this.subscribers[pubtype],
            i,
            max = subscribers != undefined ? subscribers.length : 0;

        for (i = 0; i < max; i += 1) {
            if (action === 'publish') {
                subscribers[i](arg);
            } else {
                if (subscribers[i] === arg) {
                    subscribers.splice(i, 1);
                }
            }
        }
    },
    destroy: function (type) {
        var pubtype = type || 'defaultsub',
        subscribers = this.subscribers[pubtype],
                i,
                max = subscribers != undefined ? subscribers.length : 0;
        for (i = 0; i < max; i += 1) {
            subscribers.splice(i, 1);
        }
        delete this.subscribers[pubtype];

    }
};

function makePublisher(o) {
    var i;
    for (i in plmPublisher) {
        if (plmPublisher.hasOwnProperty(i) && typeof plmPublisher[i] === "function") {
            o[i] = plmPublisher[i];
        }
    }
    o.subscribers = { defaultsub: [] };
}

$.fn.fillcontent = function () {
    return this.each(function () {
        var $contentElemenent = $(this);
        /* set the max height of this area to the bottom of the form and track any resize events... */
        handleFrameResize($contentElemenent);
        $(window).resize(function () {
            handleFrameResize($contentElemenent);
        });

        function handleFrameResize($contentElemenent) {
            var next = $contentElemenent.next(":visible").not(".inforFormButton"),
                maxHeight = $(window).height() - $contentElemenent.offset().top - next.height();	//the height of the nav

            $contentElemenent.css({ "border": "none" });
            if (next.hasClass("inforBottomFooter"))
                maxHeight -= 2;	/* subtract 2 for the border */

            $contentElemenent.height(maxHeight);
            $contentElemenent.width($(window).width());
        }
    });
};



function plm_navigate_to(url, data, control) {
    url = getFullUrl(url);

    if (typeof unloadview == "function")
        unloadview();
    // publishEmptyMessage();
    $("#" + control).html = "";
    $("#" + control).load(url, data, function (event, xhr, settings) { plm_ajax_success(event, xhr, settings); });
    return false;
}

function plm_ajax_post(url, data, successcallback, errorcallback, isASynchronous) {
    //<![CDATA[
    //var href = location.href;
    //if (href.substring(href.length - 1) != "/") {
    //    href = href + "/";
    //}
    //url = href + url;    
    //START:kkrishnamurty  code changes to post always a new form
    //]]>
    var t = new Date;
    t = t.getTime();

    if (data == null || data == undefined)
        data = {};
    /*END:kkrishnamurty  code changes to post always a new form */

    data.t = t;
    url = getFullUrl(url);
    /* _showLoader = true; */
    var jqxhr = $.ajax({ type: "POST", url: url, data: data, async: isASynchronous })
    .done(function (data, textStatus, jqXHR) { removeoverlay(); plm_ajax_success(data, textStatus, jqXHR, successcallback); })
    .fail(function (jqXHR, textStatus, errorThrown) { removeoverlay(); plm_ajax_error(jqXHR, textStatus, errorThrown, errorcallback); })
}

function plm_ajax_json_post(url, data, successcallback, errorcallback, isASynchronous) {
    var t = new Date;
    t = t.getTime();

    if (data == null || data == undefined)
        data = {};

    data.t = t;
    url = getFullUrl(url);

    var jqxhr = $.ajax({
        url: url,
        async: isASynchronous,
        type: 'POST',
        data: JSON.stringify(data),
        dataType: 'json',
        traditional: true,
        contentType: 'application/json'
    })
    .done(function (data, textStatus, jqXHR) { removeoverlay(); plm_ajax_success(data, textStatus, jqXHR, successcallback); })
    .fail(function (jqXHR, textStatus, errorThrown) { removeoverlay(); plm_ajax_error(jqXHR, textStatus, errorThrown, errorcallback); })
}

function plm_ajax_get(url, data, successcallback, errorcallback, isASynchronous) {
    /* START:kkrishnamurty  code changes to make always a fresh request */
    var t = new Date;
    t = t.getTime();
    if (data == null || data == undefined)
        data = {};
    data.t = t;
    /* //END:kkrishnamurty  code changes to make always a fresh request */

    _menuClicked = false;
    url = getFullUrl(url);
    var jqxhr = $.ajax({ url: url, data: data, async: isASynchronous })
    .done(function (data, textStatus, jqXHR) { removeoverlay(); plm_ajax_success(data, textStatus, jqXHR, successcallback); })
    .fail(function (jqXHR, textStatus, errorThrown) { removeoverlay(); plm_ajax_error(jqXHR, textStatus, errorThrown, errorcallback); })
}

function loadOverlayIndicator() {
}

function showOverlayIndicator() {
    
}

function removeoverlay() {
    
}

function hideOverlay() {
    // $("body").inforBusyIndicator("close");
    _showLoader = false;
}
function scallback(data, textStatus, jqXHR) {

}

function InvokeExport(url, data) {
    //<![CDATA[
    //var href = location.href;
    //if (href.substring(href.length - 1) != "/") {
    //    href = href + "/";
    //}
    //url = href + url;    
    //]]>
    loadOverlayIndicator();
    url = getFullUrl(url);
    location.href = GetDecodeURIFromJson(url, data);
    removeoverlay();
}

function InvokeExportEx(url, data) {

    url = getFullUrl(url);
    var form = $("<form></form>").appendTo("body")
        .attr("action", url)
        .attr("method", "POST");

    $("<input type='hidden'/>").appendTo(form)
      .attr("name", "data")
      .attr("value", data);

    form.submit();
    form.remove();
}

function GetDecodeURIFromJson(url, data) {
    return url + "?" + decodeURIComponent($.param(data));
}

function GetDecodeDataFromJson(data) {
    return decodeURIComponent($.param(data));
}

function fcallback(jqXHR, textStatus, errorThrown) {

}

function plm_ajax_success(data, textStatus, jqXHR, callback) {

    if (typeof callback == "function")
        callback(data, textStatus, jqXHR);
    removeoverlay();
}

function showDataDialog(options) {
    /* //$('body').inforDataDialog({ url: "/Home/Dialog", options: null, height: "auto", width: 800, "id": "newCatalogDialog", tabid: "inforTabContainer" }); */
    $('body').inforDataDialog(options);
}
function plm_ajax_error(jqXHR, textStatus, errorThrown, callback) {

    /* //DisplayYesNoCancelDlg("title", "short message", "detail message",  null, null, null); */
    var tmp = jqXHR.responseText;
    var result;
    if (textStatus != "timeout") {
        try {
            result = jQuery.parseJSON(tmp);

        } catch (err) {

        }
        if (result && result.IsRedirect) {
            location.href = result.URL;
            return;
        }
    }

    /* // handles error, redirection etc... */
    if (typeof callback == "function")
        callback(jqXHR, textStatus, errorThrown);
}

function DisplayError(ititle, ishortMessage, idetailMessage) {
    showCustomMessage(ititle, ishortMessage, idetailMessage, "Error");
}
function DisplayAlert(ititle, ishortMessage, idetailMessage) {
    showCustomMessage(ititle, ishortMessage, idetailMessage, "Alert");
}
function DisplayInfo(ititle, ishortMessage, idetailMessage) {
    showCustomMessage(ititle, ishortMessage, idetailMessage, "Information");
}
function DisplayYesNoDlg(ititle, ishortMessage, idetailMessage, yesptr, noptr) {
    showConfirmationMessage(ititle, ishortMessage, idetailMessage, false, yesptr, noptr, null, 20);
}
function DisplayYesNoCancelDlg(ititle, ishortMessage, idetailMessage, yesptr, noptr, cancelptr) {
    showConfirmationMessage(ititle, ishortMessage, idetailMessage, false, yesptr, noptr, cancelptr, 30);
}
function showCustomMessage(ititle, ishortMessage, idetailMessage, idialogType) {
    $('body').inforMessageDialog({
        title: ititle,
        shortMessage: ishortMessage,
        detailedMessage: idetailMessage,
        dialogType: idialogType
    });
}
function showConfirmationMessage(ititle, ishortMessage, idetailMessage, ishowTitleClose, yesptr, noptr, cancelptr, buttonType) {
    var ibuttons;
    if (buttonType == "30") {
        ibuttons = getYesNoForCancelConfirmation(yesptr, noptr, cancelptr)
    } else {
        ibuttons = getYesNoForConfirmation(yesptr, noptr)
    }
    $('body').inforMessageDialog({
        title: ititle,
        shortMessage: ishortMessage,
        detailedMessage: idetailMessage,
        dialogType: "Confirmation",
        showTitleClose: ishowTitleClose,
        buttons: ibuttons
    });
}

function getYesNoForConfirmation(yesptr, noptr) {
    return [{
        text: Globalize.localize("Yes"),
        click: function () { if (typeof yesptr == "function") { yesptr(); } $(this).inforDialog("close"); },
        isDefault: true
    }, {
        text: Globalize.localize("No"),
        click: function () { if (typeof noptr == "function") { noptr(); } $(this).inforDialog("close"); }
    }];
}

function getYesNoForCancelConfirmation(yesptr, noptr, cancelptr) {
    return [{
        text: Globalize.localize("Yes"),
        click: function () { if (typeof yesptr == "function") { yesptr(); } $(this).inforDialog("close"); },
        isDefault: true
    }, {
        text: Globalize.localize("No"),
        click: function () { if (typeof noptr == "function") { noptr(); } $(this).inforDialog("close"); }
    }, {
        text: Globalize.localize("Cancel"),
        click: function () { if (typeof cancelptr == "function") { cancelptr(); } $(this).inforDialog("close"); }
    }];
}

function loadAndDisplayDialog(url, data, divId, iTitle, iWidth, iHeight, closeptr, okptr, cancelptr) {

    showDialog(divId, iTitle, "General", iWidth, iHeight, true, closeptr, okptr, cancelptr);
}
function showDialog(divId, iTitle, iDialogType, iWidth, iHeight, imodal, closeptr, okptr, cancelptr) {
    $('#' + divId).inforMessageDialog({
        title: iTitle,
        dialogType: iDialogType,
        width: iWidth,
        height: iHeight,
        modal: imodal,
        close: function (event, ui) {
            if (typeof closeptr == "function") closeptr();
            $('#' + divId).remove();
        },
        buttons: [{
            id: 'okButton',
            text: Globalize.localize("Ok"),
            click: function () {
                $("#okButton").addClass("active");
                if (typeof okptr == "function") okptr();
                $(this).inforDialog("close");
            },
            isDefault: true
        }, {
            text: Globalize.localize("Cancel"),
            click: function () {
                if (typeof cancelptr == "function") cancelptr();
                $(this).inforDialog("close");
            }
        }]
    });

}

function OnUploadframeload(currentFrame) {
}

function unloadview() {
}

function getFullUrl(url) {
    /* //var href = location.href; */

    href = $("#hdnBaseUrl").val();
    if (href.substring(href.length - 1) != "/") {
        href = href + "/";
    }
    href = href + url;
    var t = new Date;
    t = t.getTime();
    /* //href = href + "/t=" + t;     */
    return href;
}

function removejscssfile(filename, filetype) {
    var targetelement = (filetype == "js") ? "script" : (filetype == "css") ? "link" : "none" //determine element type to create nodelist from
    var targetattr = (filetype == "js") ? "src" : (filetype == "css") ? "href" : "none" //determine corresponding attribute to test for
    var allsuspects = document.getElementsByTagName(targetelement)
    for (var i = allsuspects.length; i >= 0; i--) { //search backwards within nodelist for matching elements to remove
        if (allsuspects[i] && allsuspects[i].getAttribute(targetattr) != null && allsuspects[i].getAttribute(targetattr).indexOf(filename) != -1)
            allsuspects[i].parentNode.removeChild(allsuspects[i]) //remove element by calling parentNode.removeChild()
    }
}

function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

var dropDownEnable = function (elem) {

    if (elem == null)
        return;

    if (elem.next().hasClass("disabled"))
        $(elem).enable();
};


var dropDownDisable = function (elem) {
    if (elem == null)
        return;

    $(elem).disable();

};

function defineCodeFromName(xName) {
    var t, VocStr, i;
    VocStr = "AaEeIiOoUuYyÅåÄäÖö";
    var j = 1;
    for (i = 0; i <= xName.length; i++) {
        if (xName.substring(i, j) == " ") {
            xName = xName.substring(0, i) + xName.substring(i + 1, i + 2).toUpperCase() + xName.substring(i + 2, 2000);
        }
        j = j + 1;
    }
    t = xName.replace(" ", "");
    t = t.replace(/e/g, "");
    t = t.replace(/a/g, "");
    t = t.replace(/i/g, "");
    t = t.replace(/o/g, "");
    t = t.replace(/u/g, "");
    t = t.replace(/y/g, "");
    t = t.replace(/å/g, "");
    t = t.replace(/ä/g, "");
    t = t.replace(/ö/g, "");
    t = t.replace(/Å/g, "");
    t = t.replace(/Ä/g, "");
    if (VocStr.indexOf(Left(xName, 1)) != -1) {
        t = Left(xName, 1) + t.substring(2, 2000);
    }
    return Left(t, 45);
}

var _globalGrid = null;
function _clearGrid() {
    if (_globalGrid != null) {
        _globalGrid.destroy()
    }
}
function _setGrid(gridObj) {
    _globalGrid = gridObj;
}
function buildTree(resultData, divID) {
    var treeData = [];
    for (var i = 0; i < resultData.length; i++) {
        var treeNode = {
            "data": {
                "title": resultData[i].title,
                "attr": { "id": resultData[i].id }
            },
            "metadata": { "id": resultData[i].id },
            "state": "open jstree-undetermined",
        }

        var childrens = resultData[i].children;
        var childData = []
        for (var j = 0; j < childrens.length; j++) {
            var childNode = {
                "data": {
                    "title": childrens[j].title,
                    "attr": { "id": childrens[j].id },
                },
                "metadata": { "id": childrens[j].id },
                "state": "closed jstree-undetermined",
            }

            //Look for Third Level Node.
            //This logic has to be modified to be more generic to loop any node levels.
            var schildrens = childrens[j].children;
            var schildData = []
            for (var k = 0; k < schildrens.length; k++) {
                var schildNode = {
                    "data": {
                        "title": schildrens[k].title,
                        "attr": { "id": schildrens[k].id },
                    },
                    "metadata": { "id": schildrens[k].id },
                    "state": "closed jstree-undetermined",
                }
                schildData.push(schildNode);
            }
            $.extend(childNode, { "children": schildData });
            //End of Third Level
            childData.push(childNode);
        }
        $.extend(treeNode, { "children": childData });
        treeData.push(treeNode);
    }
    var treeDataEx = {
        "data": treeData
    }
    $("#" + divID).inforTree({
        "json_data": treeDataEx,
        "plugins": ["json_data"]
    })
	.bind("select_node.jstree", function (e, data) {
	    //Get the Node Id....
	    var treeId = $.data(data.rslt.obj[0], "id");

	});
}

function getRandomColor() {
    alert(1);
    var letters = '0123456789ABCDEF'.split('');
    var color = '#';
    for (var i = 0; i < 6; i++ ) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}