var paging;
var timer;
var abonents_body;
$(function () {
    abonents_body = $("#abonents_body");
    paging = $("#paging");

    //$("#txt_filter").keyup(function () {
    //    //alert($(this).val());
    //    filterAbonents($(this).val(), 1);

    //});

    //$('#txt_filter').change(function () {
    //    var optionSelected = $(this).find("option:selected");
    //    var valueSelected = optionSelected.val();
    //    var textSelected = optionSelected.text();
    //});

    //$('#filter_type').on('change', function () {
    //    //alert(this.value);
    //    //alert($("#drp_filter").val());
    //    if (this.value == "")
    //        location.href = location.href;
    //    filterAbonents(this.value, $("#drp_filter").val(), 1);
    //})

    $('#picker_picker_nav').on("click", function (e) {
        //alert(this.value);
        //alert($("#drp_filter").val());
        var attrval = $(this).attr("href");
        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var letters = $('#filter_type').val();
        var column = $("#drp_filter").val();
        //CustomerAttach/?dt_from=26072017&dt_to=26072017
        var new_attr = /*($(this).attr("href").indexOf('Accountant') != -1?'Accountant/':'')+*/ "?dt_from=" + dtfrom + "&dt_to=" + dtto + "&letter=" + letters + "&column=" + column + "&page=" + 1;
        $(this).attr("href", new_attr);
        //filterAbonents($('#filter_type').val(), $("#drp_filter").val(), 1);
    })

    function filterAbonents(letters, column, page)
    {
        var attrval = $('#picker_picker_nav').attr("href");
        var splitval="";
        var dt_from="";
        var dt_to="";

        if (attrval.indexOf('dt_from') != -1)
        {
            splitval = attrval.split("&");
            dt_from = splitval[0].substr(splitval[0].indexOf('dt_from')).replace(/^\D+/g, '');
            dt_to = splitval[1].substr(splitval[1].indexOf('dt_to')).replace(/^\D+/g, '');
        }

        if (timer) {
            clearTimeout(timer);
        }

        //if (letters.length >= 3)
        {
            
            timer = setTimeout(function () {
                var href = "/Accountant/FilterAbonents?letter=" + letters + "&column=" + column + "&page=" + page + "&dt_from=" + dt_from + "&dt_to=" + dt_to;
                $('#picker_picker_nav').attr('href', href);

                //$.post("/Accountant/FilterAbonents", { letter: letters, column: column, page: page, dt_from:dt_from, dt_to:dt_to }, function (data, status) {

                //    if(data)
                //    {
                //        var approve = '';
                //        var str = '';
                //        var attr='';
                //        var grptype = "";
                //        var date;
                //        var sumPrice = 0;
                //        for (var i = 0; i < data.PriceDictionary.length; i++)
                //        {
                //            sumPrice += data.PriceDictionary[i].Value;
                //            grptype = '';
                //            attr = '';
                //            approve = '';
                //            var date = data.Abonents[i].tdate;
                //            //Remove all non-numeric (except the plus)
                //            date = date.replace(/[^0-9 +]/g, '');
                //            //Create date
                //            var birthDate = new Date(parseInt(date));

                //            var cDate = birthDate.getDate();
                //            var cMonth = birthDate.getMonth() + 1;
                //            var cYear = birthDate.getFullYear();

                //            var cHour = birthDate.getHours();
                //            var cMin = birthDate.getMinutes();
                //            var cSec = birthDate.getSeconds();

                //            switch(data.Abonents[i].SType)
                //            {
                //                case 0:
                //                {
                //                    grptype = "შ.პ.ს";
                //                }
                //                break;

                //                case 1:
                //                {
                //                    grptype = "ინდ. მეწარმე";
                //                }
                //                break;
                //            }

                //            if (data.Abonents[i].Mode == 2)
                //            {
                //                attr = "approved";
                //                approve = '<span class="bg-success">OK</span>';
                //            }
                //            else
                //            {
                //                approve = '<button class="btn btn-default btn-xs" type="button" onclick="recordApprove(' + data.Abonents[i].ID + ')"><span class="glyphicon glyphicon-ok" aria-hidden="true"></span></button>';
                //            }

                //            str +=  '<tr data-id="' + data.Abonents[i].ID + '" class="' + attr + '">' +
                //                    '<td>' + data.Abonents[i].row_num + '</td>' +
                //                    '<td>' + cMonth + '/' + cDate + '/' + cYear + ' ' + cHour + ':' + cMin + ':' + cSec + '</td>' +
                //                    '<td>' + data.Abonents[i].CustomerName + '</td>' +
                //                    '<td>' + data.Abonents[i].CustomerLastName + '</td>' +
                //                    '<td>' + data.Abonents[i].CustomerCode + '</td>' +
                //                    '<td>' + data.PackageDictionary[i].Value + '</td>' +
                //                    '<td>' + data.Abonents[i].CustomerAddress + '</td>' +
                //                    '<td>' + data.Abonents[i].ObjectName + '</td>' +
                //                    '<td>' + data.Abonents[i].ObjectAddress + '</td>' +
                //                    '<td>' + data.Abonents[i].Group + '</td>' +
                //                    '<td>' + data.Abonents[i].UserCode + '</td>' +
                //                    '<td>' + grptype + '</td>' +
                //                    '<td data-approve="' + data.Abonents[i].ID + '" id="data-approve-' + data.Abonents[i].ID + '">' + approve + '</td></tr>';
                //        }
                //        //სულ ჯამი: @sumPrice, &nbsp;&nbsp; პაკეტი 12 GEL: @ViewBag.pack12price, &nbsp;&nbsp; პაკეტი 15 GEL: @ViewBag.pack15price
                //        $("#sumPrice").html('სულ ჯამი: ' + sumPrice + ', &nbsp;&nbsp; ' +'პაკეტი 12 GEL: ' + data.pack12price + ', &nbsp;&nbsp; ' +'პაკეტი 15 GEL: ' + data.pack15price );
                //        //$("#sum12Price").html('სულ ჯამი: ' + data.pack12price);
                //        //$("#sum15Price").html('სულ ჯამი: ' + data.pack15price);

                //        abonents_body.html(str);
                //        paging.data("mode", "ajax").html(data.Paging);
                //    }
                    
                //});
            }, 300, letters);
        }
        //else
        //{

        //}
    }

    
    
});

function recordApprove(id) {

    if (confirm("ნამდვილად გსურთ დადასტურება?") == true) {
        //txt = "You pressed OK!";
    } else {
        return;
    }
     
    $.post("/Accountant/RecordApprove", { id: id }, function (data) {
        if (data === 1) {
            $("[data-id='" + id + "']").css('background-color', '#a4ff7e');
            $("#data-approve-"+id).html('<span class="bg-success">OK</span>');
            //location.href = location.href;
        }
    }, "json");
    
}

function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}