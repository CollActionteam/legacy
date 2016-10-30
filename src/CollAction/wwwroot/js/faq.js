$(document).ready(function () {

    $('.collapse').on('show.bs.collapse', function () {
        $(this).parent().find(".fa-angle-down").removeClass("fa-angle-down").addClass("fa-angle-up");
    });
    $('.collapse').on('hide.bs.collapse', function () {
        $(this).parent().find(".fa-angle-up").removeClass("fa-angle-up").addClass("fa-angle-down");
    });

});