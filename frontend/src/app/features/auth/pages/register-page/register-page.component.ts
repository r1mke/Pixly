import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CustomValidators } from '../../pages/register-page/custom-validators'; 
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { Router, RouterLink } from '@angular/router';
import { RegisterService } from '../../services/register.service';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component"; 

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgbModule, RouterLink, NavBarComponent],
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css'] 
})
export class RegisterPageComponent {
  public frmSignup: FormGroup;
  public showPassword: boolean = false;
  public showConfirmPassword: boolean = false;
  public emailError: string = '';
  public usernameError: string = '';
  public isLoading: boolean = false;
  
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private registerService = inject(RegisterService);

  constructor() {
    this.frmSignup = this.createSignupForm();  
    this.setupListeners();
  }

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
      this.isLoading = true;
      const userData = this.frmSignup.value;
      console.log(userData);
  
      this.registerService.registerUser(userData).subscribe({
        next: (response) => {
          //console.log('Registration successful', response);
          this.router.navigate(['/auth/verify-email']);
        },
        error: (err) => {
          //console.error('Error occurred during registration', err);
        },
        complete: () => {
          this.isLoading = false;
        },
      });
    } 
  }
  
 checkEmail(email: string): void {
  this.emailError = '';
  
  if (!email) return;

  this.registerService.checkEmail(email).subscribe({
    next: (response) => {
      if (!response.available)
        this.emailError = response.message;
    },
    error: (err) => {
      //console.error('Greška prilikom provjere emaila', err);
    }
  });
}

checkUsername(username: string): void {
  this.usernameError = '';
  this.registerService.checkUsername(username).subscribe({
    next: (response) => {
      if (!response.available) 
        this.usernameError = response.message;
    },
    error: (err) => {
      //console.error('Greška prilikom provjere korisničkog imena', err);
    }
  });
}

  private setupListeners(): void {
    this.frmSignup.get('email')?.valueChanges.subscribe((email) => {
      if (email) 
        this.checkEmail(email);
    });

    this.frmSignup.get('username')?.valueChanges.subscribe((username) => {
      if (username) 
        this.checkUsername(username);
    });
  }
}
