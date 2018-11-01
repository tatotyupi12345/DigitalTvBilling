$(function () {

    $("#save_veried_abonent").on("click", function (e) {
        e.preventDefault();
        $.post("/Verify/SaveVerify",
            {
                id: $(this).closest("tr").attr("data-id"),// 
                verify_status: $("#save_veried_abonent").val()
            },
            function (data) {
                if (data === 1) {
                    alert("save success!");
                }
                else {
                    alert("not saved!");
                }
            }, "json");
    });

    //$("#save_veried_abonent").on("click", function (e) {
    //    e.preventDefault();
    //    $.post("/Verify/SaveVerify",
    //        {
    //            id: $(this).closest("tr").attr("data-id"),// 
    //            verify_status: $("#save_veried_abonent").val()
    //        },
    //        function (data) {
    //            if (data === 1) {
    //                alert("save success!");
    //            }
    //            else {
    //                alert("not saved!");
    //            }
    //        }, "json");
    //});

});