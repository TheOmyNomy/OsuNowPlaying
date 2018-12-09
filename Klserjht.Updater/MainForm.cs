using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Klserjht.Updater
{
    public partial class MainForm : Form
    {
        private WebClient _client;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                var currentFolder = System.Reflection.Assembly.GetExecutingAssembly().Location;
                currentFolder = currentFolder.Remove(currentFolder.LastIndexOf('\\') + 1);

                var updateFolder = $"{currentFolder}_update\\";
                if (Directory.Exists(updateFolder)) Directory.Delete(updateFolder, true);

                var updateFolderInfo = Directory.CreateDirectory(updateFolder);
                updateFolderInfo.Attributes |= FileAttributes.Hidden;

                Version latestVersion;
                string latestFile = null;

                _client = new WebClient
                {
                    Headers =
                    {
                        [HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2"
                    }
                };

                _client.DownloadProgressChanged += (i, j) =>
                {
                    _progressBar.BeginInvoke((Action)(() => _progressBar.Value = j.ProgressPercentage));
                };

                _client.DownloadStringCompleted += (i, j) =>
                {
                    _progressLabel.BeginInvoke((Action)(() => _progressLabel.Text = "Downloading..."));
                    _progressBar.BeginInvoke((Action)(() => _progressBar.Value = 0));

                    latestVersion = new Version(Newtonsoft.Json.JsonConvert.DeserializeObject<Version>(j.Result).TagName);
                    latestFile = $"{updateFolder}Klserjht-{latestVersion}.zip";

                    _client.DownloadFileAsync(new Uri($"https://github.com/TheOmyNomy/Klserjht/releases/download/{latestVersion}/Klserjht.zip"), latestFile);
                };

                _client.DownloadFileCompleted += (i, j) =>
                {
                    _progressLabel.BeginInvoke((Action)(() => _progressLabel.Text = "Extracting..."));
                    _progressBar.BeginInvoke((Action)(() => _progressBar.Value = 0));

                    System.IO.Compression.ZipFile.ExtractToDirectory(latestFile, updateFolder);
                    _progressBar.BeginInvoke((Action)(() => _progressBar.Value = 50));

                    _progressLabel.BeginInvoke((Action)(() => _progressLabel.Text = "Updating..."));

                    var files = Directory.EnumerateFiles(updateFolder);
                    foreach (var file in files)
                    {
                        if (file == latestFile) continue;

                        var name = file.Substring(file.LastIndexOf('\\') + 1);
                        if (name == "Newtonsoft.Json.dll" || name == "Klserjht.Updater.exe") continue;

                        var path = currentFolder + name;

                        File.Delete(path);
                        File.Move(file, path);
                    }

                    _progressLabel.BeginInvoke((Action)(() => _progressLabel.Text = "Complete."));
                    _progressBar.BeginInvoke((Action)(() => _progressBar.Value = 100));

                    Thread.Sleep(1000);

                    System.Diagnostics.Process.Start("Klserjht.exe");
                    BeginInvoke((Action)Close);
                };

                _client.DownloadStringAsync(new Uri("https://api.github.com/repos/TheOmyNomy/Klserjht/releases/latest"));
            }).Start();
        }
    }
}
