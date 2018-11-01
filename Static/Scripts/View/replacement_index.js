$(function () {
    $('#picker_picker_nav').on("click", function (e) {
        var attrval = $(this).attr("href");
        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        var new_attr ="?dt_from=" + dtfrom + "&dt_to=" + dtto;
        $(this).attr("href", new_attr);

    });

});