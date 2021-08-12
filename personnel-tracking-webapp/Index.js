$(() => {
    $(document).ready(() => {
        $.get('Area/Index.html', (e) => {
            $('#container-main').append(e);
        });
    });
});
$(function () {
    $("#CompanyDiv").load("Company/Index.html");
});