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

            this.TabMain.AdTabPage("texteditor", "Editor", Color.Empty, (char)FontAwesomeIcons.fa_edit);
            m_Editor = this.TabMain.TabPages["texteditor"].AddChild(new TextEditorEnsemble("editor1"));

            mnuNew = this.MenuPanel.MainMenu.FindItem("New");
            mnuNew.Click += (object sender, EventArgs e) =>
            {
                m_Editor.Text = null;
            };

            mnuOpen = this.MenuPanel.MainMenu.FindItem("Open");
            mnuOpen.Click += (object sender, EventArgs e) => {                
                OpenFileDialog dlg = new OpenFileDialog();
                var result = dlg.ShowDialog(this, "Open File", "Text files (*.txt)|*.txt|All files (*.*)|*.*");                
                if (result == DialogResults.OK && Strings.FileExists(dlg.FileName))
                {
                    ShowStatus(String.Format("Loading {0}..", Strings.GetFilename(dlg.FileName)), true);
                    Task.Run(() =>
                    {
                        m_Editor.FilePath = dlg.FileName;
                        m_Editor.Text = TextFile.LoadTextFile(dlg.FileName);
                    });
                }
            };

            m_Editor.Editor.RowManager.LoadingCompleted += (sender, e) =>
            {
                ShowStatus();
            };

            mnuSave = this.MenuPanel.MainMenu.FindItem("Save");
            mnuSave.Click += (object sender, EventArgs e) =>
            {
                this.LogWarning("mnuSave not implemented.");
            };

            //this.LeftSideMenu.Style.BackColorBrush = new LinearGradientBrush (Theme.Colors.Base0, Theme.Colors.Base00);

            m_Editor.Focus();
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

