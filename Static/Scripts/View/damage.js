var menu_dialog;
$(function () {
    $(".datepickers").datePickers({ prefix: "picker" });
    var status;
    menu_dialog = $("#damage_dialog");
    $("#show_modal_bort").on("click", function (e) {
        if ($('#user_filter').val() == 0) {
            alert("გთხოვთ აირჩიოთ ბორტი!");
            return;
        }
        e.preventDefault();
        var customer_attach_code = $("#Customer_Code").val();
        var dt_from = $('#picker_picker_from').val();
        var dt_to = $('#picker_picker_to').val();
        dt_from = dt_from.replace(/\//g, "");
        dt_to = dt_to.replace(/\//g, "");
        var checked_user = false;
        var checked_bort_end = false;
        var abonent_name = $('#txt_filter').val();
        var user_id = $('#user_filter').val();
        var order_status = $('#order_status_filter').val();
        var operator_status = $('#userOperator_filter').val();
        $.post("/Damage/MenueBort/",
            {
                user_id: user_id
            },

            function (data) {
                var content = $(data);
                //switch (content.find(this).val()) {
                //    case 0: user_id = $('#user_filter').val();
                //        break;
                //    case 1: order_status = 6;
                //        break;
                //    case 2: order_status = -2;
                //        break;
                //    case 3: checked_user = true;
                //        break;
                //    case 4: checked_bort_end = true;
                //        break;
                //}
                // content.find(".selectpicker").selectpicker();
                $.post("/Damage/BortShowInfo/",
                    {
                        dt_from: dt_from,
                        dt_to: dt_to,
                        user_id: user_id,
                        name: abonent_name,
                        order_status: order_status,
                        operator_status: operator_status,
                        checked_user: checked_user,
                        checked_bort_end: checked_bort_end
                        //code: customer_attach_code
                    },

                    function (response) {
                        content.find('#modal_bort').html(response);
                    });
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                    //location.href = location.href;
                });
                BortContent(content);
            }, "html");
    });
    
    //$("#show_modal_bort").on("click", function (e) {
    //    e.preventDefault();
    //    var customer_attach_code = $("#Customer_Code").val();
    //    var dt_from = $('#picker_picker_from').val();
    //    var dt_to = $('#picker_picker_to').val();
    //    dt_from = dt_from.replace(/\//g, "");
    //    dt_to = dt_to.replace(/\//g, "");
    //    //var checked_user = $('#checked_bort').is(':checked');
    //    //var checked_bort_end = $('#checked_bort_end').is(':checked');
    //    var abonent_name = $('#txt_filter').val();
    //    var user_id = $('#user_filter').val();
    //    var order_status = $('#order_status_filter').val();
    //    var operator_status = $('#userOperator_filter').val();
    //    $.post("/Damage/BortShowInfo/",
    //        {
    //            dt_from: dt_from,
    //            dt_to: dt_to,
    //            user_id: user_id,
    //            name: abonent_name,
    //            order_status: order_status,
    //            operator_status:operator_status,
    //            code: customer_attach_code
    //        },

    //        function (data) {
    //            var content = $(data);
    //           // content.find(".selectpicker").selectpicker();
    //            content.modal("show");
    //            content.on('hidden.bs.modal', function () {
    //                content.remove();
    //                //location.href = location.href;
    //            });
    //            initNewAttachContent(content);
    //        }, "html");
    //});




    $("#damages_body").on("click", "tr", function (e) {
        id = $(this).data("id");
        status = $(this).data("status");
        if (e.target.tagName === "BUTTON" || e.target.tagName === "INPUT") {
            return;
        }
        menu_dialog.modal("show");
        e.preventDefault();
    });

    $("#check_all").on("change", function () {
        if ($(this).is(':checked')) {
            $("#damages_body").find("input[type='checkbox']").each(function () { this.checked = true; });
        }
        else {
            $("#damages_body").find("input[type='checkbox']").each(function () { this.checked = false; });
        }
    });

    menu_dialog.on("click", "li", function (ev) {
        ev.preventDefault();

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
                location.href = "/Damage/New/" + id;
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
                        $.post("/Damage/ChangeDate", { ids: ids == "" ? id : ids, date: modalInstance.find("#txt_date").find("input").val() }, function (res) {
                            if (res === 1)
                                location.href = "/Damage";
                        }, "json");
                    });

                });
                break;
            case 3:
                $.get("/Static/templates/damage_status_change.html", function (data) {
                    menu_dialog.modal("hide");
                    var modalInstance = showModal(data);

                    modalInstance.find("#status_change").on("click", function (b) {
                        b.preventDefault();
                        var ids = $("#orders_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                        var status = modalInstance.find("#drp_status").find("option:selected").val();
                        //var executerID = modalInstance.find("#executer").val();
                        $.post("/Damage/ChangeStatus", { ids: ids == "" ? id : ids, status: status }, function (res) {
                            if (res === 1) {
                                if (status === 'Canceled' || status === 'Loading') {
                                    $.get("/Static/templates/reason_desc.html", function (data1) {
                                        modalInstance.modal("hide");
                                        var reason_desc = showModal(data1);
                                        reason_desc.find("#save_reason").on("click", function (c) {
                                            c.preventDefault();
                                            $.post("/Damage/SaveReason", { id: id, reason_id: reason_desc.find("#drp_reason").find("option:selected").val(), desc: reason_desc.find("textarea").val() }, function (res1) {
                                                if (res1 === 1) {
                                                    //alert("სტატუსი შეიცვალა წამრატებით!");
                                                    //location.href = "/Order";
                                                    modalInstance.modal("hide");
                                                }

                                            });
                                        });
                                    });
                                }
                                else if (status == 'Montage') {
                                    $.get("/Static/templates/order_cards.html", function (cards) {
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
                                else {
                                    //location.href = "/Order";
                                    modalInstance.modal("hide");
                                }

                                var status_geo = "";
                                switch (status) {
                                    case "loading":
                                        status_geo = 'ლოდინი';
                                        break;
                                    case "CardProblem":
                                        status_geo = 'ბარათის პრობლემა';
                                        break;

                                    case "TVTransfer":
                                        status_geo = 'TV/AV გადაყვანა';
                                        break;

                                    case "SenderProblem":
                                        status_geo = 'sender-ის პრობლემა';
                                        break;

                                    case "FactoryDefects":
                                        status_geo = 'ქარხნული წუნი';
                                        break;

                                    case "ProgramNew":
                                        status_geo = 'პროგრამული განახლება';
                                        break;

                                    case "AntenProblem":
                                        status_geo = 'ანტენა/კაბელის პრობლემები';
                                        break;
                                    case "Closed":
                                        status_geo = 'დასრულება';
                                        break;
                                    case "OtherReason":
                                        status_geo = 'მიზეზი სხვა';
                                        break;
                                    case "NoSignal":
                                        status_geo = 'სიგნალი არ არის';
                                        break;
                                    case "ImprovedInstallerDidNot":
                                        status_geo = 'თვითონ გამოსწორდა ისე რომ ინსტალატორი არ მივიდა';
                                        break;
                                    case "Unresolved":
                                        status_geo = 'მოუგვარებელი';
                                        break;
                                    case "Processing":
                                        status_geo = 'დამუშავება';
                                        break;
                              
                                    default:
                                }

                                $("[data-id=" + id + "]").find('._status').text(status_geo);
                            }
                            else {
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
                $.get("/Damage/GroupChange", { damage_id: id }, function (data) {
                    menu_dialog.modal("hide");
                    var modalInstance = showModal(data);

                    modalInstance.find("#group_change").on("click", function (b) {
                        b.preventDefault();
                        var ids = $("#damages_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                        $.post("/Damage/GroupChange", { order_id: id, ids: ids == "" ? id : ids, group_id: modalInstance.find("#group_combo").find("option:selected").val() }, function (res) {
                            if (res === 1) {
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
                $.post("/Damage/Detail", { id: id }, function (data) {
                    wopen(data, 999, 600);
                });
                break;
        }
        //switch ($(this).data("index")) {
        //    case 1:
        //        $.get("/Static/templates/damage_status_change.html", function (data) {
        //            menu_dialog.modal("hide");
        //            var modalInstance = showModal(data);

        //            modalInstance.find("#status_change").on("click", function (b) {
        //                b.preventDefault();
        //                var status = modalInstance.find("#drp_status").find("option:selected").val();
        //                var ids = $("#damages_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
        //                $.post("/Damage/ChangeDamageStatus", { ids: ids == "" ? id : ids, status: status }, function (res) {
        //                    if (res === 1) {

        //                        if (status === 'Executed') {
        //                            modalInstance.modal("hide");
        //                            $.post("/Abonent/GetServicesList", {}, function (services) {
        //                                var services_content = showModal(services);
        //                                services_content.find("#services_add").on("click", function (e) {
        //                                    e.preventDefault();
        //                                    var ids = $.makeArray(services_content.find("#services_body tr").map(function (i, val) {
        //                                        if ($(val).find("input[type='checkbox']").prop("checked")) {
        //                                            return { ServiceId: $(val).data("id"), Amount: $(val).find("input[type='text']").eq(0).val(), PayType: $(val).find("select option:selected").val() }
        //                                        }
        //                                    }));
        //                                    $.post("/Damage/SaveDamageServices", { id: id, services: ids }, function (res) {
        //                                        if (res === 1)
        //                                            location.href = "/Damage";
        //                                    }, "json");
        //                                });
        //                            }, "html");
        //                        } else if (status === 'Loading') {
        //                            $.get("/Static/templates/reason_desc.html", function (data1) {
        //                                modalInstance.modal("hide");
        //                                var reason_desc = showModal(data1);
        //                                reason_desc.find("#save_reason").on("click", function (c) {
        //                                    c.preventDefault();
        //                                    $.post("/Damage/SaveReason", { id: id, reason_id: reason_desc.find("#drp_reason").find("option:selected").val(), desc: reason_desc.find("textarea").val() }, function (res1) {
        //                                        if (res1 === 1)
        //                                            location.href = "/Damage";
        //                                    });
        //                                });
        //                            });
        //                        } else {
        //                            location.href = "/Damage";
        //                        }
        //                    }
        //                }, "json");
        //            });

        //        });
        //        break;
        //    case 2:
        //        $.get("/Static/templates/group_change.html", function (data) {
        //            menu_dialog.modal("hide");
        //            var modalInstance = showModal(data);

        //            modalInstance.find("#group_change").on("click", function (b) {
        //                b.preventDefault();
        //                var ids = $("#damages_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
        //                $.post("/Damage/GroupChange", { ids: ids == "" ? id : ids, group_id: modalInstance.find("#group_combo").find("option:selected").val() }, function (res) {
        //                    if (res === 1)
        //                        location.href = "/Damage";
        //                }, "json");
        //            });

        //        });
        //        break;
        //    case 3:
        //        $.get("/Static/templates/getdate_change.html", function (data) {
        //            menu_dialog.modal("hide");
        //            var modalInstance = showModal(data);
        //            modalInstance.find("#txt_date").datepicker({
        //                autoclose: true,
        //                language: "ka",
        //                format: 'yyyy-mm-dd'
        //            }).datepicker('update', new Date().toDateString()).on("changeDate", function (ev) {
        //                $(this).datepicker("hide");
        //            });

        //            modalInstance.find("#change_date").on("click", function (b) {
        //                b.preventDefault();
        //                var ids = $("#damages_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
        //                $.post("/Damage/ChangeDate", { ids: ids == "" ? id : ids, date: modalInstance.find("#txt_date").find("input").val() }, function (res) {
        //                    if (res === 1)
        //                        location.href = "/Damage";
        //                }, "json");
        //            });

        //        });
        //        break;
        //    case 4:
        //        menu_dialog.modal("hide");
        //        $.post("/Order/Detail", { id: id }, function (data) {
        //            wopen(data, 999, 600);
        //        });
        //        break;
        //}
    });
    $('#remove_filter').on("click", function () {
        location.href = location.pathname;
    });

    $("#detail_filter").on("click", function (e) {
        e.preventDefault();
        $.post("/Damage/GetDetailFilterModal", {}, function (data) {
            var content = showModal(data);

            var drp_group = content.find("#drp_group");
            var drp_status = content.find("#drp_status");
            var txt_abonent = content.find("#txt_abonent");
            var txt_abonent_num = content.find("#txt_abonent_num");
            var txt_city = content.find("#txt_city");
            var txt_address = content.find("#txt_address");
            content.find("#detail_filter").on("click", function (b) {
                b.preventDefault();
                $("#paging").remove();
                $.post("/Damage/FilterDamages", {
                    group: drp_group.find("option:selected").val(),
                    abonent: txt_abonent.val(),
                    abonent_num: txt_abonent_num.val(),
                    status: drp_status.find("option:selected").val(),
                    city: txt_city.val(),
                    address: txt_address.val(),
                    dt_from: $("#picker_picker_from").val().ReplaceAll("/", ""),
                    dt_to: $("#picker_picker_to").val().ReplaceAll("/", ""),
                    get_date_dt_from: content.find("#picker_dt_picker_from").val().ReplaceAll("/", ""),
                    get_date_dt_to: content.find("#picker_dt_picker_to").val().ReplaceAll("/", "")
                }, function (data) {
                    content.modal("hide");
                    $("#damages_body").html(data).on("click", "tr", function (e) {
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
                location.href = "/Damage/FilterDamagesExport/?group=" + drp_group.find("option:selected").val() + "&abonent=" + txt_abonent.val() + "&abonent_num=" + txt_abonent_num.val() + "&status=" + drp_status.find("option:selected").val() +
                "&region=" + txt_region.val() + "&city=" + txt_city.val() + "&address=" + txt_address.val() + "&dt_from=" + $("#picker_picker_from").val().ReplaceAll("/", "_") +
                "&dt_to=" + $("#picker_picker_to").val().ReplaceAll("/", "_") + "&get_date_dt_from=" + content.find("#picker_dt_picker_from").val().ReplaceAll("/", "_") + "&get_date_dt_to=" + content.find("#picker_dt_picker_to").val().ReplaceAll("/", "_");

            });

        });
    });
   

    $("#btn_send_sms").on("click", function (e) {
        e.preventDefault();
        $.get("/Order/GetSmsDialog", {}, function (data) {
            var modalInstance = $(data);
            var modalInstance = showModal(data);

            modalInstance.find("#reason").hide();
            modalInstance.find("#sms_text").removeClass("hide").addClass("show");
            modalInstance.find("#send").on("click", function (ev) {
                ev.preventDefault();

                var ids = $.makeArray($("#damages_body").find("input:checked").map(function (i, val) {
                    return val.value;
                }));
                $.post("/Damage/SendSMS", { abonents: ids, message: modalInstance.find("textarea").val() }, function (data) {
                    modalInstance.modal("hide");
                    alert(data);
                });
            });
        }, "html");
    });

    $("#picker_picker_nav").on("click", function (e) {
        var dt_from = $('#picker_picker_from').val();
        var dt_to = $('#picker_picker_to').val();
        dt_from = dt_from.replace(/\//g, "");
        dt_to = dt_to.replace(/\//g, "");
        var checked_user = $('#checked_bort').is(':checked');
        var checked_bort_end = $('#checked_bort_end').is(':checked');
        var abonent_name = $('#txt_filter').val();
        var user_id = $('#user_filter').val();
        var order_status = $('#order_status_filter').val();
        var operator_status = $('#userOperator_filter').val();
        var user_answers = $("#user_answers").val();
        var cur_attr = "/Damage/?dt_from=" + dt_from + "&dt_to=" + dt_to + "&user_id=" + user_id + "&name=" + abonent_name + "&order_status=" + order_status + "&operator_status=" + operator_status + "&checked_user=" + checked_user + "&checked_bort_end=" + checked_bort_end + "&user_answers=" + user_answers;//$(this).attr('href');
        //if(cur_attr.indexOf("user_id") !== -1)
        // cur_attr += '?user_id=' + user_id;
        //e.preventDefault();
        //alert(cur_attr);
        $(this).attr('href', cur_attr);
    });

});
function BortContent(_content) {
    _content.find("#show_modal_bort").on("click", function (e) {
        e.preventDefault();
        var customer_attach_code = $("#Customer_Code").val();
        var dt_from = $('#picker_picker_from').val();
        var dt_to = $('#picker_picker_to').val();
        dt_from = dt_from.replace(/\//g, "");
        dt_to = dt_to.replace(/\//g, "");
        var checked_user = false;
        var checked_bort_end = false;
        var checked_bort = false;
        var abonent_name = $('#txt_filter').val();
        var user_id = $('#user_filter').val();
        var order_status = $('#order_status_filter').val();
        var operator_status = $('#userOperator_filter').val();

        switch (_content.find(this).val()) {
            case "0": user_id = $('#user_filter').val();
                break;
            case "1": order_status = 7;
                break;
            case "2": order_status = -2;
                break;
            case "3": checked_user = true;
                break;
            case "4": checked_bort_end = true;
                break;
            case "5": checked_bort = true;
                break;
        }
        //$.post("/Damage/MenueBort/",
        //    {
        //        user_id: user_id
        //    },

        //    function (data) {
                //var content = $(data);
       
                // content.find(".selectpicker").selectpicker();
                $.post("/Damage/BortShowInfo/",
                    {
                        dt_from: dt_from,
                        dt_to: dt_to,
                        user_id: user_id,
                        name: abonent_name,
                        order_status: order_status,
                        operator_status: operator_status,
                        checked_user: checked_user,
                        checked_bort_end: checked_bort_end,
                        checked_bort:checked_bort
                        //code: customer_attach_code
                    },

                    function (response) {
                        _content.find('#modal_bort').html(response);
                    });
                //content.modal("show");
                //content.on('hidden.bs.modal', function () {
                //    content.remove();
                //    //location.href = location.href;
                //});
                //BortContent(content);
            //}, "html");
    });

}

function damageApprove(id) {
    if (confirm("ჩაითვალოს დადასტურებულად?")) {
        $.post("/Damage/DamageApprove", { id: id }, function (data) {
            if (data === 1)
                location.href = location.href;
        }, "json");
    }
}