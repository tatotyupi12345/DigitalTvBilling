var card_num_valid = null;
var abonent_num_valid = null;
var card_error = '';
var abonent_error = '';
var isRepeat = false;
var timer;
$(function () {

    var $loader = $("#dilersWrapper");
    $("#diler_code_word").keyup(function ()
    {
        //alert($(this).val());
        filterDilers($(this).val());
        
    });

    function filterDilers(letters)
    {
        if (timer) {
            clearTimeout(timer);
        }
        //$("#attachmentsWrapper").hide();
        if(letters.length >= 3)
        {
            $("#dilersWrapper").empty();
            $loader.gSpinner("hide");
            $loader.gSpinner({ scale: .3 });
            timer = setTimeout(function () {
                //$.ajax({
                //    url: "/Books/getUserDetails",
                //    type: 'GET',
                //    data: { filter: letters },
                //    success: function(result){
                //        $("#dilersWrapper").empty();
                //        $("#dilersWrapper").html(result);
                //    },
                //    dataType: "json"
                //});
                
                $.get("/Books/getUserDetails", { filter: letters }, function (data, status) {
                    //alert("Data: " + data + "\nStatus: " + status);
                    $loader.gSpinner("hide");
                    $("#dilersWrapper").html(data);
                    var dat = eval(data);
                    //$("#attachmentsWrapper").show(200);
                    
                });
            }, 300, letters);
        }
        else
        {
            $("#dilersWrapper").empty();
            $loader.gSpinner("hide");
        }
    }

    $(':checkbox').checkboxpicker();
    $(".diler_filter").hide();
    $('#isFromDiler').checkboxpicker();
    $('#isFromDiler').on('change', function () {
        if ($(this).prop("checked") == true)
        {
            $(".diler_filter").show(200);
            $("#isFromDiler_").val('true');
        }
        else
        {
            $(".diler_filter").hide(200);
            $("#dilersWrapper").empty();
            $("#diler_code_word").val('');

            $("#isFromDiler_").val('false');
        }
    });

    $("form").each(function (i, form) {
        //GeoKBD.map($(form).attr("name"), $.map($(form).find("input[type='text'], textarea"), function (item, k) { return $(item).attr("name") }));
    });

    $("#btn_code").on("click", function (e) {
        e.preventDefault();
        if (isRepeat)
            return;
        $.post("/Abonent/GetCustomerByCode", { code: $("#Customer_Code").val() }, function (data) {
            if ($("#Customer_Type").find("option:selected").val() === "Juridical") {
                $("#Customer_Name").val(data.name);
            } else {
                $("#Customer_Name").val(data.name.split(' ')[0]);
                $("#Customer_LastName").val(data.name.split(' ')[1]);
            }
        }, "json");
    });

    $("#Customer_Code").blur(function (e) {
        var input = $(this);
        $.post("/Abonent/AbonentCheckCode", { code: input.val() }, function (res) {
            if (res !== "") {
                isRepeat = true;
                input.parent().next().removeClass("field-validation-valid").addClass("field-validation-error").html('<span>' + res + '</span>');
            } else {
                isRepeat = false;
            }
        }, "json")
    });

    $("#txt_date").datepicker({
        autoclose: true,
        language: "ka",
        format: 'dd/mm/yyyy'
    }).on("changeDate", function (ev) {
        $(this).datepicker("hide");
    });

    $("#card_add").on("click", function (e) {
        var btn = $(this);
        $.post("/Abonent/AddCard", { count: $("#cards").find(".well").size() }, function (data) {
            $(document).scrollTop($(document).height());
            var content = $(data);
            btn.before(content);
            $.validator.unobtrusive.parseDynamicContent(content);
            var timeout;
            content.find(".selectpicker").selectpicker();
            content.find("input[id$='__City']").each(function () {
                var index = $(this).attr("id").substr(6, 1);
                $(this).typeahead({
                    source: function (query, process) {
                        if (timeout) {
                            clearTimeout(timeout);
                        }

                        timeout = setTimeout(function () {
                            return $.post('/Abonent/GetCities/', { query: query }, function (data) {
                                process(data);
                            }, "json");
                        }, 200);
                    },
                    updater: function (item) {
                        var sp = item.split('-');
                        $.post('/Abonent/GetRegion/', { city: $.trim(sp[0]), raion: $.trim(sp[1]) }, function (data) {
                            if (data != "") {
                                $("#Cards_" + index + "__City").val($.trim(sp[0]));
                                $("#Cards_" + index + "__Village").val($.trim(sp[1]));
                                $("#Cards_" + index + "__Region").val(data.region);
                            }
                        }, "json");
                        return item;
                    }
                });
            });

        }, "html");
    });

    $("#Customer_Type").on("change", function () {
        if ($(this).find("option:selected").val() === "Juridical") {
            $("#juridical, #budget").show();
            $("#last_name").hide().find("#Customer_LastName").val("");
            $("#Customer_JuridicalType").find("option:selected").val("0");
            $("label[for='Customer_Code']").html("ს/კ:");
            var dt = new Date();
            $("#Customer_JuridicalFinishDate").val(dt.toDateStringJuridical());
        } else {
            $("#juridical, #budget").hide();
            $("#last_name").show().find("#Customer_LastName").val("");
            $("#Customer_JuridicalType").find("option").eq(0).val("99");
            $("label[for='Customer_Code']").html("პ/ნ:");
            $("#Customer_JuridicalFinishDate").val("");
        }
    });

    $("form").submit(function (e) {
        if (card_num_valid != null || abonent_num_valid != null) {
            if (card_num_valid != null)
                card_num_valid.closest("div[class='col-md-8']").find("span").removeClass("field-validation-valid").addClass("field-validation-error").html('<span>' + card_error + '</span>');
            if (abonent_num_valid != null)
                abonent_num_valid.closest("div[class='col-md-8']").find("span").removeClass("field-validation-valid").addClass("field-validation-error").html('<span>' + abonent_error + '</span>');
            e.preventDefault();
        }
    });

});

