import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CustomValidators } from './custom-validators'; 
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { Router, RouterLink } from '@angular/router';
import { RegisterService } from '../../services/register.service';// Importuj RegisterService/ Opcionalno za prikazivanje poruka korisnicima

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgbModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'] 
})
export class RegisterComponent {
  public frmSignup: FormGroup;
  public showPassword: boolean = false;
  public showConfirmPassword: boolean = false;
  
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private registerService = inject(RegisterService);

  constructor() {this.frmSignup = this.createSignupForm();}

  createSignupForm(): FormGroup {
    return this.fb.group({
      firstName: [null, [Validators.required, Validators.minLength(2), Validators.maxLength(20)]], 
      lastName: [null, [Validators.required, Validators.minLength(2), Validators.maxLength(20)]],
      username: [null, [Validators.required, Validators.minLength(5), Validators.maxLength(20)]],
      email: [null, [Validators.required, Validators.email]],
      password: [null, [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(64),
        CustomValidators.patternValidator(/\d/, { hasNumber: true }),
        CustomValidators.patternValidator(/[A-Z]/, { hasCapitalCase: true }),
        CustomValidators.patternValidator(/[a-z]/, { hasSmallCase: true }),
        CustomValidators.patternValidator(/[!@#$%^&*(),.?":{}|<>]/, { hasSpecialCharacters: true }),
      ]],
      confirmPassword: [null, Validators.required],
    }, { validators: CustomValidators.passwordMatchValidator });
  }  

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  submit() {
    if (this.frmSignup.valid) {
      const userData = this.frmSignup.value;
      console.log(userData);
  
      this.registerService.registerUser(userData).subscribe({
        next: (response) => {
          console.log('Registration successful', response);
          const token = response?.jwtToken;
          if (token) {
            localStorage.setItem('jwtToken', token);
            console.log('JWT token saved successfully');
            this.router.navigate(['/auth/verify-email']);
          }
        },
        error: (err) => {
          console.error('Error occurred during registration', err);
        }
      });
    } else {
      console.log('Form is invalid');
    }
  }
}
