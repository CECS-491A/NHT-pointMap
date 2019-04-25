const express = require('express');
const app = express();
const mongoose = require('mongoose');
const bodyParser = require('body-parser');
const Log = require('./models/log');
const graphqlHTTP = require('express-graphql')
const schema = require( './graphql/schema')
const {editData, query} = require('./services/analyticsService.js')
const {generateSignature} = require ('./services/authorizationService.js')
const fetch = require('isomorphic-fetch')
const {fillJson} = require('./services/loggingService')

app.use(bodyParser.urlencoded({extended: false}));
app.use(bodyParser.json());

const port = process.env.PORT || 3000;
const connectionString = 'mongodb://localhost/logs'

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
      
    fetch(req.protocol + '://' + req.get('host') + req.originalUrl + 'graphql', { //Makes a request for information from graphql
        method:'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify({
            query
        })
    }).then(r => r.json()).then(rawdata => { //Gets the raw request
        data = rawdata['data']
        if(data['successfulLoginsxRegisteredUsers'] == ""){
            return data
        }
        editData(data, (finishedData, err) => { //Performs an operation on the request
            if(err){
                console.log(err)
                res.status(500).send('Internal Server Error')
                return
            }
            res.status(200).send(finishedData)
            return callback(data)
        })
    }).catch(err => {
        console.log(err)
        res.status(500).send('Internal Server Error')
    })
});

app.post('/', (req, res) => {
    if(!req.body){//Check for valid body
        res.status(400).send({'Error': 'Invalid request format'});
        return
    }

    let data = req.body
    if(!data.signature || !data.timestamp || !data.ssoUserId || !data.email){ //Check for needed auth params
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;         
    }
        
    let signature = generateSignature(data.ssoUserId, data.email, data.timestamp)

    if(data.signature != signature){ //Checks if signatures match
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;
    }

    if(!data.ssoUserId || !data.email || !data.logCreatedAt || !data.source || !data.details){ //Checks for required fields
        res.status(400).send({'Error': 'Missing required request fields'});
        return;
    }



    let keys = Object.keys(data) //gets an array of body param keys

    let newLog = new Log(); //Creates log object
    newLog.json = fillJson(keys, data);//Fills the json object
    newLog.email = data.email;
    newLog.logCreatedAt = new Date(parseInt(data.logCreatedAt.substr(6)));
    newLog.source = data.source;
    newLog.details = data.details;
    newLog.ssoUserId = data.ssoUserId;

    Log.saveLog(newLog, (err, newLog) => { //Saving the log
        if(err){ //Error saving newLog
            console.log(err)
            res.status(500).send({'Error': 'Something went wrong'});
        }else{ //Successful log creation
            console.log(newLog)
            res.status(200).send(newLog)
        }
    })
});

app.listen(port, () => { //Server starts up on defined port
    console.log('Logging server started on port ', port);
})