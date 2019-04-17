let graphql = require('graphql');
const Session = require('../models/session');
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
            createdAt: {
                type: GraphQLDate
            },
            updatedAt: {
                type: GraphQLDate
            },
            expiredAt: {
                type: GraphQLDate
            }
        }
    }
});

const RootQuery = new GraphQLObjectType({
    name: 'RootQueryType',
    fields: {
        session: {
            type: SessionType,
            args:{
                token: {type: GraphQLID} //Arguments passed when making a query to session, i.e. session(token: "...")
            },
            resolve(parent, args){
                //code to get data from db
                
            }
        },
        sessions: {
            type: new GraphQLList(SessionType),
            resolve(parent, args){
                
            }
        }
    }
});

module.exports = new GraphQLSchema({ //Converts GraphQLObject to GraphQLSchema which defines capabilities of GraphQL Server
    query: RootQuery
});