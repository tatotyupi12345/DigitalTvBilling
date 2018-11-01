var menu_dialog;
var notclick = 0;
$(function () {
    $(".datepickers").datePickers({ prefix: "picker" });
    var status;
    menu_dialog = $("#Cancellation_dialog");

    $("#cancels_body").not("#notclick").on("click", "tr", function (e) {
        if (notclick == 0) {
            id = $(this).data("id");
            status = $(this).data("status");
            if (e.target.tagName === "BUTTON" || e.target.tagName === "INPUT") {
                return;
            }
            menu_dialog.modal("show");
            e.preventDefault();
        }
    });

    //$("#picker_picker_nav").on("click", function (e) {
    //    var dt_from = $('#picker_picker_from').val();
    //    var dt_to = $('#picker_picker_to').val();
    //    dt_from = dt_from.replace(/\//g, "");
    //    dt_to = dt_to.replace(/\//g, "");

    //    var user_id = $('#user_filter').val();
    //    var order_status = $('#cancled_status_filter').val();
    //    var cur_attr = "/Cancellation/?user_id=" + user_id + "&cancled_status=" + order_status +"&page="+5;//$(this).attr('href');
    //    //if(cur_attr.indexOf("user_id") !== -1)
    //    // cur_attr += '?user_id=' + user_id;
    //    //e.preventDefault();
    //    //alert(cur_attr);
    //    $(this).attr('href', cur_attr);
    ////});
    //$("#picker_picker_nav").on("click", function (e) {
    //    var dt_from = $('#picker_picker_from').val();
    //    var dt_to = $('#picker_picker_to').val();
    //    dt_from = dt_from.replace(/\//g, "");
    //    dt_to = dt_to.replace(/\//g, "");
    //    var abonent_name = $('#txt_filter').val();
    //    var user_id = $('#user_filter').val();
    //    var cancled_status = $('#cancled_status_filter').val();
    //    //var operator_status = $('#userOperator_filter').val();
    //    var cur_attr = "/Cancellation/?user_id=" + 15 + "&name=" + abonent_name + "&cancled_status=" + cancled_status+"&page="+1;
    //    $(this).attr('href', cur_attr);
    //});

    $("#picker_picker_nav").on("click", function (e) {
        var dt_from = $('#picker_picker_from').val();
        var dt_to = $('#picker_picker_to').val();
        dt_from = dt_from.replace(/\//g, "");
        dt_to = dt_to.replace(/\//g, "");
        var abonent_name = $('#txt_filter').val();
        var user_id = $('#user_filter').val();
        var cancled_status = $('#cancled_status_filter').val();
        var operator_status = $('#userOperator_filter').val();
        var cur_attr = "/Cancellation/?dt_from=" + dt_from + "&dt_to=" + dt_to + "&user_id=" + user_id + "&name=" + abonent_name + "&cancled_status=" + cancled_status;//$(this).attr('href');
        //if(cur_attr.indexOf("user_id") !== -1)
        // cur_attr += '?user_id=' + user_id;
        //e.preventDefault();
        //alert(cur_attr);
        //$.get("/Cancellation/Index", { user_id: user_id, name: abonent_name, cancled_status: cancled_status,page:1 }, function () {

        //});
        $(this).attr('href', cur_attr);
    });
    $('#remove_filter').on("click", function () {
        location.href = location.pathname;
    });
    $("#check_all").on("change", function () {
        if ($(this).is(':checked')) {
            $("#cancels_body").find("input[type='checkbox']").each(function () { this.checked = true; });
        }
        else {
            $("#cancels_body").find("input[type='checkbox']").each(function () { this.checked = false; });
        }
    });

    menu_dialog.on("click", "li", function (ev) {
        ev.preventDefault();

        $.ajaxSetup({
            cache: false
        });
        switch ($(this).data("index")) {
            case 1:
                location.href = "/Cancellation/New/" + id;
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
                        var ids = $("#cancels_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                        $.post("/Cancellation/ChangeDate", { ids: ids == "" ? id : ids, date: modalInstance.find("#txt_date").find("input").val() }, function (res) {
                            if (res === 1)
                                location.href = "/Cancellation";
                        }, "json");
                    });

                });
                break;
            case 3:
                $.get("/Static/templates/cancel_status_change.html", function (data) {
                    menu_dialog.modal("hide");
                    var modalInstance = showModal(data);

                    modalInstance.find("#status_change").on("click", function (b) {
                        b.preventDefault();
                        var ids = $("#cancels_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                        var status = modalInstance.find("#drp_status").find("option:selected").val();
                        //var executerID = modalInstance.find("#executer").val();
                        $.post("/Cancellation/ChangeStatus", { ids: ids == "" ? id : ids, status: status }, function (res) {
                            if (res === 1) {
                                if (status === 'Canceled' || status === 'Loading') {
                                    $.get("/Static/templates/reason_desc.html", function (data1) {
                                        modalInstance.modal("hide");
                                        var reason_desc = showModal(data1);
                                        reason_desc.find("#save_reason").on("click", function (c) {
                                            c.preventDefault();
                                            $.post("/Cancellation/SaveReason", { id: id, reason_id: reason_desc.find("#drp_reason").find("option:selected").val(), desc: reason_desc.find("textarea").val() }, function (res1) {
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
                                    case "Cancle":
                                        status_geo = 'გაუქმება';
                                    case "Closed":
                                        status_geo = 'დასრულება';
                                        break;
                                    case "NotClosed":
                                        status_geo = 'გადაიფიქრა';
                                        break;
                                    case "ReallyCancled":
                                        status_geo = 'ნამდვილად გასაუქმებელი';
                                        break;
                                    case "ServiceCenter":
                                        status_geo = 'სერვის ცენტრი';
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
                $.get("/Cancellation/GroupChange", { Cancellation_id: id }, function (data) {
                    menu_dialog.modal("hide");
                    var modalInstance = showModal(data);

                    modalInstance.find("#group_change").on("click", function (b) {
                        b.preventDefault();
                        var ids = $("#cancels_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                        $.post("/Cancellation/GroupChange", { order_id: id, ids: ids == "" ? id : ids, group_id: modalInstance.find("#group_combo").find("option:selected").val() }, function (res) {
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
                $.get("/Static/templates/cancel_coment_change.html", function (data) {
                    menu_dialog.modal("hide");
                    var modalInstance = showModal(data);
                    //modalInstance.find("#txt_date").datepicker({
                    //    autoclose: true,
                    //    language: "ka",
                    //    format: 'yyyy-mm-dd'
                    //}).datepicker('update', new Date().toDateString()).on("changeDate", function (ev) {
                    //    $(this).datepicker("hide");
                    //});

                    modalInstance.find("#comment_change").on("click", function (b) {
                        b.preventDefault();
                        var ids = $("#cancels_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
                        $.post("/Cancellation/ChangeComment", { ids: ids == "" ? id : ids ,comment: $('#Card_Desc').val() }, function (res) {
                            if (res === 1)
                                location.href = "/Cancellation";
                        }, "json");
                    });

                });
                //menu_dialog.modal("hide");
                //$.post("/Cancellation/Detail", { id: id }, function (data) {
                //    wopen(data, 999, 600);
                //});
                break;
        }

    });
    
});
function cancelApprove(id) {
    if (confirm("ჩაითვალოს დადასტურებულად?")) {
        $.post("/Cancellation/CancelApprove", { id: id }, function (data) {
            if (data === 1)
                location.href = location.href;
        }, "json");
    }
}

function updateReturnedCard(id) {
    //$('.returne_card_cancles_name').on('click', function () {
    //menu_dialog.modal("toggle");
    notclick = 1;
    var returned_card_id = $(this).closest('tr').attr('data-id');
    if (id != 0) {
        $.post('Cancellation/_ReturnedCardCancle', { returned_card_id: id /*$(this).closest('tr').attr('data-id')*/ }, function (data) {
            //menu_dialog.modal("toggle");
            var __content = $(data);
            __content.find(':checkbox').checkboxpicker();
            init_cardcancel(__content);
            __content.modal("show");
            __content.on('hidden.bs.modal', function () {
                __content.remove();

            });
            notclick = 0;
        }, "html");
    }
    //});
}