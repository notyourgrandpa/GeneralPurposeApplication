import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  constructor(public router: Router, private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.init();
  }

  public isExpanded = false;

  public toggleMenu() {
    this.isExpanded = !this.isExpanded;
  }

  isLoginRoute(): boolean {
    return this.router.url === '/login';
  }
}
