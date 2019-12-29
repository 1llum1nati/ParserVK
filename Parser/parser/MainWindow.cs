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
    NamedPipeClientStream pipeStream = new NamedPipeClientStream("pipes");



    private class Chrome : MainWindow
    {
        string login, password;
        public static readonly string imagesWrap = "page_post_thumb_wrap",
                               videoWrap = "page_post_thumb_video",
                               audioRow = "audio_row",
                               GIFWrap = "page_post_thumb_unsized";

        FileStream idTextFile = new FileStream(textPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        FileStream idImgFile = new FileStream(imgPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        FileStream allThumbsFile = new FileStream(allThumbsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        static ChromeDriver driver = new ChromeDriver(InitOptions());
        static IJavaScriptExecutor jsExecutor = driver;
        int oldCount, textCounter, imgCounter, allThumbsCounter,
            idTextWriteIteration = 23, idImgWriteIteration = 23, allThumbsWriteIteration = 23,
            idTextReadIteration = 23, idImgReadIteration = 23, allThumbsReadIteration = 23,
            daemonIdTextIteration = 23, daemonIdImgIteration = 23, daemonAllThumbsIteration = 23;

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


        List<int> idTextPlanning = new List<int>(24),
                  idImgPlanning = new List<int>(24),
                  allThumbsFilePlanning = new List<int>(24);


        protected internal void Init(string log, string pass)
        {
            Thread WriteThread = new Thread(() => WriteAll());
            Thread ReadThread = new Thread(() => ReadRand());
            Thread DaemonThread = new Thread(() => Daemon());


            Thread planningThread = new Thread(() => Planning());

            CloseStreams();
            login = log;
            password = pass;

            driver.Navigate().GoToUrl("https://vk.com");
            Login();


            WriteThread.Start();
            ReadThread.Start();
            DaemonThread.Start();
            if (count == 3)
            {
                pipeStream.Connect();
            }
            planningThread.Start();

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

        protected void for2()
        {
            int rand1 = rnd.Next(count),
                rand2 = rnd.Next(count),
                rand3 = rnd.Next(count);

            for (int i = 0; i != count; ++i)
            {
                if (i != rand1)
                {
                    rand2 = i;
                    break;
                }
            }
            for (int i = 0; i != count; ++i)
            {
                if (i != rand2)
                {
                    rand3 = i;
                    break;
                }
            }

            idTextPlanning[0] = rand1;
            idImgPlanning[0] = rand2;
            allThumbsFilePlanning[0] = rand3;

            for (int i = 0; i != count; ++i)
            {
                if (i != idTextPlanning[0])
                {
                    idTextPlanning[1] = i;
                    break;
                }
            }
            for (int i = 0; i != count; ++i)
            {
                if (i != idImgPlanning[0])
                {
                    idImgPlanning[1] = i;
                    break;
                }
            }
            for (int i = 0; i != count; ++i)
            {
                if (i != allThumbsFilePlanning[0])
                {
                    allThumbsFilePlanning[1] = i;
                    break;
                }
            }
        }

        protected void for3()
        {
            int rand1 = rnd.Next(count),
                rand2 = rnd.Next(count),
                rand3 = rnd.Next(count);
            for (int i = 0; i != count; ++i)
            {
                if (i != rand1)
                {
                    rand2 = i;
                    break;
                }
            }
            for (int i = 0; i != count; ++i)
            {
                if (i == rand1)
                {
                    continue;
                }
                if (i == rand2)
                {
                    continue;
                }
                rand3 = i;
            }

            idTextPlanning[0] = rand1;
            idImgPlanning[0] = rand2;
            allThumbsFilePlanning[0] = rand3;

            for (int i = 0; i != count; ++i)
            {
                if (i == idTextPlanning[0])
                    continue;
                idTextPlanning[1] = i;
            }
            for (int i = 0; i != count; ++i)
            {
                if (i == idImgPlanning[0])
                {
                    continue;
                }
                if (i == idTextPlanning[1])
                {
                    continue;
                }
                idImgPlanning[1] = i;
            }

            for (int i = 0; i != count; ++i)
            {
                if (i == idImgPlanning[1])
                {
                    continue;
                }
                if (i == idTextPlanning[1])
                {
                    continue;
                }
                allThumbsFilePlanning[1] = i;
            }
            for (int i = 0; i != count; ++i)
            {
                if (i == idTextPlanning[0])
                {
                    continue;
                }
                if (i == idTextPlanning[1])
                {
                    continue;
                }
                idTextPlanning[2] = i;
            }
            for (int i = 0; i != count; ++i)
            {
                if (i == idImgPlanning[0])
                {
                    continue;
                }
                if (i == idImgPlanning[1])
                {
                    continue;
                }
                idImgPlanning[2] = i;
            }
            for (int i = 0; i != count; ++i)
            {
                if (i == allThumbsFilePlanning[0])
                {
                    continue;
                }
                if (i == allThumbsFilePlanning[1])
                {
                    continue;
                }
                allThumbsFilePlanning[2] = i;
            }
        }

        protected void Planning()
        {
            for (int i = 0; i != 24; ++i)
            {
                idTextPlanning.Add(-1);
                idImgPlanning.Add(-1);
                allThumbsFilePlanning.Add(-1);
            }

            while (true)
            {
                if (!pipeStream.IsConnected) 
                { 
                    count = 2;
                    try
                    {
                        pipeStream.Connect(100);
                        count = 3;
                    }
                    catch { }
                }
                else
                {
                    count = 3;
                }


                if (count == 2)
                {
                    if (idTextWriteIteration >= 23 && idTextReadIteration >= 23 && idImgWriteIteration >= 23 && idImgReadIteration >= 23 && allThumbsWriteIteration >= 23 && allThumbsReadIteration >= 23) 
                    {
                        idTextWriteIteration = 0;
                        idTextReadIteration = 0;
                        idImgWriteIteration = 0;
                        idImgReadIteration = 0;
                        allThumbsWriteIteration = 0;
                        allThumbsReadIteration = 0;
                        for2();
                        int i = 0;
                        do
                        {
                            idTextPlanning[i + 2] = idTextPlanning[i];
                            idTextPlanning[i + 3] = idTextPlanning[i + 1];
                            idImgPlanning[i + 2] = idImgPlanning[i];
                            idImgPlanning[i + 3] = idImgPlanning[i + 1];
                            allThumbsFilePlanning[i + 2] = allThumbsFilePlanning[i];
                            allThumbsFilePlanning[i + 3] = allThumbsFilePlanning[i + 1];
                            i += 2;
                        }
                        while (i < 22);
                    }
                }

                if (count == 3)
                {
                    if (idTextWriteIteration >= 23 && idTextReadIteration >= 23 && daemonIdTextIteration >= 23
                    && idImgWriteIteration >= 23 && idImgReadIteration >= 23 && daemonIdImgIteration >= 23
                    && allThumbsWriteIteration >= 23 && allThumbsReadIteration >= 23 && daemonAllThumbsIteration >= 23 )
                    {
                        idTextWriteIteration = 0;
                        idTextReadIteration = 0;
                        daemonIdTextIteration = 0;
                        idImgWriteIteration = 0;
                        idImgReadIteration = 0;
                        daemonIdImgIteration = 0;
                        allThumbsWriteIteration = 0;
                        allThumbsReadIteration = 0;
                        daemonAllThumbsIteration = 0;
                        for3();
                        for (int i = 0; i < 21; i += 3)
                        {
                            idTextPlanning[i + 3] = idTextPlanning[i];
                            idTextPlanning[i + 4] = idTextPlanning[i + 1];
                            idTextPlanning[i + 5] = idTextPlanning[i + 2];
                            idImgPlanning[i + 3] = idImgPlanning[i];
                            idImgPlanning[i + 4] = idImgPlanning[i + 1];
                            idImgPlanning[i + 5] = idImgPlanning[i + 2];
                            allThumbsFilePlanning[i + 3] = allThumbsFilePlanning[i];
                            allThumbsFilePlanning[i + 4] = allThumbsFilePlanning[i + 1];
                            allThumbsFilePlanning[i + 5] = allThumbsFilePlanning[i + 2];
                        }
                    }
                }
            }
        }
       
        protected void Daemon()
        {
            while (true)
            {
                if (daemonIdTextIteration < 23)
                {
                    ++daemonIdTextIteration;
                    if (idTextPlanning[daemonIdTextIteration] == 2)
                    {
                        while (!idTextIsFree)
                            Thread.Sleep(100);
                        idTextIsFree = false;
                        if (pipeStream.IsConnected)
                        {
                            StreamWriter sw = new StreamWriter(pipeStream)
                            {
                                AutoFlush = true
                            };
                            try
                            {
                                sw.WriteLine("text");
                            }
                            catch { }
                            StreamReader sr = new StreamReader(pipeStream);

                            while (true)
                            {
                                try
                                {
                                    string temp = sr.ReadLine();

                                    if (temp == "end")
                                        break;
                                }
                                catch { }
                            }

                        }
                        idTextIsFree = true;
                    }
                }
                if (daemonIdImgIteration < 23)
                {
                    ++daemonIdImgIteration;
                    if (idImgPlanning[daemonIdImgIteration] == 2)
                    {
                        while (!idImgIsFree)
                            Thread.Sleep(100);
                        idImgIsFree = false;
                        if (pipeStream.IsConnected)
                        {
                            StreamWriter sw = new StreamWriter(pipeStream)
                            {
                                AutoFlush = true
                            };
                            try
                            {
                                sw.WriteLine("img");
                            }
                            catch { }
                            StreamReader sr = new StreamReader(pipeStream);

                            while (true)
                            {
                                try
                                {
                                    string temp = sr.ReadLine();

                                    if (temp == "end")
                                        break;
                                }
                                catch { }
                            }
                        }
                        idImgIsFree = true;
                    }
                }
                if (daemonAllThumbsIteration < 23)
                {
                    ++daemonAllThumbsIteration;
                    if (allThumbsFilePlanning[daemonAllThumbsIteration] == 2)
                    {
                        while (!allThumbsIsFree)
                            Thread.Sleep(100);
                        allThumbsIsFree = false;
                        if (pipeStream.IsConnected)
                        {
                            StreamWriter sw = new StreamWriter(pipeStream)
                            {
                                AutoFlush = true
                            };
                            try
                            {
                                sw.WriteLine("allThumbs");
                            }
                            catch { }
                            StreamReader sr = new StreamReader(pipeStream);

                            while (true)
                            {
                                try
                                {
                                    string temp = sr.ReadLine();

                                    if (temp == "end")
                                        break;
                                }
                                catch { }
                            }
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
                if (idTextWriteIteration < 23)
                {
                    ++idTextWriteIteration;
                    if (idTextPlanning[idTextWriteIteration] == 0)
                    {
                        while (!idTextIsFree)
                            Thread.Sleep(100);
                        idTextIsFree = false;
                        int TempElementsCount = oldCount;

                        List<Text> TextFromFile = new List<Text>();
                        StreamReader sr = new StreamReader(textPath);
                        string a = sr.ReadLine();
                        if(a != null)
                            TextFromFile.AddRange(JsonConvert.DeserializeObject<List<Text>>(a));
                        sr.Dispose();
                        for (int i = textCounter; i < TempElementsCount; ++i)
                        {
                            int flag = 0;
                            for (int j = 0; j != TextFromFile.Count; ++j)
                            {
                                if (TextFromFile[j].ID == PostID[i].Replace("\n", " ") && TextFromFile[j].PostText == TextElements[i].Replace("\n", " "))
                                {
                                    flag++;
                                }
                            }
                            if(flag == 0)
                            {
                                Text temp = new Text
                                {
                                    ID = PostID[i].Replace("\n", " "),
                                    PostText = TextElements[i].Replace("\n", " ")
                                };
                                TextFromFile.Add(temp);
                            }

                            ++textCounter;
                        }
                        StreamWriter swText = File.CreateText(textPath);

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(swText, TextFromFile);
                        swText.Dispose();

                        
                        idTextIsFree = true;
                    }
                }

                if (idImgWriteIteration < 23)
                {
                    ++idImgWriteIteration;
                    if (idImgPlanning[idImgWriteIteration] == 0)
                    {
                        while (!idImgIsFree)
                            Thread.Sleep(100);
                        idImgIsFree = false;
                        int TempElementsCount = oldCount;
                        List<Image> ImgFromFile = new List<Image>();
                        StreamReader sr = new StreamReader(imgPath);
                        string a = sr.ReadLine();
                        if (a != null)
                            ImgFromFile.AddRange(JsonConvert.DeserializeObject<List<Image>>(a));
                        sr.Dispose();
                        for (int i = imgCounter; i < TempElementsCount; ++i)
                        {
                            int flag = 0;
                            for (int j = 0; j != ImgFromFile.Count; ++j)
                            {
                                if (ImgFromFile[j].ID == PostID[i].Replace("\n", " ") && ImgFromFile[j].Img == ImagesLinks[i].Replace("\n", " "))
                                {
                                    flag++;
                                }
                            }
                            if (flag == 0)
                            {
                                Image temp = new Image
                                {
                                    ID = PostID[i].Replace("\n", " "),
                                    Img = ImagesLinks[i].Replace("\n", " ")
                                };
                                ImgFromFile.Add(temp);
                            }

                            ++imgCounter;
                        }
                        StreamWriter swImg = File.CreateText(imgPath);

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(swImg, ImgFromFile);
                        swImg.Dispose();

                        idImgIsFree = true;
                    }
                }

                if (allThumbsWriteIteration < 23)
                {
                    ++allThumbsWriteIteration;
                    if (allThumbsFilePlanning[allThumbsWriteIteration] == 0)
                    {
                        while (!allThumbsIsFree)
                            Thread.Sleep(100);
                        allThumbsIsFree = false;
                        int TempElementsCount = oldCount;

                        List<AllThumbs> AllThumbsFromFile = new List<AllThumbs>();
                        StreamReader sr = new StreamReader(allThumbsPath);
                        string a = sr.ReadLine();
                        if (a != null)
                            AllThumbsFromFile.AddRange(JsonConvert.DeserializeObject<List<AllThumbs>>(a));
                        sr.Dispose();
                        for (int i = allThumbsCounter; i < TempElementsCount; ++i)
                        {
                            int flag = 0;
                            for (int j = 0; j != AllThumbsFromFile.Count; ++j)
                            {
                                if (AllThumbsFromFile[j].ID == PostID[i].Replace("\n", " ") && AllThumbsFromFile[j].Video == VideosLinks[i].Replace("\n", " ") && AllThumbsFromFile[j].Audio == AudiosLinks[i].Replace("\n", " ") &&
                                    AllThumbsFromFile[j].GIF == GIFsLinks[i].Replace("\n", " ") && AllThumbsFromFile[j].Doc == DocumentsLinks[i].Replace("\n", " ") && AllThumbsFromFile[j].Article == ArticlesLinks[i].Replace("\n", " ") &&
                                    AllThumbsFromFile[j].Poll == PollsLinks[i].Replace("\n", " ") && AllThumbsFromFile[j].ThumbedLink == ThumbedLinks[i].Replace("\n", " ") && AllThumbsFromFile[j].Geotag == Geotags[i].Replace("\n", " ") &&
                                    AllThumbsFromFile[j].Poster == PostersLinks[i].Replace("\n", " ") && AllThumbsFromFile[j].MediaThumbedLink == MediaThumbedLinks[i].Replace("\n", " "))
                                {
                                    flag++;
                                }
                            }
                            if (flag == 0)
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
                                AllThumbsFromFile.Add(temp);
                            }

                            ++allThumbsCounter;
                        }
                        StreamWriter swAll = File.CreateText(allThumbsPath);

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(swAll, AllThumbsFromFile);
                        swAll.Dispose();
                        allThumbsIsFree = true;
                    }
                }
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
                if (idTextReadIteration < 23)
                {
                    ++idTextReadIteration;
                    if(idTextPlanning[idTextReadIteration] == 1)
                    {
                        ReadIdText();
                    }
                }
                if (idImgReadIteration < 23)
                {
                    ++idImgReadIteration;
                    if(idImgPlanning[idImgReadIteration] == 1)
                    {
                        ReadIdImg();
                    }
                }
                if (allThumbsReadIteration < 23)
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
        Thread.Sleep(10000);
        count = 3;
    }
}
