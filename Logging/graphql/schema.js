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
const _ = require('lodash');

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
        minMaxSessionDuration: {
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
        successfulLoginsxRegisteredUsersMonth:{
            type: new GraphQLList(LoginRegisteredUsers),
            resolve(parent, args){
                let logs = Log.aggregate([
                    {
                        $match: {$or :[{"json.page": "Registration"}, {"json.page": "Login"}]} //Gets only pages that were login or register
                    },
                    {
                        $group: {
                            _id: {
                                "month": {$month : {$toDate : "$logCreatedAt"}}, //Groups by month and year
                                "year": {$year : {$toDate : "$logCreatedAt"}}
                            },
                            totalRegisteredUsers:{$sum: {$cond : [//Counts successful registration users during month and year
                                {$and: [
                                    {$eq : ["$json.page", "Registration"]},
                                    {$eq: ["$json.success", true]}
                                ]}, 1, 0]}}, 
                            month: {$max : {$month : {$toDate : "$logCreatedAt"}}}, //Gets the month
                            year: {$max : {$year : {$toDate : "$logCreatedAt"}}}, //Gets the year
                            loginAttempts: {$sum : 
                                {$cond : [
                                    {$and : [
                                        {$eq : ["$json.page", "Login"]}, //successful login attempt
                                        {$eq : [true, "$json.success"]}
                                    ]}, 1, 0]
                                }} //Counts successful logins
                        }
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
                        $match: {"json.page" : "Login"}
                    },
                    {
                        $group: {
                            _id: {
                                "login": "$json.page"
                            },
                            successfulLoginAttempts: {$sum : {$cond: [{$eq: ["$json.success", true]}, 1, 0]}},
                            failedLoginAttempts: {$sum : {$cond: [{$eq: ["$json.success", false]}, 1, 0]}}
                        }
                    }
                ])
                return logs
            }
        },
        topFeatures: {
            type: new GraphQLList(mostUsedFeature),
            resolve(parent, args){
                let logs = Log.aggregate([
                    {
                        $group: {
                            _id: "$json.page",
                            topfeature: {$max: "$json.page"},
                            numUses: {$sum : 1}
                        }
                    },
                    {
                        $sort: {numUses: -1}
                    },
                    {
                        $limit: 5
                    }
                ])
                return logs
            }
        },
        longestPageUse: {
            type: new GraphQLList(pageUse),
            resolve(parent, args){
                let logs = Log.aggregate([
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