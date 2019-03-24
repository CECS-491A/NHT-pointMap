const express = require('express');
const app = express();
const mongoose = require('mongoose');
const bodyParser = require('body-parser');
const Log = require('./models/log');
var CryptoJS = require("crypto-js");

app.use(bodyParser.urlencoded({extended: false}));
app.use(bodyParser.json());

let port = 3000;
let connectionString = 'mongodb://localhost/log'
let sharedSecret = "D078F2AFC7E59885F3B6D5196CE9DB716ED459467182A19E04B6261BBC8E36EE"

mongoose.connect(connectionString, {useNewUrlParser: true}).then(()=> {
    console.log('Connected to database: ', connectionString);
},err => {
    console.log('Error connecting to db');
});

app.post('/', (req, res) => {
    console.log(req.body);
    if(!req.body.signature || !req.body.timestamp || !req.body.ssoUserId || !req.body.email){
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;          
    }
        
    let testString = "ssoUserId=" + req.body.ssoUserId + ";email=" + req.body.email + ";timestamp=" + req.body.timestamp + ";"

    var hash = CryptoJS.HmacSHA256(testString, sharedSecret);
    var hashInBase64 = CryptoJS.enc.Base64.stringify(hash);

    if(req.body.signature != hashInBase64){
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;
    }
    if(!req.body.user || !req.body.source || !req.body.desc || !req.body.createdDate){
        res.status(400).send({'Error': 'Invalid request format'});
        return
    }
    
    let log = new Log();
    log.Source = req.body.source;
    log.AssociatedUser = req.body.user;
    log.Description = req.body.desc;
    log.CreatedDate = req.body.createdDate;
    log.RecievedDate = new Date;

    Log.saveLog(log, (err, newLog) => {
        if(err) {
            res.status(500).send({'Error': 'Something went wrong'});
            console.log(err);
            return
        } else {
            res.status(200).send(newLog)
            return
        }
    });
    
});

app.listen(port, () => {
    console.log('Logging server started on port ', port);
})