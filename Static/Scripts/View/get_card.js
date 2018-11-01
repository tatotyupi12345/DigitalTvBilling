var timer = null;
var payments_body;
var drp_filter;
var txt_filter;
var paging;
$(function () {

    $("#showModal").on("click", function (e) {
        e.preventDefault();
        openPayment(0);
        var p = $(e).parents("#cards");
    });

    jQuery.fn.exists = function () { return this.length > 0; }
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

function openPayment(id) {
    $.get("/Abonent/GetExistingCard", { id: id }, function (data) {
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

        //content.find("#btn_return_money").on("click", function (e) {
        //    e.preventDefault();
        //    $.post("/Payment/GetReturnMoney", { id: content.find("#Id").val() }, function (data) {

        //        var btn = $('<a class="btn btn-xs btn-success" data-amount="' + data + '">დასაბრუნებელი თანხა ' + data + ' ლარი</a>');
        //        btn.on("click", function () {
        //            $.get("/Payment/CancelPayment", { card_id: content.find("#cards_area").find("button").data("id"), amount: $(this).data("amount") }, function (dt) {
        //                var cancel_modal = showModal(dt);
        //                cancel_modal.find("#cards_info").html('<input id="Cards_0_1" name="Cards[0]" value="' + content.find('#Cards_0_').val() + '" type="hidden">');
        //                cancel_modal.find("#cancel_operation").on("click", function (e) {
        //                    var val = $(this).find("option:selected").val();
        //                    if (val === "3") {
        //                        cancel_modal.find("#cards_find_area").show();
        //                    } else {
        //                        cancel_modal.find("#cards_find_area").hide();
        //                        cancel_modal.find("#cards_info, #cards_area, #card_message, #abonent_area").empty();
        //                        cancel_modal.find("#cards_info").html('<input id="Cards_0_1" name="Cards[0]" value="' + content.find('#Cards_0_').val() + '" type="hidden">');
        //                    }

        //                    cancel_modal.find("#find").on("click", function (e) {
        //                        e.preventDefault();
        //                        onFindCards(cancel_modal);
        //                    });
        //                });
        //            }, "html");
        //        });

        //        content.find("#return_money_summ").show().html(btn);
        //        content.find("#Amount").attr("readonly", "readonly").val(data);
        //    }, "json");
        //});

        content.find("form").submit(function (e) {
            if (content.find("#cards_info").find(":hidden").size() == 0) {
                content.find("#card_message").html("აირჩიეთ ბარათი");
                e.preventDefault();
                return false;
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

        content.find("#choosebtn").on("click", function (e) {
            if (content.find("#cards_info").find(":hidden").size() == 0) {
                content.find("#card_message").html("აირჩიეთ ბარათი");
                e.preventDefault();
                return false;
            }

            //var val = content.find('input[type=file]').val().toLowerCase(); 
            //var regex = new RegExp("(.*?)\.(jpg|jpeg|pdf)$"); 
            //if (!(regex.test(val))) { 
            //    alert('არასწორი ფაილი!!');
            //    e.preventDefault();
            //    return false;
            //}

            //setFieldsChange($(this).find("input[data-tag], select[data-tag]"));
            //$(this).prop("disabled", true);

            var ids = [];

            for (var i = 0; ; i++)
            {
                if ($("input[name='Cards[" + i + "]']").exists())
                {
                    ids.push($("input[name='Cards[" + i + "]']").val());
                }
                else
                    break;
            }

            //id = $('#Cards_id').val();
            if(ids)
            {
                getCardInfo(ids, content);
            }
        });

        content.find("#find").on("click", function (e) {
            e.preventDefault();
            onFindCards(content);
        });
    }, "html");
}

function onFindCards(content) {
    $.post("/api/data/getcards", { type: content.find("#find_by").find("option:selected").val(), value: content.find("#payment_find").val(), secure: generateSecure() }, function (data) {
        var buttons = '';
        content.find("#cards_info, #cards_area, #card_message, #abonent_area").empty();
        $.each(data.groups, function (i, group) {
            var id = '';
            var txt = '';
            $.each(group.cards, function (j, card) {
                txt += '<p>' + card.name + '</p>';
                id += "," + card.id;
            });
            buttons += '<button type="button" class="btn btn-default btn-sm" data-id="' + id.substr(1, id.length) + '">' + txt + '</button>';
        });
        if (data.abonent.name != null)
            content.find("#abonent_area").html('<div class="alert alert-success" role="alert">' + data.abonent.name + '</div>');
        else
            content.find("#abonent_area").html('<div class="alert alert-danger" role="alert">ბარათი ვერ მოიძებნა</div>');
        var area = content.find("#cards_area")
        area.html(buttons).on("click", "button", function (ev) {
            ev.preventDefault();
            var btn = $(this);
            area.find("button").attr("class", "btn btn-sm btn-default");
            btn.attr("class", "btn btn-sm active btn-primary");
            var str = '';
            $.each(btn.data("id").toString().split(','), function (i, val) {
                //str += '<input id="Cards_' + i + '_" name="Cards[' + i + ']" value="' + val + '" type="hidden">';
                str += '<input id="Cards_id" name="Cards[' + i + ']" value="' + val + '" type="hidden">';
            });
            content.find("#cards_info").html(str);
        });

    }, "json");
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

function getCardInfo(ids, content)
{

    if (!ids)
        return;

    $("#cards").append("<input id='' name='is_card_forward' value='true' type='hidden'>");

    for (var i = 0; i < ids.length; i++) {

        if ($("input[name='Cards[" + i + "].Id']").exists())
        {
            $("input[name='Cards[" + i + "].Id']").val(ids[i]);
        }
        else
        $("#cards").append("<input id='' name='Cards[" + i + "].Id' value='"+ids[i]+"' type='hidden'>");
        
    }

    setFieldsNew($("#abonent_panel, #cards_panel").find("input, textarea, select"));

    $("#abonent_form")[0].submit(function (event) {
        
        alert("Handler for .submit() called.");
        event.preventDefault();
        content.modal("hide");
    });

    //alert(id);
    //$.ajax({
    //    type: 'POST',
    //    url: "getCardsInfo",
    //    data: id,
    //    success: function(result) {
    //        if(result.isOk == false)
    //            alert(result.message);
    //    }
    //    //dataType: "json",
    //    //async: false
    //});

    //$.post("/Abonent/getCardsInfo", { ids: ids }, function (data) {
    //    if (data)
    //    {
    //        //alert(data);

    //        //$("#Cards_0__CardNum").val(data[0].CardNum);
    //        //$("#Cards_0__City").val(data[0].City);
    //        //$("#Cards_0__Address").val(data[0].Address);
    //        //$("#Cards_0__ReceiverId").val(data[0].ReceiverId);
    //        //$("#Cards_0__CardStatus").val(data[0].CardStatus);
    //        //$("#Cards_0__Discount").val(data[0].Discount);
    //        //$("#Cards_0__Group").val(data[0].Group);
    //        //$("#Cards_0__HasFreeDays").val(data[0].HasFreeDays);

    //        //content.modal("hide");
            


    //        //$("#abonent_form").submit(function () {
    //        //    alert("Submitted");
    //        //});

    //        $("#abonent_form")[0].submit(function (event) {
    //            alert("Handler for .submit() called.");
    //            event.preventDefault();
    //            content.modal("hide");
    //        });
    //    }
    //});
}