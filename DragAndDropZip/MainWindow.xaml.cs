using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Compression;
using Path = System.IO.Path;

namespace DragAndDropZip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string,Encoding> encodings = new Dictionary<string, Encoding> {
            { "UTF8",Encoding.UTF8}
            ,{ "Unicode",Encoding.Unicode}
            ,{ "ASCII",Encoding.ASCII},};
        private Encoding currentEncoding = Encoding.Unicode;
        public MainWindow()
        {
            InitializeComponent();
            EncodingSelection.ItemsSource = encodings.Keys;
            EncodingSelection.SelectedItem = encodings.Keys.First();
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            var zips = new Dictionary<string,string>();
            var data = (string [])e.Data.GetData(DataFormats.FileDrop);
            var result = ZipHelper.OpenArchive(data.First(),zips);
            var selectedLogsText = new List<string>();
            foreach (var file in data)
            {

            }
            if (zips.Count > 0)
                 
            foreach (var logText in selectedLogsText)
            {
                TextBox.Text += string.Format("\n"+logText);
            }
        }

        private string ReadFile(string fileName)
        {
            string textFromFile = string.Empty;
            using (var stream  = new FileStream(fileName,FileMode.Open))
            {
                // преобразуем строку в байты
                var array = new byte[stream.Length];
                // считываем данные
                stream.Read(array, 0, array.Length);
                // декодируем байты в строку
                textFromFile = Encoding.Default.GetString(array);
            }
            return textFromFile;
        }

        private string ChangeEncoding(string text)
        {

            return text;
        }

        private void EncodingSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox.Text)) return;
            var previousEncoding = currentEncoding;
            currentEncoding = encodings.First(x => x.Key  == (string)EncodingSelection.SelectedItem).Value;
            var previousEncodingBytes = previousEncoding.GetBytes(TextBox.Text);
            var newEncodingBytes = Encoding.Convert(previousEncoding, currentEncoding, previousEncodingBytes);
            TextBox.Text = currentEncoding.GetString(newEncodingBytes);
        }

        private List<string> ExtractZips(List<string> zips,string path)
        {
            var logsText = new List<string>();
            foreach (var zip in zips)
            {
                var tempDir = Directory.CreateDirectory(Path.GetFileNameWithoutExtension(zip));
                using (ZipArchive archive = ZipFile.OpenRead(zip))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            var pathOfNewFile = Path.Combine(tempDir.FullName, Path.GetFileNameWithoutExtension(entry.FullName) + "Temp" + Path.GetExtension(entry.FullName));
                            entry.ExtractToFile(pathOfNewFile);
                            var file = new FileInfo(pathOfNewFile);
                            logsText.Add(ReadFile(file.FullName));
                            File.Delete(pathOfNewFile);
                        }
                    }
                }
                Directory.Delete(tempDir.FullName);
            }
            return logsText;
        }
    }
}
