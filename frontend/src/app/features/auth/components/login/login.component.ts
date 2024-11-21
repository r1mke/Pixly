import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'; 
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgbModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  public frmLogin: FormGroup;
  public showPassword: boolean = false;

  constructor(private fb: FormBuilder, private router: Router) {
    this.frmLogin = this.createLoginForm();
  }

  createLoginForm(): FormGroup {
    return this.fb.group({
      email: [null, [Validators.required, Validators.email]],
      password: [null, [Validators.required]],
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  submit() {
    if (this.frmLogin.valid) {
      console.log(this.frmLogin.value);
      this.router.navigate(['/home']);
    } else {
      console.log('Form is invalid');
    }
  }
}
