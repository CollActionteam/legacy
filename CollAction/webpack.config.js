const webpack = require('webpack');

function getPlugins() {
  const productionPlugins = [
    new webpack.optimize.UglifyJsPlugin({
      minimize: true,
    }),
  ];

  const compatibilityPlugins = [
    new webpack.ProvidePlugin({
      'Promise': 'bluebird', // Provide a Promise polyfill for older or less suportive browsers like IE9-11.
    }),
  ];

  return compatibilityPlugins.concat(process.env.NODE_ENV === "production" ? productionPlugins : []);
}

module.exports = {
  entry: [ 'whatwg-fetch', './Frontend/app/index.ts' ],
  output: {
    path: './wwwroot/js',
    filename: 'bundle.js',
  },
  resolve: {
    extensions: ['', '.webpack.js', '.web.js', '.ts', '.tsx', '.js']
  },
  module: {
    loaders: [
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
