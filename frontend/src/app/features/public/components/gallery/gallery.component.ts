import { Component, HostListener } from '@angular/core';
import { GetAllPhotosService } from '../../services/Photos/get-all-photos.service';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import _ from 'lodash';
import { AuthService } from '../../../auth/services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { PhotosSearchService } from '../../services/Photos/photos-search.service';
import { FormsModule } from '@angular/forms';
import { SearchRequest } from '../../model/searchRequest';
import { Router } from '@angular/router';
import { PhotoGetAllRequest } from '../../model/PhotoGetAllRequest';
import { PhotoGetAllResult } from '../../model/PhotoGetAllResult';
import { SearchResult } from '../../model/SearchResult';
export class AppModule {}
@Component({
  selector: 'app-gallery',
  standalone: true,
  imports: [CommonModule, FormsModule ],
  templateUrl: './gallery.component.html',
  styleUrl: './gallery.component.css'
})
export class GalleryComponent implements OnInit {

  searchRequest : SearchRequest = {
     Popularity: '',
     Title: '',
     Orientation: null,
     Size: null,
     Color: null,
     PageNumber: 1,
     PageSize: 10 
  };

  searchResult : SearchResult = {
    Photos: [],
    TotalPhotos: 0,
    TotalPages: 0,
    PageNumber: 1,
    PageSize: 0
  }

  getAllRequest : PhotoGetAllRequest = {
    PageNumber: 1,
    PageSize: 2
  }

  getAllResult : PhotoGetAllResult = {
    Photos: [],
    TotalPhotos: 0,
    TotalPages: 0,
    PageNumber: 1,
    PageSize: 0
  }

  photos: any[] = [];
  originalPhotos: any[] = []; 

  isLoading: boolean = false;

  selectedOption: string = 'photos';
  selectedFilter: string = 'trending';

  user: any = null;
  currentUrl : string = '';
  
  
  //more filters  section
  currentPopularity : string = 'Trending';
  currentOrientation : string = 'All Orientations';
  currentSize : string = 'All Sizes';
  selectedColor: string = '';
  isMoreFiltersDropdownOpen: boolean = false;
  openMoreFiltersDropdown: string | null = null;
  isFilterDropdownOpen: boolean = false;

  //colors dropdown
  predefinedColors: string[] = [
    '#795548', '#F44336', '#E91E63', '#9C27B0', '#673AB7',
    '#3F51B5', '#2196F3', '#00BCD4', '#009688', '#4CAF50',
    '#8BC34A', '#CDDC39', '#FFEB3B', '#FFC107', '#FF9800',
    '#FF5722', '#9E9E9E', '#607D8B', '#000000', '#FFFFFF'
  ];

  constructor(private getAllPhotosService: GetAllPhotosService,
              private authService: AuthService,
              private route: ActivatedRoute,
              private photosSearchService: PhotosSearchService,
              private router: Router ) { }



  ngOnInit(): void {
    this.checkQueryParams();
    this.checkUrl();
    this.checkUser();
  }


  checkUrl(){
    this.route.url.subscribe((segment) => {
      this.currentUrl = segment.join('/');
      console.log(this.currentUrl);
      this.loadPhotos();
    })
  }

  checkQueryParams(){
    this.route.queryParams.subscribe(params => {
      this.searchRequest = 
      {  
         Popularity: this.currentPopularity,
         Title: params['q'],
         Orientation: params['orientation'],
         Size: params['size'],
         Color: params['color'],
         PageNumber: 1,
         PageSize: 10
      };
      if (this.currentUrl.includes('search')) {
        this.loadSearchPhotos();
      }
    })
  }

