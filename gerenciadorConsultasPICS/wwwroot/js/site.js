/**
 * AGENDA PICS - JavaScript Principal
 * Sistema de Agendamento de Práticas Integrativas
 */

// ==========================================================================
// VARIÁVEIS GLOBAIS
// ==========================================================================

var telaAnterior = null;

// ==========================================================================
// NAVEGAÇÃO
// ==========================================================================

/**
 * Volta para a tela anterior
 */
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

/**
 * Configura o destino do botão voltar
 * @param {string} pTelaAnterior - URL da tela anterior
 */
function configurarBotaoVoltar(pTelaAnterior) {
    telaAnterior = pTelaAnterior;
}

// ==========================================================================
// ALERTAS E TOASTS
// ==========================================================================

/**
 * Exibe um alerta usando SweetAlert2
 * @param {string} mensagem - Mensagem do alerta
 */
function exibirAlerta(mensagem) {
    Swal.fire({
        heightAuto: false,
        title: 'Atenção!',
        text: mensagem,
        icon: 'warning',
        confirmButtonColor: '#4CAF50',
        confirmButtonText: 'OK'
    });
}

/**
 * Exibe um alerta de sucesso
 * @param {string} mensagem - Mensagem de sucesso
 */
function exibirSucesso(mensagem) {
    Swal.fire({
        heightAuto: false,
        title: 'Sucesso!',
        text: mensagem,
        icon: 'success',
        confirmButtonColor: '#4CAF50',
        confirmButtonText: 'OK'
    });
}

/**
 * Exibe um alerta de erro
 * @param {string} mensagem - Mensagem de erro
 */
function exibirErro(mensagem) {
    Swal.fire({
        heightAuto: false,
        title: 'Erro!',
        text: mensagem,
        icon: 'error',
        confirmButtonColor: '#4CAF50',
        confirmButtonText: 'OK'
    });
}

/**
 * Exibe uma confirmação
 * @param {string} titulo - Título da confirmação
 * @param {string} mensagem - Mensagem da confirmação
 * @param {function} onConfirm - Callback quando confirmado
 */
function exibirConfirmacao(titulo, mensagem, onConfirm) {
    Swal.fire({
        heightAuto: false,
        title: titulo,
        text: mensagem,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#4CAF50',
        cancelButtonColor: '#ff6060',
        confirmButtonText: 'Sim',
        cancelButtonText: 'Não'
    }).then((result) => {
        if (result.isConfirmed && typeof onConfirm === 'function') {
            onConfirm();
        }
    });
}

/**
 * Exibe um toast (notificação pequena)
 * @param {string} mensagem - Mensagem do toast
 * @param {string} tipo - Tipo: 'success', 'error', 'warning', 'info'
 * @param {number} duracao - Duração em ms (padrão: 3000)
 */
function exibirToast(mensagem, tipo = 'info', duracao = 3000) {
    // Cria container se não existir
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    // Cria o toast
    const toast = document.createElement('div');
    toast.className = `toast toast--${tipo}`;

    // Ícone baseado no tipo
    const icones = {
        success: '✓',
        error: '✕',
        warning: '⚠',
        info: 'ℹ'
    };

    toast.innerHTML = `
        <span class="toast__icon">${icones[tipo] || icones.info}</span>
        <span class="toast__message">${mensagem}</span>
    `;

    container.appendChild(toast);

    // Remove após duração
    setTimeout(() => {
        toast.style.animation = 'toast-out 0.3s ease forwards';
        setTimeout(() => toast.remove(), 300);
    }, duracao);
}

// ==========================================================================
// LOADING
// ==========================================================================

/**
 * Exibe o overlay de loading
 */
function exibirLoading() {
    $('#overlay-loading').fadeIn(200);
}

/**
 * Oculta o overlay de loading
 */
function ocultarLoading() {
    $('#overlay-loading').fadeOut(200);
}

// ==========================================================================
// SIDEBAR / MENU MOBILE
// ==========================================================================

/**
 * Abre o menu sidebar
 */
