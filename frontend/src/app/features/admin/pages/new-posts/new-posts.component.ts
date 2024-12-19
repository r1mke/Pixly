import { Component } from '@angular/core';
import { GalleryComponent } from "../../../public/components/gallery/gallery.component";
import { NavigationComponent } from '../../components/navigation/navigation.component';
@Component({
  selector: 'app-new-posts',
  standalone: true,
  imports: [GalleryComponent,NavigationComponent],
  templateUrl: './new-posts.component.html',
  styleUrl: './new-posts.component.css'
})
export class NewPostsComponent {

}
