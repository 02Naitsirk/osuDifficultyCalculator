using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace osuDifficultyCalculator
{
    public class Calculate
    {
        private const double consistencyMultiplier = 0.9;
        private const double starRatingMultiplier = 56.25;
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
        private double ConvertCircleSize(double circleSize)
        {
            return 2 * (54.41 - 4.48 * circleSize);
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

        /// Calculates the distance between two notes.
        private double Distance(Beatmap.Note currentNote, Beatmap.Note lastNote)
        {
            return Sqrt(Pow(currentNote.xCoordinate - lastNote.xCoordinate, 2) + Pow(currentNote.yCoordinate - lastNote.yCoordinate, 2));
        }

        /// Calculates the angle between 3 notes. Return NaN if the angle does not exist.
        private double Angle(Beatmap.Note secondLastNote, Beatmap.Note lastNote, Beatmap.Note currentNote)
        {
            double lastToSecondLast = Distance(lastNote, secondLastNote);
            double currentToSecondLast = Distance(currentNote, secondLastNote);
            double currentToLast = Distance(currentNote, lastNote);
            return currentToLast * lastToSecondLast > 0 ? Acos((Pow(lastToSecondLast, 2) + Pow(currentToLast, 2) - Pow(currentToSecondLast, 2)) / (2 * currentToLast * lastToSecondLast)) : double.NaN;
        }

        ///  Calculate the note density at a time.
        private int NoteDensity(List<Beatmap.Note> osuNotes, int noteIndex, double approachRate, int speedUpMod)
        {
            int noteDensity = 1;
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;
            double approachTime = ConvertApproachRate(approachRate, speedUpMod);
            for (int i = noteIndex; i + 1 < osuNotes.Count; ++i)
            {
                if ((osuNotes[i + 1].time - osuNotes[noteIndex].time) / speedUpFactor >= approachTime)
                {
                    break;
                }
                noteDensity++;
            }
            return noteDensity;
        }

        /// A multiplicative pp bonus when the FL mod is enabled. The bonus increases as the object count increases.
        /// Minimum bonus is 1.0 at AR 10 or less. Maximum bonus is unbounded.
        private double FlashlightBonus(List<Beatmap.Note> osuNotes, int speedUpMod)
        {
            double flashlightBonus = 0;
            double timeDistanceRatio;
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;
            for (int i = 1; i < osuNotes.Count; ++i)
            {
                timeDistanceRatio = speedUpFactor * (Distance(osuNotes[i - 1], osuNotes[i]) + osuNotes[i].realTravelDistance) / Max(minimumTime, osuNotes[i].time - osuNotes[i - 1].time);
                flashlightBonus += timeDistanceRatio;
            }
            return Pow(1 + Sqrt(flashlightBonus) / 125, 2);
        }

        /// A multiplicative pp nerf for hidden when the DT mod is enabled. The bonus increases as the note density increases.
        private double HiddenBonus(List<Beatmap.Note> osuNotes, double approachRate, double circleSize, int speedUpMod)
        {
            double hiddenBonus = 0;
            for (int i = 0; i < osuNotes.Count; ++i)
            {
                double stackNerf = 1;
                int currentNoteDensity = Min(NoteDensity(osuNotes, i, approachRate, speedUpMod), 26);

                /// Nerf stacks and low-spaced streams. Prevents maps like Ascension to Heaven from becoming overbuffed.
                if (i > 0 && Distance(osuNotes[i], osuNotes[i - 1]) / ConvertCircleSize(circleSize) < 0.5)
                {
                    stackNerf = Pow(2 * Distance(osuNotes[i], osuNotes[i - 1]) / ConvertCircleSize(circleSize), 4);
                }
                hiddenBonus += 1 + Pow(Sin(PI * currentNoteDensity / 52), 2) * stackNerf;
            }
            return hiddenBonus / osuNotes.Count;
        }

        /// A multiplicative pp bonus for high AR when the DT mod is enabled. The bonus increases as the approach rate increases.
        /// Minimum bonus is 1.0 at AR 10 or less; maximum bonus is 1.2 at AR 11.
        private double HighApproachRateBonus(double approachRate, int speedUpMod)
        {
            double approachTime = ConvertApproachRate(approachRate, speedUpMod);
            return 1 + 0.8 * Pow((Min(approachTime, 450) - 450) / 300, 2);
        }

        /// Buff high density (low AR); minimum buff to high AR stream maps.
        private double HighDensityBonus(List<Beatmap.Note> osuNotes, double approachRate, int speedUpMod)
        {
            double highDensityBonus = 0;
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;
            double approachTime = ConvertApproachRate(approachRate, speedUpMod);
            for (int i = 0; i < osuNotes.Count; ++i)
            {
                /// Finds the time of the last visible note (osuNotes[j].time) for a note at position i. 
                /// Prevents stream maps from receiving too much of a buff.
                for (int j = i; j + 1 < osuNotes.Count; ++j)
                {
                    if ((osuNotes[j + 1].time - osuNotes[i].time) / speedUpFactor >= approachTime)
                    {
                        highDensityBonus += (osuNotes[j].time - osuNotes[i].time) / speedUpFactor;
                        break;
                    }
                }
            }
            return 1 + 0.5 * Pow(highDensityBonus / (osuNotes.Count * 1200), 2);
        }

        /// A multiplicative pp bonus for overall difficulty. The bonus increases as the overall difficulty increases.
        /// Minimum bonus is 1.0 at OD 0; maximum bonus is >2.0 at OD 11. 
        private double OverallDifficultyBonus(double overallDifficulty, int speedUpMod)
        {
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;
            double hitWindow = (79.5 - 6 * overallDifficulty) / speedUpFactor;
            return 2 * Pow(2, (19.5 - hitWindow) / 60);
        }

        /// A multiplicative pp bonus for circle count. The bonus increases as the total amount of circles increases.
        /// Minimum bonus is 1.0 at 0 circles; maximum bonus is 3.0 at ∞ circles.
        private double LengthBonus(int circleCount)
        {
            return 1 + 1.8 * circleCount / (circleCount + 2000);
        }
        
        /// Nerf short maps with object count less than 500 objects.
        private double ShortMapNerf(int objectCount)
        {
            return 1 - Pow((double) Min(objectCount, 500) / 500 - 1, 2) / 10;
        }

        /// Buff angle changes.
        private double AngleChangeBonus(double angle, double lastAngle, double timeDifference, double lastTimeDifference)
        {
            double timeDifferenceNerf = Pow(10, -Pow(timeDifference - lastTimeDifference, 2) / 1000);
            double scaledAngle = Pow(Cos(angle / 2), 2);
            double scaledLastAngle = Pow(Cos(lastAngle / 2), 2);
            double angleDifference = Abs(scaledAngle - scaledLastAngle);
            return double.IsNaN(angleDifference) ? 0 : timeDifferenceNerf * angleDifference;
        }

        ///  Buff wide angles.
        private double AngleBonus(double angle, double currentDistance, double timeDifference, double lastTimeDifference)
        {
            double timeDifferenceNerf = Pow(10, -Pow(timeDifference - lastTimeDifference, 2) / 1000);
            /// Instead of reducing the wide angle effect on high BPMs, the distance between notes must be large enough for the angle buff to be significant. 
            /// This is to prevent spaced streams from being buffed or nerfed too much, whilst also buffing fast, spaced, wide-angle patterns (Disconnected Hardkore DT).
            double lowDistanceNerf = Min(1, Pow(currentDistance / (640 * 6.4 * Pow(2, -0.05 * timeDifference)), 4));
            double wideAngleBuff = Pow(Sin(angle / 2), 2);
            return double.IsNaN(wideAngleBuff) ? 0 : wideAngleBuff * lowDistanceNerf * timeDifferenceNerf / 3;
        }

        /// Calculates the difficulty of a particular note.
        public double Difficulty(Beatmap.Note thirdLastNote, Beatmap.Note secondLastNote, Beatmap.Note lastNote, Beatmap.Note currentNote, Beatmap.Note nextNote, int speedUpMod, double circleSize, double tickRate, bool doPrint = false)
        {
            double currentDistance = Distance(currentNote, lastNote) + 10 * Cos(PI * Min(Distance(currentNote, lastNote), 25) / 50);
            double overlap = currentDistance / ConvertCircleSize(circleSize);
            double overlapNerf = Min(1, Pow(overlap, 2));
            double angle = Angle(secondLastNote, lastNote, currentNote);
            double lastAngle = Angle(thirdLastNote, secondLastNote, lastNote);
            double speedUpFactor = speedUpMod == 0 ? 1.0 : speedUpMod == 1 ? 1.5 : 0.75;

            double timeDifference = Max(minimumTime, (currentNote.time - lastNote.time) / speedUpFactor);
            double lastTimeDifference = Max(minimumTime, (lastNote.time - secondLastNote.time) / speedUpFactor);
            double speedBuff = Max(1, 2 - 3 * timeDifference / 200);
            double sliderDifficulty = 1 + Max(0, tickRate * currentNote.realTravelDistance) / Max(minimumTime, (nextNote.time - currentNote.time) / speedUpFactor);
            double baseDifficulty = (100 * currentDistance * sliderDifficulty + currentDistance * sliderDifficulty * timeDifference) / Pow(timeDifference, 3);
            double difficulty = baseDifficulty * speedBuff * (1 + overlapNerf * AngleChangeBonus(angle, lastAngle, timeDifference, lastTimeDifference)) * (1 + overlapNerf * AngleBonus(angle, currentDistance, timeDifference, lastTimeDifference));
            return difficulty;
        }

        /// Calculates the star rating of a beatmap.
        public double StarRating(List<double> difficulties, double circleSize)
        {
            int sectionLength = 4;
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
                sectionDifficulties[i] *= Pow(consistencyMultiplier, i);
            }
            return Pow(sectionDifficulties.Sum(), starRatingExponent) * starRatingMultiplier / Sqrt(ConvertCircleSize(circleSize)) * ShortMapNerf(difficulties.Count);
        }

        /// Calculates the pp of a beatmap.
        public double PP(double starRating, double overallDifficulty, double approachRate, double circleSize, int circleCount, List<Beatmap.Note> osuNotes, int modEnum = 17)
        {
            double basePP = ppMultiplier * Pow(starRating, ppExponent) * LengthBonus(circleCount);
            if (modEnum % (int)Mods.EZ == 0)
            {
                overallDifficulty /= 2;
                approachRate /= 2;
            }
            if (modEnum % (int)Mods.HR == 0)
            {
                overallDifficulty = Min(10, overallDifficulty * 1.4);
                approachRate = Min(10, approachRate * 1.4);
            }
            if (modEnum % (int)Mods.DT == 0)
            {
                if (modEnum % (int)Mods.HD == 0)
                {
                    basePP *= HiddenBonus(osuNotes, approachRate, circleSize, 1);
                }
                if (modEnum % (int)Mods.FL == 0)
                {
                    basePP *= FlashlightBonus(osuNotes, 1);
                }
                basePP *= OverallDifficultyBonus(overallDifficulty, 1) * HighApproachRateBonus(approachRate, 1) * HighDensityBonus(osuNotes, approachRate, 1);
            }
            if (modEnum % (int)Mods.HT == 0)
            {
                if (modEnum % (int)Mods.HD == 0)
                {
                    basePP *= HiddenBonus(osuNotes, approachRate, circleSize, -1);
                }
                if (modEnum % (int)Mods.FL == 0)
                {
                    basePP *= FlashlightBonus(osuNotes, -1);
                }
                basePP *= OverallDifficultyBonus(overallDifficulty, -1) * HighApproachRateBonus(approachRate, -1) * HighDensityBonus(osuNotes, approachRate, -1);
            }
            if (modEnum % (int)Mods.DT != 0 && modEnum % (int)Mods.HT != 0)
            {
                if (modEnum % (int)Mods.HD == 0)
                {
                    basePP *= HiddenBonus(osuNotes, approachRate, circleSize, 0);
                }
                if (modEnum % (int)Mods.FL == 0)
                {
                    basePP *= FlashlightBonus(osuNotes, 0);
                }
                basePP *= OverallDifficultyBonus(overallDifficulty, 0) * HighApproachRateBonus(approachRate, 0) * HighDensityBonus(osuNotes, approachRate, 0);
            }
            return Round(basePP, 2);
        }
    }
}
