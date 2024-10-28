using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrimeCheckerDZ
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // проверка на простоту вручную введенного числа
        private async void BtnCheckPrime_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtNumberInput.Text, out int number))
            {
                lblStatus.Content = "Проверка...";
                bool isPrime = await Task.Run(() => IsPrime(number)); //сама проверка
                string result = $"{number} - {(isPrime ? "простое" : "не простое")}";

                lstResults.Items.Add(result);
                AppendToFile(result);
                lblStatus.Content = "Готово";
            }
            else
            {
                MessageBox.Show("Введите корректное число.");
            }
        }

        // загрузка файла и проверка чисел на простоту
        private async void BtnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog //открываем дмалог
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                lblStatus.Content = "Загрузка и проверка чисел...";
                List<int> numbers = new List<int>();

                // считываем числа из файла
                using (StreamReader reader = new StreamReader(openFileDialog.FileName, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (int.TryParse(line, out int number))
                        {
                            numbers.Add(number);
                        }
                    }
                }

                // асинхронно проверяем каждое число
                foreach (var number in numbers)
                {
                    await Task.Run(() => CheckAndDisplayPrime(number));
                }

                lblStatus.Content = "Готово"; 
            }
        }

        // проверка и отображение результата для числа
        private void CheckAndDisplayPrime(int number)
        {
            bool isPrime = IsPrime(number); 
            string result = $"{number} - {(isPrime ? "простое" : "не простое")}";

            Dispatcher.Invoke(() =>
            {
                lstResults.Items.Add(result);
                AppendToFile(result);
            });
        }

        // просто проверка числа на простоту
        private bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number <= 3) return true;

            if (number % 2 == 0 || number % 3 == 0) return false;

            for (int i = 5; i * i <= number; i += 6)
            {
                if (number % i == 0 || number % (i + 2) == 0) return false;
            }
            return true;
        }

        // добавление результ в файл
        private void AppendToFile(string result)
        {
            string filePath = "PrimeCheckResults.txt";
            using (StreamWriter writer = new StreamWriter(filePath, append: true, Encoding.UTF8))
            {
                writer.WriteLine(result);
            }
        }

    }
}