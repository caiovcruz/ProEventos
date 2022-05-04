import { Component, OnInit } from '@angular/core';
import { NavigationEnd, NavigationStart, Router } from '@angular/router';
import { AccountService } from '@app/services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent implements OnInit {
  public isCollapsed = true;
  public usuarioLogado = false;

  constructor(public accountService: AccountService, private router: Router) {
    router.events.subscribe((val) => {
      if (val instanceof NavigationEnd) {
        this.accountService.currentUser$.subscribe(
          (value) => (this.usuarioLogado = value !== null)
        );
      }
    });
  }

  ngOnInit(): void {}

  logout(): void {
    this.accountService.logout();
    this.router.navigateByUrl('/user/login');
  }

  public showMenu(): boolean {
    return this.router.url !== '/user/login';
  }
}
