import { Component, EventEmitter, OnInit, Output, HostListener, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { PhotosSearchService } from '../../../public/services/Photos/photos-search.service';
import { BehaviorSubject, debounceTime, Observable, Subject } from 'rxjs';
import { catchError, switchMap,tap } from 'rxjs/operators';
import { of } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css'],
})

export class NavBarComponent implements OnInit,OnDestroy {
  isScrolled: boolean = false;
  windowWidth: number = 0;
  menuOpen: boolean = false;
  exploreHovered = false;
  profileHovered = false;
  dotsHovered = false;
  user: any = null;
  currentSearch: string = '';
  currentUrl: string = '';
  dropDown : boolean = false;
  dropDownExplore : boolean = false;
  hoverTimeout : any;
  isInputFocused :boolean = false;
  Suggestions$!: Observable<any>
  private searchSubject  = new BehaviorSubject<string>('');
  private ngOnDestroy$ = new Subject<void>();
  @Output() hoverStateChange = new EventEmitter<{ key: string; state: boolean }>();

  constructor(
    private router: Router,
    private authService: AuthService,
    private route: ActivatedRoute,
    private photosSearchService: PhotosSearchService,
  ) {}


  ngOnInit(): void {
    this.checkUrl();
    this.getCurrentUser();
    this.Suggestions$ = this.searchSubject.pipe(
      debounceTime(100),
      catchError((error) => {
        console.error('Error fetching suggestions:', error);
        return [];
      }),
      switchMap((value) => {
        console.log("vrijednost: "+ value, "suggestion "+ this.Suggestions$);
        if (value.trim() === '') {
          return of([]); 
        }
        return this.photosSearchService.suggestionsPhotos(value);
      }), 
      tap((res)=> console.log(res))
    );
  }

  ngOnDestroy(): void {
    this.ngOnDestroy$.next();
    this.ngOnDestroy$.complete();
  }


  checkUrl(): void {
      this.route.url.pipe(takeUntil(this.ngOnDestroy$)).subscribe((segment) => {
        console.log (segment);
        this.currentUrl = segment.join('/');
      })

      if(this.currentUrl.includes('search')){
        this.route.queryParams.pipe(takeUntil(this.ngOnDestroy$)).subscribe(params => {
          this.currentSearch=params['q'];
        })
      }
  }

  goToSearchPage(): void {
    if(this.currentSearch)
    this.router.navigate(["/public/search"], { queryParams: { q: this.currentSearch } });
  }

  goToProfilePage(name:string): void {
    this.router.navigate([`/public/profile/user/${name}`]);
  }


  updateSearch(value: string) {
    if(this.currentSearch){
      this.currentSearch = value;
      this.goToSearchPage();
      this.onBlur();
    }
  }


  updateSearchWithoutValue(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    if(value === '') return;
    this.searchSubject.next(value);
    console.log(this.searchSubject);
    
  }


  onFocus() {
    this.isInputFocused = true;
  }

  onBlur() {
    this.isInputFocused = false;
  }

  onKeyDown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      this.goToSearchPage();
    }
  }

  getCurrentUser(): void {

    this.authService.currentUser$.subscribe((user) => {
      this.user = user;
    });
 
    if (!this.user) {
      this.authService.getCurrentUser().subscribe({
        error: () => {
          //console.error('Error fetching user');
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
        this.router.navigate(["public/home"]);
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

  goToAiGenerator(){
    this.router.navigate(['/public/generate-image']);
  }


  toggleDropdown(): void {
    this.dropDown = !this.dropDown;
    if (this.dropDown) {
      this.dropDownExplore = false;
    }
  }
  
  toggleDropdownExplore(): void {
    this.dropDownExplore = !this.dropDownExplore;
    if (this.dropDownExplore) {
      this.dropDown = false;
    }
  }
  
  onMouseEnterDropdown(): void {
    clearTimeout(this.hoverTimeout); 
    this.dropDown = true;
  }

  onMouseLeaveDropdown(): void {
    this.hoverTimeout = setTimeout(() => {
      this.dropDown = false;
    }, 200); // Zatvori nakon 5 sekundi
  }

  onMouseEnterExplore(): void {
    clearTimeout(this.hoverTimeout);
    this.dropDownExplore = true;
  }

  onMouseLeaveExplore(): void {
    this.hoverTimeout = setTimeout(() => {
      this.dropDownExplore = false;
    }, 200);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;

    // Proveri da li je klik izvan dropdown-a
    if (!target.closest('.dropdown')) {
      this.dropDown = false;
    }
    if (!target.closest('.dropdown-explore')) {
      this.dropDownExplore = false;
    }

    if(!target.closest('.search')){ 
      this.onBlur();
    }
  }
  
}
