import { Component, OnInit, Output, EventEmitter, OnDestroy, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { map, shareReplay } from 'rxjs/operators';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { AuthService } from '../auth/auth.service';
import { User } from "../auth/user.model";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
  animations: [
    trigger('slideInOut', [
      transition(':enter', [
        style({ height: '0px', overflow: 'hidden' }),
        animate('300ms ease-in-out', style({ height: '*' }))
      ]),
      transition(':leave', [
        style({ height: '*', overflow: 'hidden' }),
        animate('300ms ease-in-out', style({ height: '0px' }))
      ])
    ])
  ]
})
export class NavMenuComponent implements OnInit, OnDestroy {
  private destroySubject = new Subject<boolean>();
  
  isLoggedIn = false;
  isMinimized = false;
  activeItem = 'dashboard';
  user: User | null = null;
  openSubmenu: string | null = null;
  appTitle = 'Your App Name';

  // Responsive breakpoint detection
  isHandset$ = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );

  menu = [
    { 
      label: 'Fetch-data', 
      icon: 'dashboard', 
      route: '/fetch-data' 
    },
    { 
      label: 'Products', 
      icon: 'inventory_2', 
      children: [
        { label: 'All Products', route: '/products' },
        { label: 'Add Product', route: '/products/add' }
      ] 
    },
    { 
      label: 'Categories', 
      icon: 'category', 
      route: '/categories' 
    }
  ];

  @Output() sidenavToggled = new EventEmitter<boolean>();

  constructor(
    private authService: AuthService, 
    private router: Router,
    private breakpointObserver: BreakpointObserver
  ) {
    this.authService.authStatus
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
        this.user = result ? this.authService.getCurrentUser() : null;
      });
  }

  ngOnInit(): void {
    const savedState = localStorage.getItem('navMinimized');
    if (savedState) {
      this.isMinimized = JSON.parse(savedState);
    }
    
    this.isLoggedIn = this.authService.isAuthenticated();
    this.user = this.isLoggedIn ? this.authService.getCurrentUser() : null;
  }

  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }

  toggleMinimize(): void {
    this.isMinimized = !this.isMinimized;
    localStorage.setItem('navMinimized', JSON.stringify(this.isMinimized));
    this.sidenavToggled.emit(this.isMinimized);
    
    // Close submenus when minimizing
    if (this.isMinimized) {
      this.openSubmenu = null;
    }
  }

  setActiveItem(item: string): void {
    this.activeItem = item;
  }

  toggleSubmenu(label: string): void {
    if (this.isMinimized) return;
    this.openSubmenu = this.openSubmenu === label ? null : label;
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(["/login"]);
  }

  onLogin(): void {
    this.router.navigate(["/login"]);
  }
}
