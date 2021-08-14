$(() => {
    $(document).ready(() => {
        $("#CompanyPage").click(() => {
            $.get('Company/Index.html', (obj) => {
                $('#container-main').empty().append(obj);
            });
        });
        $("#PersonnelTypePage").click(() => {
            $.get('PersonnelType/Index.html', (obj) => {
                $('#container-main').empty().append(obj);
            });
        });
    });
});