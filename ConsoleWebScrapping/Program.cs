using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace ConsoleWebScrapping
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            var urlBase = "https://www.fundsexplorer.com.br/funds";
            var listTikers = ListTickersCode(urlBase);
            var listTickersData = ListTickersData(urlBase, listTikers);
            
            foreach (var ticker in listTickersData)
                Console.WriteLine(ticker);

            var endTime = DateTime.Now;
            var tookTime = endTime - startTime;
            Console.WriteLine("FIIs listados no Funds Explorer: {0}", listTikers.Count);
            Console.WriteLine("Páginas de FIIs encontradas: {0} ", listTickersData.Count);
            Console.WriteLine("Duração da pesquisa: {0}", tookTime);
            Console.WriteLine();
        }

        static HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);
            return document;
        }

        static List<string> ListTickersCode(string url)
        {
            var Tickers = new List<string>();
            HtmlDocument document = GetDocument(url);
            HtmlNodeCollection tickerNodes = document.DocumentNode.SelectNodes("//select/option");
            foreach (var node in tickerNodes)
            {
                var ticker = node.Attributes["value"].Value;
                Tickers.Add(ticker);
            }
            return Tickers;
        }
        
        static List<string> ListTickersData(string url, List<string> tickers)
        {
            var tickersData = new List<string>();
            foreach (string ticker in tickers)
            {
                var tickerUrl = $"{url}/{ticker.Trim()}";
                var tickerDocument = GetDocument(tickerUrl);
                var tickerData = GetTickerData(tickerDocument);
                tickersData.Add(tickerData);
            }
            return tickersData;
        }

        static string GetTickerData(HtmlDocument tickerDocument)
        {
            var tickerData = string.Empty;
            var tickerCodePath = "//div[@class=\"ticker-wrapper\"]/h1";
            var pricePath = "//span[@class=\"price\"]";

            if (tickerDocument.DocumentNode.SelectSingleNode(tickerCodePath) is not null)
            {
                var tickerCode = tickerDocument.DocumentNode.SelectSingleNode(tickerCodePath).InnerText;
                var price = tickerDocument.DocumentNode.SelectSingleNode(pricePath).InnerText;
                var text = $"{tickerCode} - {price.Replace("\n", "").Trim()}";

                tickerData = text;
            }
            return tickerData;
        }
    }
}
