import {Button, Col} from "react-bootstrap";
import React from "react";
import Highcharts from 'highcharts'
import HighchartsReact from 'highcharts-react-official'
import { useElementSize } from 'use-element-size'

const useChart = (props) => {
    let {chartData} = props;
    let {categories, prices, filterStatus, valueSuffix} = chartData;
    Highcharts.setOptions({
        lang: {
            thousandsSep: '.',
            numericSymbols: [ 'K' , 'M' , 'B' , 'T' , 'P' , 'E']
        }
    });
    const options = (_categories, _price, _filterStatus, _valueSuffix) => ({
        chart: {
            type: 'bar',
         
        },
        title: {
            text: ''
        },
        subtitle: {
            text: ''
        },
        xAxis: {
            categories: _categories,
            title: {
                text: null
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: '',
                align: 'high'
            },
            labels: {
                overflow: 'justify',
            },
          
        },
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
            bar: {
                dataLabels: {
                    enabled: true
                }
            }
        },
        // legend: {
        //     align: 'center',
        //     verticalAlign: 'bottom',
        //     x: 0,
        //     y: 0,
        //     floating: true,
        //     borderWidth: 1,
        //     backgroundColor:
        //         Highcharts.defaultOptions.legend.backgroundColor || '#FFFFFF',
        //     shadow: true
        // },
        credits: {
            enabled: false
        },
        series: [{
            name: _filterStatus,
            data: _price,
            dataLabels: {
                formatter:  function () {
                    return Highcharts.numberFormat(this.y, 0, ',', '.') ;
                }
            },
            color: "#2E5ADD",
            tooltip: {
                valueSuffix: _valueSuffix
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
            categories, prices,filterStatus, valueSuffix
        }, method: {
            options,
        }
    }
}

function Chart(props) {
    let {ref,state, method} = useChart(props)
    let {categories,  prices, filterStatus, valueSuffix} = state;
    return (
            <Col md={12}  ref={ref}>
                    <HighchartsReact
                        highcharts={Highcharts}
                        options={method.options(categories, prices, filterStatus, valueSuffix)}
                    />  
            </Col>
       
        );
}

export default Chart;