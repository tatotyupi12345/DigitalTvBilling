var mode_fields = {
    '0': [{ name: 'c.name+c.lastname', value: 'სახელი, გვარი' }, { name: 'c.code', value: 'პ/ნ' }, { name: 'cr.doc_num', value: 'ხელშეკრულების №' }, { name: 'c.city', value: 'ქალაქი' }, { name: 'c.phone1', value: 'ტელეფონი' }, { name: 'cr.tower_id', value: 'ანძა' }],
    '1': [{ name: 'c.abonent_num', value: 'აბონენტის №' }, { name: 'c.card_num', value: 'ბარათის №' }, { name: 'c.status', value: 'სტატუსი' }, { name: 'cr.tower_id', value: 'ანძა' }],
    '2': [{ name: 'c.name+c.lastname', value: 'აბონენტი' }, { name: 'crd.abonent_num', value: 'აბონენტის №' }, { name: 'crd.card_num', value: 'ბარათის №' }],
    '3': [{ name: 'c.name', value: 'დასახელება' }, { name: 'c.price', value: 'ფასი' }],
    '4': [{ name: 'c.name', value: 'დასახელება' }],
    '5': [{ name: 'c.name+c.lastname+c.code', value: 'აბონენტი' }, { name: 'c.city', value: 'ქალაქი' }, { name: 'c.region', value: 'რეგიონი' }],
    '6': [{ name: 'c.name', value: 'სახელი' }, { name: 'c.login', value: 'შესვლის სახელი' }],
    '7': [{ name: 'c.name', value: 'დასახელება' }],
    '8': [{ name: 'c.name', value: 'დასახელება' }],
    '9': [{ name: 'c.name', value: 'დასახელება' }],
    '10': [],
    '11': [{ name: 'user', value: 'მომხმარებლები' }, { name: 'usergroup', value: 'მომხმარებლის ჯგუფები' }],
    '12': [{ name: 'c.name', value: 'დასახელება' }],
    '13': [{ name: 'c.name', value: 'დასახელება' }],
    '14': [{ name: 'c.name', value: 'დასახელება' }],
    '15': [{ name: 'c.name', value: 'დასახელება' }, { name: 'c.code', value: 'ს/კ' }],
    '16': [{ name: 'c.text', value: 'დასახელება' }, ],
    '17': [{ name: 'c.address', value: 'მისამართი' }, { name: 'c.name', value: 'სახელი, გვარი' }, { name: 'c.card_num', value: 'ბარათის №' }],
    '18': [],
    '19': [{ name: 'c.name', value: 'დასახელება' }]
};

var types = [{ value: 0, name: 'აბონენტი' }, { value: 1, name: 'ბარათი' }, { value: 2, name: 'გადახდა' }, { value: 3, name: 'პაკეტი' },
    { value: 4, name: 'არხი' }, { value: 5, name: 'შეტყობინება' }, { value: 6, name: 'მომხმარებელი' }, { value: 7, name: 'მომხმარებლის ჯგუფი' },
    { value: 8, name: 'რესივერი' }, { value: 9, name: 'ანძა' }, { value: 10, name: 'ბარათის პაკეტი' }, { value: 11, name: 'მომხმარებლის მოქმედება' }, {value:12, name: 'პარამეტრები'},
    { value: 13, name: 'გაწეული მომსახურება' }, { value: 14, name: 'ბარათის მომსახურება' }, { value: 15, name: 'შეკვეთა' }, { value: 16, name: 'მიზეზი' },
    { value: 17, name: 'ოფისის ბარათი' }, { value: 18, name: 'ინვოისი' }, { value: 19, name: 'გადახდის სახეობა' }, ];

var modes = [{ value: 0, name: 'ცვლილება' }, { value: 1, name: 'დამატება' }, { value: 2, name: 'წაშლა' }, { value: 3, name: 'ბარათის მოქმედებება' }, { value: 4, name: 'ავტორიზაცია' }];

var timer = null;
var txt_filter;
var dropdown_cols;
var dropdown_type;
var timer;
var previousValue;
var loggin_list;
var paging;
var export_btn;

