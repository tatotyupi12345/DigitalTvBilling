var statsusArray = [];
$(function () {
    $(".datepickers").datePickers({ prefix: "picker" });
    $(".selectpicker").selectpicker();
    $('#picker_picker_nav').on("click", function (e) {
        //alert(this.value);
        //alert($("#drp_filter").val());
        var attrval = $(this).attr("href");
        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var status = statsusArray.sort();// $('#filter_status').val();
        var filter = $('#txt_filter').val();
        var drp_filter = $('#drp_filter').val();
        var j_checked = $('#j_checked').is(':checked');
        //CustomerAttach/?dt_from=26072017&dt_to=26072017
        var new_attr = /*($(this).attr("href").indexOf('Accountant') != -1?'Accountant/':'')+*/ "?dt_from=" + dtfrom + "&dt_to=" + dtto + "&status=" + status + "&name=" + filter + "&drp_filter=" + drp_filter + "&j_checked=" + j_checked;
        $(this).attr("href", new_attr);
        //filterAbonents($('#filter_type').val(), $("#drp_filter").val(), 1);
    })
    $('#remove_filter').on("click", function () {
        location.href = location.pathname;
    });
    $(".change_card_status").on("click", function (e) {
        e.preventDefault();
        $.post("/Juridical/GetStatusInfo/",
            {
                card_id: $(this).closest("tr").attr("data-id")// $("#Customer_Id").val()
            },
            function (data) {
                var content = $(data);
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                content.remove();
                    //location.href = location.href;
                });
                initContent(content);
            }, "html");
    });
    //$(".dwoload_doc_abonent").on("click", function (e) {
    //    e.preventDefault();
    //    $.post("/Juridical/DocsJuridicalInfos/",
    //        {
    //            card_id: $(this).closest("tr").attr("data-id")// $("#Customer_Id").val()
    //        },
    //        function (data) {
    //            var content = $(data);
    //            content.modal("show");
    //            content.on('hidden.bs.modal', function () {
    //                content.remove();
    //                //location.href = location.href;
    //            });
    //            infoDocs(content);
    //        }, "html");
    //});
    //$(".dwoload_doc_abonent").on("click", function (e) {
    //    e.preventDefault();
    //    $.post("/Juridical/DowloadDocs/",
    //        {
    //            code: $(this).closest("tr").attr("data-id"), lastname_name: $(this).closest("tr").attr("data-value")// $("#Customer_Id").val()
    //        },
    //        function (data) {
    //            var content = $(data);
    //            content.modal("show");
    //            content.on('hidden.bs.modal', function () {
    //                content.remove();
    //                //location.href = location.href;
    //            });
    //            infoDocs(content);
    //        }, "html");
    //});
    $(".dwoload_doc_abonent").on("click", function (e) {
        e.preventDefault();
        $.post("/Juridical/DowloadDocs/",
            {
                filePath: $(this).closest("tr").attr("data-value")// $("#Customer_Id").val()
            },
            function (data) {
                var content = $(data);
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                    //location.href = location.href;
                });
                infoDocs(content);
            }, "html");
    });
    $("#filter_status").on("changed.bs.select", function (e, clickedIndex, newValue, oldValue) {
        if (newValue == true) {
            if (clickedIndex == 11) {
                statsusArray.push(-1);
            }
            else {
                statsusArray.push(clickedIndex);
            }
        }
        else {
            statsusArray = statsusArray.filter(function (element) {
                return element != clickedIndex;
            });
        }
    });

    var timer, value;
    //$('#txt_filter').bind('keyup', function () {
    //    clearTimeout(timer);
    //    var str = $(this).val();
    //    if (str.length > 2 && value != str)
    //    {
    //        timer = setTimeout(function ()
    //        {
    //            value = str;
    //            var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
    //            var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
    //            var status = $('#filter_status').val();
    //            location.href = "Juridical?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&status=" + status;
    //            //console.log('Perform search for: '+location.href);
    //            //alert(location.href);
    //            //if (location.href.indexOf('?') > -1)
    //            //    location.href = location.href + "&name=" + value;
    //            //else
    //            //    location.href = location.href + "?name=" + value;
    //        }, 1000);
    //    }
    //});

    $('#txt_filter').keypress(function (e) {
        var key = e.which;
        var str = $(this).val();
        if (key == 13)  // the enter key code
        {
            value = str;
            //if (value == "" || value.length < 3)
            //    return;
            //var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
            //var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
            //var status = $('#filter_status').val();
            //var drp_filter = $('#drp_filter').val();
            //location.href = "Juridical?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&status=" + status + "&drp_filter=" + drp_filter;
            filter(value, $('#drp_filter').val());
            
        }
    });

    $('#drp_filter').keypress(function (e) {
        var key = e.which;
        var str = $(this).val();
        if (key == 13)  // the enter key code
        {
            //value = str;
            //if (value == "" || value.length < 3)
            //    return;
            //var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
            //var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
            //var status = $('#filter_status').val();
            //var drp_filter = $('#drp_filter').val();
            //location.href = "Juridical?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&status=" + status + "&drp_filter=" + drp_filter;
            filter($('#txt_filter').val(), str);

        }
    });

    $(".packages").on("click", function (e)
    {
        e.preventDefault();
        //if ($(e.target).prop("tagName") != "TD")
        //    return;
        var cardid = $(this).attr("data-id");//$("#abonents_body > tr").attr("data-id");
        getCardInfo(cardid);
    });
    $(".bort").on("click", function (e) {
        e.preventDefault();
        
        var cardid = $(this).attr("data-id");// $("#abonents_body > tr").attr("data-id");
        bortInfo(cardid);
    });

});
function infoDocs(content) {
    content.find("#dowloaddoc").on("click", function () {
        $.post("/Juridical/DowloadDocs", { filePath: $(this).attr("data-value") }, function () {


        });
    });
}

