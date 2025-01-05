import { Component } from '@angular/core';
import { StripeService } from '../../services/stripe.service';
import { ActivatedRoute, Router } from '@angular/router';
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
  photoId: number | null = null;
  photo: any | null = null;

  constructor(
    private stripeService: StripeService,
    private activatedRoute: ActivatedRoute,
    private photoService: PhotoService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.getSessionAndProcessPayment();
  }

  private async getSessionAndProcessPayment(): Promise<void> {
    try {
      const sessionId = this.activatedRoute.snapshot.paramMap.get('session_id');
      const photoIdString = this.activatedRoute.snapshot.paramMap.get('id');
    
      this.photoId = photoIdString ? Number(photoIdString) : null;

      if (sessionId && this.photoId) {
        await this.verifyPayment(sessionId, this.photoId); 
        await this.getPhotoById(this.photoId); 
      } else {
        this.router.navigate(['public/home']);
      }
    } catch (error) {
      this.router.navigate(['public/home']);
    }
  }

  private async verifyPayment(sessionId: string, photoId: number): Promise<void> {
    try {
      const response = await this.stripeService.savePayment(sessionId, photoId, 10).toPromise(); // Pretvaramo u promise

      if (!response.isValid) {
        this.router.navigate(['public/home']);
      } else {
      }
    } catch (error) {
      console.error('Error verifying payment:', error);
      this.router.navigate(['public/home']);
    }
  }

  private async getPhotoById(photoId: number): Promise<void> {
    try {
      const data = await this.photoService.getPhotoById(photoId).toPromise();
      this.photo = data;
    } catch (error) {
      console.error('Error fetching photo:', error);
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
          console.error('Error downloading photo:', error);
        }
      });
    }
  }
}
