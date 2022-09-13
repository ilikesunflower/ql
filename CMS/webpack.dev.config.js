"use strict";

const fs = require('fs');
const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");


// Custom variables
const applicationBasePath = "./ReactApp";
const getAllFiles = function (dirPath, arrayOfFiles) {
    let files = fs.readdirSync(dirPath)
    arrayOfFiles = arrayOfFiles || {}
    if (files.length > 0) {
        files.forEach(function (file) {
            if (fs.statSync(dirPath + "/" + file).isDirectory()) {
                arrayOfFiles = getAllFiles(dirPath + "/" + file, arrayOfFiles)
            } else {
                if (file.toLowerCase() === "app.ts" || file.toLowerCase() === "app.js" || file.toLowerCase() === "app.jsx") {
                    let name = dirPath.replace(applicationBasePath, "");
                    arrayOfFiles[name] = dirPath + "/" + file;
                }
            }
        })
    }
    return arrayOfFiles
}

let appEntryFiles = getAllFiles(applicationBasePath);

module.exports = function () {

    const mode = 'development';

    return {
        mode,
        entry: appEntryFiles,
        output: {
            path: path.resolve(__dirname, "wwwroot/dist"),
            filename: "js/[name]/bundle.js",
            chunkFilename: "js/[name]/bundle.js?v=[chunkhash]",
            publicPath: "/dist/"
        },
        resolve: {
            extensions: [".ts", ".js", ".jsx", ".json", "scss", "css"],
            modules: [path.join(__dirname, './node_modules')]
        },
        module: {
            rules: [/* config.module.rule('js') */
                {
                    test: /\.js$|jsx/, exclude: /(node_modules|bower_components)/, loader: "babel-loader",

                }, /* config.module.rule('ts') */
                {
                    test: /\.ts$/, loader: "ts-loader", options: {
                        transpileOnly: true
                    }
                }, /* config.module.rule('sass') */
                {
                    test: /\.scss$/,
                    use: [{
                        loader: MiniCssExtractPlugin.loader,
                        options: {
                            publicPath: '/dist/',
                        }
                    }, 'css-loader', 'postcss-loader', 'sass-loader']
                },
                {
                    test: /\.css$/i,
                    use: [
                        'style-loader',
                        'css-loader',
                        'postcss-loader'
                    ],
                },
                {
                    test: /\.(png|jpe?g|gif|webp)(\?.*)?$/, use: [{
                        loader: 'url-loader', options: {
                            limit: 4096, fallback: {
                                loader: 'file-loader', options: {
                                    name: 'img/[name].[hash:8].[ext]'
                                }
                            }
                        }
                    }]
                }, /* config.module.rule('svg') */
                {
                    test: /\.(svg)(\?.*)?$/, use: [{
                        loader: 'file-loader', options: {
                            name: 'img/[name].[hash:8].[ext]'
                        }
                    }]
                }]
        },
        performance: {
            hints: false
        },
        devServer: {
            compress: false,
            hot: true,
            inline: true,
            noInfo: true,
            overlay: true,
            contentBase: path.join(__dirname, '/dist'),
            watchContentBase: false,
            watchOptions: {
                aggregateTimeout: 1000,
                poll: 300,
                ignored: '**/node_modules',
            }
        },
        devtool: (mode === 'development') ? 'source-map' : false,
        plugins: [
            new webpack.ProvidePlugin({
                Promise: "es6-promise-promise",
            }),
            new MiniCssExtractPlugin()
        ]
    };
};
