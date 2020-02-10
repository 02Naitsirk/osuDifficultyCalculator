using System;

namespace osuDifficultyCalculator
{
    class Output
    {
        static void Main()
        {
            Beatmap beatmap = new Beatmap();
            Calculate calculate = new Calculate();
            while (true)
            {
                string fileName = Console.ReadLine().Replace("\"", "");
                if (fileName.Trim().ToLowerInvariant() == "e")
                {
                    break;
                }
                try
                {
                    beatmap.GetBeatmapData(fileName);

                    /// Calculate difficulty for every note.
                    for (int i = 1; i < beatmap.osuNotes.Count; i++)
                    {
                        Beatmap.Note previousPreviousPreviousNote = i - 3 > -1 ? beatmap.osuNotes[i - 3] : new Beatmap.Note(beatmap.osuNotes[i].xCoordinate, beatmap.osuNotes[i].yCoordinate, double.NegativeInfinity, 12, 0, 0);
                        Beatmap.Note previousPreviousNote = i - 2 > -1 ? beatmap.osuNotes[i - 2] : new Beatmap.Note(beatmap.osuNotes[i].xCoordinate, beatmap.osuNotes[i].yCoordinate, double.MinValue, 12, 0, 0);
                        Beatmap.Note previousNote = i - 1 > -1 ? beatmap.osuNotes[i - 1] : new Beatmap.Note(beatmap.osuNotes[i].xCoordinate, beatmap.osuNotes[i].yCoordinate, int.MinValue, 12, 0, 0);
                        Beatmap.Note currentNote = beatmap.osuNotes[i];
                        Beatmap.Note nextNote = i + 1 < beatmap.osuNotes.Count ? beatmap.osuNotes[i + 1] : new Beatmap.Note(beatmap.osuNotes[i].xCoordinate, beatmap.osuNotes[i].yCoordinate, double.PositiveInfinity, 12, 0, 0);

                        double ezhtAimDifficulty = calculate.AimDifficulty(previousPreviousPreviousNote, previousPreviousNote, previousNote, currentNote, nextNote, -1, beatmap.circleSize / 2, beatmap.sliderTickRate);
                        double nmhtAimDifficulty = calculate.AimDifficulty(previousPreviousPreviousNote, previousPreviousNote, previousNote, currentNote, nextNote, -1, beatmap.circleSize, beatmap.sliderTickRate);
                        double hrhtAimDifficulty = calculate.AimDifficulty(previousPreviousPreviousNote, previousPreviousNote, previousNote, currentNote, nextNote, -1, Math.Min(10, beatmap.circleSize * 1.3), beatmap.sliderTickRate);

                        double ezAimDifficulty = calculate.AimDifficulty(previousPreviousPreviousNote, previousPreviousNote, previousNote, currentNote, nextNote, 0, beatmap.circleSize / 2, beatmap.sliderTickRate);
                        double nmAimDifficulty = calculate.AimDifficulty(previousPreviousPreviousNote, previousPreviousNote, previousNote, currentNote, nextNote, 0, beatmap.circleSize, beatmap.sliderTickRate);
                        double hrAimDifficulty = calculate.AimDifficulty(previousPreviousPreviousNote, previousPreviousNote, previousNote, currentNote, nextNote, 0, Math.Min(10, beatmap.circleSize * 1.3), beatmap.sliderTickRate);

                        double ezdtAimDifficulty = calculate.AimDifficulty(previousPreviousPreviousNote, previousPreviousNote, previousNote, currentNote, nextNote, 1, beatmap.circleSize / 2, beatmap.sliderTickRate);
                        double nmdtAimDifficulty = calculate.AimDifficulty(previousPreviousPreviousNote, previousPreviousNote, previousNote, currentNote, nextNote, 1, beatmap.circleSize, beatmap.sliderTickRate, true);
                        double hrdtAimDifficulty = calculate.AimDifficulty(previousPreviousPreviousNote, previousPreviousNote, previousNote, currentNote, nextNote, 1, Math.Min(10, beatmap.circleSize * 1.3), beatmap.sliderTickRate);

                        double htSpeedDifficulty = calculate.SpeedDifficulty(previousPreviousNote, previousNote, currentNote, -1);
                        double nmSpeedDifficulty = calculate.SpeedDifficulty(previousPreviousNote, previousNote, currentNote, 0);
                        double dtSpeedDifficulty = calculate.SpeedDifficulty(previousPreviousNote, previousNote, currentNote, 1);

                        beatmap.ezhtAimDifficulties.Add(ezhtAimDifficulty);
                        beatmap.nmhtAimDifficulties.Add(nmhtAimDifficulty);
                        beatmap.hrhtAimDifficulties.Add(hrhtAimDifficulty);

                        beatmap.ezAimDifficulties.Add(ezAimDifficulty);
                        beatmap.nmAimDifficulties.Add(nmAimDifficulty);
                        beatmap.hrAimDifficulties.Add(hrAimDifficulty);

                        beatmap.ezdtAimDifficulties.Add(ezdtAimDifficulty);
                        beatmap.nmdtAimDifficulties.Add(nmdtAimDifficulty);
                        beatmap.hrdtAimDifficulties.Add(hrdtAimDifficulty);

                        beatmap.htSpeedDifficulties.Add(htSpeedDifficulty);
                        beatmap.nmSpeedDifficulties.Add(nmSpeedDifficulty);
                        beatmap.dtSpeedDifficulties.Add(dtSpeedDifficulty);
                    }

                    /// Try to prevent SR from becoming extremely inflated.
                    for (int i = beatmap.nmAimDifficulties.Count - 1; i > 0; i--)
                    {
                        double ezhtAimProportion = beatmap.ezhtAimDifficulties[i] > 0 ? beatmap.ezhtAimDifficulties[i - 1] / beatmap.ezhtAimDifficulties[i] : double.PositiveInfinity;
                        double nmhtAimProportion = beatmap.nmhtAimDifficulties[i] > 0 ? beatmap.nmhtAimDifficulties[i - 1] / beatmap.nmhtAimDifficulties[i] : double.PositiveInfinity;
                        double hrhtAimProportion = beatmap.hrhtAimDifficulties[i] > 0 ? beatmap.hrhtAimDifficulties[i - 1] / beatmap.hrhtAimDifficulties[i] : double.PositiveInfinity;

                        double ezAimProportion = beatmap.ezAimDifficulties[i] > 0 ? beatmap.ezAimDifficulties[i - 1] / beatmap.ezAimDifficulties[i] : double.PositiveInfinity;
                        double nmAimProportion = beatmap.nmAimDifficulties[i] > 0 ? beatmap.nmAimDifficulties[i - 1] / beatmap.nmAimDifficulties[i] : double.PositiveInfinity;
                        double hrAimProportion = beatmap.hrAimDifficulties[i] > 0 ? beatmap.hrAimDifficulties[i - 1] / beatmap.hrAimDifficulties[i] : double.PositiveInfinity;

                        double ezdtAimProportion = beatmap.ezdtAimDifficulties[i] > 0 ? beatmap.ezdtAimDifficulties[i - 1] / beatmap.ezdtAimDifficulties[i] : double.PositiveInfinity;
                        double nmdtAimProportion = beatmap.nmdtAimDifficulties[i] > 0 ? beatmap.nmdtAimDifficulties[i - 1] / beatmap.nmdtAimDifficulties[i] : double.PositiveInfinity;
                        double hrdtAimProportion = beatmap.hrdtAimDifficulties[i] > 0 ? beatmap.hrdtAimDifficulties[i - 1] / beatmap.hrdtAimDifficulties[i] : double.PositiveInfinity;

                        double htSpeedProportion = beatmap.htSpeedDifficulties[i] > 0 ? beatmap.htSpeedDifficulties[i - 1] / beatmap.htSpeedDifficulties[i] : double.PositiveInfinity;
                        double nmSpeedProportion = beatmap.nmSpeedDifficulties[i] > 0 ? beatmap.nmSpeedDifficulties[i - 1] / beatmap.nmSpeedDifficulties[i] : double.PositiveInfinity;
                        double dtSpeedProportion = beatmap.dtSpeedDifficulties[i] > 0 ? beatmap.dtSpeedDifficulties[i - 1] / beatmap.dtSpeedDifficulties[i] : double.PositiveInfinity;

                        if (ezhtAimProportion < 0.5)
                            beatmap.ezhtAimDifficulties[i] *= 1 - 4 * Math.Pow(ezhtAimProportion - 0.5, 2);
                        if (nmhtAimProportion < 0.5)
                            beatmap.nmhtAimDifficulties[i] *= 1 - 4 * Math.Pow(nmhtAimProportion - 0.5, 2);
                        if (hrhtAimProportion < 0.5)
                            beatmap.hrhtAimDifficulties[i] *= 1 - 4 * Math.Pow(hrhtAimProportion - 0.5, 2);

                        if (ezAimProportion < 0.5)
                            beatmap.ezAimDifficulties[i] *= 1 - 4 * Math.Pow(ezAimProportion - 0.5, 2);
                        if (nmAimProportion < 0.5)
                            beatmap.nmAimDifficulties[i] *= 1 - 4 * Math.Pow(nmAimProportion - 0.5, 2);
                        if (hrAimProportion < 0.5)
                            beatmap.hrAimDifficulties[i] *= 1 - 4 * Math.Pow(hrAimProportion - 0.5, 2);

                        if (ezdtAimProportion < 0.5)
                            beatmap.ezdtAimDifficulties[i] *= 1 - 4 * Math.Pow(ezdtAimProportion - 0.5, 2);
                        if (nmdtAimProportion < 0.5)
                            beatmap.nmdtAimDifficulties[i] *= 1 - 4 * Math.Pow(nmdtAimProportion - 0.5, 2);
                        if (hrdtAimProportion < 0.5)
                            beatmap.hrdtAimDifficulties[i] *= 1 - 4 * Math.Pow(hrdtAimProportion - 0.5, 2);

                        if (htSpeedProportion < 0.5)
                            beatmap.htSpeedDifficulties[i] *= 1 - 4 * Math.Pow(htSpeedProportion - 0.5, 2);
                        if (nmSpeedProportion < 0.5)
                            beatmap.nmSpeedDifficulties[i] *= 1 - 4 * Math.Pow(nmSpeedProportion - 0.5, 2);
                        if (dtSpeedProportion < 0.5)
                            beatmap.dtSpeedDifficulties[i] *= 1 - 4 * Math.Pow(dtSpeedProportion - 0.5, 2);
                    }

                    /// Calculates aim and speed star rating and pp.
                    double consistencyMultiplier = 0.98;
                    double starRatingMultiplier = 350;

                    beatmap.ezhtAimStarRating = calculate.StarRating(beatmap.ezhtAimDifficulties, beatmap.circleSize / 2, consistencyMultiplier, starRatingMultiplier);
                    beatmap.nmhtAimStarRating = calculate.StarRating(beatmap.nmhtAimDifficulties, beatmap.circleSize, consistencyMultiplier, starRatingMultiplier);
                    beatmap.hrhtAimStarRating = calculate.StarRating(beatmap.hrhtAimDifficulties, Math.Min(10, beatmap.circleSize * 1.3), consistencyMultiplier, starRatingMultiplier);

                    beatmap.ezAimStarRating = calculate.StarRating(beatmap.ezAimDifficulties, beatmap.circleSize / 2, consistencyMultiplier, starRatingMultiplier);
                    beatmap.nmAimStarRating = calculate.StarRating(beatmap.nmAimDifficulties, beatmap.circleSize, consistencyMultiplier, starRatingMultiplier);
                    beatmap.hrAimStarRating = calculate.StarRating(beatmap.hrAimDifficulties, Math.Min(10, beatmap.circleSize * 1.3), consistencyMultiplier, starRatingMultiplier);

                    beatmap.ezdtAimStarRating = calculate.StarRating(beatmap.ezdtAimDifficulties, beatmap.circleSize / 2, consistencyMultiplier, starRatingMultiplier);
                    beatmap.nmdtAimStarRating = calculate.StarRating(beatmap.nmdtAimDifficulties, beatmap.circleSize, consistencyMultiplier, starRatingMultiplier);
                    beatmap.hrdtAimStarRating = calculate.StarRating(beatmap.hrdtAimDifficulties, Math.Min(10, beatmap.circleSize * 1.3), consistencyMultiplier, starRatingMultiplier);

                    beatmap.htSpeedStarRating = calculate.StarRating(beatmap.htSpeedDifficulties, beatmap.circleSize, 0.995, 0.67 * starRatingMultiplier);
                    beatmap.nmSpeedStarRating = calculate.StarRating(beatmap.nmSpeedDifficulties, beatmap.circleSize, 0.995, 0.67 * starRatingMultiplier);
                    beatmap.dtSpeedStarRating = calculate.StarRating(beatmap.dtSpeedDifficulties, beatmap.circleSize, 0.995, 0.67 * starRatingMultiplier);

                    double ezhtStarRating = beatmap.ezhtAimStarRating + beatmap.htSpeedStarRating + Math.Abs(beatmap.ezhtAimStarRating - beatmap.htSpeedStarRating) / 8;
                    double htStarRating = beatmap.nmhtAimStarRating + beatmap.htSpeedStarRating;
                    double hrhtStarRating = beatmap.hrhtAimStarRating + beatmap.htSpeedStarRating;

                    double ezStarRating = beatmap.ezAimStarRating + beatmap.nmSpeedStarRating + Math.Abs(beatmap.ezAimStarRating - beatmap.nmSpeedStarRating) / 8;
                    double nmStarRating = beatmap.nmAimStarRating + beatmap.nmSpeedStarRating + Math.Abs(beatmap.nmAimStarRating - beatmap.nmSpeedStarRating) / 8;
                    double hrStarRating = beatmap.hrAimStarRating + beatmap.nmSpeedStarRating + Math.Abs(beatmap.hrAimStarRating - beatmap.nmSpeedStarRating) / 8;

                    double ezdtStarRating = beatmap.ezdtAimStarRating + beatmap.dtSpeedStarRating + Math.Abs(beatmap.ezdtAimStarRating - beatmap.dtSpeedStarRating) / 8;
                    double dtStarRating = beatmap.nmdtAimStarRating + beatmap.dtSpeedStarRating + Math.Abs(beatmap.nmdtAimStarRating - beatmap.dtSpeedStarRating) / 8;
                    double hrdtStarRating = beatmap.hrdtAimStarRating + beatmap.dtSpeedStarRating + Math.Abs(beatmap.hrdtAimStarRating - beatmap.dtSpeedStarRating) / 8;

                    double nm = calculate.PP(nmStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes);

                    double dt = calculate.PP(dtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT);
                    double ez = calculate.PP(ezStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ);
                    double fl = calculate.PP(nmStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL);
                    double hd = calculate.PP(nmStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HD);
                    double hr = calculate.PP(hrStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HR);
                    double ht = calculate.PP(htStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HT);

                    double dtez = calculate.PP(ezdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.EZ);
                    double dtfl = calculate.PP(dtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.FL);
                    double dthd = calculate.PP(dtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.HD);
                    double dthr = calculate.PP(hrdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.HR);
                    double ezfl = calculate.PP(ezStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL);
                    double ezhd = calculate.PP(ezStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.HD);
                    double ezht = calculate.PP(ezhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.HT);
                    double flhd = calculate.PP(nmStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HD);
                    double flhr = calculate.PP(nmStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HR);
                    double flht = calculate.PP(htStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HT);
                    double hdhr = calculate.PP(hrStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HD * (int)Calculate.Mods.HR);
                    double hdht = calculate.PP(htStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HD * (int)Calculate.Mods.HT);
                    double hrht = calculate.PP(hrhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HR * (int)Calculate.Mods.HT);

                    double dtezfl = calculate.PP(ezdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL);
                    double dtezhd = calculate.PP(ezdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.EZ * (int)Calculate.Mods.HD);
                    double dtflhd = calculate.PP(dtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD);
                    double dtflhr = calculate.PP(hrdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.FL * (int)Calculate.Mods.HR);
                    double dthdhr = calculate.PP(hrdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.HD * (int)Calculate.Mods.HR);
                    double ezflhd = calculate.PP(ezStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD);
                    double ezflht = calculate.PP(ezhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL * (int)Calculate.Mods.HT);
                    double ezhdht = calculate.PP(ezhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.HD * (int)Calculate.Mods.HT);
                    double flhdhr = calculate.PP(hrStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HR);
                    double flhdht = calculate.PP(htStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HT);
                    double flhrht = calculate.PP(hrhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HR * (int)Calculate.Mods.HT);
                    double hdhrht = calculate.PP(hrhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HD * (int)Calculate.Mods.HR * (int)Calculate.Mods.HT);

                    double dtezflhd = calculate.PP(ezdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD);
                    double dtflhdhr = calculate.PP(hrdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HR);
                    double ezflhdht = calculate.PP(ezhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HT);
                    double flhdhrht = calculate.PP(hrhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HR * (int)Calculate.Mods.DT);

                    /// Outputs star rating and pp.
                    Console.WriteLine($"\n{beatmap.artist} - {beatmap.title} ({beatmap.creator}) [{beatmap.version}]");
                    Console.WriteLine($"CS: {beatmap.circleSize}, OD: {beatmap.overallDifficulty}, AR: {beatmap.approachRate}\n");
                    Console.WriteLine($"{"Mods",-10} {"Stars",-10} {"NM pp",-10} {"HD pp",-10} {"FL pp",-10} {"FLHD pp",-10}\n");

                    Console.WriteLine($"{"EZHT",-10} {Math.Round(ezhtStarRating, 2),-10} {ezht,-10} {ezhdht,-10} {ezflht,-10} {ezflhdht,-10}");
                    Console.WriteLine($"{"HT",-10} {Math.Round(htStarRating, 2),-10} {ht,-10} {hdht,-10} {flht,-10} {flhdht,-10}");
                    Console.WriteLine($"{"HRHT",-10} {Math.Round(hrhtStarRating, 2),-10} {hrht,-10} {hdhrht,-10} {flhrht,-10} {flhdhrht,-10}\n");

                    Console.WriteLine($"{"EZ",-10} {Math.Round(ezStarRating, 2),-10} {ez,-10} {ezhd,-10} {ezfl,-10} {ezflhd,-10}");
                    Console.WriteLine($"{"NM",-10} {Math.Round(nmStarRating, 2),-10} {nm,-10} {hd,-10} {fl,-10} {flhd,-10}");
                    Console.WriteLine($"{"HR",-10} {Math.Round(hrStarRating, 2),-10} {hr,-10} {hdhr,-10} {flhr,-10} {flhdhr,-10}\n");

                    Console.WriteLine($"{"EZDT",-10} {Math.Round(ezdtStarRating, 2),-10} {dtez,-10} {dtezhd,-10} {dtezfl,-10} {dtezflhd,-10}");
                    Console.WriteLine($"{"DT",-10} {Math.Round(dtStarRating, 2),-10} {dt,-10} {dthd,-10} {dtfl,-10} {dtflhd,-10}");
                    Console.WriteLine($"{"HRDT",-10} {Math.Round(hrdtStarRating, 2),-10} {dthr,-10} {dthdhr,-10} {dtflhr,-10} {dtflhdhr,-10}\n");

                    /// Reset objects in preparation for the next beatmap.
                    beatmap.osuNotes.Clear();

                    beatmap.ezhtAimDifficulties.Clear();
                    beatmap.nmhtAimDifficulties.Clear();
                    beatmap.hrhtAimDifficulties.Clear();

                    beatmap.ezAimDifficulties.Clear();
                    beatmap.nmAimDifficulties.Clear();
                    beatmap.hrAimDifficulties.Clear();

                    beatmap.ezdtAimDifficulties.Clear();
                    beatmap.nmdtAimDifficulties.Clear();
                    beatmap.hrdtAimDifficulties.Clear();

                    beatmap.htSpeedDifficulties.Clear();
                    beatmap.nmSpeedDifficulties.Clear();
                    beatmap.dtSpeedDifficulties.Clear();

                    beatmap.circleCount = 0;
                }

                /// In case of invalid input.
                catch (Exception ex)
                {
                    Console.WriteLine($"\n{ex.ToString()}\n", Console.ForegroundColor = ConsoleColor.Red);
                    Console.ResetColor();
                }
            }
        }
    }
}
