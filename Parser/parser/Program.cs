﻿using System.Threading;
using Gtk;

namespace parser
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow();
            win.ShowAll();
            Application.Run();
        }
    }
}
