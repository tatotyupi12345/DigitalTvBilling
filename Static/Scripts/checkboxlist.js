$(function () {
    $('.list-group.checked-list-box .list-group-item').each(function () {
        var $widget = $(this),
            $checkbox = $('<input type="checkbox" class="hidden" name="' + $widget.data("name") + '" />'),
            color = ($widget.data('color') ? $widget.data('color') : "primary"),
            style = ($widget.data('style') == "button" ? "btn-" : "list-group-item-"),
            settings = {
                on: {
                    icon: 'glyphicon glyphicon-check'
                },
                off: {
                    icon: 'glyphicon glyphicon-unchecked'
                }
            };
        $widget.css('cursor', 'pointer')
        $widget.append($checkbox);
        $widget.on('click', function () {
            $checkbox.prop('checked', !$checkbox.is(':checked'));
            $checkbox.triggerHandler('change');

            if ($checkbox.is(':checked')) {
                $widget.after($('<input id="id_' + $widget.data("id") + '" type="hidden" value="' + $widget.data("id") + '" data-name="' + $widget.find("span").eq(1).html() + '" />'));
            } else {
                $widget.next().remove();
            }
            updatePrice();
            updateDisplay();
        });
        $checkbox.on('change', function () {
            updateDisplay();
        });
        function updateDisplay() {
            var isChecked = $checkbox.is(':checked');
            $widget.data('state', (isChecked) ? "on" : "off");
            $widget.find('.state-icon')
                .removeClass()
                .addClass('state-icon ' + settings[$widget.data('state')].icon);
            if (isChecked) {
                $widget.addClass(style + color + ' active');
            } else {
                $widget.removeClass(style + color + ' active');
            }
        }
        function init() {
            if ($widget.data('checked') == true) {
                $checkbox.prop('checked', !$checkbox.is(':checked'));
            }
            updateDisplay();
            if ($widget.find('.state-icon').length == 0) {
                $widget.prepend('<span class="state-icon ' + settings[$widget.data('state')].icon + '"></span>');
            }
        }
        init();
    });

    updatePrice();
    function updatePrice() {
        var summ = 0;
        $(".checked-list-box li.active").each(function (idx, li) {
            summ += parseFloat($(li).data("price"));
        });
        $("#packages_summ").html(summ);
    }
});

