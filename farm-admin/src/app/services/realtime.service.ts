import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { Alert } from '../models/alert.model';
import { ApiEndPoints } from '../shared/config/api-end-points';

/**
 * SignalR client wrapper. Connects to the Farm.Api NotificationHub and
 * emits incoming "AlertCreated" events on `alert$`.
 *
 * Usage in app.component or a "after-login" effect:
 *   this.realtime.connect(() => this.oauthService.getAccessToken());
 *   this.realtime.alert$.subscribe(alert => toast(alert.title));
 */
@Injectable({ providedIn: 'root' })
export class RealtimeService {
  private connection: signalR.HubConnection | null = null;
  private alertSubject = new Subject<Alert>();
  public alert$ = this.alertSubject.asObservable();

  constructor(private api: ApiEndPoints) { }

  async connect(getAccessToken: () => string): Promise<void> {
    if (this.connection) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.api.notificationHub(), {
        accessTokenFactory: () => getAccessToken()
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this.connection.on('AlertCreated', (alert: Alert) => this.alertSubject.next(alert));

    try {
      await this.connection.start();
    } catch (err) {
      console.error('[RealtimeService] Failed to start hub:', err);
      this.connection = null;
    }
  }

  async joinFarmGroup(farmId: string): Promise<void> {
    if (!this.connection) return;
    try { await this.connection.invoke('JoinFarmGroup', farmId); } catch (e) { console.warn(e); }
  }

  async leaveFarmGroup(farmId: string): Promise<void> {
    if (!this.connection) return;
    try { await this.connection.invoke('LeaveFarmGroup', farmId); } catch (e) { console.warn(e); }
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return;
    try { await this.connection.stop(); } catch { }
    this.connection = null;
  }
}
