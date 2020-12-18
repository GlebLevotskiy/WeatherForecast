$(function () {
    $("#GetButton").click(function () {
        var url = '@Url.Action("GetCityTemperature", "Home")';
        var cityName = document.getElementById("cityName").value;
        $.ajax({
            url: url,
            type: "GET",
            data: "cityName=" + cityName,
        });
    });
});