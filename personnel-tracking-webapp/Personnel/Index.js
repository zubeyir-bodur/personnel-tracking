createTable();
function createTable() {
    alert("here");
    $.ajax({
        url: "http://localhost:5000/api/personnel",
        headers: {
            "Authorization": "Bearer token"
        }
    }).done((response) => {
        $('#table-personnel').DataTable({
            language: {
                url: 'https://cdn.datatables.net/plug-ins/1.10.25/i18n/Turkish.json'
            },
            data: dataset,
            columns: [
                { title: "#" },
                { title: "Personnel id" },
                { title: "Company id" },
                { title: "Personel type" },
                { title: "identity" },
                { title: "Name" },
                { title: "Surname"}
            ],
            "columnDefs": [{
                "targets": -1,
                "data": null,
                "defaultContent": "<button id='edit-button'>Edit</button> <button id='delete-button'>Delete</button>"
            }]
        });
        //console.log(dataset);
        return dataset;
    });
}