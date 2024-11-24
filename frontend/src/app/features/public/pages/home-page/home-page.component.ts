import { Component } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { GalleryComponent } from '../../components/gallery/gallery.component';
import { HeroSectionComponent } from "../../components/hero-section/hero-section.component";
@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [NavBarComponent, GalleryComponent, HeroSectionComponent],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent {

}
