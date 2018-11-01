$(function () {

    $('#new_register').on('change', function () {
        if ($(this).prop("checked") == true) {
            $.post("/Abonent/newReRegistering/",
                {
                    code: $("#Customer_Code").val()

                },
                function (data) {
                    if (data == 0) {
                        alert("გადაფორმება წარმატების შესრულდა");
                    } else {
                        alert("გადაფორმება ვერ მოხერხდა!");
                    }

                },"html");
        }
        else {
            alert(1);
        }
    });
    //$('#registering').click(function () {
    //    $.post("/Abonent/HistoryReRegistering/",
    //        {
    //            code: $("#Customer_Code").val()

    //        },
    //        function (data) {
    //            var content = $(data);
    //            content.find(".selectpicker").selectpicker();
    //            content.modal("show");
    //            content.on('hidden.bs.modal', function () {
    //                content.remove();
    //            });
    //            //histryReRegistering(content)
    //        }, "html");
    //});
    $("#new_attachment").on("click", function (e) {
        e.preventDefault();
        var customer_attach_code = $("#Customer_Code").val();
        $.post("/CustomerAttach/GetAttachsInfoEdit/",
            {
                code: customer_attach_code
            },

            function (data) {
                var content = $(data);
                content.find(".selectpicker").selectpicker();
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                    //location.href = location.href;
                });
                initNewAttachContent(content);
            }, "html");
    });
    function initNewAttachContent(content) {
        //content.find('#team').change(function () {

        //    alert(0);
        //});
        content.find(':checkbox').checkboxpicker();
        content.find("#team").on('change', function (e, clickedIndex, newValue, oldValue) {
            $.post("/CustomerAttach/GetCode", {}, function (data) {
                var contentCode = $(data);
                contentCode.modal("show");
                contentCode.on('hidden.bs.modal', function () {
                    contentCode.remove();

                });
                contentCode.find("#diler_code_edit").keyup(function () {
                    //alert($(this).val());
                    filterDilers_edit_user($(this).val(), contentCode, content);

                });
                //content.find(".diler_filter_edit").show(200);
            });
        });
        var k = 1;
        content.find(".item_each").each(function () {
            var id_item = (content.find(this).data("val"));
            var dataHtml = '<br /> <button type="button" id="save_user" class="btn btn-success" data-val=' + k + ' value=' + id_item + ' style="margin-top: 1%;margin-right:35%; ">შენახვა</button>';// "<br /><button type=\"button\" id=\"user_save\" value=\"@id_item\" class=\"btn btn-default btn-success\" style=\"margin-top: 1%;\">შენახვა</button>";
            content.find('#add_button_' + k + '').last().append(dataHtml);
            k++;
        });
        content.find("#diler_code_edit").keyup(function () {
            //alert($(this).val());
            filterDilers_edit_user($(this).val(), content);

        });
        content.find('#save_user').on("click", function (e) {
            e.preventDefault();
            var attachmentVals = [];
            var i = 0;
            var temporarily = 0;
            if (content.find("#Temporarily").prop("checked") == true) {
                temporarily = 1;
            }
            var id_each = (content.find(this).data("val"));
            var diller_id = content.find(this).val();
            var code = $("#Customer_Code").val();
            var Date_time = content.find("#date_diler_" + id_each).attr('data-val');

            content.find(".foreach_" + id_each).each(function () {
                var _id = i.toString() + diller_id.toString();
                var id = parseInt(_id);
                var count_ = content.find("#add_button_" + id_each + " #attachment_" + _id).val();
                var ID_ = content.find("#add_button_" + id_each + " #attachment_" + _id).attr('data-val');
                attachmentVals.push({ id: ID_, count: count_ });
                i++;
            });

            $.post("/CustomerAttach/UpdateEntryEdit",
                {
                    attachmentVals: attachmentVals,
                    code: code,
                    diller_id: diller_id,
                    customer_id: 0,
                    date: Date_time,
                    temporarily: temporarily
                },
                function (data) {
                    if (data === 1) {
                        location.href = location.href;
                    }
                    else {
                        alert("შეცდომა: მონაცემები ვერ შეიცვალა!");
                    }
                }, "json");
        });

        content.find('#save').on("click", function (e) {
            e.preventDefault();
            var attachmentVals = [];
            var i = 0;
            var select = document.getElementById("team");
            var diler = select.options[select.selectedIndex].value;
            var code = $("#Customer_Code").val();
            var temporarily = 0;
            if (content.find("#Temporarily").prop("checked") == true) {
                temporarily = 1;
            }
            while (typeof (content.find("#attachments_" + i).attr('data-val')) != 'undefined') {
                var count_ = content.find("#attachments_" + i).val();
                var ID_ = content.find("#attachments_" + i).attr('data-val');

                attachmentVals.push({ id: ID_, count: count_ });
                i++;
                //attachments[@i].Value
            }

            $.post("/CustomerAttach/NewEntryEdit",
                {
                    attachmentVals: attachmentVals,
                    code: code,
                    diler_id: diler,
                    temporarily : temporarily
                },
                function (data) {
                    if (data === 1) {
                        location.href = location.href;
                    }
                    else {
                        alert("შეცდომა: მონაცემები ვერ შეიცვალა!");
                    }
                }, "json");
        });
    }

    var $loader = $("#dilersWrapper_edit");
    function filterDilers_edit_user(letters, contentCode, content) {
        if (letters.length >= 3) {

            var select = document.getElementById("team");
            var diler = select.options[select.selectedIndex].value;
            $.post("/CustomerAttach/GetUsersFilter", { filter: letters, diler_id: diler }, function (data) {

                if (data == 0) {
                    contentCode.find('#_label_true_false').text('კოდური სიტყვა არასწორია').css("color", "red");
                    content.find(".for_hiden").each(function () {
                        content.find(this).attr('disabled', 'disabled');
                    });
                    content.find('#save').attr('disabled', 'disabled');
                    //alert(0);
                }
                else {
                    contentCode.find('#_label_true_false').text('კოდური სიტყვა სწორია').css("color", "green");
                    content.find(".for_hiden").each(function () {
                        content.find(this).removeAttr('disabled');
                    });
                    content.find('#save').removeAttr('disabled');
                    contentCode.modal('hide');

                }
            });
        }
    }
    $('button#return_card_modal').click(function (event) {
        event.preventDefault();
        $.post("/Abonent/GetReturnedCard", { card_id: $(this).closest("button").attr("data-id") }, function (data) {
            var __content = $(data);
            __content.find(':checkbox').checkboxpicker();
            init_cardcancel(__content);
            __content.modal("show");
            __content.on('hidden.bs.modal', function () {
                __content.remove();

            });
        }, "html");
    });
    $("#diler_code_word_edit").keyup(function () {
        //alert($(this).val());
        filterDilers_edit($(this).val());

    });
    var content;
    function filterDilers_edit(letters) {
        if (timer) {
            clearTimeout(timer);
        }
        //$("#attachmentsWrapper").hide();
        if (letters.length >= 3) {
            $("#dilersWrapper_edit").empty();
            $loader.gSpinner("hide");
            $loader.gSpinner({ scale: .3 });
            var custumer_id = $('#Customer_Id').val();
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
                $.get("/Books/getUserEditDetails", { filter: letters, custumer_id: custumer_id }, function (data, status) {
                    //alert("Data: " + data + "\nStatus: " + status);
                    $loader.gSpinner("hide");
                    content = $(data);
                    $("#dilersWrapper_edit").html(content);
                    content.find(".selectpicker").selectpicker();

                    var id__ = content.find("input[name='dilerCards.diler_id']:checked").val();
                    var towerids = $("#diler_" + id__ + "_cardid").val();
                    content.find("#diler_92_cardid").on("changed.bs.select", function (e, clickedIndex, newValue, oldValue) {
                        alert(0);
                    });
                    initContent(content);
                    //var dat = eval(data);
                    //$("#attachmentsWrapper").show(200);

                });
            }, 300, letters);
        }
        else {
            $("#dilersWrapper_edit").empty();
            // $loader.gSpinner("hide");
        }

    }


    function initContent(content) {
        $('#diler_cards_wrapper').html('');
        content.find('.dilerCards_change').on('change', function () {
            var checkedcards = content.find(".diler_Cards");
            for (var i = 0; i < checkedcards.length; i++) {
                if ($(checkedcards[i]).is(":checked"))
                    content.find(checkedcards[i]).prop('checked', false);
            }
        });
        content.find(".diler_Cards").click(function (e) {
            $('#diler_cards_wrapper').html('')
            var id__ = content.find("input[name='dilerCards.diler_id']:checked").val();
            //alert($(this).attr("id"));
            var parrenttr = $(this).parents("tr");
            var clicked_diler_id = $(parrenttr).find("input[name = 'dilerCards.diler_id']").attr("id");
            if (clicked_diler_id == id__) {
                var checkedcards = content.find(".diler_Cards");
                var k = 0;
                for (var i = 0; i < checkedcards.length; i++) {
                    if ($(checkedcards[i]).is(":checked")) {
                        var data = '<input type="hidden" name="dilerCards.card_id[' + k + ']" value="' + $(checkedcards[i]).val() + '" />';
                        $('#diler_cards_wrapper').append(data);
                        k++;
                        //alert($(checkedcards[i]).attr("id"));
                    }
                }

            }
            else
                return false;

        });

    }

    $("form").submit(function (event) {
        if ($('#isFromDiler').prop("checked") == true && $("#diler_code_word_edit").val() == "") {
            alert('შეიყვანეთ კოდური სიტყვა');
            return false;

        }
    });

    $(':checkbox').checkboxpicker();
    $(".dilersWrapper_edit").hide();
    $('#isFromDiler').checkboxpicker();
    $('#isFromDiler').on('change', function () {
        if ($(this).prop("checked") == true) {
            $(".diler_filter").show(200);
            $("#isFromDiler_").val('true');
        }
        else {
            $(".dilersWrapper_edit").hide(200);
            $("#dilersWrapper_edit").empty();
            $("#diler_code_word").val('');

            $("#isFromDiler_").val('false');
        }
    });

    $("form").submit(function (e) {
        setFieldsChange($("#abonent_panel, #cards_panel").find("input[data-tag], textarea[data-tag], select[data-tag]"));
    });

    $("#btn_get_history, #btn_get_all_history").on("click", function (e) {
        e.preventDefault();
        var cardid = $(this).data("card-id");
        var cust_id = $("#Customer_Id").val();
        getHistory(cardid, cust_id);
    });

    function getHistory(cardid, cust_id, dtfrom, dtto) {
        $.post("/Abonent/GetCardInfo/",
            {
                dt_from: dtfrom,
                dt_to: dtto,
                card_id: cardid,
                cust_id: cust_id,
                detaled: false
            },
            function (data) {
                var content = $(data);
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                });

                initHistoryContent(content, cardid, cust_id);
            }, "html");
    }

    function initHistoryContent(content, cardid, cust_id) {
        content.find(".datepickers").datePickers({ prefix: "picker" });
        content.find('#picker_picker_nav').on("click", function (e) {
            var dtfrom = content.find('#picker_picker_from').val().replace(/\//gi, '');
            var dtto = content.find('#picker_picker_to').val().replace(/\//gi, '');
            e.preventDefault();
            content.modal("hide");
            getHistory(cardid, cust_id, dtfrom, dtto);
        });
    }

    $("#btn_open_block").on("click", function (e) {
        e.preventDefault();
        var ctl_code = $("#txt_enter_code");
        $("#edit_modal_error").empty();
        $.post("/Abonent/ValidateKey", { key: ctl_code.val(), original_key: $("#Customer_SecurityCode").data("original") }, function (data) {
            if (data === true) {
                $("#save_area").html('<input type="submit" value="შენახვა" class="btn btn-default btn-sm pull-right" />');
                $("#edit_modal").modal("hide");
                has_readonly = 0;
            } else {
                has_readonly = 1;
                ctl_code.focus();
                $("#edit_modal_error").html('<div class="alert alert-danger alert-dismissible" style="margin-bottom:0px;" role="alert">' +
                    '<button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span></button>' +
                    'კოდური სიტყვა არასწრია</div>');
            }
            ctl_code.val("");
        }, "json");
    });

    $("#btn_chat").on("click", function (e) {
        e.preventDefault();
        var txt = $("#btn-input");
        if ($.trim(txt.val()) === '') {
            txt.focus();
            return;
        }

        $.post("/Abonent/AddChat", { text: txt.val(), customer_id: $("#Customer_Id").val() }, function (data) {
            if (data.res === 1) {
                $(".chat").append('<li class="left clearfix"><div class="chat-body clearfix"><div class="header">' +
                    '<strong class="primary-font">' + data.UserName + '</strong> <small class="pull-right text-muted">' +
                    '<span class="glyphicon glyphicon-time"></span> ' + data.Tdate +
                    '</small></div><p>' + data.Message + '</p></div></li>');
                txt.val("");
            }
        }, "json");
    });

    $('#damageModal').on('show.bs.modal', function (event) {
        if (has_readonly === 1)
            return;
        var button = $(event.relatedTarget);
        var id = button.data('id');
        var modal = $(this);

        modal.find("#damage_card_id").val(id);
        modal.find("#damage_create").on("click", function (e) {
            $.post("/Damage/CreateDamage", { card_id: id, reason_id: modal.find("#damage_reason").find("option:selected").val(), message: modal.find("#damage_text").val() }, function (data) {
                if (data === 1)
                    location.href = location.href;
            }, "json");
        });
    });

    //$('#card_cancel_modal').on('show.bs.modal', function (event) {
    //    if (has_readonly === 1)
    //        return;
    //    var button = $(event.relatedTarget);
    //    var id = button.data('id');
    //    var modal = $(this);

    //    modal.find("#card_cancel").on("click", function (e) {
    //        $.post("/Abonent/CardCancel", { card_id: id,mode: modal.find("#card_cancel_mode").find("option:selected").val() }, function (data) {
    //            if (data === true) {
    //                location.href = location.href;
    //            } else {
    //                alert("ბარათის გაუქმება ვერ მოხერხდა!");
    //            }
    //        }, "json");
    //    });  
    //});

    $("#btn_restore_money").on("click", function (e) {
        e.preventDefault();
        if (has_readonly === 1)
            return;
        $.post("/Abonent/GetRestoreMoney", { abonent_id: $("#Customer_Id").val() }, function (data) {
            var content = $(data);
            content.modal("show");
            content.on('hidden.bs.modal', function () {
                content.remove();
            });

            content.find("#money_move_from").on("change", function () {
                var balance_group = content.find("#balance_group");
                var card_id = $(this).find("option:selected").val();
                if (card_id === "0") {
                    balance_group.addClass("hide");
                    return;
                }
                $.get("/api/utils/getbalancebycard/" + card_id, {}, function (data) {
                    balance_group.removeClass("hide");
                    balance_group.find("input").val(data.balance).data(data).after('<input type="hidden" name="Status" value="' + data.status + '" />');
                }, "json");
            });

            content.find("form").submit(function (ev) {
                var balance = content.find("#balance_group").find("input");
                var card_to = content.find("#money_move_to").find("option:selected").val();
                var card_from = content.find("#money_move_from").find("option:selected").val();

                //if (balance.data("status") !== 7 && card_to == "0") {
                //    alert("შეუძლებელია აქტიური ბარათიდან თანხის გატანა!");
                //    return false;
                //}

                if (parseFloat(balance.val()) <= 0) {
                    alert("შიყვანეთ თანხა!");
                    balance.focus();
                    return false;
                }

                if (balance.data("balance") < parseFloat(balance.val())) {
                    alert("მითითებული თანხა აღემატება ნაშთს!");
                    balance.focus();
                    return false;
                }

                if (card_from === card_to) {
                    alert("ორ ტოლფას ბარათს შორის გადატანა აკრძალულია!");
                    return false;
                }
            });

        }, "html");
    });

});

