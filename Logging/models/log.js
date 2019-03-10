const mongoose = require('mongoose');

let logSchema = mongoose.Schema({
    Source: {
        type: String,
        required: true
    },
    AssociatedUser: {
        type: String,
        required: false
    },
    Description:{
        type: String,
        required: false
    },
    CreatedAt: {
        type: Date,
        required: true
    }
});

let Log = module.exports = mongoose.model('Log', logSchema);

module.exports.saveLog = function(newLog, callback){
    newLog.save(callback);
}