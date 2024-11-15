import { Component } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { GalleryComponent } from '../../components/gallery/gallery.component';
@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [NavBarComponent, GalleryComponent],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent {

}
