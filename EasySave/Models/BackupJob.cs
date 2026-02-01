using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Models
{
    public enum BackupType
    {
        Full,
        Differential
    }
    class BackupJob
    {

        //this class is here in order to define what is a job ands its attributes
        string Name { get; set; }
        string SourcePath { get; set; }
        string TargetPath { get; set; }
        BackupType TypeOfBackup { get; set; }

        public BackupJob(string name, string sourcepath, string tagetpath, BackupType typeofbackup)
        {
            name = Name;
            sourcepath = SourcePath;
            tagetpath = TargetPath;
            typeofbackup = TypeOfBackup;
        }
    }
}
