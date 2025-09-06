using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

class FileSorter
{
    static string type = ".png";

    static string path = @"C:\Users\dmitr\OneDrive\Desktop\DataSet\DataSet\Cards\";

    static void Main(string[] args)
    {
        // --- НАСТРОЙКИ ---
        // Укажите путь к исходной папке с файлами.


        string sourceDirectory = path +"images";

        // Укажите путь к папке для первой группы PNG файлов.
        string pngDestination1 = path + @"images\train";

        // Укажите путь к папке для второй группы PNG файлов.
        string pngDestination2 = path + @"images\val";

        // Укажите путь к папке для первой группы TXT файлов.
        string txtDestination1 = path +@"\labels\train";

        // Укажите путь к папке для второй группы TXT файлов.
        string txtDestination2 = path + @"\labels\val";

        // Задайте процент файлов, который пойдет в первую группу (например, 0.7 = 70%).
        double percentageForFirstGroup = 0.8;


        try
            {
                Console.WriteLine("Начало процесса сортировки и перемещения файлов...");

                // 1. Создаем папки назначения, если они еще не существуют.
                Directory.CreateDirectory(pngDestination1);
                Directory.CreateDirectory(pngDestination2);
                Directory.CreateDirectory(txtDestination1);
                Directory.CreateDirectory(txtDestination2);

                // 2. Получаем все PNG файлы в исходной папке.
                string[] pngFiles = Directory.GetFiles(sourceDirectory, "*"+ type);

                // 3. Находим совпадающие пары (PNG + TXT).
                List<string> matchingFileBases = new List<string>();
                foreach (string pngPath in pngFiles)
                {
                    // Получаем имя файла без расширения (например, "image1" из "image1.png").
                    string baseName = Path.GetFileNameWithoutExtension(pngPath);

                    // Формируем ожидаемый путь к TXT файлу.
                    string txtPath = Path.Combine(sourceDirectory, baseName + ".txt");

                    // Проверяем, существует ли такой TXT файл.
                    if (File.Exists(txtPath))
                    {
                        matchingFileBases.Add(baseName);
                    }
                }

                // 4. Выводим общее количество найденных пар.
                int totalMatches = matchingFileBases.Count;
                Console.WriteLine($"Найдено совпадающих пар (JPG + TXT): {totalMatches}");

                if (totalMatches == 0)
                {
                    Console.WriteLine("Нет файлов для перемещения. Завершение работы.");
                    Console.ReadKey();
                    return;
                }

                // 5. Перемешиваем список, чтобы выборка была случайной.
                Random rng = new Random();
                List<string> shuffledBases = matchingFileBases.OrderBy(a => rng.Next()).ToList();

                // 6. Рассчитываем, сколько файлов пойдет в первую группу.
                int countForFirstGroup = (int)Math.Round(totalMatches * percentageForFirstGroup);
                int countForSecondGroup = totalMatches - countForFirstGroup;

                // ИЗМЕНЕНИЕ: Обновлен текст сообщения
                Console.WriteLine($"Будет перемещено в первую группу: {countForFirstGroup} пар.");
                Console.WriteLine($"Будет перемещено во вторую группу: {countForSecondGroup} пар.");

                // 7. Делим список на две группы.
                List<string> firstGroup = shuffledBases.Take(countForFirstGroup).ToList();
                List<string> secondGroup = shuffledBases.Skip(countForFirstGroup).ToList();

                // 8. Перемещаем файлы первой группы.
                // ИЗМЕНЕНИЕ: Обновлен текст сообщения и вызов метода
                Console.WriteLine("\n--- Перемещение файлов первой группы ---");
                foreach (string baseName in firstGroup)
                {
                    MoveFilePair(baseName, sourceDirectory, pngDestination1, txtDestination1);
                }

                // 9. Перемещаем файлы второй группы.
                // ИЗМЕНЕНИЕ: Обновлен текст сообщения и вызов метода
                Console.WriteLine("\n--- Перемещение файлов второй группы ---");
                foreach (string baseName in secondGroup)
                {
                    MoveFilePair(baseName, sourceDirectory, pngDestination2, txtDestination2);
                }

                // ИЗМЕНЕНИЕ: Обновлен текст сообщения
                Console.WriteLine("\nПроцесс перемещения успешно завершен!");
            }
            catch (Exception ex)
            {
                // В случае ошибки (например, неправильный путь) выводим сообщение.
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }

            // Ожидаем нажатия клавиши, чтобы консольное окно не закрылось сразу.
            Console.WriteLine("\nНажмите любую клавишу для выхода.");
            Console.ReadKey();
        }

        /// <summary>
        /// ИЗМЕНЕНИЕ: Название и описание метода обновлены.
        /// Вспомогательный метод для перемещения пары файлов (PNG и TXT).
        /// </summary>
        /// <param name="baseName">Имя файла без расширения.</param>
        /// <param name="sourceDir">Исходная папка.</param>
        /// <param name="pngDestDir">Папка назначения для PNG.</param>
        /// <param name="txtDestDir">Папка назначения для TXT.</param>
        static void MoveFilePair(string baseName, string sourceDir, string pngDestDir, string txtDestDir)
        {
            // Формируем полные пути к исходным и целевым файлам
            string sourcePng = Path.Combine(sourceDir, baseName + type);
            string sourceTxt = Path.Combine(sourceDir, baseName + ".txt");

            string destPng = Path.Combine(pngDestDir, baseName + type);
            string destTxt = Path.Combine(txtDestDir, baseName + ".txt");

            try
            {
            // ИЗМЕНЕНИЕ: File.Copy заменен на File.Move.
            // Третий параметр 'true' означает, что файл будет перезаписан, если он уже существует в папке назначения.
            File.Move(sourcePng, destPng, true);
                File.Move(sourceTxt, destTxt, true);

                // ИЗМЕНЕНИЕ: Обновлен текст сообщения
                Console.WriteLine($"Перемещена пара: {baseName}.png + {baseName}.txt");
            }
            catch (Exception ex)
            {
                // ИЗМЕНЕНИЕ: Обновлен текст сообщения
                Console.WriteLine($"Не удалось переместить пару {baseName}: {ex.Message}");
            }
        }
    }