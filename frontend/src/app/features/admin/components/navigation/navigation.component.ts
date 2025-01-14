import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import {AuthService} from '../../../auth/services/auth.service';
import { OnInit } from '@angular/core';
import { HostListener } from '@angular/core';
@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css',
})
export class NavigationComponent implements OnInit {
  currentSearch = '';
  isInputFocused = false;
  Suggestions$!: Observable<any>
  isSearchOpen = false
  user: any;
  isScreenTooSmall: boolean = false;
  constructor(private authService: AuthService, private router: Router){}

  ngOnInit(): void {
    this.getCurrentUser();
    this.checkScreenSize();
  }

  getCurrentUser(): void {
    this.authService.currentUser$.subscribe((user) => {
      this.user = user;
      console.log(this.user);
    });
  }

  goToNewPosts(){
    this.router.navigate(['/admin/new-posts']);
  } 
  goToDashboard(){
    this.router.navigate(['/admin/dashboard']);
  }
  goToUsers(){
    this.router.navigate(['/admin/users']);
  }

  @HostListener('window:resize', [])
  onResize() {
    this.checkScreenSize();
  }

  checkScreenSize() {
    this.isScreenTooSmall = window.innerWidth < 1000;
    if(this.isScreenTooSmall){
      this.router.navigate(['admin/error']);
    }
  }



  updateSearchWithoutValue(event: Event) {

  }

  onFocus(){}

  onKeyDown(event: Event){}

  goToSearchPage(){}

  updateSearch(value: any){}

  goToProfilePage(value: any){}

  toggleSearch(){
    this.isSearchOpen = !this.isSearchOpen
  }
}