function pause_card_reset(card_id) {
    if (has_readonly === 1)
        return;
    $.post("/Abonent/ResetCardPause", { card_id: card_id }, function (data) {
        if (data == true) {
            $("#lbl_card_status").html("აქტიური").next().remove();
            location.href = location.href;
        }
    });
}

function onRestoreMoney(res) {
    if (res)
        location.href = location.href;
    else
        alert("ოპერაცია ვერ შესრულდა");
}

function pause_card(card_id) {
    if (has_readonly === 1)
        return;

    $.get("/Abonent/CardPause",
        {
            card_id: card_id// $("#Customer_Id").val()
        },
        function (data) {
            var content = $(data);
            content.modal("show");


            content.on('hidden.bs.modal', function () {
                content.remove();
                //location.href = location.href;
            });

            content.find("#card_pause_modal_error").empty();

            content.find("#btn_card_pause").on("click", function (e) {
                e.preventDefault();
                var pause_checked = false;
                if (content.find('#_paused').prop("checked")) {
                    pause_checked = content.find('#_paused').prop("checked");
                }
                var pause_type = content.find("#txt_enter_pause_day").val();
                $.post("/Abonent/CardPause", { id: card_id, day: pause_type, privilegies_pause: pause_checked }, function (data) {
                    if (data === "") {
                        location.href = location.href;
                    } else {
                        content.find("#card_pause_modal_error").html('<div class="alert alert-danger alert-dismissible" style="margin-bottom:0px;" role="alert">' +
                            '<button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span></button>' +
                            '' + data + '</div>');
                    }
                }, "json");
            });
        }, "html");
}

