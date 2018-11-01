var months = [{ id: 0, nam: 'იანვარი' }, { id: 1, nam: 'თებერვალი' }, { id: 2, nam: 'მარტი' }, { id: 3, nam: 'აპრილი' }, { id: 4, nam: 'მაისი' }, { id: 5, nam: 'ივნისი' }, { id: 6, nam: 'ივლისი' }, { id: 7, nam: 'აგვისტო' }, { id: 8, nam: 'სექტემბერი' }, { id: 9, nam: 'ოქტომბერი' }, { id: 10, nam: 'ნოემბერი' }, { id: 11, nam: 'დეკემბერი' }];
$(function () {
    var colors = Highcharts.getOptions().colors;
    var cards_count = 0;
    var cards_count_active = 0;
    var abonents_count = 0;
    var poll = 0;
    var promoCount = 0;
    var returnedcount = 0;

    //$.each(returnedCard, function (i, val) {
    //    returnedcount += val[1];
    //});


    $.each(promo_count, function (i, val) {
        promoCount += val[1];
    });

    $.each(cards_chart_data, function (i, val) {
        cards_count += val[1];
    });
    $.each(order_poll, function (i, val) {
        poll += val[1];
    });

    $.each(cards_active, function (i, val) {
        cards_count_active += val[1];
    });
    $.each(abonents_by_city_data, function (i, val) {
        abonents_count += val[1];
    });


    $('#order_poll').highcharts({
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
            text: 'გამოკითხვა - ' + poll
        },
        series: [{
            name: 'რაოდენობა',
            data: order_poll
        }]
    });


    $('#promo_count').highcharts({
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
            text: 'პრომო ბარათები - ' + promoCount
        },
        series: [{
            name: 'რაოდენობა',
            data: promo_count
        }]
    });
    $('#returnedCard').highcharts({
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
            text: ''
        },
        series: [{
            name: 'რაოდენობა',
            data: returnedCard
        }]
    });

    $('#promo_change_pakc').highcharts({
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
            text: ''
        },
        series: [{
            name: 'რაოდენობა',
            data: promo_change
        }]
    });


    $('#cards_count').highcharts({
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
            text: 'ბარათები - ' + cards_count
        },
        series: [{
            name: 'რაოდენობა',
            data: cards_chart_data
        }]
    });
    $('#cards_active').highcharts({
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
            text: ''
        },
        series: [{
            name: 'რაოდენობა',
            data: cards_active
        }]

    });

    $('#picker_picker_returned').click(function () {

        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var chart = $('#returnedCard').highcharts();
        $.post("/Main/ReturnedCard", { dt_from: dtfrom, dt_to: dtto }, function (response) {

            var res = JSON.parse(response);
            var respons = [res];

            var cardsum = 0;
            if (res.length > 0) {

                var obj = { name: [], data: [] };
                res.forEach(function (p) {
                    if (p[1] != 0) {
                        obj.data.push(p);
                        cardsum = cardsum + p[1];
                    }
                });
                chart.setTitle({ text: '' });
                chart.series[0].update({ data: obj.data, name: 'რაოდენობა' });
            }
        }, "json");

    });

    $('#picker_picker_nav').click(function () {

        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var chart = $('#cards_active').highcharts();
        $.post("/Main/CardsActive", { dt_from: dtfrom, dt_to: dtto }, function (response) {

            var res = JSON.parse(response);
            var respons = [res];

            var cardsum = 0;
            if (res.length > 0) {

                var obj = { name: [], data: [] };
                res.forEach(function (p) {
                    if (p[1] != 0) {
                        obj.data.push(p);
                        cardsum = cardsum + p[1];
                    }
                });
                chart.setTitle({ text: '' });
                chart.series[0].update({ data: obj.data, name: 'რაოდენობა' });
            }
        }, "json");

    });
    $('#picker_picker_poll').click(function () {

        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var chart = $('#order_poll').highcharts();
        $.post("/Main/StaticPoll", { dt_from: dtfrom, dt_to: dtto }, function (response) {

            var res = JSON.parse(response);
            var respons = [res];
            var cardsum = 0;
            if (res.length > 0) {

                var obj = { name: [], data: [] };
                res.forEach(function (p) {
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
    $("<ul style=\"list-style: none; height: 18px;\"><li style=\"display: inline-block; margin-bottom: 5px;\"><div class=\"col-md-12\" style=\"font-size:11px;\">გაუქმებული ბარათები: " + canceled_cards_count + "</div></li><li style=\"display: inline-block;\"><div class=\"col-md-2\"><button class=\"btn btn-default btn-xs\" id=\"btn_excel_cards_count_static\"><span class=\"glyphicon glyphicon-print\"></span> ექპორტი </button></div></li></ul> ").insertBefore('#cards_active .highcharts-container');
    $("<ul style=\"list-style: none; height: 18px;\"><li style=\"display: inline-block; margin-bottom: 5px;\"><div class=\"col-md-12\" style=\"font-size:11px;\">გაუქმებული ბარათები: " + canceled_cards_count + "</div></li><li style=\"display: inline-block;\"><div class=\"col-md-2\"><button class=\"btn btn-default btn-xs\" id=\"btn_excel_cards_count\"><span class=\"glyphicon glyphicon-print\"></span> ექპორტი </button></div></li></ul>").insertBefore('#cards_count .highcharts-container');
    $("<ul style=\"list-style: none; height: 18px;\"><li style=\"display: inline-block; margin-bottom: 5px;\"><div class=\"col-md-12\" style=\"font-size:11px;\"></div></li><li style=\"display: inline-block;\"><div class=\"col-md-2\"><button class=\"btn btn-default btn-xs\" id=\"btn_excel_cards_count\"><span class=\"glyphicon glyphicon-print\"></span> ექპორტი </button></div></li></ul>").insertBefore('#order_poll .highcharts-container');
    $("#btn_excel_cards_count").click(function () {
        url = "/Main/CardsCountExport/";
        location.href = url;
    });
    //$('<div class="col-md-12" style="font-size:11px;">გაუქმებული ბარათები: ' + canceled_cards_count + '</div>').insertBefore('#cards_count .highcharts-container');

    $('#abonents_by_city_count').highcharts({
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
                    format: '<b>{point.name}</b>: {point.y}'
                }
            }
        },
        title: {
            text: 'აბონენტები ქალაქების მიხედვით - ' + abonents_count
        },
        series: [{
            turboThreshold: 9000,
            name: 'აბონენტების რაოდენობა',
            data: abonents_by_city_data
        }]
    });

    //$('<div class="col-md-12" style="font-size:11px;">&nbsp;</div>').insertBefore('#abonents_by_city_count .highcharts-container');
    $('<div class="col-md-2" style="margin-bottom: 3px;"><button class="btn btn-default btn-xs" id="btn_excel_cities"><span class="glyphicon glyphicon-print"></span> ექპორტი </button></div>').insertBefore('#abonents_by_city_count .highcharts-container');
    $("#btn_excel_cities").click(function () {
        url = "/Main/CountByCitiesExport/";
        location.href = url;
    });

    $('#monthly_deals').highcharts({
        chart: {
            type: 'line',
            events: {
                redraw: function () {
                }
            }
        },
        title: {
            text: 'შეკვეთები, მონტაჟები დაზიანებები თვიურად'
        },
        xAxis: {
            categories: ['იან', 'თებ', 'მარ', 'აპრ', 'მაი', 'ივნ', 'ივლ', 'აგვ', 'სექ', 'ოქტ', 'ნოემ', 'დეკ']
        },
        yAxis: {
            title: {
                text: 'რაოდენობები'
            },
            allowDecimals: false
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true
                },
                enableMouseTracking: false
            }
        },
        series: [{
            name: 'შეკვეთები',
            data: monthly_deals.orders
        }, {
            name: 'მონტაჟები',
            data: monthly_deals.montages
        }, {
            name: 'დაზიანებები',
            data: monthly_deals.damages
        }]
    });
    $('<div class="col-md-3"><input type="number" id="monthly_deals_date" class="form-control input-sm" min="2014" max="2060" value="' + new Date().getFullYear() + '" onchange="MonthlyDealsChange(this)" /></div>').insertBefore('#monthly_deals .highcharts-container');
    $('<div class="col-md-2"><button class="btn btn-default btn-xs" id="btn_excel"><span class="glyphicon glyphicon-print"></span> ექპორტი </button></div>').insertBefore('#monthly_deals .highcharts-container');

    $("#btn_excel").on("click", function () {
        var year = $("#monthly_deals_date").val().toString();
        url = "/Main/ChartDataExport/?year=" + year;
        location.href = url;
    });

    $('#packets_by_cards').highcharts({
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
                    format: '<b>{point.name}</b> {point.percentage:.1f} %'
                }
            }
        },
        title: {
            text: 'ბარათები პაკეტებით'
        },
        series: [{
            name: 'რაოდენობა',
            data: packets_by_cards
        }]
    });

    //$('<div class="col-md-3" style="height:30px;">&nbsp;</div>').insertBefore('#packets_by_cards .highcharts-container');
    $('<div class="col-md-2" style="margin-bottom: 3px;"><button class="btn btn-default btn-xs" id="btn_excel_packets_by_cards"><span class="glyphicon glyphicon-print"></span> ექპორტი </button></div>').insertBefore('#packets_by_cards .highcharts-container');
    $("#btn_excel_packets_by_cards").click(function () {
        url = "/Main/PackagesByCardsExport/";
        location.href = url;
    });


    $('#charges_and_payments').highcharts({
        chart: {
            type: 'line',
            events: {
                redraw: function () {
                }
            }
        },
        title: {
            text: 'გადახდები და დარიცხვები'
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
            valueSuffix: ' ლარი'
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: false
                }
            }
        },
        series: [{
            name: 'გადახდები',
            data: charges_and_payments.payments
        }, {
            name: 'დარიცხვები',
            data: charges_and_payments.charges
        }]
    });
    $('<div class="col-md-2"><select id="monthly_charges_payments" class="form-control input-sm" onchange="ChargesPaymentsTypeChange(this)"><option value="1">თვე</option><option value="2">დღე</option></select></div><div class="col-md-2" id="months_charges_payments"></div>').insertBefore('#charges_and_payments .highcharts-container');



    //returned card
    $('#returned_card').highcharts({
        chart: {
            type: 'line',
            events: {
                redraw: function () {
                }
            }
        },
        title: {
            text: 'გაუქმება'
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
            name: 'გაუქმება',
            data: returned_card.Cancled,
            color: '#df2f2f'
        }]
    });
    $('<div class="col-md-2"><select id="monthly_charges_cancled" class="form-control input-sm" onchange="ReturnedCardTypeChange(this)"><option value="1">თვე</option><option value="2">დღე</option></select></div><div class="col-md-2" id="months_charges_cancled"></div>').insertBefore('#returned_card .highcharts-container');

    // damage
    $('#damage_card').highcharts({
        chart: {
            type: 'line',
            events: {
                redraw: function () {
                }
            }
        },
        title: {
            text: 'დაზიანება'
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
            name: 'დაზიანება-სრული',
            data: damage_card.damage

        },
        {
            name: 'დაზიანება-რეაგირება',
            data: damage_card.damage_proces
        },
        {
            name: 'ქოლცენტრის-დასრულებული',
            data: damage_card.damage_coll
        }]
    });
    $('<div class="col-md-2"><select id="monthly_damage" class="form-control input-sm" onchange="DamageCardTypeChange(this)"><option value="1">თვე</option><option value="2">დღე</option></select></div><div class="col-md-2" id="months_damage"></div>').insertBefore('#damage_card .highcharts-container');
    //
    $('#retunedCardChart').highcharts({
        chart: {
            type: 'line',
            events: {
                redraw: function () {
                }
            }
        },
        title: {
            text: 'გაუქმება'
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
            name: 'აქტიურის-გაუქმება',
            data: retunedCardChart.countactive

        },
        {
            name: '1-თვე გათიშულის-გაუქმება',
            data: retunedCardChart.count1mont
        },
        {
            name: '2-თვე გათიშულის-გაუქმება',
            data: retunedCardChart.count2mont
        },
        {
            name: '2-თვეზე მეტი გათიშულის-გაუქმება',
            data: retunedCardChart.count3mont
        },
        {
            name: 'პრომოს-გაუქმება',
            data: retunedCardChart.countpromo
        }
        ]
    });
    $('<div class="col-md-2"><select id="monthly_returned" class="form-control input-sm" onchange="ReturnedCardTypeChangeChart(this)"><option value="1">თვე</option><option value="2">დღე</option></select></div><div class="col-md-2" id="months_returned"></div>').insertBefore('#retunedCardChart .highcharts-container');
    var packets = [];
    var series_data = [];
    packets_by_cities[0].count.forEach(function (p) {
        packets.push(p.packet);
    });

    packets_by_cities.forEach(function (p) {
        var obj = { name: p.status, data: [] };
        p.count.forEach(function (c) {
            obj.data.push(c.count);
        });
        series_data.push(obj);
    });

    $('#packets_by_cities').highcharts({
        chart: {
            type: 'column'
        },
        title: {
            text: 'პაკეტები ქალაქებით'
        },
        xAxis: {
            categories: packets
        },
        yAxis: {
            min: 0,
            title: {
                text: 'რაოდენობები სტატუსებით'
            },
            stackLabels: {
                enabled: true,
                style: {
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            align: 'right',
            x: -30,
            verticalAlign: 'top',
            y: 25,
            floating: true,
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || 'white',
            borderColor: '#CCC',
            borderWidth: 1,
            shadow: false
        },
        tooltip: {
            formatter: function () {
                return this.series.name + ': ' + this.y + '<br/>' +
                    'სულ: ' + this.point.stackTotal;
            }
        },
        plotOptions: {
            column: {
                stacking: 'normal',
                dataLabels: {
                    enabled: true,
                    color: (Highcharts.theme && Highcharts.theme.dataLabelsColor) || 'white',
                    style: {
                        textShadow: '0 0 3px black'
                    }
                }
            }
        },
        series: series_data
    });
    var str = '<option value="თბილისი">თბილისი</value>';
    cities.forEach(function (r) {
        str += '<option value="' + r + '">' + r + '</value>'
    });
    $('<div class="col-md-2"><select id="packets_by_regions_cities" class="form-control input-sm" onchange="PacketsByCitiesChange(this)">' + str + '</select></div>').insertBefore('#packets_by_cities .highcharts-container');
    $('<div class="col-md-2"><button class="btn btn-default btn-xs" id="btn_excel_packets_by_cities"><span class="glyphicon glyphicon-print"></span> ექპორტი </button></div>').insertBefore('#packets_by_cities .highcharts-container');
    $("#btn_excel_packets_by_cities").click(function () {
        var city = $("#packets_by_regions_cities").val().toString();
        url = "/Main/PackagesByCitiesExport/?city=" + city;
        location.href = url;
    });

});

