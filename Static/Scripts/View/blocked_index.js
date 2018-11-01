

$(function () {
    $(".datepickers").datePickers({ prefix: "picker" });
    $('#picker_picker_nav').on("click", function (e) {
        //alert(this.value);
        //alert($("#drp_filter").val());
        var attrval = $(this).attr("href");
        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var status = $('#filter_status').val();
        var filter = $('#txt_filter').val();
        var drp_filter = $('#drp_filter').val();
        //CustomerAttach/?dt_from=26072017&dt_to=26072017
        var new_attr = /*($(this).attr("href").indexOf('Accountant') != -1?'Accountant/':'')+*/ "?dt_from=" + dtfrom + "&dt_to=" + dtto + "&status=" + status + "&name=" + filter + "&drp_filter=" + drp_filter + "&Discontinued=" + 1;
        $(this).attr("href", new_attr);
        //filterAbonents($('#filter_type').val(), $("#drp_filter").val(), 1);
    })

    $(".change_card_status").on("click", function (e) {
        e.preventDefault();
        $.post("/BlockedCards/GetStatusInfo/",
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

    //$('#txt_filter').keypress(function (e) {
    //    var key = e.which;
    //    var str = $(this).val();
    //    if (key == 13)  // the enter key code
    //    {
    //        value = str;
    //        //if (value == "" || value.length < 3)
    //        //    return;
    //        //var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
    //        //var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
    //        //var status = $('#filter_status').val();
    //        //var drp_filter = $('#drp_filter').val();
    //        //location.href = "Juridical?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&status=" + status + "&drp_filter=" + drp_filter;
    //        filter(value, $('#drp_filter').val());

    //    }
    //});


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


});

function filter(value, filter) {
    if (value == "" || value.length < 3)
        return;
    var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
    var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
    var status = $('#filter_status').val();
    var drp_filter = filter;
    location.href = "Juridical?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&status=" + status + "&drp_filter=" + drp_filter;
}
function Discontinued(value, filter) {
    //if (value == "" || value.length < 3)
    //    return;
    var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
    var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
    var status = $('#filter_status').val();
    var drp_filter = filter;
    location.href = "Juridical?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&status=" + status + "&drp_filter=" + drp_filter + "&Discontinued=1";
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
    content.find('#block_save').on("click", function (e) {
        e.preventDefault();
        var id = content.find("#card_").attr("data-val");//$(this).closest("tr").attr("data-id"),//
        var call_status = content.find("#card_call_status").val();
        var desc = content.find("#Card_Desc").val();
        var clasname = "";
        $.post("/BlockedCards/SaveStatus",
            {
                id: id,
                call_status: call_status,
                desc: desc
            },
            function (BlockStatus, ID) {
                if (BlockStatus.ID == 1) {
                    alert("სტატუსი შეიცვალა წარმატებით.");

                    //switch (parseInt(call_status)) {
                    //    case 0:

                    //        clasname = "Delivered";
                    //        break;

                    //    case 1:
                    //        clasname = "Passed";
                    //        break;

                    //    case 2:
                    //        clasname = "FixableProblematic";
                    //        break;

                    //    case 3:
                    //        clasname = "PassedProblematic";
                    //        break;

                    //    case 4:
                    //        clasname = "NotFixableProblem";
                    //        break;

                    //    case 5:
                    //        clasname = "Stopped";
                    //        break;

                    //    default:
                    //        break;
                    //}
                    switch (parseInt(call_status)) {
                        case 0:

                            clasname = "";
                            break;

                        case 1:
                            clasname = "goingToPay";
                            break;

                        case 2:
                            clasname = "goingToCancel";
                            break;

                        case 3:
                            clasname = "technicalProblem";
                            break;

                        case 4:
                            clasname = "unConnected";
                            break;

                        case 5:
                            clasname = "owner";
                            break;

                        default:
                            break;
                    }
                    $("[data-id=" + id + "]").find('#BlockStatus').text(BlockStatus.BlockStatus);
                    $("[data-id=" + id + "]").attr("class", clasname);
                    $("[data-id=" + id + "]").find('.desc').text(desc);
                    //content.remove();
                    //updateColor(content);
                }
                else {
                    alert("შეცდომა: სტატუსი ვერ შეიცვალა!");

                }
            }, "json");
    });
}