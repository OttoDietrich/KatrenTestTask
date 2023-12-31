﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.

            System.Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> Statistic = new List<LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                string str = Regex.Match((stream.ReadNextChar()).ToString(), @"[a-zA-Zа-яА-Я]").Value;

                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.

                if (str != "")
                {
                    if (Statistic.Find(x => x.Letter == str).Count > 0)
                    {
                        var index = Statistic.FindIndex(x => x.Letter == str);

                        Statistic[index] = IncStatistic(Statistic[index]);
                    }
                    else
                    {
                        Statistic.Add(new LetterStats() { Letter = str, Count = 1 });
                    }
                }
            }

            return Statistic;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();

            List<LetterStats> Statistic = new List<LetterStats>();

            string prevStr = null;

            while (!stream.IsEof)
            {
                string str = Regex.Match((stream.ReadNextChar()).ToString(), @"[a-zA-Zа-яА-Я]").Value.ToUpper();

                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.

                if (str != "")
                {
                    if ((prevStr != null) && prevStr == str)
                    {
                        if (Statistic.Find(x => x.Letter == prevStr + str).Count > 0)
                        {
                            var index = Statistic.FindIndex(x => x.Letter == prevStr + str);

                            Statistic[index] = IncStatistic(Statistic[index]);
                        }
                        else
                        {
                            Statistic.Add(new LetterStats() { Letter = prevStr + str, Count = 1 });
                        }
                    }

                    prevStr = str;
                }
            }

            return Statistic;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.

            List<string> RuVowel = new List<string>() { "а", "я", "у", "ю", "о", "е", "ё", "э", "и", "ы" };

            List<string> EnVowel = new List<string>() { "a", "e", "i", "o", "u", "y" };

            switch (charType)
            {
                case CharType.Consonants:

                    for (int i = 0; i < letters.Count; i++)
                    {
                        if (!RuVowel.Contains(letters[i].Letter.ToLower()[0].ToString()) && !EnVowel.Contains(letters[i].Letter.ToLower()[0].ToString()))
                        {
                            letters.Remove(letters[i]);

                            i--;
                        }
                    }


                    break;
                case CharType.Vowel:

                    for(int i = 0; i < letters.Count; i++)
                    {
                        if (RuVowel.Contains(letters[i].Letter.ToLower()[0].ToString()) || EnVowel.Contains(letters[i].Letter.ToLower()[0].ToString()))
                        {
                            letters.Remove(letters[i]);

                            i--;
                        }

                    }

                    break;
            }

        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!

            letters = letters.OrderBy(x => x.Letter.ToLower()).ToList();

            foreach(LetterStats letter in letters)
            {
                Console.WriteLine(letter.Letter + " : " + letter.Count);
            }

            int sum = letters.Sum(x => x.Count);

            Console.WriteLine("\n" + "Итого : " + sum + "\n");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static LetterStats IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;

            return letterStats;
        }


    }
}
