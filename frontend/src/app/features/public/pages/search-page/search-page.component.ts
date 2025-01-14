import { Component, OnInit } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { GalleryComponent } from "../../components/gallery/gallery.component";
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-search-page',
  standalone: true,
  imports: [NavBarComponent, GalleryComponent,CommonModule],
  templateUrl: './search-page.component.html',
  styleUrl: './search-page.component.css'
})
export class SearchPageComponent implements OnInit{
  currentSearch: string = '';
  photosTab: boolean = true;
  displayUsersComponent: boolean = false;
  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    this.route.url.subscribe((segments) => {
      const currentUrl = segments.map(segment => segment.path).join('/');
      this.photosTab = currentUrl.includes('photos');
      this.currentSearch = this.photosTab ? 'Photo Search' : 'User Search';
      console.log('Current URL:', currentUrl);
      console.log('Photos Tab:', this.photosTab);
    });

    

    this.route.queryParams.subscribe(params => {
      this.currentSearch = params['q'];
      if(!this.currentSearch) this.router.navigate(['home']);
    });


  }

  receivedmeesage(message: boolean){
    this.displayUsersComponent = message;
  }

}
