using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace CleanerApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Структура для хранения информации о файле
        public class JunkFile
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        // Сканируем диск C: на мусорные файлы
        private async void btnScan_Click(object sender, RoutedEventArgs e)
        {
            var junkFiles = await ScanForJunkFilesAsync("C:\\"); // Начинаем с диска C
            listViewFiles.ItemsSource = junkFiles;
        }

        // Асинхронная операция для сканирования
        private async Task<List<JunkFile>> ScanForJunkFilesAsync(string directory)
        {
            List<JunkFile> junkFiles = new List<JunkFile>();
            string[] extensions = { "*.tmp", "*.log", "*.bak" };

            try
            {
                var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                                      .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()));

                foreach (var file in files)
                {
                    try
                    {
                        // Проверим, можем ли мы получить доступ к файлу
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Exists)
                        {
                            junkFiles.Add(new JunkFile
                            {
                                FileName = Path.GetFileName(file),
                                FilePath = file
                            });
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Обрабатываем ошибку доступа, если файл заблокирован системой
                        // Можно добавить логирование, чтобы узнать, какой файл не удалось проверить
                    }
                    catch (Exception ex)
                    {
                        // Логируем любые другие ошибки
                        MessageBox.Show($"Ошибка при обработке файла {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сканирования: " + ex.Message);
            }

            return junkFiles;
        }

        // Удаление выбранных файлов
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedFiles = listViewFiles.SelectedItems.Cast<JunkFile>().ToList();

            foreach (var file in selectedFiles)
            {
                try
                {
                    File.Delete(file.FilePath); // Удаляем файл
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления: " + ex.Message);
                }
            }

            MessageBox.Show("Файлы удалены.");
        }
    }
}