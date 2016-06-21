using System.Threading.Tasks;
using Xunit;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using SuperSocket.ClientEngine;

namespace SuperSocket.ClientEngine.Test
{
    public class HttpTest
    {
        [Fact]
        public async Task TestGet()
        {
            var client = new EasyClient();
            
            client.Security =  new SecurityOption();
            client.Security.AllowUnstrustedCertificate = true;
            client.Security.AllowNameMismatchCertificate = true;
            
            client.Error += (s, e) =>
            {
                Console.WriteLine(e.Exception.Message);
            };
            
            var taskCompleteSrc = new TaskCompletionSource<HttpPackageInfo>();
            
            client.Initialize(new HttpReceiveFilter(), (p) =>
            {
                taskCompleteSrc.SetResult(p);
            });
            
            var ret = await client.ConnectAsync(new DnsEndPoint("github.com", 443));
            
            Assert.True(ret);
            Console.WriteLine("Get https connection established");
            
            var sb = new StringBuilder();
            
            sb.AppendLine("GET https://github.com/ HTTP/1.1");
            sb.AppendLine("Accept: text/html, application/xhtml+xml, image/jxr, */*");
            sb.AppendLine("Accept-Language: en-US,en;q=0.8,zh-Hans-CN;q=0.5,zh-Hans;q=0.3");
            sb.AppendLine("Accept-Encoding: gzip, deflate");
            sb.AppendLine("Host: github.com");
            sb.AppendLine("Connection: Keep-Alive");
            sb.AppendLine();
            
            var data = Encoding.ASCII.GetBytes(sb.ToString());
            
            client.Send(new ArraySegment<byte>(data, 0, data.Length));
            
            var response = await taskCompleteSrc.Task;
            
            Console.WriteLine(response.Body);
        }
    }
}
