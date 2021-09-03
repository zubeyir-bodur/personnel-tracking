$(document).ready(() => {
    var company = {
        companyId: -1,
        companyName : ""
    }
    const chosen = {
        PERSONNEL: "personnel",
        AREA: "area",
        NULL: "null",
    }
    var choice = chosen.NULL;
    $("#report-personnel-btn").click((e) => {
        $("#downloadMessage").empty();
        $("#personnel-input").val("");
        $("#company-input").val("");
        e.preventDefault();
        choice = chosen.PERSONNEL;
        configurateCompanies();
        $("#list-areas").css("display", "none");
        $("#calendar").css("display", "none");
        $("#calendar").val("");
        $("#list-personnel").css("display", "block");
        $("#calendar").css("display", "block");
    });

    $("#report-area-btn").click((e) => {
        $("#downloadMessage").empty();
        $("#area-input").val("");
        $("#company-input").val("");
        e.preventDefault();
        choice = chosen.AREA;
        configurateCompanies();
        $("#list-personnel").css("display", "none");
        $("#calendar").css("display", "none");
        $("#calendar").val("");
        $("#list-areas").css("display", "block");
        $("#calendar").css("display", "block");
    });

    $("#company-input").change((e) => {
        company.companyId = Number($("#company-input").val());
        switch (choice) {
            case chosen.AREA:
                configurateAreas(company);
                break;
            case chosen.PERSONNEL:
                configuratePersonnels(company);
                break;
            default:
                break;
        }
        $("#download").css("display", "block");
    });

    $("#download").click( (e) => {
        switch (choice) {
            case chosen.AREA:
                var areaParams = {
                    areaId : Number($("#area-input").val()),
                    companyId : Number($("#company-input").val()),
                    Start : $("#start-date").val(),
                    End : $("#end-date").val()
                };
                downloadArea(areaParams);
                break;
            case chosen.PERSONNEL:
                var personnelParams = {
                    personnelId : Number($("#personnel-input").val()),
                    Start : $("#start-date").val(),
                    End : $("#end-date").val()
                };
                downloadPersonnel(personnelParams);
                break;
            default:
                break;
        }
    });
});

function configurateCompanies() {
    $.ajax({
        url: "http://localhost:5000/api/company",
        headers: {
            "Authorization": "Bearer" + localStorage.getItem("token")
        },
        dataType: "json",
        success: (response) => {
            if (!response.hasError) {
                $("#company-input").empty();
                for (var i = 0; i < response.data.length; i++) {
                    $("#company-input").append($('<option>')
                        .val(response.data[i].companyId)
                        .text(response.data[i].companyName));
                }
                $("#company-input").append($('<option value="" selected disabled hidden>')
                    .text("Choose a company"));
                $("#list-companies").css("display", "block");
            }
            else {
                $('#error-message').text(response.errorMessage);
                $('#error-modal').modal('show');
            }
        }
    });
}

function configuratePersonnels(company) {
    $.ajax({
        url: "http://localhost:5000/api/report/personnelsOfCompany",
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer" + localStorage.getItem("token")
        },
        data: JSON.stringify(company),
        dataType: "json",
        success: (response) => {
            if (!response.hasError) {
                $("#personnel-input").empty();
                for (var i = 0; i < response.data.length; i++) {
                    $("#personnel-input").append($('<option>')
                        .val(response.data[i].personnelId)
                        .text(response.data[i].personnelName + " " + response.data[i].personnelSurname));
                        
                }
                $("#personnel-input").append($('<option value="" selected disabled hidden>')
                    .text("Choose a personnel"));
                $("#personnel-input").css("display", "block");
            }
            else {
                $('#error-message').text(response.errorMessage);
                $('#error-modal').modal('show');
            }
        }
    });
}

function configurateAreas(company) {
    $.ajax({
        url: "http://localhost:5000/api/area",
        headers: {
            "Authorization": "Bearer" + localStorage.getItem("token")
        },
        dataType: "json",
        success: (response) => {
            if (!response.hasError) {
                $("#area-input").empty();
                for (var i = 0; i < response.data.length; i++) {
                    if (response.data[i].companyId == company.companyId)
                        $("#area-input").append($('<option>')
                            .val(response.data[i].areaId)
                            .text(response.data[i].areaName));
                }
                $("#area-input").append($('<option value="" selected disabled hidden>')
                    .text("Choose an area"));
                $("#area-input").css("display", "block");
            }
            else {
                $('#error-message').text(response.errorMessage);
                $('#error-modal').modal('show');
            }
        }
    });
}

async function downloadArea(areaParams) {
    await $.ajax({
        url: "http://localhost:5000/api/report/area",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer" + localStorage.getItem("token")
        },
        method: "PUT",
        data: JSON.stringify(areaParams),
        dataType: "json",
        success: (response) => {
            if (response.hasError) {
                console.log(response.errorMessage);
            }
            else {
                console.log(response);
                download(response.data[0]);
                download(response.data[1]);
            }
        }
    });
}

async function downloadPersonnel(personnelParams) {
    await $.ajax({
        url: "http://localhost:5000/api/report/personnel",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer" + localStorage.getItem("token")
        },
        method: "PUT",
        data: JSON.stringify(personnelParams),
        dataType: "json",
        success: (response) => {
            if (response.hasError) {
                console.log(response.errorMessage);
            }
            else {
                console.log(response);
                download(response.data[0]);
                download(response.data[1]);
            }
        }
    });
}

function download(path) {
    var extension = path.split(".").pop();
    var contentType = "";
    var message = "";
    console.log(extension);
    if (extension === "xlsx") {
        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        message = "Your Excel file is generated in " + path + "<br>";
    }
    else if (extension === "json") {
        contentType = "application/json";
        message = "Your JSON file is generated in " + path + "<br>";
    }
    console.log(message);
    $("#downloadMessage").append(message);
    console.log($("#downloadMessage"));
    /*
    await $.ajax({
        url: "http://localhost:5000/api/report/download?path=" + path,
        headers: {
            "Content-Type": contentType,
            "Authorization": "Bearer" + localStorage.getItem("token")
        },
        method: "GET",
        success: (response) => {
            if (response.hasError) {
                console.log(response.errorMessage);
            }
            else {
                console.log(response);
            }
        }
    });
    $.ajax({
        url: path,
        cache: false,
        xhr: function () {
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 2) {
                    if (xhr.status == 200) {
                        xhr.responseType = "blob";
                    } else {
                        xhr.responseType = "text";
                    }
                }
            };
            return xhr;
        },
        success: function (data) {
            //Convert the Byte Data to BLOB object.
            var blob = new Blob([data], { type: "application/octetstream" });

            //Check the Browser type and download the File.
            var isIE = false || !!document.documentMode;
            if (isIE) {
                window.navigator.msSaveBlob(blob, fileName);
            } else {
                var url = window.URL || window.webkitURL;
                link = url.createObjectURL(blob);
                var a = $("<a />");
                a.attr("download", fileName);
                a.attr("href", link);
                $("body").append(a);
                a[0].click();
                $("body").remove(a);
            }
        }
    });*/
}