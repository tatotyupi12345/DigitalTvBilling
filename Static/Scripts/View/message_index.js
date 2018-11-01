var list;
var txt_abonent_select;
var drp_abonent_select_by;
var drp_status;
var drp_receiver;
var drp_tower;
var txt_abonent_num;
var finish_date_where;
var finish_date_val;
var pause_date_where;
var pause_date_val;
var credit_date_where;
var credit_date_val;
var balance_where;
var balance_val;
var discount_where;
var discount_val;
var status_where;
var status_val;
var service_where;
var service_val;
var MessageText;
var chk_disposable;
var drp_abonent_type;
$(function () {
    $(".datepickers").datePickers({ prefix: "picker" });
    $('input[name="daterange"]').daterangepicker({
        timePicker: true,
        timePickerIncrement: 30,
        locale: {
            format: 'MM/DD/YYYY HH:mm'
        }
    });
    $("#messages_body").on("click","tr", function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        $.ajaxSetup({
            cache: false
        });
        $.get("/Static/templates/message_result_modal.html", function (data) {
            var modalInstance = showModal(data);
            $.post("/Message/GetDetails", { id: id }, function (res) {
                modalInstance.find("#message").html(res.text);
                var str = '';
                for (var i = 0; i < res.abonents.length; i++) {
                    str += '<li class="list-group-item">' + res.abonents[i] + '</li>';
                }
                modalInstance.find("#abonents_list").html(str);
                modalInstance.find("#abonents_count").html(res.abonents.length);
            }, "json");
        });
    });


    $("#get_auto_messages").on("click", function (e) {
        e.preventDefault();
        $.post("/Message/GetAutoTemplates", {}, function (data) {
            var content = $(data);
            content.modal("show");
            content.find("#messages_body").on("click", "button", function (ev) {
                ev.preventDefault();
                var btn = $(this);
                $.post("/Message/DeleteAutoTemplate", { id: btn.data("id") }, function (res) {
                    if (res === 1)
                        btn.closest("tr").remove();
                }, "json");
            });
        }, "html");
    });

    var remove_array;
    var lennght = 0;
    var resize = 0;
    var selectedAll = false;
    var ind = 0;
    $("#show_modal").on("click", function (e) {
        e.preventDefault();
        $.post("/Message/GetMessage", {}, function (data) {
            var content = $(data);
            content.modal("show");
            resize = 0;
            ind = 0;
            lennght = 0;
            selectedAll = false;
            content.find(".selectpicker").selectpicker();
            //content.find(".datepickers").datePickers({ prefix: "picker" });

            
            content.on('hidden.bs.modal', function () {
                content.remove();
            });
            initContent(content);
        }, "html");
    });

    function initContent(content) {

        //content.find("#expand_dates").on("click", function (e) {
        //    var towerids = $("#team").val();

        //    if (towerids != null) {

        //        for (var i = 0; i < towerids.length; i++) {
        //            content.find(".datepickers").datePickers({ prefix: "picker" });
        //            var clone = content.find('#clone_pattern').clone();
        //            $(clone).css('display', 'block');
        //            $(clone).attr("id", towerids[i]);
        //            $(clone).find("#select_name").html(content.find('#' + towerids[i] + '').html());
        //            content.find('#clones_wrapper').append(clone);
        //            content.find('input[name="daterange"]').daterangepicker({
        //                timePicker: true,
        //                timePickerIncrement: 1,
        //                locale: {
        //                    format: 'MM/DD/YYYY HH:mm'
        //                }
        //            });
        //        }
        //    }
        //    else
        //    {
        //        content.find("#clones_wrapper").html('');
        //    }
        //});

        content.find("#team").on("changed.bs.select", function (e, clickedIndex, newValue, oldValue) {
            if (newValue == undefined) {
                
            }
            else {
                var tower_count = content.find("li").length;
                selectedAll = false;
                content.find("#clones_wrapper").html('');
                var towerids = $("#team").val();

                if (towerids != null)
                {
                    if (towerids.length >= tower_count)
                    {
                        selectedAll = true;
                        content.find("#clones_wrapper").html('');

                        content.find(".datepickers").datePickers({ prefix: "picker" });
                        var clone = content.find('#clone_pattern').clone();
                        $(clone).css('display', 'block');
                        $(clone).find("#select_name").html('<h4>მონიშნულია ყველა</h4>').html();
                        $(clone).attr("class", "clone");
                        $(clone).find("#manual").change(function (e) {
                            if (this.checked) {
                                $(this).parents(".clone").find("#selected").css('display', 'none');
                                $(this).parents(".clone").find("#sms_text").css('display', 'block');
                            }
                            else {
                                $(this).parents(".clone").find("#selected").css('display', 'block');
                                $(this).parents(".clone").find("#sms_text").css('display', 'none');
                            }
                        });
                        content.find('#clones_wrapper').append(clone);
                        content.find('input[name="daterange"]').daterangepicker({
                            timePicker: true,
                            timePickerIncrement: 1,
                            locale: {
                                format: 'MM/DD/YYYY HH:mm'
                            }
                        });
                    }
                    else
                    {
                        for (var i = 0; i < towerids.length; i++) {
                            content.find(".datepickers").datePickers({ prefix: "picker" });
                            var clone = content.find('#clone_pattern').clone();
                            $(clone).css('display', 'block');
                            $(clone).attr("id", towerids[i]);
                            $(clone).attr("class", "clone");
                            $(clone).find("#select_name").html(content.find('#' + towerids[i] + '').html());
                            $(clone).find("#manual").change(function (e) {
                                if (this.checked) {
                                    $(this).parents(".clone").find("#selected").css('display', 'none');
                                    $(this).parents(".clone").find("#sms_text").css('display', 'block');
                                }
                                else {
                                    $(this).parents(".clone").find("#selected").css('display', 'block');
                                    $(this).parents(".clone").find("#sms_text").css('display', 'none');
                                }
                            });
                            content.find('#clones_wrapper').append(clone);
                            content.find('input[name="daterange"]').daterangepicker({
                                timePicker: true,
                                timePickerIncrement: 1,
                                locale: {
                                    format: 'MM/DD/YYYY HH:mm'
                                }
                            });
                        }
                    }
                }
                else {
                    content.find("#clones_wrapper").html('');
                }
            }
            ////var div = '<div class="form-group"' + ' style="margin-top:5%; display: block;"' + ' id="' + clickedIndex + '">' + '</div>';
            //if (newValue != undefined)
            //{
            //    selectedAll = false;
            //    clickedIndex = $(this).val()[$(this).val().length - 1];//$(this).find("option:selected").last().attr("value");// clickedIndex + 1;
            //    //var towerid = $(this).find("option:selected").attr("value");
            //    if (newValue == true) {
            //        if (content.find('#height_id').height() < 300) {
            //            resize += 130;
            //            content.find('#height_id').css("height", resize);

            //        }
            //        else {
            //            content.find('#height_id').css("overflow", "auto");
            //        }
            //        if (lennght == content.find("#message_lenght").val() - 1 || lennght == content.find("#message_lenght").val()) {
            //            var _remov = content.find('#clones_wrapper');
            //            for (var i = 0; i <= content.find("#message_lenght").val(); i++) {
            //                _remov.find('#' + i + '').remove();
            //            }
            //            ind = 1;
            //            selectedAll = true;
            //            content.find('#height_id').height('120');
            //            content.find(".datepickers").datePickers({ prefix: "picker" });
            //            var clone = content.find('#clone_pattern').clone();
            //            $(clone).css('display', 'block');
            //            $(clone).find("#select_name").html('<h4>მონიშნულია ყველა</h4>').html();
            //            content.find('#clones_wrapper').append(clone);
            //            content.find('input[name="daterange"]').daterangepicker({
            //                timePicker: true,
            //                timePickerIncrement: 1,
            //                locale: {
            //                    format: 'MM/DD/YYYY HH:mm'
            //                }
            //            });
            //        }
            //        else {
            //            lennght++;

            //            content.find(".datepickers").datePickers({ prefix: "picker" });
            //            var clone = content.find('#clone_pattern').clone();
            //            $(clone).css('display', 'block');
            //            $(clone).attr("id", clickedIndex);
            //            $(clone).find("#select_name").html(content.find('#' + clickedIndex + '').html());
            //            content.find('#clones_wrapper').append(clone);
            //            content.find('input[name="daterange"]').daterangepicker({
            //                timePicker: true,
            //                timePickerIncrement: 1,
            //                locale: {
            //                    format: 'MM/DD/YYYY HH:mm'
            //                }
            //            });
            //        }
            //    }
            //    else {
            //        lennght--;
            //        var _remove = content.find('#clones_wrapper');
            //        if (ind == 1) {
            //            _remove.find('#clone_pattern').remove();
            //            content.find('#height_id').css("overflow", "auto");

            //            lennght = content.find("#team > :selected").size()+1;
                       
            //            content.find("#team > :selected").each(function (index, value) {
                      
            //                index = index + 1;
            //                if (content.find('#height_id').height() < 300) {
            //                    content.find('#height_id').css("height", "350");
            //                    resize = 380;
            //                }
            //                var clone = content.find('#clone_pattern').clone();
            //                $(clone).css('display', 'block');
            //                content.find(".datepickers").datePickers({ prefix: "picker" });
            //                $(clone).attr("id", index);
            //                $(clone).find("#select_name").html(content.find('#' + index + '').html());
            //                content.find('#clones_wrapper').append(clone);
            //                content.find('input[name="daterange"]').daterangepicker({
            //                    timePicker: true,
            //                    timePickerIncrement: 1,
            //                    locale: {
            //                        format: 'MM/DD/YYYY HH:mm'
            //                    }
            //                });
            //                //}
            //            });
            //            ind = 0;
            //        }
            //        _remove.find('#' + clickedIndex + '').remove();
            //        var leng_ht = content.find('#clones_wrapper > div');
            //        if (content.find('#height_id').height() < 300) {
            //            content.find('#height_id').css("overflow", "hidden");
            //        }
            //        if (leng_ht.length < 3) {
            //            resize -= 130;
            //            content.find('#height_id').css("height", resize);
            //        }
            //    }
            //}
        });


        
        content.find(".bs-select-all").on('click', function () { // select-all
            //ind = 1;
            selectedAll = true;
            //var _remove = content.find('#clones_wrapper');
            //for (var i = 0; i <= content.find("#message_lenght").val(); i++) {
            //    _remove.find('#' + i + '').remove();
            //}
            //content.find('#height_id').height('120');

            content.find("#clones_wrapper").html('');

            content.find(".datepickers").datePickers({ prefix: "picker" });
            var clone = content.find('#clone_pattern').clone();
            $(clone).css('display', 'block');
            $(clone).find("#select_name").html('<h4>მონიშნულია ყველა</h4>').html();
            $(clone).attr("class", "clone");
            $(clone).find("#manual").change(function (e) {
                if (this.checked) {
                    $(this).parents(".clone").find("#selected").css('display', 'none');
                    $(this).parents(".clone").find("#sms_text").css('display', 'block');
                }
                else {
                    $(this).parents(".clone").find("#selected").css('display', 'block');
                    $(this).parents(".clone").find("#sms_text").css('display', 'none');
                }
            });
            content.find('#clones_wrapper').append(clone);
            content.find('input[name="daterange"]').daterangepicker({
                timePicker: true,
                timePickerIncrement: 1,
                locale: {
                    format: 'MM/DD/YYYY HH:mm'
                }
            });
        });
        content.find(".bs-deselect-all").on('click', function () { //deselect-all
            $("#clones_wrapper").html('');
            selectedAll = false;
            //content.find('#height_id').height('10');
            //resize = 0;
            //lennght = 0;
            //ind = 0;
            //var _remove = content.find('#clones_wrapper');
            //_remove.find('#clone_pattern').remove();

            //for (var i = 0; i <= content.find("#message_lenght").val(); i++) {
            //    _remove.find('#' + i + '').remove();
            //}
                
            
        });
        var clone_input = content.find("#clones_wrapper");
        clone_input.find('#manual').on('change', function () {
            alert('asa');
    
        });

        content.find('#send').on('click', function () {
            var data = [];
                var foreach = content.find('#clones_wrapper > div');
                var sms_text = "";
                var _checked = false;
                for (var i = 0; i < foreach.length; i++) {
                    
                    var tdate = foreach.find('#picker_picker_to');
                    var dt_from_to = tdate[i];
                    var dtfrom = dt_from_to.value.split(" - ")[0];
                    var dtto = dt_from_to.value.split(" - ")[1];
                    if (foreach.find('#manual')[i].checked == true) {
                        if (foreach.find('#sms_text')[i].value == "") {
                            alert("შეავსეთ ყველა ველი");
                            return;
                        }
                        else {

                        }
                        sms_text = foreach.find('#sms_text')[i].value;
                        _checked = true;
                    }
                    else {
                        var smsID = foreach.find("#selected")[i];
                        //var smsID = select[i];
                        sms_text = smsID.options[smsID.selectedIndex].value;
                        _checked = false;
                    }
                    if (content.find("#message_types").find("input:checked").size() == 0) {
                        alert("აირჩიეთ შეტყობინების ტიპი");
                        return false;
                    }
                    else {
                        var smsType = content.find("#message_types").find("input:checked").val();
                    }
                    if (selectedAll == true) {

                        data.push({ id: 1, selected: selectedAll, smsId: sms_text, dateFrom: dtfrom, dateTo: dtto, smsType: smsType, manual: _checked });
                    }
                    else {
                        data.push({ id: foreach[i].id, selected: selectedAll, smsId: sms_text, dateFrom: dtfrom, dateTo: dtto, smsType: smsType, manual: _checked });
                    }
                
            }

            $.post("/Message/SendMessage",
                {
                    data: data
                },
                function (d) {
                    if (d !="") {
                        location.href = "/Message";
                        alert(d);
                      
                    }
                    else {

                        alert("შეტყობინების გაგზავნა ვერ შესრულდა!");
                    }
                }, "json");
        });
    }
    
    $("#showModal").on("click", function (e) {
        e.preventDefault();
        $.get("/Message/NewMessage", {}, function (data) {
            var content = $(data);
            $.validator.unobtrusive.parse(content.find("#message_form"));
            content.modal("show");

            list = content.find("#abonents_list");
            txt_abonent_select = content.find("#txt_abonent_select");
            drp_abonent_select_by = content.find("#drp_abonent_select_by");
            drp_status = content.find("#drp_status");
            drp_receiver = content.find("#drp_receiver");
            drp_tower = content.find("#drp_tower");
            txt_abonent_num = content.find("#txt_abonent_num");
            finish_date_where = content.find("#finish_date_where");
            finish_date_val = content.find("#finish_date_val");
            pause_date_where = content.find("#pause_date_where");
            pause_date_val = content.find("#pause_date_val");
            credit_date_where = content.find("#credit_date_where");
            credit_date_val = content.find("#credit_date_val");
            balance_where = content.find("#balance_where");
            balance_val = content.find("#balance_val");
            discount_where = content.find("#discount_where");
            discount_val = content.find("#discount_val");
            status_where = content.find("#status_where");
            status_val = content.find("#status_val");
            service_where = content.find("#service_where");
            service_val = content.find("#service_val");
            MessageText = content.find("#MessageText");
            chk_disposable = content.find("#chk_disposable");
            drp_abonent_type = content.find("#drp_abonent_type");

            content.find("#drp_template").on("change", function (e) {
                MessageText.val($(this).find("option:selected").val());
                content.find("#message_template").val($(this).find("option:selected").text());
            });

            content.find("#btn_users_clear").on("click", function (e) {
                e.preventDefault();
                list.empty();
            });

            content.find("input[type=submit]").on("click", function (e) {
                if (content.find("#message_types").find("input:checked").size() == 0) {
                    alert("აირჩიეთ შეტყობინების ტიპი");
                    return false;
                }
                setLog(content);
            });

            content.find("#drp_message_repl").on("change", function () {
                MessageText.val(MessageText.val() + ' [' + $(this).find("option:selected").val() + '] ').focus();
            });

            content.find("#btn_save_auto").on("click", function (ev) {
                ev.preventDefault();
                $.post("/Message/SaveAutoTemplate", {
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
                    abonent_type: drp_abonent_type.find("option:selected").val(),
                    text: MessageText.val(),
                    types: content.find("#MessageType").val(),
                    is_disposable: chk_disposable.prop("checked"),
                    template_name: content.find("#template_name").val()
                }, function (data) {
                    if (data === 1)
                        alert("შენახვა წარმატებით შესრულდა!");
                    else
                        alert("შენახვა ვერ შესრულდა!");
                }, "json");
            });

            content.find("form").each(function (i, form) {
                GeoKBD.map($(form).attr("name"), $.map($(form).find("input[type='text'], textarea"), function (item, k) {
                    return $(item).attr("name");
                }), $(form).get(0));
            });

            drp_abonent_select_by.on("change", function (e) {
                txt_abonent_select.val("").focus();
            });

            content.find("#message_types input").change(function (e) {
                var vals = $.map(
                    content.find("#message_types input:checked"), function (item, i) {
                        return $(item).val()
                    }).join(',');
                content.find("#MessageType").val(vals);
            });

            txt_abonent_select.typeahead({
                source: function (query, process) {

                    var selected_type = drp_abonent_select_by.find("option:selected").val();
                    if (selected_type === "")
                        return null;

                    return $.post("/Message/GetDataSelectBy", { type: selected_type, query: query }, function (response) {
                        var data = [];
                        $.each(response, function (i, name) {
                            data.push(name);
                        })
                        return process(data);
                    }, "json");
                },
                minLength: 2
            });

            content.find("#btn_search").on("click", function (e) {
                e.preventDefault();

                $.post("/Message/FindAbonents", {
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
                    var rows = list.find("li");
                    $.each(data, function (i, val) {
                        var has_row = false;
                        $.each(rows, function (i, row) {
                            if ($(row).data("id") === val.Id) {
                                has_row = true;
                                return false;
                            }
                        });
                        if (!has_row) {
                            list.append('<li class="list-group-item" data-id="' + val.Id + '"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button> <span>' + val.Name + '</span> <input type="hidden" name="AbonentIds[' + (rows.size() + i) + ']" value="' + val.Id + '" /></li>');
                        }
                    });
                }, "json");

            });
            GeoKBD.map("MessageForm", ['abonent_select'], content.find("#MessageForm").get(0));
        });
    });
});

