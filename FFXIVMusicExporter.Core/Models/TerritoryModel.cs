namespace FFXIVMusicExporter.Core.Models
{
    public class TerritoryModel
    {
        public int Index { get; set; }
        public string PlaceName { get; set; }
        public int WeatherRateIndex { get; set; }

        public TerritoryModel(int index, string placeName, int weatherRateIndex)
        {
            Index = index;
            PlaceName = placeName;
            WeatherRateIndex = weatherRateIndex;
        }
    }
}