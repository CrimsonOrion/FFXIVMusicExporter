namespace FFXIVMusicExporter.Core.Models
{
    public class WeatherTypeModel
    {
        public int Key { get; set; }
        public string Value { get; set; }

        public WeatherTypeModel()
        {
            Key = 0;
            Value = string.Empty;
        }

        public WeatherTypeModel(int key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}