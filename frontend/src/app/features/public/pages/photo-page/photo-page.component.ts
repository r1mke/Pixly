import { Component, OnInit } from '@angular/core';
import { PhotoService } from '../../services/photo.service';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { GalleryComponent } from "../../components/gallery/gallery.component";
import { AuthService } from '../../../auth/services/auth.service';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { CustomDatePipe } from '../../helper/custom-date.pipe';
import { Location } from '@angular/common';

@Component({
  selector: 'app-photo-page',
  standalone: true,
  templateUrl: './photo-page.component.html',
  styleUrls: ['./photo-page.component.css'],
  imports: [NavBarComponent, CommonModule, GalleryComponent, CustomDatePipe],
})
export class PhotoPageComponent implements OnInit {
  photo: any;
  currentUser: any = null;
  currentUserId : number = 0;
  profileUserId: number = 0;
  isOwnProfile: boolean = false;

  constructor(
    private photoService: PhotoService, 
    private route: ActivatedRoute, 
    private authService: AuthService, 
    private router: Router,
    private location: Location) {}

  ngOnInit(): void {
     this.getPhotoById();

     this.authService.currentUser$.subscribe((user) => {
      this.currentUser = user;
    });

    if (!this.currentUser) {
      this.authService.getCurrentUser().subscribe({
        error: () => {
          console.error('Error fetching user');
        },
      });
    }
  }

  goToEditPhoto() {
    this.router.navigate([`auth/photo/${this.photo.id}/edit`]);
  }

 getPhotoById(): void {
    const photoId = Number(this.route.snapshot.paramMap.get('id'));
    if (photoId) {
      this.photoService.getPhotoById(photoId).subscribe({
        next: (data) => {
          this.photo = data;
          this.profileUserId = data.user.id;
          this.checkIfOwnprofile();
        },
        error: (error) => {
          console.error('Error fetching photo:', error);
          this.location.back();
        }
      });
    } else {
      console.error('No photo ID provided');
    }
  }

  private checkIfOwnprofile(): void{
    if(this.currentUser && this.currentUser.userId === this.profileUserId)
      this.isOwnProfile = true;
    else
      this.isOwnProfile = false; 
  }


  toggleLike(photo: any, event: Event) {
    if(this.currentUser)
      this.currentUserId = this.currentUser.userId;
    else 
      this.router.navigate(['auth/login']);

      event.stopPropagation();
      const action = photo.isLiked ? this.photoService.unlikePhoto(photo.id, this.currentUser.userId) : this.photoService.likePhoto(photo.id, this.currentUser.userId);
   
      action.subscribe({
        next: () => {
          photo.isLiked = !photo.isLiked;
          this.getPhotoById();
        },
        error: (err) => {
          console.error('Error updating like status:', err.error?.Message || err.message);
        },
      });
  }
}
