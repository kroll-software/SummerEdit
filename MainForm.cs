using System;
using System.Drawing;
using System.Linq;
using SummerGUI;
using KS.Foundation;
using System.Runtime.InteropServices;

namespace SummerEdit
{
	public class MainForm : ApplicationWindow
	{
		public TextEditorEnsemble m_Editor;

		IGuiMenuItem mnuOpen;
		IGuiMenuItem mnuNew;

		public MainForm () : base("SummerGUI Demo", 800, 600)
		{
			this.Title = "SummerEdit - X-Platform Texteditor";

			this.TabMain.AdTabPage ("texteditor", "Editor", Color.Empty, (char)FontAwesomeIcons.fa_edit);
			m_Editor = this.TabMain.TabPages ["texteditor"].AddChild (new TextEditorEnsemble ("editor1"));

			mnuOpen = this.MenuPanel.MainMenu.FindItem ("Open");
			mnuNew = this.MenuPanel.MainMenu.FindItem ("New");

			/***
			mnuOpen.Click += (object sender, EventArgs e) => 
				StandardDialogs.OpenFileDialog("SummerEditor", 
					Strings.ApplicationPath(true),
					new string[]{"", ""}, "Text Files", false);	
			***/

			mnuOpen.Click += (object sender, EventArgs e) => {
				SummerGUI.SystemSpecific.Linux.SystemDialogs dlg = new SummerGUI.SystemSpecific.Linux.SystemDialogs();									
				string fpath = dlg.OpenFileDialog("Open File", this);
			};

			//this.LeftSideMenu.Style.BackColorBrush = new LinearGradientBrush (Theme.Colors.Base0, Theme.Colors.Base00);

			m_Editor.Focus ();
		}

        protected override void Dispose(bool manual)
        {
            if (manual)
            {
                // Dispose your objects here
            }
            base.Dispose(manual);
        }
    }
}

