const express = require('express');
const app = express();
const mongoose = require('mongoose');
const bodyParser = require('body-parser');
const Log = require('./models/log');
const CryptoJS = require("crypto-js");
const graphqlHTTP = require('express-graphql')
const schema = require( './graphql/schema')
const fetch = require('isomorphic-fetch')

app.use(bodyParser.urlencoded({extended: false}));
app.use(bodyParser.json());

let port = process.env.PORT || 3000;
let connectionString = 'mongodb://localhost/logs'
let sharedSecret = "5E5DDBD9B984E4C95BBFF621DF91ABC9A5318DAEC0A3B231B4C1BC8FE0851610"

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
}));

app.get('/', (req, res) => { //GraphQL query
    var query = `query RootQueryType{
        averageSessionDuration{
            sessionDuration
        },
        successfulLoginsxRegisteredUsers{
            totalRegisteredUsers
            month
            year
            loginAttempts
        },
        loginAttempts{
            successfulLoginAttempts
            failedLoginAttempts
        },
        topFeaturesByPageVisits{
            topfeature
        },
        topFeaturesByPageTime{
            pageName
        }
      }`;
    fetch('http://localhost:3000/graphql', { //Makes a request for information from graphql
        method:'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify({
            query
        })
    }).then(r => r.json()).then(data => { //Gets the raw request
        editData(data['data'], (finishedData, err) => { //Performs an operation on the request
            if(err){
                console.log(err)
                res.status(500).send('Internal Server Error')
                return
            }
            res.status(200).send(finishedData)
            return
        })
    }).catch(err => {
        console.log(err)
        res.status(500).send('Internal Server Error')
    })
});

function editData(data, callback){ //Adds the users of each previous month together so totalusers displays totalUsers and not newly registered users
    let users = 0
    let count = 0
    data['successfulLoginsxRegisteredUsers'].forEach((ele => {
        count++
        users += parseInt(ele['totalRegisteredUsers'])
        ele['totalRegisteredUsers'] = users
        if(count == data['successfulLoginsxRegisteredUsers'].length)
            return callback(data)
    }))
}

app.post('/', (req, res) => {
    if(!req.body){//Check for valid body
        res.status(400).send({'Error': 'Invalid request format'});
        return
    }
    let data = req.body
    if(!data.signature || !data.timestamp || !data.ssoUserId || !data.email){ //Check for needed auth params
        res.status(401).send({'Error': 'Unauthorized Request'});
        console.log("missing auth fields")
        return;         
    }
        
    let plaintext = "ssoUserId=" + data.ssoUserId + ";email=" + data.email + ";timestamp=" + data.timestamp + ";"
    var hash = CryptoJS.HmacSHA256(plaintext, sharedSecret); //Computes auth
    var hashInBase64 = CryptoJS.enc.Base64.stringify(hash);

    if(data.signature != hashInBase64){ //Checks if signatures match
        res.status(401).send({'Error': 'Unauthorized Request'});
        console.log("Bad signature")
        return;
    }

    if(!data.ssoUserId || !data.email || !data.logCreatedAt || !data.source || !data.details){ //Checks for required fields
        res.status(400).send({'Error': 'Missing required request fields'});
        console.log("missing request fields")
        return;
    }
    let keys = Object.keys(data) //gets an array of body param keys
    let json = {}
    keys.forEach(key => {
        if(key != 'email' && key != 'logCreatedAt' && key != 'source' && key != 'details' && 
        key != 'signature' && key != 'ssoUserId' && key != 'timestamp')
            json[key] = data[key]; //Adds an unused or required field to the json object
    })
    let newLog = new Log(); //Creates log object
    newLog.email = data.email;
    newLog.logCreatedAt = new Date(parseInt(data.logCreatedAt.substr(6)));
    newLog.source = data.source;
    newLog.details = data.details;
    newLog.ssoUserId = data.ssoUserId;
    
    if('sessionCreatedAt' in json && 'sessionUpdatedAt' in json && 'sessionExpiredAt' in json){ 
        let createdDate = new Date(parseInt(json.sessionCreatedAt.substr(6))); //Formats the fields
        let updatedDate = new Date(parseInt(json.sessionUpdatedAt.substr(6)));
        let expiredAt = new Date(parseInt(json.sessionUpdatedAt.substr(6)));
        let duration = (updatedDate.getTime() - createdDate.getTime()) / 1000; //Retrieves duration of session
        json.sessionCreatedAt = createdDate;
        json.sessionUpdatedAt = updatedDate;
        json.sessionExpiredAt = expiredAt;
        json['sessionDuration'] = duration;//Adds duration of session
    }
    newLog.json = json
    Log.saveLog(newLog, (err, newLog) => { //Saving the log
        if(err){ //Error saving newLog
            console.log(err)
            res.status(500).send({'Error': 'Something went wrong'});
        }else{ //Successful log creation
            res.status(200).send(newLog)
        }
    })
});

app.listen(port, () => { //Server starts up on defined port
    console.log('Logging server started on port ', port);
})