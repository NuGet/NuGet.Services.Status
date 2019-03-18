(function () {
    'use strict';

    $('time').each(function () {
        var el = $(this);
        var dt = moment(el.attr('datetime'));
        var format = el.data('format');
        if (!format) {
            format = 'LLL';
        }

        el.attr('title', dt.utc().format());
        el.text(dt.local().format(format));
    });
}());