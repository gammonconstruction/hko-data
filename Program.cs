// See https://aka.ms/new-console-template for more information
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
IConfiguration config = builder.Build();

RainfallRadorDownload(config);


void RainfallRadorDownload(IConfiguration configuration)
{
    var urlTemplate = configuration["UrlTemplates:RainfallRadar"];
    var rootFolder = configuration["Storage:RainfallRadar"];

    var lastHour = DateAndTime.Today.AddHours(DateTime.Now.AddHours(-1).Hour);
    Console.WriteLine(lastHour.ToString());

    var folder = $"{rootFolder}{lastHour.ToString("yyyy-MM-dd")}";

    if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);

    var client = new WebClient();

    for(var i = 0; i < 60; i += 6)
    {
        var url = string.Format(urlTemplate, lastHour.AddMinutes(i).ToString("yyyyMMddHHmm"));
        Console.WriteLine($"Download from {url}");
        byte[] imageData = client.DownloadData(url);
        var fileName = $"{lastHour.AddMinutes(i).ToString("yyyyMMddHHmm")}.jpg";

        using(var ms = new MemoryStream(imageData))
        {
            using(var fs = new FileStream($"{folder}/{fileName}", FileMode.Create))
            {
                ms.WriteTo(fs);
            }
        }
    }
}

