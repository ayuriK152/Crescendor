const express = require('express')
const mysql = require('mysql')
const db = require('./config/database.js')
const connection = mysql.createConnection(db)
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
  res.status(200).send('Root')
})

// =====================================    Users     =====================================
app.get('/users', (req, res) => {
  pool.query('SELECT * FROM Crescendor.users;', (error, rows) => {
    if (error){
      res.status(400).send('ERROR: Data')
      return
    }
    console.log('User info is: ', rows)
    res.status(200).send(rows)
  })
  
})

// signup API (회원가입)
// 실패하면 ERROR, 성공하면 SUCCESS 리턴
app.post('/signup', (req, res) => {
  const { id, password } = req.body;
  const hashedPassword = bcrypt.hashSync(password, 10)

    pool.getConnection((err, connection)=>{
      connection.query(`INSERT INTO Crescendor.users SET id = "${id}", nickname = "${id}", password = "${hashedPassword}";`, (error, rows) => {
        if (error){
          console.log(error)
          res.status(400).send('ERROR: id')
          return
        }
        res.status(200).send('SUCCESS')
      })
    })
})

// login API (로그인)
// 실패하면 ERROR, 성공하면 SUCCESS 리턴
app.post('/login', (req, res) => {
  const { id, password } = req.body

  pool.query(`SELECT password from Crescendor.users where id = "${id}";`, (error, rows) => {
    if (error){
      res.status(400).send('ERROR: Data')
      return
    }
    if (rows == null){
      res.status(400).send('ERROR: id')
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
      res.status(400).send('ERROR: Data')
      return
    }
    console.log('Record info is: ', rows)
    res.status(200).send(rows)
  })

})

// getscore API
app.get('/record/getscore/:user_id/:music_name', (req, res) => {
  const user_id = req.params.user_id
  const music_name = req.params.music_name

  pool.query(`SELECT score from Crescendor.record where (user_id = "${user_id}" && music_name = "${music_name}");`, (error, rows) => {
    if (error){
      res.status(400).send('ERROR: Data')
      return
    }
    console.log('getscore \n user: %s \n music: %d \n', user_id, music_name)
    console.log(rows)
    res.status(200).send(rows)
  })
})
// addscore API
app.post('/record/addscore/:user_id/:music_name', (req, res) => {
  const user_id = req.params.user_id
  const music_name = req.params.music_name
  const { score,  midi } = req.body

  console.log(midi)

  let today = new Date() 
  const date = new String(
    today.getFullYear() + '-' + (today.getMonth()+1) + '-' + today.getDate() + " " + (today.getUTCHours()+1)  + ':' + today.getMinutes() + ':' + today.getSeconds()
    ).valueOf()

  pool.query(`INSERT INTO Crescendor.record SET user_id = "${user_id}", music_name = "${music_name}", score = ${score}, date = "${date}", midi = '\{"tempo":${midi.tempo}, "noteRecords": "${midi.noteRecords}"\}';`, (error, rows) => {
    if (error){
      console.log(error)
      res.status(400).send('ERROR: Exist Record')
      return
    }
    // console.log('addscore \n user: %s \n music: %d \n', user_id, music_name)
    res.status(200).send("SUCCESS")
  })

  
})

// setscore API
app.put('/record/setscore/:user_id/:music_name', (req, res) => {
  const user_id = req.params.user_id
  const music_name = req.params.music_name
  const { score, midi } = req.body

  let today = new Date()
  const date = new String(
    today.getFullYear() + '-' + (today.getMonth()+1) + '-' + today.getDate() + " " + (today.getUTCHours() + 1) + ':' + today.getMinutes() + ':' + today.getSeconds()
    ).valueOf()

  pool.query(`UPDATE Crescendor.record SET score = ${score}, date = '${date}', midi = '\{"tempo" : ${midi.tempo}, "noteRecords" : "${midi.noteRecords}", "originFileName" : "${midi.originFileName}"\}' where (user_id = "${user_id}" and music_name = "${music_name}");`, (error, rows) => {

    if (error){
      console.log(error)
      res.status(400).send('ERROR: Data')
      return
    }
    // console.log('setscore \n user: %s \n music: %d \n', user_id, music_name)
    // console.log(rows)
    res.status(200).send("SUCCESS")
  })

  
}) 

// ranking API
app.get('/ranking/:music_name', (req, res) => {
  const music_name = req.params.music_name

  pool.query(`SELECT music_name, user_id, score, date, midi from Crescendor.record where music_name = "${music_name}" order by 3 DESC, 4 ASC;`, (error, rows) => {
    if (error){
      res.status(400).send('ERROR: Data')
      return
    }
    console.log('Ranking \n music: %s \n', music_name)
    console.log(rows)
    res.status(200).send(rows)
  })
})

// =====================================    Practice    =====================================
app.get('/practice', (req, res) => {
  pool.query('SELECT * from Crescendor.practice;', (error, rows) => {
    if (error){
      res.status(400).send('ERROR: Data')
      return
    }
    console.log('Practice info is: ', rows)
    res.status(200).send(rows)
  })

  
})

// =====================================    Music   =====================================
app.get('/music', (req, res) => {
  pool.query('SELECT * from Crescendor.music;', (error, rows) => {
    if (error){
      res.status(400).send('ERROR: Data')
      return
    }
    console.log('Music info is: ', rows)
    res.status(200).send(rows)
  })

  
})


app.listen(app.get('port'), () => {
  console.log('Express server listening on port ' + app.get('port'))
})
