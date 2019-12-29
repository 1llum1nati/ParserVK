using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;

namespace trash
{
    public class Text
    {
        public string ID { get; set; }
        public string PostText { get; set; }
    }
    public class Image
    {
        public string ID { get; set; }
        public string Img { get; set; }
    }

    public class AllThumbs
    {
        public string ID { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public string GIF { get; set; }
        public string Doc { get; set; }
        public string Article { get; set; }
        public string Poll { get; set; }
        public string ThumbedLink { get; set; }
        public string Geotag { get; set; }
        public string Poster { get; set; }
        public string MediaThumbedLink { get; set; }
    }

    public class MainClass 
    {
        static string pathDB = "/home/r3pl1c4nt/Docs/TestDB.db";
        static string connectionString = String.Format("Data Source={0};Version=3;", pathDB);
        static int counterText = 0, counterImg = 0, counterAllThumbs = 0, flagText = 0, flagImg = 0, flagAll = 0;

        public static void Main(string[] args)
        {
            NamedPipeServerStream pipeStream = new NamedPipeServerStream("pipes");
            Console.WriteLine("[Server] Pipe created {0}", pipeStream.GetHashCode());
            pipeStream.WaitForConnection();
            Console.WriteLine("[Server] Pipe connection established");
            StreamReader sr = new StreamReader(pipeStream);
            StreamWriter sw = new StreamWriter(pipeStream)
            {
                AutoFlush = true
            };
            CreateDB();
            Thread write1 = new Thread(() => Texxt());
            Thread write2 = new Thread(() => Imagge());
            Thread write3 = new Thread(() => AllThumbbs());
            write1.Start();
            write2.Start();
            write3.Start();
            while (true)
            {
                if(!pipeStream.IsConnected)
                {
                    break;
                }
                string temp = sr.ReadLine();

                if (temp == "text")
                {
                    flagText = 1;
                    while (flagText == 1)
                        Thread.Sleep(100);
                    sw.WriteLine("end");
                }
                if (temp == "img")
                {
                    flagImg = 1;
                    while (flagImg == 1)
                        Thread.Sleep(100);
                    sw.WriteLine("end");
                }

                if (temp == "allThumbs")
                {
                    flagAll = 1;
                    while (flagAll == 1)
                        Thread.Sleep(100);
                    sw.WriteLine("end");
                }

            }

        
            Console.WriteLine("Connection lost");
        }

        static void CreateDB()
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (File.Exists(pathDB))
                File.Delete(pathDB);

