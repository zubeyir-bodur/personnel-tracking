$(() => {
    $(document).ready(() => {
        $.get('Area/Index.html', (e) => {
            $('#area-table-container').append(e);
        });
        $.get('Personnel/Index.html', (e) => {
            $('#personnel-table-container').append(e);
        });
        $('#personnel-table-container').hide();


    });


    $("#PersonnelPage").click(() => {
   
        $('#area-table-container').hide();
        $('#company-table-container').hide();
        $('#leave-table-container').hide();
        $('#personnel-table-container').show();
        $('#personnel-type-table-container').hide();
        $('#tracking-table-container').hide();
    });
});