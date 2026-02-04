using EasySave.Models;
using EasySave.Services;
using System;
using System.IO;

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
                manager.RunSequential(0,2);
            }
            else
            {
                Console.WriteLine("mauvaise entrée");
            }

        }
    }
}