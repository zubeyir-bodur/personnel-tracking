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
        $("#AreaPage").click(() => {
            $.get('Area/Index.html', (obj) => {
                $('#container-main').empty().append(obj);
            });
        });
        $("#PersonnelPage").click(() => {
            $.get('Personnel/Index.html', (obj) => {
                $('#container-main').empty().append(obj);
            });
        });
        $('#Leave').click(function (e) {
            e.preventDefault();
            $.get('Leave/Index.html', (e) => {
                $('#container-main').html(e);
            }); return false;
        });
        $("#TrackingPage").click(() => {
            $.get('Tracking/Index.html', (obj) => {
                $('#container-main').empty().append(obj);
            });
        });
    });
});

/*function showTrackingContent() {

    $('#area-table-container').hide();

    $(document).ready(() => {
        $.get('Tracking/Index.html', (e) => {
            $('#tracking-table-container').append(e);
        });
    });

}*/
