const express = require('express')
const mysql = require('mysql')
const db = require('./config/database.js')
const connection = mysql.createConnection(db)
const bcrypt = require('bcrypt');

const app = express()

app.set('port', process.env.PORT || 3000)

app.use(express.json())

// ===========================================    API DEFINITION    ===========================================
app.get('/', (req, res) => {
  res.send('Root')
})

// =====================================    Users     =====================================
app.get('/users', (req, res) => {
  connection.query('SELECT * FROM Crescendor.users;', (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('User info is: ', rows)
    res.send(rows)
  })

  connection.end();
})

app.post('/signin', async (req, res) => {
  const { id, password } = req.body;

  connection.query('SELECT count(*) from Crescendor.users where id = ?;', id, (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('sign count:', rows)
    if (rows > 0){
      res.status(400).send(`ERROR: exist id`)
      return
    }
  })

  const hashedPassword = await bcrypt.hash(password, 10);
  
  connection.query("INSERT INTO Crescendor.users SET id = ?, nickname = ?, password = ?;", [id, id, hashedPassword], (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('signin \n id: %s \n', id)
    res.status(200).send('SUCCESS')
  })

  connection.end();
})

app.post('/login', async (req, res) => {
  const { id, password } = req.body
  const user_password = null

  connection.query('SELECT password from Crescendor.users where id = ?;', id, (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    if (rows == null){
      res.status(400).send(`ERROR: id`)
      return
    }
    user_password = rows[0].password
  })

  const matchPassword = bcrypt.compare(password, user_password)

  if (!matchPassword) {
      res.status(400).send('ERROR: password')
      return
  }

  if (matchPassword){
    res.status(200).send('SUCCESS')
    return
  }

  connection.end();
})

// =====================================    Record    =====================================
app.get('/record', (req, res) => {
  connection.query('SELECT * from Crescendor.record;', (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('Record info is: ', rows)
    res.send(rows)
  })

  connection.end();
})

// getscore API
app.get('/record/getscore/:user_id/:music_id', (req, res) => {
  const user_id = req.params.user_id
  const music_id = Number(req.params.music_id)

  connection.query("SELECT score from Crescendor.record where (user_id = ? && music_id = ?);", [user_id, music_id], (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('getscore \n user: %s \n music: %d \n', user_id, music_id)
    console.log(rows)
    res.send(rows)
  })

  connection.end();
}) 

// addscore API
app.post('/record/addscore/:user_id/:music_id', (req, res) => {
  const user_id = req.params.user_id
  const music_id = Number(req.params.music_id)
  const { score, date, midi } = req.body

  connection.query("INSERT INTO Crescendor.record SET user_id = ?, music_id = ? score = ?, date = ?, midi = ?;", [user_id, music_id, score, date, midi], (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('addscore \n user: %s \n music: %d \n', user_id, music_id)
    res.send(rows)
  })

  connection.end();
})

// setscore API
app.put('/record/setscore/:user_id/:music_id', (req, res) => {
  const user_id = req.params.user_id
  const music_id = Number(req.params.music_id)
  const { score, date, midi } = req.body

  connection.query("UPDATE Crescendor.record SET score = ?, date = ?, midi = ? where (user_id = ? && music_id = ?);", [score, date, midi,user_id, music_id], (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
      console.log('setscore \n user: %s \n music: %d \n', user_id, music_id)
    console.log(rows)
    res.send(rows)
  })

  connection.end();
}) 

// =====================================    Practice    =====================================
app.get('/practice', (req, res) => {
  connection.query('SELECT * from Crescendor.practice;', (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('Practice info is: ', rows)
    res.send(rows)
  })

  connection.end();
})

// =====================================    Music   =====================================
app.get('/music', (req, res) => {
  connection.query('SELECT * from Crescendor.music;', (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('Music info is: ', rows)
    res.send(rows)
  })

  connection.end();
})


app.listen(app.get('port'), () => {
  console.log('Express server listening on port ' + app.get('port'))
})
