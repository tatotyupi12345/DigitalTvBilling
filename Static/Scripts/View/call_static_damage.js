var timer = null;
var user_ids;
var drp_filter;
var orders_body;
var txt_filter;
$(function () {
    menu_dialog = $("#order_dialog");
});

function ButtonClickDamage(user_id) {
    user_ids = user_id;

    $.post("/CallCenter/HistoryShowDamage/", { user_id: user_id, check_user: false }, function (data) {
        var content = $(data);
        content.modal("show");
        $("body").css("overflow", "hidden");
        content.find("#height_damage").css("height", $(document).height() - 950);
        content.on('hidden.bs.modal', function () {
            content.remove();
            $("body").css("overflow", "Enable");
        });
        changeDamageContent(content);
    }, "html");
}
function ButtonClickDamageCategorized(user_id) {
    user_ids = user_id;

    $.post("/CallCenter/DamageCategorized/", { user_id: user_id, check_user: false }, function (data) {
        var content = $(data);
        content.modal("show");
        content.on('hidden.bs.modal', function () {
            content.remove();
        });
        changeDamageContent(content);
    }, "html");
}
function content_group_combo_damage(content) {
    content.find("#group_combo").on("change", function (b) {
        var ids = $(this).children("tr").attr("value");
        var image = $(this).children(":selected").attr("data-value")
        var id = $(this).children(":selected").attr("value");
        content.find("#user_image_" + id).attr("src", image);
        content.find("#user_name_" + id).text($(this).children(":selected").text());
        content.find("#user_name_group_" + id).text($(this).children(":selected").text());
        if (content.find("#user_name_group_" + id).attr("data-id") != $(this).children(":selected").attr("id") /*&& content.find("#user_name_group_" + id).attr("data-id") != 0*/) {
            $("#static_damage_count_" + content.find("#user_name_group_" + id).attr("data-id")).text($("#static_damage_count_" + content.find("#user_name_group_" + id).attr("data-id")).text() - 1);
            $("#static_damage_remainder_" + content.find("#user_name_group_" + id).attr("data-id")).text($("#static_damage_remainder_" + content.find("#user_name_group_" + id).attr("data-id")).text() - 1);
            $("#static_damage_count_" + $(this).children(":selected").attr("id")).text(parseInt($("#static_damage_count_" + $(this).children(":selected").attr("id")).text()) + 1);
            $("#static_damage_remainder_" + $(this).children(":selected").attr("id")).text(parseInt($("#static_damage_remainder_" + $(this).children(":selected").attr("id")).text()) + 1);
        }

        $.post("/Damage/GroupChange", { order_id: id, ids: id, group_id: $(this).children(":selected").attr("id") }, function (res) {

        }, "json");
    });
}
function content_change_statusDamage(content) {
    content.find("#damage_status_filter").on("change", function () {
        var id = $(this).children(":selected").attr("value");

        $.post("/Damage/ChangeStatus", { ids: id, status: $(this).children(":selected").attr("data-value") }, function (res) {

        }, "json");
    });
}
function txtDateDamage(content) {
    content.find("#txt_date").datepicker({
        autoclose: true,
        language: "ka",
        format: 'yyyy-mm-dd'
    }).datepicker('update', new Date().toDateString()).on("changeDate", function (ev) {
        $(this).datepicker("hide");
        var ids = $(this).attr("value");
        content.find("#change_date_" + ids).html(content.find(this).find("input").val());
        $.post("/Damage/ChangeDate", { ids: ids == "" ? id : ids, date: content.find(this).find("input").val() }, function (res) {

        }, "json");
    });
}
function changeDamageContent(content) {

    content.find("#check_user").on("click", function () {
        if ($(this).is(':checked')) {
            $.post("/CallCenter/HistoryShowDamage/", { user_id: user_ids, check_user: true }, function (data) {
                var clone = $(data);
                txtDateDamage(clone);
                content_group_combo_damage(clone);
                content_change_statusDamage(clone);
                content.find("#orders_body").html('');
                content.find("#orders_body").append(clone.find("#clone"));

                //changeContent(content);
            }, "html");
        }
        else {
            $.post("/CallCenter/HistoryShowDamage/", { user_id: user_ids, check_user: false }, function (data) {
                var clone = $(data);
                txtDateDamage(clone);
                content_group_combo_damage(clone);
                content_change_statusDamage(clone);
                content.find("#orders_body").html('');
                content.find("#orders_body").append(clone.find("#clone"));

                //changeContent(content);
            }, "html");
        }

    });
    // date change
    txtDateDamage(content);
    //user change
    content_group_combo_damage(content);
    content_change_statusDamage(content);

    orders_body = content.find("#orders_body");
    Date.prototype.yyyymmdd = function () {
        var mm = this.getMonth() + 1;
        var dd = this.getDate();
        var hr = this.getHours();
        var min = this.getMinutes();

        return [this.getFullYear() + '/',
        (mm > 9 ? '' : '0') + mm + '/',
        (dd > 9 ? '' : '0') + dd + ' ', (hr > 9 ? '' : '0') + hr + ':', (min > 9 ? '' : '0') + min
        ].join('');
    };

    txt_filter_damage = content.find("#txt_filter_damage");


    txt_filter_damage.keyup(function (e) {
        filter($(this).val(), 1);
        $(".pagination-container").remove();
    });

    function filter(letter, page) {
        if (page === undefined) return;
        if (timer) {
            clearTimeout(timer);
        }

        if (letter.length < 3)
            return;

        timer = setTimeout(function () {
            $.post("/Damage/FilterDamageByName", { letter: letter, user_id: user_ids, page: page }, function (data) {
                var str = '';
                var str_user = '<option id="' + 0 + '" value="' + 0 + '" data-value="' + 0 + '" onclick="groupsChange(' + 0 + ')">' + 0 + '</option>';

                $.each(data.Abonents, function (i, val) {
                    val.data = JSON.parse(val.data);
                    var nn = returnUserDamage(data.execs, val.id, val.changer_user);
                    var get_date = new Date(parseInt(val.get_date.replace("/Date(", "").replace(")/", ""), 10));
                    get_date = get_date.yyyymmdd();
                    var change_date = new Date(parseInt(val.change_date.replace("/Date(", "").replace(")/", ""), 10));
                    change_date = change_date.yyyymmdd();
                    var tdate = new Date(parseInt(val.tdate.replace("/Date(", "").replace(")/", ""), 10));
                    tdate = tdate.yyyymmdd();
                    var image_to_go = "";
                    var image_name = returnImage(data.execs, val.executor_id);
                    var image = "/tStyles/dist / img / user9 - 128x128.png"
                    var name = "";

                    var class_color = "";
                    if (image_name.length == 0) {
                        image = '/tStyles/dist/img/user9-128x128.png';
                    }
                    else {

                        image = image_name[0].image;
                        name = image_name[0].name;
                    }
                    if (val.to_go == 1) {
                        image_to_go = '/tStyles/dist/img/work1.gif';
                    }
                    else {
                        image_to_go = '/tStyles/dist/img/damage.gif';
                    }
                    if (val.status == 7) {
                        image_to_go = '/tStyles/dist/img/closed1.gif';
                        class_color = "closed";
                    }
                    str += '<tr class="header"></tr>' + '<tr data-code="' + val.code + '" data-id="' + val.id + '" data-card ="' + val.CardNum + '" class="shadow vamp  ' + (val.montage_status ? ("success") : ("")) + '">' +
                        '<td class="pictures">' + ' <img src="' + image + '" class="user-image picturesimg" alt="User Image" id="user_image_' + val.id + '" />' + '<img id="work_id_' + val.executor_id + '" src="' + image_to_go + '" class="pictures_work" />'
                        + ' <p class="statusBar">სტატუსი</p>'
                        + '<div >' + '<div>' + ' <select class="form-control shadow input-sm" id="group_combo">' +
                        '<option id="0"></option>' + returnUserDamage(data.execs, val.id, val.exec_name) + '</select>' +
                        '<div class="">' +

                        ' <div class="">' +
                        '<div class="input-group input-group-sm date" id="txt_date" value="' + val.id + '">' +
                        '<input type="text" class="form-control input-sm" readonly />' +
                        '<a class="input-group-addon" style="cursor: pointer;">' +
                        '<span class="glyphicon glyphicon-calendar"></span>' +
                        '</a>' +
                        '</div>' +
                        '</div>' +
                        '</div>' + '</div>' + '</div>' + '</td><td class=' + class_color + '>' + tdate + '<br />' + '<p id="change_date_' + val.id + '">' + get_date + '</p>' + '<br />' + '<p>' + change_date + '</p>' + '</td>' +
                        '<td class=' + class_color + '>' + val.name.split("/")[0] + '<br/>' + '<p>' + val.code + '</p>' + '</td><td  class=' + class_color + '>' + val.data.Customer.City + '<br />' + val.data.Customer.District + '<br/><p>' + val.data.Customer.Region + '</p>' + '<br />' + '<p>' + val.data.Customer.Village + '</p>' + '</td><td  class=' + class_color + '>' + val.data.Customer.Phone1 + '<br />' + '<p>' + val.data.Customer.Phone2 + '</p>' + '</td><td  class=' + class_color + '>' + val.receivers_count + '</td><td class="_status ' + class_color + '">' + getStatusDesc(val.status) + '<label id="user_name_group_' + val.id + '">' + '</label>' + '</td>' +
                        '<td class=' + class_color + ' >' + val.create_user + '<br/>' + '<p>' + val.changer_user + '</p>' + '<br/>' + '<p>' + val.approve_user + '</p>' + '</td><td  class=' + class_color + '>' + val.data.Customer.Desc + '</td></tr>';
                });
                var clones = orders_body.clone();
                clones.html('');
                clones.append(str);
                txtDate(clones);
                content_group_combo(clones);

                orders_body.html('');
                orders_body.after(clones);
            }, "json");
        }, 300, letter);
    }
}


