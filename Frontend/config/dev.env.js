'use strict'
const merge = require('webpack-merge')
const prodEnv = require('./prod.env')
const api_url = '"http://localhost:58896"'

module.exports = merge(prodEnv, {
  NODE_ENV: '"development"',
  API_URL: api_url
})
