using System;
using System.Collections.Generic;
using System.Linq;

namespace osuDifficultyCalculator
{
    public class Calculate
    {
        private const double consistencyMultiplier = 0.98;
        private const double starRatingMultiplier = 53.0;
        private const double starRatingExponent = 0.34;
        private const double ppMultiplier = 1.5;
        private const double ppExponent = 2.43;
        private const double minimumTime = 37.5;

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
        public double Diameter(double circleSize)
        {
            return 2 * (54.41 - 4.48 * circleSize);
        }

        /// Calculates the distance between two notes.
        private double Distance(Beatmap.Note currentNote, Beatmap.Note lastNote)
        {
            return Math.Sqrt(Math.Pow(currentNote.xCoordinate - lastNote.xCoordinate, 2) + Math.Pow(currentNote.yCoordinate - lastNote.yCoordinate, 2));
        }

        ///  Calculate the note density at a note.
        private int NoteDensity(List<Beatmap.Note> osuNotes, int notePosition, double approachRate, int speedUpMod)
        {
            int noteDensity = 1;
            bool nextNoteIsInvisible = false;
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;
            double approachTime = ConvertApproachRate(approachRate, speedUpMod);
            for (int i = notePosition; nextNoteIsInvisible == false; ++i)
            {
                if (i + 1 == osuNotes.Count)
                {
                    break;
                }
                if ((osuNotes[i + 1].time - osuNotes[notePosition].time) / speedUpFactor < approachTime)
                {
                    noteDensity++;
                }
                else
                {
                    nextNoteIsInvisible = true;
                }
            }
            return noteDensity;
        }

        /// Convert AR to milliseconds.
        private double ConvertApproachRate(double approachRate, int speedUpMod)
        {
            double approachTime;
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;
            if (approachRate >= 5)
                approachTime = 150 / speedUpFactor * (13 - approachRate);
            else
                approachTime = 120 / speedUpFactor * (15 - approachRate);
            return approachTime;
        }

        /// A multiplicative pp bonus when the FL mod is enabled. The bonus increases as the object count increases.
        /// Minimum bonus is 1.0 at AR 10 or less. Maximum bonus is unbounded.
        private double FlashlightBonus(List<Beatmap.Note> osuNotes, int speedUpMod)
        {
            double flBonus = 0;
            double timeDistanceRatio;
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;
            for (int i = 1; i < osuNotes.Count; ++i)
            {
                timeDistanceRatio = speedUpFactor * (Distance(osuNotes[i - 1], osuNotes[i]) + osuNotes[i].realTravelDistance) / Math.Max(minimumTime, osuNotes[i].time - osuNotes[i - 1].time);
                flBonus += timeDistanceRatio;
            }
            return Math.Pow(1 + Math.Sqrt(flBonus) / 125, 2);
        }

        /// A multiplicative pp antibonus for hidden when the DT mod is enabled. The bonus increases as the note density increases.
        private double HiddenBonus(List<Beatmap.Note> osuNotes, double approachRate, int speedUpMod)
        {
            double hiddenBonusSum = 0;
            for (int i = 0; i < osuNotes.Count; ++i)
            {
                int currentNoteDensity = Math.Min(NoteDensity(osuNotes, i, approachRate, speedUpMod), 26);
                hiddenBonusSum += 1 + Math.Pow(Math.Sin(Math.PI * currentNoteDensity / 52), 2);
            }
            return hiddenBonusSum / osuNotes.Count;
        }

        /// A multiplicative pp bonus for high AR when the DT mod is enabled. The bonus increases as the approach rate increases.
        /// Minimum bonus is 1.0 at AR 10 or less; maximum bonus is 1.25 at AR 11.
        private double HighApproachRateBonus(double approachRate, int speedUpMod)
        {
            double approachTime = ConvertApproachRate(approachRate, speedUpMod);
            return 1 + 0.8 * Math.Pow((Math.Min(approachTime, 450) - 450) / 300, 2);
        }

        /// A multiplicative pp bonus for low AR. The bonus increases as the approach rate decreases.
        /// Minimum bonus is 1.0 at AR 11; maximum bonus is 2.0 at AR 0.
        private double LowApproachRateBonus(double approachRate, int speedUpMod)
        {
            double approachTime = ConvertApproachRate(approachRate, speedUpMod);
            return 1 + Math.Pow((Math.Max(approachTime, 300) - 300) / 1650, 2);
        }

        /// A multiplicative pp bonus for overall difficulty. The bonus increases as the overall difficulty increases.
        /// Minimum bonus is 1.0 at OD 0; maximum bonus is >2.0 at OD 11. 
        private double OverallDifficultyBonus(double overallDifficulty, int speedUpMod)
        {
            double hitWindow;
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;
            hitWindow = (79.5 - 6 * overallDifficulty) / speedUpFactor;
            return 2 * Math.Pow(2, (19.5 - hitWindow) / 60);
        }

        /// A multiplicative pp bonus for circle count. The bonus increases as the total amount of circles increases.
        /// Minimum bonus is 1.0 at 0 circles; maximum bonus is 3.0 at ∞ circles.
        private double LengthBonus(int circleCount)
        {
            return 1 + 1.8 * circleCount / (circleCount + 2000);
        }

