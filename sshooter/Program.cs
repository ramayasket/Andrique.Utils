using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Text;
using System.Web;
using Kw.Common;
using Kw.Networking;

namespace sshooter
{
    class Program
    {
        static int Main(string[] args)
        {
            var bars = HttpUtility.UrlEncode("#");

            if (args.Length < 1)
            {
                Console.WriteLine("Example: ss.exe http://site.domain/path/1.jpg [] [start] [stop] [padding]");
                return 1;
            }

            var inUrl = args[0];

            var urlParts = inUrl.Split('/');
            var imgPart = urlParts.Last();

            var imgParts = //"1.jpg"
                imgPart
                .Split('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

            var b = new StringBuilder();

            const string TOKEN = "%%N%%";

            bool inNumber = false;

            foreach (var part in imgParts)
            {
                if (part == "")
                {
                    inNumber = true;
                }
                else
                {
                    if (part.ToLower().Contains(".jpg"))
                    {
                        if (inNumber)
                        {
                            b.Append(TOKEN);
                            b.Append(part);
                        }
                    }
                    else
                    {
                        b.Append(part);
                    }
                }
            }

            var finalUrl = b.ToString();

            if (!finalUrl.Contains(TOKEN))
            {
                Console.WriteLine("Problem parsing URL {0}", inUrl);
                return 1;
            }

            int start = 1;
            int stop = 150;
            int padding = 2;

            if (args.Length > 1)
            {
                int.TryParse(args[1], out start);
            }

            if (args.Length > 2)
            {
                int.TryParse(args[2], out stop);
            }

            if (args.Length > 3)
            {
                int.TryParse(args[3], out padding);
            }

            for (int i = start; i <= stop; i++)
            {
                var log = Math.Log(i, 10);
                var ndigits = RoundUp(log);

                if (0 == ndigits)
                {
                    ndigits = 1;
                }

                if (ndigits < padding)
                {
                    ndigits = padding;
                }

                b.Clear();

                foreach (var part in urlParts)
                {
                    if (part != imgPart)
                    {
                        b.Append(part);
                        b.Append("/");
                    }
                }

                var urlBase = b.ToString();

                var nformat = string.Format("{{0:D{0}}}", ndigits);
                var iformat = finalUrl.Replace(TOKEN, nformat);

                var uformat = urlBase + iformat;

                var url = string.Format(uformat, i);

                var wc = new HttpClient();

                //wc.Proxy = new WebProxy("180.243.160.39", 8080);

                try
                {
                    var bytes = wc.DownloadBytes(url);

                    var filename = url.Split('/').Last();

                    File.WriteAllBytes(filename, bytes);

                    Console.WriteLine("Downloaded #{0}", i);
                }
                catch(Exception)
                {
                    Console.Write("Failed #{0}\r", i);

                    //Console.WriteLine("Unable to download {0}", url);
                }
            }

            return 0;
        }
        
        static int RoundUp(double d)
        {
            var l = Convert.ToInt32(d);

            if (d > l)
            {
                l++;
            }

            d = l;

            return l;
        }
    }
}