$(function () {
    $(".datepickers").datePickers({ prefix: "picker" });

    dropdown_cols = $("#id_cols");
    dropdown_type = $("#id_type");
    txt_filter = $("#FilterText");
    loggin_list = $("#logging_body");
    paging = $("#paging");
    var filter_area = $("#filter_area");
    export_btn = $("#btn_excel");

    nav_url = $(this).find("#picker_picker_nav");
    $("#logging_body").on("click", "tr", function (e) {
        var id = $(this).data("id");
        $.get("/Main/GetLogDetails", { id: id }, function (data) {
            var content = $(data);
            content.modal("show");
            content.on('hidden.bs.modal', function () {
                content.remove();
            });
        });
    });

    dropdown_cols.on("change", function () {
        txt_filter.val("").focus();
        var field = $(this).find("option:selected").val();
        if (field == "c.status") {

            filter_area.data("sign", "select").html('<select class="form-control input-sm" id="card_status" onchange="cardStatusChange(this)">' +
                '<option value="-1"></option>' +
                '<option value="0">აქტიური</option>' +
                '<option value="1">გათიშული</option>' +
                '<option value="2">დაპაუზებული</option>' +
                '<option value="3">მონტაჟი</option>' +
                '<option value="4">გაუქმებული</option></select>');
            return;
        } else if (field === "user" || field === "usergroup") {
            $.post("/Main/GetUserData", { sign: field }, function (data) {
                var str = '<select class="form-control input-sm" id="user_combo" onchange="usersChange(this)"><option></option>';
                $.each(data, function (i, val) {
                    str += '<option value="' + val.id + '">' + val.name + '</option>';
                });
                str += '</select>';
                filter_area.data("sign", "select").html(str);
            }, "json");
        } else if (field == "c.tower_id" || field == "cr.tower_id") {
            $.post("/Abonent/GetTowers", {}, function (data) {
                var str = '<select class="form-control input-sm" id="tower_combo" onchange="towersChange(this)"><option></option>';
                $.each(data, function (i, val) {
                    str += '<option value="' + val.id + '">' + val.name + '</option>';
                });
                str += '</select>';
                filter_area.data("sign", "select").html(str);
            }, "json");
        }
        else {
            if (filter_area.data("sign") === "select") {
                filter_area.data("sign", "text").html('<input type="search" class="form-control input-sm" onkeyup="onFilterKeyUp(this)" id="txt_filter" autofocus>');
                return;
            }
        }
    });

    dropdown_type.on("change", function (e) {
        onChangeField();
    });

    if (dropdown_type.find("option:selected").val() !== "") {
        onFillFields();
    }

    export_btn.on("click", function (e) {
        e.preventDefault();
        var q = filter_area.children().eq(0);
        var url = "/Main/ExportToExcel?type=" + dropdown_type.find("option:selected").val() + "&field=" + dropdown_cols.find("option:selected").val()
                        + "&dt_from=" + $("#picker_picker_from").val().ReplaceAll("/", "") + "&dt_to=" + $("#picker_picker_to").val().ReplaceAll("/", "")
                        + "&letter=" + (q.is("select") ? q.find("option:selected").val() : q.val());
        location.href = url;
    });

});

function onFillFields() {
    var val = dropdown_type.find("option:selected").val();
    if(val === "10") {
        $.post("/Main/GetPackages", { }, function (data) {
            fillfields(data);
        }, "json");
    }else {
        fillfields(mode_fields[val]);
    }
    export_btn.removeClass("hide").addClass("show");
}

function fillfields(items) {
    str = '<option value="0">--ყველა--</option>';
    $.each(items, function (i, item) {
        str += '<option value="' + item.name + '">' + item.value + '</option>';
    });
    dropdown_cols.html(str);
    dropdown_cols.trigger("change");
}

function onChangeField() {
    var value = dropdown_type.find("option:selected").val();
    location.href = getUrl(value);
}

function cardStatusChange(select) {
    filter($(select).find("option:selected").val(), 1);
}

function onFilterKeyUp(input) {
    filter($(input).val(), 1);
}

function usersChange(select) {
    filter($(select).find("option:selected").val(), 1);
}

function towersChange(select) {
    filter($(select).find("option:selected").val(), 1);
}

function getUrl(type_val) {
    return '?type=' + type_val;
}

function getTypeName(type) {
    var res = '';
    $.each(types, function (i, val) {
        if (val.value === type) {
            res = val.name;
            return false;
        }
    });
    return res;
}

function getModeName(mode) {
    var res = '';
    $.each(modes, function (i, val) {
        if (val.value === mode) {
            res = val.name;
            return false;
        }
    });
    return res;
}

function filter(letter, page) {
    if (letter != previousValue) {
        if (timer) clearTimeout(timer);
        var str = '';
        timer = setTimeout(function () {
            var col = dropdown_cols.find("option:selected").val();
            if ((col === "c.status" || col === "c.tower_id" || col === "cr.tower_id" || col === "user" || col === "usergroup") || letter.length > 1) {
                $.post("/Main/FilterLogs",
                    {
                        field: col,
                        letter: letter,
                        type: dropdown_type.find("option:selected").val(),
                        dt_from: $("#picker_picker_from").val().ReplaceAll("/", ""),
                        dt_to: $("#picker_picker_to").val().ReplaceAll("/", "")
                    }, function (data) {

                        $.each(data, function (i, log) {
                            str += '<tr data-id="' + log.Id + '" data-mode="' + log.Mode + '">' +
                                   '<td>' + parseJsonDate(log.Tdate).toDateString() + '</td>' +
                                   '<td>' + log.UserGroupName + '</td>' +
                                   '<td>' + log.UserName + '</td>' +
                                   '<td>' + getTypeName(log.Type) + '</td>' +
                                   '<td>' + getModeName(log.Mode) + '</td>' +
                                   '<td>' + log.Value + '</td></tr>';
                        });
                        loggin_list.html(str);
                        paging.empty();
                    }, "json");
            }
        }, 300, letter);

        previousValue = letter;
    }
}