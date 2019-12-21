using System;
using System.IO;
using System.Collections.Generic;

namespace osuDifficultyCalculator
{
    public class Beatmap
    {
        public double circleSize, overallDifficulty, approachRate,
            ezhtStarRating, nmhtStarRating, hrhtStarRating,
            ezStarRating, nmStarRating, hrStarRating,
            ezdtStarRating, nmdtStarRating, hrdtStarRating;

        public int circleCount, objectCount;

        public string title, artist, creator, version;

        public List<Note> osuNotes = new List<Note>();

        public List<double> ezhtDifficulties = new List<double>();
        public List<double> nmhtDifficulties = new List<double>();
        public List<double> hrhtDifficulties = new List<double>();

        public List<double> ezDifficulties = new List<double>();
        public List<double> nmDifficulties = new List<double>();
        public List<double> hrDifficulties = new List<double>();

        public List<double> ezdtDifficulties = new List<double>();
        public List<double> nmdtDifficulties = new List<double>();
        public List<double> hrdtDifficulties = new List<double>();

        public struct Note
        {
            public int xCoordinate, yCoordinate, time, objectType;
            public Note (int x, int y, int t, int o)
            {
                xCoordinate = x;
                yCoordinate = y;
                time = t;
                objectType = o;
            }
        }

        public void GetBeatmapData(string fileName)
        {
            string line;
            using (StreamReader file = new StreamReader(fileName))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains("Title:"))
                    {
                        title = line.Split(new[] { ':' }, 2)[1];
                    }
                    if (line.Contains("Artist:"))
                    {
                        artist = line.Split(new[] { ':' }, 2)[1];
                    }
                    if (line.Contains("Creator:"))
                    {
                        creator = line.Split(new[] { ':' }, 2)[1];
                    }
                    if (line.Contains("Version:"))
                    {
                        version = line.Split(new[] { ':' }, 2)[1];
                    }
                    if (line.Contains("CircleSize:"))
                    {
                        circleSize = Convert.ToDouble(line.Split(':')[1]);
                    }
                    if (line.Contains("OverallDifficulty:"))
                    {
                        overallDifficulty = Convert.ToDouble(line.Split(':')[1]);
                        approachRate = overallDifficulty;
                    }
                    if (line.Contains("ApproachRate:"))
                    {
                        approachRate = Convert.ToDouble(line.Split(':')[1]);
                    }
                    if (line.Contains("[HitObjects]"))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            int xCoordinate = Convert.ToInt32(line.Split(',')[0]);
                            int yCoordinate = Convert.ToInt32(line.Split(',')[1]);
                            int time = Convert.ToInt32(line.Split(',')[2]);
                            int objectType = Convert.ToInt32(line.Split(',')[3]);
                            Note note = new Note(xCoordinate, yCoordinate, time, objectType);
                            if (note.objectType != 12)
                            {
                                objectCount++;
                                osuNotes.Add(note);
                                if (!line.Contains('|'))
                                {
                                    circleCount++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
