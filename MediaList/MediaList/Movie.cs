// <copyright file="Movie.cs" company="corb.co">
//     corb.co. All rights reserved.
// </copyright>
// <author>Corban Mailloux</author>
namespace MediaList
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a single Movie (essentially a file).
    /// </summary>
    public class Movie : IComparable // TODO: Maybe this should extend FileInfo, as there are many passthrough methods.
    {
        /// <summary>
        /// The internal FileInfo representation of this Movie.
        /// </summary>
        private FileInfo file;

        /// <summary>
        /// Initializes a new instance of the <see cref="Movie"/> class.
        /// Essentially, store the FileInfo object.
        /// </summary>
        /// <param name="inFile">The file of this Movie.</param>
        public Movie(FileInfo inFile)
        {
            this.file = inFile;
        }

        /// <summary>
        /// Override. Use the file name for ToString.
        /// </summary>
        /// <returns>The string of the Movie's filename.</returns>
        public override string ToString()
        {
            return this.file.Name;
        }

        /// <summary>
        /// Return just the filename with extension.
        /// Ideally, this is the name of a Movie.
        /// </summary>
        /// <returns>The string of the Movie's filename.</returns>
        public string Name()
        {
            return this.file.Name;
        }

        /// <summary>
        /// Return the file size in bytes.
        /// </summary>
        /// <returns>Size of the file.</returns>
        public long Size()
        {
            return this.file.Length;
        }

        /// <summary>
        /// Return the full path.
        /// </summary>
        /// <returns>String of the full path.</returns>
        public string Path()
        {
            return this.file.FullName;
        }

        /// <summary>
        /// Open a new Windows Explorer windows to the folder that 
        /// contains this Movie, with the Movie selected.
        /// </summary>
        public void OpenInExplorer()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C explorer /select, \"" + this.file.FullName + "\"";
            process.StartInfo = startInfo;
            process.Start();
        }

        /// <summary>
        /// Compares this Movie to another.
        /// Used for sorting.
        /// </summary>
        /// <param name="obj">Object to compare this to.</param>
        /// <returns>
        /// less than 0 if this.Name() is alphabetically before the other.
        /// 0 if this and other have the same name.
        /// greater than 0 if this.Name() is alphabetically after the other.
        /// </returns>
        public int CompareTo(object obj)
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
