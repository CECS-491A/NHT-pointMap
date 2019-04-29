const express = require('express');
const app = express();
const mongoose = require('mongoose');
const bodyParser = require('body-parser');
const graphqlHTTP = require('express-graphql')
const schema = require( './graphql/schema')
const routes = require('./router/routes')

app.use(bodyParser.urlencoded({extended: false}));
app.use(bodyParser.json());

const port = process.env.PORT || 3000;
const connectionString = 'mongodb://localhost/logs'

//Connects to local mongodb instance using defined connection string
mongoose.connect(connectionString, {useNewUrlParser: true}).then(()=> {
    console.log('Connected to database: ', connectionString);
},err => {
    console.log('Error connecting to db');
});

//Setsup the graphql route using the rootSchema
app.use('/graphql', graphqlHTTP({
    schema,
    graphiql: true
}));

app.use('/', routes)

app.listen(port, () => { //Server starts up on defined port
    console.log('Logging server started on port ', port);
})