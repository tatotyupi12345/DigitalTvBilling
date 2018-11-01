var timer = null;
var abonents_body;
var drp_filter;
var txt_filter;
var paging;
$(function () {
    abonents_body = $("#abonents_body");

    abonents_body.on("click", "tr", function (e) {
        e.preventDefault();
        if (e.target.id == "edit_abonent") {
            if ($(this).data("id") === undefined)
                return;
            window.location.href = "/Abonent/Edit/" + $(this).data("id") + "/" + $(this).data("card");
        }
    });

    txt_filter = $("#txt_filter");
    drp_filter = $("#drp_filter");
    paging = $("#paging");
    var title = $("#filter_title");

    drp_filter.on("change", function (e) {
        txt_filter.focus();
        if ($(this).find("option:selected").val() == "c.type") {
            title.data("sign", "select").next().remove();
            title.after('<select class="form-control" id="card_status" onchange="cardstypefilter(this)">' +
                '<option value="-1"></option>' +
                '<option value="0">ფიზიკური</option>' +
                '<option value="1">იურდიული</option></select>');
            return;
        }
        if ($(this).find("option:selected").val() == "cr.status") {
            title.data("sign", "select").next().remove();
            title.after('<select class="form-control" id="card_status" onchange="cardStatusChange(this)">' +
                '<option value="-1"></option>' +
                '<option value="0">აქტიური</option>' +
                '<option value="1">გათიშული</option>' +
                '<option value="2">დაპაუზებული</option>' +
                '<option value="3">მონტაჟი</option>' +
                '<option value="4">გაუქმებული</option>' +
                '<option value="5">დაბლოკილი</option>' +
                '<option value="7">იჯარა</option>'+
                '<option value="8">შეწყვეტილი</option></select > ');
            return;
        } else if ($(this).find("option:selected").val() == "cr.tower_id") {
            $.post("/Abonent/GetTowers", {}, function (data) {
                title.data("sign", "select").next().remove();
                var str = '<select class="form-control input-sm" id="tower_combo" onchange="towersChange(this)"><option></option>';
                $.each(data, function (i, val) {
                    str += '<option value="' + val.id + '">' + val.name + '</option>';
                });
                str += '</select>';
                title.after(str);
            }, "json");
        }
        else {
            if (title.data("sign") === "select") {
                title.data("sign", "text").next().remove();
                title.after('<input type="search" class="form-control" id="txt_filter" autofocus>');
                return;
            }
        }

        if ($.trim(txt_filter.val()) != "") {
            filter(txt_filter.val(), 1);
        }
    });

    txt_filter.keyup(function (e) {
            filter($(this).val(), 1);
            $(".pagination-container").remove();
    });

    paging.on("click", "a", function (e) {
        if (paging.data("mode") === "ajax") {
            e.preventDefault();
            e.stopPropagation();
            filter(title.data("sign") === "text" ? txt_filter.val() : title.next().find("option:selected").val(), $(this).attr("href"));
        }
    });

    $("#detail_filter").on("click", function (e) {
        e.preventDefault();
        $.post("/Abonent/GetDetailFilterModal", {}, function (data) {
            var content = showModal(data);
            var txt_abonent_select = content.find("#txt_abonent_select");
            var drp_abonent_select_by = content.find("#drp_abonent_select_by");
            var drp_status = content.find("#drp_status");
            var drp_receiver = content.find("#drp_receiver");
            var drp_tower = content.find("#drp_tower");
            var txt_abonent_num = content.find("#txt_abonent_num");
            var finish_date_where = content.find("#finish_date_where");
            var finish_date_val = content.find("#finish_date_val");
            var pause_date_where = content.find("#pause_date_where");
            var pause_date_val = content.find("#pause_date_val");
            var credit_date_where = content.find("#credit_date_where");
            var credit_date_val = content.find("#credit_date_val");
            var balance_where = content.find("#balance_where");
            var balance_val = content.find("#balance_val");
            var discount_where = content.find("#discount_where");
            var discount_val = content.find("#discount_val");
            var status_where = content.find("#status_where");
            var status_val = content.find("#status_val");
            var service_where = content.find("#service_where");
            var service_val = content.find("#service_val");
            var drp_abonent_type = content.find("#drp_abonent_type");

            content.find("#detail_filter").on("click", function (b) {
                b.preventDefault();
                $.post("/Abonent/DetailFilterAbonents", {
                    type: drp_abonent_select_by.find("option:selected").val(),
                    abonent: txt_abonent_select.val(),
                    status: drp_status.find("option:selected").val(),
                    tower: drp_tower.find("option:selected").val(),
                    receiver: drp_receiver.find("option:selected").val(),
                    abonent_num: txt_abonent_num.val(),
                    finish_date: { where: finish_date_where.find("option:selected").val(), val: finish_date_val.val() },
                    pause_date: { where: pause_date_where.find("option:selected").val(), val: pause_date_val.val() },
                    credit_date: { where: credit_date_where.find("option:selected").val(), val: credit_date_val.val() },
                    balance: { where: balance_where.find("option:selected").val(), val: balance_val.val() },
                    discount: { where: discount_where.find("option:selected").val(), val: discount_val.val() },
                    service: { where: service_where.find("option:selected").val(), val: service_val.val() },
                    status2: { where: status_where.find("option:selected").val(), val: status_val.val() },
                    abonent_type: drp_abonent_type.find("option:selected").val()
                }, function (data) {
                    var str = '';
                    $.each(data, function (i, val) {
                        str += '<tr data-id="' + val.Id + '" data-card ="' + val.CardNum + '">' +
                            '<td>' + val.Name + ' <br /><small>' + val.Code + '</small></td><td>' + GetCustomerTypeDesc(val.Type) + '</td>' +
                            '<td>' + val.City + '</td><td>' + val.Phone + '</td><td>' + val.Num + '</td><td>' + val.CardNum + '</td><td>' + SetCardStatus(val.Status) + '</td>' +
                            '<td>' + val.ActivePacket + '</td></tr>';
                    });
                    abonents_body.html(str);
                    paging.remove();
                    content.modal("hide");
                });
            });

            content.find("#detail_filter_print").on("click", function (b) {
                b.preventDefault();

                location.href = "/Abonent/DetailFilterAbonentsExport?type=" + drp_abonent_select_by.find("option:selected").val() + "&abonent=" + txt_abonent_select.val() +
                    "&status=" + drp_status.find("option:selected").val() + "&tower=" + drp_tower.find("option:selected").val() + "&receiver=" + drp_receiver.find("option:selected").val() +
                    "&abonent_num=" + txt_abonent_num.val() + "&abonent_type=" + drp_abonent_type.find("option:selected").val() + "&finish_date_where=" + finish_date_where.find("option:selected").val() +
                    "&finish_date_val=" + finish_date_val.val() + "&pause_date_where=" + pause_date_where.find("option:selected").val() + "&pause_date_val=" + pause_date_val.val() +
                    "&credit_date_where=" + credit_date_where.find("option:selected").val() + "&credit_date_val=" + credit_date_val.val() + "&balance_where=" + balance_where.find("option:selected").val() +
                    "&balance_val=" + balance_val.val() + "&discount_where=" + discount_where.find("option:selected").val() + "&discount_val=" + discount_val.val() +
                    "&service_where=" + service_where.find("option:selected").val() + "&service_val=" + service_val.val() + "&status2_where=" + status_where.find("option:selected").val() +
                    "&status2_val=" + status_val.val();

            });

        });
    });
});

