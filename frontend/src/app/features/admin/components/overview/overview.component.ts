import { Component } from '@angular/core';
import { PhotoEndpointsService } from "../../services/Endpoints/Photo/photo-endpoints.service";
import { Observable, Subject, takeUntil } from 'rxjs';
import { Router } from '@angular/router';
@Component({
  selector: 'app-overview',
  standalone: true,
  imports: [],
  templateUrl: './overview.component.html',
  styleUrl: './overview.component.css'
})
export class OverviewComponent {
   photos : any[] = [];
   private ngOnDestroy$ = new Subject<void>();
  
      constructor(private photoService: PhotoEndpointsService,
        private router: Router
      ) {}
  
      ngOnInit(): void {
          this.photoService.getAllPhotos().pipe(takeUntil(this.ngOnDestroy$)).subscribe(data => 
            {
              this.photos = data;
              console.log(this.photos);
            });
      }

      ngOnDestroy(): void {
        this.ngOnDestroy$.next();
        this.ngOnDestroy$.complete();
    }

    goToNewPosts(){
      this.router.navigate(['/admin/new-posts']);
    }
}
