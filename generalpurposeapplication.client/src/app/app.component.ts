import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AuthService } from './auth/auth.service';
import { filter } from 'rxjs/operators';

import { ConnectionService, ConnectionServiceOptions, ConnectionState } from 'ng-connection-service';
import { Observable, map } from 'rxjs';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  public isOffline: Observable<boolean>;

  isLoginRoute = false;
  constructor(
    public router: Router,
    private authService: AuthService,
    private connectionService: ConnectionService) {
    const options: ConnectionServiceOptions = {
      enableHeartbeat: true,
      heartbeatUrl: environment.baseUrl + 'api/heartbeat',
      heartbeatInterval: 10000
    };
    this.isOffline = this.connectionService.monitor(options)
      .pipe(map(state => !state.hasNetworkConnection || !state.
        hasInternetAccess));
  }

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
