var txt_filter;
var filter_abonent;
var filter_abonent_result = 0;
var filter_name;
$(function () {

    filter_abonent = $("#filter_abonent");
    $("#drp_filter_call").on("change", function (e) {
        filter_abonent_result = parseInt($(this).children(":selected").attr("value"));
        if (filter_abonent_result == 0) {
            $("#_filter").css("display", "none");
        }
        filter(filter_name, 1);
    });

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

    txt_filter = $("#txt_filter");


    txt_filter.keyup(function (e) {
        filter($(this).val(), 1);
        filter_name = $(this).val();
        $(".pagination-container").remove();
    });

    function filter(letter, page) {
        if (page === undefined) return;
        if (timer) {
            clearTimeout(timer);
        }

        if (letter.length < 3 || filter_abonent_result == 0) {
            $("#_filter").css("display", "none");
            return;
        }
        else {
            $("#_filter").css("display", "block");
        }
        timer = setTimeout(function () {
            switch (filter_abonent_result) {

                case 1:

                    $.post("/Order/FilterOrdersByName", { letter: letter, user_id: 0 , page: page }, function (data) {
                        var str = '';
                        var str_user = '<option id="' + 0 + '" value="' + 0 + '" data-value="' + 0 + '" onclick="groupsChange(' + 0 + ')">' + 0 + '</option>';
                       
                        $.each(data.Abonents, function (i, val) {
                            
                            val.data = JSON.parse(val.data);
                            var nn = returnUserOrder(data.execs, val.id, val.changer_user);
                            var get_date = new Date(parseInt(val.get_date.replace("/Date(", "").replace(")/", ""), 10));
                            get_date = get_date.yyyymmdd();
                            var change_date = new Date(parseInt(val.change_date.replace("/Date(", "").replace(")/", ""), 10));
                            change_date = change_date.yyyymmdd();
                            var tdate = new Date(parseInt(val.tdate.replace("/Date(", "").replace(")/", ""), 10));
                            tdate = tdate.yyyymmdd();
                            var image_to_go = "";
                            var image_name = returnImage(data.execs, val.executor_id);
                            var image = '/tStyles/dist/img/user9-128x128.png';
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
                                image_to_go = '/tStyles/dist/img/waiting.gif';
                            }
                            if (val.status == 7) {
                                image_to_go = '/tStyles/dist/img/closed1.gif';
                                class_color = "closed";
                            }
                            str += '<tr class="header"></tr>'+'<tr data-code="' + val.code + '" data-id="' + val.id + '" data-card ="' + val.CardNum + '" class="shadow vamp  ' + (class_color) + '">' +
                                '<td class="pictures">' + ' <img src="' + image + '" class="user-image picturesimg" alt="User Image" id="user_image_' + val.id + '" />' + '<img id="work_id_' + val.executor_id + '" src="' + image_to_go + '" class="pictures_work" />'
                                + ' <p class="statusBar">სტატუსი</p>'
                                + '<div >' + '<div>' + ' <select class="form-control shadow input-sm" id="group_combo">' +
                                '<option id="0"></option>' + returnUserOrder(data.execs, val.id, val.exec_name) + '</select>' +
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
                                '<td class=' + class_color + ' >' + val.create_user + '<br/>' + '<p>' + val.changer_user + '</p>' + '<br/>' + '<p>' + val.approve_user + '</p>' + '</td><td  class=' + class_color + '>' + val.data.Customer.Desc + '</td><td class=' + class_color + ' >' + val.comment + '</td><td class=' + class_color + ' >'
                                + data.ordeReserve.filter(function (filt) { return filt.order_id == val.id }).map(function (item) { return orderAnswers(item.reserve_answer); }), + '</td></tr>';
                        });
                        var clones = filter_abonent.clone();
                        clones.html('');
                        clones.append(str);
                        txtDate(clones);
                        content_group_combo(clones);

                        filter_abonent.html('');
                        filter_abonent.after(clones);
                    }, "json");
                    break;
                case 2:

                    $.post("/Damage/FilterDamageByName", { letter: letter, user_id: 0, page: page }, function (data) {
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
                            str += '<tr data-code="' + val.code + '" data-id="' + val.id + '" data-card ="' + val.CardNum + '" class="shadow vamp  ' + (val.montage_status ? ("success") : ("")) + '">' +
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
                                '</div>' + '</div>' + '</div>' + '</td><td>' + tdate + '<br />' + '<p id="change_date_' + val.id + '">' + get_date + '</p>' + '<br />' + '<p>' + change_date + '</p>' + '</td>' +
                                '<td>' + val.name.split("/")[0] + '<br/>' + '<p>' + val.code + '</p>' + '</td><td>' + val.data.Customer.City + '<br />' + val.data.Customer.District + '<br/><p>' + val.data.Customer.Region + '</p>' + '<br />' + '<p>' + val.data.Customer.Village + '</p>' + '</td><td>' + val.data.Customer.Phone1 + '<br />' + '<p>' + val.data.Customer.Phone2 + '</p>' + '</td><td>' + val.receivers_count + '</td><td class="_status">' + getStatusDesc(val.status) + '<label id="user_name_group_' + val.id + '">' + '</label>' + '</td>' +
                                '<td>' + val.create_user + '<br/>' + '<p>' + val.changer_user + '</p>' + '<br/>' + '<p>' + val.approve_user + '</p>' + '</td><td>' + val.data.Customer.Desc + '</td><td class=' + class_color + ' >' + val.comment + '</td><td class=' + class_color + ' >'
                                + data.damageReserve.filter(function (filt) { return filt.damage_id == val.id }).map(function (item) { return damageAnswers(item.reserve_answer); }), + '</td></tr>';
                        });
                        var clones = filter_abonent.clone();
                        clones.html('');
                        clones.append(str);
                        txtDate(clones);
                        content_group_combo(clones);

                        filter_abonent.html('');
                        filter_abonent.after(clones);
                        //paging.data("mode", "ajax").html(data.Paging);
                    }, "json");
                    break;
                case 3:
                    $.post("/Cancellation/FilterCancellactionByName", { letter: letter, user_id: 0, page: page }, function (data) {
                        var str = '';
                        var str_user = '<option id="' + 0 + '" value="' + 0 + '" data-value="' + 0 + '" onclick="groupsChange(' + 0 + ')">' + 0 + '</option>';

                        $.each(data.Abonents, function (i, val) {
                            val.data = JSON.parse(val.data);
                            var nn = returnUserCancellation(data.execs, val.id, val.changer_user)
                            var get_date = new Date(parseInt(val.get_date.replace("/Date(", "").replace(")/", ""), 10));
                            get_date = get_date.yyyymmdd();
                            var change_date = new Date(parseInt(val.change_date.replace("/Date(", "").replace(")/", ""), 10));
                            change_date = change_date.yyyymmdd();
                            var tdate = new Date(parseInt(val.tdate.replace("/Date(", "").replace(")/", ""), 10));
                            tdate = tdate.yyyymmdd();
                            var image_to_go = "";
                            var image_name = returnImage(data.execs, val.executor_id);
                            var image = "/tStyles/dist/img /user9-128x128.png";
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
                                image_to_go = '/tStyles/dist/img/cancellation.gif';
                            }
                            if (val.status == 2) {
                                image_to_go = '/tStyles/dist/img/closed1.gif';
                                class_color = "closed";
                            }
                            str += '<tr data-code="' + val.code + '" data-id="' + val.id + '" data-card ="' + val.CardNum + '" class="shadow vamp  ' + (val.montage_status ? ("success") : ("")) + '">' +
                                '<td class="pictures">' + ' <img src="' + image + '" class="user-image picturesimg" alt="User Image" id="user_image_' + val.id + '" />' + '<img id="work_id_' + val.executor_id + '" src="' + image_to_go + '" class="pictures_work" />'
                                + ' <p class="statusBar">სტატუსი</p>'
                                + '<div >' + '<div>' + ' <select class="form-control shadow input-sm" id="group_combo">' +
                                '<option id="0"></option>' + returnUserCancellation(data.execs, val.id, val.exec_name) + '</select>' +
                                '<div class="">' +

                                ' <div class="">' +
                                '<div class="input-group input-group-sm date" id="txt_date" value="' + val.id + '">' +
                                '<input type="text" class="form-control input-sm" readonly />' +
                                '<a class="input-group-addon" style="cursor: pointer;">' +
                                '<span class="glyphicon glyphicon-calendar"></span>' +
                                '</a>' +
                                '</div>' +
                                '</div>' +
                                '</div>' + '</div>' + '</div>' + '</td><td>' + tdate + '<br />' + '<p id="change_date_' + val.id + '">' + get_date + '</p>' + '<br />' + '<p>' + change_date + '</p>' + '</td>' +
                                '<td>' + val.name.split("/")[0] + '<br/>' + '<p>' + val.code + '</p>' + '</td><td>' + val.data.Customer_City + '<br />' + val.data.Customer_District + '<br/><p>' + val.data.Customer_Region + '</p>' + '<br />' + '<p>' + val.data.Customer_Village + '</p>' + '</td><td>' + val.data.Customer_Phone1 + '<br />' + '<p>' + val.data.Customer_Phone2 + '</p>' + '</td><td>' + val.receivers_count + '</td><td class="_status">' + getStatusDesc(val.status) + '<label id="user_name_group_' + val.id + '">' + '</label>' + '</td>' +
                                '<td>' + val.create_user + '<br/>' + '<p>' + val.changer_user + '</p>' + '<br/>' + '<p>' + val.approve_user + '</p>' + '</td><td>' + val.data.Customer_Desc + '</td><td  class=' + class_color + '>' + val.data.Customer.Desc + '</td><td class=' + class_color + ' >' + val.comment + '</td></tr>';
                        });
                        var clones = filter_abonent.clone();
                        clones.html('');
                        clones.append(str);
                        txtDate(clones);
                        content_group_combo(clones);
                        filter_abonent.html('');
                        filter_abonent.after(clones);
                    }, "json");
                    break;
            }
        }, 300, letter);
    }
});
function orderAnswers(ansers) {
    var _ans = "";
    switch (ansers) {
        case 0:
            _ans = "";
            break;
        case 1:
            _ans = "დასრულება";
            break;
        case 2:
            _ans = "სხვა:მიზეზი";
            break;
        case 3:
            _ans = "არ არის სიგანალი";
            break;
        case 4:
            _ans = "ვერ მივიღე სიგნალი";
            break;
        case 5:
            _ans = "სხვა ოპერატორი ისარგებლა";
            break;
        case 6:
            _ans = "გადაიფიქრა";
            break;
        case 7:
            _ans = "არ აქვს ფული";
            break;
        case 8:
            _ans = "ვიზიტის გადადება";
            break;
    }
    return _ans;
}
function damageAnswers(ansers) {
    var _ans = "";
    switch (ansers) {
        case 0:
            _ans = "";
            break;
        case 1:
            _ans = "დასრულება";
            break;
        case 2:
            _ans = "სხვა:მიზეზი";
            break;
        case 3:
            _ans = "კაბელის დაზიანება";
            break;
        case 4:
            _ans = "ანტენის პრობლემა";
            break;
        case 5:
            _ans = "ბოქსის ქარხნული წუნი/შეცვლა";
            break;
        case 6:
            _ans = "კვების ბლოკის დაზიანება/გაყიდვა";
            break;
        case 7:
            _ans = "TV/AV";
            break;
        case 8:
            _ans = "მისამართის შეცვლა";
            break;
        case 9:
            _ans = "აქსესუარის დაზიანება/გაყიდვა";
            break;
        case 10:
            _ans = "სიგნალი აღარ აქვს";
            break;
    }
    return _ans;
}