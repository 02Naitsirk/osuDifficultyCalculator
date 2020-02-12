using System;
using System.IO;
using System.Collections.Generic;

namespace osuDifficultyCalculator
{
    public class Beatmap
    {
        readonly Calculate calculate = new Calculate();

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
            public double xCoordinate, yCoordinate, time, realTravelDistance;
            public Note(double xCoordinate, double yCoordinate, double time, double realTravelDistance)
            {
                this.xCoordinate = xCoordinate;
                this.yCoordinate = yCoordinate;
                this.time = time;
                this.realTravelDistance = realTravelDistance;
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
                    int objectType = Convert.ToInt32(line.Split(',')[3]);
                    int slideCount;
                    double travelDistance;
                    double realTravelDistance = 0;
                    if (objectType != 12)
                    {
                        objectCount++;
                        if (!line.Contains("|"))
                        {
                            circleCount++;
                        }
                        else
                        {
                            slideCount = Convert.ToInt32(line.Split(',')[6]);
                            travelDistance = Convert.ToDouble(line.Split(',')[7]);
                            realTravelDistance = slideCount * Math.Max(0, travelDistance - 2 * calculate.Diameter(circleSize));
                        }
                        Note note = new Note(xCoordinate, yCoordinate, time, realTravelDistance);
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
