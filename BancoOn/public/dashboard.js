let usuario = null;

function login() {
  const cpf = document.getElementById('cpf').value.trim();
  const senha = document.getElementById('senha').value.trim();
  if (!cpf || !senha) {
    showMessage('login-msg', 'Informe CPF e senha');
    return;
  }

  fetch('/api/conta/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ cpf, senha })
  })
  .then(res => {
    if (!res.ok) throw res;
    return res.json();
  })
  .then(data => {
    usuario = data;
    document.getElementById('login-section').style.display = 'none';
    document.getElementById('dashboard').style.display = 'block';
    atualizarDashboard();
    limparMensagens();
  })
  .catch(async err => {
    const msg = await err.json().then(e => e.erro).catch(() => 'Erro no login');
    showMessage('login-msg', msg);
  });
}

function atualizarDashboard() {
  if (!usuario) return;
  document.getElementById('nome').textContent = usuario.Nome;
  document.getElementById('conta').textContent = usuario.NumeroConta;
  document.getElementById('agencia').textContent = usuario.Agencia;
  atualizarSaldo(usuario.Saldo);
  carregarExtrato(); // atualizar extrato sempre que atualizar dashboard
}

function limparMensagens() {
  ['login-msg', 'msg-deposito', 'msg-saque', 'msg-transferencia'].forEach(id => {
    document.getElementById(id).textContent = '';
  });
}

function logout() {
  usuario = null;
  document.getElementById('dashboard').style.display = 'none';
  document.getElementById('login-section').style.display = 'block';
  limparMensagens();
}

function showMessage(id, msg, tipo='error') {
  const el = document.getElementById(id);
  el.textContent = msg;
  el.className = tipo === 'error' ? 'msg error' : 'msg success';
}
