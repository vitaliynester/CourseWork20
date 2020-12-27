using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CourseWork20
{
    public partial class ProgressWinow : Window
    {
        public string Path { get; set; }
        public FileInfo[] Files { get; set; }
        public CryptChoise Choise { get; set; }
        public BackgroundWorker Worker { get; set; }
        public RSACrypt rsa { get; set; }
        public int CurrentProgress { get; set; }
        public Stopwatch Watch { get; set; }
        public ProgressWinow()
        {
            InitializeComponent();
        }

        public ProgressWinow(string path, FileInfo[] files, CryptChoise choise)
        {
            InitializeComponent();
            CurrentProgress = 0;
            Path = path;
            Files = files;
            Choise = choise;
            gProgressBar.Maximum = files.Length;
            rsa = new RSACrypt();
            initWorker();
            if (Choise == CryptChoise.Encrypt)
            {
                Title = $"Шифрование {Path}";
            }
            else if (Choise == CryptChoise.Decrypt)
            {
                Title = $"Дешифровка {Path}";
            }
        }

        private void initWorker()
        {
            Worker = new BackgroundWorker();
            Worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            Worker.WorkerReportsProgress = true;
            Worker.DoWork += worker_DoWork;
            Worker.ProgressChanged += worker_ProgressChanged;
            Worker.RunWorkerAsync();
        }

        public void startCrypt(FileInfo file)
        {
            try
            {
                var pathOfFile = $"{Path}/{file}";
                if (File.Exists(pathOfFile))
                {
                    var encrypt = rsa.Encrypt(File.ReadAllBytes(pathOfFile));
                    File.WriteAllBytes($"{Path}_Crypted/{file}", encrypt);
                    encrypt = null;
                    GC.Collect();
                }
                Worker.ReportProgress(CurrentProgress++, $"Шифрование файла {file} завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void startDecrypt(FileInfo file)
        {
            try
            {
                var pathOfFile = $"{Path}/{file}";
                if (File.Exists(pathOfFile))
                {
                    var encrypt = rsa.Decrypt(File.ReadAllBytes(pathOfFile));
                    File.WriteAllBytes($"{Path}_Decrypted/{file}", encrypt);
                    encrypt = null;
                    GC.Collect();
                }
                Worker.ReportProgress(CurrentProgress++, $"Дешифровка файла {file} завершена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            gProgressBar.Value = e.ProgressPercentage;
            tb_status.Text = (string)e.UserState;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Choise == CryptChoise.Encrypt)
            {
                Worker.ReportProgress(CurrentProgress, $"Началось шифрование файлов");
                Watch = Stopwatch.StartNew();
                Directory.CreateDirectory($"{Path}_Crypted");
                Parallel.ForEach(Files, (file) =>
                {
                    startCrypt(file);
                });
            }
            else if (Choise == CryptChoise.Decrypt)
            {
                Worker.ReportProgress(CurrentProgress, $"Началась дешифровка файлов");
                Watch = Stopwatch.StartNew();
                Directory.CreateDirectory($"{Path}_Decrypted");
                Parallel.ForEach(Files, (file) =>
                {
                    startDecrypt(file);
                });
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Watch.Stop();
            var elapsedMs = Watch.ElapsedMilliseconds;
            string msg = Choise == CryptChoise.Encrypt ? "Шифрование завершено" : "Дешифрование завершено";
            msg += $"\nЗатраченное время: {elapsedMs} мсек";
            MessageBox.Show(msg);
            tb_status.Text = msg;
            gProgressBar.Value = Files.Length;
        }
    }
}
