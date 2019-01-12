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

        IGuiMenuItem mnuNew;
        IGuiMenuItem mnuOpen;		
        IGuiMenuItem mnuSave;

        public MainForm () : base("SummerGUI Demo", 800, 600)
		{
			this.Title = "SummerEdit - X-Platform Texteditor";

			this.TabMain.AdTabPage ("texteditor", "Editor", Color.Empty, (char)FontAwesomeIcons.fa_edit);
			m_Editor = this.TabMain.TabPages ["texteditor"].AddChild (new TextEditorEnsemble ("editor1"));

            mnuNew = this.MenuPanel.MainMenu.FindItem("New");
            mnuNew.Click += (object sender, EventArgs e) =>
            {
                m_Editor.Text = null;
            };

            mnuOpen = this.MenuPanel.MainMenu.FindItem ("Open");
			mnuOpen.Click += (object sender, EventArgs e) => {
				SummerGUI.SystemSpecific.Linux.SystemDialogs dlg = new SummerGUI.SystemSpecific.Linux.SystemDialogs();									
				string fpath = dlg.OpenFileDialog("Open File", this);
                if (Strings.FileExists(fpath))
                {
                    ShowStatus(String.Format("Loading {0}..", Strings.GetFilename(fpath)), true);
                    System.Threading.Tasks.Task.Factory.StartNew(() => {
                        m_Editor.FilePath = fpath;
                        m_Editor.Text = TextFile.LoadTextFile(fpath);
                    }).ContinueWith(t => ShowStatus());
                }
			};

            mnuSave = this.MenuPanel.MainMenu.FindItem("Save");
            mnuSave.Click += (object sender, EventArgs e) =>
            {
                this.LogWarning("mnuSave not implemented.");
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

