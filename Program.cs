using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Globalization;
using KS.Foundation;
using SummerGUI;

namespace SummerEdit
{
	class Program
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool SetDllDirectory(string path);

		public static Queue<Exception> StartupExceptions = new Queue<Exception>();
		static void InitApplication()
		{			
			string name = GetAppName ();
			InitProgramFolders (name);
			InitConfiguration (name);

			//public static clsLanguageSelection LanguageSelection = null;
			//CultureInfo StartUpUICulture;

			// Setup the GUI language
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

			CultureInfo.DefaultThreadCurrentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo.DefaultThreadCurrentUICulture = Thread.CurrentThread.CurrentUICulture;
		}

		static bool MakeDir(string path)
		{
			if (String.IsNullOrEmpty(path)) 
				return false;
			path = path.BackSlash(false);
			if (Strings.DirExists(path))
			{
				return true;
			}
			else
			{
				try
				{
					Directory.CreateDirectory(path);
				}
				catch (Exception ex)
				{					
					ex.LogError ();
					StartupExceptions.Enqueue (ex);
					return false;
				}

				return true;
			}
		}

		static string GetAppName()
		{
			//return Assembly.GetExecutingAssembly ().GetName ().Name;
			return "SummerEdit";
		}

		public static string ApplicationRoot { get; protected set; }
		static void InitProgramFolders (string appName)
		{		
			if (Environment.OSVersion.Platform == PlatformID.Unix)
				appName = "." + appName.ToLowerInvariant ();

			string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

			if (Strings.DirExists(path)) {
				path = Path.Combine(path, appName);
				if (!MakeDir(path)) {
					path = Strings.ApplicationPath(true);
				}
			} else {
				path = Strings.ApplicationPath(true);
			}
			ApplicationRoot = path.BackSlash(true);
		}			

		static void InitConfiguration(string appName)
		{
			string configFileName = null;
			string rootDir = ApplicationRoot;
			if (String.IsNullOrEmpty (rootDir) || !Directory.Exists (rootDir))
				rootDir = Strings.ApplicationPath (false);

			configFileName = Path.Combine (rootDir, appName + ".conf");
			ConfigurationService.Instance.InitConfig (configFileName);
		}

        public static SummerGUIWindow MainWindow { get; private set; }

        [STAThread]
        static void Main(string[] args)
		{
			// Setup logging
			Logging.SetupLogging (LogLevels.Verbose, LogTargets.Console);

			// Setup global exception handlers
			AppDomain.CurrentDomain.UnhandledException += ExceptionUnhandled;
			AppDomain.CurrentDomain.FirstChanceException += AppDomain_CurrentDomain_FirstChanceException;            		

			// This step is required.
			// Since we can't make this static class inheritable,
			// you have to do it here, until we find a smarter solution
			int pf = (int)Environment.OSVersion.Platform;
			if (pf != 4 && pf != 6 && pf != 128)
			{
				string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				path = Path.Combine(path, IntPtr.Size == 8 ? "x64" : "x86");
				if (!SetDllDirectory(path))
					throw new System.ComponentModel.Win32Exception();
			}

			InitApplication ();

            // >>> Bug in OpenTK: Program hangs on Exit, when using this option..
            // don't use SDL2
            //OpenTK.ToolkitOptions.Default.Backend = OpenTK.PlatformBackend.PreferNative;
            //OpenTK.Toolkit.Init ();

            // testing the ThemeLoader class
            //ThemeLoader loader = new ThemeLoader (Strings.ApplicationPath (true) + "ColorTheme.config");

            using (MainForm wnd = new MainForm ()) {
				MainWindow = wnd;
				// The frame rate is throttled to a value between 30 and 60 Hz
				// depending on the monitor device refresh rate.
				// You can call another overload to set these explicitly
				wnd.Run ();
			}
		}			

		static void ExceptionUnhandled(object sender, UnhandledExceptionEventArgs args)
		{
			Exception e = (Exception) args.ExceptionObject;
			e.LogFatal ("UNHANDLED Exception");
		}

		static void AppDomain_CurrentDomain_FirstChanceException (object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
		{
			e.Exception.LogWarning ("First-Chance Exception");
		}
	}
}
