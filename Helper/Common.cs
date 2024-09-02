using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text;
using Serilog;
using Microsoft.Extensions.Configuration;
using webapi_DLoadFile.service;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;

namespace webapi_DLoadFile.Helper
{
    public  class Common
    {
        private readonly ILogger<Common> _logger = null;

        public  Common( ILogger<Common> loggerInput)
        {
            _logger = loggerInput;
        }
        public static string GetCurrentDirectory()
        {
            var result = Directory.GetCurrentDirectory();
            return result;
        }
        public static string GetStaticContentDirectory()
        {
            var result = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\StaticContent\\");
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
            return result;
        }
        public static string GetFilePath(string FileName)
        {
            var _GetStaticContentDirectory = GetStaticContentDirectory();
            var result = Path.Combine(_GetStaticContentDirectory, FileName);
            return result;
        }
        public static string base64Decode(string inputString)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(inputString));
        }

        public  async Task<MemoryStream> CWTIntenalGetFile(string uncFile)
        {
            var stream = new MemoryStream();
            var myConfig= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string baseaddr= myConfig.GetValue<string>("ServicesBaseAddress:InternalDownload")??"";
            if (baseaddr =="")
            {
               TypedResults.BadRequest(new StreamWriter(stream));
               throw new ArgumentException( $"File not found.", "File requested");
            }
            //throw new HttpResponseException(HttpStatusCode.NotFound);

            _logger.LogInformation($"CWTIntenalGetFile \"{uncFile}\"");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddr);
                Uri myuri = new Uri($"{client.BaseAddress}{uncFile}");

                Stream file = await client.GetStreamAsync(myuri).ConfigureAwait(false);
                await file.CopyToAsync(stream);
                TypedResults.Ok();
            }
            return  stream;
        }

        internal MemoryStream test(string uncFile)
        {
            var memoryStream = new MemoryStream();
            var myConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string baseaddr = myConfig.GetValue<string>("ServicesBaseAddress:InternalDownload");

            _logger.LogInformation($"test \"{uncFile}\"");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddr);
                Uri myuri = new Uri($"{client.BaseAddress}{uncFile}");
                Console.Write(myuri);
                _logger.LogInformation($"Uri myuri \"{myuri}\"");
                Stream file = client.GetStreamAsync(myuri).GetAwaiter().GetResult();//.ConfigureAwait(false);
                file.CopyToAsync(memoryStream).GetAwaiter().GetResult();
            }
            _logger.LogInformation($"test memoryStream: \"{memoryStream.Length}\"");
            return memoryStream;
        }

        public async Task<MemoryStream> CwtInternalFileGet(string uncFile)
        {
            var stream = new MemoryStream();
            var myConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string baseaddr = myConfig.GetValue<string>("ServicesBaseAddress:InternalDownload");

            _logger.LogInformation($"CWTIntenalGetFile \"{uncFile}\"");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddr);
                Uri myuri = new Uri($"{client.BaseAddress}{uncFile}");
                _logger.LogInformation($"client.BaseAddress \"{myuri}\"");
                Stream file = await client.GetStreamAsync(myuri).ConfigureAwait(false);
                file.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                //using (FileStream stream = System.IO.File.OpenRead(filePath))
                //{
                //    return new FileStreamResult(stream, "text/html");
                //}

            }
            return stream;

            //using var ms = new MemoryStream();
            //using var writer = new StreamWriter(ms);
            //writer.WriteLine("my content");
            //writer.Flush();
            //memoryStream.Position = 0;
            //return File(ms, "text/plain");
        }
    }
}
