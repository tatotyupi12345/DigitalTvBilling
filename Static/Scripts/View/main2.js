var months = [{ id: 0, nam: 'იანვარი' }, { id: 1, nam: 'თებერვალი' }, { id: 2, nam: 'მარტი' }, { id: 3, nam: 'აპრილი' }, { id: 4, nam: 'მაისი' }, { id: 5, nam: 'ივნისი' }, { id: 6, nam: 'ივლისი' }, { id: 7, nam: 'აგვისტო' }, { id: 8, nam: 'სექტემბერი' }, { id: 9, nam: 'ოქტომბერი' }, { id: 10, nam: 'ნოემბერი' }, { id: 11, nam: 'დეკემბერი' }];
$(function () {
    var colors = Highcharts.getOptions().colors;
    var orderansvers = 0;
    var damageansvers = 0;

    $.each(order_distinguished_answers, function (i, val) {
        orderansvers += val[1];
    });
    $.each(damage_distinguished_answers, function (i, val) {
        damageansvers += val[1];
    });
    $('#order_distinguished_answers').highcharts({
        chart: {
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0,
            },
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.y}</b>'
        },
        plotOptions: {
            pie: {
                depth: 25,
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.y} - {point.percentage:.1f}%'
                }
            }
        },
        title: {
            text: 'შეკვეთები - ' + orderansvers
        },
        series: [{
            name: 'რაოდენობა',
            data: order_distinguished_answers
        }]
    });
    $('#damage_distinguished_answers').highcharts({
        chart: {
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0,
            },
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.y}</b>'
        },
        plotOptions: {
            pie: {
                depth: 25,
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.y} - {point.percentage:.1f}%'
                }
            }
        },
        title: {
            text: 'დაზიანება - ' + damageansvers
        },
        series: [{
            name: 'რაოდენობა',
            data: damage_distinguished_answers
        }]
    });
    $('#picker_picker_nav').click(function () {

        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var chart = $('#order_distinguished_answers').highcharts();
        $.post("/Main2/OrderFilterDateTime", { dt_from: dtfrom, dt_to: dtto, user_id: $('#order_status_filter').val() }, function (response) {

            var res = JSON.parse(response);
            var respons = [res];
            var cardsum = 0;
            if (res.length > 0) {

                var obj = { name: [], data: [] };
                res.map(function (p) {
                    if (p[1] != 0) {
                        obj.data.push(p);
                        cardsum = cardsum + p[1];
                    }
                });
                chart.setTitle({ text: 'გამოკითხვა - ' + cardsum });
                chart.series[0].update({ data: obj.data, name: 'რაოდენობა' });
            }
            else {
                chart.setTitle({ text: '' });
                chart.series[0].update({ data: '', name: '' });
            }
        }, "json");

    });
    $('#picker_picker_poll').click(function () {

        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var chart = $('#damage_distinguished_answers').highcharts();
        $.post("/Main2/DamageFilterDateTime", { dt_from: dtfrom, dt_to: dtto, user_id: $('#damage_status_filter').val() }, function (response) {

            var res = JSON.parse(response);
            var respons = [res];
            var cardsum = 0;
            if (res.length > 0) {

                var obj = { name: [], data: [] };
                res.map(function (p) {
                    if (p[1] != 0) {
                        obj.data.push(p);
                        cardsum = cardsum + p[1];
                    }
                });
                chart.setTitle({ text: 'გამოკითხვა - ' + cardsum });
                chart.series[0].update({ data: obj.data, name: 'რაოდენობა' });
            }
            else {
                chart.setTitle({ text: '' });
                chart.series[0].update({ data: '', name: '' });
            }
        }, "json");

    });

    // statusebis statistica

    $('#card_status').highcharts({
        chart: {
            type: 'line',
            events: {
                redraw: function () {
                }
            }
        },
        title: {
            text: 'სტატუსები'
        },
        xAxis: {
            categories: ['იანვარი', 'თებერვალი', 'მარტი', 'აპრილი', 'მაისი', 'ივნისი', 'ივლისი', 'აგვისტო', 'სექტემბერი', 'ოქტომბერი', 'ნოემბერი', 'დეკემბერი']
        },
        yAxis: {
            title: {
                text: 'რაოდენობები'
            },
            allowDecimals: true,
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }]
        },
        tooltip: {
            valueSuffix: ''
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true
                }
            }
        },
        series: [{
            name: 'აქტიური',
            data: card_status.Active

        },
        {
            name: 'გათიშული',
            data: card_status.Closed
        },
        {
            name: 'დაპაუზებული',
            data: card_status.Pause
        },
        {
            name: 'დაბლოკილი',
            data: card_status.Blocked
        },
        {
            name: 'შეწყვეტილი',
            data: card_status.Discontinued
        },
        {
            name: 'გაუქმება',
            data: card_status.Cancel
        }
        ]
    });
    $('<div class="col-md-2"><select id="monthly_status" class="form-control input-sm" onchange="statusCardTypeChange(this)"><option value="1">თვე</option><option value="2">დღე</option></select></div><div class="col-md-2" id="_status"></div>').insertBefore('#card_status .highcharts-container');

});

function statusCardTypeChange(select) {
    var chart_area = $('#card_status');
    var chart = $('#card_status').highcharts();
    var s = $(select);
    if (s.find('option:selected').val() === '1') {
        chart_area.find('#_status select').remove();
        statusCardChange(null);
    } else {
        var str = '<select class="form-control input-sm">';
        months.forEach(function (month) {
            var s = month.id === new Date().getMonth() ? ' selected="selected"' : '';
            str += '<option onchange="statusCardChange(this)" value="' + month.id + '"' + s + '>' + month.nam + '</option>';
        });
        str += '</select>';
        var drp = $(str);
        drp.on('change', statusCardChange);
        drp.trigger('change');
        chart_area.find('#_status').html(drp);
    }
}

function statusCardChange(e) {
    var chart = $('#card_status').highcharts();
    var type = $('#card_status #monthly_status').find('option:selected').val();
    $.post("/Main2/UpdateChargesStatus", { type: type, month: (e === null ? 0 : parseInt($(e.target).val()) + 1) }, function (res) {

        if (type === '2') {
            var days = getDaysInMonth(parseInt($(e.target).val()));
            var arr = [];
            for (var i = 1; i <= days; i++) {
                arr.push(i.toString());
            }
            chart.xAxis[0].setCategories(arr);
        }
        else {
            chart.xAxis[0].setCategories(['იანვარი', 'თებერვალი', 'მარტი', 'აპრილი', 'მაისი', 'ივნისი', 'ივლისი', 'აგვისტო', 'სექტემბერი', 'ოქტომბერი', 'ნოემბერი', 'დეკემბერი']);
        }

        chart.series[0].setData(res.Active);
        chart.series[1].setData(res.Closed);
        chart.series[2].setData(res.Pause);
        chart.series[3].setData(res.Blocked);
        chart.series[4].setData(res.Discontinued);
        chart.series[5].setData(res.Cancel);

    }, "json");
}
function getDaysInMonth(month) {
    var year = new Date().getFullYear();
    return [31, (isLeapYear(year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
}

function isLeapYear(year) {
    return (((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0));
}