import { Component } from '@angular/core';
import  { CommonModule } from '@angular/common';
import { OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { NavigationComponent } from "../../components/navigation/navigation.component";
import { LinechartComponent } from "../../components/linechart/linechart.component";
import { Observable, Subject } from 'rxjs';
import {PhotoEndpointsService} from '../../services/Endpoints/Photo/photo-endpoints.service'
import { takeUntil } from 'rxjs/operators';
import { OverviewComponent } from "../../components/overview/overview.component";
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, NavigationComponent, LinechartComponent, OverviewComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit, OnDestroy {
  PhotosData$! : Observable<any[]>;
  private ngOnDestroy$ = new Subject<void>();
  chartBarData: any;
  chartLineData: any;
  constructor(private router: Router, private DataService: PhotoEndpointsService){}
  
  ngOnInit(): void {
    this.getDataForLineChart();
  }

  ngOnDestroy(): void {
    this.ngOnDestroy$.next();
    this.ngOnDestroy$.complete();
  }

  goToNewPosts(){
    this.router.navigate(['/admin/new-posts']);
  } 

  getDataForLineChart() {
    this.DataService.getDataForPhotoLineChart()
      .pipe(
        takeUntil(this.ngOnDestroy$)
      )
      .subscribe(data => {
        console.log(data)
        this.chartBarData = data.photosPerDay;
        this.chartLineData = data.usersPerDay;
      }, error => {
        console.error("Greška pri preuzimanju podataka:", error);
      });
  }
  


}
