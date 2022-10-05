using System.Net.Sockets;


class Program
{
    public static int counter = 0;
    static void Main(string[] args)
    {
        TcpListener server = new TcpListener(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 5000);
        server.Start();
        int teller = 0;
        while (true)
        {
            string data = File.ReadAllText("src\\pages\\Homepagina");
            using Socket connectie = server.AcceptSocket();
            using Stream request = new NetworkStream(connectie);
            using StreamReader requestLezer = new StreamReader(request);
            string[]? regel1 = requestLezer.ReadLine()?.Split(" ");
            if (regel1 == null) return;
            (string methode, string url, string httpversie) = (regel1[0], regel1[1], regel1[2]);
            string? regel = requestLezer.ReadLine();
            int contentLength = 0;
            while (!string.IsNullOrEmpty(regel) && !requestLezer.EndOfStream)
            {
                string[] stukjes = regel.Split(":");
                (string header, string waarde) = (stukjes[0], stukjes[1]);
                if (header.ToLower() == "content-length")
                    contentLength = int.Parse(waarde);
                regel = requestLezer.ReadLine();
            }
            if (contentLength > 0)
            {
                char[] bytes = new char[(int)contentLength];
                requestLezer.Read(bytes, 0, (int)contentLength);
            }
            if(url.Length > 1)
            {
                data = setData(url);
                if (url.Equals("/Teller"))
                {
                    teller++;
                    data = "<h1>" + teller + "</h1>";
                }
                if (url.Contains("/add?"))
                {
                    data = setData("/Add");
                }
            }
            connectie.Send(System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: text/HTML\r\nContent-Length: 11\r\n\r\n"+data));
        }
    }

    public static string setData(string url)
    {
        if (File.Exists("src\\pages\\" + url))
        {
            return File.ReadAllText("src\\pages\\" + url);
        }
        return File.ReadAllText("src\\pages\\Error404");
    }

    public static string TelOP()
    {
        return "<h1>"+ counter++ +"</h1>";
    }
}
