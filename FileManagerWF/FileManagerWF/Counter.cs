using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerWF
{
    public class Counter
    {
        public int FilesCount { get; private set; }
        public int DirectoriesCount { get; private set; }
        public double FolderSize { get; private set; }

        public Counter()
        {
            FilesCount = DirectoriesCount = 0;
        }

        public void Clear()
        {
            FilesCount = DirectoriesCount = 0;
        }

        public void Count(DirectoryInfo dir)
        {
            try
            {
                ArrayList files = new ArrayList();
                files.AddRange(dir.GetDirectories());
                files.AddRange(dir.GetFiles());

                foreach (var file in files)
                {
                    if (file is DirectoryInfo)
                    {
                        DirectoriesCount++;
                        Count(file as DirectoryInfo);
                    }
                    else
                    {
                        FilesCount++;
                    }
                }
            }
            catch (Exception) { }
        }

        public void CountSize(DirectoryInfo dir)
        {
            try
            {
                ArrayList files = new ArrayList();
                files.AddRange(dir.GetDirectories());
                files.AddRange(dir.GetFiles());

                foreach (var file in files)
                {
                    if (file is DirectoryInfo)
                    {
                        CountSize(file as DirectoryInfo);
                    }
                    else
                    {
                        FolderSize += (file as FileInfo).Length / Math.Pow(1024, 2);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
