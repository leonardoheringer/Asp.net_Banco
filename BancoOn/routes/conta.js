const express = require('express');
const router = express.Router();
const db = require('../db');

// Criar conta
router.post('/criar', async (req, res) => {
    const { nome, cpf, senha } = req.body;
    const numeroConta = Math.floor(Math.random() * 9000000000 + 1000000000).toString();
    const agencia = Math.floor(Math.random() * 9000 + 1000).toString();

    const conn = await db.getConnection();
    await conn.beginTransaction();

    try {
        const [pessoaResult] = await conn.query("INSERT INTO Pessoas (Nome, CPF, Senha) VALUES (?, ?, ?)", [nome, cpf, senha]);
        const pessoaId = pessoaResult.insertId;

        await conn.query("INSERT INTO Conta (NumeroConta, Agencia, Saldo, PessoaID) VALUES (?, ?, 0, ?)", [numeroConta, agencia, pessoaId]);

        await conn.commit();
        res.json({ mensagem: "Conta criada com sucesso", numeroConta, agencia });
    } catch (err) {
        await conn.rollback();
        res.status(500).json({ erro: "Erro ao criar conta", detalhe: err.message });
    } finally {
        conn.release();
    }
});

// Login
router.post('/login', async (req, res) => {
    const { cpf, senha } = req.body;
    try {
        const [rows] = await db.query(`
            SELECT p.PessoaID, p.Nome, c.Saldo, c.NumeroConta, c.Agencia
            FROM Pessoas p
            JOIN Conta c ON p.PessoaID = c.PessoaID
            WHERE p.CPF = ? AND p.Senha = ?`, [cpf, senha]);

        if (rows.length > 0) {
            res.json(rows[0]);
        } else {
            res.status(401).json({ erro: "CPF ou senha inválidos" });
        }
    } catch (err) {
        res.status(500).json({ erro: err.message });
    }
});

// Depósito
router.post('/deposito', async (req, res) => {
    const { numeroConta, valor } = req.body;
    if (valor <= 0) return res.status(400).json({ erro: "Valor deve ser positivo" });

    const conn = await db.getConnection();
    await conn.beginTransaction();

    try {
        const [contas] = await conn.query("SELECT Saldo FROM Conta WHERE NumeroConta = ?", [numeroConta]);
        if (contas.length === 0) throw new Error("Conta não encontrada");

        const novoSaldo = parseFloat(contas[0].Saldo) + parseFloat(valor);
        await conn.query("UPDATE Conta SET Saldo = ? WHERE NumeroConta = ?", [novoSaldo, numeroConta]);

        await conn.query("INSERT INTO Extrato (ContaID, TipoTransacao, Valor) VALUES ((SELECT ContaID FROM Conta WHERE NumeroConta = ?), 'Deposito', ?)", [numeroConta, valor]);

        await conn.commit();
        res.json({ mensagem: "Depósito realizado com sucesso", novoSaldo });
    } catch (err) {
        await conn.rollback();
        res.status(500).json({ erro: err.message });
    } finally {
        conn.release();
    }
});

// Saque
router.post('/saque', async (req, res) => {
    const { numeroConta, valor } = req.body;
    if (valor <= 0) return res.status(400).json({ erro: "Valor deve ser positivo" });

    const conn = await db.getConnection();
    await conn.beginTransaction();

    try {
        const [contas] = await conn.query("SELECT Saldo FROM Conta WHERE NumeroConta = ?", [numeroConta]);
        if (contas.length === 0) throw new Error("Conta não encontrada");
        if (contas[0].Saldo < valor) throw new Error("Saldo insuficiente");

        const novoSaldo = parseFloat(contas[0].Saldo) - parseFloat(valor);
        await conn.query("UPDATE Conta SET Saldo = ? WHERE NumeroConta = ?", [novoSaldo, numeroConta]);

        await conn.query("INSERT INTO Extrato (ContaID, TipoTransacao, Valor) VALUES ((SELECT ContaID FROM Conta WHERE NumeroConta = ?), 'Saque', ?)", [numeroConta, valor]);

        await conn.commit();
        res.json({ mensagem: "Saque realizado com sucesso", novoSaldo });
    } catch (err) {
        await conn.rollback();
        res.status(500).json({ erro: err.message });
    } finally {
        conn.release();
    }
});

// Transferência
router.post('/transferencia', async (req, res) => {
    const { numeroContaOrigem, numeroContaDestino, valor } = req.body;
    if (valor <= 0) return res.status(400).json({ erro: "Valor deve ser positivo" });
    if (numeroContaOrigem === numeroContaDestino) return res.status(400).json({ erro: "Contas devem ser diferentes" });

    const conn = await db.getConnection();
    await conn.beginTransaction();

    try {
        const [contasOrigem] = await conn.query("SELECT ContaID, Saldo FROM Conta WHERE NumeroConta = ?", [numeroContaOrigem]);
        const [contasDestino] = await conn.query("SELECT ContaID, Saldo FROM Conta WHERE NumeroConta = ?", [numeroContaDestino]);

        if (contasOrigem.length === 0) throw new Error("Conta origem não encontrada");
        if (contasDestino.length === 0) throw new Error("Conta destino não encontrada");
        if (contasOrigem[0].Saldo < valor) throw new Error("Saldo insuficiente na conta origem");

        const novoSaldoOrigem = parseFloat(contasOrigem[0].Saldo) - parseFloat(valor);
        const novoSaldoDestino = parseFloat(contasDestino[0].Saldo) + parseFloat(valor);

        await conn.query("UPDATE Conta SET Saldo = ? WHERE ContaID = ?", [novoSaldoOrigem, contasOrigem[0].ContaID]);
        await conn.query("UPDATE Conta SET Saldo = ? WHERE ContaID = ?", [novoSaldoDestino, contasDestino[0].ContaID]);

        await conn.query("INSERT INTO Extrato (ContaID, TipoTransacao, Valor) VALUES (?, 'Transferência Saída', ?)", [contasOrigem[0].ContaID, valor]);
        await conn.query("INSERT INTO Extrato (ContaID, TipoTransacao, Valor) VALUES (?, 'Transferência Entrada', ?)", [contasDestino[0].ContaID, valor]);

        await conn.commit();
        res.json({ mensagem: "Transferência realizada com sucesso", novoSaldoOrigem, novoSaldoDestino });
    } catch (err) {
        await conn.rollback();
        res.status(500).json({ erro: err.message });
    } finally {
        conn.release();
    }
});

// Extrato
router.get('/extrato/:numeroConta', async (req, res) => {
    const { numeroConta } = req.params;
    try {
        const [contas] = await db.query("SELECT ContaID FROM Conta WHERE NumeroConta = ?", [numeroConta]);
        if (contas.length === 0) {
            return res.status(404).json({ erro: "Conta não encontrada" });
        }
        const contaId = contas[0].ContaID;
        const [extrato] = await db.query("SELECT DataTransacao, TipoTransacao, Valor FROM Extrato WHERE ContaID = ? ORDER BY DataTransacao DESC", [contaId]);

        res.json(extrato);
    } catch (err) {
        res.status(500).json({ erro: err.message });
    }
});

module.exports = router;
