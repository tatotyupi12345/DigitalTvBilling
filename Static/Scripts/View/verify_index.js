$(function () {
    $("form").submit(function (e) {
        setFieldsChange($("#abonent_panel, #cards_panel").find("input[data-tag], textarea[data-tag], select[data-tag]"));
    });

    $(".verfiy_abonent").on("click", function (e) {
        e.preventDefault();
        $.post("/Verify/GetVerifications/",
            {
                cust_id: $(this).closest("tr").attr("data-id")// $("#Customer_Id").val()
            },
            function (data)
            {
                var content = $(data);
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                    content.remove();
                    location.href = location.href;
                });
                initContent(content);
            }, "html");
    });

    $('#picker_picker_nav').on("click", function (e) {
        var attrval = $(this).attr("href");
        var addParam = "";
        if (attrval.indexOf('page=') > -1)
        {
            attrval = attrval.replace('page=', '');
        }
        if (attrval.indexOf('?') > -1) {
            
            if (attrval.indexOf('filter_status') > -1)
            {
                addParam = "filter_status=" + $("#status-filter").val();
                attrval = attrval.replace(attrval.substr(attrval.indexOf('filter_status')), '');
            }
            else
            addParam = "&filter_status=" + $("#status-filter").val();
        }
        else {
            addParam = "/?filter_status=" + $("#status-filter").val();
        }

        $(this).attr("href", attrval + addParam);
        //alert();
    });

    //$("#paging > li")
    $("#paging").on("click", "a", function (event) {
        //alert($(this).attr("href"));
        var attrval = $(this).attr("href");
        var addParam = "";
        if (attrval.indexOf('?') > -1) {

            if (attrval.indexOf('filter_status') > -1) {
                addParam = "filter_status=" + $("#status-filter").val();
                attrval = attrval.replace(attrval.substr(attrval.indexOf('filter_status')), '');
            }
            else
                addParam = "&filter_status=" + $("#status-filter").val();
        }
        else {
            addParam = "/?filter_status=" + $("#status-filter").val();
        }

        $(this).attr("href", attrval + addParam);
    });
});

function initContent(content) {
    content.find('#save_veried_abonent').on("click", function (e) {
        e.preventDefault();
        $.post("/Verify/SaveVerify",
            {
                id: $(this).closest("tr").attr("data-id"),// 
                verify_status: content.find("#card_verify_status").val(),
                info: content.find("#Card_Desc").val()
            },
            function (data) {
                if (data === 1) {
                    alert("სტატუსი შეიცვალა წარმატებით.");
                    updateColor(content);
                }
                else {
                    alert("შეცდომა: სტატუსი ვერ შეიცვალა!");
                }
            }, "json");
    });

    content.find('#save_abonent_stat').on("click", function (e)
    {
        var dataval = $(this).closest("tr").attr("data-val");
        var custid = $(this).closest("tr").attr("data-id"); 
        var info = dataval == "is_satisfied"? content.find("#Card_satstat_Desc").val(): content.find("#Card_byustat_Desc").val();
        var ab_satisfied_status = content.find("#ab_satisfied_status").val();
        var ab_buyreason_status = content.find("#ab_buyreason_status").val();
        //switch(dataval)
        //{
        //    case "buy_reason":
        //        {

        //        }
        //        break;

        //    case "is_satisfied":
        //        {

        //        }
        //        break;
        //}

        e.preventDefault();
        $.post("/Verify/UpdateAbonentStatus",
            {
                cust_id: custid,// 
                dataval: dataval,
                satisfied_status: ab_satisfied_status,
                buyreason_status: ab_buyreason_status,
                info: info
            },
            function (data) {
                if (data === 1) {
                    alert("სტატუსი შეიცვალა წარმატებით.");
                    updateColor(content);
                }
                else {
                    alert("შეცდომა: სტატუსი ვერ შეიცვალა!");
                }
            }, "json");
    });
}

function updateColor(content)
{
    switch (parseInt(content.find("#card_verify_status").val())) {
        case 0:
            content.find('#save_veried_abonent').closest("tr").css("background-color", "lightgray");
            //$(this).closest("tr").css("background-color", "dimgray");
            break;

        case 1:
            content.find('#save_veried_abonent').closest("tr").css("background-color", "#a4ff7e");
            break;

        case 2:
            content.find('#save_veried_abonent').closest("tr").css("background-color", "#fce14b");
            break;

        case 3:
            content.find('#save_veried_abonent').closest("tr").css("background-color", "#e82966");
            break;
    }

    //switch (parseInt(content.find("#card_verify_status").val())) {
    //    case 0:
    //        content.find('#content.find').closest("tr").attr("class", "forPass");
    //        //$(this).closest("tr").css("background-color", "dimgray");
    //        break;

    //    case 1:
    //        content.find('#content.find').closest("tr").attr("class", "passed");
    //        break;

    //    case 2:
    //        content.find('#content.find').closest("tr").attr("class", "passedWithError");
    //        break;
    //}
}