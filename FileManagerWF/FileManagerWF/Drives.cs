using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileManagerWF
{
    public class Drives
    {
        public List<DriveInfo> Disks { get; set; }

        public Drives()
        {
            Disks = new List<DriveInfo>();
            SetDrives();
        }

        public void SetDrives()
        {
            Disks.Clear();
            foreach (var i in DriveInfo.GetDrives())
            {
                if (i.IsReady)
                {
                    Disks.Add(i);
                }
            }
        }
    }
}
