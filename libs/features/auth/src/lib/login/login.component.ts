import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '@wealth-manager/shared/data-access';
import { Router } from '@angular/router';

@Component({
  selector: 'lib-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  //create form
  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  })


  errorMessage = '';

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.authService.login({
      email: this.loginForm.value.email!,
      password: this.loginForm.value.password!
    }).subscribe({
      next: (res) => {
        console.log('Login success', res);
        this.router.navigate(['/'])
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Login failed! Check your credentials.'
      }
    })
  }
}
