import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot
} from '@angular/router';
import { AuthService } from './auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

export const AuthGuard: CanActivateFn = (
  next: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService: AuthService = inject(AuthService);
  const router: Router = inject(Router);
  const snackBar: MatSnackBar = inject(MatSnackBar);
  // If the user is authenticated, return true...
  if (authService.isAuthenticated()) {
    return true;
  }
  // ... otherwise, redirects to the login page
  snackBar.open('Please log in to continue.', 'Close', {
    duration: 5000
  });
  return router.createUrlTree(['/login'], {
    queryParams: {
      returnUrl: state.url
    }
  });
};
