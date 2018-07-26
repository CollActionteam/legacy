const webpack = require('webpack');
const path = require('path')

module.exports = {
  mode: process.env.NODE_ENV === 'production' ? 'production' : 'development',
  entry: './Frontend/app/index.ts',
  output: {
    path: path.resolve(__dirname, './wwwroot/js'),
    filename: 'bundle.js',
  },
  resolve: {
    extensions: ['.webpack.js', '.web.js', '.ts', '.tsx', '.js']
  },
  module: {
    rules: [
      { test: /\.tsx?$/, use: 'ts-loader' },      
      { test: /\.scss$/, use: ["style-loader", "css-loader?-url", "sass-loader"] },
      { test: /\.css$/, use: ["style-loader", "css-loader?-url"] },
      { test: /\.svg$/, use: 'svg-inline-loader' }
    ]
  }
}