function cardStatusChange(select) {
    filter($(select).find("option:selected").val(), 1);
}

function cardstypefilter(select) {
    filter($(select).find("option:selected").val(), 1);
}

function towersChange(select) {
    filter($(select).find("option:selected").val(), 1);
}

function filter(letter, page) {
    var reload_block = event.which || event.keyCode;
    if (letter.length == 0 && reload_block == 8) { /*location.reload(); */}
    else {
        if (page === undefined) return;
        if (timer) {
            clearTimeout(timer);
        }

        timer = setTimeout(function () {
            var col = drp_filter.find("option:selected").val();
            if ((col === "cr.status" || col === "c.doc_num" || col === "cr.tower_id" || col === "c.type") || letter.length > 2) {
                $.post("/Abonent/FilterAbonents", { letter: letter, column: col, page: page }, function (data) {
                    var str = '';
                    $.each(data.Abonents, function (i, val) {
                        str += '<tr data-id="' + val.Id + '" data-card ="' + val.CardNum + '">' +
                            '<td>' + val.Name + '</td><td>' + GetCustomerTypeDesc(val.Type) + '</td>' +
                            '<td>' + val.City + '</td><td>' + val.Phone + '</td><td id="abonent_num">' + val.Num + '</td><td>' + val.CardNum + '</td><td>' + SetCardStatus(val.Status) + '</td>' +
                            '<td>' + val.ActivePacket + '</td><td>' + val.Code + '</td><td>' + val.DocNum + '</td><td id="edit_abonent"><a href=""><span class="glyphicon glyphicon-edit" id="edit_abonent"></span></a></td></tr>';
                    });
                    abonents_body.html(str);
                    paging.data("mode", "ajax").html(data.Paging);
                }, "json");
            }
        }, 300, letter);
    }
}

function exportToExcel() {
    location.href = "/Abonent/ExportToExcel";
}