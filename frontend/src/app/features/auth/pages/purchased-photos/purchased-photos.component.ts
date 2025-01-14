import { Component } from '@angular/core';
import { GalleryComponent } from "../../../public/components/gallery/gallery.component";
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";

@Component({
  selector: 'app-purchased-photos',
  standalone: true,
  imports: [GalleryComponent, NavBarComponent],
  templateUrl: './purchased-photos.component.html',
  styleUrl: './purchased-photos.component.css'
})
export class PurchasedPhotosComponent {

}
