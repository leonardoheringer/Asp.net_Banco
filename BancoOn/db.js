const mysql = require('mysql2/promise');

const pool = mysql.createPool({
    host: 'localhost',
    user: 'root',
    password: 'informaticasenai',
    database: 'DigiBank'
});

module.exports = pool;
