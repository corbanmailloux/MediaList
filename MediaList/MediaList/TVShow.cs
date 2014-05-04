﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
         * String representation of the highest season and episode.
         */
        private String highString = null;

        private String missingEpisodes = null;

        private String extensions = null;

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
        public String NewestEpisode()
        {
            // Avoid recreating this string if it's already been built.
            if (String.IsNullOrEmpty(highString))
            {
                int highSeason = 0;
                int highEpisode = 0;

                foreach (DirectoryInfo season in dir.EnumerateDirectories("Season *"))
                {
                    if (Int32.Parse(season.Name.Substring(7)) > highSeason)
                    {
                        highEpisode = 0;
                        foreach (FileInfo episode in season.EnumerateFiles("* - S??E?? - *"))
                        {
                            string[] split1 = episode.Name.Split(new string[] { " - S" }, StringSplitOptions.None);
                            string[] split2 = split1[1].Split(new string[] { " - " }, StringSplitOptions.None);
                            string[] split3 = split2[0].Split(new string[] { "E" }, StringSplitOptions.None);
                            if (Int32.Parse(split3[1]) > highEpisode)
                            {
                                highSeason = Int32.Parse(split3[0]);
                                highEpisode = Int32.Parse(split3[1]);
                            }
                        }
                    }
                }
                if (highSeason > 0 && highEpisode > 0)
                {
                    highString = ("S" + highSeason.ToString().PadLeft(2, '0') + "E" + highEpisode.ToString().PadLeft(2, '0'));
                }
                else
                {
                    highString = "Not available.";
                }
            }
            
            return highString;
        }

        public String MissingEpisodes()
        {
            // Avoid recreating this string if it's already been built.
            if (String.IsNullOrEmpty(missingEpisodes))
            {
                int highestSeasonNum = 0;
                List<String> missingEps = new List<string>();
                Dictionary<int, List<int>> seasonEpisodes = new Dictionary<int, List<int>>();

                foreach (DirectoryInfo season in dir.EnumerateDirectories("Season *"))
                {
                    List<int> thisSeason = new List<int>();
                    int thisSeasonNum = Int32.Parse(season.Name.Substring(7));
                    if (thisSeasonNum > highestSeasonNum) {
                        highestSeasonNum = thisSeasonNum;
                    }
                    seasonEpisodes.Add(thisSeasonNum, thisSeason);
                    
                    foreach (FileInfo episode in season.EnumerateFiles("* - S??E?? - *"))
                    {
                        string[] split1 = episode.Name.Split(new string[] { " - S" }, StringSplitOptions.None);
                        string[] split2 = split1[1].Split(new string[] { " - " }, StringSplitOptions.None);
                        string[] split3 = split2[0].Split(new string[] { "E" }, StringSplitOptions.None);
                        thisSeason.Add(Int32.Parse(split3[1]));
                    }
                }
                
                // Find if there are missing numbers in the hashset for each season.
                for (int i = 1; i <= highestSeasonNum; i++) {
                    if (seasonEpisodes.ContainsKey(i)) {
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
                    } else {
                        missingEps.Add("S" + i.ToString().PadLeft(2, '0') + "E**");
                    }
                }

                if (missingEps.Count == 0)
                {
                    missingEpisodes = "None";
                }
                else
                {
                    missingEpisodes = String.Empty;
                    bool firstDone = false;
                    foreach (string missing in missingEps)
                    {
                        if (firstDone)
                        {
                            missingEpisodes += (", ");
                        }
                        missingEpisodes += (missing);
                        firstDone = true;
                    }
                }
            }
            return missingEpisodes;
        }

        public String ExtensionsUsed()
        {
            return ("Not yet implemented.");
        }
    }
}
