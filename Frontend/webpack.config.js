module.exports = {
  entry: './app/index.ts',
  output: {
    path: '../Backend/wwwroot/js',
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
        loaders: ["style-loader", "css-loader", "sass-loader"],
      },
      {
        test: /\.css$/,
        loaders: ["style-loader", "css-loader"],
      }
    ]
  }
}
