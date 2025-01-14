import { Component } from '@angular/core';
import {UserService} from '../../../public/services/user.service';
import { NavigationComponent } from '../../../admin/components/navigation/navigation.component';
import { OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subject,takeUntil } from 'rxjs';
import {CommonModule} from '@angular/common';
import { Router } from '@angular/router';
@Component({
  selector: 'app-display-users',
  standalone: true,
  imports: [NavigationComponent, CommonModule],
  templateUrl: './display-users.component.html',
  styleUrl: './display-users.component.css'
})
export class DisplayUsersComponent implements OnInit, OnDestroy {
  users$! : Observable<any[]>;
  private ngOnDestroy$ = new Subject<void>();
  currentUrl : string = '';
  fullUrl:boolean = false;
  searchRequest: any;
  constructor(private userService: UserService,
     private route: ActivatedRoute,
     private router: Router) {}


  ngOnInit(): void {
    this.checkUrl();
    if(this.fullUrl && this.currentUrl.includes('users')) this.loadUsers();   
    if(!this.fullUrl && this.currentUrl.includes('photos')) this.checkQueryParams(); 
  }

  ngOnDestroy(): void {
    this.ngOnDestroy$.next();
    this.ngOnDestroy$.complete();
  }

  checkQueryParams() {
    console.log("poziv");
    this.route.queryParams.pipe(takeUntil(this.ngOnDestroy$)).subscribe(params => {
      this.searchRequest = {  
        query: params['q'] || null,
      };
      this.loadSearchUsers();
    });
  }
  
  loadSearchUsers() {
    this.users$ = this.userService.getAllUsers(this.searchRequest).pipe(takeUntil(this.ngOnDestroy$));
    console.log(this.users$); // Debugging; ukloniti u produkciji
  }

  checkUrl(): void {
    this.route.url.pipe(takeUntil(this.ngOnDestroy$)).subscribe((segment) => {  
      this.currentUrl = segment.join('/');
    })
    this.fullUrl = this.router.url.includes('admin');
  }

  loadUsers(): void {
    if (!this.searchRequest || !this.searchRequest.query) {
      console.warn('searchRequest nije pravilno inicijaliziran.');
      this.searchRequest = { query: null }; // Postavite default vrijednosti
    }
  
    this.users$ = this.userService.getAllUsers(this.searchRequest).pipe(takeUntil(this.ngOnDestroy$));
  }

  goToProfilePage(user : any){
    if(this.fullUrl) this.router.navigate([`admin/user/${user.username}`]);
    else this.router.navigate([`public/profile/user/${user.username}`]);
  }

  goToUserPhotos(user : any){
    if(this.fullUrl) this.router.navigate([`admin/user/${user.username}/gallery`]);
  }

}
