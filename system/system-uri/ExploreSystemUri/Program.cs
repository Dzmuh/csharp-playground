using System;

namespace SystemUri
{
    class Program
    {
        static void Main(string[] args)
        {
            switch (args[1])
            {
                case "UriIntro":
                    UriIntro();
                    break;
                case "UrlParser":
                    UrlParser();
                    break;
            }
        }

        static void UriIntro(){

            #region UriIntro
            Uri uri1 = new Uri("http://WWW.DZMUH.COM:80/portfolio/generic.asp?pageid=666&section=Details#Links");
            Console.WriteLine(uri1.ToString());
            Console.WriteLine(uri1.OriginalString);
            #endregion UriIntro

        }

        #region UrlParser

        static void UrlParser()
        {
            ParseUrl("http://dxczjjuegupb.cloudfront.net/wp-content/uploads/2017/10/Оуэн-Мэтьюс.jpg");
        }

        static void ParseUrl(string url)
        {
            var u = new Uri(url);
            Console.WriteLine("URL:         {0}", u.AbsoluteUri);
            Console.WriteLine("Scheme:      {0}", u.Scheme);
            Console.WriteLine("Host:        {0}", u.DnsSafeHost);
            Console.WriteLine("Port:        {0}", u.Port);
            Console.WriteLine("Path:        {0}", u.LocalPath);
            Console.WriteLine("Query:       {0}", u.Query);
            Console.WriteLine("Fragment:    {0}", u.Fragment);
            Console.WriteLine();
        }

        #endregion UrlParser
    }
}
