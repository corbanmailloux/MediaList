using System;
using System.IO;

namespace MediaList
{
    /*
     * TODO: Maybe this should extend DirectoryInfo, as there are many passthrough methods.
     */
    class TVShow : IComparable
    {
        /*
         * The internal DirectoryInfo representation of this TVShow.
         */
        private DirectoryInfo dir;

        /*
         * Constructor. 
         * Essentially, store the DirectoryInfo object.
         */
        public TVShow(DirectoryInfo inDir)
        {
            dir = inDir;
        }

        /*
         * Override. Use the folder name for ToString.
         */
        public override string ToString()
        {
            return dir.Name;
        }

        /*
         * Return just the folder name.
         * Ideally, this is the name of a TV Show.
         */
        public string Name()
        {
            return dir.Name;
        }

        /*
         * Return the full path.
         */
        public string Path()
        {
            return dir.FullName;
        }

        /*
         * Open a new Windows Explorer windows to the folder that 
         *  contains this TV show folder, with the TV show folder selected.
         */
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

        /*
         * Compares this TVShow to another.
         * Used for sorting.
         * Returns:
         *  < 0 if this.Name() is alphebetically before the other.
         *  0 if this and other have the same name.
         *  > 0 if this.Name() is alphebetically after the other.
         */
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

        /*
         * Go through each folder in the TV Show and find the highest numbered episode.
         */
        public string NewestEpisode()
        {
            return "Not yet available.";
        }
    }
}
