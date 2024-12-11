import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { WindowSizeService } from '../../../../services/window-size.service';
import { AuthService } from '../../../auth/services/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css'],
})

export class NavBarComponent implements OnInit {
  isScrolled: boolean = false;
  windowWidth: number = 0;
  menuOpen: boolean = false;
  exploreHovered = false;
  profileHovered = false;
  dotsHovered = false;
  user: any = null;
  currentSearch: string = '';
  currentUrl: string = '';
  @Output() hoverStateChange = new EventEmitter<{ key: string; state: boolean }>();

  constructor(
    private windowSizeService: WindowSizeService,
    private router: Router,
    private authService: AuthService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.checkUrl();
    this.getCurrentUser();
  }

  checkUrl(): void {
      this.route.url.subscribe((segment) => {
        this.currentUrl = segment.join('/');
        if(this.currentUrl.includes('search')){
          this.route.queryParams.subscribe(params => {
            this.currentSearch = params['q'];
          })
        }
      })
  }

  goToSearchPage(): void {
    if(this.currentSearch === '') return;
    this.router.navigate(['/public/search'], { queryParams: { q: this.currentSearch } });
  }

  getCurrentUser(): void {

    this.authService.currentUser$.subscribe((user) => {
      this.user = user;
    });
 
    if (!this.user) {
      this.authService.getCurrentUser().subscribe({
        error: () => {
          console.error('Error fetching user');
        },
      });
    }

  }


  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  goToHome() {
    this.router.navigate(['/']);
  }

  public logout() {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
        this.user = null;
      },
      error: (err) => {
        console.error('Logout error:', err);
      },
    });
  }

  public goToProfile(): void {
    if (this.user && this.user.username)
      this.router.navigate([`/public/profile/user/${this.user.username}`]);
    else
      console.error('User or username is not available');
  }

  emitHoverStateChange(key: string, state: boolean) {
    this.hoverStateChange.emit({ key, state });
    if (key === 'explore')
      this.exploreHovered = state;
    else if (key === 'profile') 
      this.profileHovered = state;
    else if (key === 'dots') 
      this.dotsHovered = state;
  }
}
