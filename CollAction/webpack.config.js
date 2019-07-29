const webpack = require("webpack");
const path = require("path");

module.exports = {
  mode: process.env.NODE_ENV === "production" ? "production" : "development",
  entry: {
    frontend: "./Frontend/app/index.ts",
    validation: "./Frontend/app/validation.ts",
    tagmanager: "./Frontend/app/tagmanager.ts"
  },
  output: {
    filename: "[name].bundle.js",
    path: path.resolve(__dirname, "./wwwroot/js")
  },
  resolve: {
    extensions: [".webpack.js", ".web.js", ".ts", ".tsx", ".js"]
  },
  module: {
    rules: [
      { test: /\.tsx?$/, use: "ts-loader" },
      {
        test: /\.scss$/,
        use: ["style-loader", "css-loader?-url", "sass-loader"]
      },
      { test: /\.css$/, use: ["style-loader", "css-loader?-url"] },
      { test: /\.svg$/, use: "svg-inline-loader" }
    ]
  },
  plugins: [
    new webpack.ProvidePlugin({
      Promise: "bluebird", // Provide a Promise polyfill for older or less suportive browsers like IE9-11.
      URLSearchParams: "@ungap/url-search-params/cjs" // URLSearchParams polyfill
    })
  ]
};
