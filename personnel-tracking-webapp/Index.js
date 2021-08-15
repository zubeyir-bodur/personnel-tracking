$(() => {
    $(document).ready(() => {
        $.get('Area/Index.html', (e) => {
            $('#area-table-container').append(e);
        });

        $("#AreaPage").click(() => {
            $('#area-table-container').show();
            $('#company-table-container').hide();
            $('#leave-table-container').hide();
            $('#personnel-table-container').hide();
            $('#personnel-type-table-container').hide();
            $('#tracking-table-container').hide();
        });
        // Same goes to others
    });
});