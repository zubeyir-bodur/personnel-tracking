$(() => {
    $(document).ready(() => {
        $.get('Area/Index.html', (e) => {
            $('#container-main').append(e);
        });
    });
});