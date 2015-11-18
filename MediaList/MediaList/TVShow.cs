// <copyright file="TVShow.cs" company="corb.co">
//     corb.co. All rights reserved.
// </copyright>
// <author>Corban Mailloux</author>
namespace MediaList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Represents a TV Show series (essentially a directory).
    /// </summary>
    public class TVShow : IComparable // TODO: Maybe this should extend DirectoryInfo, as there are many passthrough methods.
    {
        /// <summary>
        /// The internal DirectoryInfo representation of this TVShow.
        /// </summary>
        private DirectoryInfo dir;

        /// <summary>
        /// String representation of the highest season and episode.
        /// </summary>
        private string highString = null;

        /// <summary>
        /// String representation of the show's missing episodes.
        /// </summary>
        private string missingEpisodes = null;

        /// <summary>
        /// String representation of the file extensions used in the show.
        /// </summary>
        private string extensions = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TVShow"/> class.
        /// Essentially, store the DirectoryInfo object.
        /// </summary>
        /// <param name="inDir">The folder for this show.</param>
        public TVShow(DirectoryInfo inDir)
        {
            this.dir = inDir;
        }

        /// <summary>
        /// Override. Use the folder's name for ToString.
        /// </summary>
        /// <returns>The string of the TVShow's folder's name.</returns>
        public override string ToString()
        {
            return this.dir.Name;
        }

        /// <summary>
        /// Return just the folder's name.
        /// Ideally, this is the name of a TV show.
        /// </summary>
        /// <returns>The string of the TVShow's name.</returns>
        public string Name()
        {
            return this.dir.Name;
        }

        /// <summary>
        /// Return the full path.
        /// </summary>
        /// <returns>The string of the TVShow's full path.</returns>
        public string Path()
        {
            return this.dir.FullName;
        }

        /// <summary>
        /// Open a new Windows Explorer windows to the folder that 
        /// contains this TV show folder, with the TV show folder selected.
        /// </summary>
        public void OpenInExplorer()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C explorer /select, \"" + this.dir.FullName + "\"";
            process.StartInfo = startInfo;
            process.Start();
        }

        /// <summary>
        /// Compares this TVShow to another.
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

        /// <summary>
        /// Go through each folder in the TV Show and find the highest numbered episode.
        /// </summary>
        /// <returns>String ("S??E??") of the newest episode.</returns>
        public string NewestEpisode()
        {
            // Avoid recreating this string if it's already been built.
            if (string.IsNullOrEmpty(this.highString))
            {
                int highSeason = 0;
                int highEpisode = 0;

                foreach (DirectoryInfo season in this.dir.EnumerateDirectories("Season *"))
                {
                    if (int.Parse(season.Name.Substring(7)) > highSeason)
                    {
                        highEpisode = 0;
                        foreach (FileInfo episode in season.EnumerateFiles("* - S??E?? - *"))
                        {
                            string[] split1 = episode.Name.Split(new string[] { " - S" }, StringSplitOptions.None);
                            string[] split2 = split1[1].Split(new string[] { " - " }, StringSplitOptions.None);
                            string[] split3 = split2[0].Split(new string[] { "E" }, StringSplitOptions.None);
                            if (int.Parse(split3[1]) > highEpisode)
                            {
                                highSeason = int.Parse(split3[0]);
                                highEpisode = int.Parse(split3[1]);
                            }
                        }
                    }
                }

                if (highSeason > 0 && highEpisode > 0)
                {
                    this.highString = 
                        "S" + highSeason.ToString().PadLeft(2, '0') + "E" + 
                        highEpisode.ToString().PadLeft(2, '0');
                }
                else
                {
                    this.highString = "Not available.";
                }
            }

            return this.highString;
        }

        /// <summary>
        /// Find missing episodes in the series.
        /// </summary>
        /// <returns>String of all of the missing episodes ("S??E??, S??E??")</returns>
        public string MissingEpisodes()
        {
            // Avoid recreating this string if it's already been built.
            if (string.IsNullOrEmpty(this.missingEpisodes))
            {
                int highestSeasonNum = 0;
                List<string> missingEps = new List<string>();
                Dictionary<int, List<int>> seasonEpisodes = new Dictionary<int, List<int>>();

                foreach (DirectoryInfo season in this.dir.EnumerateDirectories("Season *"))
                {
                    List<int> thisSeason = new List<int>();
                    int thisSeasonNum = int.Parse(season.Name.Substring(7));
                    if (thisSeasonNum > highestSeasonNum) 
                    {
                        highestSeasonNum = thisSeasonNum;
                    }

                    seasonEpisodes.Add(thisSeasonNum, thisSeason);
                    
                    foreach (FileInfo episode in season.EnumerateFiles("* - S??E?? - *"))
                    {
                        string[] split1 = episode.Name.Split(new string[] { " - S" }, StringSplitOptions.None);
                        string[] split2 = split1[1].Split(new string[] { " - " }, StringSplitOptions.None);
                        string[] split3 = split2[0].Split(new string[] { "E" }, StringSplitOptions.None);
                        thisSeason.Add(int.Parse(split3[1]));
                    }
                }
                
                // Find if there are missing numbers in the hashset for each season.
                for (int i = 1; i <= highestSeasonNum; i++) 
                {
                    if (seasonEpisodes.ContainsKey(i)) 
                    {
                        List<int> currEps = seasonEpisodes[i];
                        currEps.Sort();
                        int maxEpNum = currEps[currEps.Count - 1];
                        for (int j = 1; j <= maxEpNum; j++)
                        {
                            if (!currEps.Contains(j))
                            {
                                missingEps.Add("S" + i.ToString().PadLeft(2, '0') + "E" + j.ToString().PadLeft(2, '0'));
                            }
                        }
                    } 
                    else 
                    {
                        missingEps.Add("S" + i.ToString().PadLeft(2, '0') + "E**");
                    }
                }

                if (missingEps.Count == 0)
                {
                    this.missingEpisodes = "None";
                }
                else
                {
                    this.missingEpisodes = string.Empty;
                    bool firstDone = false;
                    foreach (string missing in missingEps)
                    {
                        if (firstDone)
                        {
                            this.missingEpisodes += ", ";
                        }

                        this.missingEpisodes += missing;
                        firstDone = true;
                    }
                }
            }

            return this.missingEpisodes;
        }

        /// <summary>
        /// Find all of the file extensions used by files in this series.
        /// </summary>
        /// <returns>String (".ext, .ext") of extensions used.</returns>
        public string ExtensionsUsed()
        {
            // Avoid recreating this string if it's already been built.
            if (string.IsNullOrEmpty(this.extensions))
            {
                HashSet<string> fileExts = new HashSet<string>();
                foreach (DirectoryInfo season in this.dir.EnumerateDirectories("Season *"))
                {
                    foreach (FileInfo episode in season.EnumerateFiles("* - S??E?? - *"))
                    {
                        fileExts.Add(episode.Extension.ToLower());
                    }
                }

                bool firstDone = false;
                foreach (string ext in fileExts)
                {
                    if (firstDone)
                    {
                        this.extensions += ", ";
                    }

                    this.extensions += ext;
                    firstDone = true;
                }
            }
            
            return this.extensions;
        }
    }
}
