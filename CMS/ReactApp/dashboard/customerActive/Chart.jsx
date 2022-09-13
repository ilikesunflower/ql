import {Button, Col} from "react-bootstrap";
import React from "react";
import Highcharts from 'highcharts'
import HighchartsReact from 'highcharts-react-official'
import {useElementSize} from 'use-element-size'
import highcharts3d from "highcharts/highcharts-3d";
highcharts3d(Highcharts);
Highcharts.setOptions({
    lang: {
        thousandsSep: '.',
        numericSymbols: ['K', 'M', 'B', 'T', 'P', 'E']
    }
});
const useChart = (props) => {
    let {chartData} = props;
    const options = (data) => ({
        chart: {
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            }
        },
        title: {
            text: ''
        },
        subtitle: {
            text: ''
        },
        accessibility: {
            point: {
                valueSuffix: '%'
            }
        },
        tooltip: {
            pointFormat: '{series.name}: {point.y}</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                depth: 35,
                dataLabels: {
                    enabled: true,
                    format: '{point.name} {point.percentage:.1f}%'
                }
            }
        },
        series: [{
            type: 'pie',
            name: 'Khách hàng',
            data: data
        }]
    });
    const ref = useElementSize((size, prevSize, elem) => {
        if (prevSize != null && size != null && prevSize.width != size.width) {
            Highcharts.charts.forEach(d => {
                if (d != undefined)
                    d.reflow();
            });
        }
    });

    return {
        ref,
        chartData, 
        method: {
            options,
        }
    }
}

function Chart(props) {
    let {ref, chartData, method} = useChart(props)
    return (
        <Col md={12} ref={ref}>
            <HighchartsReact
                highcharts={Highcharts}
                options={method.options(chartData)}
            />
        </Col>
    );
}

export default Chart;