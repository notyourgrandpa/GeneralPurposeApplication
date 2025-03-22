import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  isMinimized = false;
  activeItem = 'dashboard';

  @Output() sidenavToggled = new EventEmitter<boolean>();

  constructor(private router: Router) { }

  ngOnInit(): void {
    // Initialize from localStorage if available
    const savedState = localStorage.getItem('navMinimized');
    if (savedState) {
      this.isMinimized = JSON.parse(savedState);
    }
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

  logout(): void {
    this.router.navigate(['/login']);
  }
}