function onSuccessNewMessage(res) {
    if (res === "1") {
        $("#message_modal").modal("hide");
        document.location.href = "/Message";
    }
}

function onAddTemplate(id) {
    $.post("/Message/GetTemplate", { id: id }, function (data) {
        var content = $(data);
        $.validator.unobtrusive.parse(content.find("#template_form"));
        content.modal("show");
        content.on('hidden.bs.modal', function () {
            content.remove();
        });
    }, "html");
}

function onDeleteTemplate(id) {
    if (confirm("წაიშალოს შაბლონი?")) {
        $.post("/Message/DeleteTemplate", { id: id }, function (data) {
            if (data === 1)
                $("#templates_body").find("tr[data-id='" + id + "']").remove();
        }, "json");
    }
}

function onSuccessAddTemplate(res) {
    if (res != null) {
        var modal = $("#template_modal");
        modal.modal("hide");
        has_row = false;
        var tbody = $("#templates_body");
        tbody.find("tr").each(function (i, row) {
            if ($(row).data("id") === res.Id) {
                has_row = true;
                return false;
            }
        });
        if (!has_row) {
            tbody.append('<tr data-id="' + res.Id + '"><td onclick="onAddTemplate(' + res.Id + ')">' + res.Name + '</td><td style="text-align:center;"><span class="glyphicon glyphicon-remove" onclick="onDeleteTemplate(' + res.Id + ')"></span></td></tr>');
        } else {
            tbody.find("tr[data-id='" + res.Id + "']").html('<td onclick="onAddTemplate(' + res.Id + ')">' + res.Name + '</td><td style="text-align:center;"><span class="glyphicon glyphicon-remove" onclick="onDeleteTemplate(' + res.Id + ')"></span></td>');
        }
    }
}

function getTemplates() {
    $.post("/Message/GetTemplates", {}, function (data) {
        var content = $(data);
        content.modal("show");
        content.on('hidden.bs.modal', function () {
            content.remove();
        });
    }, "html");
}

function setLog(content) {
    var arr = [];
    content.find("input[type='text'],select").each(function (i, val) {
        var input = $(val);
        if (input.is("input")) {
            arr.push({ "field": $("label[for='" + input.attr("id") + "']").html(), "old_val": "", "new_val": input.data("log") !== undefined ? content.find("select[data-log='" + input.attr("id") + "']").find("option:selected").val() + " " + input.val() : input.val(), "type": "" });
        } else {
            if (input.data("log") === undefined)
                arr.push({ "field": $("label[for='" + input.attr("id") + "']").html(), "old_val": "", "new_val": input.find("option:selected").text(), "type": "" });
        }
    });
    content.find("#Logging").val(JSON.stringify(arr));
}