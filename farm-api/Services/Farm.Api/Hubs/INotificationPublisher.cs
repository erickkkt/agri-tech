// NOTE: The INotificationPublisher interface has been moved to
// Farm.Business.Services.Interfaces.INotificationPublisher so that Hangfire jobs
// in Farm.Business can depend on it without referencing Farm.Api.
// See: Infrastructure/Farm.Business/Services/Interfaces/INotificationPublisher.cs
//
// The concrete SignalR implementation remains in this folder
// (SignalRNotificationPublisher.cs).
