using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaList
{
    class Movie : IComparable
    {
        private FileInfo file;

        public Movie(FileInfo inFile)
        {
            file = inFile;
        }


        public override string ToString()
        {
            return file.Name;
        }

        public string Name()
        {
            return file.Name;
        }

        public long Size()
        {
            return file.Length;
        }

        public string Path()
        {
            return file.FullName;
        }

        public void OpenInExplorer()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C explorer /select, \"" + file.FullName + "\"";
            process.StartInfo = startInfo;
            process.Start();
        }

        public Int32 CompareTo(Object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            Movie inMovie = (Movie)obj;
            if (inMovie != null)
            {
                return this.Name().CompareTo(inMovie.Name());
            }
            else
            {
                throw new ArgumentException("Object is not a Movie.");
            }
        }
    }
}
