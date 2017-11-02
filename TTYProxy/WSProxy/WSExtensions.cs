using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace TTYProxy.WSProxy
{
    public static class WSExtensions
    {
        public static Task BindStreams(this WebSocket socket1, WebSocket socket2, CancellationToken token)
        {
            // Copy streams both ways. 
            // Replace with an ReplaySubject on source. 
            var t1 = Task.Run(async () => await socket1.CopyAsync(socket2, token));
            var t2 = Task.Run(async () => await socket2.CopyAsync(socket1, token));
            return Task.WhenAll(t1, t2);
        }

        public static async Task CopyAsync(this WebSocket src, WebSocket dst, CancellationToken token)
        {
            var buffer = new byte[1024 * 4];
            while (true)
            {
                var result = await src.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                if (!result.CloseStatus.HasValue)
                {
                    await dst.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count),
                                        result.MessageType,
                                        result.EndOfMessage,
                                        CancellationToken.None);
                }
                else
                {
                    await dst.CloseAsync(result.CloseStatus.Value, 
                                        result.CloseStatusDescription, 
                                        CancellationToken.None);
                    break;
                }       
            }
        }
    }
}
