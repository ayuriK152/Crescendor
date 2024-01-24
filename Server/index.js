const express = require('express')
const mysql = require('mysql')
const db = require('./config/database.js')
const connection = mysql.createConnection(db)

const app = express()

// configuration =========================
app.set('port', process.env.PORT || 3000)

app.get('/', (req, res) => {
  res.send('Root')
})

app.get('/users', (req, res) => {
  connection.query('SELECT * from users', (error, rows) => {
    if (error) throw error
    console.log('User info is: ', rows)
    res.send(rows)
  })
})

app.listen(app.get('port'), () => {
  console.log('Express server listening on port ' + app.get('port'))
})