function getStatusDesc(stat) {
    var ret = "";
    switch (stat) {

        case 0:
            ret = "დარეგისტრირდა";
            break;

        case 1:
            ret = "მონტაჟი";
            break;

        case 2:
            ret = "გაუქმება";
            break;

        case 3:
            ret = "დამუშავება";
            break;

        case 4:
            ret = "გადადება";
            break;

        case 5:
            ret = "ლოდინი";
            break;

        case 6:
            ret = "გაგზავნა";
            break;

        case 7:
            ret = "დასრულება";
            break;

        default:
            break;
    }

    return ret;
}
Array.prototype.map = function (projectionFunction) {
    var results = [];
    this.forEach(function (itemInArray) {
        results.push(projectionFunction(itemInArray));

    });

    return results;
};
function groupsChange(id) {
    var ids = id;
    $.post("/Damage/GroupChange", { order_id: id, ids: ids == "" ? id : ids, group_id: user_ids }, function (res) {
        if (res === 1) {

        }

    }, "json");
}
function returnUserDamage(user, order_id, exec_name) {
    return user.map(function (data) {
        if (data.Name == exec_name) {
            return str_user = '<option selected id="' + data.Id + '" value="' + order_id + '" data-value="' + data.image + '" onclick="groupsChange(' + data.Id + ')">' + exec_name + '</option>';
        }
        else {
            return str_user = '<option id="' + data.Id + '" value="' + order_id + '" data-value="' + data.image + '" onclick="groupsChange(' + data.Id + ')">' + data.Name + '</option>';
        }
    });
}
function returnImage(user, user_id) {
    return user.filter(function (data) {
        return data.Id == user_id
    }).map(function (image) {
        return { image: image.image, name: image.Name };
    });
}
