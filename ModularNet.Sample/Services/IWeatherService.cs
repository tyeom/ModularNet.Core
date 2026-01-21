using ModularNet.Sample.Models;

namespace ModularNet.Sample.Services;

public interface IWeatherService
{
    IEnumerable<WeatherForecast> GetForecasts(int count);
}
