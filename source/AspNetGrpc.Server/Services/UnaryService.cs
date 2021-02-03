using AspNetGrpc.V1.Unary;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetGrpc.Server.Services
{
    public class UnaryService : Hello.HelloBase
    {
        public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloResponse() { Message = $"Unary Request: {request.Name} - Response: {DateTime.Now.ToString()}"  });
        }
    }
}
