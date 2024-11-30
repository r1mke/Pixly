import { AfterViewInit, Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { WindowSizeService } from '../../../../services/window-size.service';
import { AuthService } from '../../../auth/services/auth.service';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})

export class NavBarComponent implements OnInit, AfterViewInit {
  isScrolled: boolean = false;
  windowWidth: number = 0;
  menuOpen: boolean = false;
  exploreHovered = false;
  profileHovered = false;
  dotsHovered = false;
  user: any = null;
  username! : string;
  @Output() hoverStateChange = new EventEmitter<{ key: string, state: boolean }>();

  constructor(private windowSizeService: WindowSizeService, private router: Router, private authService: AuthService) {
    this.windowSizeService.data$.subscribe((w) => {
      this.windowWidth = w;
      this.menuOpen = this.windowWidth >= 900;
    });
  }

  ngOnInit(): void {
    this.windowSizeService.data$.subscribe((w) => {
      this.windowWidth = w;
      this.menuOpen = this.windowWidth >= 900;
    });

    this.fetchCurrentUser(); // Direktan poziv na dohvaćanje korisnika
  }

  ngAfterViewInit() {}

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  goToHome() {
    this.router.navigate(['/']);
  }

  private fetchCurrentUser() {
    this.authService.getCurrentUser().subscribe({
      next: (res) => {
        if (res) {
          this.user = res.user;
          //console.log('User after fetch:', this.user);
        } else {
          //console.error('No user found');
          this.user = null;
        }
      },
      error: () => {
        //console.error('Error fetching user');
        this.user = null;
      }
    });
  }

  public logout() {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
        this.user = null;
      },
      error: (err) => {
        console.error('Logout error:', err);
      }
    });
  }

  public goToProfile(): void {
    if (this.user && this.user.username) {
      this.router.navigate([`/public/profile/user/${this.user.username}`]);
    } else {
      console.error('User or username is not available');
    }
  }


  emitHoverStateChange(key: string, state: boolean) {
    this.hoverStateChange.emit({ key, state });

    if (key === 'explore') {
      this.exploreHovered = state;
    } else if (key === 'profile') {
      this.profileHovered = state;
    }
    else if(key === 'dots')
      this.dotsHovered = state;
  }
}