
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.VBox vbox1;

	private global::Gtk.Label label1;

	private global::Gtk.Entry entry1;

	private global::Gtk.Label label2;

	private global::Gtk.Entry entry2;

	private global::Gtk.Button button2;

	private global::Gtk.HSeparator hseparator2;

	private global::Gtk.Button button5;

	private global::Gtk.VBox vbox3;

	private global::Gtk.Button button6;

	private global::Gtk.Button button7;

	private global::Gtk.Button button1;

	protected virtual void Build()
	{
		global::Stetic.Gui.Initialize(this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		this.vbox1.BorderWidth = ((uint)(3));
		// Container child vbox1.Gtk.Box+BoxChild
		this.label1 = new global::Gtk.Label();
		this.label1.Name = "label1";
		this.label1.LabelProp = global::Mono.Unix.Catalog.GetString("login");
		this.vbox1.Add(this.label1);
		global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.label1]));
		w1.Position = 0;
		w1.Expand = false;
		w1.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.entry1 = new global::Gtk.Entry();
		this.entry1.CanFocus = true;
		this.entry1.Name = "entry1";
		this.entry1.IsEditable = true;
		this.entry1.InvisibleChar = '•';
		this.vbox1.Add(this.entry1);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.entry1]));
		w2.Position = 1;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.label2 = new global::Gtk.Label();
		this.label2.Name = "label2";
		this.label2.LabelProp = global::Mono.Unix.Catalog.GetString("password");
		this.vbox1.Add(this.label2);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.label2]));
		w3.Position = 2;
		w3.Expand = false;
		w3.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.entry2 = new global::Gtk.Entry();
		this.entry2.CanFocus = true;
		this.entry2.Name = "entry2";
		this.entry2.IsEditable = true;
		this.entry2.Visibility = false;
		this.entry2.InvisibleChar = '•';
		this.vbox1.Add(this.entry2);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.entry2]));
		w4.Position = 3;
		w4.Expand = false;
		w4.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.button2 = new global::Gtk.Button();
		this.button2.CanFocus = true;
		this.button2.Events = ((global::Gdk.EventMask)(256));
		this.button2.Name = "button2";
		this.button2.UseUnderline = true;
		this.button2.Label = global::Mono.Unix.Catalog.GetString("go");
		this.vbox1.Add(this.button2);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.button2]));
		w5.Position = 4;
		w5.Expand = false;
		w5.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hseparator2 = new global::Gtk.HSeparator();
		this.hseparator2.Name = "hseparator2";
		this.vbox1.Add(this.hseparator2);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hseparator2]));
		w6.Position = 5;
		w6.Expand = false;
		w6.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.button5 = new global::Gtk.Button();
		this.button5.CanFocus = true;
		this.button5.Name = "button5";
		this.button5.UseUnderline = true;
		this.button5.Label = global::Mono.Unix.Catalog.GetString("start daemon");
		this.vbox1.Add(this.button5);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.button5]));
		w7.Position = 6;
		w7.Expand = false;
		w7.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.vbox3 = new global::Gtk.VBox();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.button6 = new global::Gtk.Button();
		this.button6.CanFocus = true;
		this.button6.Name = "button6";
		this.button6.UseUnderline = true;
		this.button6.Label = global::Mono.Unix.Catalog.GetString("stop daemon");
		this.vbox3.Add(this.button6);
		global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.button6]));
		w8.Position = 0;
		w8.Expand = false;
		w8.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.button7 = new global::Gtk.Button();
		this.button7.CanFocus = true;
		this.button7.Name = "button7";
		this.button7.UseUnderline = true;
		this.button7.Label = global::Mono.Unix.Catalog.GetString("status daemon");
		this.vbox3.Add(this.button7);
		global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.button7]));
		w9.Position = 1;
		w9.Expand = false;
		w9.Fill = false;
		this.vbox1.Add(this.vbox3);
		global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.vbox3]));
		w10.Position = 7;
		w10.Expand = false;
		w10.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.button1 = new global::Gtk.Button();
		this.button1.CanFocus = true;
		this.button1.Name = "button1";
		this.button1.UseUnderline = true;
		this.button1.Label = global::Mono.Unix.Catalog.GetString("Try connect");
		this.vbox1.Add(this.button1);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.button1]));
		w11.Position = 8;
		w11.Expand = false;
		w11.Fill = false;
		this.Add(this.vbox1);
		if ((this.Child != null))
		{
			this.Child.ShowAll();
		}
		this.DefaultWidth = 400;
		this.DefaultHeight = 360;
		this.Show();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
		this.button2.Clicked += new global::System.EventHandler(this.OnButton2Clicked);
		this.button5.Clicked += new global::System.EventHandler(this.OnButton5Clicked);
		this.button6.Clicked += new global::System.EventHandler(this.OnButton6Clicked);
		this.button7.Clicked += new global::System.EventHandler(this.OnButton7Clicked);
		this.button1.Clicked += new global::System.EventHandler(this.OnButton1Clicked);
	}
}
