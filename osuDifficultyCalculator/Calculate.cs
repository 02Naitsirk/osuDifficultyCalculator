using System;
using System.Collections.Generic;
using System.Linq;
using static osuDifficultyCalculator.Beatmap;

namespace osuDifficultyCalculator
{
    public static class Calculate
    {
        private const double consistencyMultiplier = 0.98;
        private const double starRatingMultiplier = 53.0;
        private const double starRatingExponent = 0.34;
        private const double ppMultiplier = 1.5;
        private const double ppExponent = 2.43;
        public const double minimumTime = 37.5;

        public enum Mods
        {
            EZ = 2,
            HT = 3,
            HR = 5,
            DT = 7,
            HD = 11,
            FL = 13
        }

        /// Converts the circle size to its corresponding diameter.
        public static double Diameter(double circleSize)
        {
            return 2 * (54.41 - 4.48 * circleSize);
        }

        /// A multiplicative pp bonus when the FL mod is enabled. The bonus increases as the object count increases.
        /// Minimum bonus is 1.0 at AR 10 or less. Maximum bonus is unbounded.
        private static double FlashlightBonus(List<Note> beatmapObjects, int speedUpMod)
        {
            double flBonus = 0;
            double timeDistanceRatio;
            switch (speedUpMod)
            {
                case 1:  /// DT
                    for (int i = 1; i < beatmapObjects.Count; ++i)
                    {
                        timeDistanceRatio = 1.5 * Distance(beatmapObjects[i - 1], beatmapObjects[i]) / Math.Max(minimumTime, beatmapObjects[i].time - beatmapObjects[i - 1].time);
                        flBonus += timeDistanceRatio;
                    }
                    break;
                case 0:  /// NM
                    for (int i = 1; i < beatmapObjects.Count; ++i)
                    {
                        timeDistanceRatio = Distance(beatmapObjects[i - 1], beatmapObjects[i]) / Math.Max(minimumTime, beatmapObjects[i].time - beatmapObjects[i - 1].time);
                        flBonus += timeDistanceRatio;
                    }
                    break;
                case -1: /// HT
                    for (int i = 1; i < beatmapObjects.Count; ++i)
                    {
                        timeDistanceRatio = 0.75 * Distance(beatmapObjects[i - 1], beatmapObjects[i]) / Math.Max(minimumTime, beatmapObjects[i].time - beatmapObjects[i - 1].time);
                        flBonus += timeDistanceRatio;
                    }
                    break;
            }
            return Math.Pow(1 + Math.Sqrt(flBonus) / 125, 2);
        }

        /// A multiplicative pp antibonus for hidden when the DT mod is enabled. The bonus decreases as the approach rate increases.
        /// Minimum bonus is 1.0 at AR 11; maximum bonus is 1.1 at AR 10.33 or less.
        private static double HiddenBonus(double approachRate, int speedUpMod)
        {
            double approachTime;
            switch (speedUpMod)
            {
                case 1:  /// DT
                    approachTime = 100 * (13 - approachRate);
                    break;
                case 0:  /// NM
                    approachTime = 150 * (13 - approachRate);
                    break;
                case -1: /// HT
                    approachTime = 200 * (13 - approachRate);
                    break;
                default:
                    approachTime = double.NaN;
                    break;
            }
            return 1.1 - 0.08 * Math.Pow(3 - Math.Min(approachTime, 450) / 150, 2);
        }

        /// A multiplicative pp bonus for high AR when the DT mod is enabled. The bonus increases as the approach rate increases.
        /// Minimum bonus is 1.0 at AR 10 or less; maximum bonus is 1.25 at AR 11.
        private static double HighApproachRateBonus(double approachRate, int speedUpMod)
        {
            double approachTime;
            switch (speedUpMod)
            {
                case 1:  /// DT
                    approachTime = 100 * (13 - approachRate);
                    break;
                case 0:  /// NM
                    approachTime = 150 * (13 - approachRate);
                    break;
                case -1: /// HT
                    approachTime = 200 * (13 - approachRate);
                    break;
                default:
                    approachTime = double.NaN;
                    break;
            }
            return 1 + 0.8 * Math.Pow((Math.Min(approachTime, 450) - 450) / 300, 2);
        }

        /// A multiplicative pp bonus for low AR. The bonus increases as the approach rate decreases.
        /// Minimum bonus is 1.0 at AR 11; maximum bonus is 2.0 at AR 0.
        private static double LowApproachRateBonus(double approachRate, int speedUpMod)
        {
            double approachTime;
            switch (speedUpMod)
            {
                case 1:  /// DT
                    approachTime = 100 * (13 - approachRate);
                    break;
                case 0:  /// NM
                    approachTime = 150 * (13 - approachRate);
                    break;
                case -1: /// HT
                    approachTime = 200 * (13 - approachRate);
                    break;
                default:
                    approachTime = double.NaN;
                    break;
            }
            return 1 + Math.Pow((Math.Max(approachTime, 300) - 300) / 1650, 2);
        }

