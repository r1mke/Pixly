import { Component, HostListener } from '@angular/core';
import { GetAllPhotosService } from '../../services/get-all-photos.service';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import _ from 'lodash';
import { InfiniteScrollModule   } from '@robingenz/ngx-infinite-scroll';
import { AuthService } from '../../../auth/services/auth.service';
import { PhotoService } from '../../services/photo.service';
import { Router } from '@angular/router';


export class AppModule {}
@Component({
  selector: 'app-gallery',
  standalone: true,
  imports: [CommonModule,InfiniteScrollModule ],
  templateUrl: './gallery.component.html',
  styleUrl: './gallery.component.css'
})
export class GalleryComponent implements OnInit {
  photos: any[] = [];
  totalPages : number = 0;
  originalPhotos: any[] = []; 
  selectedFilter: string = 'trending';
  isLoading: boolean = false;
  user: any = null;
  userId : number = 0;

  constructor(private getAllPhotosService: GetAllPhotosService, private authService: AuthService, private router: Router, private photoService: PhotoService ) { }

  ngOnInit(): void {
    this.loadPhotos();
  }

  loadPhotos() {
    if (this.isLoading) return;
    this.isLoading = true;

    this.getAllPhotosService.getAllPhotos().subscribe(
    {
      next:(res)=>{
        console.log(res);
        this.totalPages = res.totalPages;
        this.originalPhotos = [...this.originalPhotos, ...res.photos];
        this.photos = this.originalPhotos;
        this.getAllPhotosService.incrementPageNumber();
      },
      error:(error) => {
        console.error('Error fetching user:', error);
      }, 
      complete: () => {
        this.isLoading = false; 
      }
    }  
    )

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

  loadMoreItems(){
    if (this.isLoading) return;
    console.log("load more items");
    if(this.getAllPhotosService.getCurrentPageNumber() <= this.totalPages){
      this.loadPhotos();
    }
  }

  sortByTrending(data: any) {
    return _.orderBy(data, ['likeCount', 'viewCount'], ['desc', 'desc']);
  }

  sortByLatest(data: any) {
    return _.orderBy(data, ['createAt'], ['desc']);
  }

  filter(event: any) {
    this.selectedFilter = event.target.value;

    if (this.selectedFilter === 'trending') {
      this.photos = [];
      this.photos = this.sortByTrending(this.originalPhotos);
      console.log(this.photos);
    }
    else{
      this.photos = [];
      this.photos = this.sortByLatest(this.originalPhotos);
      console.log(this.photos);
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


  toggleLike(photo: any, event: Event) {
    event.stopPropagation(); 

    if(this.user)
      this.userId = this.user.userId;
    else 
      this.router.navigate(['auth/login']);

    if (!photo.isLiked) {
      this.photoService.likePhoto(photo.id, this.userId).subscribe({
        next: (res) => {
          photo.isLiked = true;
        },
        error: (err) => {
          console.error('Error liking photo:', err.error?.Message || err.message);
        },
      });
    } 
    else {
      this.photoService.unlikePhoto(photo.id, this.userId).subscribe({
        next: (res) => {
          photo.isLiked = false;
        },
        error: (err) => {
          console.error('Error unliking photo:', err.error?.Message || err.message);
        },
      });
    }
  }
  
  openPhotoDetail(photo: any) {
    this.router.navigate(['public/photo', photo.id]);
  }
}


