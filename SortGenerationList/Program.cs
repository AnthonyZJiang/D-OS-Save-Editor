using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SortGenerationList
{
    internal class Program
    {
        private const string FileName = "GenerationBoosts.txt";
        private const string OneDriveFileName = @"G:\OneDrive\Public\D-OS SE Resource\GenerationBoosts.txt";

        [STAThread]
        private static void Main(string[] args)
        {
            if (File.Exists(FileName))
            {
                Console.WriteLine($"{FileName} found.");
            }
            else
            {
                Console.WriteLine($"{FileName} not found. Press any key to exit.");
                Console.ReadKey();
                return;
            }
            if (File.Exists(OneDriveFileName))
            {
                Console.WriteLine($"{OneDriveFileName} found.");
            }
            else
            {
                Console.WriteLine($"{OneDriveFileName} not found. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n******************************");

            string cmd;
            if (args.Length == 0)
            {
                Console.WriteLine("Type \"ADD\" to add a new boost, \"ADDC\" to add from clipboard, or \"SORT\" to sort existing list.");
                cmd = Console.ReadLine();    
            }
            else
            {
                cmd = args[0];
            }

            Console.WriteLine("\n******************************");
            switch (cmd)
            {
                case "ADD":
                    Console.WriteLine("Boost entry: ");
                    AddBoosts(new [] {Console.ReadLine()});
                    break;
                case "ADDC":
                    var s = Clipboard.GetText();
                    var list = s.Split(new[] {"\r\n","\n"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var l in list)
                    {
                        Console.WriteLine(l);
                    }

                    if (list.Length <= 0)
                    {
                        Console.WriteLine("No entry has been found in clipboard. Press any key to exit.");
                        Console.ReadKey();
                        return;
                    }

                    Console.Write($"{list.Length} entries have been found. Type \"Y\" to continue or any other key to exit: ");
                    if (Console.ReadLine() == "Y")
                        AddBoosts(list.ToArray());
                    break;
                case "SORT":
                    SortBoosts();
                    break;
                default:
                        return;
            }
        }

        private static void AddBoosts(string[] boosts)
        {
            Console.WriteLine("\nA******************************");
            var list = new List<string>();
            using (var sr = new StreamReader(FileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }

            var prevLineCount = list.Count;
            Console.WriteLine($"{prevLineCount} existing boosts found.\n");

            foreach (var b in boosts)
            {
                if (list.Contains(b))
                {
                    Console.WriteLine($"*-->{b}<-- is duplicated.");
                    continue;
                }
                list.Add(b);
                Console.WriteLine($"-->{b}<-- will be added.");
            }
            Console.WriteLine($"{list.Count-prevLineCount} new boosts found.\n");

            if (list.Count - prevLineCount <= 0)
            {
                Console.WriteLine("Files have not been changed or updated. Press any key to exit.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Type \"Y\" to continue or any other key to exit: ");
            if (Console.ReadLine() != "Y")
                return;
            
            list.Sort();
            WriteBoosts(list);
        }

        private static void SortBoosts()
        {
            Console.WriteLine("\n******************************");
            var list = new List<string>();
            var totalCount = 0;
            using (var sr = new StreamReader(FileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    totalCount++;
                    if (list.Contains(line))
                    {
                        Console.WriteLine($"\"{line}\" is duplicated.");
                        continue;
                    }

                    list.Add(line);
                }
            }
            Console.WriteLine($"Total count: {totalCount}");
            Console.WriteLine($"Unique count: {list.Count}");

            list.Sort();
            WriteBoosts(list);
        }

        private static void WriteBoosts(IEnumerable<string> boosts)
        {
            Console.WriteLine("\n******************************");
            // make a backup
            if (File.Exists("GenerationBoosts.bak")) File.Delete("GenerationBoosts.bak");
            File.Move(FileName, "GenerationBoosts.bak");

            // overwrite 
            using (var sw = new StreamWriter(FileName, false))
            {
                foreach (var b in boosts)
                {
                    sw.WriteLine(b);
                }
            }
            File.Copy(FileName, OneDriveFileName, true);
            Console.WriteLine("Files have been updated. Press any key to exit.");

            Console.ReadKey();
        }
    }
}
