using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SaintCoinach.Xiv;

using SaintCoinach;

namespace FFXIVMusicExporter.Core.Weather
{
    public class Weather : IWeather
    {
        private readonly IRealm _realm;
        //private readonly ISendMessageEvent _sendMessageEvent;

        private readonly List<TerritoryType> _territoryList = new List<TerritoryType>();

        public Weather(IRealm realm)
        {
            _realm = realm;

            _territoryList = LoadZones();
        }

        public List<string> GetTerritoryPlaceNames() => _territoryList.Select(_ => _.PlaceName.Name.ToString()).ToList();

        public async Task GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones, int forcastIntervals, CancellationToken cancellationToken)
        {
            if (zones == null)
            {
                foreach (var territory in _territoryList)
                {
                    var eorzeaDateTime = new EorzeaDateTime(dateTime);
                    var zone = territory.PlaceName;
                    for (var i = 0; i < forcastIntervals; i++)
                    {
                        var weather = await Task.Run(() => territory.WeatherRate.Forecast(eorzeaDateTime).Name, cancellationToken);
                        //var localTime = eorzeaDateTime.GetRealTime().ToLocalTime();
                        //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs($"{zone} - {weather}"));
                        eorzeaDateTime = Increment(eorzeaDateTime);
                    }
                }
            }
            else
            {
                foreach (var zone in zones)
                {
                    var eorzeaDateTime = new EorzeaDateTime(dateTime);

                    for (var i = 0; i < forcastIntervals; i++)
                    {
                        var weather = await Task.Run(() => _territoryList.FirstOrDefault(_ => _.PlaceName.ToString() == zone).WeatherRate.Forecast(eorzeaDateTime).Name, cancellationToken);
                        var localTime = eorzeaDateTime.GetRealTime().ToLocalTime();
                        //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs($"{eorzeaDateTime}({localTime}): {zone} - {weather}"));
                        eorzeaDateTime = Increment(eorzeaDateTime);
                    }
                }
            }
        }

        public void GetMoonPhase(EorzeaDateTime eDate)
        {
            string[] moons = { "New Moon", "Waxing Crescent", "First Quarter", "Waxing Gibbous", "Full Moon", "Waning Gibbous", "Last Quarter", "Waning Crescent" };

            if (eDate == null)
            {
                eDate = new EorzeaDateTime(DateTime.Now);
            }

            var daysIntoCycle = DaysIntoLunarCycle(eDate);
            // 16 days until new or full moon.
            var percent = Math.Round(((daysIntoCycle % 16) / 16) * 100);
            // 4 days per moon.
            var index = Convert.ToInt32(Math.Floor(daysIntoCycle / 4));
            //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs($"Moon Phase: {moons[index]} {percent}%"));
        }

        private List<TerritoryType> LoadZones()
        {
            var territoryType = _realm.RealmReversed.GameData.GetSheet("TerritoryType").ToList();
            var territoryList = new List<TerritoryType>();
            var keyList = new List<int>()
            {128,129,130,131,132,133,134,135,137,138,139,140,141,145,146,147,148,152,153,154,155,156,180,250,339,340,341,397,398,399,400,401,402,418,419,478,612,613,614,620,621,622,628,635,641,759,813,814,815,816,817,818,819,820};
            foreach (var key in keyList)
            {
                territoryList.Add((TerritoryType)territoryType.FirstOrDefault(_ => _.Key == key));
            }

            return territoryList;
        }

        private EorzeaDateTime Increment(EorzeaDateTime eorzeaDateTime)
        {
            eorzeaDateTime.Minute = 0;
            if (eorzeaDateTime.Bell < 8)
            {
                eorzeaDateTime.Bell = 8;
            }
            else if (eorzeaDateTime.Bell < 16)
            {
                eorzeaDateTime.Bell = 16;
            }
            else
            {
                eorzeaDateTime.Bell = 0;
                eorzeaDateTime.Sun++;
            }
            return eorzeaDateTime;
        }

        private double DaysIntoLunarCycle(EorzeaDateTime eDate)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var epochTimeFactor = (60.0 * 24.0) / 70.0; //20.571428571428573

            // Take an Eorzean DateTime and turn it into UTC
            var eorzeaToUTC = eDate.GetRealTime().ToUniversalTime();

            // Find total eorzian milliseconds since epoch
            var eorzeaTotalMilliseconds = eorzeaToUTC.Subtract(epoch).TotalMilliseconds * epochTimeFactor;

            // Get number of days into the cycle.
            // Moon is visible starting around 6pm. Change phase around noon when it can't be seen.
            // ((Total Eorzian Milliseconds since epoch / ([milliseconds in second] * [seconds in minute] * [minutes in hour] * [hours in day])) + mid-day) % [days in cycle(month)]
            return ((eorzeaTotalMilliseconds / (1000 * 60 * 60 * 24)) + .5) % 32;
        }
    }
}