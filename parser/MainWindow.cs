using System;
using Gtk;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using Mono.Data.Sqlite;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel) => Build();

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

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

    public static readonly object idTextLocker = 1, idImgLocker = 1, allLocker = 1;
    public static readonly string textPath = @"/home/r3pl1c4nt/Docs/idText.json",
                                  imgPath = @"/home/r3pl1c4nt/Docs/idImg.json",
                                  allThumbsPath = @"/home/r3pl1c4nt/Docs/allThumbs.json";
    public static bool idTextIsFree = true, idImgIsFree = true, allThumbsIsFree = true, clientIsConnected = false;
    public static int count = 2;
    public static Random rnd = new Random();
    public static string pathDB = "/home/r3pl1c4nt/Docs/TestDB.db";
    public static string connectionString = String.Format("Data Source={0};Version=3;", pathDB);

    NamedPipeClientStream pipeStream = new NamedPipeClientStream("pipes");


    private class Chrome : MainWindow
    {
        string login, password;
        public static readonly string imagesWrap = "page_post_thumb_wrap",
                               videoWrap = "page_post_thumb_video",
                               audioRow = "audio_row",
                               GIFWrap = "page_post_thumb_unsized";

        FileStream idTextFile = new FileStream(textPath, FileMode.Create, FileAccess.ReadWrite);
        FileStream idImgFile = new FileStream(imgPath, FileMode.Create, FileAccess.ReadWrite);
        FileStream allThumbsFile = new FileStream(allThumbsPath, FileMode.Create, FileAccess.ReadWrite);

        static ChromeDriver driver = new ChromeDriver(InitOptions());
        static IJavaScriptExecutor jsExecutor = driver;
        int oldCount, textCounter, imgCounter, allThumbsCounter,
            idTextWriteIteration = 19, idImgWriteIteration = 19, allThumbsWriteIteration = 19,
            idTextReadIteration = 19, idImgReadIteration = 19, allThumbsReadIteration = 19,
            daemonIdTextIteration = 19, daemonIdImgIteration = 19, daemonAllThumbsIteration = 19;

        List<IWebElement> Elements = new List<IWebElement>();
        List<string> PostID = new List<string>(),
                     TextElements = new List<string>(),
                     ImagesLinks = new List<string>(),
                     VideosLinks = new List<string>(),
                     AudiosLinks = new List<string>(),
                     GIFsLinks = new List<string>(),
                     DocumentsLinks = new List<string>(),
                     ArticlesLinks = new List<string>(),
                     ThumbedLinks = new List<string>(),
                     PollsLinks = new List<string>(),
                     Geotags = new List<string>(),
                     PostersLinks = new List<string>(),
                     MediaThumbedLinks = new List<string>();

        List<int> idTextPlanning = new List<int>(20),
                  idImgPlanning = new List<int>(20),
                  allThumbsFilePlanning = new List<int>(20);


        protected internal void Init(string log, string pass)
        {
            Thread WriteThread = new Thread(() => WriteAll());
            Thread ReadThread = new Thread(() => ReadRand());
            Thread DaemonThread = new Thread(() => Daemon());

            Thread planningThread = new Thread(() => Planning());

            for (int i = 0; i != 20; ++i)
            {
                idTextPlanning.Add(rnd.Next(count));
                idImgPlanning.Add(rnd.Next(count));
                allThumbsFilePlanning.Add(rnd.Next(count));
            }

            CreateDB();
            CloseStreams();
            login = log;
            password = pass;

            driver.Navigate().GoToUrl("https://vk.com");
            Login();

            planningThread.Start();
            WriteThread.Start();
            ReadThread.Start();
            if (count == 3)
            {
                pipeStream.Connect();
                DaemonThread.Start();
            }
            while (true)
            {
                oldCount = Elements.Count;
                jsExecutor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Elements = driver.FindElementsByCssSelector(".wall_post_cont").Distinct().ToList();

                for (var item = oldCount; item != Elements.Count; ++item)
                {
                    PostID.Add("https://vk.com/wall" + Elements[item].GetAttribute("id").Substring(3, Elements[item].GetAttribute("id").Length - 3));
                    FindText(item);
                    FindThumbs(item);
                }
            }
        }

        protected void Planning()
        {
            while(true)
            {
                if (count == 2)
                {
                    if (idTextWriteIteration >= 19 && idTextReadIteration >= 19)
                    {
                        idTextWriteIteration = 0;
                        idTextReadIteration = 0;
                        for (int i = 0; i != 20; ++i)
                        {
                            idTextPlanning[i] = rnd.Next(count);
                        }
                    }

                    if (idImgWriteIteration >= 19 && idImgReadIteration >= 19)
                    {
                        idImgWriteIteration = 0;
                        idImgReadIteration = 0;
                        for (int i = 0; i != 20; ++i)
                        {
                            idImgPlanning[i] = rnd.Next(count);
                        }
                    }
                    if (allThumbsWriteIteration >= 19 && allThumbsReadIteration >= 19)
                    {
                        allThumbsWriteIteration = 0;
                        allThumbsReadIteration = 0;
                        for (int i = 0; i != 20; ++i)
                        {
                            allThumbsFilePlanning[i] = rnd.Next(count);
                        }
                    }
                }
                if (count == 3)
                {
                    if (idTextWriteIteration >= 19 && idTextReadIteration >= 19 && daemonIdTextIteration >= 19)
                    {
                        idTextWriteIteration = 0;
                        idTextReadIteration = 0;
                        daemonIdTextIteration = 0;
                        for (int i = 0; i != 20; ++i)
                        {
                            idTextPlanning[i] = rnd.Next(count);
                        }
                    }

                    if (idImgWriteIteration >= 19 && idImgReadIteration >= 19 && daemonIdImgIteration >= 19)
                    {
                        idImgWriteIteration = 0;
                        idImgReadIteration = 0;
                        daemonIdImgIteration = 0;
                        for (int i = 0; i != 20; ++i)
                        {
                            idImgPlanning[i] = rnd.Next(count);
                        }
                    }
                    if (allThumbsWriteIteration >= 19 && allThumbsReadIteration >= 19 && daemonAllThumbsIteration >= 19)
                    {
                        allThumbsWriteIteration = 0;
                        allThumbsReadIteration = 0;
                        daemonAllThumbsIteration = 0;
                        for (int i = 0; i != 20; ++i)
                        {
                            allThumbsFilePlanning[i] = rnd.Next(count);
                        }
                    }
                }
            }
        }
       
        protected void Daemon()
        {
            StreamWriter sw = new StreamWriter(pipeStream)
            {
                AutoFlush = true
            };
            while (true)
            {
                if (daemonIdTextIteration < 19)
                {
                    ++daemonIdTextIteration;
                    if (idTextPlanning[daemonIdTextIteration] == 2)
                    {
                        while (!idTextIsFree)
                            Thread.Sleep(100);
                        idTextIsFree = false;

                        sw.WriteLine("text");
                        StreamReader sr = new StreamReader(pipeStream);

                        while (true)
                        {
                            string temp = sr.ReadLine();

                            if (temp == "end")
                                break;
                        }

                        idTextIsFree = true;
                    }
                }
                if (daemonIdImgIteration < 19)
                {
                    ++daemonIdImgIteration;
                    if (idImgPlanning[daemonIdImgIteration] == 2)
                    {
                        while (!idImgIsFree)
                            Thread.Sleep(100);
                        idImgIsFree = false;

                        sw.WriteLine("img");
                        StreamReader sr = new StreamReader(pipeStream);

                        while (true)
                        {
                            string temp = sr.ReadLine();

                            if (temp == "end")
                                break;
                        }
                        
                        idImgIsFree = true;
                    }
                }
                if (daemonAllThumbsIteration < 19)
                {
                    ++daemonAllThumbsIteration;
                    if (allThumbsFilePlanning[daemonAllThumbsIteration] == 2)
                    {
                        while (!allThumbsIsFree)
                            Thread.Sleep(100);
                        allThumbsIsFree = false;

                        sw.WriteLine("allThumbs");
                        StreamReader sr = new StreamReader(pipeStream);

                        while (true)
                        {
                            string temp = sr.ReadLine();

                            if (temp == "end")
                                break;
                        }

                        allThumbsIsFree = true;
                    }
                }
            }
        }

        protected void WriteAll()
        {
            while (true)
            {
                if (idTextWriteIteration < 19)
                {
                    ++idTextWriteIteration;
                    if (idTextPlanning[idTextWriteIteration] == 0)
                    {
                        while (!idTextIsFree)
                            Thread.Sleep(100);
                        idTextIsFree = false;
                        int TempElementsCount = oldCount;
                        using (StreamWriter swText = new StreamWriter(textPath, true))
                        {
                            for (int i = textCounter; i < TempElementsCount; ++i)
                            {
                                Text temp = new Text
                                {
                                    ID = PostID[i].Replace("\n", " "),
                                    PostText = TextElements[i].Replace("\n", " ")
                                };
                                swText.WriteLine(JsonConvert.SerializeObject(temp, Formatting.None) + ",");
                                ++textCounter;
                            }
                        }
                        idTextIsFree = true;
                    }
                }

                if (idImgWriteIteration < 19)
                {
                    ++idImgWriteIteration;
                    if (idImgPlanning[idImgWriteIteration] == 0)
                    {
                        while (!idImgIsFree)
                            Thread.Sleep(100);
                        idImgIsFree = false;
                        int TempElementsCount = oldCount;
                        using (StreamWriter swImg = new StreamWriter(imgPath, true))
                        {
                            for (int i = imgCounter; i < TempElementsCount; ++i)
                            {
                                Image temp = new Image
                                {
                                    ID = PostID[i].Replace("\n", " "),
                                    Img = ImagesLinks[i].Replace("\n", " ")
                                };
                                swImg.WriteLine(JsonConvert.SerializeObject(temp, Formatting.None) + ",");
                                ++imgCounter;
                            }
                        }
                        idImgIsFree = true;
                    }
                }

                if (allThumbsWriteIteration < 19)
                {
                    ++allThumbsWriteIteration;
                    if (allThumbsFilePlanning[allThumbsWriteIteration] == 0)
                    {
                        while (!allThumbsIsFree)
                            Thread.Sleep(100);
                        allThumbsIsFree = false;
                        int TempElementsCount = oldCount;
                        using (StreamWriter swAll = new StreamWriter(allThumbsPath, true))
                        {
                            for (int i = allThumbsCounter; i < TempElementsCount; ++i)
                            {
                                AllThumbs temp = new AllThumbs
                                {
                                    ID = PostID[i].Replace("\n", " "),
                                    Video = VideosLinks[i].Replace("\n", " "),
                                    Audio = AudiosLinks[i].Replace("\n", " "),
                                    GIF = GIFsLinks[i].Replace("\n", " "),
                                    Doc = DocumentsLinks[i].Replace("\n", " "),
                                    Article = ArticlesLinks[i].Replace("\n", " "),
                                    Poll = PollsLinks[i].Replace("\n", " "),
                                    ThumbedLink = ThumbedLinks[i].Replace("\n", " "),
                                    Geotag = Geotags[i].Replace("\n", " "),
                                    Poster = PostersLinks[i].Replace("\n", " "),
                                    MediaThumbedLink = MediaThumbedLinks[i].Replace("\n", " ")
                                };
                                swAll.WriteLine(JsonConvert.SerializeObject(temp, Formatting.None) + ",");
                                ++allThumbsCounter;
                            }
                        }
                        allThumbsIsFree = true;
                    }
                }
            }
        }

        void CreateDB()
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

        protected void CloseStreams()
        {
            idTextFile.Close();
            idImgFile.Close();
            allThumbsFile.Close();
        }
        protected static ChromeOptions InitOptions()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--incognito");
            options.AddArgument("--start-maximized");
            return options;
        }
        protected void Login()
        {
            driver.FindElementById("index_email").SendKeys(login);
            driver.FindElementById("index_pass").SendKeys(password);
            driver.FindElementById("index_login_button").SendKeys(Keys.Enter);
        }

        protected void FindText(int item)
        {
            if (Elements[item].FindElements(By.ClassName("wall_post_text")).Count > 0)
            {
                string TextElement = "";
                if (Elements[item].FindElements(By.XPath(".//*[@class='wall_post_text']/span")).Count > 0)
                {
                    TextElement += Elements[item].FindElement(By.ClassName("wall_post_text")).Text;
                    //Substring(0, Elements[item].FindElement(By.ClassName("wall_post_text")).Text.Length - 19);
                    jsExecutor.ExecuteScript("arguments[0].style.display = 'block';", Elements[item].FindElement(By.ClassName("wall_post_text")).FindElement(By.TagName("span")));
                    TextElement += Elements[item].FindElement(By.XPath(".//*[@class='wall_post_text']/span")).Text + "\n";
                }
                else
                {
                    TextElement += Elements[item].FindElement(By.ClassName("wall_post_text")).Text;
                }
                TextElements.Add(TextElement);
                PostersLinks.Add("");
            }
            else
            {   
                if (Elements[item].FindElements(By.ClassName("poster__wrap")).Count > 0)
                {
                    PostersLinks.Add(Elements[item].FindElement(By.XPath(".//*[@class='poster__wrap']/*[@class='poster__image']")).GetCssValue("background-image").
                    Substring(5, Elements[item].FindElement(By.XPath(".//*[@class='poster__wrap']/*[@class='poster__image']")).GetCssValue("background-image").Length - 7));
                    TextElements.Add(Elements[item].FindElement(By.XPath(".//*[@class='poster__wrap']/*[@class='poster__text']")).Text);
                }
                else
                {
                    PostersLinks.Add("");
                    TextElements.Add("");
                }
            }
        }

        protected void SplitThumbsURL(List<IWebElement> Thumbs)
        {
            string TempListImages = "",
                   TempListVideos = "",
                   TempListGIFs = "";

            foreach (var target in Thumbs)
            {
                foreach (var links in target.FindElements(By.ClassName(imagesWrap)))
                {
                    if (!links.GetAttribute("class").Contains(videoWrap))
                    {
                        TempListImages += links.GetCssValue("background-image").Substring(5, links.GetCssValue("background-image").Length - 7) + "\n";
                    }
                    else
                    {
                        TempListVideos += "https://vk.com/video" + links.GetAttribute("data-video") + "\n";
                    }
                }

                if (target.FindElements(By.ClassName(GIFWrap)).Count > 0)
                {
                    foreach (var dataGIF in target.FindElements(By.ClassName(GIFWrap)))
                    {
                        TempListGIFs += dataGIF.GetAttribute("href") + "\n";
                    }
                }

                ImagesLinks.Add(TempListImages);
                GIFsLinks.Add(TempListGIFs);
                VideosLinks.Add(TempListVideos);
            }
        }

        protected void FindAudiosURL(List<IWebElement> Audios)
        {
            string TempListAudios = "";
            foreach (var dataAudio in Audios[0].FindElements(By.ClassName(audioRow)))
            {
                TempListAudios += "https://vk.com/audio" + dataAudio.GetAttribute("data-full-id") + "\n";
            }
            AudiosLinks.Add(TempListAudios);
        }

        protected void FindDocumentsURL(List<IWebElement> Documents)
        {
            string TempListDocuments = "";
            foreach (var dataDocument in Documents)
            {
                TempListDocuments += dataDocument.FindElement(By.ClassName("page_doc_icon")).GetAttribute("href") + "\n";
            }
            DocumentsLinks.Add(TempListDocuments);
        }

        protected void FindThumbs(int item)
        {
            List<IWebElement> Thumbs = new List<IWebElement>(),
                              Audios = new List<IWebElement>(),
                              Documents = new List<IWebElement>(),
                              Articles = new List<IWebElement>(),
                              Polls = new List<IWebElement>(),
                              TLinks = new List<IWebElement>(),
                              GTags = new List<IWebElement>(),
                              MLinks = new List<IWebElement>();
                              
            Thumbs = Elements[item].FindElements(By.ClassName("page_post_sized_thumbs")).Distinct().ToList();
            Audios = Elements[item].FindElements(By.ClassName("wall_audio_rows")).Distinct().ToList();
            Documents = Elements[item].FindElements(By.ClassName("media_desc__doc")).Distinct().ToList();
            Articles = Elements[item].FindElements(By.ClassName("article_snippet")).Distinct().ToList();
            Polls = Elements[item].FindElements(By.ClassName("media_voting")).Distinct().ToList();
            TLinks = Elements[item].FindElements(By.ClassName("thumbed_link")).Distinct().ToList();
            GTags = Elements[item].FindElements(By.ClassName("page_media_place_label_inline")).Distinct().ToList();
            MLinks = Elements[item].FindElements(By.ClassName("media_link")).Distinct().ToList();

            if (Thumbs.Count > 0)
            {
                SplitThumbsURL(Thumbs);
            }
            else
            {
                ImagesLinks.Add("");
                VideosLinks.Add("");
                GIFsLinks.Add("");
            }

            if (Audios.Count > 0)
            {
                FindAudiosURL(Audios);
            }
            else
            {
                AudiosLinks.Add("");
            }

            if (Documents.Count > 0)
            {
                FindDocumentsURL(Documents);
            }
            else
            {
                DocumentsLinks.Add("");
            }

            if (Articles.Count > 0)
            {
                ArticlesLinks.Add(Articles[0].GetAttribute("href") + "\n");
            }
            else
            {
                ArticlesLinks.Add("");
            }

            if (Polls.Count > 0)
            {
                PollsLinks.Add("https://vk.com/poll" + Polls[0].GetAttribute("data-owner-id") + "_" + Polls[0].GetAttribute("data-id") + "\n");
            }
            else
            {
                PollsLinks.Add("");
            }
            if (TLinks.Count > 0)
            {
                ThumbedLinks.Add(TLinks[0].FindElement(By.ClassName("thumbed_link__thumb")).GetAttribute("href") + "\n");
            }
            else
            {
                ThumbedLinks.Add("");
            }
            if (MLinks.Count > 0)
            {
                MediaThumbedLinks.Add(MLinks[0].FindElement(By.ClassName("media_link__title")).GetAttribute("href") + "\n");
            }
            else
            {
                MediaThumbedLinks.Add("");
            }
            if (GTags.Count > 0)
            {
                Geotags.Add(GTags[0].Text + "\n");
            }
            else
            {
                Geotags.Add("");
            }
        }

        protected void ReadRand()
        {
            while (true)
            {
                if (idTextReadIteration < 19)
                {
                    ++idTextReadIteration;
                    if(idTextPlanning[idTextReadIteration] == 1)
                    {
                        ReadIdText();
                    }
                }
                if (idImgReadIteration < 19)
                {
                    ++idImgReadIteration;
                    if(idImgPlanning[idImgReadIteration] == 1)
                    {
                        ReadIdImg();
                    }
                }
                if (allThumbsReadIteration < 19)
                {
                    ++allThumbsReadIteration;
                    if (allThumbsFilePlanning[allThumbsReadIteration] == 1)
                    {
                        ReadAllFile();
                    }
                }
            }
        }
    }



    protected void ReadIdText()
    {
        while (!idTextIsFree)
            Thread.Sleep(100);
        if (idTextIsFree)
        {
            idTextIsFree = false;
            /*Process readerIdText = new Process();
            string args = "--command nano " + textPath;
            ProcessStartInfo readInfo = new ProcessStartInfo
            {
                FileName = "lxterminal",
                Arguments = args
            };
            readerIdText.StartInfo = readInfo;
            readerIdText.Start();*/
            using (StreamReader srIdText = new StreamReader(textPath, true))
            {
                Console.WriteLine(srIdText.ReadToEnd());
            }
            idTextIsFree = true;
        }
    }

    protected void ReadIdImg()
    {
        while (!idImgIsFree)
            Thread.Sleep(100);
        if (idImgIsFree)
        {
            idImgIsFree = false;
            /*Process readerIdImg = new Process();
            string args = "--command nano " + imgPath;
            ProcessStartInfo readInfo = new ProcessStartInfo
            {
                FileName = "lxterminal",
                Arguments = args
            };
            readerIdImg.StartInfo = readInfo;
            readerIdImg.Start();*/
            using (StreamReader srIdImg = new StreamReader(imgPath, true))
            {
                Console.WriteLine(srIdImg.ReadToEnd());
            }
            idImgIsFree = true;
        }
    }

    protected void ReadAllFile()
    {
        while (!allThumbsIsFree)
            Thread.Sleep(100);
        if (allThumbsIsFree)
        {
            allThumbsIsFree = false;

            /*Process readerAll = new Process();
            string args = "--command nano " + allThumbsPath;
            ProcessStartInfo readInfo = new ProcessStartInfo
            {
                FileName = "lxterminal",
                Arguments = args
            };
            readerAll.StartInfo = readInfo;
            readerAll.Start();*/
            using (StreamReader srAllThumbs = new StreamReader(allThumbsPath, true))
            {
                Console.WriteLine(srAllThumbs.ReadToEnd());
            }
            allThumbsIsFree = true;
        }
    }

    protected void ClearEntrys()
    {
        Application.Invoke(delegate {
            entry1.Text = string.Empty;
            entry2.Text = string.Empty;
        });
    }

    protected void OnButton2Clicked(object sender, EventArgs e)
    {
        Chrome Main = new Chrome();
        //Main.Init(entry1.Text, entry2.Text);
        Thread mainThread = new Thread(() => Main.Init(entry1.Text, entry2.Text));
        mainThread.Start();
    }

    protected void OnButton5Clicked(object sender, EventArgs e)
    {
        Process readerAll = new Process();
        string args = "--command sudo systemctl start trash.service";
        ProcessStartInfo readInfo = new ProcessStartInfo
        {
            FileName = "lxterminal",
            Arguments = args
        };
        readerAll.StartInfo = readInfo;
        readerAll.Start();
    }

    protected void OnButton6Clicked(object sender, EventArgs e)
    {
        Process readerAll = new Process();
        string args = "--command sudo systemctl stop trash.service";
        ProcessStartInfo readInfo = new ProcessStartInfo
        {
            FileName = "lxterminal",
            Arguments = args
        };
        readerAll.StartInfo = readInfo;
        readerAll.Start();
    }

    protected void ThreadStartClient() 
    {

            count = 3;

        
    }


    protected void OnButton7Clicked(object sender, EventArgs e)
    {
        Process readerAll = new Process();
        string args = "--command systemctl status trash.service";
        ProcessStartInfo readInfo = new ProcessStartInfo
        {
            FileName = "lxterminal",
            Arguments = args
        };
        readerAll.StartInfo = readInfo;
        readerAll.Start();
    }

    protected void OnButton1Clicked(object sender, EventArgs e)
    {
        Process readerAll = new Process();
        string args = "--command sudo systemctl restart trash.service";
        ProcessStartInfo readInfo = new ProcessStartInfo
        {
            FileName = "lxterminal",
            Arguments = args
        };
        readerAll.StartInfo = readInfo;
        readerAll.Start();
        Thread.Sleep(6000);
        ThreadStartClient();
    }
}