            SqliteConnection.CreateFile(pathDB);

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                string commandIDTEXT = "CREATE TABLE idText (id TEXT, text TEXT)",
                       commandIDIMG = "CREATE TABLE idIMG (id TEXT, img TEXT)",
                       commandALLTHUMBS = "CREATE TABLE allThumbs (id TEXT, audios TEXT, videos TEXT, GIF TEXT, doc TEXT," +
                        "article TEXT, poll TEXT, tLink TEXT, geotag TEXT, poster TEXT, mtLink TEXT)";
                connection.Open();
                using (SqliteCommand liteCommand = new SqliteCommand(commandIDTEXT, connection))
                {
                    liteCommand.ExecuteNonQuery();
                    liteCommand.CommandText = commandIDIMG;
                    liteCommand.ExecuteNonQuery();
                    liteCommand.CommandText = commandALLTHUMBS;
                    liteCommand.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        static void Texxt()
        {
            while (true)
            {
                if (flagText == 1)
                {
                    using (SqliteConnection connection = new SqliteConnection(connectionString))
                    {
                        connection.Open();
                        List<Text> TextFromFile = new List<Text>();
                        StreamReader sr = new StreamReader(@"/home/r3pl1c4nt/Docs/idText.json");
                        string a = sr.ReadLine();
                        if (a != null)
                            TextFromFile.AddRange(JsonConvert.DeserializeObject<List<Text>>(a));
                        sr.Dispose();
                        using (SqliteCommand liteCommand = new SqliteCommand(connection))
                        {
                            for (int i = counterText; i != TextFromFile.Count; ++i)
                            {
                                liteCommand.CommandText = "INSERT INTO idText (id, text) VALUES (@ID, @PostText)";
                                liteCommand.Prepare();
                                liteCommand.Parameters.AddWithValue("@ID", TextFromFile[i].ID);
                                liteCommand.Parameters.AddWithValue("@PostText", TextFromFile[i].PostText);
                                liteCommand.ExecuteNonQuery();
                            }
                        }
                        counterText = TextFromFile.Count;
                        connection.Close();
                        flagText = 0;
                    }
                }
            }
        }

        static void Imagge()
        {
            while (true)
            {
                if (flagImg == 1)
                {
                    using (SqliteConnection connection = new SqliteConnection(connectionString))
                    {
                        connection.Open();

                        List<Image> ImgFromFile = new List<Image>();
                        StreamReader sr = new StreamReader(@"/home/r3pl1c4nt/Docs/idImg.json");
                        string a = sr.ReadLine();
                        if (a != null)
                            ImgFromFile.AddRange(JsonConvert.DeserializeObject<List<Image>>(a));
                        sr.Dispose();
                        using (SqliteCommand liteCommand = new SqliteCommand(connection))
                        {
                            for (int i = counterImg; i != ImgFromFile.Count; ++i)
                            {
                                liteCommand.CommandText = "INSERT INTO idIMG (id, img) VALUES (@ID, @Img)";
                                liteCommand.Prepare();
                                liteCommand.Parameters.AddWithValue("@ID", ImgFromFile[i].ID);
                                liteCommand.Parameters.AddWithValue("@Img", ImgFromFile[i].Img);
                                liteCommand.ExecuteNonQuery();
                            }
                        }

                        counterImg = ImgFromFile.Count;

                        connection.Close();
                        flagImg = 0;
                    }
                }
            }
        }
        static void AllThumbbs()
        {
            while (true)
            {
                if (flagAll == 1)
                {
                    using (SqliteConnection connection = new SqliteConnection(connectionString))
                    {
                        connection.Open();


                        List<AllThumbs> AllThumbsFromFile = new List<AllThumbs>();
                        StreamReader sr = new StreamReader(@"/home/r3pl1c4nt/Docs/allThumbs.json");
                        string a = sr.ReadLine();
                        if (a != null)
                            AllThumbsFromFile.AddRange(JsonConvert.DeserializeObject<List<AllThumbs>>(a));
                        sr.Dispose();
                        using (SqliteCommand liteCommand = new SqliteCommand(connection))
                        {
                            for (int i = counterAllThumbs; i != AllThumbsFromFile.Count; ++i)
                            {
                                liteCommand.CommandText = "INSERT INTO allThumbs (id, audios, videos, GIF, doc," +
                                "article, poll, tLink, geotag, poster, mtLink) VALUES (@ID, @audios, @videos, @GIF, @doc, @article, @poll," +
                                "@tLink, @geotag, @poster, @mtLink)";
                                liteCommand.Prepare();
                                liteCommand.Parameters.AddWithValue("@ID", AllThumbsFromFile[i].ID);
                                liteCommand.Parameters.AddWithValue("@audios", AllThumbsFromFile[i].Audio);
                                liteCommand.Parameters.AddWithValue("@videos", AllThumbsFromFile[i].Video);
                                liteCommand.Parameters.AddWithValue("@GIF", AllThumbsFromFile[i].GIF);
                                liteCommand.Parameters.AddWithValue("@doc", AllThumbsFromFile[i].Doc);
                                liteCommand.Parameters.AddWithValue("@article", AllThumbsFromFile[i].Article);
                                liteCommand.Parameters.AddWithValue("@poll", AllThumbsFromFile[i].Poll);
                                liteCommand.Parameters.AddWithValue("@tLink", AllThumbsFromFile[i].ThumbedLink);
                                liteCommand.Parameters.AddWithValue("@geotag", AllThumbsFromFile[i].Geotag);
                                liteCommand.Parameters.AddWithValue("@poster", AllThumbsFromFile[i].Poster);
                                liteCommand.Parameters.AddWithValue("@mtLink", AllThumbsFromFile[i].MediaThumbedLink);
                                liteCommand.ExecuteNonQuery();
                            }
                        }

                        counterAllThumbs = AllThumbsFromFile.Count;


                        connection.Close();
                        flagAll = 0;
                    }
                }
            }
        }

    }
}