function abrirSidebar() {
    const sidebar = document.querySelector('.sidebar');
    const overlay = document.querySelector('.sidebar-overlay');
    const hamburger = document.querySelector('.hamburger-menu');

    if (sidebar) sidebar.classList.add('active');
    if (overlay) overlay.classList.add('active');
    if (hamburger) hamburger.classList.add('active');

    document.body.style.overflow = 'hidden';
}

/**
 * Fecha o menu sidebar
 */
function fecharSidebar() {
    const sidebar = document.querySelector('.sidebar');
    const overlay = document.querySelector('.sidebar-overlay');
    const hamburger = document.querySelector('.hamburger-menu');

    if (sidebar) sidebar.classList.remove('active');
    if (overlay) overlay.classList.remove('active');
    if (hamburger) hamburger.classList.remove('active');

    document.body.style.overflow = '';
}

/**
 * Alterna o menu sidebar
 */
function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    if (sidebar && sidebar.classList.contains('active')) {
        fecharSidebar();
    } else {
        abrirSidebar();
    }
}

// ==========================================================================
// VALIDAÇÃO DE FORMULÁRIOS
// ==========================================================================

/**
 * Valida CPF
 * @param {string} cpf - CPF a ser validado
 * @returns {boolean} - True se válido
 */
function validarCPF(cpf) {
    cpf = cpf.replace(/[^\d]/g, '');

    if (cpf.length !== 11) return false;
    if (/^(\d)\1{10}$/.test(cpf)) return false;

    let soma = 0;
    for (let i = 0; i < 9; i++) {
        soma += parseInt(cpf.charAt(i)) * (10 - i);
    }
    let resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(cpf.charAt(9))) return false;

    soma = 0;
    for (let i = 0; i < 10; i++) {
        soma += parseInt(cpf.charAt(i)) * (11 - i);
    }
    resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(cpf.charAt(10))) return false;

    return true;
}

/**
 * Valida CNPJ
 * @param {string} cnpj - CNPJ a ser validado
 * @returns {boolean} - True se válido
 */
function validarCNPJ(cnpj) {
    cnpj = cnpj.replace(/[^\d]/g, '');

    if (cnpj.length !== 14) return false;
    if (/^(\d)\1{13}$/.test(cnpj)) return false;

    let tamanho = cnpj.length - 2;
    let numeros = cnpj.substring(0, tamanho);
    let digitos = cnpj.substring(tamanho);
    let soma = 0;
    let pos = tamanho - 7;

    for (let i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2) pos = 9;
    }

    let resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado !== parseInt(digitos.charAt(0))) return false;

    tamanho = tamanho + 1;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;

    for (let i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2) pos = 9;
    }

    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado !== parseInt(digitos.charAt(1))) return false;

    return true;
}

/**
 * Aplica validação visual em campo
 * @param {HTMLElement} campo - Elemento do campo
 * @param {boolean} valido - Se está válido
 */
function aplicarValidacaoVisual(campo, valido) {
    campo.classList.remove('is-valid', 'is-invalid');
    campo.classList.add(valido ? 'is-valid' : 'is-invalid');
}

// ==========================================================================
// MÁSCARAS
// ==========================================================================

/**
 * Aplica máscara de CPF
 * @param {HTMLElement} input - Elemento input
 */
function mascaraCPF(input) {
    let valor = input.value.replace(/\D/g, '');
    valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
    valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
    valor = valor.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    input.value = valor;
}

/**
 * Aplica máscara de CNPJ
 * @param {HTMLElement} input - Elemento input
 */
function mascaraCNPJ(input) {
    let valor = input.value.replace(/\D/g, '');
    valor = valor.replace(/^(\d{2})(\d)/, '$1.$2');
    valor = valor.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3');
    valor = valor.replace(/\.(\d{3})(\d)/, '.$1/$2');
    valor = valor.replace(/(\d{4})(\d)/, '$1-$2');
    input.value = valor;
}

