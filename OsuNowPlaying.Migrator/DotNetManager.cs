using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OsuNowPlaying.Migrator
{
	public static class DotNetManager
	{
		private const string Channel = "6.0";

		private static HttpClient _client;

		private static HttpClient Client
		{
			get
			{
				if (_client == null)
				{
					_client = new HttpClient();
					_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; rv:91.0) Gecko/20100101 Firefox/91.0");
				}

				return _client;
			}
		}

		public static bool IsInstalled
		{
			get
			{
				string windowsDesktopRuntimesPath =
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "shared", "Microsoft.WindowsDesktop.App");

				if (!Directory.Exists(windowsDesktopRuntimesPath))
					return false;

				foreach (string folder in Directory.EnumerateDirectories(windowsDesktopRuntimesPath))
				{
					string version = folder.Substring(folder.LastIndexOf('\\') + 1);

					if (version.StartsWith(Channel))
						return true;
				}

				return false;
			}
		}

		private static readonly string DownloadedExecutablePath = Path.Combine(App.WorkingPath, "windowsdesktop-runtime-win-x64.exe");

		public static async Task DownloadAsync()
		{
			File.Delete(DownloadedExecutablePath);

			Stream stream = await Client.GetStreamAsync($"https://aka.ms/dotnet/{Channel}/windowsdesktop-runtime-win-x64.exe");
			FileStream fileStream = new FileStream(DownloadedExecutablePath, FileMode.Create);

			await stream.CopyToAsync(fileStream);
			await fileStream.FlushAsync();

			fileStream.Close();
			stream.Close();
		}

		public static bool Install()
		{
			if (!File.Exists(DownloadedExecutablePath))
				return false;

			Process installProcess = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = DownloadedExecutablePath,
					Arguments = "/quiet /install",
					Verb = "runas"
				}
			};

			bool result = installProcess.Start();

			if (!result)
				return false;

			installProcess.WaitForExit();
			return true;
		}
	}
}