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
    "font-awesome/scss/font-awesome.scss",
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
      },
      {
          test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
          use: [{
              loader: 'file-loader',
              options: {
                name: 'fonts/[name].[ext]',
                mimetype: 'application/font-woff',
                publicPath: '../'
              }
          }]
      }
    ]
  },
  plugins: getPlugins()
}
