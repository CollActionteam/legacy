const webpack = require('webpack');

function getPlugins() {
  const productionPlugins = [
    new webpack.optimize.UglifyJsPlugin({
      minimize: true,
    }),
  ];
  return process.env.NODE_ENV === "production" ? productionPlugins : [];
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
