import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { UpdateUserService } from '../../services/update-user.service';
import { CommonModule } from '@angular/common';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.css'],
})
export class EditProfileComponent implements OnInit {
  editProfileForm!: FormGroup;
  isEmailDisabled: boolean = true;
  isUsernameDisabled: boolean = true;
  public usernameError: string = '';
  profileImgUrl: string | null = null;
  isImageDeleted: boolean = false;
  public isLoading: boolean = false;

  @ViewChild('fileInput') fileInput!: ElementRef;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private updateUserService: UpdateUserService,
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$
      .pipe(
        tap((user) => {
          if (user) {
            this.initForm(user);
            this.profileImgUrl = user.profileImgUrl || null;
          }
        })
      )
      .subscribe({
        error: (err) => console.error('Error fetching current user:', err),
      });
  }

  private initForm(user: any): void {
    this.editProfileForm = this.fb.group({
      firstName: [
        user.firstName || null,
        [Validators.required, Validators.minLength(2), Validators.maxLength(20)],
      ],
      lastName: [
        user.lastName || null,
        [Validators.required, Validators.minLength(2), Validators.maxLength(20)],
      ],
      username: [
        user.username || null,
        [Validators.required, Validators.minLength(5), Validators.maxLength(20)],
      ],
      email: [
        user.email || null,
        [Validators.required, Validators.email],
      ],
    });

    this.editProfileForm.get('email')?.disable();
  }

  saveProfile(): void {
    if (this.editProfileForm && this.editProfileForm.valid) {
      const { firstName, lastName, username } = this.editProfileForm.getRawValue();
      const profileImg = this.isImageDeleted ? null : this.fileInput?.nativeElement?.files[0];
      this.isLoading = true;
      this.updateUserService
        .updateUser({firstName, lastName, username, profileImg, isImageDeleted: this.isImageDeleted,
        })
        .subscribe({
          next: () => {
            this.usernameError = '';
          },
          error: (err) => {
            if (err?.message && err.message.includes('Username is already taken')) {
                this.usernameError = 'Username is already taken.';
                this.editProfileForm.get('username')?.valueChanges.subscribe(() => {
                this.usernameError = '';
              });
              this.isLoading = false;
            } 
          },
          complete: () => {
            this.isLoading = false;
          },
        });
      } 
    else
      console.log('Form is invalid');
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
  }
}
