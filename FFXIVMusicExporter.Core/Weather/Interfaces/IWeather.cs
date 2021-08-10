using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using SaintCoinach;

namespace FFXIVMusicExporter.Core.Weather
{
    public interface IWeather
    {
        void GetMoonPhase(EorzeaDateTime eDate);
        List<string> GetTerritoryPlaceNames();
        Task GetWeatherAsync(DateTime dateTime, IEnumerable<string> zones, int forcastIntervals, CancellationToken cancellationToken);
    }
}