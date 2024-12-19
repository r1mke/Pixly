import { Component, OnDestroy, OnInit } from '@angular/core';
import { EditProfilePageComponent } from '../../../auth/pages/edit-profile-page/edit-profile-page.component';
import { GalleryComponent } from "../../../public/components/gallery/gallery.component";
import { UserService } from '../../../public/services/user.service';
import { takeUntil } from 'rxjs';
import { Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule,EditProfilePageComponent, GalleryComponent],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css'
})
export class UserProfileComponent implements OnInit,OnDestroy {
  username: string | undefined = undefined;
  private ngOnDestory = new Subject<void>();
  show:boolean = false
  constructor(private userService: UserService) {
  }
  
  ngOnInit(): void {
    this.userService.updateAdminPhotos$.pipe(takeUntil(this.ngOnDestory)).subscribe((value) => {
      this.show = value;
   }
  )}

  ngOnDestroy(): void {
    this.ngOnDestory.next();
    this.ngOnDestory.complete();
  }
}


