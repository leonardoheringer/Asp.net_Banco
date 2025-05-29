// saldo.js
function atualizarSaldo(valor) {
  const saldoEl = document.getElementById('saldo');
  saldoEl.textContent = parseFloat(valor).toFixed(2);
}
