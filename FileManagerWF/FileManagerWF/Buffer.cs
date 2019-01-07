using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagerWF
{
    public enum TransferType { Copy, Cut }

    public class Buffer
    {
        private List<string> _files;
        private List<string> _folders;
        public TransferType Type { get; set; }

        public Buffer()
        {
            _files = new List<string>();
            _folders = new List<string>();
        }

        public void AddFile(string name)
        {
            _files.Add(name);
        }

        public void AddFolder(string name)
        {
            _folders.Add(name);
        }

        public List<string> GetFiles()
        {
            return _files;
        }

        public List<string> GetFolders()
        {
            return _folders;
        }

        public void Clear()
        {
            _files.Clear();
            _folders.Clear();
        }
    }
}
