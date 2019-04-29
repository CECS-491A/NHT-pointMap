let graphql = require('graphql');
const Log = require('../models/log');
const {GraphQLDate} = require('graphql-iso-date');
const {GraphQLObjectType, 
    GraphQLString, 
    GraphQLSchema,
    GraphQLID,
    GraphQLList,
    GraphQLInt,
    GraphQLFloat
 } = graphql;

let SessionType = new GraphQLObjectType({ //Type used for grabbing user duration data
    name: 'SessionType',
    fields: () => {
        return{
            sessionDuration: {
                type: GraphQLString
            }
        }
    }
});

let LoginRegisteredUsers = new GraphQLObjectType({ //Type used for grabbing user duration data
    name: 'LoginRegisteredUsers',
    fields: () => {
        return{
            totalRegisteredUsers: {
                type: GraphQLID
            },
            month: {
                type: GraphQLID
            },
            year: {
                type: GraphQLID
            },
            loginAttempts:{
                type: GraphQLID,
            }
        }
    }
});

let mostUsedFeature = new GraphQLObjectType({
    name: 'mostUsedFeature',
    fields: () => {
        return{
            topfeature : {
                type: GraphQLString
            }
        }
    }
})

let loginSuccessFail = new GraphQLObjectType({
    name: 'LoginSuccessFail',
    fields: () => {
        return{
            failedLoginAttempts:{
                type: GraphQLInt
            },
            successfulLoginAttempts:{
                type: GraphQLInt
            }
        }
    }
})

let pageUse = new GraphQLObjectType({
    name: 'pageUse',
    fields: () => {
        return{
            pageName: {
                type: GraphQLID
            }
        }
    }
})

const RootQuery = new GraphQLObjectType({
    name: 'RootQueryType',
    fields: {
        averageSessionDuration: {
            type: new GraphQLList(SessionType),
            resolve(parent, args){
                let logs = Log.aggregate([
                    { $group: { //Gets the duration of every token
                            _id: "$json.token", 
                            "sessionDuration" :  {$max : '$json.sessionDuration'}
                        }
                    },
                    {
                        $group: { //groups together the tokens to find the average 
                            _id: "sessionDuration",
                            "sessionDuration" : {$avg : "$sessionDuration"}
                        }
                    }
                ]);
                return logs
            }
        },
        successfulLoginsxRegisteredUsers:{
            type: new GraphQLList(LoginRegisteredUsers),
            resolve(parent, args){
                let logs = Log.aggregate([
                    {
                        $match: {$or :[{"source": "Registration"}, {"source": "Login"}]} //Gets only pages that were login or register
                    },
                    {
                        $group: {
                            _id: {
                                "month": {$month : {$toDate : "$logCreatedAt"}}, //Groups by month and year
                                "year": {$year : {$toDate : "$logCreatedAt"}}
                            },
                            totalRegisteredUsers:{$sum: {$cond : [//Counts successful registration users during month and year
                                {$and: [
                                    {$eq : ["$source", "Registration"]},
                                    {$eq: ["$json.success", true]}
                                ]}, 1, 0]}
                            }, 
                            month: {$max : {$month : {$toDate : "$logCreatedAt"}}}, //Gets the month
                            year: {$max : {$year : {$toDate : "$logCreatedAt"}}}, //Gets the year
                            loginAttempts: {$sum : 
                                {$cond : [
                                    {$and : [
                                        {$eq : ["$source", "Login"]}, //successful login attempt
                                        {$eq : [true, "$json.success"]}
                                    ]}, 1, 0]
                                }
                            } //Counts successful logins
                        }
                    },
                    {
                        $sort: {year: 1, month : 1}
                    }
                ])
                return logs
            }
        },
        loginAttempts: {
            type: new GraphQLList(loginSuccessFail),
            resolve(parent, args){
                let logs = Log.aggregate([
                    {
                        $match: {"source" : "Login"}
                    },
                    {
                        $group: {
                            _id: {
                                "login": "$source"
                            },
                            successfulLoginAttempts: {$sum : {$cond: [{$eq: ["$json.success", true]}, 1, 0]}}, //Sums every successful login
                            failedLoginAttempts: {$sum : {$cond: [{$eq: ["$json.success", false]}, 1, 0]}} //Sums every unsuccessful login
                        }
                    }
                ])
                return logs
            }
        },
        topFeaturesByPageVisits: {
            type: new GraphQLList(mostUsedFeature),
            resolve(parent, args){
                let logs = Log.aggregate([
                    {
                        $match: {
                            "json.page" : {"$exists": true, "$ne": null}
                        }
                    },
                    {
                        $group: {
                            _id: "$json.page",
                            topfeature: {$max: "$json.page"},
                            numUses: {$sum : 1} //Counts the number every page was pinged
                        }
                    },
                    {
                        $sort: {numUses: -1} //Sorts by number of uses in descending order
                    },
                    {
                        $limit: 5 //Takes the top 5 results
                    }
                ])
                return logs
            }
        },
        topFeaturesByPageTime: {
            type: new GraphQLList(pageUse),
            resolve(parent, args){
                let logs = Log.aggregate([
                    {
                        $match: {
                            "json.page" : {"$exists": true, "$ne": null}
                        }
                    },
                    {
                        $group: { //Groups by token duration with page
                            _id: {
                                "page" : "$json.page",
                                "token" : "$json.token"
                            },
                            "duration" :  {$max : '$json.sessionDuration'},
                            "token" :  {$max : '$json.token'},
                            "pageName": {$max: "$json.page"}
                        }
                    },
                    {
                        $group: {
                            _id: "$pageName", //Combines all durations on each page
                            "pageName": {$max: "$pageName"},
                            "duration": {$sum : "$duration"}
                        }
                    },
                    { //Sorts by duration in descending order
                        $sort: {duration: -1}
                    },
                ])
                return logs
            }
        }
    }
});

module.exports = new GraphQLSchema({ //Converts GraphQLObject to GraphQLSchema which defines capabilities of GraphQL Server
    query: RootQuery
});