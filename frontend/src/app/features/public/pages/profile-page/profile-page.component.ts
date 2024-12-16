import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../../auth/services/auth.service';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { GalleryComponent } from "../../components/gallery/gallery.component";

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterLink, NavBarComponent, GalleryComponent],
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.css'],
})

export class ProfilePageComponent implements OnInit {
  currentUser: any = null;
  profileUser: any = null;
  isOwnProfile: boolean = false;
  username: string | null = null;

  navItems = [
    { label: 'Gallery', active: false },
    { label: 'Collections', active: false },
    { label: 'Liked', active: false },
    { label: 'AI', active: false },
  ];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.username = params.get('username');
      if (this.username) this.getUserProfile();
    });
    this.getCurrentUser();

    this.route.url.subscribe(urlSegments => {
      this.updateActiveTab(urlSegments);
    });
  }
  
  getCurrentUser(): void {
    this.authService.currentUser$.subscribe({
      next: (res) => {
        this.currentUser = res;
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
        error: (error) => {},
      });
    }
  }
  
  private checkIfOwnProfile(): void {
    if (this.currentUser && this.profileUser)
      this.isOwnProfile = this.currentUser.username === this.profileUser.username;
  }

  private updateActiveTab(urlSegments: any[]): void {
    const currentPath = urlSegments.map(segment => segment.path).join('/');
    this.navItems.forEach(nav => {
      nav.active = currentPath.includes(nav.label.toLowerCase());
    });
  }
  

  public setActive(item: any, event: Event): void {
    event.preventDefault();

    this.navItems.forEach((nav) => (nav.active = false));
    item.active = true;

    if (item.label === 'Liked') {
      this.router.navigate([`public/profile/user/${this.username}/liked`]);
    }

    if (item.label === 'Gallery') {
      this.router.navigate([`public/profile/user/${this.username}/gallery`]);
    }
  }
}

