import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AuthService } from './auth/auth.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  isLoginRoute = false;
  constructor(public router: Router, private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.init();

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event) => {
        const navEndEvent = event as NavigationEnd;
        this.isLoginRoute = navEndEvent.urlAfterRedirects.startsWith('/login');
      });
  }

  public isExpanded = false;

  public toggleMenu() {
    this.isExpanded = !this.isExpanded;
  }
}
