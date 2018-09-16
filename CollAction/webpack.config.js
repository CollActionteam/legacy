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

module.exports = {
  entry: [
    'font-awesome/scss/font-awesome.scss',
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
      },
      { 
        test: /\.woff(2)?(\?v=[0-9]\.[0-9]\.[0-9])?$/, 
        loader: "url-loader?limit=10000&mimetype=application/font-woff" 
      },
      { 
        test: /\.(ttf|eot|svg)(\?v=[0-9]\.[0-9]\.[0-9])?$/, 
        use: [{
          loader: 'file-loader',
          options: {
              name: '[name].[ext]',
              outputPath: 'fonts/'
          }
        }]
      }
    ]
  },
  plugins: getPlugins()
}
