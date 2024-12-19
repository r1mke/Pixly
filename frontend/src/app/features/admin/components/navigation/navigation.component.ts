import { Component } from '@angular/core';
import { Router } from '@angular/router';
@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css'
})
export class NavigationComponent {

  constructor(private router: Router){}


  goToNewPosts(){
    this.router.navigate(['/admin/new-posts']);
  } 
  goToDashboard(){
    this.router.navigate(['/admin/dashboard']);
  }
  goToUsers(){
    this.router.navigate(['/admin/users']);
  }
}
