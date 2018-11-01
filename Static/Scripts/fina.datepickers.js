var nav_url;
(function ($) {
    $.fn.datePickers = function (options) {
        options = $.extend({
            prefix: '',
        }, options);

        var dateFrom = $(this).find("#" + options.prefix + "_picker_from");
        var dateTo = $(this).find("#" + options.prefix + "_picker_to");
        var choiseList = $(this).find(".dropdown-menu");
        nav_url = $(this).find("#" + options.prefix + "_picker_nav");
        dateFrom.datepicker({
            autoclose: true,
            language: "ka",
            format: 'dd/mm/yyyy'
        }).focus(function () { choiseList.hide(); }).on('changeDate', function (ev) {
            $(this).datepicker('hide');
            setUrl($(this).val(), dateTo.val());
        }).on('hide', function () {
            $(".choices").removeAttr("style");
        });
        dateTo.datepicker({
            autoclose: true,
            language: "ka",
            format: 'dd/mm/yyyy'
        }).focus(function () { choiseList.hide(); }).on('changeDate', function (ev) {
            $(this).datepicker('hide');
            setUrl(dateFrom.val(), $(this).val());
        }).on('hide', function () {
            $(".choices").removeAttr("style");
        });

        choiseList.on("click", "a", function () {
            var sign = $(this).data("val");
            var year = new Date().getFullYear();
            switch (sign) {
                case "today":
                    dateFrom.val(getDateString(new Date()));
                    dateTo.val(getDateString(new Date()));
                    break;
                case "year":
                    dateFrom.val(getDateString(new Date(year, 0, 1)));
                    dateTo.val(getDateString(new Date(year, 11, 31)));
                    break;
                case "kvartali1":
                    dateFrom.val(getDateString(new Date(year, 0, 1)));
                    dateTo.val(getDateString(new Date(year, 2, 31)));
                    break;
                case "kvartali2":
                    dateFrom.val(getDateString(new Date(year, 3, 1)));
                    dateTo.val(getDateString(new Date(year, 5, 30)));
                    break;
                case "kvartali3":
                    dateFrom.val(getDateString(new Date(year, 6, 1)));
                    dateTo.val(getDateString(new Date(year, 8, 30)));
                    break;
                case "kvartali4":
                    dateFrom.val(getDateString(new Date(year, 9, 1)));
                    dateTo.val(getDateString(new Date(year, 11, 31)));
                    break;
                case "yan":
                    dateFrom.val(getDateString(new Date(year, 0, 1)));
                    dateTo.val(getDateString(new Date(year, 0, 31)));
                    break;
                case "feb":
                    dateFrom.val(getDateString(new Date(year, 1, 1)));
                    dateTo.val(getDateString(new Date(year, 1, daysInMonth(1, year))));
                    break;
                case "mar":
                    dateFrom.val(getDateString(new Date(year, 2, 1)));
                    dateTo.val(getDateString(new Date(year, 2, 31)));
                    break;
                case "apr":
                    dateFrom.val(getDateString(new Date(year, 3, 1)));
                    dateTo.val(getDateString(new Date(year, 3, 30)));
                    break;
                case "may":
                    dateFrom.val(getDateString(new Date(year, 4, 1)));
                    dateTo.val(getDateString(new Date(year, 4, 31)));
                    break;
                case "jun":
                    dateFrom.val(getDateString(new Date(year, 5, 1)));
                    dateTo.val(getDateString(new Date(year, 5, 30)));
                    break;
                case "jul":
                    dateFrom.val(getDateString(new Date(year, 6, 1)));
                    dateTo.val(getDateString(new Date(year, 6, 31)));
                    break;
                case "aug":
                    dateFrom.val(getDateString(new Date(year, 7, 1)));
                    dateTo.val(getDateString(new Date(year, 7, 31)));
                    break;
                case "sep":
                    dateFrom.val(getDateString(new Date(year, 8, 1)));
                    dateTo.val(getDateString(new Date(year, 8, 30)));
                    break;
                case "oct":
                    dateFrom.val(getDateString(new Date(year, 9, 1)));
                    dateTo.val(getDateString(new Date(year, 9, 31)));
                    break;
                case "nov":
                    dateFrom.val(getDateString(new Date(year, 10, 1)));
                    dateTo.val(getDateString(new Date(year, 10, 30)));
                    break;
                case "dec":
                    dateFrom.val(getDateString(new Date(year, 11, 1)));
                    dateTo.val(getDateString(new Date(year, 11, 31)));
                    break;
            }

            setUrl(dateFrom.val(), dateTo.val());
        });

    }
})(jQuery);

function getDateString(date) {
    return ((date.getDate() < 10 ? '0' : '') + date.getDate()) + '/' + (((date.getMonth() + 1) < 10 ? '0' : '') + (date.getMonth() + 1)) + '/' + date.getFullYear();
}

function daysInMonth(m, y) {
    return 32 - new Date(y, m, 32).getDate();
}

String.prototype.ReplaceAll = function(stringToFind,stringToReplace){
    var temp = this;
    var index = temp.indexOf(stringToFind);
    while(index != -1){
        temp = temp.replace(stringToFind,stringToReplace);
        index = temp.indexOf(stringToFind);
    }
    return temp;
}

function setUrl(date1, date2) {
    if (nav_url.data("href").indexOf("?") > -1)
        nav_url.attr("href", nav_url.data("href") + "&dt_from=" + date1.ReplaceAll("/", "") + "&dt_to=" + date2.ReplaceAll("/", ""));
    else
        nav_url.attr("href", nav_url.data("href") + "?dt_from=" + date1.ReplaceAll("/", "") + "&dt_to=" + date2.ReplaceAll("/", ""));
    
}