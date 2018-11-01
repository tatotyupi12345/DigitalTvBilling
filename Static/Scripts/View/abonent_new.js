var isRepeat = false;

$(function () {
    $("form").submit(function (e) {
        setFieldsNew($("#abonent_panel, #cards_panel").find("input, textarea, select"));
        //return false;
    });

    $("#btn_res").on("click", function (e) {
        e.preventDefault();
        $.post("/Setting/UpdateRs", {}, function (data) {
            if (data === true)
                alert("RS.ge-ის მონაცემები განახლდა");
        }, "json");
    });

});