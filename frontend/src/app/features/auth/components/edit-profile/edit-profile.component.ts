import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { UpdateUserService } from '../../services/update-user.service';
import { CommonModule } from '@angular/common';
import { RegisterService } from '../../services/register.service';

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
  profileImgUrl: string | null = null; // Store the profile image URL
  isImageDeleted: boolean = false;
  public isLoading: boolean = false;

  @ViewChild('fileInput') fileInput!: ElementRef;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private updateUserService: UpdateUserService,
  ) {}

  ngOnInit(): void {
    this.authService.getCurrentUser().subscribe({
      next: (response: any) => {
        if (response?.user) {
          this.initForm(response.user);
          this.profileImgUrl = response.user.profileImgUrl || null; // Set the profile image URL
        }
      },
      error: (err) => {
        console.error('Error fetching user:', err);
      },
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
    //this.editProfileForm.get('username')?.disable();
  }

  saveProfile(): void {
    if (this.editProfileForm && this.editProfileForm.valid) {
      const { firstName, lastName, username } = this.editProfileForm.getRawValue();
      const profileImg = this.isImageDeleted ? null : this.fileInput?.nativeElement?.files[0];
      this.isLoading = true;
      this.updateUserService.updateUser({
        firstName,
        lastName,
        username,
        profileImg,
        isImageDeleted: this.isImageDeleted,
      }).subscribe({
        next: (response) => {
          console.log('Profile updated successfully:', response);
          this.usernameError = '';
        },
        error: (err) => {
          console.error('Error updating profile:', err);

          if (err?.message) {
            if (err.message.includes('Username is already taken')) {
              this.usernameError = 'Username is already taken.';
              this.editProfileForm.get('username')?.valueChanges.subscribe(() => {
                this.usernameError = '';
              });
            } else {
              alert(err.message || 'An error occurred while updating the profile.');
            }
          }
        },

        complete:() =>{
          this.isLoading = false;
        }
      });
    } else {
      console.log('Form is invalid');
    }
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
      // Optionally, you can upload the image to the server here.
    }
  }

  // This method can be used to delete the profile image
  deleteImage(): void {
    this.profileImgUrl = null; // Uklanja prikaz slike
    this.isImageDeleted = true; // Oznaka da je slika obrisana
    this.editProfileForm.markAsDirty(); // Oznaka da je forma izmijenjena
  }

  resetUsernameError(): void {
    this.usernameError = '';  // Resetuje grešku korisničkog imena
  }
}
