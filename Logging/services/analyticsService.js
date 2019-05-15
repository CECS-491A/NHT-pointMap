const Error = require('../models/error')
const Log = require('../models/log')


module.exports.query = `query RootQueryType{
    averageSessionDuration{
        month
        year
        sessionDuration
    },
    successfulLoginsxRegisteredUsers{
        month
        year
        totalRegisteredUsers
        loginAttempts
    },
    topFeaturesByPageVisits{
        topfeature
        numUses
    },
    topFeaturesByPageTime{
        pageName
        duration
    },
    averageUserLogin6Months{
        month
        year
        numLogins
    },
    averageSessionDuration6Months{
        month
        year
        sessionDuration
    }
  }`;

module.exports.editData = function(data, callback){ //Adds the users of each previous month together so totalusers displays totalUsers and not newly registered users
    let users = 0
    let count = 0
    if(data['successfulLoginsxRegisteredUsers'] == null)
        return callback(data)
    data['successfulLoginsxRegisteredUsers'].forEach((ele => {
        count++
        users += parseInt(ele['totalRegisteredUsers'])
        ele['totalRegisteredUsers'] = users
        if(count == data['successfulLoginsxRegisteredUsers'].length)
            return callback(data)
    }))
}

module.exports.addLoginAttempts = function(callback){
    let logs = Log.aggregate([
        {
            $match: {"source" : "Login"}
        },
        {
            $group: {
                _id: {
                    "year" : {$year : "$logCreatedAt"},
                    "month" : {$month : "$logCreatedAt"}
                },
                successfulLoginAttempts: {$sum : 1}, //Sums every successful login
                year: {$max : {$year : "$logCreatedAt"}},
                month: {$max : {$month : "$logCreatedAt"}}
            }
        },
        {
            $sort: {year: 1, month : 1}
        },
        {
            $project:{
                month: "$month",
                year: "$year",
                successfulLoginAttempts: "$successfulLoginAttempts",
                _id: 0
            }
        }
    ])
    let errors = Error.aggregate([
        {
            $match: {"source": "Login"}
        },
        {
            $group: {
                _id: {
                    "year" : {$year : "$logCreatedAt"},
                    "month" : {$month : "$logCreatedAt"}
                },
                failedLoginAttempts: {$sum : 1}, //Sums every successful login
                year: {$max : {$year : "$logCreatedAt"}},
                month: {$max : {$month : "$logCreatedAt"}}
            }
        },
        {
            $sort: {year: 1, month : 1}
        },
        {
            $project:{
                month: "$month",
                year: "$year",
                failedLoginAttempts: "$failedLoginAttempts",
                _id: 0
            }
        }
    ])
    logs.exec((err, logData) => {
        if(err){
            console.log(err)
            return {}
        }else{
            errors.exec((err, errorData) => {
                if(err){
                    console.log(err)
                    return callback({})
                }else{
                    let errjson = {};
                    let analyticsJson = {};
                    let temp = {}
                    for(var i = 0; i < logData.length; i++){
                        temp = {}
                        temp['successfulLoginAttempts'] = logData[i].successfulLoginAttempts
                        temp['month'] = logData[i].month
                        temp['year'] = logData[i].year
                        let key = logData[i].year.toString() + "-" + logData[i].month.toString()
                        analyticsJson[key] = temp
                    }
                    for(var i = 0; i < errorData.length; i++){
                        temp = {}
                        temp['failedLoginAttempts'] = errorData[i].failedLoginAttempts
                        temp['month'] = errorData[i].month
                        temp['year'] = errorData[i].year
                        temp['successfulLoginAttempts'] = 0
                        let key = errorData[i].year.toString() + "-" + errorData[i].month.toString()
                        errjson[key] = temp
                    }
                    Object.keys(analyticsJson).forEach((key) => {
                        if(errjson[key] == null){
                            temp = {}
                            temp['failedLoginAttempts'] = 0
                            errjson[key] = temp
                        }
                        errjson[key].month = analyticsJson[key].month
                        errjson[key].year = analyticsJson[key].year
                        errjson[key].successfulLoginAttempts = analyticsJson[key].successfulLoginAttempts
                    })
                    return callback(errjson)
                }
            })
        }
    })
}

