using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVMusicExporter.Core.Models
{
    public class WeatherRateModel
    {
        public int Index { get; set; }
        public string Weather1 { get; set; }
        public int WeatherRate1 { get; set; }
        public string Weather2 { get; set; }
        public int WeatherRate2 { get; set; }
        public string Weather3 { get; set; }
        public int WeatherRate3 { get; set; }
        public string Weather4 { get; set; }
        public int WeatherRate4 { get; set; }
        public string Weather5 { get; set; }
        public int WeatherRate5 { get; set; }
        public string Weather6 { get; set; }
        public int WeatherRate6 { get; set; }
        public string Weather7 { get; set; }
        public int WeatherRate7 { get; set; }
        public string Weather8 { get; set; }
        public int WeatherRate8 { get; set; }

        public WeatherRateModel(int index, string w1, int wR1, string w2, int wR2, string w3, int wR3, string w4, int wR4, string w5, int wR5, string w6, int wR6, string w7, int wR7, string w8, int wR8)
        {
            Index = index;
            Weather1 = w1;
            WeatherRate1 = wR1;
            Weather2 = w2;
            WeatherRate2 = WeatherRate1 + wR2;
            Weather3 = w3;
            WeatherRate3 = WeatherRate2 + wR3;
            Weather4 = w4;
            WeatherRate4 = WeatherRate3 + wR4;
            Weather5 = w5;
            WeatherRate5 = WeatherRate4 + wR5;
            Weather6 = w6;
            WeatherRate6 = WeatherRate5 + wR6;
            Weather7 = w7;
            WeatherRate7 = WeatherRate6 + wR7;
            Weather8 = w8;
            WeatherRate8 = WeatherRate7 + wR8;
        }
    }
}