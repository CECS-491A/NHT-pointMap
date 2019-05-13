const graphql = require('graphql');
const Log = require('../models/log');
const Error = require('../models/error');
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

 let comparisonDate = new Date();
 comparisonDate.setMonth(comparisonDate.getMonth() - 6);
 let comparisonYear = comparisonDate.getFullYear();
 let comparisonMonth = comparisonDate.getMonth();

let SessionType = new GraphQLObjectType({ //Type used for grabbing user duration data
    name: 'SessionType',
    fields: () => {
        return{
            sessionDuration: {
                type: GraphQLString
            },
            month: {
                type: GraphQLID
            },
            year: {
                type: GraphQLID
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

let LoggedInUsers = new GraphQLObjectType({ //Type used for grabbing user duration data
    name: 'LoggedInUsers',
    fields: () => {
        return{
            month: {
                type: GraphQLID
            },
            year: {
                type: GraphQLID
            },
            numLogins:{
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
            },
            month: {
                type: GraphQLID
            },
            year: {
                type: GraphQLID
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
                    {
                        $match: {"json.token": {"$exists": true, "$ne": null}}
                    },
                    { $group: { //Gets the duration of every token
                            _id: {
                                "token": "$json.token",
                                "year" : {$year :  "$logCreatedAt"},
                                "month" : {$month : "$logCreatedAt"}

                            },
                            "sessionDuration" :  {$max : '$json.sessionDuration'},
                            year: {$max : {$year :  "$logCreatedAt"}},
                            month: {$max : {$month :  "$logCreatedAt"}}

                        }
                    },
                    {
                        $group: { //groups together the tokens to find the average 
                            _id: {
                                "year" : "$year",
                                "month": "$month"
                            },
                            "sessionDuration" : {$avg : "$sessionDuration"},
                            "year": {$max: "$year"},
                            "month": {$max: "$month"}
                        }
                    },
                    {
                        $sort: {year: 1, month : 1}
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
                                "month": {$month : "$logCreatedAt"}, //Groups by month and year
                                "year": {$year : "$logCreatedAt"}
                            },
                            totalRegisteredUsers:{$sum: {$cond : [//Counts successful registration users during month and year
                                {$eq : ["$source", "Registration"]}, 1, 0]}
                            }, 
                            month: {$max : {$month : "$logCreatedAt"}}, //Gets the month
                            year: {$max : {$year : "$logCreatedAt"}}, //Gets the year
                            loginAttempts: {$sum : 
                                {$cond : [
                                    {$eq : ["$source", "Login"]}, 1, 0]
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
                            _id: {
                                page: "$json.page",
                            },
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
                            "json.pageDuration" : {"$exists": true, "$ne": null}
                        }
                    },
                    {
                        $group: { //Groups by token duration with page
                            _id: {
                                "page" : "$json.page",
                                "token" : "$json.token"
                            },
                            "duration" :  {$max : '$json.pageDuration'},
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
                    {
                        $match: {
                            "pageName" : {"$exists": true, "$ne": null}
                        }
                    },
                    { //Sorts by duration in descending order
                        $sort: {duration: -1}
                    },
                ])
                return logs
            }
        },
        averageUserLogin6Months: {
            type: new GraphQLList(LoggedInUsers),
            resolve(parent, args){
                let logs = Log.aggregate([
                    {
                        $match: {"source": "Login"}
                    },
                    {
                        $group: {
                            _id: {
                                "token": "$json.token",
                                "year": {$year : "$logCreatedAt"},
                                "month" : {$month : "$logCreatedAt"},
                            },
                            month: {$max: { 
                                $cond: [
                                    //logMonth > compMonth && logYear >= comparisonYear || logMonth < compMonth && logYear > compYear
                                    {$or : [
                                        {$and : [
                                            {$gte: [{$month : "$logCreatedAt"}, comparisonMonth]},
                                            {$gte: [{$year : "$logCreatedAt"}, comparisonYear]}
                                        ]}, 
                                        {$and : [
                                            {$lt: [{$month : "$logCreatedAt"}, comparisonMonth]},
                                            {$gt: [{$year : "$logCreatedAt"}, comparisonYear]}
                                        ]}, 
                                    ]},
                                    {$month : "$logCreatedAt"},
                                    "0"
                                ]}
                            },
                            year: {$max: { 
                                $cond: [
                                    {$or : [
                                        {$and : [
                                            {$gte: [{$month : "$logCreatedAt"}, comparisonMonth]},
                                            {$gte: [{$year : "$logCreatedAt"}, comparisonYear]}
                                        ]}, 
                                        {$and : [
                                            {$lt: [{$month : "$logCreatedAt"}, comparisonMonth]},
                                            {$gt: [{$year : "$logCreatedAt"}, comparisonYear]}
                                        ]}, 
                                    ]},
                                    {$year : "$logCreatedAt"},
                                    "0"
                                ]}
                            },
                            source: {$max: "$source"}
                        }
                    },
                    {
                        $match: {
                            $and: [
                                {"source": "Login"},
                                {"month": {"$exists": true, "$ne": "0"}},
                                {"year": {"$exists": true, "$ne": "0"}}
                            ]
                        } //Gets only pages that were login or register
                    },
                    {
                        $group: {
                            _id: {
                                "month": "$month", //Groups by month and year
                                "year": "$year"
                            },
                            "year": {$max: "$year"},
                            "month": {$max: "$month"},
                            numLogins: {$sum : 1 } //Counts successful logins
                        }
                    },
                    {
                        $sort: {year: 1, month : 1}
                    }
                ])
                return logs
            }
        },

        averageSessionDuration6Months:{
            type: new GraphQLList(SessionType),
            resolve(parent, args){
                let logs = Log.aggregate([
                    {
                        $group: {
                            _id: {
                                "token": "$json.token",
                                "year": {$year : "$logCreatedAt"},
                                "month" : {$month : "$logCreatedAt"}
                            },
                            month: {$max: { 
                                $cond: [
                                    //logMonth > compMonth && logYear >= comparisonYear || logMonth < compMonth && logYear > compYear
                                    {$or : [
                                        {$and : [
                                            {$gte: [{$month : "$logCreatedAt"}, comparisonMonth]},
                                            {$gte: [{$year : "$logCreatedAt"}, comparisonYear]}
                                        ]}, 
                                        {$and : [
                                            {$lt: [{$month : "$logCreatedAt"}, comparisonMonth]},
                                            {$gt: [{$year : "$logCreatedAt"}, comparisonYear]}
                                        ]}, 
                                    ]},
                                    {$month : "$logCreatedAt"},
                                    "0"
                                ]}
                            },
                            year: {$max: { 
                                $cond: [
                                    {$or : [
                                        {$and : [
                                            {$gte: [{$month : "$logCreatedAt"}, comparisonMonth]},
                                            {$gte: [{$year : "$logCreatedAt"}, comparisonYear]}
                                        ]}, 
                                        {$and : [
                                            {$lt: [{$month : "$logCreatedAt"}, comparisonMonth]},
                                            {$gt: [{$year : "$logCreatedAt"}, comparisonYear]}
                                        ]}, 
                                    ]},
                                    {$year : "$logCreatedAt"},
                                    "0"
                                ]}
                            },
                            token: {$max: "$json.token"},
                            sessionDuration: {$max: "$json.sessionDuration"}
                        }
                    },
                    {
                        $match: {$and :[
                            {"token": {"$exists": true, "$ne": null}},
                            {"month": {"$exists": true, "$ne": "0"}},
                            {"year": {"$exists": true, "$ne": "0"}}
                        ]}
                    },
                    {
                        $group: { //groups together the tokens to find the average 
                            _id: {
                                "year" : "$year",
                                "month": "$month"
                            },
                            "sessionDuration" : {$avg : "$sessionDuration"},
                            "year": {$max: "$year"},
                            "month": {$max: "$month"}
                        }
                    },
                    {
                        $sort: {year: 1, month : 1}
                    }
                ]);
                return logs
            }
        }
    }
});

module.exports = new GraphQLSchema({ //Converts GraphQLObject to GraphQLSchema which defines capabilities of GraphQL Server
    query: RootQuery
});