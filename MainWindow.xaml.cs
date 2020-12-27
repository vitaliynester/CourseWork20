using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using CourseWork20.Controls;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CourseWork20
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FileInfo[] Files { get; set; }
        private RSACrypt rsaCrypt;
        private string filePath = "C:/";
        private bool isFile = false;
        private string currentlySelectedItemName = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            rsaCrypt = new RSACrypt();
        }

        private void encrypt_item_Click(object sender, RoutedEventArgs e)
        {
            var new_path = $"{filePath}/{currentlySelectedItemName}";
            var fileList = new DirectoryInfo(new_path);
            FileInfo[] files = fileList.GetFiles();
            Files = files;
            
            Task.Run(() => {
                Dispatcher.Invoke(() =>
                {
                    var window = new ProgressWinow(new_path, Files, CryptChoise.Encrypt);
                    window.ShowDialog();
                });
            });
        }

        private void decrypt_item_Click(object sender, RoutedEventArgs e)
        {
            var new_path = $"{filePath}/{currentlySelectedItemName}";
            var fileList = new DirectoryInfo(new_path);
            FileInfo[] files = fileList.GetFiles();
            Files = files;

            Task.Run(() => {
                Dispatcher.Invoke(() =>
                {
                    var window = new ProgressWinow(new_path, Files, CryptChoise.Decrypt);
                    window.ShowDialog();
                });
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tb_path.Text = filePath;
            loadFilesAndDirectories();
        }

        public void loadFilesAndDirectories()
        {
            DirectoryInfo fileList;
            try
            {
                if (isFile)
                {
                    var new_path = filePath + "/" + currentlySelectedItemName;
                    Process.Start(new_path);
                }
                else
                {
                    fileList = new DirectoryInfo(filePath);
                    FileInfo[] files = fileList.GetFiles();
                    Files = files;
                    DirectoryInfo[] dirs = fileList.GetDirectories();
                    listItems.SelectedItem = null;
                    listItems.Items.Clear();

                    for (int i = 0; i < dirs.Length; i++)
                    {
                        listItems.Items.Add(new ListItemFM(dirs[i].Name, Properties.Resources.folder));
                    }

                    for (int i = 0; i < files.Length; i++)
                    {
                        listItems.Items.Add(new ListItemFM(files[i].Name, Properties.Resources.file));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_to_Click(object sender, RoutedEventArgs e)
        {
            filePath = tb_path.Text;
            loadFilesAndDirectories();
            isFile = false;
            currentlySelectedItemName = string.Empty;
        }

        private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentlySelectedItemName == string.Empty || (e.AddedItems.Count > 0 && currentlySelectedItemName != e?.AddedItems[0]?.ToString()))
            {
                currentlySelectedItemName = ((ListItemFM)e.AddedItems[0]).Path;
                var new_path = filePath + "/" + currentlySelectedItemName;
                FileAttributes fileAttr = File.GetAttributes(new_path);

                if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    isFile = false;
                    tb_path.Text = new_path;
                }
                else
                {
                    isFile = true;
                }
            }
        }

        private void listItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            removeBackSlash();
            filePath = tb_path.Text;
            loadFilesAndDirectories();
            isFile = false;
        }

        public void removeBackSlash()
        {
            string path = tb_path.Text;
            if (path.LastIndexOf("/") == path.Length - 1)
            {
                tb_path.Text = path.Substring(0, path.Length - 1);
            }
        }

        private void btn_up_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                removeBackSlash();
                string path = tb_path.Text;
                path = path.Substring(0, path.LastIndexOf("/"));
                isFile = false;
                tb_path.Text = path;
                removeBackSlash();
                filePath = tb_path.Text;
                loadFilesAndDirectories();
                currentlySelectedItemName = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
