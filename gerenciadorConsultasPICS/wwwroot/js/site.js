
function voltarTela() {
    history.go(-1);
    return false;
}
function exibirAlerta(mensagem) {
    Swal.fire({
        heightAuto: false,
        title: 'Atenção!',
        text: mensagem,
        icon: 'warning',
        confirmButtonColor: 'LightSeaGreen',
        confirmButtonText: 'OK'
    });
}

$(document).ready(function () {
    $(document).ajaxStart(function () {
        $('#overlay-loading').fadeIn();
    });

    $(document).ajaxStop(function () {
        $('#overlay-loading').fadeOut();
    });
});