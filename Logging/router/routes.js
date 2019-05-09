const express = require('express');
const router = express.Router();
const {fillJson, checkLogSpace} = require('../services/loggingService')
const {generateSignature} = require ('../services/authorizationService.js')
const {editData, query, addLoginAttempts} = require('../services/analyticsService.js')
const fetch = require('isomorphic-fetch')
const Log = require('../models/log')
const Error = require('../models/error')

router.post('/log', (req, res) => { //POST ROUTE FOR ADDING AN ANALYTICS LOG
    if(!req.body){//Check for valid body
        res.status(400).send({'Error': 'Invalid request format'});
        return
    }

    let data = req.body
    if(!data.signature || !data.timestamp || !data.salt){ //Check for needed auth params
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;         
    }

    let signature = generateSignature(data.salt, data.timestamp)

    if(data.signature != signature){ //Checks if signatures match
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;
    }

    if(!data.ssoUserId || !data.logCreatedAt || !data.source){ //Checks for required fields
        res.status(400).send({'Error': 'Missing required request fields'});
        return;
    }

    let keys = Object.keys(data) //gets an array of body param keys

    let newLog = new Log(); //Creates log object
    newLog.json = fillJson(keys, data);//Fills the json object
    newLog.logCreatedAt = new Date(parseInt(data.logCreatedAt.substr(6)));
    newLog.source = data.source;
    newLog.ssoUserId = data.ssoUserId;

    checkLogSpace((canStore) => { //return boolean through callback function
        if(canStore){
            Log.saveLog(newLog, (err, newLog) => { //Saving the log
                if(err){ //Error saving newLog
                    console.log(err)
                    res.status(500).send({'Error': 'Something went wrong'});
                }else{ //Successful log creation
                    res.status(200).send(newLog)
                }
            })
        }else{
            res.status(503).send({'Error': 'Logging service temporarily unavailable'}) //Service Unavailable
        }
    })   
});

router.post('/error', (req, res) => { //POST ROUTE FOR ADDING AN ERROR
    if(!req.body){//Check for valid body
        res.status(400).send({'Error': 'Invalid request format'});
        return
    }

    let data = req.body
    if(!data.signature || !data.timestamp || !data.salt){ //Check for needed auth params
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;         
    }
    let signature = generateSignature(data.salt, data.timestamp)

    if(data.signature != signature){ //Checks if signatures match
        res.status(401).send({'Error': 'Unauthorized Request'});
        return;
    }

    if(!data.details || !data.logCreatedAt || !data.source){ //Checks for required fields
        res.status(400).send({'Error': 'Missing required request fields'});
        return;
    }

    let keys = Object.keys(data) //gets an array of body param keys

    let newError = new Error(); //Creates log object
    newError.json = fillJson(keys, data);//Fills the json object
    newError.logCreatedAt = new Date(parseInt(data.logCreatedAt.substr(6)));
    newError.source = data.source;
    newError.details = data.details;

    checkLogSpace((canStore) => { //return boolean through callback function
        if(canStore){
            Error.saveError(newError, (err, newError) => { //Saving the log
                if(err){ //Error saving newError
                    console.log(err)
                    res.status(500).send({'Error': 'Something went wrong'});
                }else{ //Successful log creation
                    res.status(200).send(newError)
                }
            })
        }else{
            res.status(503).send({'Error': 'Logging service temporarily unavailable'}) //Service Unavailable
        }
    })   
});

router.get('/analytics', (req, res) => { //GET ROUTE FOR ANALYTICS DATA
    console.log(req.protocol + '://' + req.get('host') + 'graphql')
    fetch(req.protocol + '://' + req.get('host') + '/graphql', { //Makes a request for information from graphql
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
        data['loginAttempts'] = addLoginAttempts()
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
            return
        })
    }).catch(err => {
        console.log(err)
        res.status(500).send('Internal Server Error')
        return
    })
});

module.exports = router;