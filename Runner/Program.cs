using AE.Net.Mail;
using AngleSharp;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = ConfigurationManager.AppSettings["EmailHost"];
            var username = ConfigurationManager.AppSettings["EmailUser"];
            var password = ConfigurationManager.AppSettings["EmailPassword"];
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPort"]);
            var isSSL = ConfigurationManager.AppSettings["EmailSSL"].ToLowerInvariant()=="true";

            using (var imap = new AE.Net.Mail.ImapClient(host, username, password, AE.Net.Mail.AuthMethods.Login, port, isSSL))
            {
                var msgs = imap.SearchMessages(
                  SearchCondition.Unseen().And(SearchCondition.From("kalljoel@gmail.com"))
                  );

                foreach(var msg in msgs)
                {

                    Log("Found message: " + msg.Value.Subject);

                    //get album link
                    var link = GetAlbumUrl(msg.Value.Body);
                    if(link==null)
                    {
                        Log("Could not find album link.");
                        break;
                    }

                    var imageLinks = GetImageUrls(link);

                    //Log("Marking message as read.");
                    //imap.SetFlags(Flags.Seen, msg.Value);
                }
            }

            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static void Log(string s)
        {
            Console.WriteLine(s);
        }

        private static string GetAlbumUrl(string msg)
        {
            var linkStart = msg.IndexOf("View Album");
            if (linkStart == -1)
            {
                Log("Could not find start of link in message.");
                return null;
            }
            linkStart += 12;
            var linkEnd = msg.IndexOf(">", linkStart);
            if (linkEnd == -1)
            {
                Log("Could not find end of link in message.");
                return null;
            }
            return msg.Substring(linkStart, linkEnd - linkStart);
        }


        private static string GetHttp(string url, HttpMethod method=null, string data=null)
        {
            if (method == null)
                method = HttpMethod.Get;

            var client = new WebClient();
            if (method == HttpMethod.Get)
                return client.DownloadString(url);
            else if (method == HttpMethod.Post)
                return client.UploadString(url, data);

            throw new NotSupportedException("HttpMethod not supported.");
        }

        private static IEnumerable<string> GetImageUrls(string albumLink)
        {
            albumLink = albumLink.Replace("/s/m/", "/s/m/album/");

            var albumId = albumLink.Split('/').Last();

            var config = AngleSharp.Configuration.Default.WithDefaultLoader();
            // Load the names of all The Big Bang Theory episodes from Wikipedia
            // Asynchronously get the document in a new context using the configuration
            var document = BrowsingContext.New(config).OpenAsync(albumLink).Result;

            // This CSS selector gets the desired content
            var cellSelector = "a.albumItem";
            // Perform the query to get all cells with the content
            var links = document.QuerySelectorAll(cellSelector);

            foreach(var link in links)
            {
                link.Clic
            }

            // We are only interested in the text - select it with LINQ
            var titles = links.Select(m => m.Id.Replace("albumItemA",""));

            return titles;
        }
    }
}

