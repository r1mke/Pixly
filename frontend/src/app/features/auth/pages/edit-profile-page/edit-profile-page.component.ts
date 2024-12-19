import { Component, ElementRef, OnInit,OnDestroy, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { UpdateUserService } from '../../services/update-user.service';
import { CommonModule } from '@angular/common';
import { tap } from 'rxjs/operators';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { ActivatedRoute } from '@angular/router';
import { Observable, Subject, takeUntil } from 'rxjs';
import { NavigationComponent } from "../../../admin/components/navigation/navigation.component";
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { UserService } from '../../../public/services/user.service';
import { NgbdToast } from "../../../shared/components/toast/toast.component";

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavBarComponent, NgbdToast, NavigationComponent],
  templateUrl: './edit-profile-page.component.html',
  styleUrls: ['./edit-profile-page.component.css'],
})
export class EditProfilePageComponent implements OnInit,OnDestroy {
  editProfileForm!: FormGroup;
  isEmailDisabled: boolean = true;
  isUsernameDisabled: boolean = true;
  public usernameError: string = '';
  profileImgUrl: string | null = null;
  isImageDeleted: boolean = false;
  public isLoading: boolean = false;
  currentUser: any = null;
  currentRoute : string = '';
  private ngOnDestroy$ = new Subject<void>();
  adminPage : boolean = false;
  username : string = '';
  userData$!: Observable<any>;
  @ViewChild('fileInput') fileInput!: ElementRef;
  @ViewChild(NgbdToast)
  ngbdToast!: NgbdToast;


  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private updateUserService: UpdateUserService,
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.checkUrl();
    this.loadData();
  }

    


  ngOnDestroy(): void {
    this.ngOnDestroy$.next();
    this.ngOnDestroy$.complete();
  }

  loadData(): void {
    if(!this.adminPage) this.checkUser();
  }

  checkUser(): void {
    this.authService.currentUser$
    .pipe(
      takeUntil(this.ngOnDestroy$),
      tap((user) => {
        if (user) {
          this.updateForm(user);
          this.profileImgUrl = user.profileImgUrl || null;
          this.currentUser = user;
        }
      })
    )
    .subscribe({
      error: (err) => console.error('Error fetching current user:', err),
    });
   }

  checkUrl(): void {
    this.currentRoute = this.router.url;
    this.adminPage = this.currentRoute.includes('admin'); 
  
    
    this.route.paramMap.pipe(
      takeUntil(this.ngOnDestroy$),
      map(params => params.get('username') || '')
    ).subscribe(username => {
      this.username = username;
      if (this.username) this.getUserByUsername();
    });
  }


  getUserByUsername(): void {
    this.userService.getUserByUsername(this.username).subscribe({
      next: (data) => {
        console.log('User Data:', data); // Provjeri podatke u konzoli
        if (data) {
          this.updateForm(data);
          this.profileImgUrl = data.profileImgUrl || null;
        }
      },
      error: (err) => {
        console.error('Error fetching user by username:', err);
      }
    });
  }

  private initForm(): void {
    this.editProfileForm = this.fb.group({
      firstName: [null, [Validators.required, Validators.minLength(2), Validators.maxLength(20)]],
      lastName: [null, [Validators.required, Validators.minLength(2), Validators.maxLength(20)]],
      username: [null, [Validators.required, Validators.minLength(5), Validators.maxLength(20)]],
      email: [null, [Validators.required, Validators.email]],
    });

    this.editProfileForm.get('email')?.disable();
  }


  private updateForm(user: any): void {
    this.editProfileForm.patchValue({
      firstName: user.firstName,
      lastName: user.lastName,
      username: user.username,
      email: user.email,
    });
  }

  saveProfile(): void {
    if (this.editProfileForm.valid) {
      const { firstName, lastName, username } = this.editProfileForm.getRawValue();
      const profileImg = this.isImageDeleted ? null : this.fileInput?.nativeElement?.files[0];
      this.isLoading = true;

      this.updateUserService
        .updateUser({
          firstName,
          lastName,
          username,
          profileImg,
          isImageDeleted: this.isImageDeleted,
        }).pipe(takeUntil(this.ngOnDestroy$))
        .subscribe({
          next: () => this.handleUpdateSuccess(),
          error: (err) => this.handleUpdateError(err),
          complete: () => {
            this.isLoading = false;
            this.ngbdToast.showMessage('Successfully updated!', 'success');
          }
        });
    } else {
      console.log('Form is invalid');
    }
  }

  private handleUpdateSuccess(): void {
    this.usernameError = '';
    this.authService.getCurrentUser().subscribe({
      next: (user) => {
        if (user.username === this.currentUser.username) {
          this.updateForm(user);
          this.profileImgUrl = user.profileImgUrl || null;
          this.currentUser = user;
        }
      },
      error: (err) => console.error('Error syncing current user:', err),
    });
  }

  private handleUpdateError(err: any): void {
    console.error('Error updating profile:', err);
    if (err?.message?.includes('Username is already taken')) {
      this.usernameError = 'Username is already taken.';
      this.resetUsernameErrorOnChange();
    } else {
      alert(err.message || 'An error occurred while updating the profile.');
    }
    this.isLoading = false;
  }

  private resetUsernameErrorOnChange(): void {
    this.editProfileForm.get('username')?.valueChanges.subscribe(() => {
      this.usernameError = '';
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.profileImgUrl = reader.result as string;
        this.editProfileForm.markAsDirty();
      };
      reader.readAsDataURL(file);
    }
  }

  deleteImage(): void {
    this.profileImgUrl = null;
    this.isImageDeleted = true;
    this.editProfileForm.markAsDirty();
    //this.ngbdToast.showMessage('You need to save the profile!', 'warning');
  }

  resetUsernameError(): void {
    this.usernameError = '';
  }

  loadPhotos(){
    this.userService.updateAdminPhotosSubject();
  }

}
