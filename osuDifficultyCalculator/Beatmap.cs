using System;
using System.IO;
using System.Collections.Generic;

namespace osuDifficultyCalculator
{
    public class Beatmap
    {
        public double circleSize, overallDifficulty, approachRate, sliderTickRate,
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
            public double sliderDifficulty;
            public Note(int x, int y, int t, int o, double sD)
            {
                xCoordinate = x;
                yCoordinate = y;
                time = t;
                objectType = o;
                sliderDifficulty = sD;
            }
        }

        public void GetBeatmapData(string fileName)
        {
            string[] allLines = File.ReadAllLines(fileName);
            bool reachedHitObjectsLine = false;
            for (int i = 0; i < allLines.Length; ++i)
            {
                string line = allLines[i];
                if (reachedHitObjectsLine)
                {
                    int xCoordinate = Convert.ToInt32(line.Split(',')[0]);
                    int yCoordinate = Convert.ToInt32(line.Split(',')[1]);
                    int time = Convert.ToInt32(line.Split(',')[2]);
                    int nextTime = (i != allLines.Length - 1 ? Convert.ToInt32(allLines[i + 1].Split(',')[2]) : int.MaxValue);
                    int objectType = Convert.ToInt32(line.Split(',')[3]);
                    double sliderDifficulty = 0;
                    if (objectType != 12)
                    {
                        objectCount++;
                        if (!line.Contains("|"))
                        {
                            circleCount++;
                        }
                        else
                        {
                            int slideCount = Convert.ToInt32(line.Split(',')[6]);
                            double travelDistance = Math.Max(0, Convert.ToDouble(line.Split(',')[7]) - 2 * Calculate.Diameter(circleSize));
                            sliderDifficulty = sliderTickRate * slideCount * travelDistance / Math.Max(37.5, nextTime - time);
                        }
                        Note note = new Note(xCoordinate, yCoordinate, time, objectType, sliderDifficulty);
                        osuNotes.Add(note);
                    }
                    continue;
                }
                if (line.Contains("Title:"))
                {
                    title = line.Split(new[] { ':' }, 2)[1];
                    continue;
                }
                if (line.Contains("Artist:"))
                {
                    artist = line.Split(new[] { ':' }, 2)[1];
                    continue;
                }
                if (line.Contains("Creator:"))
                {
                    creator = line.Split(new[] { ':' }, 2)[1];
                    continue;
                }
                if (line.Contains("Version:"))
                {
                    version = line.Split(new[] { ':' }, 2)[1];
                    continue;
                }
                if (line.Contains("CircleSize:"))
                {
                    circleSize = Convert.ToDouble(line.Split(':')[1]);
                    continue;
                }
                if (line.Contains("OverallDifficulty:"))
                {
                    overallDifficulty = Convert.ToDouble(line.Split(':')[1]);
                    approachRate = overallDifficulty;
                    continue;
                }
                if (line.Contains("ApproachRate:"))
                {
                    approachRate = Convert.ToDouble(line.Split(':')[1]);
                    continue;
                }
                if (line.Contains("SliderTickRate:"))
                {
                    sliderTickRate = Convert.ToDouble(line.Split(':')[1]);
                    continue;
                }
                if (line.Contains("[HitObjects]"))
                {
                    reachedHitObjectsLine = true;
                    continue;
                }
            }
        }
    }
}
