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
                        Beatmap.Note thirdLastNote = i - 3 > -1 ? beatmap.osuNotes[i - 3] : new Beatmap.Note(beatmap.osuNotes[i].xCoordinate, beatmap.osuNotes[i].yCoordinate, double.NegativeInfinity, 0);
                        Beatmap.Note secondLastNote = i - 2 > -1 ? beatmap.osuNotes[i - 2] : new Beatmap.Note(beatmap.osuNotes[i].xCoordinate, beatmap.osuNotes[i].yCoordinate, double.MinValue, 0);
                        Beatmap.Note lastNote = i - 1 > -1 ? beatmap.osuNotes[i - 1] : new Beatmap.Note(beatmap.osuNotes[i].xCoordinate, beatmap.osuNotes[i].yCoordinate, int.MinValue, 0);
                        Beatmap.Note currentNote = beatmap.osuNotes[i];
                        Beatmap.Note nextNote = i + 1 < beatmap.osuNotes.Count ? beatmap.osuNotes[i + 1] : new Beatmap.Note(beatmap.osuNotes[i].xCoordinate, beatmap.osuNotes[i].yCoordinate, double.PositiveInfinity, 0);

                        double ezhtDifficulty = calculate.Difficulty(thirdLastNote, secondLastNote, lastNote, currentNote, nextNote, -1, beatmap.circleSize / 2, beatmap.sliderTickRate);
                        double nmhtDifficulty = calculate.Difficulty(thirdLastNote, secondLastNote, lastNote, currentNote, nextNote, -1, beatmap.circleSize, beatmap.sliderTickRate);
                        double hrhtDifficulty = calculate.Difficulty(thirdLastNote, secondLastNote, lastNote, currentNote, nextNote, -1, Math.Min(10, beatmap.circleSize * 1.3), beatmap.sliderTickRate);

                        double ezDifficulty = calculate.Difficulty(thirdLastNote, secondLastNote, lastNote, currentNote, nextNote, 0, beatmap.circleSize / 2, beatmap.sliderTickRate);
                        double nmDifficulty = calculate.Difficulty(thirdLastNote, secondLastNote, lastNote, currentNote, nextNote, 0, beatmap.circleSize, beatmap.sliderTickRate);
                        double hrDifficulty = calculate.Difficulty(thirdLastNote, secondLastNote, lastNote, currentNote, nextNote, 0, Math.Min(10, beatmap.circleSize * 1.3), beatmap.sliderTickRate);

                        double ezdtDifficulty = calculate.Difficulty(thirdLastNote, secondLastNote, lastNote, currentNote, nextNote, 1, beatmap.circleSize / 2, beatmap.sliderTickRate);
                        double nmdtDifficulty = calculate.Difficulty(thirdLastNote, secondLastNote, lastNote, currentNote, nextNote, 1, beatmap.circleSize, beatmap.sliderTickRate);
                        double hrdtDifficulty = calculate.Difficulty(thirdLastNote, secondLastNote, lastNote, currentNote, nextNote, 1, Math.Min(10, beatmap.circleSize * 1.3), beatmap.sliderTickRate);

                        beatmap.ezhtDifficulties.Add(ezhtDifficulty);
                        beatmap.nmhtDifficulties.Add(nmhtDifficulty);
                        beatmap.hrhtDifficulties.Add(hrhtDifficulty);

                        beatmap.ezDifficulties.Add(ezDifficulty);
                        beatmap.nmDifficulties.Add(nmDifficulty);
                        beatmap.hrDifficulties.Add(hrDifficulty);

                        beatmap.ezdtDifficulties.Add(ezdtDifficulty);
                        beatmap.nmdtDifficulties.Add(nmdtDifficulty);
                        beatmap.hrdtDifficulties.Add(hrdtDifficulty);
                    }

                    /// Try to prevent SR from becoming extremely inflated.
                    for (int i = beatmap.nmDifficulties.Count - 1; i > 0; i--)
                    {
                        double ezhtProportion = beatmap.ezhtDifficulties[i] > 0 ? beatmap.ezhtDifficulties[i - 1] / beatmap.ezhtDifficulties[i] : double.PositiveInfinity;
                        double nmhtProportion = beatmap.nmhtDifficulties[i] > 0 ? beatmap.nmhtDifficulties[i - 1] / beatmap.nmhtDifficulties[i] : double.PositiveInfinity;
                        double hrhtProportion = beatmap.hrhtDifficulties[i] > 0 ? beatmap.hrhtDifficulties[i - 1] / beatmap.hrhtDifficulties[i] : double.PositiveInfinity;

                        double ezProportion = beatmap.ezDifficulties[i] > 0 ? beatmap.ezDifficulties[i - 1] / beatmap.ezDifficulties[i] : double.PositiveInfinity;
                        double nmProportion = beatmap.nmDifficulties[i] > 0 ? beatmap.nmDifficulties[i - 1] / beatmap.nmDifficulties[i] : double.PositiveInfinity;
                        double hrProportion = beatmap.hrDifficulties[i] > 0 ? beatmap.hrDifficulties[i - 1] / beatmap.hrDifficulties[i] : double.PositiveInfinity;

                        double ezdtProportion = beatmap.ezdtDifficulties[i] > 0 ? beatmap.ezdtDifficulties[i - 1] / beatmap.ezdtDifficulties[i] : double.PositiveInfinity;
                        double nmdtProportion = beatmap.nmdtDifficulties[i] > 0 ? beatmap.nmdtDifficulties[i - 1] / beatmap.nmdtDifficulties[i] : double.PositiveInfinity;
                        double hrdtProportion = beatmap.hrdtDifficulties[i] > 0 ? beatmap.hrdtDifficulties[i - 1] / beatmap.hrdtDifficulties[i] : double.PositiveInfinity;

                        const int h = 4;
                        if (ezhtProportion < 0.5)
                            beatmap.ezhtDifficulties[i] *= 1 - h * Math.Pow(ezhtProportion - 0.5, 2);
                        if (nmhtProportion < 0.5)
                            beatmap.nmhtDifficulties[i] *= 1 - h * Math.Pow(nmhtProportion - 0.5, 2);
                        if (hrhtProportion < 0.5)
                            beatmap.hrhtDifficulties[i] *= 1 - h * Math.Pow(hrhtProportion - 0.5, 2);

                        if (ezProportion < 0.5)
                            beatmap.ezDifficulties[i] *= 1 - h * Math.Pow(ezProportion - 0.5, 2);
                        if (nmProportion < 0.5)
                            beatmap.nmDifficulties[i] *= 1 - h * Math.Pow(nmProportion - 0.5, 2);
                        if (hrProportion < 0.5)
                            beatmap.hrDifficulties[i] *= 1 - h * Math.Pow(hrProportion - 0.5, 2);

                        if (ezdtProportion < 0.5)
                            beatmap.ezdtDifficulties[i] *= 1 - h * Math.Pow(ezdtProportion - 0.5, 2);
                        if (nmdtProportion < 0.5)
                            beatmap.nmdtDifficulties[i] *= 1 - h * Math.Pow(nmdtProportion - 0.5, 2);
                        if (hrdtProportion < 0.5)
                            beatmap.hrdtDifficulties[i] *= 1 - h * Math.Pow(hrdtProportion - 0.5, 2);
                    }

                    /// Calculates star rating and pp.
                    beatmap.ezhtStarRating = calculate.StarRating(beatmap.ezhtDifficulties, beatmap.circleSize / 2);
                    beatmap.nmhtStarRating = calculate.StarRating(beatmap.nmhtDifficulties, beatmap.circleSize);
                    beatmap.hrhtStarRating = calculate.StarRating(beatmap.hrhtDifficulties, Math.Min(10, beatmap.circleSize * 1.3));

                    beatmap.ezStarRating = calculate.StarRating(beatmap.ezDifficulties, beatmap.circleSize / 2);
                    beatmap.nmStarRating = calculate.StarRating(beatmap.nmDifficulties, beatmap.circleSize);
                    beatmap.hrStarRating = calculate.StarRating(beatmap.hrDifficulties, Math.Min(10, beatmap.circleSize * 1.3));

                    beatmap.ezdtStarRating = calculate.StarRating(beatmap.ezdtDifficulties, beatmap.circleSize / 2);
                    beatmap.nmdtStarRating = calculate.StarRating(beatmap.nmdtDifficulties, beatmap.circleSize);
                    beatmap.hrdtStarRating = calculate.StarRating(beatmap.hrdtDifficulties, Math.Min(10, beatmap.circleSize * 1.3));

                    double nm = calculate.PP(beatmap.nmStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes);

                    double dt = calculate.PP(beatmap.nmdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT);
                    double ez = calculate.PP(beatmap.ezStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ);
                    double fl = calculate.PP(beatmap.nmStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL);
                    double hd = calculate.PP(beatmap.nmStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HD);
                    double hr = calculate.PP(beatmap.hrStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HR);
                    double ht = calculate.PP(beatmap.nmhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HT);

                    double dtez = calculate.PP(beatmap.ezdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.EZ);
                    double dtfl = calculate.PP(beatmap.nmdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.FL);
                    double dthd = calculate.PP(beatmap.nmdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.HD);
                    double dthr = calculate.PP(beatmap.hrdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.HR);
                    double ezfl = calculate.PP(beatmap.ezStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL);
                    double ezhd = calculate.PP(beatmap.ezStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.HD);
                    double ezht = calculate.PP(beatmap.ezhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.HT);
                    double flhd = calculate.PP(beatmap.nmStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HD);
                    double flhr = calculate.PP(beatmap.hrStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HR);
                    double flht = calculate.PP(beatmap.nmhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HT);
                    double hdhr = calculate.PP(beatmap.hrStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HD * (int)Calculate.Mods.HR);
                    double hdht = calculate.PP(beatmap.nmhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HD * (int)Calculate.Mods.HT);
                    double hrht = calculate.PP(beatmap.hrhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HR * (int)Calculate.Mods.HT);

                    double dtezfl = calculate.PP(beatmap.ezdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL);
                    double dtezhd = calculate.PP(beatmap.ezdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.EZ * (int)Calculate.Mods.HD);
                    double dtflhd = calculate.PP(beatmap.nmdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD);
                    double dtflhr = calculate.PP(beatmap.hrdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.FL * (int)Calculate.Mods.HR);
                    double dthdhr = calculate.PP(beatmap.hrdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.HD * (int)Calculate.Mods.HR);
                    double ezflhd = calculate.PP(beatmap.ezStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD);
                    double ezflht = calculate.PP(beatmap.ezhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL * (int)Calculate.Mods.HT);
                    double ezhdht = calculate.PP(beatmap.ezhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.HD * (int)Calculate.Mods.HT);
                    double flhdhr = calculate.PP(beatmap.hrStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HR);
                    double flhdht = calculate.PP(beatmap.nmhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HT);
                    double flhrht = calculate.PP(beatmap.hrhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HR * (int)Calculate.Mods.HT);
                    double hdhrht = calculate.PP(beatmap.hrhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.HD * (int)Calculate.Mods.HR * (int)Calculate.Mods.HT);

                    double dtezflhd = calculate.PP(beatmap.ezdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD);
                    double dtflhdhr = calculate.PP(beatmap.hrdtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.DT * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HR);
                    double ezflhdht = calculate.PP(beatmap.ezhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.EZ * (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HT);
                    double flhdhrht = calculate.PP(beatmap.hrhtStarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, beatmap.osuNotes, (int)Calculate.Mods.FL * (int)Calculate.Mods.HD * (int)Calculate.Mods.HR * (int)Calculate.Mods.DT);

                    /// Outputs star rating and pp.
                    Console.WriteLine($"\n{beatmap.artist} - {beatmap.title} ({beatmap.creator}) [{beatmap.version}]");
                    Console.WriteLine($"CS: {beatmap.circleSize}, OD: {beatmap.overallDifficulty}, AR: {beatmap.approachRate}\n");
                    Console.WriteLine($"{"Mods",-10} {"Stars",-10} {"NM pp",-10} {"HD pp",-10} {"FL pp",-10} {"FLHD pp",-10}\n");

                    Console.WriteLine($"{"EZHT",-10} {Math.Round(beatmap.ezhtStarRating, 2),-10} {ezht,-10} {ezhdht,-10} {ezflht,-10} {ezflhdht,-10}");
                    Console.WriteLine($"{"HT",-10} {Math.Round(beatmap.nmhtStarRating, 2),-10} {ht,-10} {hdht,-10} {flht,-10} {flhdht,-10}");
                    Console.WriteLine($"{"HRHT",-10} {Math.Round(beatmap.hrhtStarRating, 2),-10} {hrht,-10} {hdhrht,-10} {flhrht,-10} {flhdhrht,-10}\n");

                    Console.WriteLine($"{"EZ",-10} {Math.Round(beatmap.ezStarRating, 2),-10} {ez,-10} {ezhd,-10} {ezfl,-10} {ezflhd,-10}");
                    Console.WriteLine($"{"NM",-10} {Math.Round(beatmap.nmStarRating, 2),-10} {nm,-10} {hd,-10} {fl,-10} {flhd,-10}");
                    Console.WriteLine($"{"HR",-10} {Math.Round(beatmap.hrStarRating, 2),-10} {hr,-10} {hdhr,-10} {flhr,-10} {flhdhr,-10}\n");

                    Console.WriteLine($"{"EZDT",-10} {Math.Round(beatmap.ezdtStarRating, 2),-10} {dtez,-10} {dtezhd,-10} {dtezfl,-10} {dtezflhd,-10}");
                    Console.WriteLine($"{"DT",-10} {Math.Round(beatmap.nmdtStarRating, 2),-10} {dt,-10} {dthd,-10} {dtfl,-10} {dtflhd,-10}");
                    Console.WriteLine($"{"HRDT",-10} {Math.Round(beatmap.hrdtStarRating, 2),-10} {dthr,-10} {dthdhr,-10} {dtflhr,-10} {dtflhdhr,-10}\n");

                    /// Reset objects in preparation for the next beatmap.
                    beatmap.osuNotes.Clear();

                    beatmap.ezhtDifficulties.Clear();
                    beatmap.nmhtDifficulties.Clear();
                    beatmap.hrhtDifficulties.Clear();

                    beatmap.ezDifficulties.Clear();
                    beatmap.nmDifficulties.Clear();
                    beatmap.hrDifficulties.Clear();

                    beatmap.ezdtDifficulties.Clear();
                    beatmap.nmdtDifficulties.Clear();
                    beatmap.hrdtDifficulties.Clear();

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
