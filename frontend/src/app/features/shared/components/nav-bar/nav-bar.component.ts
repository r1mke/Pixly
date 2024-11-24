import { AfterViewInit, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { WindowSizeService } from '../../../../services/window-size.service';
import { Renderer2 } from '@angular/core';
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

  user: any = null; // Čuva podatke o korisniku

  constructor(
    private windowSizeService: WindowSizeService,
    private router: Router,
    private renderer: Renderer2,
    private authService: AuthService
  ) {
    this.windowSizeService.data$.subscribe((w) => {
      this.windowWidth = w;
      this.menuOpen = this.windowWidth >= 900;
      console.log(this.menuOpen, this.windowWidth);
    });
  }

  ngOnInit(): void {
    this.windowSizeService.data$.subscribe((w) => {
      this.windowWidth = w;
      this.menuOpen = this.windowWidth >= 900;
      console.log(this.menuOpen, this.windowWidth);
    });

    this.authService.verifyJwtToken().subscribe({
      next: (response) => {
        if (response.isValid)
          this.fetchCurrentUser();
        else 
          this.user = null;
      },
      error: () => {
        this.user = null;
      }
    });
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
      next: (user) => {
        this.user = user;
      },
      error: () => {
        this.user = null;
      }
    });
  }

  public logout() {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/login']);
        this.user = null;
      },
      error: (err) => {
        console.error('Logout error:', err);
      }
    });
  }
  
}
