﻿var timer = null;
var payments_body;
var drp_filter;
var txt_filter;
var paging;
$(function () {
    $(".datepickers").datePickers({ prefix: "picker" });

    payments_body = $("#payments_body");

    txt_filter = $("#txt_filter");
    drp_filter = $("#drp_filter");
    paging = $("#paging");
    var title = $("#filter_title");

    sub_txt_filter = $("#sub_txt_filter");

    drp_filter.on("change", function (e) {
        txt_filter.focus();

        $("#sub_txt_filter").css('display', 'none');
        $("#sub_txt_filter").val(null);



        if ($(this).find("option:selected").val() == "p.pay_type_id") {


            fcontent = "";
            //$.post("/Payment/getPayTypes", {},
            //function (data) {
            //    if (data) {
            //        for (var i = 0; i < data.length; i++) {
            //            fcontent += '<option value="' + data[i].Id + '">' + data[i].Name + '</option>';
            //        }
            //    }
            //})

            jQuery.ajax({
                type: 'POST',
                url: "/Payment/getPayTypes",
                success: function (data) {
                    if (data) {
                        for (var i = 0; i < data.length; i++) {
                            fcontent += '<option value="' + data[i].Id + '">' + data[i].Name + '</option>';
                        }
                    }
                },
                dataType: "json",
                async: false
            });

            title.data("sign", "select").next().remove();
            title.after('<select class="form-control" id="pay_type" onchange="PayTypeChange(this)">' +
                '<option selected="selected" value="0"></option>' + fcontent + '</select>');

            //title.after('<select class="form-control" id="pay_type" onchange="PayTypeChange(this)">' +
            //    '<option value="0"></option>' +
            //    '<option value="2">ნაღდი</option>' +
            //    '<option value="3">ელექტრონული გადახდა</option>' +
            //    '<option value="4">გატანა</option>' +
            //    '<option value="5">ძვ. ბილინგში გადატანა</option>' +
            //    '<option value="6">სხვა ბარათზე გადატანა</option>' +
            //    '<option value="7">გაურკვეველი თანხის გადატანა</option></select>');


        } else {
            if (title.data("sign") === "select") {
                title.data("sign", "text").next().remove();
                title.after('<input type="search" class="form-control" id="txt_filter" autofocus>');
                $("#txt_filter").keyup(txtfilter_keyup);
                return;
            }
        }

        if ($.trim(txt_filter.val()) != "") {
            filter(txt_filter.val(), 1);
        }
    });

    function txtfilter_keyup(e) {
        filter($(this).val(), 1);
        $(".pagination-container").remove();
    }

    txt_filter.keyup(txtfilter_keyup);

    sub_txt_filter.keyup(function (e) {
        filter($("#pay_type").val(), 1, $(this).val());
        $(".pagination-container").remove();
    });

    paging.on("click", "a", function (e) {
        if (paging.data("mode") === "ajax") {
            e.preventDefault();
            e.stopPropagation();
            filter(txt_filter.val(), $(this).attr("href"));
            filter(title.data("sign") === "text" ? txt_filter.val() : title.next().find("option:selected").val(), $(this).attr("href"));
        }
    });

    $("#payments_body").on("click", "tr", function (e) {

        if (e.target.tagName == 'SPAN') {
            onCancelPayment($(this).data("id"));
            return;
        }
        if ($(e.target).prop("tagName") != "TD")
            return;
        openPayment($(this).data("id"));
    });

    $(".payType").click(function (e) {
        e.preventDefault();
        $.post("/Payment/changePayType/",
            {
                payId: $(this).parent().attr("data-id")
            },
            function (data) {
                var content = $(data);
                content.modal("show");
                content.find("#saveType").click(function () {
                    e.preventDefault();
                    //saveType();
                    var Ok = confirm("ნამდვილად გსურთ გადახდის სახეობის შეცვლა?");
                    if (Ok == true) {
                        var payType = $("#typeSelect").val();
                        var paymentId = $("#payId").text();
                        $.ajax({
                            url: "/Payment/editPayType",
                            data: { "payType": payType, "paymentId": paymentId },
                            type: "post",
                            cache: false,
                            success: function () {
                                content.modal("toggle");
                                location.reload();
                            },
                            error: function () { }
                        });
                    }
                    else { alert("ცვლილება არ განხორციელდა."); }
                });
                content.on('hidden.bs.modal', function () {
                    content.remove();
                });
            }, "html");

    });



    $("#showModal").on("click", function (e) {
        e.preventDefault();
        openPayment(0);
    });

});

