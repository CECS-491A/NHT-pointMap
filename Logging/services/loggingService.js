const mongoose = require('mongoose');

module.exports.fillJson = function(keys, data){
    let json = {}
    for(i = 0; i < keys.length; i++){
        let key = keys[i]
        if(key != 'logCreatedAt' && key != 'source' && key != 'details' && 
        key != 'signature' && key != 'ssoUserId' && key != 'timestamp')
            json[key] = data[key]; //Adds an unused or required field to the json object
    }

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

    return json
}

module.exports.checkLogSpace = function(callback){
    let db = mongoose.connection;
    db.db.stats({scale: 1024}, (err, stats) => { //bytes are in kilobytes
        if(err){
            console.log(err)
            return callback(false);
        }else{
            if(stats['fsUsedSize'] > (stats['fsTotalSize'] + 2048)) //at least two megabytes left before storing a log
                return callback(false);
            return callback(true);
        }
    })
}