using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebhookReciver
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://127.0.0.1:8080/webhook/");

                listener.Start();

                _ = Task.Run(
                    async () =>
                    {
                        while (listener.IsListening)
                        {
                            var context = await listener.GetContextAsync();

                            var request = context.Request;
                            var response = context.Response;

                            try
                            {
                                if (request.HttpMethod != "POST" )
                                {
                                    response.StatusCode = 204;
                                }
                                else
                                {
                                    if (request.HasEntityBody)
                                    {
                                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                                        {
                                            JObject jsonObject = JObject.Parse(reader.ReadToEnd());
                                            string? value = (string?)jsonObject["zen"];
                                            Console.WriteLine(value);
                                            Console.WriteLine(request.Url);
                                        }
                                    }

                                    response.ContentType = "application/json";

                                    using (var writer = new StreamWriter(response.OutputStream))
                                    {
                                        writer.Write("{\"status\" : \"Recived!!\"}");
                                    }
                                }
                            }
                            finally
                            {
                                response.Close();
                            }
                        }
                    });

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(false);
            }
        }
    }
}
