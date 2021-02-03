using AspNetGrpc.V1.ClientStreaming;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetGrpc.Client
{
    class Program
    {
        private static readonly Random _random = new Random();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Started");
            using var channel = GrpcChannel.ForAddress("https://localhost:5041");


            await UnaryCallExample(channel);


            await ClientStreamingCallExample(channel);

            await ServerStreamingCallExample(channel);
            await BidirectionalStreamingCallExample(channel);
            Console.WriteLine("Stoppped");
            Console.ReadKey();


            //var httpHandler = new HttpClientHandler();
            //// Return `true` to allow certificates that are untrusted/invalid
            //httpHandler.ServerCertificateCustomValidationCallback =
            //    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            //var channel = GrpcChannel.ForAddress("https://localhost:5001",
            //    new GrpcChannelOptions { HttpHandler = httpHandler });
            //var client = new Greet.GreeterClient(channel);



            //// This switch must be set before creating the GrpcChannel/HttpClient.
            //AppContext.SetSwitch(
            //    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //// The port number(5000) must match the port of the gRPC server.
            //var channel = GrpcChannel.ForAddress("http://localhost:5000");
            //var client = new Greet.GreeterClient(channel);
        }
        private static async Task UnaryCallExample(GrpcChannel grpcChannel)
        {
            var unaryClient = new V1.Unary.Hello.HelloClient(grpcChannel);
            var helloResponse = await unaryClient.SayHelloAsync(new V1.Unary.HelloRequest() { Name = Guid.NewGuid().ToString("N") });
            Console.WriteLine(helloResponse.Message);
        }

        private static async Task ClientStreamingCallExample(GrpcChannel grpcChannel)
        {
            var clientStreamingClient = new V1.ClientStreaming.Sum.SumClient(grpcChannel);
            using var call = clientStreamingClient.Increase();
            for (var i = 0; i < 3; i++)
            {
                var number = _random.Next(19);
                Console.WriteLine($"ClientStreaming Request {number}");
                await call.RequestStream.WriteAsync(new SumRequest { Number = number });
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            await call.RequestStream.CompleteAsync();

            var response = await call;
            Console.WriteLine($"ClientStreaming Response: {response.Total}");
        }
        private static async Task ServerStreamingCallExample(GrpcChannel grpcChannel)
        {
            var serverStreamingClient = new V1.ServerStreaming.Countdown.CountdownClient(grpcChannel);
            using var call = serverStreamingClient.Print(new V1.ServerStreaming.CountdownRequest() { Seconds = _random.Next(5, 15) });

            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"ServerStreaming Countdown: {message.Message}");
            }
        }
        private static async Task BidirectionalStreamingCallExample(GrpcChannel grpcChannel)
        {
            var bidirectionalClient = new V1.BidirectionalStreaming.Message.MessageClient(grpcChannel);
            var call = bidirectionalClient.SayHello();

            Task request = Task.Run(async () =>
            {
                for(int i = 0;i<10;i++)
                {
                    var date = DateTime.Now.ToString();
                    Console.WriteLine("BidirectionalStreaming Client Message Sended: " + date);
                    await call.RequestStream.WriteAsync(new V1.BidirectionalStreaming.MessageRequest { Message = date });
                    await Task.Delay(1000);
                }
            });

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task response = Task.Run(async () =>
            {
                while (await call.ResponseStream.MoveNext(cancellationTokenSource.Token))
                    Console.WriteLine("BidirectionalStreaming Server Message Received: " + call.ResponseStream.Current.Message);
            });
            await request;
            await call.RequestStream.CompleteAsync();
            await response;

        }
    }
}
