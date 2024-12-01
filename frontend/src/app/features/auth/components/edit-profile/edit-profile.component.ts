import { Component, OnInit } from '@angular/core';
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

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private updateUserService: UpdateUserService,
    private registerService: RegisterService,
  ) {}

  ngOnInit(): void {
    this.authService.getCurrentUser().subscribe({
      next: (response: any) => {
        if (response?.user) 
          this.initForm(response.user);
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

      this.updateUserService.updateUser({ firstName, lastName, username }).subscribe({
        next: (response) => {
          console.log('Profile updated successfully:', response);
          this.checkUsername(response.username);
          alert(response.message);
        },
        error: (err) => {
          console.error('Error updating profile:', err);
          alert(err.message || 'An error occurred while updating the profile.');
        },
      });
    } else {
      console.log('Form is invalid');
    }
  }


  checkUsername(username: string): void {
    this.usernameError = '';
    this.registerService.checkUsername(username).subscribe({
      next: (response) => {
        if (!response.available) 
          this.usernameError = response.message;
      },
      error: (err) => {
        console.error('Greška prilikom provjere korisničkog imena', err);
      }
    });
  }

}
