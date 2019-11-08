using System.Collections.Generic;

namespace osuDifficultyCalculator
{
    public class Beatmap
    {
        public double circleSize, overallDifficulty, approachRate,
        starRating, HRstarRating, DTstarRating, DTHRstarRating,
        nmPP, hdPP, hrPP, hdhrPP, dtPP, dthdPP, dthrPP, dthdhrPP;
        public string title, artist, creator, version;
        public List<int> xCoordinates = new List<int>();
        public List<int> yCoordinates = new List<int>();
        public List<int> timeCoordinates = new List<int>();
        public List<int> objectTypes = new List<int>();
        public List<double> strains = new List<double>();
        public List<double> DTstrains = new List<double>();
        public List<double> distances = new List<double>();
        public List<double> timeDifferences = new List<double>();
        public double circleCount = 0;
    }
}
