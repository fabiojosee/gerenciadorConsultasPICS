
var telaAnterior = null;

function voltarTela() {
    if (telaAnterior == null) {
        history.go(-1);
    } else {
        let telaAnteriorAux = telaAnterior;
        telaAnterior = null;
        window.location.href = telaAnteriorAux;
    }
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

function configurarBotaoVoltar(pTelaAnterior) {
    telaAnterior = pTelaAnterior;
}

$(document).ready(function () {
    $(document).ajaxStart(function () {
        $('#overlay-loading').fadeIn();
    });

    $(document).ajaxStop(function () {
        $('#overlay-loading').fadeOut();
    });
});