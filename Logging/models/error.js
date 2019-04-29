const mongoose = require('mongoose')

let errorSchema = mongoose.Schema({
    details: {
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
    json: {}
})

let Error = module.exports = mongoose.model('Error', errorSchema);

module.exports.saveError = function(newError, callback){
    newError.save(callback);
}