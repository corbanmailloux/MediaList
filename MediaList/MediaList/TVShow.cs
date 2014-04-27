using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaList
{
    class TVShow : IComparable
    {
        private DirectoryInfo dir;

        public TVShow(DirectoryInfo inDir)
        {
            dir = inDir;
        }

        public override string ToString()
        {
            return dir.Name;
        }

        public string Name()
        {
            return dir.Name;
        }

        public string Path()
        {
            return dir.FullName;
        }

        public void OpenInExplorer()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C explorer /select, \"" + dir.FullName + "\"";
            process.StartInfo = startInfo;
            process.Start();
        }

        public Int32 CompareTo(Object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            TVShow inShow = (TVShow)obj;
            if (inShow != null)
            {
                return this.Name().CompareTo(inShow.Name());
            }
            else
            {
                throw new ArgumentException("Object is not a TVShow.");
            }
        }

        public string NewestEpisode()
        {
            return "Not yet available.";
        }
    }
}
