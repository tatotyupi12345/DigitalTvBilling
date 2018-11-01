var timer = 2806;
$(function () {

    $("#drp_filter").change(function () {
        var str = $(this).text();
        var val = $(this).val();

        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        //CustomerAttach/?dt_from=26072017&dt_to=26072017
     })
     .change();

    $('#picker_picker_nav').on("click", function (e) {
        var attrval = $(this).attr("href");
        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var filter_id = $("#drp_filter").val();
        var filter_user_id = $("#user_filter").val();
        //CustomerAttach/?dt_from=26072017&dt_to=26072017

        var new_attr = "?dt_from=" + dtfrom + "&dt_to=" + dtto + "&attach_filter_id=" + filter_id + "&user_id=" + filter_user_id;
        $(this).attr("href", new_attr);

        //var addParam = "";
        //if (attrval.indexOf('page=') > -1) {
        //    attrval = attrval.replace('page=', '');
        //}
        //if (attrval.indexOf('?') > -1) {

        //    if (attrval.indexOf('filter_status') > -1) {
        //        addParam = "filter_status=" + $("#status-filter").val();
        //        attrval = attrval.replace(attrval.substr(attrval.indexOf('filter_status')), '');
        //    }
        //    else
        //        addParam = "&filter_status=" + $("#status-filter").val();
        //}
        //else {
        //    addParam = "/?filter_status=" + $("#status-filter").val();
        //}

        //$(this).attr("href", attrval + addParam);
        //alert();
        //var abonent = getParameterByName('abonent');
        //var url
    });

    $("#abonents_body").on("click", "tr", function (e) {
        e.preventDefault();
        if ($(e.target).prop("tagName") != "TD")
            return;
        var customer_attach_id = $(this).closest("tr").attr("data-customerid");
        var attach_diller_id = $(this).closest("tr").attr("data-id");
        $.post("/CustomerAttach/GetAttachsInfo/",
            {
                customer_attach_id: customer_attach_id,
                attach_diller_id: attach_diller_id
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

    $("#new_attachment").on("click", function (e) {
        e.preventDefault();
        var customer_attach_id = $(this).closest("tr").attr("data-id");
        $.post("/CustomerAttach/NewAttach/",

            function (data) {
                var content = $(data);
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                    //location.href = location.href;
                });
                initNewAttachContent(content);
            }, "html");
    });

});

function recordApprove(id, diler_id) {

    if (confirm("ნამდვილად გსურთ დადასტურება?") == true) {
        //txt = "You pressed OK!";
    } else {
        return;
    }

    $.post("/CustomerAttach/RecordApprove", { id: id, diler_id: diler_id }, function (data) {
        if (data === 1) {
            $("[data-id='" + diler_id + "'][data-customerid='" + id + "']").css('background-color', '#a4ff7e');
            $("[data-id='" + diler_id + "'][data-approve='" + id + "']").html('<span class="bg-success">OK</span>');
            //location.href = location.href;
        }
        else {
            alert("ჩანაწერის დადასტურება ვერ მოხერხდა!");
        }
    }, "json");

}


function initContent(content) {
    content.find('#save').on("click", function (e) {
        e.preventDefault();
        //var id = content.find("#card_").attr("data-val");//$(this).closest("tr").attr("data-id"),//
        //var call_status = content.find("#card_call_status").val();
        //var desc = content.find("#Card_Desc").val();
        //var clasname = "";
        var attachmentVals = [];
        var i = 0;
        var code = content.find("#customer_code").val();
        var customer_id = content.find("#customer_id").val();
        var diller_id = content.find("#diller_id").val();
        while (typeof(content.find("#attachments_" + i).attr('data-val')) != 'undefined')
        {
            var count_ = content.find("#attachments_" + i).val();
            var ID_ = content.find("#attachments_" + i).attr('data-val');

            attachmentVals.push({ id: ID_, count: count_ });
            i++;
            //attachments[@i].Value
        }

        $.post("/CustomerAttach/UpdateEntry",
            {
                attachmentVals: attachmentVals,
                code: code,
                diller_id: diller_id,
                customer_id: customer_id
            },
            function (data)
            {
                if (data === 1)
                {
                    location.href = location.href;
                }
                else
                {
                    alert("შეცდომა: მონაცემები ვერ შეიცვალა!");
                }
            }, "json");
    });
}
var loader = null;
function initNewAttachContent(content) {
    //content.find(':checkbox').checkboxpicker();
    ////$(':checkbox').checkboxpicker();
    //content.find(".dilersWrapper_edit").hide();
    //content.find('#isFromDiler').checkboxpicker();
    //content.find('#isFromDiler').on('change', function () {
    //    if (content.find(this).prop("checked") == true) {
    //        content.find(".diler_filter").show(200);
    //        content.find("#isFromDiler_").val('true');
    //    }
    //    else {
    //        content.find(".dilersWrapper_edit").hide(200);
    //        content.find(".diler_filter").hide(200);
    //        content.find("#dilersWrapper_edit").empty();
    //        content.find("#diler_code_word").val('');

    //        content.find("#isFromDiler_").val('false');
    //    }
    //});
    // loader = content.find("#dilersWrapper_edit");
    //content.find("#diler_code_word").keyup(function () {
    //    //alert($(this).val());
    //    filterDilers_edit(content.find(this).val(), content);

    //});
    content.find('#save').on("click", function (e) {
        e.preventDefault();
        //var id = content.find("#card_").attr("data-val");//$(this).closest("tr").attr("data-id"),//
        //var call_status = content.find("#card_call_status").val();
        //var desc = content.find("#Card_Desc").val();
        var dilerchecked = null;
        if (content.find('#isFromDiler').prop("checked") == true) {
            dilerchecked = content.find("input[name='dilerCards.diler_id']:checked").val();
        }
        else {
            dilerchecked = 0;
        }

        var attachmentVals = [];
        var i = 0;
        var code = content.find("#customer_code").val();
        while (typeof (content.find("#attachments_" + i).attr('data-val')) != 'undefined') {
            var count_ = content.find("#attachments_" + i).val();
            var ID_ = content.find("#attachments_" + i).attr('data-val');

            attachmentVals.push({ id: ID_, count: count_ });
            i++;
            //attachments[@i].Value
        }

        $.post("/CustomerAttach/NewEntry",
            {
                attachmentVals: attachmentVals,
                code: code
            },
            function (data) {
                if (data === 1) {
                    location.href = location.href;
                }
                else {
                    alert("შეცდომა: მონაცემები ვერ შეიცვალა!");
                }
            }, "json");
    });
}

//var content;
//function filterDilers_edit(letters,contentFind) {
//    if (timer) {
//        clearTimeout(timer);
//    }
//    if (letters.length >= 3) {
//        contentFind.find("#dilersWrapper_edit").empty();
//        loader.gSpinner("hide");
//        loader.gSpinner({ scale: .3 });
     
//        timer = setTimeout(function () {

//            $.get("/Books/getUserDetailsAttachment", { filter: letters }, function (data, status) {
//                //alert("Data: " + data + "\nStatus: " + status);
//                //$loader.gSpinner("hide");
//                content = $(data);
//                contentFind.find("#dilersWrapper_edit").html(content);
//                content.find(".selectpicker").selectpicker();

//                var id__ = content.find("input[name='dilerCards.diler_id']:checked").val();
//                var towerids = content.find("#diler_" + id__ + "_cardid").val();
//                content.find("#diler_92_cardid").on("changed.bs.select", function (e, clickedIndex, newValue, oldValue) {
//                    alert(0);
//                });
//                initContent(content);

//            });
//        }, 300, letters);
//    }
//    else {
//        contentFind.find("#dilersWrapper_edit").empty();
//        // $loader.gSpinner("hide");
//    }

//}