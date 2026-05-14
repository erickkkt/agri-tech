import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { AuthService } from '../services/auth.service';

type Tab = 'login' | 'register';

/**
 * Login / register form for the marketplace web-user app.
 *
 * - Two tabs: "Đăng nhập" and "Đăng ký" — share one route /login for simplicity.
 * - Social buttons (Google / Facebook) are placeholders for MVP. Wire real OAuth
 *   later by injecting the provider SDK and calling /auth/social-login.
 * - On success: navigate to returnUrl (from query string) or fall back to /.
 */
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: false
})
export class LoginComponent implements OnInit {

  tab: Tab = 'login';

  // login form
  loginEmail = '';
  loginPassword = '';

  // register form
  regEmail = '';
  regPassword = '';
  regConfirm = '';
  regDisplayName = '';

  submitting = false;
  error: string | null = null;

  private returnUrl: string = '/';

  constructor(
    private readonly auth: AuthService,
    private readonly route: ActivatedRoute,
    private readonly router: Router
  ) { }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';

    // Already logged in? Bounce immediately.
    if (this.auth.hasValidToken()) {
      this.router.navigateByUrl(this.returnUrl);
    }
  }

  setTab(t: Tab): void {
    this.tab = t;
    this.error = null;
  }

  async submitLogin(): Promise<void> {
    this.error = null;
    if (!this.loginEmail.trim() || !this.loginPassword) {
      this.error = 'Vui lòng nhập email và mật khẩu.';
      return;
    }
    this.submitting = true;
    try {
      await this.auth.login(this.loginEmail.trim(), this.loginPassword);
      this.router.navigateByUrl(this.returnUrl);
    } catch (e: any) {
      this.error = e?.error?.error ?? 'Đăng nhập thất bại.';
    } finally {
      this.submitting = false;
    }
  }

  async submitRegister(): Promise<void> {
    this.error = null;
    if (!this.regEmail.trim() || !this.regPassword) {
      this.error = 'Email và mật khẩu là bắt buộc.';
      return;
    }
    if (this.regPassword.length < 6) {
      this.error = 'Mật khẩu phải có ít nhất 6 ký tự.';
      return;
    }
    if (this.regPassword !== this.regConfirm) {
      this.error = 'Xác nhận mật khẩu không khớp.';
      return;
    }
    this.submitting = true;
    try {
      await this.auth.register({
        email: this.regEmail.trim(),
        password: this.regPassword,
        displayName: this.regDisplayName.trim() || undefined
      });
      this.router.navigateByUrl(this.returnUrl);
    } catch (e: any) {
      this.error = e?.error?.error ?? 'Đăng ký thất bại.';
    } finally {
      this.submitting = false;
    }
  }

  socialLogin(provider: 'google' | 'facebook'): void {
    // MVP placeholder — wire real OAuth later (Google Identity Services / Facebook SDK).
    this.error = `Đăng nhập bằng ${provider === 'google' ? 'Google' : 'Facebook'} sẽ ra mắt sớm.`;
  }
}
