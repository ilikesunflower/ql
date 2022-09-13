import {Button, Col, Row} from "react-bootstrap";
import React from "react";
import Highcharts from 'highcharts'
import HighchartsReact from 'highcharts-react-official'
import highcharts3d from "highcharts/highcharts-3d";
import { useElementSize } from 'use-element-size'

highcharts3d(Highcharts);
Highcharts.setOptions({
    lang: {
        thousandsSep: '.',
        numericSymbols: [ 'K' , 'M' , 'B' , 'T' , 'P' , 'E']
    }
});
function Chart(props) {
    let {chartData} = props
    let {listGroup1,  listGroup2} = chartData
    
    const optionsGroup = {
        chart: {
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            }
        },
        title: {
            text: 'Doanh số bán ra theo khu vực'
        },
        colors: ['#F15C80','#7CB5EC', '#90ED7D','#F7A35C','#5B9BD5'],
        tooltip: {
            useHTML: true,
            formatter() {
                return `${this.point.options.name}: <b>${Highcharts.numberFormat(this.point.options.y, 0, ",")} VND</b>`;
            },
            // pointFormat: '{series.name}: <b>{this.point.total:.1f}%</b>'
        },
        accessibility: {
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b><br>{point.percentage:.1f} %',
                },
                depth: 35,
                // colors: ['#997300', '#E82C0C', '#FF0000', '#E80C7A', '#E80C7A']
            }
        },
        series: [{
            name: 'Sản lượng',
            colorByPoint: true,
            data: listGroup1 || []
        }]
    }

    const optionsCompany = {
        chart: {
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            }
        },
        title: {
            text: 'Sản lượng bán ra theo khu vực'
        },
        colors: ['#F15C80','#7CB5EC', '#90ED7D','#F7A35C','#5B9BD5'],
        tooltip: {
            useHTML: true,
            formatter() {
                return `${this.point.options.name}: <b>${Highcharts.numberFormat(this.point.options.y, 0, ",")} pcs</b>`;
            },
            // pointFormat: '{series.name}: <b>{this.point.total:.1f}%</b>'
        },
        accessibility: {
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                depth: 35,
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b><br>{point.percentage:.1f} %',
                }
            }
        },
        series: [{
            name: 'Doanh số',
            colorByPoint: true,
            data: listGroup2 || []
        }]
    }
    
    const ref = useElementSize((size, prevSize, elem) => {
        if (prevSize != null && size != null && prevSize.width != size.width){
            Highcharts.charts.forEach(d => {
                if (d != undefined)
                    d.reflow();
            });
        }
    });
    return (
        <div className="col-12" ref={ref}>
            <Row  >
                <Col  md={6}>
                    <HighchartsReact highcharts={Highcharts} options={optionsCompany}/>
                </Col>
                <Col  md={6}>
                    <HighchartsReact highcharts={Highcharts} options={optionsGroup}/>
                </Col>
            </Row>
        </div>
   
    );
}
export default Chart;