        /// Calculates the angle between 3 notes. Return NaN if the angle does not exist.
        private double Angle(Beatmap.Note secondLastNote, Beatmap.Note lastNote, Beatmap.Note currentNote)
        {
            double lastToSecondLast = Distance(lastNote, secondLastNote);
            double currentToSecondLast = Distance(currentNote, secondLastNote);
            double currentToLast = Distance(currentNote, lastNote);
            return currentToLast * lastToSecondLast > 0
                ? Math.Acos((Math.Pow(lastToSecondLast, 2) + Math.Pow(currentToLast, 2) - Math.Pow(currentToSecondLast, 2)) / (2 * currentToLast * lastToSecondLast))
                : double.NaN;
        }

        /// Buff angle changes.
        private double AngleChangeBuff(double angle, double lastAngle, double timeDifference, double lastTimeDifference)
        {
            lastTimeDifference = Math.Max(minimumTime, lastTimeDifference);
            timeDifference = Math.Max(minimumTime, timeDifference);
            double timeDifferencePunishment = Math.Pow(3, -Math.Pow(timeDifference - lastTimeDifference, 2) / 1000);
            double scaledAngle = Math.Pow(Math.Cos(angle / 2), 2);
            double scaledLastAngle = Math.Pow(Math.Cos(lastAngle / 2), 2);
            double angleDifference = Math.Abs(scaledAngle - scaledLastAngle) - 0.1;
            return double.IsNaN(angle) || double.IsNaN(lastAngle) ? 0 : timeDifferencePunishment * angleDifference;
        }

        /// Calculates the difficulty of a particular note.
        public double Difficulty(Beatmap.Note thirdLastNote, Beatmap.Note secondLastNote, Beatmap.Note lastNote, Beatmap.Note currentNote, Beatmap.Note nextNote, int speedUpMod, double circleSize, double tickRate, bool doPrint = false)
        {
            double currentDistance = Distance(currentNote, lastNote) + (10 * Math.Pow(Math.Cos(Math.PI * Math.Min(Distance(currentNote, lastNote), 25) / 50), 1));
            double overlap = currentDistance / Diameter(circleSize);
            double overlapPunishment = Math.Min(1, Math.Pow(overlap, 2));
            double angle = Angle(secondLastNote, lastNote, currentNote);
            double lastAngle = Angle(thirdLastNote, secondLastNote, lastNote);
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;

            double lastTimeDifference = Math.Max(minimumTime, (lastNote.time - secondLastNote.time) / speedUpFactor);
            double timeDifference = Math.Max(minimumTime, (currentNote.time - lastNote.time) / speedUpFactor);
            double speedBuff = Math.Max(1, 62.5 / timeDifference);
            double sliderDifficulty = 1 + Math.Max(0, tickRate * currentNote.realTravelDistance) / Math.Max(minimumTime, (nextNote.time - currentNote.time) / speedUpFactor);
            double baseDifficulty = (100 * currentDistance * sliderDifficulty + currentDistance * sliderDifficulty * timeDifference) / Math.Pow(timeDifference, 3);
            double difficulty = baseDifficulty * speedBuff * (1 + overlapPunishment * AngleChangeBuff(angle, lastAngle, timeDifference, lastTimeDifference));
            return difficulty;
        }

        /// Calculates the star rating of a beatmap.
        public double StarRating(List<double> difficulties, double circleSize)
        {
            int sectionLength = 1;
            int maximumMultiple = sectionLength * (difficulties.Count / sectionLength);
            double currentSectionDifficulty = 0;
            List<double> sectionDifficulties = new List<double>();
            for (int i = 0; i < difficulties.Count; ++i)
            {
                currentSectionDifficulty += difficulties[i];
                if (i != 0 && i % sectionLength == 0 && i < maximumMultiple)
                {
                    sectionDifficulties.Add(currentSectionDifficulty);
                    currentSectionDifficulty = 0;
                }
                if (i >= maximumMultiple && i == difficulties.Count - 1)
                {
                    sectionDifficulties.Add(currentSectionDifficulty);
                }
            }
            sectionDifficulties = sectionDifficulties.OrderByDescending(x => x).ToList();
            for (int i = 0; i < sectionDifficulties.Count; ++i)
            {
                sectionDifficulties[i] *= Math.Pow(consistencyMultiplier, i);
            }
            return Math.Pow(sectionDifficulties.Sum(), starRatingExponent) * starRatingMultiplier / Math.Sqrt(Diameter(circleSize));
        }

        /// Calculates the pp of a beatmap.
        public double PP(double starRating, double overallDifficulty, double approachRate, int circleCount, List<Beatmap.Note> osuNotes, int modEnum = 17)
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
                    basePP *= HiddenBonus(osuNotes, approachRate, 1);
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
                    basePP *= HiddenBonus(osuNotes, approachRate, -1);
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
                    basePP *= HiddenBonus(osuNotes, approachRate, 0);
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
