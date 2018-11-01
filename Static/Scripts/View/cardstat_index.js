$(function () {
    $("#detail_filter").on("click", function (e) {
        e.preventDefault();
        $.post("/CardStat/GetDetailFilterModal", {}, function (data) {
            var content = showModal(data);
            content.find(".datepickers").datePickers({ prefix: "picker" });
            content.find("#picker_picker_nav").css("display", "none");
            content.find("#drp_abonent_select_by").on('change', function () {
                //alert(this.value);
                if (this.value == 2) {
                    content.find("#txt_abonent_select").hide();
                    content.find("#txt_abonent_select").attr('name', '_abonent_select');
                    content.find("#item_abonent_select").show();
                    content.find("#item_abonent_select").attr('name', 'abonent_select');
                }
                else {
                    content.find("#txt_abonent_select").show();
                    content.find("#txt_abonent_select").attr('name', 'abonent_select');
                    content.find("#item_abonent_select").hide();
                    content.find("#item_abonent_select").attr('name', '_abonent_select');
                }
            });





            content.find('#detail_filter').on("click", function (e) {
                var dtfrom = content.find('#picker_picker_from').val().replace(/\//gi, '');
                var dtto = content.find('#picker_picker_to').val().replace(/\//gi, '');
                content.find("#date_from_").val(dtfrom);
                content.find("#date_to_").val(dtto);
                //alert();
            });

        });
    });

    $('#picker_picker_nav').on("click", function (e) {
        var attrval = $(this).attr("href");
        //var addParam = "";
        //if (attrval.indexOf('page=') > -1) {
        //    attrval = attrval.replace('page=', '');
        //}
        //if (attrval.indexOf('?') > -1) {

        //    if (attrval.indexOf('filter_status') > -1) {
        //        addParam = "filter_status=" + $("#status-filter").val();
        //        attrval = attrval.replace(attrval.substr(attrval.indexOf('filter_status')), '');
        //    }
        //    else
        //        addParam = "&filter_status=" + $("#status-filter").val();
        //}
        //else {
        //    addParam = "/?filter_status=" + $("#status-filter").val();
        //}

        //$(this).attr("href", attrval + addParam);
        //alert();
        var abonent = getParameterByName('abonent');
        //var url
    });

    $("#abonents_body_status").on("click", "tr", function (e) {
        e.preventDefault();
        if ($(e.target).prop("tagName") != "TD")
            return;
        var cardid = $(this).attr("data-id");
        getCardInfo(cardid)
    });

    function getCardInfo(cardid, dtfrom, dtto) {
        $.post("/Abonent/GetCardInfo/",
            {
                dt_from: dtfrom,
                dt_to: dtto,
                card_id: cardid,
                cust_id: 0,
                detaled: false
            },
            function (data) {
                var content = $(data);
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                });

                initCardInfoContent(content, cardid);
            }, "html");
    }

    function initCardInfoContent(content, cardid) {
        content.find(".datepickers").datePickers({ prefix: "picker" });
        content.find('#picker_picker_nav').on("click", function (e) {
            var dtfrom = content.find('#picker_picker_from').val().replace(/\//gi, '');
            var dtto = content.find('#picker_picker_to').val().replace(/\//gi, '');
            e.preventDefault();
            content.modal("hide");
            getCardInfo(cardid, dtfrom, dtto);
        });
    }

    function getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }
});