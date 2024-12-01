import { Component } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { GalleryComponent } from '../../components/gallery/gallery.component';
import { HeroSectionComponent } from "../../components/hero-section/hero-section.component";
import { DisplayCategoriesComponent } from "../../../shared/components/display-categories/display-categories.component";
import { FooterComponent } from '../../../shared/components/footer/footer.component';
@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [NavBarComponent, FooterComponent, HeroSectionComponent, DisplayCategoriesComponent, GalleryComponent],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent {

}
