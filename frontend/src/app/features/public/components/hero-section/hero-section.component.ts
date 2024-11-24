import { Component } from '@angular/core';
import { GetAllPhotosService } from '../../services/get-all-photos.service';
import { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-hero-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './hero-section.component.html',
  styleUrl: './hero-section.component.css'
})
export class HeroSectionComponent implements OnInit {

  url : string = '';

  constructor(private getAllPhotosService: GetAllPhotosService) {

   }

  ngOnInit(): void {
    this.getAllPhotosService.getRandomPhoto().subscribe((data) => {
      this.url = data;
      console.log(this.url);
    })
  }

}
