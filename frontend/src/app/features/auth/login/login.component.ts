import { Component, ElementRef, NgZone, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {

  loginForm: FormGroup;
  isLoading = false;
  hidePassword = true;
  private mouseMoveListener?: (event: MouseEvent) => void;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar,
    private el: ElementRef,
    private ngZone: NgZone
  ) {
    this.loginForm = this.fb.group({
      email:    ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit() {
    // Run outside Angular to prevent change detection on every mouse move (keeps Lighthouse score high)
    this.ngZone.runOutsideAngular(() => {
      this.mouseMoveListener = (e: MouseEvent) => {
        this.el.nativeElement.style.setProperty('--mouse-x', `${e.clientX}px`);
        this.el.nativeElement.style.setProperty('--mouse-y', `${e.clientY}px`);
      };
      window.addEventListener('mousemove', this.mouseMoveListener);
    });
  }

  ngOnDestroy() {
    if (this.mouseMoveListener) {
      window.removeEventListener('mousemove', this.mouseMoveListener);
    }
  }

  // Convenience getters — lets the HTML template access controls as `email` instead of `loginForm.get('email')`
  get email() { return this.loginForm.get('email'); }
  get password() { return this.loginForm.get('password'); }

  onSubmit(): void {
    // If the form has validation errors, don't even bother making an API call
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.isLoading = false;
        // On success, navigate to the protected dashboard
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        // Show the error message from the backend (e.g., "Invalid email or password.")
        const message = err?.error?.message ?? 'Login failed. Please try again.';
        this.snackBar.open(message, 'Dismiss', { duration: 4000, panelClass: ['error-snackbar'] });
      }
    });
  }
}
