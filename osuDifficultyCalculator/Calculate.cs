using System;
using System.Collections.Generic;
using System.Linq;

namespace osuDifficultyCalculator
{
    public class Calculate
    {
        private const double consistencyMultiplier = 0.98;
        private const double starRatingMultiplier = 53.5;
        private const double starRatingExponent = 0.34;
        private const double ppMultiplier = 1.50;
        private const double ppExponent = 2.43;

        /// Converts the circle size to its corresponding diameter.
        public double Diameter(double circleSize)
        {
            return 2 * (54.41 - 4.48 * circleSize);
        }

        /// A multiplicative pp antibonus for hidden when the DT mod is enabled. The bonus decreases as the approach rate increases.
        /// Minimum bonus is 1.0 at AR 11; maximum bonus is 1.1 at AR 10.33 or less.
        private double HiddenBonus(double approachRate, bool dtEnabled)
        {
            double ms = dtEnabled ? 100 * (13 - approachRate) : 150 * (13 - approachRate);
            return 1.1 - 0.1 * Math.Pow((450 - Math.Min(ms, 450)) / 150, 2);
        }

        public double FlashlightBonus(List<double> distances, List<double> times, bool doubleTime = false)
        {
            double flBonus = 0;
            for (int i = 0; i < distances.Count; i++)
            {
                if (!doubleTime)
                {
                    flBonus += Math.Sqrt(distances[i] / times[i]);
                }
                else
                {
                    flBonus += Math.Sqrt(1.5 * distances[i] / times[i]);
                }
            }
            return Math.Pow(1 + Math.Sqrt(flBonus) / 125, 2);
        }

        /// A multiplicative pp bonus for high AR when the DT mod is enabled. The bonus increases as the approach rate increases.
        /// Minimum bonus is 1.0 at AR 10 or less; maximum bonus is 1.25 at AR 11.
        private double HighApproachRateBonus(double approachRate, bool dtEnabled)
        {
            double ms = dtEnabled ? 100 * (13 - approachRate) : 150 * (13 - approachRate);
            return 1 + 0.8 * Math.Pow((Math.Min(ms, 450) - 450) / 300, 2);
        }

        /// A multiplicative pp bonus for low AR. The bonus increases as the approach rate decreases.
        /// Minimum bonus is 1.0 at AR 11; maximum bonus is 2.0 at AR 0.
        private double LowApproachRateBonus(double approachRate, bool dtEnabled)
        {
            double ms = dtEnabled ? 100 * (13 - approachRate) : 150 * (13 - approachRate);
            return 1 + Math.Pow((Math.Max(ms, 300) - 300) / 1650, 2);
        }

        /// A multiplicative pp bonus for overall difficulty. The bonus increases as the overall difficulty increases.
        /// Minimum bonus is 1.0 at OD 0; maximum bonus is >2.0 at OD 11. 
        private double OverallDifficultyBonus(double overallDifficulty, bool dtEnabled)
        {
            double ms = dtEnabled ? 53 - 4 * overallDifficulty : 79.5 - 6 * overallDifficulty;
            return 2 * Math.Pow(2, (19.5 - ms) / 60);
        }

        /// A multiplicative pp bonus for circle count. The bonus increases as the total amount of circles increases.
        /// Minimum bonus is 1.0 at 0 circles; maximum bonus is 3.0 at ∞ circles.
        private double LengthBonus(double circleCount)
        {
            return 1 + 2 * circleCount / (circleCount + 2000);
        }

        /// Calculates the distance between two notes.
        public double Distance(int previousX, int previousY, int currentX, int currentY)
        {
            return Math.Sqrt(Math.Pow(previousX - currentX, 2) + Math.Pow(previousY - currentY, 2));
        }

        /// Calculates the angle between 3 notes. Return NaN if the angle does not exist.
        public double Angle(int previousPreviousX, int previousPreviousY, int previousX, int previousY, int currentX, int currentY)
        {
            double previousPreviousToPrevious = Distance(previousPreviousX, previousPreviousY, previousX, previousY);
            double previousToCurrent = Distance(previousX, previousY, currentX, currentY);
            double previousPreviousToCurrent = Distance(previousPreviousX, previousPreviousY, currentX, currentY);
            return previousToCurrent * previousPreviousToPrevious > 0 ? Math.Acos((Math.Pow(previousPreviousToPrevious, 2) + Math.Pow(previousToCurrent, 2) - Math.Pow(previousPreviousToCurrent, 2)) / (2 * previousToCurrent * previousPreviousToPrevious)) : double.NaN;
        }

        /// Buff low-bpm linear patterns and high-bpm snappy patterns.
        public double AngleBuff(double angle, double timeDifference, double previousTimeDifference)
        {
            double timeDifferencePunishment = Math.Pow(2, -Math.Abs(1 - timeDifference / previousTimeDifference)); /// If the time difference between the last 3 notes varies significantly, reduce the angle buff.
            double slowAngleBuff = ((1 - Math.Pow(2, 9 - (Math.Max(90, timeDifference) / 10))) * Math.Pow(Math.Sin(angle / 2), 2)) + 1; /// Low BPM angle buff.
            double fastAngleBuff = ((1 - Math.Pow(2, (Math.Min(100, timeDifference) / 10) - 10)) * Math.Pow(Math.Cos(angle / 2), 2)) + 1; /// High BPM angle buff.
            return double.IsNaN(angle) ? 0 : (timeDifferencePunishment * ((slowAngleBuff * fastAngleBuff) - 1) / 2);
        }

        /// Calculates the strain of a particular note.
        public double Strain(double distance, double time)
        {
            return (100 * distance + distance * time) / Math.Pow(time, 3);
        }

        /// Calculates the star rating of a beatmap.
        public double StarRating(List<double> strains, double circleSize)
        {
            strains = strains.OrderByDescending(x => x).ToList();
            for (int i = 0; i < strains.Count; i++)
            {
                strains[i] *= Math.Pow(consistencyMultiplier, i);
            }
            return Math.Pow(strains.Sum(), starRatingExponent) * starRatingMultiplier / Math.Sqrt(Diameter(circleSize));
        }

        /// Calculates the pp of a beatmap.
        public double PP(double starRating, double overallDifficulty, double approachRate, double circleCount, string mod = "")
        {
            double standardPP = ppMultiplier * Math.Pow(starRating, ppExponent) * LengthBonus(circleCount);
            if (!mod.Contains("DT")) // NM, HD, HR, HDHR
            {
                if (mod.Contains("HD"))
                {
                    standardPP *= HiddenBonus(approachRate, false);
                }
                return standardPP * OverallDifficultyBonus(overallDifficulty, false) * HighApproachRateBonus(approachRate, false) * LowApproachRateBonus(approachRate, false);
            }
            if (mod.Contains("DT"))
            {
                if (mod.Contains("HD"))
                {
                    standardPP *= HiddenBonus(approachRate, true);
                }
                return standardPP * OverallDifficultyBonus(overallDifficulty, true) * HighApproachRateBonus(approachRate, true) * LowApproachRateBonus(approachRate, true);
            }
            return -1;
        }
    }
}
