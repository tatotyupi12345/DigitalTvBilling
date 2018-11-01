$(function () {
    var timeout;
    $("#Customer_City").typeahead({
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
                    $("#Customer_City").val($.trim(sp[0]));
                    $("#Customer_Village").val($.trim(sp[1]));
                    $("#Customer_Region").val(data.region);
                    if (data.status === 'ok')
                        $('#status_alert').html('<div class="alert alert-success" style="padding:5px;margin-bottom:0px;" role="alert">ok</div>');
                    else if(data.status === 'soon')
                        $('#status_alert').html('<div class="alert alert-warning" style="padding:5px;margin-bottom:0px;" role="alert">ok</div>');
                    else
                        $('#status_alert').html('<div class="alert alert-danger" style="padding:5px;margin-bottom:0px;" role="alert">no</div>');
                }
            }, "json");
            return item;
        }
    });

    $("#cards input[id$='__City']").each(function () {
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
    $('.damage_code').on('paste, keyup', function () {
        var data = $(this).val();
        $.post('/Abonent/Custumer', { code: $(this).val(), phone: $(this).val() }, function (data) {
            if (data != "") {
                $("#Customer_City").val(data.City);
                $("#Customer_Region").val(data.Region);
                $("#Customer_Name").val(data.Name);
                $("#Customer_LastName").val(data.LastName);
                $("#Customer_Address").val(data.Address);
                $("#Customer_Phone1").val(data.Phone1);
            }
        });

    });
    $('.damage_phone').on('paste, keyup', function () {
        var data = $(this).val();
        $.post('/Abonent/Custumer', { code: $(this).val(), phone: $(this).val() }, function (data) {
            if (data != "") {
                $("#Customer_City").val(data.City);
                $("#Customer_Code").val(data.Code);
                $("#Customer_Region").val(data.Region);
                $("#Customer_Name").val(data.Name);
                $("#Customer_LastName").val(data.LastName);
                $("#Customer_Address").val(data.Address);
                $("#Customer_Phone1").val(data.Phone1);
            }
        });

    });
    $('.cancled_code').on('paste, keyup', function (e) {
        var data = $(this).val();
        
        $.post('/Cancellation/Custumer', { code: $(this).val(), phone: $(this).val() }, function (data) {
            if (data != "" && e.keyCode == 86 || (e.keyCode >= 96 && e.keyCode <= 105)) {
                ind = 1;
                $("#Customer_City").val(data.customer.City);
                $("#Customer_Region").val(data.customer.Region);
                $("#Customer_Name").val(data.customer.Name);
                $("#Customer_LastName").val(data.customer.LastName);
                $("#Customer_Address").val(data.customer.Address);
                $("#Customer_Phone1").val(data.customer.Phone1);
                $.each(data.card, function (i, val) {
                    //var dataAppend = data.card[i] + '   <input class="diler_Cards textbox option-input" type="checkbox" id="" value="' + data.card[i] + '" name="diler_checked_id" />'
                    var dataAppend = '<label style="background-color:#1532bd42">' + data.card[i] + '</label>' + '   <input class="diler_Cards textbox option-input" type="checkbox" id="" value="' + data.card[i] + '" name="diler_checked_id" />'
                    $('#card_count').append(dataAppend);

                });
            }
        });

    });
    $('.cancled_phone').on('paste, keyup', function (e) {
        var data = $(this).val();
        $.post('/Cancellation/Custumer', { code: $(this).val(), phone: $(this).val() }, function (data) {
            if (data != "" && e.keyCode == 86 || (e.keyCode >= 96 && e.keyCode<=105)) {
                ind = 1;
                $("#Customer_City").val(data.customer.City);
                $("#Customer_Code").val(data.customer.Code);
                $("#Customer_Region").val(data.customer.Region);
                $("#Customer_Name").val(data.customer.Name);
                $("#Customer_LastName").val(data.customer.LastName);
                $("#Customer_Address").val(data.customer.Address);
                $("#Customer_Phone1").val(data.customer.Phone1);
                $.each(data.card, function (i, val) {
                    var dataAppend = '<label style="background-color:#1532bd42">' + data.card[i] +'</label>'+'   <input class="diler_Cards textbox option-input" type="checkbox" id="" value="' + data.card[i] +'" name="diler_checked_id" />'
                    $('#card_count').append(dataAppend);

                });
            }
        });

    });
});