/**
 * Aplica máscara de telefone
 * @param {HTMLElement} input - Elemento input
 */
function mascaraTelefone(input) {
    let valor = input.value.replace(/\D/g, '');
    if (valor.length <= 10) {
        valor = valor.replace(/(\d{2})(\d)/, '($1) $2');
        valor = valor.replace(/(\d{4})(\d)/, '$1-$2');
    } else {
        valor = valor.replace(/(\d{2})(\d)/, '($1) $2');
        valor = valor.replace(/(\d{5})(\d)/, '$1-$2');
    }
    input.value = valor;
}

/**
 * Aplica máscara de CEP
 * @param {HTMLElement} input - Elemento input
 */
function mascaraCEP(input) {
    let valor = input.value.replace(/\D/g, '');
    valor = valor.replace(/(\d{5})(\d)/, '$1-$2');
    input.value = valor;
}

/**
 * Aplica máscara de data
 * @param {HTMLElement} input - Elemento input
 */
function mascaraData(input) {
    let valor = input.value.replace(/\D/g, '');
    valor = valor.replace(/(\d{2})(\d)/, '$1/$2');
    valor = valor.replace(/(\d{2})(\d)/, '$1/$2');
    input.value = valor;
}

// ==========================================================================
// UTILIDADES
// ==========================================================================

/**
 * Debounce para otimizar chamadas frequentes
 * @param {function} func - Função a ser executada
 * @param {number} wait - Tempo de espera em ms
 */
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

/**
 * Formata data para exibição (DD/MM/YYYY)
 * @param {string|Date} data - Data a ser formatada
 * @returns {string} - Data formatada
 */
function formatarData(data) {
    if (!data) return '';
    const d = new Date(data);
    const dia = String(d.getDate()).padStart(2, '0');
    const mes = String(d.getMonth() + 1).padStart(2, '0');
    const ano = d.getFullYear();
    return `${dia}/${mes}/${ano}`;
}

/**
 * Formata data e hora para exibição
 * @param {string|Date} data - Data a ser formatada
 * @returns {string} - Data e hora formatadas
 */
function formatarDataHora(data) {
    if (!data) return '';
    const d = new Date(data);
    const dia = String(d.getDate()).padStart(2, '0');
    const mes = String(d.getMonth() + 1).padStart(2, '0');
    const ano = d.getFullYear();
    const hora = String(d.getHours()).padStart(2, '0');
    const min = String(d.getMinutes()).padStart(2, '0');
    return `${dia}/${mes}/${ano} ${hora}:${min}`;
}

// ==========================================================================
// INICIALIZAÇÃO
// ==========================================================================

$(function () {
    // Loading automático em requisições AJAX
    $(document).ajaxStart(function () {
        exibirLoading();
    });

    $(document).ajaxStop(function () {
        ocultarLoading();
    });

    // Fecha sidebar ao clicar no overlay
    $(document).on('click', '.sidebar-overlay', function () {
        fecharSidebar();
    });

    // Fecha sidebar com tecla ESC
    $(document).on('keydown', function (e) {
        if (e.key === 'Escape') {
            fecharSidebar();
        }
    });

    // Adiciona classe de animação aos cards ao entrar na viewport
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-slide-up');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    // Observa cards para animação
    document.querySelectorAll('.card').forEach(card => {
        observer.observe(card);
    });

    // Configura inputmode para campos numéricos
    $('input[data-mask="cpf"], input[data-mask="cnpj"], input[data-mask="telefone"], input[data-mask="cep"]')
        .attr('inputmode', 'numeric');

    // Auto-aplica máscaras baseado em data-mask
    $('input[data-mask="cpf"]').on('input', function () { mascaraCPF(this); });
    $('input[data-mask="cnpj"]').on('input', function () { mascaraCNPJ(this); });
    $('input[data-mask="telefone"]').on('input', function () { mascaraTelefone(this); });
    $('input[data-mask="cep"]').on('input', function () { mascaraCEP(this); });
    $('input[data-mask="data"]').on('input', function () { mascaraData(this); });
});