function bortInfo(card_id) {
    $.post("/Juridical/_Logging/", { card_id: card_id }, function (data) {
        var content = $(data);
        content.modal("show");
        content.on('hidden.bs.modal', function () {
            content.remove();
        });

    },"html");
}
function getCardInfo(cardid, dtfrom, dtto)
{
    $.post("/Abonent/GetCardInfo/",
            {
                dt_from:dtfrom,
                dt_to: dtto,
                card_id: cardid,
                cust_id: 0,
                detaled: false
            },
            function (data) {
                var content = $(data);
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                });

                initCardInfoContent(content, cardid);
            }, "html");
}

function initCardInfoContent(content, cardid)
{
    content.find(".datepickers").datePickers({ prefix: "picker" });
    content.find('#picker_picker_nav').on("click", function (e) {
        var dtfrom = content.find('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = content.find('#picker_picker_to').val().replace(/\//gi, '');
        e.preventDefault();
        content.modal("hide");
        getCardInfo(cardid, dtfrom, dtto);
    });
}

function filter(value, filter)
{
    if (value == "" || value.length < 3)
        return;
    var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
    var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
    var status = statsusArray.sort(); //$('#filter_status').val();
    var drp_filter = filter;
    var j_checked = $('#j_checked').is(':checked');
    location.href = "Juridical?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&status=" + status + "&drp_filter=" + drp_filter + "&j_checked" + j_checked;
}

function recordApprove(id) {

    if (confirm("ნამდვილად გსურთ დადასტურება?") == true) {
        //txt = "You pressed OK!";
    } else {
        return;
    }

    $.post("/Juridical/RecordApprove", { id: id }, function (data) {
        if (data === 1) {
            $("[data-id='" + id + "']").css('background-color', '#a4ff7e');
            $("#data-approve-" + id).html('<span class="bg-success">OK</span>');
            //location.href = location.href;
        }
    }, "json");

}

function initContent(content) {
    content.find('#save').on("click", function (e) {
        e.preventDefault();
        var id = content.find("#card_").attr("data-val");//$(this).closest("tr").attr("data-id"),//
        var _statusArray = [];
        content.find("#_each > tr").each(function (value) {
            if (content.find("#manual_" + value).prop("checked") == true) {
                if (value == 11) {
                    _statusArray.push(-1);
                }
                else {
                    _statusArray.push(value);
                }
            }
        });
        _statusArray.sort();
        var call_status = content.find("#card_call_status").val();
        var desc = content.find("#Card_Desc").val();
        var clasname = "";
        $.post("/Juridical/SaveStatus",
            {
                id: id,
                statusArray: _statusArray,
                _status: _statusArray
            },
            function (Status, ID) {
                if (Status.ID == 1) {
                    alert("სტატუსი შეიცვალა წარმატებით.");
                    
                    switch (parseInt(call_status)) {
                        case 0:

                            clasname = "Delivered";
                            break;

                        case 1:
                            clasname = "Passed";
                            break;

                        case 2:
                            clasname = "FixableProblematic";
                            break;

                        case 3:
                            clasname = "PassedProblematic";
                            break;

                        case 4:
                            clasname = "NotFixableProblem";
                            break;

                        case 5:
                            clasname = "Stopped";
                            break;

                        default:
                            break;
                    }
                    var ind = 0;
                    $("[data-id=" + id + "]").find('#status').html("");
                    Status.Status.forEach(function (status) {
                        if ((status == 6 || status == 7 || status == 8 || status == 9) && ind==0) {
                            $("[data-id=" + id + "]").find('#status').append("პრობლემური" + "<hr/>");
                            ind = 1;
                        }
                        switch (status) {
                            case -1:
                                $("[data-id=" + id + "]").find('#status').append("<small>სტატუსის გარეშე</small>" +"<hr/>");
                                break;
                            case 0:
                                $("[data-id=" + id + "]").find('#status').append("<small>ჩაბარებული</small>" + "<hr/>");
                                break;
                            case 1:
                                $("[data-id=" + id + "]").find('#status').append("<small>გავლილი</small>" + "<hr/>");
                                break;
                            case 2:
                                $("[data-id=" + id + "]").find('#status').append("<small>პრობლემური გამოსწორებადი</small>" + "<hr/>");
                                break;
                            case 3:
                                $("[data-id=" + id + "]").find('#status').append("<small>პრობლემური გავლილი</small>" + "<hr/>");
                                break;
                            case 4:
                                $("[data-id=" + id + "]").find('#status').append("<small>პრობლემური გამოუსწორებელი</small>" + "<hr/>");
                                break;
                            case 5:
                                $("[data-id=" + id + "]").find('#status').append("<small>გაუქმებული</small>" + "<hr/>");
                                break;
                            case 10:
                                $("[data-id=" + id + "]").find('#status').append("<small>ატვირთული</small>");
                                break;

                        }
                        
                    });
                   
                   
                    $("[data-id=" + id + "]").attr("class", clasname);
                    $("[data-id=" + id + "]").find('.desc').text(desc);
                    content.modal("hide");
                }
                else {
                    alert("შეცდომა: სტატუსი ვერ შეიცვალა!");

                }
               
            }, "json");
        
    });
  
}
