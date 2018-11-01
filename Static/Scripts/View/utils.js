
$(function () {

    $("#newOfficeCard").on("click", function () {
        openNewOfficeCard(0);
    });

    $("#cards_body").on("click", "tr", function (e) {
        openNewOfficeCard($(this).data("id"));
    });

    $("#newAutoSubscrib").on("click", function (e) {
        $.get("/Utils/NewAutoSubscrib", {}, function (data) {
            var content = showModal(data);
            content.find("#find").on("click", function (e) {
                e.preventDefault();
                onFindCards(content);
            });

            content.find("#txt_date").datepicker({
                autoclose: true,
                language: "ka",
                format: 'yyyy-mm-dd'
            }).datepicker('update', new Date().toDateString()).on("changeDate", function (ev) {
                $(this).datepicker("hide");
            });

            content.find("#is_change_date").on("change", function () {
                $("#SendDate").prop('disabled', !$(this).is(':checked'));
            });

            content.find("#add_subscription").on("click", function () {
                $.get("/Abonent/AddSubscription/", { id: 0, type: 1 }, function (data) {
                    var cont = showModal(data);
                    cont.find("#add_new_subscrb").on("click", function (e) {
                        e.preventDefault();
                        var str = '';
                        var name = '';
                        $.each(cont.find("input:hidden[id^='id_']"), function (i, val) {
                            str += '<input type="hidden" name="Packages[' + i + ']" value="' + $(val).val() + '" />';
                            name += '+' + $(val).data("name");
                        });
                        content.find("#subscribs").html(str);
                        content.find("#subscribs_names").html(name.substr(1));
                        cont.modal("hide");
                    });
                }, "html");
            });

        });
    });

    $("#autopackages_body").on("click", "button", function (ev) {
        ev.preventDefault();
        if (confirm("წაიშალოს აღნიშნული ჩანაწერი?")) {
            var btn = $(this);
            $.post("/Utils/DeleteAutoSubscrib", { id: btn.data("id") }, function (res) {
                if (res === 1)
                    btn.closest("tr").remove();
            }, "json");
        }
    });
});


function onFindCards(content) {
    $.post("/api/data/getcards", { type: content.find("#find_by").find("option:selected").val(), value: content.find("#payment_find").val(), secure: generateSecure() }, function (data) {
        var buttons = '';
        content.find("#cards, #cards_area, #card_message, #abonent_area, #subscribs,#subscribs_names").empty();
        $.each(data.groups, function (i, group) {
            var id = '';
            var txt = '';
            $.each(group.cards, function (j, card) {
                txt += '<p>' + card.name + '</p>';
                id += "," + card.id;
            });
            buttons += '<button type="button" class="btn btn-default btn-xs" data-id="' + id.substr(1, id.length) + '">' + txt + '</button>';
        });
        if (data.abonent.name != null)
            content.find("#abonent_area").html('<div class="alert alert-success" role="alert">' + data.abonent.name + '</div>');
        else
            content.find("#abonent_area").html('<div class="alert alert-danger" role="alert">ბარათი ვერ მოიძებნა</div>');
        var area = content.find("#cards_area")
        area.html(buttons).on("click", "button", function (ev) {
            ev.preventDefault();
            var btn = $(this);
            area.find("button").attr("class", "btn btn-xs btn-default");
            btn.attr("class", "btn btn-xs active btn-primary");
            var str = '';
            $.each(btn.data("id").toString().split(','), function (i, val) {
                str += '<input id="Cards_' + i + '_" name="Cards[' + i + ']" value="' + val + '" type="hidden">';
            });
            content.find("#cards").html(str);
        });

    }, "json");
}

function openNewOfficeCard(id) {
    $.get("/Utils/NewOfficeCard", {id: id}, function (data) {
        var content = showModal(data);
        $.validator.unobtrusive.parse(content.find("#office_card_form"));
        content.find("form").each(function (i, form) {
            GeoKBD.map($(form).attr("name"), $.map($(form).find("input[type='text']"), function (item, k) {
                return $(item).attr("name");
            }), $(form).get(0));
        });
        content.find("form").submit(function (e) {
            setFieldsChange($(this).find("input[data-tag]"));
        });

        content.find("#delete_office_card").on("click", function (e) {
            if (confirm("წაიშალოს ბარათი?")) {
                $.post("/Utils/DeleteOfficeCard", { id: $(this).data("id") }, onSuccessNewNewOfficeCard, "json");
            }
        });

        content.find("#add_subscription").on("click", function () {
            $.get("/Abonent/AddSubscription/", { id: 0, type: 1 }, function (data) {
                var cont = showModal(data);
                cont.find("#add_new_subscrb").on("click", function (e) {
                    e.preventDefault();
                    var str = '';
                    var name = '';
                    $.each(cont.find("input:hidden[id^='id_']"), function (i, val) {
                        str += '<input type="hidden" name="Packages[' + i + ']" value="' + $(val).val() + '" />';
                        name += '+' + $(val).data("name");
                    });
                    content.find("#subscribs").html(str);
                    content.find("#subscribs_names").html(name.substr(1));
                    cont.modal("hide");
                });
            }, "html");
        });

    });
}

function onSuccessNewNewOfficeCard(res) {
    if (res === 1)
        location.href = "/Utils/OfficeCards";
}

function onSuccessNewAutoSubscrib(res) {
    if (res === 1)
        location.href = "/Utils/AutoSubscribs";
    else
        alert(res);
}

Date.prototype.toSecureDate = function () {
    return this.getFullYear() + "-" + ((this.getMonth() + 1).toString().length === 1 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1)) + "-" + (this.getDate().toString().length === 1 ? "0" + this.getDate() : this.getDate()) + " " + (this.getHours().toString().length === 1 ? "0" + this.getHours() : this.getHours()) + ":" + (this.getMinutes().toString().length === 1 ? "0" + this.getMinutes() : this.getMinutes()) + ":" + (this.getSeconds().toString().length === 1 ? "0" + this.getSeconds() : this.getSeconds());
}
function generateSecure() {
    var dt = new Date();
    return { "date": dt.toSecureDate(), "key": md5(dt.toSecureDate() + (dt.getFullYear() + (dt.getMonth() + 1) + dt.getDate() + dt.getHours() + dt.getMinutes() + dt.getSeconds()).toString()) };
}