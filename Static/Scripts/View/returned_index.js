var drp_filter;
var txt_filter;
var paging;
var abonents_body;
$(function () {
    
    $('#picker_picker_nav').on("click", function (e) {
        var attrval = $(this).attr("href");
        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        //var status = $('#filter_status').val();
        var filter = $('#txt_filter').val();
        var drp_filter = $('#drp_filter').val();
        //CustomerAttach/?dt_from=26072017&dt_to=26072017
        var new_attr = "?name=" + filter + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&drp_filter=" + drp_filter;
        $(this).attr("href", new_attr);

    });

    var timer, value;


    $('#txt_filter').keypress(function (e) {
        var key = e.which;
        var str = $(this).val();
        if (key == 13)  // the enter key code
        {
            value = str;

            filter(value, $('#drp_filter').val());

        }
    });

    $('#drp_filter').keypress(function (e) {
        var key = e.which;
        var str = $(this).val();
        if (key == 13)  // the enter key code
        {

            filter($('#txt_filter').val(), str);

        }
    });
    $('#remove_filter').on("click", function () {
        location.href = location.pathname;
    });
    ////
    $('.returnedAttachment').on("click", function (e) {
        var returned_id = $(this).attr('id');

        $.post("/Accessory/GetAccessoryInfo/",
            {
                ReturnedCardsID: returned_id
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
    var returned_id;
    $('.changeBort').on("click", function (e) {
        returned_id = $(this).attr('id');
        $.post("/Returned/GetReturnedBort/",
            {
                card_id: returned_id
            },

            function (data) {
                var content = $(data);
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                    //location.href = location.href;
                });
                inContent(content);
            }, "html");
    });

    function initContent(content) {
        content.find(':checkbox').checkboxpicker();
        content.find('#save').on("click", function (e) {
            e.preventDefault();

            var attachmentVals = [];
            var i = 0;
            var code = content.find("#abonent_id").val();
            var data = [];
            content.find(":checkbox").each(function (index) {
                var chkID = $(this).val();
                data.push({ check: $(this).is(":checked"), AccesoryID: chkID });
                //alert(chkID);
            });

            $.post("/Accessory/UpdateEntry",
                {
                    data: data,
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
    
    function inContent(content) {
        content.find(".selectpicker").selectpicker();
        content.find('#return_save').on("click", function () {
            var select = document.getElementById("select");
            var diler = select.options[select.selectedIndex].value;
            var search = document.getElementById("search");
            var card_id = returned_id;
            $.post("/Returned/UpdateEntry",
                {
                    bortID: diler,
                    card_id: card_id

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

    $('.returne_card_cancles_name').on('click', function () {
        var returned_card_id =$(this).closest('tr').attr('data-id');
        $.post('Returned/GetReturnedCardCancle', { returned_card_id: $(this).closest('tr').attr('data-id') }, function (data) {

            var __content = $(data);
            __content.find(':checkbox').checkboxpicker();
            init_cardcancel(__content);
            __content.modal("show");
            __content.on('hidden.bs.modal', function () {
                __content.remove();

            });
        }, "html");

    });
});

function record(card_id) {
    alert(card_id);
    if (confirm("ნამდვილად გსურთ დადასტურება?") == true) {

    } else {
        return;
    }

    $.post("/Returned/RecordApprove", { card_id: card_id }, function (data) {
        if (data === 1) {
            $("[data-id='" + card_id + "']").css('background-color', '#a4ff7e');
            $("#data-approve-" + card_id).html('<span class="bg-success">OK</span>');
            //location.href = location.href;
        }
        else
        {
            alert("ვერიფიკაცია ვერ მოხერხდა!");
        }
    }, "json");

}
function deleteID(id){
    if (confirm("ნამდვილად გსურთ წაშლა?") == true) {

    } else {
        return;
    }
    $.post("/Returned/DeleteCard", { id: id }, function (data) {
        if (data == 0) {
            alert("გაუქმება წაშალა არ მოხერხდა");
        }
        else {
            alert("გაუქმება წარმატებით წაიშალა");
            location.href = location.href;
        }

    });
}

function filter(value, filter) {

    if (value == "" || value.length < 3) {
        location.href = "?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&drp_filter=" + drp_filter;
        return;
    }
    var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
    var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
    var status = $('#filter_status').val();
    var drp_filter = filter;
    location.href = "?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto +  "&drp_filter=" + drp_filter;
}