const express = require('express');
const app = express();
const mongoose = require('mongoose');
const bodyParser = require('body-parser');
const Log = require('./models/log');
const CryptoJS = require("crypto-js");
const graphqlHTTP = require('express-graphql')
const schema = require( './graphql/schema')

app.use(bodyParser.urlencoded({extended: false}));
app.use(bodyParser.json());

let port = process.env.PORT || 3000;
let connectionString = 'mongodb://localhost/logs'
let sharedSecret = "FDF9B8E2935D6F4C7336604164B92B82E36D1BD87FF96333194D41FDDA023449"

//Connects to local mongodb instance using defined connection string
mongoose.connect(connectionString, {useNewUrlParser: true}).then(()=> {
    console.log('Connected to database: ', connectionString);
},err => {
    console.log('Error connecting to db');
});

//Setsup the graphql route using the rootSchema
app.use('/graphql', graphqlHTTP({
    schema,
    graphiql: true
}))

app.post('/', (req, res) => {
    if(!req.body){
        res.status(400).send({'Error': 'Invalid request format'});
        return
    }
    if(!req.body.signature || !req.body.timestamp || !req.body.ssoUserId || !req.body.email){
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;         
    }
        
    let plaintext = "ssoUserId=" + req.body.ssoUserId + ";email=" + req.body.email + ";timestamp=" + req.body.timestamp + ";"
    var hash = CryptoJS.HmacSHA256(plaintext, sharedSecret);
    var hashInBase64 = CryptoJS.enc.Base64.stringify(hash);

    if(req.body.signature != hashInBase64){
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;
    }
    if(!req.body.user || !req.body.source || !req.body.desc || !req.body.createdDate || !req.body.details){
        res.status(400).send({'Error': 'Invalid request format'});
        return
    }
    let newLog = new Log();
    newLog.Source = req.body.source;
    newLog.AssociatedUser = req.body.user;
    newLog.Description = req.body.desc;
    newLog.Details = req.body.details;
    newLog.CreatedDate = req.body.createdDate;
    newLog.RecievedDate = new Date;


    Log.saveLog(newLog, (err, newLog) => {
        if(err) {
            res.status(500).send({'Error': 'Something went wrong'});
            return;
        } else {
            console.log(newLog)
            res.status(200).send(newLog);
            return; 
        }
    });
});

app.listen(port, () => {
    console.log('Logging server started on port ', port);
})