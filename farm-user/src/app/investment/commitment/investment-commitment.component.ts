import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
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
  exporting = false;

  @ViewChild('contractRef') contractRef?: ElementRef<HTMLElement>;

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

  /**
   * Render the contract section to PDF via html2canvas + jsPDF.
   * Multi-page handled by slicing the canvas image into A4-sized chunks.
   */
  async exportPdf(): Promise<void> {
    if (!this.contractRef || !this.commitment) return;
    this.exporting = true;
    try {
      const el = this.contractRef.nativeElement;
      const canvas = await html2canvas(el, {
        scale: 2,
        useCORS: true,
        backgroundColor: '#ffffff'
      });
      const imgData = canvas.toDataURL('image/jpeg', 0.92);

      const pdf = new jsPDF({ unit: 'mm', format: 'a4', orientation: 'portrait' });
      const pageWidthMm = pdf.internal.pageSize.getWidth();   // 210
      const pageHeightMm = pdf.internal.pageSize.getHeight(); // 297
      const imgWidthMm = pageWidthMm - 20;                    // 10mm margin each side
      const imgHeightMm = (canvas.height * imgWidthMm) / canvas.width;

      let heightLeft = imgHeightMm;
      let position = 10;

      pdf.addImage(imgData, 'JPEG', 10, position, imgWidthMm, imgHeightMm);
      heightLeft -= (pageHeightMm - 20);

      while (heightLeft > 0) {
        position = heightLeft - imgHeightMm + 10;
        pdf.addPage();
        pdf.addImage(imgData, 'JPEG', 10, position, imgWidthMm, imgHeightMm);
        heightLeft -= (pageHeightMm - 20);
      }

      pdf.save(`Cam-ket-dau-tu-${this.commitment.transferReference}.pdf`);
    } catch (e) {
      console.error('PDF export failed', e);
      window.alert('Không tạo được PDF. Vui lòng dùng nút In thay thế.');
    } finally {
      this.exporting = false;
    }
  }
}
