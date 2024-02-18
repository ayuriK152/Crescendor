const express = require('express')
const mysql = require('mysql')
const db = require('./config/database.js')
const pool = mysql.createPool({
  connectionLimit : 10,
  host            : db.host,
  user            : db.user,
  password        : db.password,
  database        : db.database
})
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
  pool.query('SELECT * FROM Crescendor.users;', (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('User info is: ', rows)
    res.send(rows)
  })
  
})

// signup API (회원가입)
// 실패하면 ERROR, 성공하면 SUCCESS 리턴
app.post('/signup', (req, res) => {
  const { id, password } = req.body;

  pool.getConnection((err, connection)=>{
    
    var check = connection.query('SELECT count(*) from Crescendor.users where id = ?;', id, (error, rows) => {

      if (rows[0]['count(*)'] > 0){
        res.status(400).send('ERROR: exist id')
        return
      }
      
      console.log('signup available')
    })

    check.on('error', function(err) {
      res.status(400).send('ERROR: MySQL')
      return
    })

    check.on('end', function(err) {
      const hashedPassword = bcrypt.hashSync(password, 10)
      console.log(hashedPassword)
      signup(id,hashedPassword)
    })  
  })
})

function signup(id, password){
  pool.getConnection((err, connection)=>{
    connection.query("INSERT INTO Crescendor.users SET id = ?, nickname = ?, password = ? ;", [id, id, password], (error, rows) => {
      if (error){
        res.status(400).send('ERROR: MySQL')
        return
      }
      console.log('signup \n id: %s \n', id)
      res.status(200).send('SUCCESS')
    })
  })
}

// login API (로그인)
// 실패하면 ERROR, 성공하면 SUCCESS 리턴
app.post('/login', (req, res) => {
  const { id, password } = req.body

  pool.query('SELECT password from Crescendor.users where id = ?;', id, (error, rows) => {
    if (error){
      res.status(400).send('ERROR: MySQL')
      return
    }
    if (rows == null){
      res.status(400).send(`ERROR: id`)
      return
    }

    const user_password = rows[0].password
    const matchPassword =  bcrypt.compareSync(password, user_password)

    if (!matchPassword) {
        res.status(400).send('ERROR: password')
        return
    }
    
    if (matchPassword){
      console.log('Login: %s',id)
      res.status(200).send('SUCCESS')
      return
    }
  })
})

// =====================================    Record    =====================================
app.get('/record', (req, res) => {
  pool.query('SELECT * from Crescendor.record;', (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('Record info is: ', rows)
    res.send(rows)
  })

})

// getscore API
app.get('/record/getscore/:user_id/:music_id', (req, res) => {
  const user_id = req.params.user_id
  const music_id = Number(req.params.music_id)

  pool.query("SELECT score from Crescendor.record where (user_id = ? && music_id = ?);", [user_id, music_id], (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('getscore \n user: %s \n music: %d \n', user_id, music_id)
    console.log(rows)
    res.send(rows)
  })
})

// addscore API
app.post('/record/addscore/:user_id/:music_id', (req, res) => {
  const user_id = req.params.user_id
  const music_id = Number(req.params.music_id)
  const { score,  midi } = req.body

  let today = new Date() 
  const date = new String(
    today.getFullYear + '-' + today.getMonth + '-' + today.getDate + " " +
    today.getHours + ':' + today.getMinutes + ':' + today.getSeconds
    ).valueOf()

  pool.query("INSERT INTO Crescendor.record SET user_id = ?, music_id = ? score = ?, date = ?, midi = ?;", [user_id, music_id, score, date, midi], (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('addscore \n user: %s \n music: %d \n', user_id, music_id)
    res.send(rows)
  })

  
})

// setscore API
app.put('/record/setscore/:user_id/:music_id', (req, res) => {
  const user_id = req.params.user_id
  const music_id = Number(req.params.music_id)
  const { score, midi } = req.body

  let today = new Date() 
  const date = new String(
    today.getFullYear + '-' + today.getMonth + '-' + today.getDate + " " +
    today.getHours + ':' + today.getMinutes + ':' + today.getSeconds
    ).valueOf()

  pool.query("UPDATE Crescendor.record SET score = ?, date = ?, midi = ? where (user_id = ? && music_id = ?);", [score, date, midi,user_id, music_id], (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
      console.log('setscore \n user: %s \n music: %d \n', user_id, music_id)
    console.log(rows)
    res.send(rows)
  })

  
}) 

// ranking API
app.get('/ranking/:music_name', (req, res) => {
  const music_name = req.params.music_name

  pool.query("SELECT name, user_id, score, date, record.midi from Crescendor.record, Crescendor.music where name = ? and record.music_id = music.id order by 3 DESC, 4 ASC;", music_name, (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('Ranking \n music: %s \n', music_name)
    console.log(rows)
    res.send(rows)
  })
})

// =====================================    Practice    =====================================
app.get('/practice', (req, res) => {
  pool.query('SELECT * from Crescendor.practice;', (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('Practice info is: ', rows)
    res.send(rows)
  })

  
})

// =====================================    Music   =====================================
app.get('/music', (req, res) => {
  pool.query('SELECT * from Crescendor.music;', (error, rows) => {
    if (error){
      res.send('ERROR: MySQL')
      return
    }
    console.log('Music info is: ', rows)
    res.send(rows)
  })

  
})


app.listen(app.get('port'), () => {
  console.log('Express server listening on port ' + app.get('port'))
})
