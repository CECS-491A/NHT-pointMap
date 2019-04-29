const CryptoJS = require("crypto-js");
const sharedSecret = "CHRISTOPHER-123456-NIGHTWATCH-POINTMAP"

module.exports.generateSignature = function(timestamp){
    let plaintext = "timestamp=" + timestamp + ";details=" + details + ";timestamp=" + timestamp + ";"
    var hash = CryptoJS.HmacSHA256(plaintext, sharedSecret); //Computes auth
    var hashInBase64 = CryptoJS.enc.Base64.stringify(hash);
    return hashInBase64
}