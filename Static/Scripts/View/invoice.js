var drp_filter;
var txt_filter;

var timer = null;
var invoices_body;
var index_drp_filter;
var index_txt_filter;
var paging;
$(function () {
    //$(".datepickers").datePickers({ prefix: "picker" });

    invoices_body = $("#invoices_body");

    index_txt_filter = $("#index_txt_filter");
    index_drp_filter = $("#index_drp_filter");
    paging = $("#paging");

    index_drp_filter.on("change", function (e) {
        index_txt_filter.focus();
    });

    //$('#edit_abonent').click(function () {
    //    alert($(this).data("id"));

    //});
    index_txt_filter.keyup(function (e) {
        filter($(this).val(), 1);
        $(".pagination-container").remove();
    });

    paging.on("click", "a", function (e) {
        if (paging.data("mode") === "ajax") {
            e.preventDefault();
            e.stopPropagation();
            filter(index_txt_filter.val(), $(this).attr("href"));
            filter(title.data("sign") === "text" ? index_txt_filter.val() : title.next().find("option:selected").val(), $(this).attr("href"));
        }
    });

    $("#showModal").on("click", function (e) {
        e.preventDefault();
        $.get("/Utils/NewInvoice", function (data) {
            var content = showModal(data);
            drp_filter = content.find("#find_by");
            var txt_filter = content.find("#payment_find");

            drp_filter.on("change", function () {
                txt_filter.focus();
            });

            content.find("#find").on("click", function (e) {
                e.preventDefault();
                if ($.trim(txt_filter.val()) === '')
                    return;
                $.get("/api/utils/getJuridicalCards", { type: drp_filter.find("option:selected").val(), s: txt_filter.val() }, function (res) {
                    if (res != null)
                        content.find("#abonent_area").html('<div class="alert alert-success" role="alert">' + res.abonent_name + '</div>');
                    else
                        content.find("#abonent_area").html('<div class="alert alert-danger" role="alert">აბონენტი ვერ მოიძებნა</div>');

                    var str = '';
                    $.each(res.cards,function (i, card) {
                        str += '<div class="checkbox"><label><input type="checkbox" name="Cards[' + i + ']" id="Cards[' + i + ']" value="' + card.id + '">' + card.name + ' - ' + card.finishDate.ReplaceAll('T',' ') + '</label></div>';
                    });
                    content.find("#cards").html(str);

                }, "json");

            });

        });
    });

});

function onSuccessNewInvoice(res) {
    if (res === 1)
        return location.href = "/Utils/Invoices";
    else
        alert("შენახვა ვერ შესრულდა!");
}

function filter(letter, page) {
    if (page === undefined) return;
    if (timer) {
        clearTimeout(timer);
    }

    timer = setTimeout(function () {
        var col = index_drp_filter.find("option:selected").val();
        if (col === "i.num" || letter.length > 1) {
            $.post("/Utils/FilterInvoices", {
                letter: letter, column: col, page: page,
                dt_from: $("#picker_picker_from").val().ReplaceAll("/", ""),
                dt_to: $("#picker_picker_to").val().ReplaceAll("/", "")
            }, function (data) {
                var str = '';
                $.each(data.Invoices, function (i, val) {
                    str += '<tr data-id="' + val.Id + '"><td>' + val.Num + '</td>' +
                        '<td>' + parseJsonDate(val.Tdate).toDateString() + '</td><td>' + val.AbonentName + '</td>' +
                        '<td>' + val.AbonentNum + '</td><td style="text-align:center;"><a target="_blank" href="ftp://' + data.FilePath + val.FileName + '"><span class="glyphicon glyphicon-print" aria-hidden="true"></span></a></td></tr>';
                });
                invoices_body.html(str);
                paging.data("mode", "ajax").html(data.Paging);
            }, "json");
        }
    }, 300, letter);
}

function activeabonent(id) {
    //alert(id);
    if (confirm("ნამდვილად გსურთ გაქტიურება?") == true) {

    } else {
        return;
    }
    $.post("/Utils/AbonentActive", { card_id: id }, function (data) {
        if (data == 0) {
            alert("ბარათი ვერ გაქტიურდა!");
           
        }
        else {
            alert("ბარათი გაქტიურდა!")
            return location.href = "/Utils/Invoices";
        }

    });
}