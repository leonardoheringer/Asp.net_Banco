$(document).ready(function () {
    // Máscara para CPF
    $('.cpf-mask').mask('000.000.000-00', { reverse: true });

    // Máscara para telefone
    $('.phone-mask').mask('(00) 00000-0000');

    // Máscara para valores monetários
    $('.money-mask').mask('#.##0,00', { reverse: true });
});