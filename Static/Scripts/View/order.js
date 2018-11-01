var timer = null;
var orders_body;
var drp_filter;
var txt_filter;
var paging;

var menu_dialog;
$(function () {
    var code;
    var id;
    var status;

    orders_body = $("#orders_body");
    menu_dialog = $("#order_dialog");
    txt_filter = $("#txt_filter");
    paging = $("#paging");

    Date.prototype.yyyymmdd = function () {
        var mm = this.getMonth() + 1; // getMonth() is zero-based
        var dd = this.getDate();
        var hr = this.getHours();
        var min = this.getMinutes();

        return [this.getFullYear() + '/',
                (mm > 9 ? '' : '0') + mm + '/',
                (dd > 9 ? '' : '0') + dd + ' ', (hr > 9 ? '' : '0') + hr + ':', (min > 9 ? '' : '0') + min
        ].join('');
    };

    txt_filter.keyup(function (e) {
        filter($(this).val(), 1);
        $(".pagination-container").remove();
    });

    function filter(letter, page) {
        if (page === undefined) return;
        if (timer) {
            clearTimeout(timer);
        }

        if (letter.length < 3)
            return;

        timer = setTimeout(function () {
            $.post("/Order/FilterOrdersByName", { letter: letter,user_id:0, page: page }, function (data) {
                    var str = '';
                    $.each(data.Abonents, function (i, val)
                    {
                        val.data = JSON.parse(val.data);
                        var get_date = new Date(parseInt(val.get_date.replace("/Date(", "").replace(")/", ""), 10));
                        get_date = get_date.yyyymmdd();
                        var change_date = new Date(parseInt(val.change_date.replace("/Date(", "").replace(")/", ""), 10));
                        change_date = change_date.yyyymmdd();
                        var tdate = new Date(parseInt(val.tdate.replace("/Date(", "").replace(")/", ""), 10));
                        tdate = tdate.yyyymmdd();

                        str += '<tr data-code="'+val.code+'" data-id="' + val.id + '" data-card ="' + val.CardNum + '" class="' + (val.montage_status ? ("success") : ("")) + '">' +
                            '<td>' + val.num + '</td><td>' + tdate + '<br />' + '<small>' + get_date + '</small>' + '<br />' + '<small>' + change_date + '</small>' + '</td>' +
                            '<td>' + val.name.split("/")[0] + '<br/>' + '<small>' + val.code + '</small>' + '</td><td>' + val.data.Customer.City + '<br />' + val.data.Customer.District + '<br/><small>' + val.data.Customer.Region + '</small>' + '<br />' + '<small>' + val.data.Customer.Village + '</small>' + '</td><td>' + val.data.Customer.Phone1 + '<br />' + '<small>' + val.data.Customer.Phone2 + '</small>' + '</td><td>' + val.receivers_count + '</td><td class="_status">' + getStatusDesc(val.status) + '</td>' +
                            '<td>' + val.create_user + '<br/>' + '<small>' + val.changer_user + '</small>' + '<br/>' + '<small>' + val.approve_user + '</small>' + '</td><td>' + val.data.Customer.Desc + '</td>><td></td><td>' + val.exec_name + '</td><td></td><td>' + val.comment + '</td><td></td> <td><input type="checkbox" value="'+val.id+'"></td> </tr>';
                    });
                    orders_body.html(str);
                    paging.data("mode", "ajax").html(data.Paging);
                }, "json");
        }, 300, letter);
    }

    paging.on("click", "a", function (e) {
        if (paging.data("mode") === "ajax") {
            e.preventDefault();
            e.stopPropagation();
            filter(txt_filter.val(), $(this).attr("href"));
        }
    });


    $("#orders_body").on("click", "tr", function (e) {
        code = $(this).data("code");
        status = $(this).data("status");
        id = $(this).data("id");
        if (code === undefined || e.target.tagName === "BUTTON" || e.target.tagName === 'INPUT') {
            return;
        }
        e.preventDefault();
        menu_dialog.modal("show");
    });

    $("#picker_picker_nav").on("click", function (e)
    {
        var dt_from = $('#picker_picker_from').val();
        var dt_to = $('#picker_picker_to').val();
        dt_from = dt_from.replace(/\//g, "");
        dt_to = dt_to.replace(/\//g, "");
        
        var user_id = $('#user_filter').val();
        var promo_id = $('#promo_filter').val();
        var order_status = $('#order_status_filter').val();
        var poll = $('#order_poll_filter').val();
        var user_answers = $("#user_answers").val();
        var cur_attr = "/Order/?dt_from=" + dt_from + "&dt_to=" + dt_to + "&user_id=" + user_id + "&promo_id=" + promo_id + "&user_answers=" + user_answers+ "&CallCentrPoll=" + poll + "&order_status=" + order_status;//$(this).attr('href');
        //if(cur_attr.indexOf("user_id") !== -1)
           // cur_attr += '?user_id=' + user_id;
        //e.preventDefault();
        //alert(cur_attr);
        $(this).attr('href', cur_attr);
    });

    $("#check_all").on("change", function () {
        if ($(this).is(':checked')) {
            $("#orders_body").find("input[type='checkbox']").each(function () { this.checked = true; });
        }
        else {
            $("#orders_body").find("input[type='checkbox']").each(function () { this.checked = false; });
        }
    });

    $("#detail_filter").on("click", function (e) {
        e.preventDefault();
        $.post("/Order/GetDetailFilterModal", {}, function (data) {
            var content = showModal(data);

            var drp_group = content.find("#drp_group");
            var drp_status = content.find("#drp_status");
            var txt_abonent = content.find("#txt_abonent");
            var txt_region = content.find("#txt_region");
            var txt_city = content.find("#txt_city");
            var txt_district = content.find("#txt_district");
            content.find("#detail_filter").on("click", function (b) {
                b.preventDefault();
                $("#paging").remove();
                $.post("/Order/FilterOrders", {
                    group: drp_group.find("option:selected").val(),
                    abonent: txt_abonent.val(),
                    status: drp_status.find("option:selected").val(),
                    region: txt_region.val(),
                    city: txt_city.val(),
                    district: txt_district.val(),
                    dt_from: $("#picker_picker_from").val().ReplaceAll("/", ""),
                    dt_to: $("#picker_picker_to").val().ReplaceAll("/", ""),
                    get_date_dt_from: content.find("#picker_dt_picker_from").val().ReplaceAll("/", ""),
                    get_date_dt_to: content.find("#picker_dt_picker_to").val().ReplaceAll("/", "")
                }, function (data) {
                    content.modal("hide");
                    $("#orders_body").html(data).on("click", "tr", function (e) {
                        code = $(this).data("code");
                        id = $(this).data("id");
                        if (code === undefined || e.target.tagName === "BUTTON" || e.target.tagName === 'INPUT') {
                            return;
                        }
                        e.preventDefault();
                        content.modal("show");
                    });
                });
            });

            content.find("#detail_filter_print").on("click", function (b) {
                b.preventDefault();
                content.modal("hide");
                location.href = "/Order/FilterOrdersExport/?group=" + drp_group.find("option:selected").val() + "&abonent=" + txt_abonent.val() + "&status=" + drp_status.find("option:selected").val() +
                "&region=" + txt_region.val() + "&city=" + txt_city.val() + "&district=" + txt_district.val() + "&dt_from=" + $("#picker_picker_from").val().ReplaceAll("/", "_") +
                "&dt_to=" + $("#picker_picker_to").val().ReplaceAll("/", "_") + "&get_date_dt_from=" + content.find("#picker_dt_picker_from").val().ReplaceAll("/", "_") + "&get_date_dt_to=" + content.find("#picker_dt_picker_to").val().ReplaceAll("/", "_");

            });

        });
    });


    menu_dialog.on("click", "li", function (ev) {
        ev.preventDefault();

        //if (status === 'Closed' && $(this).data("index") !== 5)
            //return;
        $.ajaxSetup({
            cache: false
        });

        switch ($(this).data("index")) {
            case 0:
                if (status === 'Montage') {
                    location.href = "/Abonent/New?code=" + code;
                }
                break;
            case 1:
                location.href = "/Order/New/" + id;
                break;
            case 2:
                $.get("/Static/templates/getdate_change.html", function (data) {
                    menu_dialog.modal("hide");
                    var modalInstance = showModal(data);
                    modalInstance.find("#txt_date").datepicker({
                        autoclose: true,
                        language: "ka",
                        format: 'yyyy-mm-dd'
                    }).datepicker('update', new Date().toDateString()).on("changeDate", function (ev) {
                        $(this).datepicker("hide");
                    });

                    modalInstance.find("#change_date").on("click", function (b) {
                        b.preventDefault();
                        var ids = $("#orders_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                        $.post("/Order/ChangeDate", { ids: ids == "" ? id : ids, date: modalInstance.find("#txt_date").find("input").val() }, function (res) {
                            if (res === 1)
                                location.href = "/Order";
                        }, "json");
                    });

                });
                break;
            case 3:
                $.get("/Static/templates/order_status_change.html", function (data) {
                    menu_dialog.modal("hide");
                    var modalInstance = showModal(data);

                    modalInstance.find("#status_change").on("click", function (b) {
                        b.preventDefault();
                        var ids = $("#orders_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                        var status = modalInstance.find("#drp_status").find("option:selected").val();
                        //var executerID = modalInstance.find("#executer").val();
                        $.post("/Order/ChangeStatus", { ids: ids == "" ? id : ids, status: status }, function (res)
                        {
                            if (res === 1)
                            {
                                if (status === 'Canceled' || status === 'Loading') {
                                    $.get("/Static/templates/reason_desc.html", function (data1) {
                                        modalInstance.modal("hide");
                                        var reason_desc = showModal(data1);
                                        reason_desc.find("#save_reason").on("click", function (c) {
                                            c.preventDefault();
                                            $.post("/Order/SaveReason", { id: id, reason_id: reason_desc.find("#drp_reason").find("option:selected").val(), desc: reason_desc.find("textarea").val() }, function (res1) {
                                                if (res1 === 1)
                                                {
                                                    //alert("სტატუსი შეიცვალა წამრატებით!");
                                                    //location.href = "/Order";
                                                    modalInstance.modal("hide");
                                                }
                                                    
                                            });
                                        });
                                    });
                                }
                                else if (status == 'Montage')
                                {
                                    $.get("/Static/templates/order_cards.html", function (cards)
                                    {
                                        modalInstance.modal("hide");
                                        var cards_modal = showModal(cards);

                                        cards_modal.find("#card_add").on("click", function () {
                                            $.get("/Order/CardsAdd", { count: cards_modal.find(".well").size(), id: id }, function (d) {
                                                var dt = $(d);
                                                cards_modal.find("#customer_type").val(dt.find("#customer_type").val());
                                                cards_modal.find("#area").append(dt);
                                                $.validator.unobtrusive.parse(dt);
                                            }, "html");
                                        });

                                    });
                                }
                                else
                                {
                                    //location.href = "/Order";
                                    modalInstance.modal("hide");
                                }

                                var status_geo = "";
                                switch (status)
                                {
                                    case "Registered":
                                        status_geo = 'დარეგისტრირდა';
                                        break;

                                    case "Montage":
                                        status_geo = 'მონტაჟი';
                                        break;

                                    case "Canceled":
                                        status_geo = 'გაუქმება';
                                        break;

                                    case "Worked":
                                        status_geo = 'დამუშავება';
                                        break;

                                    case "Delayed":
                                        status_geo = 'გადადება';
                                        break;

                                    case "Loading":
                                        status_geo = 'ლოდინი';
                                        break;

                                    case "Sended":
                                        status_geo = 'გაგზავნა';
                                        break;

                                    case "Closed":
                                        status_geo = 'დასრულება';
                                        break;

                                    case "Promo":
                                        status_geo = 'უფასო აქცია';
                                        break;

                                    default:
                                }

                                $("[data-id=" + id + "]").find('._status').text(status_geo);
                            }
                            else
                            {
                                //location.href = "/Order";
                                alert('სტატუსი ვერ შეიცვალა');
                            }
                        }, "json");
                    });
                });
                break;
            case 4:
                //$.get("/Static/templates/group_change.html", function (data) {
                //    menu_dialog.modal("hide");
                //    var modalInstance = showModal(data);

                //    modalInstance.find("#group_change").on("click", function (b) {
                //        b.preventDefault();
                //        var ids = $("#orders_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                //        $.post("/Order/GroupChange", { ids: ids == "" ? id : ids, group_id: modalInstance.find("#group_combo").find("option:selected").val() }, function (res) {
                //            if (res === 1)
                //                location.href = "/Order";
                //        }, "json");
                //    });

                //});
                $.get("/Order/GroupChange", { order_id: id }, function (data) {
                    menu_dialog.modal("hide");
                    var modalInstance = showModal(data);

                    modalInstance.find("#group_change").on("click", function (b) {
                        b.preventDefault();
                        var ids = $("#orders_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                        $.post("/Order/GroupChange", { order_id: id, ids: ids == "" ? id : ids, group_id: modalInstance.find("#group_combo").find("option:selected").val() }, function (res) {
                            if (res === 1)
                            {
                                var name = modalInstance.find("#group_combo").find("option:selected").text();
                                var txt2 = $("<label></label>").text(name);
                                $("#executor_wrapper_" + id).empty();
                                $("#executor_wrapper_" + id).append(txt2);
                                modalInstance.modal("hide");
                                //location.href = "/Order";
                            }
                                
                        }, "json");
                    });

                });
                break;
            case 5:
                menu_dialog.modal("hide");
                $.post("/Order/Detail", { id: id }, function (data) {
                    wopen(data, 999, 600);
                });
                break;
        }

    });

    $("#btn_send_sms").on("click", function (e) {
        e.preventDefault();
        var ids = $.makeArray($("#orders_body").find("input:checked").map(function (i, val) {
            return val.value;
        }));
        if (ids.length == 0)
            return;
        $.get("/Order/GetSmsDialog", {}, function (data) {
            var modalInstance = showModal(data);

            modalInstance.find("#reason").hide();
            modalInstance.find("#sms_text").removeClass("hide").addClass("show");
            modalInstance.find("#send").on("click", function (ev) {
                ev.preventDefault();

                $.post("/Order/SendSMS", { abonents: ids, message: modalInstance.find("textarea").val() }, function (data) {
                    modalInstance.modal("hide");
                    alert(data);
                });
            });
        }, "html");
    });

});

function onSuccessAddCards(res) {
    if (res === 1)
        location.href = "/Order";
    else
        alert("შენახვა ვერ მოხერხდა!");
}

function cardStatusChange(select) {
    filter($(select).find("option:selected").val(), 1);
}

function orderApprove(id) {
    if (confirm("ჩაითვალოს დადასტურებულად?")) {
        $.post("/Order/OrderApprove", { id: id }, function (data) {
            if (data === 1)
                location.href = location.href;
        }, "json");
    }
}

function getStatusDesc(stat)
{
    var ret = "";
    switch (stat) {

        case 0:
            ret = "დარეგისტრირდა";
            break;

        case 1:
            ret = "მონტაჟი";
            break;

        case 2:
            ret = "გაუქმება";
            break;

        case 3:
            ret = "დამუშავება";
            break;

        case 4:
            ret = "გადადება";
            break;

        case 5:
            ret = "ლოდინი";
            break;

        case 6:
            ret = "გაგზავნა";
            break;

        case 7:
            ret = "დასრულება";
            break;

        default:
            break;
    }

    return ret;
}