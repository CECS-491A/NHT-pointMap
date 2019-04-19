const mongoose = require('mongoose')

let logSchema = mongoose.Schema({
    ssoUserId: {
        type: String,
        required: true
    },
    email:{
        type: String,
        required: true
    },
    logCreatedAt: {
        type: Date,
        required: true
    },
    source: {
        type: String,
        required: true
    },
    details: {
        type: String,
        required: true
    },
    json: {}
})

let Log = module.exports = mongoose.model('Log', logSchema);

module.exports.saveLog = function(newLog, callback){
    newLog.save(callback);
}