function getChannelModal(id) {
    $.get("/Channel/Channel/" + id, { "package_id": package_id }, function (data) {
        var content = $(data);
        content.modal("show");
        content.on('hidden.bs.modal', function () {
            content.remove();
        });
        $.validator.unobtrusive.parse(content.find("#channel_form"));

        content.find("form").each(function (i, form) {
            GeoKBD.map($(form).attr("name"), $.map($(form).find("input[type='text'], textarea"), function (item, k) {
                return $(item).attr("name");
            }), $(form).get(0));
        });

        content.find("form").submit(function (e) {
            setFieldsChange($(this).find("input[data-tag]"));
        });
    }, "html");
}

function onDeleteChannel(id) {
    if (confirm("წაიშალოს არხი?")) {
        $.post("/Channel/DeleteChannel", { id: id, package_id: package_id }, function (data) {
            if (data === 1)
                $("#channels_body").find("tr[data-id='" + id + "']").remove();
        }, "json");
    }
}

function onSuccessChannel(res) {
    if (res === "1") {
        $("#channel_modal").modal("hide");
        document.location.href = document.location.href;
    }
}