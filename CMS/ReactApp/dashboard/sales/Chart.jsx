﻿import {Button, Col} from "react-bootstrap";
import React from "react";
import Highcharts from 'highcharts'
import HighchartsReact from 'highcharts-react-official'
import { useElementSize } from 'use-element-size'

const useChart = (props) => {
    let {chartData} = props;
    let {categories, prices} = chartData;
    let test = [];
    for(let i = 0; i < 30; i++){
        test.push(i * 4000000 );
    }
    Highcharts.setOptions({
        lang: {
            thousandsSep: '.',
            numericSymbols: [ 'K' , 'M' , 'B' , 'T' , 'P' , 'E']
        }
    });
    const options = (_categories, _price) => ({
        chart: {
            zoomType: 'xy',
            scrollablePlotArea: {
                minWidth: 635,
                scrollPositionX: 1
            },
        },
        title: {
            text: ''
        },
        subtitle: {
            text: ''
        },
        xAxis: [{
            categories: _categories,
            crosshair: true,
        }],
        yAxis: [{
            title: {
                text: 'Đơn vị VND',
                style: {
                    color: "#2E5ADD"
                }
            },
            labels: {
                style: {
                    color: "#2E5ADD"
                }
            }
        },
            {
                title: {
                    text: 'Số đơn',
                    style: {
                        color: "#2eb85c"
                    }
                },
                labels: {
                    style: {
                        color: "#2eb85c"
                    }
                },
                opposite: true
            },
        ],
        tooltip: {
            formatter: function () {
                return this.points.reduce(function (s, point) {
                    return s + '<br/>' + point.series.name + ': ' +
                        Highcharts.numberFormat(point.y, 0, ',', '.') + point.series.userOptions.tooltip.valueSuffix;
                }, '<b>' + this.x + '</b>');
            },
            shared: true
        },
        plotOptions: {
            series: {
                events: {
                    legendItemClick: function() {
                        return false;
                    }
                }
            },
            column: {
                maxPointWidth: 50
            }
        },
        legend: {
            align: 'left',
            x: 80,
            verticalAlign: 'top',
            y: 80,
            floating: true,
            backgroundColor:
                Highcharts.defaultOptions.legend.backgroundColor || // theme
                'rgba(255,255,255,0.25)'
        },

        series: [{
            name:"Doanh số",
            showInLegend: false,
            type: 'column',
            yAxis: 0,
            data: _price,
            color: "#2E5ADD",
            tooltip: {
                valueSuffix: ' VND'
            }
        },
            {
                name: 'Số đơn',
                type: 'spline',
                data: chartData?.lineData ?? [],
                showInLegend: false,
                yAxis: 1,
                color: "#2eb85c",
                tooltip: {
                    valueSuffix: ' '
                }
            }]
    });

    const ref = useElementSize((size, prevSize, elem) => {
        if (prevSize != null && size != null && prevSize.width != size.width){
            Highcharts.charts.forEach(d => {
                if (d != undefined)
                    d.reflow();
            });
        }
    });

    return {
        ref,
        state: {
            categories, prices
        }, method: {
            options,
        }
    }
}

function Chart(props) {
    let {ref,state, method} = useChart(props)
    let {categories,  prices} = state;
    return (
            <Col md={12}  ref={ref}>
                    <HighchartsReact
                        highcharts={Highcharts}
                        options={method.options(categories, prices)}
                    />  
            </Col>
       
        );
}

export default Chart;