function PayTypeChange(select) {
    if ($(select).find("option:selected").val() == 14) {
        $("#sub_txt_filter").css('display', 'block');
        $("#sub_txt_filter").val(null);
    }
    else {
        $("#sub_txt_filter").css('display', 'none');
        $("#sub_txt_filter").val(null);
    }
    filter($(select).find("option:selected").val(), 1);
}

//function saveType() {
//    $.post("/Payment/editPayType/",
//    {
//        payType: $("#typeSelect").val(),
//        paymentId: $("#payId").text()
//    },
//    function (data) {
//        var content = $(data);
//        content.modal("toggle");
//        content.on('hidden.bs.modal', function () {
//            content.remove();
//        });
//    }, "html");
//};

function openPayment(id) {
    $.get("/Payment/NewPayment", { id: id }, function (data) {
        var content = $(data);
        $.validator.unobtrusive.parse(content.find("#payment_form"));
        content.modal("show");
        content.on('hidden.bs.modal', function () {
            content.remove();
        });
        content.find(".dropdown-menu").on("click", "a", function (e) {
            content.find("#find_desc").html($(this).html() + "&nbsp;");
            content.find(".dropdown-toggle").data("sign", $(this).data("type"));
            find_textbox.focus();
        });

        content.find("#btn_return_money").on("click", function (e) {
            e.preventDefault();
            $.post("/Payment/GetReturnMoney", { id: content.find("#Id").val() }, function (data) {

                var btn = $('<a class="btn btn-xs btn-success" data-amount="' + data + '">დასაბრუნებელი თანხა ' + data + ' ლარი</a>');
                btn.on("click", function () {
                    $.get("/Payment/CancelPayment", { card_id: content.find("#cards_area").find("button").data("id"), amount: $(this).data("amount") }, function (dt) {
                        var cancel_modal = showModal(dt);
                        cancel_modal.find("#cards").html('<input id="Cards_0_1" name="Cards[0]" value="' + content.find('#Cards_0_').val() + '" type="hidden">');
                        cancel_modal.find("#cancel_operation").on("click", function (e) {
                            var val = $(this).find("option:selected").val();
                            if (val === "3") {
                                cancel_modal.find("#cards_find_area").show();
                            } else {
                                cancel_modal.find("#cards_find_area").hide();
                                cancel_modal.find("#cards, #cards_area, #card_message, #abonent_area").empty();
                                cancel_modal.find("#cards").html('<input id="Cards_0_1" name="Cards[0]" value="' + content.find('#Cards_0_').val() + '" type="hidden">');
                            }

                            cancel_modal.find("#find").on("click", function (e) {
                                e.preventDefault();
                                onFindCards(cancel_modal);
                            });
                        });
                    }, "html");
                });

                content.find("#return_money_summ").show().html(btn);
                content.find("#Amount").attr("readonly", "readonly").val(data);
            }, "json");
        });

        content.find("form").submit(function (e) {
            if (content.find("#cards").find(":hidden").size() == 0) {
                content.find("#card_message").html("აირჩიეთ ბარათი");
                e.preventDefault();
                return false;
            }
            else {
                content.find("input[type='submit']", this).val("გთხოვთ მოითმინოთ...").attr('disabled', 'disabled');
            }

            //var val = content.find('input[type=file]').val().toLowerCase(); 
            //var regex = new RegExp("(.*?)\.(jpg|jpeg|pdf)$"); 
            //if (!(regex.test(val))) { 
            //    alert('არასწორი ფაილი!!');
            //    e.preventDefault();
            //    return false;
            //}

            setFieldsChange($(this).find("input[data-tag], select[data-tag]"));
            $(this).prop("disabled", true);
        });

        content.find("#find").on("click", function (e) {
            e.preventDefault();
            onFindCards(content);
        });
    }, "html");
}

