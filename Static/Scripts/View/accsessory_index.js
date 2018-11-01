$(function () {

    $("#drp_filter").change(function () {
        var str = $(this).text();
        var val = $(this).val();

        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
    }).change();

        $('#picker_picker_nav').on("click", function (e) {
        var attrval = $(this).attr("href");

        var filter_id = $("#drp_filter").val();
        var filter_user_id = $("#user_filter").val();
        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var new_attr = "?dt_from=" + dtfrom + "&dt_to=" + dtto +"&accessory_filter_id=" + filter_id + "&user_id=" + filter_user_id;
        $(this).attr("href", new_attr);

    });


    $("#abonents_body").on("click", "tr", function (e) {
        e.preventDefault();
        if ($(e.target).prop("tagName") != "TD")
            return;
        var customer_attach_id = $(this).closest("tr").attr("data-id");
        $.post("/Accessory/GetAccessoryInfo/",
            {
                ReturnedCardsID: customer_attach_id
            },

            function (data) {
                var content = $(data);
                content.modal("show");
                content.on('hidden.bs.modal', function () {
                content.remove();
                    //location.href = location.href;
                });
               initContent(content);
            }, "html");
    });


    function initContent(content) {
    content.find(':checkbox').checkboxpicker();
    content.find('#save').on("click", function (e) {
        e.preventDefault();

        var attachmentVals = [];
        var i = 0;
        var code = content.find("#abonent_id").val();
        var data = [];
        content.find(":checkbox").each(function (index) {
            var chkID = $(this).val();
            data.push({ check: $(this).is(":checked"), AccesoryID: chkID});
            //alert(chkID);
        });

        $.post("/Accessory/UpdateEntry",
            {
                data: data,
                code: code
                
            },
            function (data)
            {
                if (data === 1)
                {
                    location.href = location.href;
                }
                else
                {
                    alert("შეცდომა: მონაცემები ვერ შეიცვალა!");
                }
            }, "json");
    });
}
});