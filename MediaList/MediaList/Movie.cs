using System;
using System.IO;

namespace MediaList
{
    /*
     * TODO: Maybe this should extend FileInfo, as there are many passthrough methods.
     */
    class Movie : IComparable
    {
        /*
         * The internal FileInfo representation of this Movie.
         */
        private FileInfo file;

        /*
         * Constructor. 
         * Essentially, store the FileInfo object.
         */
        public Movie(FileInfo inFile)
        {
            file = inFile;
        }

        /*
         * Override. Use the file name for ToString.
         */
        public override string ToString()
        {
            return file.Name;
        }

        /*
         * Return just the filename with extension.
         * Ideally, this is the name of a Movie.
         */
        public string Name()
        {
            return file.Name;
        }

        /*
         * Return the file size in bytes.
         */
        public long Size()
        {
            return file.Length;
        }

        /*
         * Return the full path.
         */
        public string Path()
        {
            return file.FullName;
        }

        /*
         * Open a new Windows Explorer windows to the folder that 
         *  contains this Movie, with the Movie selected.
         */
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

        /*
         * Compares this Movie to another.
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
