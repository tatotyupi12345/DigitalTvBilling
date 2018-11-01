function GetCustomerTypeDesc(type) {
    if (type === 0)
        return "ფიზიკური";
    else if(type===1)
        return "იურიდიული";
    else return "ტექნიკური";
}

function SetCardStatus(status) {
    var res = '';
    switch (status) {
        case 0:
            res = 'აქტიური';
            break;
        case 1:
            res = 'გათიშული';
            break;
        case 2:
            res = 'დაპაუზებული';
            break;
        case 3:
            res = 'მონტაჟი';
            break;
        case 4:
            res = 'გაუქმებული';
            break;
        case 5:
            res = 'დაბლოკილი';
            break;
        case 7:
            res = 'იჯარა';
            break;
        case 8:
            res = 'შეწყვეტილი';
            break;
    }
    return res;
}

function setFieldsChange(fields) {
    var arr = [];
    $.each(fields, function (i, val) {
        var input = $(val);
        if (input.attr("id") != undefined) {
            if (input.is("input") || input.is("textarea")) {
                if (input.attr("type") === "checkbox") {
                    if (input.prop("checked").toString() !== input.data("tag").toLowerCase())
                        arr.push({ "field": $("label[for='" + input.attr("id") + "']").html(), "old_val": input.is(":checked") ? "ჩართული" : "გამორთული", "new_val": input.is(":checked") ? "გამორთული" : "ჩართული", "type": input.attr("id").indexOf("Customer") == 0 ? "customer" : input.closest(".well").find(":hidden").eq(0).val() + '@' + input.closest(".well").find("input[type='text']").eq(1).val() });
                } else {
                    if (input.val() !== input.data("tag").toString())
                        arr.push({ "field": $("label[for='" + input.attr("id") + "']").html(), "old_val": input.data("tag"), "new_val": input.val(), "type": input.attr("id").indexOf("Customer") == 0 ? "customer" : input.closest(".well").find(":hidden").eq(0).val() + '@' + input.closest(".well").find("input[type='text']").eq(1).val() });
                }
            } else {
                if (input.find("option:selected").val() !== input.data("tag").toString())
                    arr.push({ "field": $("label[for='" + input.attr("id") + "']").html(), "old_val": input.find("option[value=" + input.data("tag") + "]").text(), "new_val": input.find("option:selected").text(), "type": input.attr("id").indexOf("Customer") == 0 ? "customer" : input.closest(".well").find(":hidden").eq(0).val() + '@' + input.closest(".well").find("input[type='text']").eq(1).val() });
            }
        }
    });
    $("#Logging").val(JSON.stringify(arr));
}

function setFieldsNew(fields) {
    var arr = [];
    $.each(fields, function (i, val) {
        var input = $(val);
        if (input.attr("id") != undefined) {
            if (input.is("input") || input.is("textarea")) {
                if (input.attr("type") === "checkbox") {
                    arr.push({ "field": $("label[for='" + input.attr("id") + "']").html(), "old_val": "", "new_val": input.val(), "type": input.attr("id").indexOf("Customer") == 0 ? "customer" : input.closest(".well").find("input[type='text']").eq(0).val() });
                } else {
                    if (input.attr("type") !== "hidden")
                        arr.push({ "field": $("label[for='" + input.attr("id") + "']").html(), "old_val": "", "new_val": input.val(), "type": input.attr("id").indexOf("Customer") == 0 ? "customer" : input.closest(".well").find("input[type='text']").eq(0).val() });
                }
            } else {
                arr.push({ "field": $("label[for='" + input.attr("id") + "']").html(), "old_val": "", "new_val": input.find("option:selected").text(), "type": input.attr("id").indexOf("Customer") == 0 ? "customer" : input.closest(".well").find("input[type='text']").eq(0).val() });
            }
        }
    });
    $("#Logging").val(JSON.stringify(arr));
}

function parseJsonDate(jsonDate) {
    var offset = new Date().getTimezoneOffset() * 60000;
    var parts = /\/Date\((-?\d+)([+-]\d{2})?(\d{2})?.*/.exec(jsonDate);

    if (parts[2] == undefined)
        parts[2] = 0;

    if (parts[3] == undefined)
        parts[3] = 0;

    return new Date(+parts[1] + offset + parts[2] * 3600000 + parts[3] * 60000);
}

Date.prototype.toDateString = function() {
    return (this.getDate().toString().length === 1 ? "0" + this.getDate() : this.getDate()) + "/" + ((this.getMonth() + 1).toString().length === 1 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1)) + "/" + this.getFullYear() + " " + (this.getHours().toString().length === 1 ? "0" + this.getHours() : this.getHours()) + ":" + (this.getMinutes().toString().length === 1 ? "0" + this.getMinutes() : this.getMinutes()) + ":" + (this.getSeconds().toString().length === 1 ? "0" + this.getSeconds() : this.getSeconds());
}

Date.prototype.toDateStringJuridical = function () {
    return (this.getDate().toString().length === 1 ? "0" + this.getDate() : this.getDate()) + "/" + ((this.getMonth() + 1).toString().length === 1 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1)) + "/" + this.getFullYear();
}

function showModal(data) {
    var modalInstance = $(data);
    modalInstance.modal('show');
    modalInstance.on('hidden.bs.modal', function () {
        modalInstance.remove();
    });
    return modalInstance;
}

function wopen(data, w, h) {
    w += 32;
    h += 96;
    wleft = (screen.width - w) / 2;
    wtop = (screen.height - h) / 2;
    if (wleft < 0) {
        w = screen.width;
        wleft = 0;
    }
    if (wtop < 0) {
        h = screen.height;
        wtop = 0;
    }
    var win = window.open("",
    "Print",
    'width=' + w + ', height=' + h + ', ' +
    'left=' + wleft + ', top=' + wtop + ', ' +
    'location=no, menubar=no, ' +
    'status=no, toolbar=no, scrollbars=yes, resizable=no');
    win.document.writeln(data);
    win.resizeTo(w, h);
    win.moveTo(wleft, wtop);
    win.focus();
    //win.print();
}
