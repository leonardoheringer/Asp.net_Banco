const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const contaRoutes = require('./routes/conta');

const app = express();
app.use(cors());
app.use(bodyParser.json());
app.use(express.static('public'));

app.use('/api/conta', contaRoutes);

const PORT = 3000;
app.listen(PORT, () => {
    console.log(`Servidor rodando em http://localhost:${PORT}`);
});
