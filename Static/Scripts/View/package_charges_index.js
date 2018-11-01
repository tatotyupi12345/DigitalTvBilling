var _card_id = 0;
var _user_id = 0;
var Tdate = "";
var audio_name = null;
var status = 0;
var value;
$(function () {
    $('#picker_picker_nav').on("click", function (e) {
        var attrval = $(this).attr("href");
        var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
        var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
        //var status = $('#filter_status').val();
        var filter = $('#txt_filter').val();
        var drp_filter = $('#drp_filter').val();
        var _filter = $('#_filter').val();
        var new_attr = "?name=" + filter + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&drp_filter=" + drp_filter + "&_filter=" + _filter;
        $(this).attr("href", new_attr);

    });
    $('#remove_filter').on("click", function () {
        location.href = location.pathname;
    });
    
    $('#drp_filter').change(function () {
        $(this).find("selected").prop("selected", true);
    });
    //$('#_filter').change(function () {
    //    $(this).find("selected").prop("selected", true);
    //});
});
function record(card_id, user_id) {

    Tdate = $('#change_date_' + card_id).val();
    _card_id = card_id;
    _user_id = user_id;
    

    var status_pack = $('#pack_' + card_id).attr("data-id");
    var status_pause = $('#pause_' + card_id).attr("data-id");
    if (status_pack == 0)
        status = status_pack;
    else {
        status = status_pause;
    }

//    if (confirm("ნამდვილად გსურთ დადასტურება?") == true) {

//    } else {
//        return;
//    }

    $.post("/PackagesCharges/GetInfoPlayer/",
        {
            card_id: card_id,
            status: status
    },

    function (data) {
        var content = $(data);
        content.modal("show");
        content.on('hidden.bs.modal', function () {
            content.remove();
            //location.href = location.href;
        });
        inContentfunction(content);
        }, "html");
//}, "json");


}

function inContentfunction(content) {

    //content.find('#record_dowload').on("click", function () {

    //    $.post("PackagesCharges/DowloadFile", {}, function (data) {


    //    });

    //});

    content.find('#type_ss').on("click", function (e) {
    var model = new FormData();
    model.append('typess', content.find("input[name='typess']")[0].files[0]);
    var x = content.find("input[name='typess']")[0].files[0];
    $.ajax({
        url: "/PackagesCharges/BrowsUploadSave",
        type: "POST",
        data: model,

        contentType: false,

        processData: false,
        success: function (result) {
            audio_name = result;
            content.find('#record_save').removeAttr("disabled");
            content.find('#record_dowload').html(result.play_name);
            content.find('#source').attr('src', result.play_name);
            var mp = document.getElementById("mp3");
            mp.src = result.play_name;
            mp.load();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
    });
    content.find('#record_save').on("click", function () {
        content.find('#record_save', this).val("გთხოვთ მოიცადოთ...").attr('disabled', 'disabled');
        $.post("/PackagesCharges/RecordApprove", { card_id: _card_id, user_id: _user_id, logging_date: Tdate, record_audio: audio_name, comment: content.find('#coment').val(), status: status }, function (data) {
        if (data === 1) {
           
            location.href = location.href;
        }
        else {
            alert("ვერიფიკაცია ვერ მოხერხდა!");
            location.href = location.href;
        }
        }, "json");
    });
}
function filter(value, filter) {

    if (value == "" || value.length < 3) {
        location.href = "?name=" + value + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&drp_filter=" + drp_filter;
        return;
    }
    var dtfrom = $('#picker_picker_from').val().replace(/\//gi, '');
    var dtto = $('#picker_picker_to').val().replace(/\//gi, '');
    var status = $('#filter_status').val();
    var drp_filter = filter;
    location.href = "?name=" + filter + "&dt_from=" + dtfrom + "&dt_to=" + dtto + "&drp_filter=" + drp_filter + "&_filter=" + _filter;
}