function ChargesPaymentsTypeChange(select) {
    var chart_area = $('#charges_and_payments');
    var chart = $('#charges_and_payments').highcharts();
    var s = $(select);
    if (s.find('option:selected').val() === '1') {
        chart_area.find('#months_charges_payments select').remove();
        ChargesPaymentsMonthsChange(null);
    } else {
        var str = '<select class="form-control input-sm">';
        months.forEach(function (month) {
            var s = month.id === new Date().getMonth() ? ' selected="selected"' : '';
            str += '<option onchange="ChargesPaymentsMonthsChange(this)" value="' + month.id + '"' + s + '>' + month.nam + '</option>';
        });
        str += '</select>';
        var drp = $(str);
        drp.on('change', ChargesPaymentsMonthsChange);
        drp.trigger('change');
        chart_area.find('#months_charges_payments').html(drp);
    }
}

function ChargesPaymentsMonthsChange(e) {
    var chart = $('#charges_and_payments').highcharts();
    var type = $('#charges_and_payments #monthly_charges_payments').find('option:selected').val();
    $.post("/Main/UpdateChargesPayments", { type: type, month: (e === null ? 0 : parseInt($(e.target).val()) + 1) }, function (res) {

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

        chart.series[0].setData(res.payments);
        chart.series[1].setData(res.charges);

    }, "json");
}
// returned card
function ReturnedCardTypeChange(select) {
    var chart_area = $('#returned_card');
    var chart = $('#returned_card').highcharts();
    var s = $(select);
    if (s.find('option:selected').val() === '1') {
        chart_area.find('#months_charges_cancled select').remove();
        ReturnedCardChange(null);
    } else {
        var str = '<select class="form-control input-sm">';
        months.forEach(function (month) {
            var s = month.id === new Date().getMonth() ? ' selected="selected"' : '';
            str += '<option onchange="ReturnedCardChange(this)" value="' + month.id + '"' + s + '>' + month.nam + '</option>';
        });
        str += '</select>';
        var drp = $(str);
        drp.on('change', ReturnedCardChange);
        drp.trigger('change');
        chart_area.find('#months_charges_cancled').html(drp);
    }
}

