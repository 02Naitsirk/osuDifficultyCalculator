using System;
using System.IO;
using System.Linq;

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
                Console.WriteLine("Beatmap: ");
                string fileName = Console.ReadLine();
                fileName = fileName.Replace("\"", "");
                string line;
                try
                {
                    using (StreamReader file = new StreamReader(fileName))
                    {
                        while ((line = file.ReadLine()) != null) /// Get beatmap information by reading the .osu file.
                        {
                            if (line.Contains("Title:"))
                            {
                                beatmap.title = line.Split(new[] { ':' }, 2)[1];
                            }
                            if (line.Contains("Artist:"))
                            {
                                beatmap.artist = line.Split(new[] { ':' }, 2)[1];
                            }
                            if (line.Contains("Creator:"))
                            {
                                beatmap.creator = line.Split(new[] { ':' }, 2)[1];
                            }
                            if (line.Contains("Version:"))
                            {
                                beatmap.version = line.Split(new[] { ':' }, 2)[1];
                            }
                            if (line.Contains("CircleSize:"))
                            {
                                beatmap.circleSize = Convert.ToDouble(line.Split(':')[1]);
                            }
                            if (line.Contains("OverallDifficulty:"))
                            {
                                beatmap.overallDifficulty = Convert.ToDouble(line.Split(':')[1]);
                                beatmap.approachRate = Convert.ToDouble(line.Split(':')[1]); /// In case an approach rate is not specified.
                            }
                            if (line.Contains("ApproachRate:"))
                            {
                                beatmap.approachRate = Convert.ToDouble(line.Split(':')[1]);
                            }
                            if (line.Contains("[HitObjects]"))
                            {
                                while ((line = file.ReadLine()) != null)
                                {
                                    beatmap.xCoordinates.Add(Convert.ToInt32(line.Split(',')[0]));
                                    beatmap.yCoordinates.Add(Convert.ToInt32(line.Split(',')[1]));
                                    beatmap.timeCoordinates.Add(Convert.ToInt32(line.Split(',')[2]));
                                    beatmap.objectTypes.Add(Convert.ToInt32(line.Split(',')[3]));
                                    if (!line.Contains('|'))
                                    {
                                        beatmap.circleCount++;
                                    }
                                }
                            }
                        }
                    }
                    foreach (int objectType in beatmap.objectTypes.ToArray()) /// Remove every spinner.
                    {
                        if (objectType == 12)
                        {
                            int spinner = beatmap.objectTypes.IndexOf(objectType);
                            beatmap.objectTypes.RemoveAt(spinner);
                            beatmap.xCoordinates.RemoveAt(spinner);
                            beatmap.yCoordinates.RemoveAt(spinner);
                            beatmap.timeCoordinates.RemoveAt(spinner);
                            beatmap.circleCount--;
                        }
                    }
                    for (int i = 0; i < beatmap.objectTypes.Count; i++) /// Preparation to calculate star rating.
                    {
                        beatmap.distances.Add(calculate.Distance(beatmap.xCoordinates[i], beatmap.yCoordinates[i], beatmap.xCoordinates[Math.Max(0, i - 1)], beatmap.yCoordinates[Math.Max(0, i - 1)]));
                        beatmap.timeDifferences.Add(Math.Max(1, beatmap.timeCoordinates[i] - beatmap.timeCoordinates[Math.Max(0, i - 1)]));
                        beatmap.strains.Add(calculate.Strain(beatmap.distances[i], beatmap.timeDifferences[i]) * (1 + Math.Min(1, Math.Pow(beatmap.distances[i] / calculate.Diameter(beatmap.circleSize), 2)) * calculate.AngleBuff(calculate.Angle(beatmap.xCoordinates[Math.Max(0, i - 2)], beatmap.yCoordinates[Math.Max(0, i - 2)], beatmap.xCoordinates[Math.Max(0, i - 1)], beatmap.yCoordinates[Math.Max(0, i - 1)], beatmap.xCoordinates[i], beatmap.yCoordinates[i]), beatmap.timeDifferences[i], beatmap.timeDifferences[Math.Max(0, i - 1)])));
                        beatmap.HRstrains.Add(calculate.Strain(beatmap.distances[i], beatmap.timeDifferences[i]) * (1 + Math.Min(1, Math.Pow(beatmap.distances[i] / calculate.Diameter(Math.Min(10, beatmap.circleSize * 1.3)), 2)) * calculate.AngleBuff(calculate.Angle(beatmap.xCoordinates[Math.Max(0, i - 2)], beatmap.yCoordinates[Math.Max(0, i - 2)], beatmap.xCoordinates[Math.Max(0, i - 1)], beatmap.yCoordinates[Math.Max(0, i - 1)], beatmap.xCoordinates[i], beatmap.yCoordinates[i]), beatmap.timeDifferences[i], beatmap.timeDifferences[Math.Max(0, i - 1)])));
                        beatmap.DTstrains.Add(calculate.Strain(beatmap.distances[i], Math.Max(1, 2.0 / 3.0 * beatmap.timeDifferences[i])) * (1 + Math.Min(1, Math.Pow(beatmap.distances[i] / calculate.Diameter(beatmap.circleSize), 2)) * calculate.AngleBuff(calculate.Angle(beatmap.xCoordinates[Math.Max(0, i - 2)], beatmap.yCoordinates[Math.Max(0, i - 2)], beatmap.xCoordinates[Math.Max(0, i - 1)], beatmap.yCoordinates[Math.Max(0, i - 1)], beatmap.xCoordinates[i], beatmap.yCoordinates[i]), Math.Max(1, 2.0 / 3.0 * beatmap.timeDifferences[i]), Math.Max(1, 2.0 / 3.0 * beatmap.timeDifferences[Math.Max(0, i - 1)]))));
                        beatmap.DTHRstrains.Add(calculate.Strain(beatmap.distances[i], Math.Max(1, 2.0 / 3.0 * beatmap.timeDifferences[i])) * (1 + Math.Min(1, Math.Pow(beatmap.distances[i] / calculate.Diameter(Math.Min(10, beatmap.circleSize * 1.3)), 2)) * calculate.AngleBuff(calculate.Angle(beatmap.xCoordinates[Math.Max(0, i - 2)], beatmap.yCoordinates[Math.Max(0, i - 2)], beatmap.xCoordinates[Math.Max(0, i - 1)], beatmap.yCoordinates[Math.Max(0, i - 1)], beatmap.xCoordinates[i], beatmap.yCoordinates[i]), Math.Max(1, 2.0 / 3.0 * beatmap.timeDifferences[i]), Math.Max(1, 2.0 / 3.0 * beatmap.timeDifferences[Math.Max(0, i - 1)]))));
                    }
                    for (int i = beatmap.strains.Count - 1; i > 0; i--) /// Try to prevent SR from becoming extremely inflated by reducing some very high strains.
                    {
                        double proportion = beatmap.strains[i] > 0 ? beatmap.strains[i - 1] / beatmap.strains[i] : double.PositiveInfinity;
                        if (proportion < 0.5)
                        {
                            beatmap.strains[i] *= 1 - 4 * Math.Pow(proportion - 0.5, 2);
                            beatmap.HRstrains[i] *= 1 - 4 * Math.Pow(proportion - 0.5, 2);
                            beatmap.DTstrains[i] *= 1 - 4 * Math.Pow(proportion - 0.5, 2);
                            beatmap.DTHRstrains[i] *= 1 - 4 * Math.Pow(proportion - 0.5, 2);
                        }
                    }

                    /// Calculates star rating and pp.
                    beatmap.starRating = calculate.StarRating(beatmap.strains, beatmap.circleSize);
                    beatmap.HRstarRating = calculate.StarRating(beatmap.HRstrains, Math.Min(10, beatmap.circleSize * 1.3));
                    beatmap.DTstarRating = calculate.StarRating(beatmap.DTstrains, beatmap.circleSize);
                    beatmap.DTHRstarRating = calculate.StarRating(beatmap.DTHRstrains, Math.Min(10, beatmap.circleSize * 1.3));

                    beatmap.nmPP = calculate.PP(beatmap.starRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount);
                    beatmap.hdPP = calculate.PP(beatmap.starRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, "HD");
                    beatmap.hrPP = calculate.PP(beatmap.HRstarRating, Math.Min(10, beatmap.overallDifficulty * 1.4), Math.Min(10, beatmap.approachRate * 1.4), beatmap.circleCount, "HR");
                    beatmap.hdhrPP = calculate.PP(beatmap.HRstarRating, Math.Min(10, beatmap.overallDifficulty * 1.4), Math.Min(10, beatmap.approachRate * 1.4), beatmap.circleCount, "HDHR");

                    beatmap.dtPP = calculate.PP(beatmap.DTstarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, "DT");
                    beatmap.dthdPP = calculate.PP(beatmap.DTstarRating, beatmap.overallDifficulty, beatmap.approachRate, beatmap.circleCount, "DTHD");
                    beatmap.dthrPP = calculate.PP(beatmap.DTHRstarRating, Math.Min(10, beatmap.overallDifficulty * 1.4), Math.Min(10, beatmap.approachRate * 1.4), beatmap.circleCount, "DTHR");
                    beatmap.dthdhrPP = calculate.PP(beatmap.DTHRstarRating, Math.Min(10, beatmap.overallDifficulty * 1.4), Math.Min(10, beatmap.approachRate * 1.4), beatmap.circleCount, "DTHDHR");

                    /// Outputs star rating and pp.
                    Console.WriteLine($"\n{beatmap.artist} - {beatmap.title} ({beatmap.creator}) [{beatmap.version}]");
                    Console.WriteLine($"CS: {beatmap.circleSize}, OD: {beatmap.overallDifficulty}, AR: {beatmap.approachRate}");
                    Console.WriteLine($"{"Mods",-10} {"Stars",-10} {"NM pp",-10} {"HD pp",-10} {"FL pp",-10} {"FLHD pp",-10}");
                    Console.WriteLine($"{"NM",-10} {Math.Round(beatmap.starRating, 2) + "*",-10} {Math.Round(beatmap.nmPP, 2) + "pp",-10} {Math.Round(beatmap.hdPP, 2) + "pp",-10} {Math.Round(beatmap.nmPP * calculate.FlashlightBonus(beatmap.distances, beatmap.timeDifferences), 2) + "pp",-10} {Math.Round(beatmap.hdPP * calculate.FlashlightBonus(beatmap.distances, beatmap.timeDifferences), 2) + "pp",-10}");
                    Console.WriteLine($"{"HR",-10} {Math.Round(beatmap.HRstarRating, 2) + "*",-10} {Math.Round(beatmap.hrPP, 2) + "pp",-10} {Math.Round(beatmap.hdhrPP, 2) + "pp",-10} {Math.Round(beatmap.hrPP * calculate.FlashlightBonus(beatmap.distances, beatmap.timeDifferences), 2) + "pp",-10} {Math.Round(beatmap.hdhrPP * calculate.FlashlightBonus(beatmap.distances, beatmap.timeDifferences), 2) + "pp",-10}");
                    Console.WriteLine($"{"DT",-10} {Math.Round(beatmap.DTstarRating, 2) + "*",-10} {Math.Round(beatmap.dtPP, 2) + "pp",-10} {Math.Round(beatmap.dthdPP, 2) + "pp",-10} {Math.Round(beatmap.dtPP * calculate.FlashlightBonus(beatmap.distances, beatmap.timeDifferences, true), 2) + "pp",-10} {Math.Round(beatmap.dthdPP * calculate.FlashlightBonus(beatmap.distances, beatmap.timeDifferences, true), 2) + "pp",-10}");
                    Console.WriteLine($"{"DTHR",-10} {Math.Round(beatmap.DTHRstarRating, 2) + "*",-10} {Math.Round(beatmap.dthrPP, 2) + "pp",-10} {Math.Round(beatmap.dthdhrPP, 2) + "pp",-10} {Math.Round(beatmap.dthrPP * calculate.FlashlightBonus(beatmap.distances, beatmap.timeDifferences, true), 2) + "pp",-10} {Math.Round(beatmap.dthdhrPP * calculate.FlashlightBonus(beatmap.distances, beatmap.timeDifferences, true), 2) + "pp",-10}\n");

                    /// Clear all lists in preparation for the next beatmap.
                    beatmap.xCoordinates.Clear();
                    beatmap.yCoordinates.Clear();
                    beatmap.timeCoordinates.Clear();
                    beatmap.objectTypes.Clear();
                    beatmap.strains.Clear();
                    beatmap.HRstrains.Clear();
                    beatmap.DTstrains.Clear();
                    beatmap.DTHRstrains.Clear();
                    beatmap.distances.Clear();
                    beatmap.timeDifferences.Clear();
                    beatmap.circleCount = 0;
                }
                catch (Exception ex) /// In case of invalid input.
                {
                    Console.WriteLine($"\n{ex.Message.ToString()}\n", Console.ForegroundColor = ConsoleColor.Red);
                    Console.ResetColor();
                }
            }
        }
    }
}
