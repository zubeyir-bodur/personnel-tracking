$(() => {
    $(document).ready(() => {
        $.get('Area/Index.html', (e) => {
            $('#container-main').html(e);
        });
    });
    $('#Leave').click(function (e) {
        e.preventDefault();
        $.get('Leave/Index.html', (e) => {
            $('#container-main').html(e);
        }); return false;
    });

});