function card_credit(card_id) {
    if (has_readonly === 1)
        return;
    if (confirm("შესრულდეს კრედიტის აღება?")) {
        $.post("/Abonent/CardCredit", { id: card_id }, function (data) {
            if (data === "") {
                location.href = location.href;
            } else {
                alert(data);
            }
        }, "json");
    }
}

function sendSMSAbonentNum(abonent_num, phone) {
    
    $.post("/Abonent/SendSMSAbonentNum", { abonent_num: abonent_num , phone:phone}, function (data) {
        if (data == 0) {
            alert("აბონენტის ნომერი წარმატებით გაიგზავნა.");

        }
        else {
            alert("აბონენტის ნომერი ვერ გაიგზავნა!");
        }

    });
}

function sendOSD(card_num) {
    //if (has_readonly === 1)
    //return;

    $.post("/Abonent/SendOSD", { card_num: card_num }, function (data) {
        if (data === 1)
            alert("OSD გაიგზავნა");
    }, "json");
}

function sendFactoryReset(card_num) {
    //if (has_readonly === 1)
    //return;

    $.post("/Abonent/SendReset", { card_num: card_num }, function (data) {
        if (data === 1)
            alert("მოთხოვნა გაიგზავნა");
    }, "json");
}

function sendShowInfo(card_num) {
    //if (has_readonly === 1)
    //return;

    $.post("/Abonent/SendShowInfo", { card_num: card_num }, function (data) {
        if (data === 1)
            alert("მოთხოვნა გაიგზავნა");
    }, "json");
}

