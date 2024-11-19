import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CustomValidators } from './custom-validators'; 
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { Router, RouterLink } from '@angular/router';
import { RegisterService } from '../../services/register.service'; 

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
  public emailError: string = '';
  public usernameError: string = '';
  
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

 // Provjera emaila
 checkEmail(email: string): void {
  this.emailError = '';  // Resetiraj grešku svaki put kad se mijenja email

  if (!email) {
    return; // Ako je polje prazno, ne šaljemo zahtjev
  }

  this.registerService.checkEmail(email).subscribe({
    next: (response) => {
      if (!response.available) {
        this.emailError = response.message; // Postavi poruku ako email nije dostupan
      }
    },
    error: (err) => {
      console.error('Greška prilikom provjere emaila', err);
    }
  });
}




// Provjera korisničkog imena
checkUsername(username: string): void {
  this.usernameError = '';  // Resetiraj grešku svaki put kad se mijenja korisničko ime
  this.registerService.checkUsername(username).subscribe({
    next: (response) => {
      // Ako korisničko ime nije dostupno, postavi grešku
      if (!response.available) {
        this.usernameError = response.message;  // Postavi poruku koju backend vraća
      }
    },
    error: (err) => {
      console.error('Greška prilikom provjere korisničkog imena', err);
    }
  });
}

  // Dodavanje listenera za promjenu emaila i korisničkog imena
  private setupListeners(): void {
    this.frmSignup.get('email')?.valueChanges.subscribe((email) => {
      if (email) {
        this.checkEmail(email); // Provjeri email na promjenu
      }
    });

    this.frmSignup.get('username')?.valueChanges.subscribe((username) => {
      if (username) {
        this.checkUsername(username); // Provjeri korisničko ime na promjenu
      }
    });
  }
}
