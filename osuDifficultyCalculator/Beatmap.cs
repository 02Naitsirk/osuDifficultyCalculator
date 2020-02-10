using System;
using System.IO;
using System.Collections.Generic;

namespace osuDifficultyCalculator
{
    public class Beatmap
    {
        public double circleSize, overallDifficulty, approachRate, sliderTickRate,

            ezhtAimStarRating, nmhtAimStarRating, hrhtAimStarRating,
            ezAimStarRating, nmAimStarRating, hrAimStarRating,
            ezdtAimStarRating, nmdtAimStarRating, hrdtAimStarRating,

            htSpeedStarRating, nmSpeedStarRating, dtSpeedStarRating;

        public int circleCount, objectCount;

        public string title, artist, creator, version;

        public List<Note> osuNotes = new List<Note>();

        /// <summary>
        ///  Aim
        /// </summary>
        public List<double> ezhtAimDifficulties = new List<double>();
        public List<double> nmhtAimDifficulties = new List<double>();
        public List<double> hrhtAimDifficulties = new List<double>();

        public List<double> ezAimDifficulties = new List<double>();
        public List<double> nmAimDifficulties = new List<double>();
        public List<double> hrAimDifficulties = new List<double>();

        public List<double> ezdtAimDifficulties = new List<double>();
        public List<double> nmdtAimDifficulties = new List<double>();
        public List<double> hrdtAimDifficulties = new List<double>();

        /// <summary>
        /// Speed
        /// </summary>
        public List<double> htSpeedDifficulties = new List<double>();
        public List<double> nmSpeedDifficulties = new List<double>();
        public List<double> dtSpeedDifficulties = new List<double>();

        public struct Note
        {
            public double xCoordinate, yCoordinate, time, objectType, slideCount, travelDistance;
            public Note(double x, double y, double t, double o, double s, double tD)
            {
                xCoordinate = x;
                yCoordinate = y;
                time = t;
                objectType = o;
                slideCount = s;
                travelDistance = tD;
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
                    int slideCount = 0;
                    double travelDistance = 0;
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
                            travelDistance = Math.Max(0, Convert.ToDouble(line.Split(',')[7]));
                        }
                        Note note = new Note(xCoordinate, yCoordinate, time, objectType, slideCount, travelDistance);
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
