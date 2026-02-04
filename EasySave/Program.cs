using System;
using EasySave.Services;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialisation du managerr
            BackupManager manager = new BackupManager();



            Console.WriteLine("Console Mode");
            Console.WriteLine("entrer le numéro du travail (ex: 1) :");

            string input = Console.ReadLine(); 
            if (int.TryParse(input, out int jobIndex))
            {
                manager.RunJob(jobIndex);
            }
            else
            {
                Console.WriteLine("mauvaise entrée");
            }

            Console.WriteLine("appuyez sur nimporte quelle touche pour quitter...");
            Console.ReadKey();
        }
    }
}