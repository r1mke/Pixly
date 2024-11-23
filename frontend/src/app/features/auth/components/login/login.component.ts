import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'; 
import { Router, RouterLink } from '@angular/router';
import { LoginService } from '../../services/login.service';
import { response } from 'express';

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
  public isLoading: boolean = false;
  public loginError: string = '';

  constructor(private fb: FormBuilder, private router: Router, private loginService: LoginService) {
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

  submit(): void{
    if(this.frmLogin.valid) {
      this.isLoading = true;
      const loginData = this.frmLogin.value;
      this.loginService.loginUser(loginData).subscribe({
        next:(response) =>{
          console.log(response.message);
          this.router.navigate(['/home']);
          this.loginError = '';
        },
        error:(error) =>{
          console.error('Login failed', error);
          this.loginError = error.message;
          this.isLoading = false;
        },
        complete:()=>{
          this.isLoading = false;
        }
      });
    } 
  }
}