function sendPinReset(card_num) {
    //if (has_readonly === 1)
    // return;
    old_pin = $("#old_pin").val();
    new_pin = $("#new_pin").val();
    $.post("/Abonent/SendPinReset", { card_num: card_num, default_pin: old_pin, new_pin: new_pin }, function (data) {
        if (data === 1)
            alert("მოთხოვნა გაიგზავნა");
    }, "json");
}

function sendPinDef(card_num) {
    //if (has_readonly === 1)
    //return;
    $.post("/Abonent/SendPinSetDefault", { card_num: card_num }, function (data) {
        if (data === 1)
            alert("მოთხოვნა გაიგზავნა");
    }, "json");
}

function refreshEntitlement(card_num, card_id) {
    if (has_readonly === 1)
        return;

    $.post("/Abonent/RefreshEntitlement", { card_num: card_num, card_id: card_id }, function (data) {
        if (data === 1)
            alert("Entitlement განახლდა");
    }, "json");
}

function cancelEntitlement(card_num, card_id) {
    if (has_readonly === 1)
        return;

    $.post("/Abonent/cancelEntitlement", { card_num: card_num, card_id: card_id }, function (data) {
        if (data === 1)
            alert("Entitlement წაიშალა");
    }, "json");
}

function block_card(card_id) {
    if (has_readonly === 1)
        return;
    var content = $("#card_block_modal");
    content.modal("show");
    content.find("#btn_card_block").on("click", function (e) {
        e.preventDefault();
        $.post("/Abonent/CardBlock", { card_id: card_id }, function (data) {
            if (data === "") {
                location.href = location.href;
            } else {
                alert(data);
            }
        }, "json");
    });
}

function block_card_reset(card_id) {
    if (has_readonly === 1)
        return;
    $.post("/Abonent/ResetCardBlock", { card_id: card_id }, function (data) {
        if (data == true) {
            location.href = location.href;
        } else {
            alert("ბარათის ბლოკის მოხსნა ვერ მოხერხდა!");
        }
    });
}

function RemoveChat(id) {
    $.post("/Abonent/RemoveChat", { id: id }, function (data) {
    });
}

function onEnable(button) {
    $(button).parent().prev().removeAttr("readonly");
    return false;
}

function GPS_Location(Latitude, Longitude, tower_id) {
    window.open('/Abonent/GPS', 'Abonent');
    $.get("/Abonent/GPS", { Latitude: Latitude, Longitude: Longitude, tower_id: tower_id }, function () {

        //var win = window.open('/Abonent/GPS', 'Abonent');

    });
}