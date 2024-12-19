import { Component } from '@angular/core';
import  { CommonModule } from '@angular/common';
import { OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OverviewComponent } from "../../components/overview/overview.component";
import { NavigationComponent } from "../../components/navigation/navigation.component";
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, OverviewComponent, NavigationComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit{

  constructor(private router: Router){}
  
  ngOnInit(): void {
    
  }

  goToNewPosts(){
    this.router.navigate(['/admin/new-posts']);
  } 

}
