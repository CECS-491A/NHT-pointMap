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
    Details:{
        type: String,
        required: false
    },
    CreatedDate: {
        type: String,
        required: true
    },
    RecievedDate:{
        type: Date,
        required: true
    },
    Category:{
        type: String,
        required: true
    },
    Successful: {
        type: String,
        required: true
    }
});

let Log = module.exports = mongoose.model('Log', logSchema);

module.exports.saveLog = function(newLog, callback){
    newLog.save(callback);
}