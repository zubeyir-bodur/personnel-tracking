$(document).ready(function () {

    $.ajax({
        "url": " http://localhost:5000/api/leave",
        "headers": {
            "Authorization": "Bearer Bearer"
        },
        "async": true,
        "type": "GET",
        "datatype": 'json',
        "success": function (response) {
            $('#table-leave').DataTable({

                data: response.data,

                columns: [
                    { 'data': 'leaveId' },
                    { 'data': 'personnelId' },
                    { 'data': 'leaveStart' },
                    { 'data': 'leaveEnd' },
                    {
                        'data': 'personnel',
                        "render": function (leaveId, type, full, meta) {
                            return '<div><button type="button" class="btn btn-primary" >Edit</button> <button type="button" class="btn btn-danger" id="deleteLeave">Delete</button></div>'
                        }
                    },

                ]
            });

        }
    });

    alert("comeheree");
    $(".btn").click(function () {
        $("#myModal").modal('show');
    });


});