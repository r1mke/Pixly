import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../../auth/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
})
export class ProfileComponent implements OnInit {
  currentUser: any = null;
  profileUser: any = null;
  isOwnProfile: boolean = false;
  username: string | null = null;

  navItems = [
    { label: 'Gallery', count: 0, active: true },
    { label: 'Collections', count: null, active: false },
    { label: 'Statistics', count: null, active: false },
    { label: 'AI', count: null, active: false },
  ];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.username = params.get('username');
      if (this.username) this.getUserProfile();
    });
  
    this.authService.getCurrentUser().subscribe({
      next: (res) => {
        this.currentUser = res.user;
        if (this.profileUser) this.checkIfOwnProfile();
      },
      error: () => {},
    });
  }
  
  
  getUserProfile(): void {
    if (this.username) {
      this.userService.getUserByUsername(this.username).subscribe({
        next: (data) => {
          this.profileUser = data;
          if (this.currentUser) this.checkIfOwnProfile();
        },
        error: (error) => {
          //console.error('Error fetching user profile:', error);
        },
      });
    }
  }
  
  private checkIfOwnProfile(): void {
    if (this.currentUser && this.profileUser) {
      console.log('Current User:', this.currentUser);
      console.log('Profile User:', this.profileUser);
      this.isOwnProfile = this.currentUser.username === this.profileUser.username;
    }
  }
  

  public setActive(item: any, event: Event): void {
    event.preventDefault();
    this.navItems.forEach((nav) => (nav.active = false));
    item.active = true;
  }
}
