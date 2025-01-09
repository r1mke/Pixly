import { Component } from '@angular/core';
import { GalleryComponent } from "../../../public/components/gallery/gallery.component";

@Component({
  selector: 'app-purchased-photos',
  standalone: true,
  imports: [GalleryComponent],
  templateUrl: './purchased-photos.component.html',
  styleUrl: './purchased-photos.component.css'
})
export class PurchasedPhotosComponent {

}
