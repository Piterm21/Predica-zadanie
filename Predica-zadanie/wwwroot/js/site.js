// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//[
//    { "LocationId": 1, "Description": "clear sky", "TempCurrent": "12.38", "TempMax": "12.78", "TempMin": "12" },
//    { "LocationId": 2, "Description": "clear sky", "TempCurrent": "13.48", "TempMax": "16.67", "TempMin": "10.56" },
//    { "LocationId": 3, "Description": "fog", "TempCurrent": "9.1", "TempMax": "11.11", "TempMin": "7.78" }
//]

function getFromTargetUrl(caller, successCallback) {
    let url = $(caller).data("target_url");

    $.get({
        url: url,
        success: function (response) {
            successCallback(response);
        }
    });
}

jQuery(document).ready(function () {
    $(document).on("click", ".weatherRefresh", function () {
        getFromTargetUrl($(this), function (response) {
            response = JSON.parse(response);

            response.forEach(function (element) {
                if ($("#WeatherTable_" + element["LocationId"]).data("page_number") == 1) {
                    let container = $("#" + element["LocationId"]);
                    let newRow = container.children(":first").clone();

                    $(newRow).find(".CreateDateTime").text(element["CreateDateTime"]);
                    $(newRow).find(".Description").text(element["Description"]);
                    $(newRow).find(".TempCurrent").text(element["TempCurrent"]);
                    $(newRow).find(".TempMax").text(element["TempMax"]);
                    $(newRow).find(".TempMin").text(element["TempMin"]);

                    container.prepend(newRow);
                    container.children(":last").remove();
                }
            });
        });
    });

    $(document).on("click", ".changePageAjax", function () {
        getFromTargetUrl($(this), function (response) {
            response = JSON.parse(response);

            $("#WeatherTable_" + response["LocationId"]).replaceWith(response["Html"]);
        });
    });
});