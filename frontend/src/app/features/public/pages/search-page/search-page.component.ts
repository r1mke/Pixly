import { Component, OnInit } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { GalleryComponent } from "../../components/gallery/gallery.component";
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
@Component({
  selector: 'app-search-page',
  standalone: true,
  imports: [NavBarComponent, GalleryComponent],
  templateUrl: './search-page.component.html',
  styleUrl: './search-page.component.css'
})
export class SearchPageComponent implements OnInit{
  currentSearch: string = '';

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.currentSearch = params['q'];
      if(!this.currentSearch) this.router.navigate(['home']);
    });


  }

}
