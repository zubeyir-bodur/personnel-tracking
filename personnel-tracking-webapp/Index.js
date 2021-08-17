
$(() => {
    $(document).ready(() => {
        $.get('Area/Index.html', (e) => {
            $('#area-table-container').append(e);
        });
    });
});

function showTrackingContent() {

    $('#area-table-container').hide();

    $(document).ready(() => {
        $.get('Tracking/Index.html', (e) => {
            $('#tracking-table-container').append(e);
        });
    });

}
