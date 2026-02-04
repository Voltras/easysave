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
    public class BackupJob
    {

        //this class is here in order to define what is a job ands its attributes
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public BackupType Type { get; set; }

        public BackupJob(string name, string sourcepath, string tagetpath, BackupType typeofbackup)
        {
            Name = name;
            SourcePath = sourcepath;
            TargetPath = tagetpath;
            Type = typeofbackup;
        }
    }
}
