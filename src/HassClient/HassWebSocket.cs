﻿using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace JoySoftware.HomeAssistant.Client
{
    public interface IClientWebSocketFactory
    {
        IClientWebSocket New();
    }
    /// <summary>
    /// Interface so we can test without the socket layer
    /// </summary>
    public interface IClientWebSocket : IDisposable
    {
        WebSocketState State { get; }
        WebSocketCloseStatus? CloseStatus { get; }

        Task ConnectAsync(Uri uri, CancellationToken cancel);
        Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);
        Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);

        Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);
        ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);


        Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);
        ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken);
    }

    internal class ClientWebSocketFactory : IClientWebSocketFactory
    {
        public IClientWebSocket New() => new HassWebSocket();
    }
    internal class HassWebSocket : IClientWebSocket
    {
        private readonly ClientWebSocket _ws = new System.Net.WebSockets.ClientWebSocket();

        public WebSocketState State => _ws.State;

        public WebSocketCloseStatus? CloseStatus => _ws.CloseStatus;

        public async Task ConnectAsync(Uri uri, CancellationToken cancel) => await _ws.ConnectAsync(uri, cancel);

        public async Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken) => await _ws.CloseAsync(closeStatus, statusDescription, cancellationToken);

        public async Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken) => await _ws.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
        public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken) => await _ws.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
        public async ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken) => await _ws.SendAsync(buffer, messageType, endOfMessage, cancellationToken);

        public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken) => await _ws.ReceiveAsync(buffer, cancellationToken);

        public async ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken) => await _ws.ReceiveAsync(buffer, cancellationToken);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _ws.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}