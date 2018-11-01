var stop_date_dialog;
var abonent_num;
$(function () {
    stop_date_dialog = $('#stop_date_dialog');
    //$("#abonents_body").on("click", "tr", function (e) {
    //    abonent_num = $(this).data("card");
    //   // status = $(this).data("status");
    //    id = $(this).data("id");
    //    //if (code === undefined || e.target.tagName === "BUTTON" || e.target.tagName === 'INPUT') {
    //    //    return;
    //    //}
    //    e.preventDefault();
    //    stop_date_dialog.modal("show");
    //});

    $(".change_card_status").on("click", function (e) {
        e.preventDefault();
        $.post("/StoppedCards/GetStatusInfo/",
            {
                card_id: $(this).closest("tr").attr("data-id")// $("#Customer_Id").val()
            },
            function (data) {
                var content = $(data);
                content.find("#txt_date").datepicker({
                    autoclose: true,
                    language: "ka",
                    format: 'yyyy-mm-dd'
                }).datepicker('update', new Date().toDateString()).on("changeDate", function (ev) {
                    $(this).datepicker("hide");
                });
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                    //location.href = location.href;
                });
                initContent(content);
            }, "html");
    });

    $('#remove_filter').on("click", function () {
        location.href = location.pathname;
    });
    $('#picker_picker_nav').on("click", function (e) {
        var attrval = $(this).attr("href");
        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var status = $('#status_filter').val();
        var filter = $('#txt_filter').val();
        var drp_filter = $('#drp_filter').val();
        //CustomerAttach/?dt_from=26072017&dt_to=26072017
        var new_attr = "?name=" + filter + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&drp_filter=" + drp_filter + "&status=" + status;
        $(this).attr("href", new_attr);

    });
    //$("select").change(function () {
    //    var str = $(this).text();
    //    var val = $(this).val();
    //    var href = '/StoppedCards/Index';

    //    href += '?status=' + val;

    //    location = href;
    //});
    //    stop_date_dialog.on("click", "li", function (ev) {
    //        ev.preventDefault();

    //        $.get("/Static/templates/getdate_change.html", function (data) {
    //            stop_date_dialog.modal("hide");
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
    //                //var ids = $("#orders_body").find("input:checked").map(function (ind, el) { return el.value; }).get().join(",");
    //                $.post("/StoppedCards/ChangeDate", { card_num: abonent_num, date: modalInstance.find("#txt_date").find("input").val() }, function (res) {
    //                    if (res === 1)
    //                        location.href = "/StoppedCards";
    //                }, "json");
    //            });
    //        });
    //    });
    //});

    function initContent(content) {
        content.find('#save').on("click", function (e) {
            e.preventDefault();
            var id = content.find("#card_").attr("data-val");//$(this).closest("tr").attr("data-id"),//
            var call_status = content.find("#card_call_status").val();
            var desc = content.find("#Card_Desc").val();
            var clasname = "";
            $.post("/StoppedCards/SaveStatus",
                {
                    id: id,
                    call_status: call_status,
                    desc: desc,
                    date: content.find("#txt_date").find("input").val()
                },
                function (data) {
                    if (data === 1) {
                        alert("სტატუსი შეიცვალა წარმატებით.");

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

        //content.find('#save_abonent_stat').on("click", function (e) {
        //    var dataval = $(this).closest("tr").attr("data-val");
        //    var custid = $(this).closest("tr").attr("data-id");
        //    var info = dataval == "is_satisfied" ? content.find("#Card_satstat_Desc").val() : content.find("#Card_byustat_Desc").val();
        //    var ab_satisfied_status = content.find("#ab_satisfied_status").val();
        //    var ab_buyreason_status = content.find("#ab_buyreason_status").val();
        //    //switch(dataval)
        //    //{
        //    //    case "buy_reason":
        //    //        {

        //    //        }
        //    //        break;

        //    //    case "is_satisfied":
        //    //        {

        //    //        }
        //    //        break;
        //    //}

        //    e.preventDefault();
        //    $.post("/Verify/UpdateAbonentStatus",
        //        {
        //            cust_id: custid,// 
        //            dataval: dataval,
        //            satisfied_status: ab_satisfied_status,
        //            buyreason_status: ab_buyreason_status,
        //            info: info
        //        },
        //        function (data) {
        //            if (data === 1) {
        //                alert("სტატუსი შეიცვალა წარმატებით.");
        //                updateColor(content);
        //            }
        //            else {
        //                alert("შეცდომა: სტატუსი ვერ შეიცვალა!");
        //            }
        //        }, "json");
        //});
    }
});