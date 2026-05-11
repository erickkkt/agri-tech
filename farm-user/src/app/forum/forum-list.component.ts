import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

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

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loading = true;
    this.http
      .get<ForumThread[]>(`${environment.apiBaseUrl}/api/v1/forum/threads`)
      .subscribe({
        next: data => { this.threads = data ?? []; this.loading = false; },
        error: err => { this.error = err?.message ?? 'Lỗi tải diễn đàn'; this.loading = false; }
      });
  }
}
