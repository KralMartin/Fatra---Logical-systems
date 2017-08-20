using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_v1
{
    class MyFile
    {
        internal string FileName { get; private set; }
        internal string FullFileName { get; private set; }
        internal DateTime LastTimeAccessed { get; private set; }

        internal MyFile(string fullFileName)
        {
            this.FullFileName = fullFileName;
            this.FileName = Path.GetFileNameWithoutExtension(fullFileName);
            this.LastTimeAccessed = File.GetLastAccessTime(fullFileName);
            this.LastTimeAccessed = File.GetLastWriteTime(fullFileName);
        }
    }
}
