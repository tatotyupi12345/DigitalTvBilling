$(function () {
    
    $("#packages_body").on("click", "tr", function (e) {
        e.preventDefault();
        if ($(this).data("id") === undefined)
            return;

        if (e.target.id == 'edit_package') {
            getPackageModal($(this).data("id"));
            return;
        }

        if (e.target.id == 'remove_package')
        {
            onDeletePackage($(this).data("id"));
            return;
        }
        document.location.href = "/Channel/Channels/" + $(this).data("id");
    });

    $("#showModal").on("click", function () {
        getPackageModal(0);
    });
});

function onDeletePackage(id) {
    if (confirm("წაიშალოს პაკეტი?")) {
        $.post("/Channel/DeletePackage", { package_id: id }, function (data) {
            if (data === 1)
                $("#packages_body").find("tr[data-id='" + id + "']").remove();
            else
                alert("პაკეტის წაშლა ვერ მოხერხდა. პაკეტი შეიცავს არხებს.")
        }, "json");
    }
}

function getPackageModal(id) {
    $.get("/Channel/Package/" + id, {}, function (data) {
        var content = $(data);
        content.modal("show");
        content.on('hidden.bs.modal', function () {
            content.remove();
        });
        $.validator.unobtrusive.parse(content.find("#package_form"));
        content.find("form").each(function (i, form) {
            GeoKBD.map($(form).attr("name"), $.map($(form).find("input[type='text'], textarea"), function (item, k) {
                return $(item).attr("name");
            }), $(form).get(0));
        });

        content.find("form").submit(function (e) {
            setFieldsChange($(this).find("input[data-tag]"));
        });
        content.find(":checkbox").checkboxpicker(); 
        if (content.find("#is_group").prop('checked') == true)
        {
            content.find(".none_group").hide();
            content.find(".in_group").show();
        }
        else
        {
            content.find(".none_group").show();
            content.find(".in_group").hide();
        }
        content.find("#is_group").on('change', function () {
            if ($(this).prop("checked") == true) {
                content.find(".none_group").hide(300);
                content.find(".in_group").show(300);
            }
            else {
                content.find(".none_group").show(300);
                content.find(".in_group").hide(300);
            }
        });
    }, "html");
}

function onSuccessPackage(res) {
    if (res === "1") {
        $("#package_modal").modal("hide");
        document.location.href = "/Channel";
    }
}