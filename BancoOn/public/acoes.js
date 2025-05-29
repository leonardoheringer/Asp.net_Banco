async function depositar() {
  const valor = parseFloat(document.getElementById('deposito-valor').value);
  if (!valor || valor <= 0) {
    showMessage('msg-deposito', 'Valor inválido');
    return;
  }
  try {
    const res = await fetch('/api/conta/deposito', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ numeroConta: usuario.NumeroConta, valor })
    });
    if (!res.ok) throw await res.json();
    const data = await res.json();
    usuario.Saldo = data.novoSaldo;
    atualizarSaldo(usuario.Saldo);
    showMessage('msg-deposito', data.mensagem, 'success');
    document.getElementById('deposito-valor').value = '';
  } catch (err) {
    showMessage('msg-deposito', err.erro || 'Erro no depósito');
  }
}

async function sacar() {
  const valor = parseFloat(document.getElementById('saque-valor').value);
  if (!valor || valor <= 0) {
    showMessage('msg-saque', 'Valor inválido');
    return;
  }
  try {
    const res = await fetch('/api/conta/saque', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ numeroConta: usuario.NumeroConta, valor })
    });
    if (!res.ok) throw await res.json();
    const data = await res.json();
    usuario.Saldo = data.novoSaldo;
    atualizarSaldo(usuario.Saldo);
    showMessage('msg-saque', data.mensagem, 'success');
    document.getElementById('saque-valor').value = '';
  } catch (err) {
    showMessage('msg-saque', err.erro || 'Erro no saque');
  }
}

async function transferir() {
  const numeroContaDestino = document.getElementById('transferencia-conta').value.trim();
  const valor = parseFloat(document.getElementById('transferencia-valor').value);
  if (!numeroContaDestino) {
    showMessage('msg-transferencia', 'Informe a conta destino');
    return;
  }
  if (!valor || valor <= 0) {
    showMessage('msg-transferencia', 'Valor inválido');
    return;
  }
  try {
    const res = await fetch('/api/conta/transferencia', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ numeroContaOrigem: usuario.NumeroConta, numeroContaDestino, valor })
    });
    if (!res.ok) throw await res.json();
    const data = await res.json();
    usuario.Saldo = data.novoSaldoOrigem;
    atualizarSaldo(usuario.Saldo);
    showMessage('msg-transferencia', data.mensagem, 'success');
    document.getElementById('transferencia-conta').value = '';
    document.getElementById('transferencia-valor').value = '';
  } catch (err) {
    showMessage('msg-transferencia', err.erro || 'Erro na transferência');
  }
}

async function carregarExtrato() {
  try {
    const res = await fetch(`/api/conta/extrato/${usuario.NumeroConta}`);
    if (!res.ok) throw await res.json();
    const extrato = await res.json();

    const tbody = document.querySelector('#extrato-table tbody');
    tbody.innerHTML = ''; // limpa extrato anterior

    extrato.forEach(tx => {
      const tr = document.createElement('tr');
      const dataHora = new Date(tx.DataTransacao).toLocaleString('pt-BR');
      tr.innerHTML = `
        <td>${dataHora}</td>
        <td>${tx.TipoTransacao}</td>
        <td class="${tx.TipoTransacao.toLowerCase().replace(/\s/g, '')}">R$ ${parseFloat(tx.Valor).toFixed(2)}</td>
      `;
      tbody.appendChild(tr);
    });
  } catch (err) {
    console.error('Erro ao carregar extrato:', err.erro || err);
  }
}
