import { Component } from '@angular/core';
import { GetAllPhotosService } from '../../services/get-all-photos.service';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import _ from 'lodash';

@Component({
  selector: 'app-gallery',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './gallery.component.html',
  styleUrl: './gallery.component.css'
})
export class GalleryComponent implements OnInit {
  photos: any[] = [];
  originalPhotos: any[] = []; 
   selectedFilter: string = 'trending';
  constructor(private getAllPhotosService: GetAllPhotosService ) { }

  ngOnInit() {
    this.getAllPhotosService.getAllPhotos().subscribe((data) => { 
      this.originalPhotos = data;
      this.photos = this.sortByTrending(data);
      console.log(this.photos);
    });
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
}


