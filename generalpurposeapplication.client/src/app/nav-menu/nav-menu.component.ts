import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { User } from "../auth/user.model";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit, OnDestroy {

  private destroySubject = new Subject();
  isLoggedIn: boolean = false;

  isMinimized = false;
  activeItem = 'dashboard';
  user: User | null = null;

  @Output() sidenavToggled = new EventEmitter<boolean>();

  constructor(private authService: AuthService,
    private router: Router) {
    this.authService.authStatus
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
        this.user = result ? this.authService.getCurrentUser() : null;
      })
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(["/login"]);
  }

  onLogin(): void {
    this.router.navigate(["/login"]);
  }

  ngOnInit(): void {
    // Initialize from localStorage if available
    const savedState = localStorage.getItem('navMinimized');
    if (savedState) {
      this.isMinimized = JSON.parse(savedState);
    }
    this.isLoggedIn = this.authService.isAuthenticated();
    this.user = this.isLoggedIn ? this.authService.getCurrentUser() : null;
  }

  ngOnDestroy() {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

  toggleMinimize(): void {
    this.isMinimized = !this.isMinimized;
    // Save state to localStorage
    localStorage.setItem('navMinimized', JSON.stringify(this.isMinimized));
    // Emit event so parent components can adjust layout
    this.sidenavToggled.emit(this.isMinimized);
  }

  setActiveItem(item: string): void {
    this.activeItem = item;
  }

  //logout(): void {
  //  this.router.navigate(['/login']);
  //}
}