function ReturnedCardChange(e) {
    var chart = $('#returned_card').highcharts();
    var type = $('#returned_card #monthly_charges_cancled').find('option:selected').val();
    $.post("/Main/UpdateReturnedCard", { type: type, month: (e === null ? 0 : parseInt($(e.target).val()) + 1) }, function (res) {

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

        chart.series[0].setData(res.Cancled);
        //chart.series[1].setData(res.charges);

    }, "json");
}

function ReturnedCardTypeChangeChart(select) {
    var chart_area = $('#retunedCardChart');
    var chart = $('#retunedCardChart').highcharts();
    var s = $(select);
    if (s.find('option:selected').val() === '1') {
        chart_area.find('#months_returned select').remove();
        ReturnedCardChangeCart(null);
    } else {
        var str = '<select class="form-control input-sm">';
        months.forEach(function (month) {
            var s = month.id === new Date().getMonth() ? ' selected="selected"' : '';
            str += '<option onchange="ReturnedCardChangeCart(this)" value="' + month.id + '"' + s + '>' + month.nam + '</option>';
        });
        str += '</select>';
        var drp = $(str);
        drp.on('change', ReturnedCardChangeCart);
        drp.trigger('change');
        chart_area.find('#months_returned').html(drp);
    }
}
function ReturnedCardChangeCart(e) {
    var chart = $('#retunedCardChart').highcharts();
    var type = $('#retunedCardChart #monthly_returned').find('option:selected').val();
    $.post("/Main/ReturnedCardChart", { type: type, month: (e === null ? 0 : parseInt($(e.target).val()) + 1) }, function (res) {

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

        chart.series[0].setData(res.countactive);
        chart.series[1].setData(res.count1mont);
        chart.series[2].setData(res.count2mont);
        chart.series[3].setData(res.count3mont);
        chart.series[4].setData(res.countpromo);
    }, "json");
}
//damage
function DamageCardTypeChange(select) {
    var chart_area = $('#damage_card');
    var chart = $('#damage_card').highcharts();
    var s = $(select);
    if (s.find('option:selected').val() === '1') {
        chart_area.find('#months_damage select').remove();
        DamageCardChange(null);
    } else {
        var str = '<select class="form-control input-sm">';
        months.forEach(function (month) {
            var s = month.id === new Date().getMonth() ? ' selected="selected"' : '';
            str += '<option onchange="DamageCardChange(this)" value="' + month.id + '"' + s + '>' + month.nam + '</option>';
        });
        str += '</select>';
        var drp = $(str);
        drp.on('change', DamageCardChange);
        drp.trigger('change');
        chart_area.find('#months_damage').html(drp);
    }
}