        /// A multiplicative pp bonus for overall difficulty. The bonus increases as the overall difficulty increases.
        /// Minimum bonus is 1.0 at OD 0; maximum bonus is >2.0 at OD 11. 
        private static double OverallDifficultyBonus(double overallDifficulty, int speedUpMod)
        {
            double hitWindow;
            switch (speedUpMod)
            {
                case 1:  /// DT
                    hitWindow = 53 - 4 * overallDifficulty;
                    break;
                case 0:  /// NM
                    hitWindow = 79.5 - 6 * overallDifficulty;
                    break;
                case -1: /// HT
                    hitWindow = 106 - 8 * overallDifficulty;
                    break;
                default:
                    hitWindow = double.NaN;
                    break;
            }
            return 2 * Math.Pow(2, (19.5 - hitWindow) / 60);
        }

        /// A multiplicative pp bonus for circle count. The bonus increases as the total amount of circles increases.
        /// Minimum bonus is 1.0 at 0 circles; maximum bonus is 3.0 at ∞ circles.
        private static double LengthBonus(int circleCount)
        {
            return 1 + 2.0 * circleCount / (circleCount + 2000);
        }

        /// Calculates the distance between two notes.
        public static double Distance(Note current, Note previous)
        {
            return Math.Sqrt(Math.Pow(current.xCoordinate - previous.xCoordinate, 2) + Math.Pow(current.yCoordinate - previous.yCoordinate, 2));
        }

        /// Calculates the angle between 3 notes. Return NaN if the angle does not exist.
        public static double Angle(Note previousPrevious, Note previous, Note current)
        {
            double previousToPreviousPrevious = Distance(previous, previousPrevious);
            double currentToPreviousPrevious = Distance(current, previousPrevious);
            double currentToPrevious = Distance(current, previous);
            return currentToPrevious * previousToPreviousPrevious > 0
                ? Math.Acos((Math.Pow(previousToPreviousPrevious, 2) + Math.Pow(currentToPrevious, 2) - Math.Pow(currentToPreviousPrevious, 2)) / (2 * currentToPrevious * previousToPreviousPrevious))
                : double.NaN;
        }

        /// Buff angle changes.
        public static double AngleComplexityBuff(double angle, double previousAngle, double timeDifference, double previousTimeDifference)
        {
            timeDifference = Math.Max(minimumTime, timeDifference);
            previousTimeDifference = Math.Max(minimumTime, previousTimeDifference);
            double timeDifferencePunishment = Math.Pow(2, -Math.Abs(1 - timeDifference / previousTimeDifference)); /// If the time difference between the last 3 notes varies significantly, reduce the buff.
            double scaledAngle = Math.Pow(Math.Cos(angle / 2), 2);
            double scaledPreviousAngle = Math.Pow(Math.Cos(previousAngle / 2), 2);
            double angleDifference = Math.Abs(scaledAngle - scaledPreviousAngle);
            return double.IsNaN(angle) || double.IsNaN(previousAngle) ? 0 : timeDifferencePunishment * angleDifference / 2;
        }

