const express = require('express');
const app = express();
const mongoose = require('mongoose');
const bodyParser = require('body-parser');
const Log = require('./models/log');

app.use(bodyParser.urlencoded({extended: false}));
app.use(bodyParser.json());

let port = 3000;
let connectionString = 'mongodb://localhost/log'

mongoose.connect(connectionString, {useNewUrlParser: true}).then(()=> {
    console.log('Connected to database: '. connectionString);
},err => {
    console.log('Error connecting to db');
});

app.post('/', (req, res) => {
    let log = new Log();
    log.Source = req.body.source;
    log.AssociatedUser = req.body.user;
    log.Description = req.body.desc;
    log.CreatedAt = new Date;

    Log.saveLog(log, (err, newLog) => {
        if(err) {
            res.status(400);
            console.log(err);
        } else {
            res.status(200)
            res.send(newLog);
        }
    });
});

app.listen(port, () => {
    console.log('Logging server started on port ', port);
})