function DamageCardChange(e) {
    var chart = $('#damage_card').highcharts();
    var type = $('#damage_card #monthly_damage').find('option:selected').val();
    $.post("/Main/UpdateDamageCard", { type: type, month: (e === null ? 0 : parseInt($(e.target).val()) + 1) }, function (res) {

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

        chart.series[0].setData(res.damage);
        chart.series[1].setData(res.damage_proces);
        chart.series[2].setData(res.damage_coll);

    }, "json");
}
//
function MonthlyDealsChange(input) {
    var chart = $('#monthly_deals').highcharts();
    $.post("/Main/UpdateMonthlyDeals", { year: input.value }, function (res) {

        chart.series[0].setData(res.orders);
        chart.series[1].setData(res.montages);
        chart.series[2].setData(res.damages);

    }, "json");
}

function PacketsByCitiesChange(input) {
    var chart = $('#packets_by_cities').highcharts();
    var city = $('#packets_by_cities').find('option:selected').val();
    $.post("/Main/PacketsByCities", { city: city }, function (res) {

        var series_data = [];
        chart.series.forEach(function (serie) {
            serie.update({ data: [] });
            serie.options.showInLegend = false;
            serie.legendItem = null;
            chart.legend.destroyItem(serie);
            chart.legend.render();
        });
        var i = 0;
        if (res.length > 0) {
            res.forEach(function (p) {
                var obj = { name: p.status, data: [] };
                p.count.forEach(function (c) {
                    obj.data.push(c.count);
                });
                chart.series[i].update({ data: obj.data, name: obj.name });
                i++;
            });
        }

    }, "json");
}


function getDaysInMonth(month) {
    var year = new Date().getFullYear();
    return [31, (isLeapYear(year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
}

function isLeapYear(year) {
    return (((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0));
}