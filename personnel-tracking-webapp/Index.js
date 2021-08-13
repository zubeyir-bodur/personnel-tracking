$(() => {
    $(document).ready(() => {
        $("#AreaPage").click(() => {
            console.log(clicked);
            var area = $.get('Area/Index.html');
            $('#container-main').empty().append(area);
        });
    });
});