        /// Calculates the difficulty of a particular note.
        public static double Difficulty(Note previousPreviousPreviousNote, Note previousPreviousNote, Note previousNote, Note currentNote, Note nextNote, int speedUpMod, double circleSize, double tickRate)
        {
            double currentDistance = Distance(currentNote, previousNote);
            double overlap = Distance(currentNote, previousNote) / Diameter(circleSize);
            double overlapPunishment = Math.Min(1, Math.Pow(overlap / (2 - overlap), 2));
            double angle = Angle(previousPreviousNote, previousNote, currentNote);
            double previousAngle = Angle(previousPreviousPreviousNote, previousPreviousNote, previousNote);
            switch (speedUpMod)
            {
                case 1:  /// DT
                    double previousDtTimeDifference = Math.Max(minimumTime, (previousNote.time - previousPreviousNote.time) / 1.5);
                    double dtTimeDifference = Math.Max(minimumTime, (currentNote.time - previousNote.time) / 1.5);
                    double dtSpeedBuff = Math.Max(1, 62.5 / dtTimeDifference);
                    double dtSliderDifficulty = Math.Max(0, tickRate * currentNote.slideCount * (currentNote.travelDistance - 2 * Diameter(circleSize))) / Math.Max(minimumTime, (nextNote.time - currentNote.time) / 1.5);
                    double baseDtDifficulty = (100 * currentDistance * (1 + dtSliderDifficulty) + currentDistance * (1 + dtSliderDifficulty) * dtTimeDifference) / Math.Pow(dtTimeDifference, 3);
                    return baseDtDifficulty * dtSpeedBuff * (1 + overlapPunishment * AngleComplexityBuff(angle, previousAngle, dtTimeDifference, previousDtTimeDifference));
                case 0:  /// NM
                    double previousNmTimeDifference = Math.Max(minimumTime, previousNote.time - previousPreviousNote.time);
                    double nmTimeDifference = Math.Max(minimumTime, currentNote.time - previousNote.time);
                    double nmSpeedBuff = Math.Max(1, 62.5 / nmTimeDifference);
                    double nmSliderDifficulty = Math.Max(0, tickRate * currentNote.slideCount * (currentNote.travelDistance - 2 * Diameter(circleSize))) / Math.Max(minimumTime, nextNote.time - currentNote.time);
                    double baseNmDifficulty = (100 * currentDistance * (1 + nmSliderDifficulty) + currentDistance * (1 + nmSliderDifficulty) * nmTimeDifference) / Math.Pow(nmTimeDifference, 3);
                    return baseNmDifficulty * nmSpeedBuff * (1 + overlapPunishment * AngleComplexityBuff(angle, previousAngle, nmTimeDifference, previousNmTimeDifference));
                case -1: /// HT
                    double previousHtTimeDifference = Math.Max(minimumTime, (previousNote.time - previousPreviousNote.time) / 0.75);
                    double htTimeDifference = Math.Max(minimumTime, (currentNote.time - previousNote.time) / 0.75);
                    double htSpeedBuff = Math.Max(1, 62.5 / htTimeDifference);
                    double htSliderDifficulty = Math.Max(0, tickRate * currentNote.slideCount * (currentNote.travelDistance - 2 * Diameter(circleSize))) / Math.Max(minimumTime, (nextNote.time - currentNote.time) / 0.75);
                    double baseHtDifficulty = (100 * currentDistance * (1 + htSliderDifficulty) + currentDistance * (1 + htSliderDifficulty) * htTimeDifference) / Math.Pow(htTimeDifference, 3);
                    return baseHtDifficulty * htSpeedBuff * (1 + overlapPunishment * AngleComplexityBuff(angle, previousAngle, htTimeDifference, previousHtTimeDifference));
                default:
                    return 0;
            }
        }

        /// Calculates the star rating of a beatmap.
        public static double StarRating(List<double> difficulties, double circleSize)
        {
            difficulties = difficulties.OrderByDescending(x => x).ToList();
            for (int i = 0; i < difficulties.Count; i++)
            {
                difficulties[i] *= Math.Pow(consistencyMultiplier, i);
            }
            return Math.Pow(difficulties.Sum(), starRatingExponent) * starRatingMultiplier / Math.Sqrt(Diameter(circleSize));
        }

        /// Calculates the pp of a beatmap.
        public static double PP(double starRating, double overallDifficulty, double approachRate, int circleCount, List<Note> osuNotes = null, int modEnum = 17)
        {
            double basePP = ppMultiplier * Math.Pow(starRating, ppExponent) * LengthBonus(circleCount);
            if (modEnum % (int)Mods.EZ == 0)
            {
                overallDifficulty /= 2;
                approachRate /= 2;
            }
            if (modEnum % (int)Mods.HR == 0)
            {
                overallDifficulty = Math.Min(10, overallDifficulty * 1.4);
                approachRate = Math.Min(10, approachRate * 1.4);
            }
            if (modEnum % (int)Mods.DT == 0)
            {
                if (modEnum % (int)Mods.HD == 0)
                {
                    basePP *= HiddenBonus(approachRate, 1);
                }
                if (modEnum % (int)Mods.FL == 0)
                {
                    basePP *= FlashlightBonus(osuNotes, 1);
                }
                basePP *= OverallDifficultyBonus(overallDifficulty, 1) * HighApproachRateBonus(approachRate, 1) * LowApproachRateBonus(approachRate, 1);
            }
            if (modEnum % (int)Mods.HT == 0)
            {
                if (modEnum % (int)Mods.HD == 0)
                {
                    basePP *= HiddenBonus(approachRate, -1);
                }
                if (modEnum % (int)Mods.FL == 0)
                {
                    basePP *= FlashlightBonus(osuNotes, -1);
                }
                basePP *= OverallDifficultyBonus(overallDifficulty, -1) * HighApproachRateBonus(approachRate, -1) * LowApproachRateBonus(approachRate, -1);
            }
            if (modEnum % (int)Mods.DT != 0 && modEnum % (int)Mods.HT != 0)
            {
                if (modEnum % (int)Mods.HD == 0)
                {
                    basePP *= HiddenBonus(approachRate, 0);
                }
                if (modEnum % (int)Mods.FL == 0)
                {
                    basePP *= FlashlightBonus(osuNotes, 0);
                }
                basePP *= OverallDifficultyBonus(overallDifficulty, 0) * HighApproachRateBonus(approachRate, 0) * LowApproachRateBonus(approachRate, 0);
            }
            return Math.Round(basePP, 2);
        }
    }
}