function onFindCards(content) {
    var balance = 0;
    var mini_pric = 0;
    var rent_balance = 0;
    var mini_rent_pric = 0;
    var mini = 0;
    $.post("/api/data/getcards", { type: content.find("#find_by").find("option:selected").val(), value: content.find("#payment_find").val(), secure: generateSecure() }, function (data) {
        var buttons = '';
        var buttonsRent = '';
        content.find("#cards, #cards_area,#cards_rent, #card_message, #abonent_area").empty();
        $.each(data.groups, function (i, group) {
            var id = '';
            var txt = '';
            $.each(group.cards, function (j, card) {
                txt += '<p>' + card.name.split() + '</p>';
                id += "," + card.id;
                balance = card.balance;
                mini_pric = card.min_price;
                if (card.rent_balance < card.rent_min_price) {
                    mini_rent_pric = card.rent_balance - card.rent_min_price;
                    mini -= card.rent_min_price;
                }
                content.find('#amount_block').css("display", "block");
                content.find('#amount_rent_block').css("display", "block");
                content.find('#min_amount').html("მინ.გად: " + card.min_price.toFixed(2));
                content.find('#balance_amount').html("ბალანსი: " + card.balance.toFixed(2));
                content.find('#min_amount').val(card.min_price.toFixed(2));
                content.find('#balance_amount').val(card.balance.toFixed(2));
                if (card.balance < 0 && card.status != "იჯარა" ) {
                    content.find('#min_rent_amount').html("მინ.გად: " + (-1 * (card.balance + mini_rent_pric)));
                    content.find('#min_rent_amount').val(card.balance + mini_rent_pric);

                    mini_rent_pric += card.balance;
                }
                else {
                    if (card.status == "აქტიური" || card.status == "იჯარა") {
                        content.find('#min_rent_amount').html("მინ.გად: " + 0);
                        content.find('#min_rent_amount').val((0).toFixed(2));
                        mini_rent_pric = 0;
                        mini = 0;
                    }
                    else {
                        content.find('#min_rent_amount').html("მინ.გად: " + (-1 * (mini_rent_pric)).toFixed(2));
                        content.find('#min_rent_amount').val((mini_rent_pric).toFixed(2));
                    }
                }
                content.find('#balance_rent_amount').html("ბალანსი: " + card.rent_balance);
                content.find('#balance_rent_amount').val(card.rent_balance);
                rent_balance = card.rent_balance;
            });
            buttons += '<button type="button" class="btn btn-default btn-sm" data-id="' + id.substr(1, id.length) + '">' + txt + '</button>';
            buttonsRent += '<button type="button" class="btn btn-default btn-sm" data-id="' + id.substr(1, id.length) + '">' + "იჯარა-" + txt + '</button>';
        });
        if (data.abonent.name != null)
            content.find("#abonent_area").html('<div class="alert alert-success" role="alert">' + data.abonent.name + '</div>');
        else
            content.find("#abonent_area").html('<div class="alert alert-danger" role="alert">ბარათი ვერ მოიძებნა</div>');

        var area = content.find("#cards_area")
        area.html(buttons).on("click", "button", function (ev) {
            ev.preventDefault();
            var btn = $(this);
            if (btn.hasClass('btn btn-sm active btn-primary')) {
                btn.attr("class", "btn btn-sm btn-default");

                content.find("#cards").html("");
                return;
            }
            if (area_rent.find("button").hasClass('btn btn-sm active btn-primary')) {
                area_rent.find("button").attr("class", "btn btn-sm btn-default");
                content.find("#_rent").html("");
                content.find("#cards").html("");
                //return;
            }
            area.find("button").attr("class", "btn btn-sm btn-default");

            btn.attr("class", "btn btn-sm active btn-primary");
            var str = '';
            $.each(btn.data("id").toString().split(','), function (i, val) {
                str += '<input id="Cards_' + i + '_" name="Cards[' + i + ']" value="' + val + '" type="hidden">';
            });
            content.find("#cards").html(str);
        });

        var area_rent = content.find("#cards_rent")
        area_rent.html(buttonsRent).on("click", "button", function (ev) {
            ev.preventDefault();
            var btnr = $(this);
            if (btnr.hasClass('btn btn-sm active btn-primary')) {
                btnr.attr("class", "btn btn-sm btn-default");
                content.find("#_rent").html("");
                content.find("#cards").html("");
                return;
            }
            if (area.find("button").hasClass('btn btn-sm active btn-primary')) {
                area.find("button").attr("class", "btn btn-sm btn-default");
                content.find("#_rent").html("");
                content.find("#cards").html("");
                //return;
            }
            area_rent.find("button").attr("class", "btn btn-sm btn-default");

            btnr.attr("class", "btn btn-sm active btn-primary");

            var str = '';
            $.each(btnr.data("id").toString().split(','), function (i, val) {
                str += '<input id="Cards_' + i + '_" name="Cards[' + i + ']" value="' + val + '" type="hidden">';
            });
            var str_rent = '<input name="PayRent" id="PayRent" value="1" type="hidden">';
            content.find("#_rent").html(str_rent);
            content.find("#cards").html(str);
        });
    }, "json");

    content.find('#Amount').bind('input', function (e) {
        if ($(this).val() == "") {
            content.find('#min_amount').html("მინ.გად: " + (mini_pric).toFixed(2));
            content.find('#balance_amount').html("ბალანსი: " + (balance).toFixed(2));
            return;
        }
        if ((mini_pric - jQuery.parseJSON($(this).val())) > 0) {
            content.find('#min_amount').html("");
            content.find('#min_amount').html("მინ.გად: " + (mini_pric - jQuery.parseJSON($(this).val())).toFixed(2));


        }
        else {
            content.find('#min_amount').html("მინ.გად: " + 0);
            content.find('#min_rent_amount').html("მინ.გად: " + mini);
            mini_rent_pric = mini;

        }
        content.find('#balance_amount').html("");
        var _balanc = balance + jQuery.parseJSON($(this).val());
        content.find('#balance_amount').html("ბალანსი: " + (_balanc).toFixed(2));
        if (_balanc < 0) {
            mini_rent_pric = mini_rent_pric + jQuery.parseJSON($(this).val());
            content.find('#min_rent_amount').html("მინ.გად: " + (mini_rent_pric).toFixed(2));
        }
    });
    content.find('#RentAmount').bind('input', function (e) {
        if ($(this).val() == "") {
            content.find('#min_rent_amount').html("მინ.გად: " + (mini_rent_pric).toFixed(2));
            content.find('#balance_rent_amount').html("ბალანსი: " + (rent_balance).toFixed(2));
            content.find('#min_amount').html("მინ.გად: " + (mini_pric).toFixed(2));
            content.find('#balance_amount').html("ბალანსი: " + (balance).toFixed(2));
            return;
        }
        if ((mini_rent_pric - jQuery.parseJSON($(this).val())) > 0 && jQuery.parseJSON($(this).val())!=0) {
            content.find('#min_rent_amount').html("");
            content.find('#min_rent_amount').html("მინ.გად: " + (mini_rent_pric - jQuery.parseJSON($(this).val())).toFixed(2));
            ontent.find('#balance_amount').html("ბალანსი: " + (0).toFixed(2));
            content.find('#min_amount').html("მინ.გად: " + (mini_pric - _balanc).toFixed(2));
        }
        else {

            content.find('#balance_rent_amount').html("");
            var _balanc = (rent_balance + jQuery.parseJSON($(this).val()));
            content.find('#balance_rent_amount').html("ბალანსი: " + (_balanc).toFixed(2));
            if ((mini_rent_pric + _balanc) >= 0)
                content.find('#min_rent_amount').html("მინ.გად: " + 0);
            else {
                if (jQuery.parseJSON($(this).val()) != 0) {
                    content.find('#min_rent_amount').html("მინ.გად: " + (mini_rent_pric + _balanc));
                }
            }
            if (balance<0) {
                if (balance + _balanc >= 0) {
                    content.find('#balance_amount').html("ბალანსი: " + (0).toFixed(2));

                    content.find('#min_amount').html("მინ.გად: " + (mini_pric + balance).toFixed(2));
                    mini_pric = mini_pric + balance;
                    balance = 0;
                }
                else {
                    content.find('#balance_amount').html("ბალანსი: " + (balance + _balanc).toFixed(2));
                    content.find('#min_amount').html("მინ.გად: " + (mini_pric - _balanc).toFixed(2));
                }
            }
        }

    });

}
function filter(letter, page, info) {
    if (info === undefined) info = null;
    if (page === undefined && (letter == "" || letter == null)) return;
    if (timer) {
        clearTimeout(timer);
    }

    timer = setTimeout(function () {
        var col = drp_filter.find("option:selected").val();
        if ((col === "c.doc_num" || col === "p.pay_type_id") || letter.length > 2) {
            $.post("/Payment/FilterPayments",
                {
                    letter: letter, column: col, page: page, info: info,
                    dt_from: $("#picker_picker_from").val().ReplaceAll("/", ""),
                    dt_to: $("#picker_picker_to").val().ReplaceAll("/", "")
                },
                function (data) {
                    var str = '';

                    //original code
                    //$.each(data.Payments, function (i, val) {
                    //    str += '<tr data-id="' + val.Id + '">' +
                    //        '<td>' + parseJsonDate(val.Date).toDateString() + '</td><td>' + val.AbonentName + '</td>' +
                    //        '<td>' + val.AbonentNum + '</td><td>' + val.CardNum + '</td><td>' + val.PayType + '</td><td>' + val.Amount + '</td><td style="text-align:center;">' + (val.FileName === '' ? '' : '<a target="_blank" href="ftp://' + data.FilePath + val.FileName + '"><span class="glyphicon glyphicon-file" aria-hidden="true"></span></a>') + '</td></tr>';
                    //});

                    $.each(data.Payments, function (i, val) {
                        str += '<tr data-id="' + val.Id + '">' +
                            '<td>' + parseJsonDate(val.Date).toDateString() + '</td><td>' + val.AbonentName + '</td>' +
                            '<td>' + val.AbonentNum + '</td><td>' + val.CardNum + '</td><td>' + val.Amount + '</td><td>' + val.UserName + '</td><td>' + val.PayType + '</td><td style="text-align:center;">' + val.FileName + '</td></tr>';
                    });

                    //$.each(data.Payments, function (i, val) {
                    //    str += '<tr data-id="' + val.Id + '">' +
                    //        '<td>' + parseJsonDate(val.Date).toDateString() + '</td><td>' + val.AbonentName + '</td>' +
                    //        '<td>' + val.AbonentNum + '</td><td>' + val.CardNum + '</td><td>' + val.Amount + '</td><td>' + val.UserName + '</td><td>' + val.PayType + '</td></tr>';
                    //});

                    payments_body.html(str);
                    paging.data("mode", "ajax").html(data.Paging);
                }, "json");
        }
    }, 300, letter);
}

Date.prototype.toSecureDate = function () {
    return this.getFullYear() + "-" + ((this.getMonth() + 1).toString().length === 1 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1)) + "-" + (this.getDate().toString().length === 1 ? "0" + this.getDate() : this.getDate()) + " " + (this.getHours().toString().length === 1 ? "0" + this.getHours() : this.getHours()) + ":" + (this.getMinutes().toString().length === 1 ? "0" + this.getMinutes() : this.getMinutes()) + ":" + (this.getSeconds().toString().length === 1 ? "0" + this.getSeconds() : this.getSeconds());
}

function generateSecure() {
    var dt = new Date();
    return { "date": dt.toSecureDate(), "key": md5(dt.toSecureDate() + (dt.getFullYear() + (dt.getMonth() + 1) + dt.getDate() + dt.getHours() + dt.getMinutes() + dt.getSeconds()).toString()) };
}
