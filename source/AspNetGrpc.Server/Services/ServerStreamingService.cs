using AspNetGrpc.V1.ServerStreaming;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetGrpc.Server.Services
{
    public class ServerStreamingService : Countdown.CountdownBase
    {
        public override async Task Print(CountdownRequest request, IServerStreamWriter<CountdownResponse> responseStream, ServerCallContext context)
        {
            for (var i = request.Seconds; i >= 0; i--)
            {
                await responseStream.WriteAsync(new CountdownResponse { Message = i });
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
