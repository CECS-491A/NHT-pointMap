const mongoose = require('mongoose')

let sessionSchema = mongoose.Schema({
    token: {
        type: String,
        required: true
    },
    createdAt:{
        type: Date,
        required: true
    },
    updatedAt: {
        type: Date,
        required: true
    },
    expiredAt: {
        type: Date,
        required: true
    },
    userId: {
        type: String,
        required: true
    }
})

let Session = module.exports = mongoose.model('Session', sessionSchema);

module.exports.saveSession = function(newSession, callback){
    newSession.save(callback);
}