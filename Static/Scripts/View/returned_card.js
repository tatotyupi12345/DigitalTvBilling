function init_cardcancel(content) {
    //parseFloat("123.456").toFixed(2);
    content.find(".selectpicker").selectpicker();
    //content.find('#commision').on('change', function () {
    //    if (content.find(this).prop("checked") == true)
    //    {
    //        content.find('#full').prop("disabled", true);
    //        content.find('#incomplete').prop("disabled", true);
    //        content.find('#checkbox_id1').prop("disabled", true);
    //        content.find('#checkbox_id3').prop("disabled", true);
    //        content.find('#force').prop("disabled", true);
    //        content.find('#select').prop("disabled", true);
    //        content.find('#checkbox_return').prop("disabled", true);
            
    //    }
    //    else {
    //        content.find('#full').prop("disabled", false);
    //        content.find('#incomplete').prop("disabled", false);
    //        content.find('#checkbox_id1').prop("disabled", false);
    //        content.find('#checkbox_id3').prop("disabled", false);
    //        content.find('#force').prop("disabled", false);
    //        content.find('#select').prop("disabled", false);
    //        content.find('#checkbox_return').prop("disabled", false);
    //    }


    //});
    content.find('#card_status').change(function () {

        if (content.find(this).val() == 5 || content.find(this).val() == 8) {

            content.find('#return_').css('display', 'block');
        }
        else {
            content.find('#return_').css('display', 'none');
        }
    });

    var balance = 0;
    content.find(':checkbox').checkboxpicker();
    content.find('#checkbox_id1').checkboxpicker();
    content.find('#checkbox_id1').on('change', function () {
        if (content.find(this).prop("checked") == true) {
            content.find('#disabled_1').removeAttr('disabled');
            content.find('#disabled_1').val('');
        }
        else {
            var computerScore = document.getElementById('balance').innerHTML;
            var combalance = Math.round(computerScore * 100) / 100;
            var min_amount = Math.round(content.find('#min_amount').val() * 100) / 100;
            var disable_1 = Math.round(content.find('#disabled_1').val() * 100) / 100;
            var dis_3 = Math.round(content.find('#disabled_3').val() * 100) / 100;
            var card_balance = Math.round(content.find('#card_balance').val() * 100) / 100;
            var balance = document.getElementById("return_balance").innerHTML - document.getElementById("comm_ision").innerHTML;

            if (combalance == 0 ) {
                content.find('#card_balance').val(Math.round((card_balance - disable_1) * 100) / 100);
                content.find('#disabled_1').attr('disabled', true);
                content.find('#disabled_1').val('ნაღდი');
                return;
            }
                min_amount = combalance + disable_1;
                //content.find("#min_amount").val(min_amount);
                document.getElementById('balance').innerHTML= min_amount;
            //}
            content.find('#disabled_1').attr('disabled', true);
            content.find('#disabled_1').val('ნაღდი');
        }
    });

    content.find('#checkbox_id3').on('change', function () {
        if (content.find(this).prop("checked") == true) {
            content.find('#disabled_3').removeAttr('disabled');
            content.find('#disabled_3').val('');
        }
        else {
            var computerScore = document.getElementById('balance').innerHTML;
            var combalance = Math.round(computerScore * 100) / 100;
            var min_amount = Math.round(content.find('#min_amount').val() * 100) / 100;
            var disable_3 = Math.round(content.find('#disabled_3').val() * 100) / 100;
            var card_balance = Math.round(content.find('#card_balance').val() * 100) / 100;
            var balance = document.getElementById("return_balance").innerHTML - document.getElementById("comm_ision").innerHTML;

            if (combalance == 0 ) {
                content.find('#card_balance').val(Math.round((card_balance - disable_3) * 100) / 100);
                content.find('#disabled_3').attr('disabled', true);
                content.find('#disabled_3').val('ნაღდი/უნაღდო');
                return;
            }
                min_amount = combalance + disable_3;
                //content.find("#min_amount").val(min_amount);
                document.getElementById('balance').innerHTML = min_amount;
            //}
            content.find('#disabled_3').attr('disabled', true);
            content.find('#disabled_3').val('ნაღდი/უნაღდო');
        }
    });

    content.find('#checkbox_return').on('change', function () {

        if (content.find(this).prop("checked") == true) {
            content.find('#card_balance').removeAttr('disabled');
        }
        else {
            content.find('#card_balance').attr('disabled', true);
        }
    });

    content.find('#card_balance,#disabled_1,#disabled_3').keypress(function (e) {
        var txt = String.fromCharCode(e.which);
        if (!txt.match(/[0-9&. ]/)) {
            return false;
        }
    });
    //T
    content.find('td input:checkbox').on("change", function () {

        if (content.find(this).prop("checked") == true) {
            //alert(content.find('#' + content.find(this).attr("value")).val());
            //var check_val = parseInt(document.getElementById(content.find(this).attr("value")).innerHTML);
            var check_val = parseInt(content.find('#' + content.find(this).attr("value")).val());
            var computerScore = document.getElementById('balance').innerHTML;
            var card_balance = Math.round(content.find('#card_balance').val() * 100) / 100;
            var bal_ance = Math.round(computerScore * 100) / 100;
            var check_balance = bal_ance + check_val;
            if ((check_balance - card_balance) > 0) {
                //var min_am = Math.abs(content.find('#min_amount').val());
                //document.getElementById('balance').innerHTML = Math.round((check_balance - card_balance) * 100) / 100;
                //content.find("#min_amount").val(Math.round(((min_am + check_val) - card_balance) * 100) / 100);
                document.getElementById('balance').innerHTML = Math.round((check_balance - card_balance) * 100) / 100;
                content.find("#min_amount").val(Math.round((check_balance - card_balance) * 100) / 100);
            }
            if (card_balance - check_val > 0) {
                content.find('#card_balance').val(Math.round((card_balance - check_val) * 100) / 100);
            }
            else {
                content.find('#card_balance').val(0);
            }

        }
        else {
            var balance = document.getElementById("return_balance").innerHTML - document.getElementById("comm_ision").innerHTML;
           // var check_val = parseInt(document.getElementById(content.find(this).attr("value")).innerHTML);
            var check_val = parseInt(content.find('#' + content.find(this).attr("value")).val());
            var computerScore = document.getElementById('balance').innerHTML;
            var card_balance = Math.round(content.find('#card_balance').val() * 100) / 100;
            var bal_ance = Math.round(computerScore * 100) / 100;
            document.getElementById('balance').innerHTML = (Math.round((computerScore) * 100) / 100) - check_val;
            content.find("#min_amount").val((Math.round((computerScore) * 100) / 100) - check_val);
            if ((bal_ance - check_val) < 0 && ((card_balance + check_val) - (Math.abs((bal_ance - check_val)))) <= balance) {

                if ((Math.abs(bal_ance - (card_balance + check_val)) + card_balance) >= balance)
                    content.find('#card_balance').val(balance);
                else
                    content.find('#card_balance').val(Math.abs(bal_ance - Math.abs(card_balance + check_val /*+ (parseInt(content.find('#card_balance').val()))*/)));

            }
            else {
                if (balance < 0)
                    content.find('#card_balance').val(0);
            }
            if ((bal_ance - check_val) < 0) {
                document.getElementById('balance').innerHTML = 0;
                content.find("#min_amount").val(0)
            }
        }
    });

    content.find('#disabled_1').on('keyup ', function (e) {
        var str = content.find('#disabled_1').val();
        var computerScore = document.getElementById('balance');
        var min_amount = Math.abs((Math.round((content.find('#min_amount').val()) * 100) / 100));
        var disabled_1 = (Math.round((content.find('#disabled_1').val()) * 100) / 100);
        var disabled_3 = (Math.round((content.find('#disabled_3').val()) * 100) / 100);
        if (e.keyCode == 8) {
            if (str[str.length - 1] == ".") {
                if (document.getElementById("disabled_3").disabled == false) {
                    computerScore.innerHTML = Math.round((min_amount - disabled_3) * 100) / 100;
                    min_amount = Math.round((min_amount - disabled_3) * 100) / 100;
                }
                else {
                    computerScore.innerHTML = min_amount;
                }
                return;
            }
            if (document.getElementById("disabled_3").disabled == false) {
                computerScore.innerHTML = Math.abs(Math.round((min_amount - disabled_3 - disabled_1) * 100) / 100);
                min_amount = Math.abs(Math.round((min_amount - disabled_3 - disabled_1) * 100) / 100);
                return;
            }
            else {
                computerScore.innerHTML = Math.round((min_amount - disabled_1) * 100) / 100;;
                min_amount = Math.round((min_amount - disabled_1) * 100) / 100;
                return;
            }
        }
        var txt = String.fromCharCode(e.which);
        if (txt.match(/[n]/)) {
            if (document.getElementById("disabled_3").disabled == false) {
                computerScore.innerHTML = Math.round((min_amount - disabled_3) * 100) / 100;
                min_amount = Math.round((min_amount - disabled_3) * 100) / 100;
            } else {
                computerScore.innerHTML = min_amount;
            }
            return;
        }
        if (document.getElementById("disabled_1").disabled == false && document.getElementById("disabled_3").disabled == false) {
            computerScore.innerHTML = Math.abs(Math.round((min_amount - (disabled_3 + disabled_1)) * 100) / 100);

            min_amount = Math.abs(Math.round((min_amount - (disabled_3 + disabled_1)) * 100) / 100);
        }
        else {
            computerScore.innerHTML = Math.round((min_amount - disabled_1) * 100) / 100;
            min_amount = Math.round((min_amount - disabled_1) * 100) / 100;
        }
        if (document.getElementById('balance').innerHTML <= 0.0) {
            computerScore.innerHTML = 0;
        }

    });
    content.find('#disabled_3').on('keyup ', function (e) {
        var str = content.find('#disabled_3').val();
        var computerScore = document.getElementById('balance');
        var min_amount = Math.abs((Math.round((content.find('#min_amount').val()) * 100) / 100));
        var disabled_1 = (Math.round((content.find('#disabled_3').val()) * 100) / 100);
        var disabled_3 = (Math.round((content.find('#disabled_1').val()) * 100) / 100);
        if (e.keyCode == 8) {
            if (str[str.length - 1] == ".") {
                if (document.getElementById("disabled_1").disabled == false) {
                    computerScore.innerHTML = Math.round((min_amount - disabled_3) * 100) / 100;
                    min_amount = Math.round((min_amount - disabled_3) * 100) / 100;
                }
                else {
                    computerScore.innerHTML = Math.round((min_amount) * 100) / 100;
                }
                return;
            }
            if (document.getElementById("disabled_1").disabled == false) {
                computerScore.innerHTML = Math.abs(Math.round((min_amount - disabled_3 - disabled_1) * 100) / 100);
                min_amount = Math.abs(Math.round((min_amount - disabled_3 - disabled_1) * 100) / 100);
                return;
            }
            else {
                computerScore.innerHTML = Math.round((min_amount - disabled_1) * 100) / 100;;
                min_amount = Math.round((min_amount - disabled_1) * 100) / 100;
                return;
            }
        }
        var txt = String.fromCharCode(e.which);
        if (txt.match(/[n]/)) {
            if (document.getElementById("disabled_1").disabled == false) {
                computerScore.innerHTML = Math.round((min_amount - disabled_3) * 100) / 100;
                min_amount = Math.round((min_amount - disabled_3) * 100) / 100;
            } else {
                computerScore.innerHTML = Math.round((min_amount) * 100) / 100;;
            }
            return;
        }
        if (document.getElementById("disabled_3").disabled == false && document.getElementById("disabled_1").disabled == false) {
            computerScore.innerHTML = Math.abs(Math.round((min_amount - (disabled_3 + disabled_1)) * 100) / 100);

            min_amount = Math.abs(Math.round((min_amount - (disabled_3 + disabled_1)) * 100) / 100);
        }
        else {
            computerScore.innerHTML = Math.round((min_amount - disabled_1) * 100) / 100;
            min_amount = Math.round((min_amount - disabled_1) * 100) / 100;
        }
        if (document.getElementById('balance').innerHTML <= 0.0) {
            computerScore.innerHTML = 0;
        }

    });


    content.find('#incomplete').on("change", function (e) {

        content.find('#hidden').css('display', 'block');

    });
    content.find('#full').on("change", function (e) {

        content.find('#hidden').css('display', 'none');

    });

    //content.find('#return_save').on('click', function () {

    //    $.post('Returned/ReturnedCancleDelete', { card_id: content.find('#card_id').val(), date: content.find('#returned_date').val() }, function (data) {

    //        if (data == 1) {
    //            alert('შეცდომა... მონაცამები არ შეიცვალა');
    //        }

    //    });

    //});
    content.find('#return_save').on("click", function (e) {
        content.find('#return_save', this).val("Please Wait...").attr('disabled', 'disabled');

        e.preventDefault();

        var Obj = { 'ReturnCardID': [] };

        content.find('td input:checkbox:checked').each(function () {

            Obj.ReturnCardID.push(content.find(this).attr("data-id"));

        });
        var commision = content.find('#commision').is(':checked') ? true : false;
        var select = document.getElementById("card_status"); // Выбираем  select по id
        var reason = select.options[select.selectedIndex].value;
        var select = document.getElementById("select");
        var diler = select.options[select.selectedIndex].value;
        var cash1 = 0, cash2 = 0, cash3 = 0;
        var force = content.find('#force').is(':checked') ? true : false;
        var pretentius = content.find('#pretentious').is(':checked') ? 1 : 0;
        var myObj = { 'commisionType': [], 'amount': [] };
        var date = "";
        if (content.find('#returned_date').val()) {
           date = content.find('#returned_date').val();
        }

        if (content.find('#checkbox_id1').is(':checked')) {
            cash1 = content.find("#disabled_1").val();
            if (cash1 == "") {
                alert("შეავსეთ ყველა ველი.");
                return;
            }
            myObj.commisionType.push(2);
            myObj.amount.push(cash1);
        }

        if (content.find('#checkbox_id3').is(':checked')) {
            cash3 = content.find("#disabled_3").val();
            if (cash3 == "") {
                alert("შეავსეთ ყველა ველი.");
                return;
            }
            myObj.commisionType.push(18);
            myObj.amount.push(cash3);
        }

        var card_id = content.find('#card_id').val();


        var amount = content.find("#card_balance").val();
        var comment = content.find('#Customer_Desc').val();

        if (content.find('#checkbox_return').is(':checked')) {

        }
        else {
            amount = 0;
        }
        if (parseInt(cash1) < 0 || parseInt(cash2) < 0 || parseInt(cash3) < 0 || parseInt(amount) < 0) {
            alert("დანაკლისით შევსება არ შეიძლება.");
            return;
        }


        $.post("/Abonent/SaveReturnedCard",
            {
                return_attachment: JSON.stringify(Obj),
                card_id: card_id,
                cash: JSON.stringify(myObj),
                amount: amount,
                comment: comment,
                reason: reason,
                bort_id: diler,
                pretentiu: pretentius,
                force: force,
                commions_not: commision,
                date: date
            },
            function (data) {
                if (data.error == 1) {

                    alert("ბარათი გაუქმდა.");
                    location.href = location.href;
                }
                else {
                    alert(data.error_message);
                    myObj = { 'commisionType': [], 'amount': [] };
                    content.find('#return_save', this).removeAttr('disabled');
                }

            }, "json");
    });


}

function filter() {
    var keyword = document.getElementById("search").value;
    var fleet = document.getElementById("select");
    for (var i = 0; i < fleet.length; i++) {
        var txt = fleet.options[i].text;
        if (txt.substring(0, keyword.length).toLowerCase() !== keyword.toLowerCase() && keyword.trim() !== "") {
            fleet.options[i].style.display = 'none';
        } else {
            fleet.options[i].style.display = 'list-item';
            document.getElementById("select").selectedIndex = i.toString();
        }
    }
}