
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.VersionControl.Views
{
	public partial class DiffWidget
	{
		private global::Gtk.VBox vbox2;
		private global::Gtk.HBox hbox1;
		private global::Gtk.HBox hbox2;
		private global::Gtk.Fixed fixed1;
		private global::Gtk.Label labelOverview;
		private global::Gtk.Button buttonDiff;
		private global::Gtk.Button buttonNext;
		private global::Gtk.Button buttonPrev;
		private global::Gtk.Notebook notebook1;
		private global::Gtk.VBox vboxComparisonView;
		private global::Gtk.Label label1;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.Label label3;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.VersionControl.Views.DiffWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "MonoDevelop.VersionControl.Views.DiffWidget";
			// Container child MonoDevelop.VersionControl.Views.DiffWidget.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.fixed1 = new global::Gtk.Fixed ();
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			this.hbox2.Add (this.fixed1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.fixed1]));
			w1.Position = 0;
			w1.Expand = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.labelOverview = new global::Gtk.Label ();
			this.labelOverview.Name = "labelOverview";
			this.hbox2.Add (this.labelOverview);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.labelOverview]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.buttonDiff = new global::Gtk.Button ();
			this.buttonDiff.CanFocus = true;
			this.buttonDiff.Name = "buttonDiff";
			this.buttonDiff.UseUnderline = true;
			this.buttonDiff.Label = global::Mono.Unix.Catalog.GetString ("Unified Diff");
			this.hbox2.Add (this.buttonDiff);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.buttonDiff]));
			w3.PackType = ((global::Gtk.PackType)(1));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			this.hbox1.Add (this.hbox2);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.hbox2]));
			w4.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonNext = new global::Gtk.Button ();
			this.buttonNext.CanFocus = true;
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.UseUnderline = true;
			// Container child buttonNext.Gtk.Container+ContainerChild
			global::Gtk.Alignment w5 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w6 = new global::Gtk.HBox ();
			w6.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w7 = new global::Gtk.Image ();
			w7.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-go-down", global::Gtk.IconSize.Menu);
			w6.Add (w7);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w9 = new global::Gtk.Label ();
			w9.LabelProp = global::Mono.Unix.Catalog.GetString ("Next Change");
			w9.UseUnderline = true;
			w6.Add (w9);
			w5.Add (w6);
			this.buttonNext.Add (w5);
			this.hbox1.Add (this.buttonNext);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonNext]));
			w13.PackType = ((global::Gtk.PackType)(1));
			w13.Position = 1;
			w13.Expand = false;
			w13.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonPrev = new global::Gtk.Button ();
			this.buttonPrev.CanFocus = true;
			this.buttonPrev.Name = "buttonPrev";
			this.buttonPrev.UseUnderline = true;
			// Container child buttonPrev.Gtk.Container+ContainerChild
			global::Gtk.Alignment w14 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			global::Gtk.HBox w15 = new global::Gtk.HBox ();
			w15.Spacing = 2;
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Image w16 = new global::Gtk.Image ();
			w16.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-go-up", global::Gtk.IconSize.Menu);
			w15.Add (w16);
			// Container child GtkHBox.Gtk.Container+ContainerChild
			global::Gtk.Label w18 = new global::Gtk.Label ();
			w18.LabelProp = global::Mono.Unix.Catalog.GetString ("Previous Change");
			w18.UseUnderline = true;
			w15.Add (w18);
			w14.Add (w15);
			this.buttonPrev.Add (w14);
			this.hbox1.Add (this.buttonPrev);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonPrev]));
			w22.PackType = ((global::Gtk.PackType)(1));
			w22.Position = 2;
			w22.Expand = false;
			w22.Fill = false;
			this.vbox2.Add (this.hbox1);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox1]));
			w23.Position = 0;
			w23.Expand = false;
			w23.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.notebook1 = new global::Gtk.Notebook ();
			this.notebook1.CanFocus = true;
			this.notebook1.Name = "notebook1";
			this.notebook1.CurrentPage = 0;
			this.notebook1.ShowBorder = false;
			this.notebook1.ShowTabs = false;
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.vboxComparisonView = new global::Gtk.VBox ();
			this.vboxComparisonView.Name = "vboxComparisonView";
			this.vboxComparisonView.Spacing = 6;
			this.notebook1.Add (this.vboxComparisonView);
			// Notebook tab
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("page2");
			this.notebook1.SetTabLabel (this.vboxComparisonView, this.label1);
			this.label1.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow1.CanFocus = true;
			this.scrolledwindow1.Name = "scrolledwindow1";
			this.scrolledwindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			this.notebook1.Add (this.scrolledwindow1);
			global::Gtk.Notebook.NotebookChild w25 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.scrolledwindow1]));
			w25.Position = 1;
			// Notebook tab
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("page2");
			this.notebook1.SetTabLabel (this.scrolledwindow1, this.label3);
			this.label3.ShowAll ();
			this.vbox2.Add (this.notebook1);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.notebook1]));
			w26.Position = 1;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}
	}
}