import { Component, OnInit, OnDestroy } from '@angular/core';
import { GalleryComponent } from "../../../public/components/gallery/gallery.component";
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { Subject, takeUntil } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-new-posts',
  standalone: true,
  imports: [GalleryComponent,NavigationComponent,CommonModule],
  templateUrl: './new-posts.component.html',
  styleUrl: './new-posts.component.css'
})
export class NewPostsComponent implements OnInit, OnDestroy {
  currentUrl: string = '';
  private ngOnDestory = new Subject<void>();
  isAdminPage: boolean = false;
  username = '';
  constructor(private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.checkUrl();
  }


  ngOnDestroy(): void {
    this.ngOnDestory.next();
    this.ngOnDestory.complete();
  }

  checkUrl(){
      this.route.url.pipe(takeUntil(this.ngOnDestory)).subscribe((segment) => {
        this.username = this.route.snapshot.paramMap.get('username')?? '';
        this.currentUrl = segment.map(segment => segment.path).join('/');
        this.isAdminPage = this.router.url.includes('admin');
        console.log(this.currentUrl, this.isAdminPage);
      })
    }

}
