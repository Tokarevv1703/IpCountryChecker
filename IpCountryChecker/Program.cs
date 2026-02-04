using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class IpData
{
    public string ip { get; set; }
    public string country { get; set; }
    public string city { get; set; }
}

class Program
{
    static async Task<IpData> GetIpData(string ip)
    {
        using HttpClient client = new HttpClient();
        string url = $"https://ipinfo.io/{ip}/json";

        string json = await client.GetStringAsync(url);

        IpData data = JsonConvert.DeserializeObject<IpData>(json);

        return data;
    }

    static async Task Main()
    {
        Console.WriteLine("Загрузка IP из файла ips.txt...\n");

        string[] ips = File.ReadAllLines("ips.txt");

        List<IpData> results = new List<IpData>();

        foreach (string ip in ips)
        {
            Console.WriteLine($"Запрос: {ip}");

            try
            {
                IpData data = await GetIpData(ip);
                results.Add(data);
            }
            catch
            {
                Console.WriteLine($"Ошибка при запросе {ip}");
            }
        }

        Console.WriteLine("\n=== Страны в файле ===");

        var grouped = results
            .GroupBy(x => x.country)
            .Select(g => new { Country = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count);

        foreach (var item in grouped)
        {
            Console.WriteLine($"{item.Country}: {item.Count}");
        }

        var topCountry = grouped.First().Country;

        Console.WriteLine($"\nСтрана с максимальным количеством IP: {topCountry}");

        Console.WriteLine("\nГорода этой страны:");

        var cities = results
            .Where(x => x.country == topCountry)
            .Select(x => x.city)
            .Distinct();

        foreach (var city in cities)
        {
            Console.WriteLine(city);
        }
    }
}
