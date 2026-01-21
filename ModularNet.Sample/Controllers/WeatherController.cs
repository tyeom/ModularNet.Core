using ModularNet.Core.Attributes;
using ModularNet.Core.Pipes;
using ModularNet.Sample.Interceptors;
using ModularNet.Sample.Services;

namespace ModularNet.Sample.Controllers;

[Controller("weather")]
[UseInterceptors(typeof(LoggingInterceptor), typeof(CachingInterceptor))]
public class WeatherController
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [Get]
    public IEnumerable<WeatherForecast> GetForecasts(
        [Pipe(typeof(ParseIntPipe), 5)] int count)
    {
        return _weatherService.GetForecasts(count);
    }

    [Get("{days}")]
    public IEnumerable<WeatherForecast> GetForecastsByDays(
        [Pipe(typeof(ParseIntPipe))] int days)
    {
        return _weatherService.GetForecasts(days);
    }
}
