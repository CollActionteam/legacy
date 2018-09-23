const webpack = require('webpack');

function getPlugins() {
  // const productionPlugins = [
  //   new webpack.optimize.UglifyJsPlugin({
  //     minimize: true,
  //   }),
  // ];

  const compatibilityPlugins = [
    new webpack.ProvidePlugin({
      'Promise': 'bluebird', // Provide a Promise polyfill for older or less suportive browsers like IE9-11.
    }),
  ];

  return compatibilityPlugins; //.concat(process.env.NODE_ENV === "production" ? productionPlugins : [])
}

function getMode() {
  return process.env.NODE_ENV === "production" ? "production" : "development"
}

module.exports = {
  mode: getMode(),
  entry: [
    'whatwg-fetch', 
    'quill', 
    './Frontend/app/index.ts' 
  ],
  output: {
    path: __dirname + '/wwwroot/js',
    filename: 'bundle.js',
  },
  resolve: {
    extensions: ['.webpack.js', '.web.js', '.ts', '.tsx', '.js']
  },
  module: {
    rules: [
      { test: /\.tsx?$/, loader: 'ts-loader' },
      {
        test: /\.scss$/,
        loaders: ["style-loader", "css-loader?-url", "sass-loader"],
      },
      {
        test: /\.css$/,
        loaders: ["style-loader", "css-loader?-url"],
      }
    ]
  },
  plugins: getPlugins()
}
