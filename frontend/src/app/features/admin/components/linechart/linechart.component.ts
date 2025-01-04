import { Component, OnInit, Input, SimpleChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import Chart from 'chart.js/auto';
import {map} from 'rxjs/operators';
import { OnChanges } from '@angular/core';
@Component({
  selector: 'app-linechart',
  standalone: true,
  imports: [],
  templateUrl: './linechart.component.html',
  styleUrls: ['./linechart.component.css']
})
export class LinechartComponent implements OnInit, OnChanges  {
  @Input() chartBarData: any; 
  @Input() chartLineData :any; 
  public chartBar: any;
  public chartLine: any
  ngOnInit() {
    this.createBarChart();
    this.createLineChart();
  }

  ngOnChanges(changes: SimpleChanges) {
    if ((changes['chartBarData'] && changes['chartBarData'].currentValue) &&
        (changes['chartLineData'] && changes['chartLineData'].currentValue)) {
      if (this.chartBar && this.chartLine) {
        this.chartBar.destroy(); 
        this.chartLine.destroy();
      }
      this.createBarChart();
      this.createLineChart();
    }
  }

  generateLast7Dates() {
    const today = new Date();
    const dates = [];

    for(let i=0; i<7; i++) {
      const date = new Date(today);
      date.setDate(today.getDate()-i);
      const formattedDate = date.toLocaleDateString("bs-BA");
      dates.push(formattedDate);
    }
    return dates.reverse();
  }
  
  

  createBarChart() {

    this.chartBar = new Chart("BarChart", {
      type: 'bar',
      data: {
        labels: this.generateLast7Dates(),
        datasets: [
          {
            label: "Number of images per day",
            data: this.chartBarData, 
            backgroundColor: '#02A388',
          },
        ]
      },
      options: {
        responsive: true, 
        maintainAspectRatio: true,
        aspectRatio: 2,
        scales: {
          y: {
            beginAtZero: true, 
            ticks: {
              stepSize: 1,
              callback: function(value: any) {
                return value; // Prikazivanje celih brojeva
              }
            }
          }
        },
        plugins: {
          title: {
            display: true,
            text: 'Number of added images for the last 7 days',
            font: {
              size: 18,
              weight: 'bold',
              family: 'Poppins, sans-serif'
            },
            padding: {
              top: 20,
              bottom: 30 
            }
          }
        }
      }
    });
  }

  createLineChart() {

    this.chartLine = new Chart("LineChart", {
      type: 'line',
      data: {
        labels: this.generateLast7Dates(),
        datasets: [
          {
            label: "Number of users per day",
            data: this.chartLineData, 
            backgroundColor: '#02A388',
          },
        ]
      },
      options: {
        responsive: true, 
        maintainAspectRatio: true,
        aspectRatio: 2,
        scales: {
          y: {
            beginAtZero: true, 
            ticks: {
              stepSize: 1,
              callback: function(value: any) {
                return value; // Prikazivanje celih brojeva
              }
            }
          }
        },
        plugins: {
          title: {
            display: true,
            text: 'Number of registered users for the last 7 days',
            font: {
              size: 18,
              weight: 'bold',
              family: 'Poppins, sans-serif'
            },
            padding: {
              top: 20,
              bottom: 30 
            }
          }
        }
      }
    });
  }
  

  


}
