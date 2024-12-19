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

  constructor(private userService: UserService,
     private route: ActivatedRoute,
     private router: Router) {}


  ngOnInit(): void {
    this.checkUrl();
    this.loadUsers();
  }

  ngOnDestroy(): void {
    this.ngOnDestroy$.next();
    this.ngOnDestroy$.complete();
  }


  checkUrl(): void {
    this.route.url.pipe(takeUntil(this.ngOnDestroy$)).subscribe((segment) => {  
      this.currentUrl = segment.join('/');
    })
  }

  loadUsers(): void {
    this.users$ = this.userService.getAllUsers().pipe(takeUntil(this.ngOnDestroy$));
    console.log(this.users$);
  }

  goToProfilePage(user : any){
    console.log(user);
    this.router.navigate([`admin/user/${user.username}`]);
  }
}
