import { Component } from '@angular/core';
import { GetAllPhotosService } from '../../services/get-all-photos.service';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {HttpClientModule} from '@angular/common/http';
@Component({
  selector: 'app-gallery',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './gallery.component.html',
  styleUrl: './gallery.component.css'
})
export class GalleryComponent implements OnInit {
  photos: any[] = [];

  constructor(private getAllPhotosService: GetAllPhotosService) { }

  ngOnInit() {
    this.getAllPhotosService.getAllPhotos().subscribe((data) => {
      this.photos = [...data];
      //console.log(this.photos);
    });
  }

}
