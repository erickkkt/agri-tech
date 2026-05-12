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

@Component({
  selector: 'app-forum-list',
  templateUrl: './forum-list.component.html',
  standalone: false
})
export class ForumListComponent implements OnInit {
  threads: ForumThread[] = [];
  loading = false;
  error: string | null = null;

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
}
