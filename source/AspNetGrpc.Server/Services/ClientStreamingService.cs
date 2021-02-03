using AspNetGrpc.V1.ClientStreaming;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetGrpc.Server.Services
{
    public class ClientStreamingService : Sum.SumBase
    {        
        public override async Task<SumResponse> Increase(IAsyncStreamReader<SumRequest> requestStream, ServerCallContext context)
        {
            int total = 0;
            await foreach (var message in requestStream.ReadAllAsync())
            {

                total += message.Number;
            }

            return new SumResponse { Total = total };
        }
    }
}