function onAddCardService(index) {
    $.post("/Abonent/GetServicesList", {}, function (services) {
        var services_content = $(services);
        services_content.modal("show");
        services_content.on('hidden.bs.modal', function () {
            services_content.remove();
        });
        services_content.find("#services_add").on("click", function (e) {
            e.preventDefault();
            var services_data = $("#services_data_" + index);
            var str = '';
            var ind = 0;
            var res_str = '';
            services_content.find("#services_body tr").each(function (i, item) {
                if ($(item).find("input[type='checkbox']").prop("checked")) {
                    str += '<input type="hidden" name="Cards[' + index + '].CardServices[' + ind + '].ServiceId" value="' + $(item).data("id") + '" />' +
                        '<input type="hidden" name="Cards[' + index + '].CardServices[' + ind + '].Amount" value="' + $(item).find("input[type='text']").eq(0).val() + '" />' +
                        '<input type="hidden" name="Cards[' + index + '].CardServices[' + ind + '].PayType" value="' + $(item).find("select option:selected").val() + '" />';

                    res_str += '<li class="list-group-item">' + $.trim($(item).find("td:nth-child(2)").html()) + '</li>';
                    ind++;
                }
            });
            
            var res_modal = $("#serv_res_modal");
            res_modal.find("ul").html(res_str);
            res_modal.modal("show");
            res_modal.find("#services_save").on("click", function (ev) {
                services_data.html(str);
                services_content.modal("hide");
            });
        });

    }, "html");
}

function add_subscription(link, index) {
    $.get("/Abonent/AddSubscription/", { id: 0, type: $("#Customer_Type").find("option:selected").val() }, function (data) {
        var content = showModal(data);
        content.find("#add_new_subscrb").on("click", function (e) {
            e.preventDefault();
            var str = '';
            $(link).closest("div").find("input[type='hidden']").remove();
            $.each(content.find("input:hidden[id^='id_']"), function (i, val) {
                $(link).before('<input type="hidden" name="Cards[' + index + '].Subscribtions[0].SubscriptionPackages[' + i + '].PackageId" value="' + $(val).val() + '" />');
                str += '+' + $(val).data("name");
            });
            $(link).closest("div.well").find(".packets").html(str === '' ? '' : str.substr(1));
            content.modal("hide");
        });
    }, "html");
}

function onSuccessAddSubscription(res) {
    if (res === "1") {
        $("#addsubscr_modal").modal("hide");
    }
}

function CardCheck(input, card_id) {
    //if($(input).prop('readonly') === true)
    //    return;
    //var b = true;

    var type = $(input).attr("id").split('__')[1];
    //$("input[id$='__" + type + "']").each(function (i, val) {

    //    if ($(val).attr("id") !== $(input).attr("id") && $(val).val() === $(input).val() || AbonentCancled == true) {
    //        var res = '';
    //        if (type === "CardNum") {
    //            var res = 'ბარათის № უკვე არსებობს';
    //            card_num_valid = $(input);
    //            card_error = res;
    //        } 
    //        else {
    //            var res = 'აბონენტის № უკვე არსებობს';
    //            abonent_num_valid = $(input);
    //            abonent_error = res;
    //        }
    //        $(input).closest("div[class='col-md-8']").find("span[class^='field-validation']").removeClass("field-validation-valid").addClass("field-validation-error").html('<span>' + res + '</span>');
                
    //        b = false;
    //        return false;
    //    }
    //});

    //if (!b)
    //    return;

    $.post("/Abonent/CardCheckCode", { code: $(input).val(), type: type, id: card_id }, function (res) {
        if (type === "CardNum")
            card_error = res;
        else
            abonent_error = res;
        if (res !== "") {
            $(input).closest("div[class='col-md-8']").find("span[class^='field-validation']").removeClass("field-validation-valid").addClass("field-validation-error").html('<span>' + res + '</span>');
            if (type === "CardNum")
                card_num_valid = $(input);
            else
                abonent_num_valid = $(input);
        } else {
            if (type === "CardNum")
                card_num_valid = null;
            else
                abonent_num_valid = null;
        }
    }, "json");
}

function CardAddressCopy(btn, index) {
    $(btn).closest(".well").find("#Cards_" + index + "__Address").val($("#Customer_Address").val());
    return false;
}

function CardAbonentNumGenerate(btn, index) {
    var s = $("#cards_panel").find("input[id$='__Id']").filter(function (index) {
        return $(this).val() !== "0";
    }).size();
    $.post("/Abonent/CardAbonentNumGenerate", { index: index - s }, function (res) {
        $(btn).closest(".well").find("#Cards_" + index + "__AbonentNum").val(res);
    }, "json");
    return false;
}
