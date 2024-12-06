import { Component, HostListener } from '@angular/core';
import { GetAllPhotosService } from '../../services/get-all-photos.service';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import _ from 'lodash';
import { InfiniteScrollModule   } from '@robingenz/ngx-infinite-scroll';


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
  totalPhotos: number = 0;
  constructor(private getAllPhotosService: GetAllPhotosService ) { }


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
        this.totalPhotos = res.totalPhotos;
      },
      error:(error) => {
        console.error('Error fetching user:', error);
      }, 
      complete: () => {
        this.isLoading = false; 
      }
    }  
    )
  }

  loadMoreItems(){
    if (this.totalPhotos === this.originalPhotos.length) return;
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
  
      if (this.totalPhotos === this.originalPhotos.length) return;
      if (scrollPosition + windowHeight >= documentHeight * 0.9) {
        this.loadMoreItems();
      }
    }
}


