using AspNetGrpc.V1.BidirectionalStreaming;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetGrpc.Server.Services
{
    public class BidirectionalStreamingService : V1.BidirectionalStreaming.Message.MessageBase
    {
        public override async Task SayHello(IAsyncStreamReader<MessageRequest> requestStream, IServerStreamWriter<MessageResponse> responseStream, ServerCallContext context)
        {
            Task response = Task.Run(async () =>
            {
                for(int i = 0;i<10;i++)
                {
                    var date = DateTime.Now.ToString();
                    Console.WriteLine("BidirectionalStreaming Server Message Sended: " + date);
                    await responseStream.WriteAsync(new MessageResponse { Message = date });
                    await Task.Delay(5000);
                }
            });

            Task request = Task.Run(async () =>
            {
                while (await requestStream.MoveNext())
                {                   
                    Console.WriteLine("BidirectionalStreaming Client Message Received: " + requestStream.Current.Message);
                }
            });

            await response;
            await request;

        }
    }
}
