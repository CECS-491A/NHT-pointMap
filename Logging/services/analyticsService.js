module.exports.query = `query RootQueryType{
    averageSessionDuration{
        sessionDuration
    },
    successfulLoginsxRegisteredUsers{
        totalRegisteredUsers
        month
        year
        loginAttempts
    },
    loginAttempts{
        successfulLoginAttempts
        failedLoginAttempts
    },
    topFeaturesByPageVisits{
        topfeature
    },
    topFeaturesByPageTime{
        pageName
    }
  }`;

module.exports.editData = function(data, callback){ //Adds the users of each previous month together so totalusers displays totalUsers and not newly registered users
    let users = 0
    let count = 0
    data['successfulLoginsxRegisteredUsers'].forEach((ele => {
        count++
        users += parseInt(ele['totalRegisteredUsers'])
        ele['totalRegisteredUsers'] = users
        if(count == data['successfulLoginsxRegisteredUsers'].length)
            return callback(data)
    }))
}