  checkUser(){
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

  loadSearchPhotos() {
    this.isLoading = true;
    this.photos = [];
    this.searchResult.Photos = [];
    this.photosSearchService.searchPhotos(this.searchRequest).subscribe({
      next: (res) => {
        this.searchResult.Photos = [...this.searchResult.Photos, ...res.photos]; 
        this.photos = this.searchResult.Photos  
        this.searchResult.TotalPhotos = res.totalPhotos;
        this.searchResult.TotalPages = res.totalPages;
        this.searchRequest.PageNumber++;
      },
      error: (error) => {
        console.error('Error fetching photos:', error);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  loadPopularPhotos() {
    if (this.isLoading) return;
    if(this.getAllResult.TotalPages > 0){
      if (this.getAllRequest.PageNumber > this.getAllResult.TotalPages) return;
    }  
    this.isLoading = true;
    this.getAllPhotosService.getAllPhotos(this.getAllRequest).subscribe({
      next: (res) => {
        console.log(res);
        this.getAllResult.Photos = [...this.getAllResult.Photos, ...res.photos]; 
        this.photos =this.getAllResult.Photos 
        this.getAllResult.TotalPages = res.totalPages;  
        this.getAllRequest.PageNumber++;  
      },
      error: (error) => {
        console.error('Error fetching photos:', error);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }


  loadPhotos() {
    if(this.currentUrl.includes('search')) this.loadSearchPhotos();
    if(this.currentUrl.includes('home')) this.loadPopularPhotos();
  }

  loadMoreItems() {
    if (this.isLoading) return;
    if (this.currentUrl.includes('search')) {
      if (this.searchRequest.PageNumber <= this.searchResult.TotalPages) {
        this.loadSearchPhotos(); 
      }
    }
    if (this.currentUrl.includes('home')) {
      if (this.getAllRequest.PageNumber <= this.getAllResult.TotalPages) {
        this.loadPopularPhotos(); 
      }
    }
  }

    @HostListener('window:scroll', ['$event'])
    onScroll(event: Event): void {
      const scrollPosition = window.scrollY;
      const windowHeight = window.innerHeight;
      const documentHeight = document.documentElement.scrollHeight;
  
      // Provjera je li korisnik došao do 90% stranice
      if (scrollPosition + windowHeight >= documentHeight * 0.9) {
        this.loadMoreItems();
      }
    }


  toggleLike(photo: any) {
    const userId = this.user.userId;

    if (!photo.isLiked) {
      this.getAllPhotosService.likePhoto(photo.id, userId).subscribe({
        next: (res) => {
          photo.isLiked = true;
        },
        error: (err) => {
          console.error('Error liking photo:', err.error?.Message || err.message);
        },
      });
    } 
    else {
      this.getAllPhotosService.unlikePhoto(photo.id, userId).subscribe({
        next: (res) => {
          photo.isLiked = false;
        },
        error: (err) => {
          console.error('Error unliking photo:', err.error?.Message || err.message);
        },
      });
    }
  }    


  selectOption(option: string) {
    this.selectedOption = option;
  }

  toggleFilterDropdown() {
    this.isFilterDropdownOpen = !this.isFilterDropdownOpen;
  }

  toggleMoreFiltersDropdown(dropdownId: string) {
    this.openMoreFiltersDropdown = 
      this.openMoreFiltersDropdown === dropdownId ? null : dropdownId;
  }

  closeDropdowns() {
    this.isFilterDropdownOpen = false;
    this.openMoreFiltersDropdown = null;
  }

  toggleMoreFilterDropdown(){
    this.isMoreFiltersDropdownOpen = !this.isMoreFiltersDropdownOpen;
    console.log(this.isMoreFiltersDropdownOpen);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;

    // Proveri da li je klik izvan dropdowna
    if (!target.closest('.dropdown') && !target.closest('.filter-select')) {
      this.closeDropdowns();
    }
  }

  updateQueryString() {
    const queryParams: any = {};

    queryParams.orientation = this.currentOrientation !== 'All Orientations'
    ? this.currentOrientation.toLowerCase()
    : null;

    queryParams.size = this.currentSize !== 'All Sizes'
    ? this.currentSize.toLowerCase()
    : null;

    queryParams.popularity = this.currentPopularity;

    queryParams.color = this.selectedColor !== null
    ? this.selectedColor.toLowerCase()
    : null;

    this.router.navigate([], {
      queryParams,
      queryParamsHandling: 'merge', 
      replaceUrl: true,
    });
  }

  selectOrientation(orientation: string) {
    this.currentOrientation = orientation;
    this.updateQueryString();
  }

  selectSize(size: string) {
    this.currentSize = size;
    this.updateQueryString();
  }
  selectPopularity(popularity: string) {
    this.currentPopularity = popularity;
    this.updateQueryString();
  }

  setColor(color: string) {
    this.selectedColor = color;
    if(this.validateHexCode()) this.updateQueryString();
  }

  validateHexCode() {
    const hexRegex = /^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$/;
    if (!hexRegex.test(this.selectedColor)) {
      return false;
    }
    return true;
    this.checkQueryParams();
  }

}

