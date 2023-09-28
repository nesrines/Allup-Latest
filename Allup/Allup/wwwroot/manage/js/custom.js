$(document).ready(function () {
    let isMain = $('#IsMain').is(':checked');
    if (isMain) {
        $('#imageContainer').removeClass('d-none');
        $('#parentContainer').addClass('d-none');
    }
    else {
        $('#imageContainer').addClass('d-none');
        $('#parentContainer').removeClass('d-none');
    }

    $('#IsMain').click(function () {
        let isMain = $(this).is(':checked')
        if (isMain) {
            $('#imageContainer').removeClass('d-none');
            $('#parentContainer').addClass('d-none');
        }
        else {
            $('#imageContainer').addClass('d-none');
            $('#parentContainer').removeClass('d-none');
        }
    });

    $(document).on('click', 'delete-img-btn', function (e) {
        e.preventDefault();
        fetch($(this).attr('href'))
            .then(res => res.text())
            .then(data => $('#productImages').html(data));
    });
})