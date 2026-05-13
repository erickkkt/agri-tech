import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommitmentDto, InvestmentService } from '../../services/investment.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-investment-commitment',
  templateUrl: './investment-commitment.component.html',
  styleUrls: ['./investment-commitment.component.css'],
  standalone: false
})
export class InvestmentCommitmentComponent implements OnInit {
  commitment: CommitmentDto | null = null;
  loading = true;
  error: string | null = null;
  copied: string | null = null;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly investmentService: InvestmentService,
    private readonly authService: AuthService
  ) {}

  async ngOnInit(): Promise<void> {
    if (!this.authService.hasValidToken()) {
      this.authService.login(this.router.url);
      return;
    }
    const orderId = this.route.snapshot.paramMap.get('id');
    if (!orderId) { this.error = 'Order id thiếu trong URL'; this.loading = false; return; }
    try {
      this.commitment = (await this.investmentService.getCommitment(orderId)) ?? null;
      if (!this.commitment) this.error = 'Không tìm thấy đơn đầu tư';
    } catch (e: any) {
      this.error = e?.error?.error ?? e?.message ?? 'Không tải được cam kết';
    } finally {
      this.loading = false;
    }
  }

  /** Suggested transfer note that includes the unique reference for reconciliation. */
  get transferNote(): string {
    if (!this.commitment) return '';
    return `DT ${this.commitment.transferReference} ${this.commitment.animal.code}`;
  }

  async copy(text: string, key: string): Promise<void> {
    try {
      await navigator.clipboard.writeText(text);
      this.copied = key;
      setTimeout(() => { if (this.copied === key) this.copied = null; }, 1500);
    } catch {
      // Clipboard may not be available (insecure context). Fallback: select & alert.
      window.prompt('Copy thủ công:', text);
    }
  }

  print(): void { window.print(); }
}
