"use strict";

const fs = require('fs');
const path = require("path");
const webpack = require("webpack");
const RemovePlugin = require('remove-files-webpack-plugin');
const CompressionPlugin = require('compression-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const {CleanWebpackPlugin} = require('clean-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");

// Custom variables
let isProduction = false;
const applicationBasePath = "./ReactApp";


const getAllFiles = function (dirPath, arrayOfFiles) {
    let files = fs.readdirSync(dirPath)
    arrayOfFiles = arrayOfFiles || {}
    if ( files.length > 0 ) {
        files.forEach(function (file) {
            if ( fs.statSync(dirPath + "/" + file).isDirectory() ) {
                arrayOfFiles = getAllFiles(dirPath + "/" + file, arrayOfFiles)
            } else {
                if ( file.toLowerCase() === "app.ts" || file.toLowerCase() === "app.js" || file.toLowerCase() === "app.jsx" ) {
                    let name = dirPath.replace(applicationBasePath, "");
                    arrayOfFiles[name] = dirPath + "/" + file;
                }
            }
        })
    }
    return arrayOfFiles
}

let appEntryFiles = getAllFiles(applicationBasePath);
module.exports = function (env, argv) {

    const mode = argv.mode || 'development'; // dev mode by default


    return {
        mode,
        entry: appEntryFiles,
        stats: {
            entrypoints: false,
            children: false
        },
        output: {
            path: path.resolve(__dirname, "wwwroot/dist"),
            filename: "js/[name]/bundle.js",
            chunkFilename: "js/[name]/bundle.js?v=[chunkhash]",
            publicPath: "/dist/"
        },
        resolve: {
            extensions: [".ts", ".js", ".jsx", ".json", "scss", "css"],
            modules: [
                path.join(__dirname, './node_modules')
            ]
        },
        devtool: 'source-map',
        devServer: {
            compress: true,
            hot: true,
            inline: true,
            noInfo: true,
            overlay: true,
            contentBase: path.join(__dirname, '/dist'),
            watchContentBase: true,
            watchOptions: {
                poll: true
            }
        },
        module: {
            rules: [
                /* config.module.rule('js') */
                {
                    test: /\.js$|jsx/,
                    exclude: /(node_modules|bower_components)/,
                    loader: "babel-loader"
                },
                /* config.module.rule('ts') */
                // {
                //     test: /\.ts$/,
                //     loader: "ts-loader",
                //     options: {
                //         transpileOnly: true
                //     }
                // }, 
                /* config.module.rule('sass') */
                {
                    test: /\.scss$/,
                    use: [
                        {
                            loader: MiniCssExtractPlugin.loader,
                            options: {
                                publicPath: '/dist/',
                                // only enable hot in development
                                hmr: process.env.NODE_ENV === 'development',
                                // if hmr does not work, this is a forceful method.
                                reloadAll: true
                            }
                        },
                        'css-loader',
                        'postcss-loader',
                        'sass-loader'
                    ]
                },
                /* config.module.rule('css') */
                {
                    test: /\.css$/i,
                    use: [
                        'style-loader',
                        'css-loader',
                        'postcss-loader'
                    ]
                },
                /* config.module.rule('images') */
                {
                    test: /\.(png|jpe?g|gif|webp)(\?.*)?$/,
                    use: [
                        {
                            loader: 'url-loader',
                            options: {
                                limit: 4096,
                                fallback: {
                                    loader: 'file-loader',
                                    options: {
                                        name: 'img/[name].[hash:8].[ext]'
                                    }
                                }
                            }
                        }
                    ]
                },
                /* config.module.rule('svg') */
                {
                    test: /\.(svg)(\?.*)?$/,
                    use: [
                        {
                            loader: 'file-loader',
                            options: {
                                name: 'img/[name].[hash:8].[ext]'
                            }
                        }
                    ]
                },
                /* config.module.rule('media') */
                {
                    test: /\.(mp4|webm|ogg|mp3|wav|flac|aac)(\?.*)?$/,
                    use: [
                        {
                            loader: 'url-loader',
                            options: {
                                limit: 4096,
                                fallback: {
                                    loader: 'file-loader',
                                    options: {
                                        name: 'media/[name].[hash:8].[ext]'
                                    }
                                }
                            }
                        }
                    ]
                },
                /* config.module.rule('fonts') */
                {
                    test: /\.(woff2?|eot|ttf|otf)(\?.*)?$/i,
                    use: [
                        {
                            loader: 'url-loader',
                            options: {
                                limit: 4096,
                                fallback: {
                                    loader: 'file-loader',
                                    options: {
                                        name: 'fonts/[name].[hash:8].[ext]'
                                    }
                                }
                            }
                        }
                    ]
                }
            ]
        },
        performance: {
            hints: false
        },
        // optimization: {
        //     minimize: true,
        //     minimizer: [new TerserPlugin({
        //         minify: TerserPlugin.uglifyJsMinify,
        //         terserOptions: {
        //         },
        //         parallel: true,
        //     })],
        // },
        plugins: [
            new MiniCssExtractPlugin({
                filename: "css/[name]/main.css"
            }),
            new webpack.DefinePlugin({
                'process.env.NODE_ENV': JSON.stringify(mode)
            }),
            new webpack.ProvidePlugin({
                Promise: "es6-promise-promise",
            }),
            new CompressionPlugin({
                test: /\.js$|\.jsx$|\.css$|\.html$/,
                filename: "[path][base].gz[query]",
                algorithm: "gzip"
            }),
            new RemovePlugin({
                before: {
                    include: [
                        './wwwroot/dist'
                    ]
                }
            }),
            new CleanWebpackPlugin(),
        ]
    };
};