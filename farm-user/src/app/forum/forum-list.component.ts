import { Component, OnInit } from '@angular/core';
import { HttpBaseService } from '../shared/services/http-base.service';
import { ApiEndPoints } from '../shared/config/api-end-points';

interface ForumThread {
  id: string;
  title: string;
  category: string;
  authorUserName: string;
  replyCount: number;
  lastReplyAt?: Date;
  createdAt: Date;
}

const CATEGORY_COLORS: Record<string, string> = {
  'Kỹ thuật': 'info',
  'Phòng bệnh': 'danger',
  'Kinh nghiệm': 'success',
  'Khai thác': 'warning'
};

@Component({
  selector: 'app-forum-list',
  templateUrl: './forum-list.component.html',
  styleUrls: ['./forum-list.component.css'],
  standalone: false
})
export class ForumListComponent implements OnInit {
  threads: ForumThread[] = [];
  loading = false;
  error: string | null = null;
  activeCategory: string = 'all';

  constructor(
    private readonly http: HttpBaseService,
    private readonly api: ApiEndPoints
  ) {}

  async ngOnInit(): Promise<void> {
    this.loading = true;
    try {
      this.threads = (await this.http.getDataAsync<ForumThread[]>(this.api.getForumThreads())) ?? [];
    } catch (err: any) {
      this.error = err?.message ?? 'Lỗi tải diễn đàn';
    } finally {
      this.loading = false;
    }
  }

  get filteredThreads(): ForumThread[] {
    if (this.activeCategory === 'all') return this.threads;
    return this.threads.filter(t => t.category === this.activeCategory);
  }

  get categories(): string[] {
    const set = new Set(this.threads.map(t => t.category).filter(Boolean));
    return Array.from(set);
  }

  badgeClass(category: string): string {
    return CATEGORY_COLORS[category] ?? '';
  }

  initial(name: string): string {
    return (name || '?').trim().charAt(0).toUpperCase();
  }

  /** Convert author name to a deterministic HSL color for avatar background */
  avatarColor(name: string): string {
    let hash = 0;
    for (let i = 0; i < (name || '').length; i++) hash = (hash << 5) - hash + name.charCodeAt(i);
    const hue = Math.abs(hash) % 360;
    return `hsl(${hue}, 65%, 55%)`;
  }
}
