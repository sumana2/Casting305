var searchOptions = {};
var colors = ['default', 'primary', 'success', 'info', 'warning', 'danger'];

$(document).ready(function () {
    var json = $('#search').val()
    if (json != '') {
        searchOptions = JSON.parse(json);
    }

    var i = 0;
    for (var prop in searchOptions) {
        $('#searchList').append('<span style="margin-right:5px;white-space: inherit;" class="label label-' + colors[i] + '">' + prop + ':' + searchOptions[prop] + '<span id="remove' + prop + '" style="top: 3px;left: 5px;" class="removeSearch glyphicon glyphicon-remove" aria-hidden="true"></span></span>');
        i++;
        if (i > colors.length-1) {
            i = 0;
        }
    }

    $("#form").submit(function (event) {
        var type = $("#searchType").text();
        var value = $("#Value").val();

        if (value) {
            searchOptions[type] = value;
        }
            
        $('#search').val(JSON.stringify(searchOptions));
    });

    $(".dropdown-menu li a").click(function () {
        $("#searchType").text($(this).text());
    });

    $(".removeSearch").click(function () {
        var prop = this.id.replace('remove', '');
        delete searchOptions[prop];

        $('#search').val(JSON.stringify(searchOptions));

        $("#form").submit();
    });
});