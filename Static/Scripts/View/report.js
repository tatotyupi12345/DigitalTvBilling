var list_header;
var list_body;
var paging;
var btn_refresh;
var btn_excel;
var loading;
var controls_area;
var picker_from;
var picket_to;

$(function () {
    $(".datepickers").datePickers({ prefix: "picker" });
    $("#picker_picker_nav").hide();
    list_header = $("#list_header");
    list_body = $("#list_body");
    paging = $("#paging_area");
    btn_refresh = $("#btn_refresh");
    btn_excel = $("#btn_excel");
    loading = $("#loading");
    controls_area = $("#controls_area");
    picker_from = $("#picker_picker_from");
    picket_to = $("#picker_picker_to");

    paging.on("click", "a", function (e) {
        e.preventDefault();
        getData($(this).data("url"), parseInt($(this).attr("href")), $(this).data("params"));
    });

    $("#report_menu").on("click", "a", function (e) {
        e.preventDefault();
        $("#report_menu a").removeClass("active");
        $(this).addClass("active");

        controls_area.empty();
        var url = "";
        var report = $(this).data("report");
        var params = {};
        switch (report) {
            case "channels":
                url = "GetChannels/?pos=";
                break;
            case "mareg_channels":
                url = "GetMaregChannels/?pos=";
                break;
            case "packages":
                $("#datetime_area").removeClass("hide");
                url = "GetPackets/?pos=";
                var dropdown = $('<select class="form-control input-sm"><option value="true">აქტიური</option><option value="false">პასიური</option></select>');
                controls_area.html(dropdown);
                params = { sign: controls_area.find("select").eq(0).find("option:selected").val(), date_from: picker_from.val(), date_to: picket_to.val(), };
                break;
            case "backages_by_jurtype":
                $("#datetime_area").removeClass("hide");
                url = "GetPacketsByAbonentType/?pos=";
                var dropdown = $('<select class="form-control input-sm"><option value="true">აქტიური</option><option value="false">პასიური</option></select>');
                controls_area.html(dropdown);
                params = { sign: controls_area.find("select").eq(0).find("option:selected").val(), date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "cards":
                $("#datetime_area").removeClass("hide");
                url = "GetCards/?pos=";
                var dropdown = $('<select class="form-control input-sm"><option value="0">სრული</option><option value="1">აქტიური</option><option value="2">პასიური</option></select>');
                controls_area.html(dropdown);
                params = { sign: controls_area.find("select").eq(0).find("option:selected").val(), date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "cards_by_abonents":
                url = "GetCardsByAbonents/?pos=";
                break;
            case "lostcards":
                $("#datetime_area").removeClass("hide");
                url = "GetLostCards/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "cards_by_status":
                $("#datetime_area").removeClass("hide");
                url = "GetCardsByStatus/?pos=";
                var dropdown = $('<select class="form-control input-sm"><option value="-1">ყველა</option><option value="0">აქტიური</option><option value="1">გათიშული</option><option value="2">დაპაუზებული</option><option value="3">მონტაჟი</option><option value="4">გაუქმებული</option><option value="5">კრედიტი</option></select>');
                controls_area.html(dropdown);
                params = { sign: controls_area.find("select").eq(0).find("option:selected").val(), date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "payments":
                $("#datetime_area").removeClass("hide");
                url = "GetPayments/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "charges":
                $("#datetime_area").removeClass("hide");
                url = "GetCharges/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "charges_summary":
                $("#datetime_area").removeClass("hide");
                url = "GetChargesSummary/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "balance_by_cards_summary":
                $("#datetime_area").removeClass("hide");
                url = "GetBalanceByCardsSummary/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "balance_by_abonents_summary":
                $("#datetime_area").removeClass("hide");
                url = "GetBalanceByAbonentsSummary/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "balance_by_cards_summary_accounting":
                $("#datetime_area").removeClass("hide");
                url = "GetBalanceByAbonentsAccountingSummary/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "balance_by_abonents_summary_accounting":
                $("#datetime_area").removeClass("hide");
                url = "GetBalanceByAbonentsAccountingSummary/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "cards_count":
                $("#datetime_area").removeClass("hide");
                url = "GetCardsCount/?pos=";
                var dropdown = $('<select class="form-control input-sm"><option selected="selected" value="0">ფიზიკური</option><option value="1">იურიდიული</option><option value="2">ტექნიკური</option></select>');
                controls_area.html(dropdown);
                params = { sign: controls_area.find("select").eq(0).find("option:selected").val(), date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "form_1_1":
                $("#datetime_area").removeClass("hide");
                url = "GetForm1_1/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "form_4_3":
                $("#datetime_area").removeClass("hide");
                url = "GetForm4_3/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
            case "form_4_4":
                $("#datetime_area").removeClass("hide");
                url = "GetForm4_4/?pos=";
                params = { date_from: picker_from.val(), date_to: picket_to.val() };
                break;
        }

        getData(url, 1, params);
    });

    btn_refresh.on("click", function (e) {

        var report = $("#report_menu a.active").data("report");
        var params = $(this).data("params");
        switch (report) {
            case "packages":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                params.sign = controls_area.find("select").eq(0).find("option:selected").val();
                break;
            case "backages_by_jurtype":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                params.sign = controls_area.find("select").eq(0).find("option:selected").val();
                break;
            case "cards":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                params.sign = controls_area.find("select").eq(0).find("option:selected").val();
                break;
            case "cards_by_abonents":
                break;
            case "lostcards":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "cards_by_status":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                params.sign = controls_area.find("select").eq(0).find("option:selected").val();
                break;
            case "payments":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "charges":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "charges_summary":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "balance_by_cards_summary":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "balance_by_abonents_summary":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "balance_by_cards_summary_accounting":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "balance_by_abonents_summary_accounting":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "cards_count":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                params.sign = controls_area.find("select").eq(0).find("option:selected").val();
                break;
            case "form_1_1":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "form_4_3":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
            case "form_4_4":
                params.date_from = picker_from.val();
                params.date_to = picket_to.val();
                break;
        }

        getData($(this).data("url"), 1, params);
    });

    btn_excel.on("click", function (e) {
        var report = $("#report_menu a.active").data("report");
        var params = btn_refresh.data("params");
        var url = '';
        switch (report) {
            case "channels":
                url = "/Report/GetChannelsExport";
                break;
            case "mareg_channels":
                url = "/Report/GetMaregChannelsExport";
                break;
            case "packages":
                url = "/Report/GetPacketsExport?sign=" + params.sign + "&date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "backages_by_jurtype":
                url = "/Report/GetPacketsByAbonentTypeExport?sign=" + params.sign + "&date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "cards":
                url = "/Report/GetCardsExport?sign=" + params.sign + "&date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "cards_by_abonents":
                url = "/Report/GetCardsByAbonentsExport";
                break;
            case "lostcards":
                url = "/Report/GetLostCardsExport?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "cards_by_status":
                url = "/Report/GetCardsByStatusExport/?sign=" + params.sign + "&date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "payments":
                url = "/Report/GetPaymentsExport/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "charges":
                url = "/Report/GetChargesExport/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "charges_summary":
                url = "/Report/GetChargesSummaryExport/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "balance_by_cards_summary":
                url = "/Report/GetBalanceByCardsSummaryExport/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "balance_by_abonents_summary":
                url = "/Report/GetBalanceByAbonentsSummaryExport/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "balance_by_cards_summary_accounting":
                url = "/Report/GetBalanceByCardsAccountingSummaryExport/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "balance_by_abonents_summary_accounting":
                url = "/Report/GetBalanceByAbonentsAccountingSummaryExport/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "form_1_1":
                url = "/Report/GetForm1_1Export/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "form_4_3":
                url = "/Report/GetForm4_3Export/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
            case "form_4_4":
                url = "/Report/GetForm4_4Export/?date_from=" + params.date_from.ReplaceAll("/", "_") + "&date_to=" + params.date_to.ReplaceAll("/", "_");
                break;
        }
        location.href = url;
    });

});

function getData(method, pos, params) {
    setLoading();
    $.post("/Report/" + method + pos, params, function (result) {
        removeLoading();
        if (pos == 1) {
            var header_str = '';
            $.each(result.cols, function (i, col) {
                header_str += '<th style="width:' + col.width + '%;">' + col.name + '</th>';
            });
            list_header.html(header_str);
        }

        var body_str = '';
        $.each(result.rows, function (i, row) {
            body_str += '<tr>';
            $.each(result.cols, function (j, col) {
                body_str += '<td>' + (col.format === true ? new Date(parseInt(row[col.column].substr(6))).toDateString() : row[col.column]) + '</td>';
            });
            body_str += '</tr>';
        });

        if (method === 'GetPayments/?pos=' || method === 'GetCharges/?pos=' || method === 'GetChargesSummary/?pos=' || method === 'GetBalanceByCardsSummary/?pos=' || method === 'GetBalanceByAbonentsSummary/?pos=') {
            controls_area.html('<span class="label label-default">სულ ჯამი: ' + result.amount + '</span></h3>');
        }

        paging.html(result.paging).find("li:first-child").remove();
        var a = paging.find("li").addClass("active").find("a");
        
        a.data({ "url": method, "params": params });
        btn_refresh.data({ "url": method, "params": params });

        if (pos == 1)
            list_body.html(body_str);
        else {
            list_body.append(body_str);
            var top = a.offset().top - 60;
            $("body").animate({ scrollTop: top + 'px' }, 100);
        }

        if (paging.find("li").hasClass("disabled"))
            paging.find("li").remove();

    }, "json");
}

function setLoading() {
    loading.prepend('<div class="progress progress-striped active" id="progress"><div class="progress-bar" style="width: 100%;"></div></div>');
}
function removeLoading() {
    loading.find('#progress').remove();
}