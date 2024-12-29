import { Component } from '@angular/core';
import { StripeService } from '../../services/stripe.service';
import { ActivatedRoute } from '@angular/router';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { PhotoService } from '../../services/photo.service';

@Component({
  selector: 'app-success-page',
  standalone: true,
  imports: [NavBarComponent],
  templateUrl: './success-page.component.html',
  styleUrls: ['./success-page.component.css']
})
export class SuccessPageComponent {
  photoId: any | null = null;
  photo: any | null = null;

  constructor(private stripeService: StripeService, private activatedRoute: ActivatedRoute, private photoService: PhotoService) {}

  ngOnInit(): void {
    this.getPhotoById();
  }

  getPhotoById(): void {
    this.photoId = this.activatedRoute.snapshot.paramMap.get('id');

    if (this.photoId) {
      this.photoService.getPhotoById(this.photoId).subscribe({
        next: (data) => {
          this.photo = data;
        },
        error: (error) => {
          console.error('Error fetching photo:', error);
        }
      });
    } else {
      console.error('No photo ID provided');
    }
  }

  downloadPhoto(): void {
    if (this.photoId) {
      this.stripeService.downloadPhoto(this.photoId).subscribe({
        next: (blob: Blob) => {
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.download = `photo-${this.photoId}.jpg`;
          link.click();
        },
        error: (error) => {
          console.error('Greška prilikom preuzimanja slike:', error);
        }
      });
    }
  }
  
}
