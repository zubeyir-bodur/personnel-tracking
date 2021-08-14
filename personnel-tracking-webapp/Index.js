$(() => {
    $(document).ready(() => {
        $("#CompanyPage").click(() => {
            var company = $.get('Company/Index.html');
            $('#container-main').empty().append(company);
        });
    });
});