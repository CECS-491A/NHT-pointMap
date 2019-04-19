let graphql = require('graphql');
const Log = require('../models/log');
const {GraphQLDate} = require('graphql-iso-date');
const {GraphQLObjectType, 
    GraphQLString, 
    GraphQLSchema,
    GraphQLID,
    GraphQLList
 } = graphql;
const _ = require('lodash');

let SessionType = new GraphQLObjectType({ //Type used for grabbing user duration data
    name: 'SessionType',
    fields: () => {
        return{
            token: {
                type: GraphQLID //Allows for loose typing when passing the token, surrounded by quotes or not
            },
            minSessionDuration: {
                type: GraphQLString
            },
            maxSessionDuration: {
                type: GraphQLString
            }
        }
    }
});

const RootQuery = new GraphQLObjectType({
    name: 'RootQueryType',
    fields: {
        minMaxSessionDuration: {
            type: new GraphQLList(SessionType),
            resolve(parent, args){
                let logs = Log.aggregate([
                    { $group: { //Gets the min and max duration of every token
                            _id: "$json.token", 
                            token: {$min : '$json.token'},
                            minSessionDuration : {$min : '$json.sessionDuration'}, 
                            maxSessionDuration : {$max : '$json.sessionDuration'}
                        }
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