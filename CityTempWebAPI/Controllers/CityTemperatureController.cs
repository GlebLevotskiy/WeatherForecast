using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TemperatureProjectLibrary;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.Json;

namespace CityTempWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityTemperatureController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public CityTemperatureController(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new HttpClient();
        }


        [HttpGet("GetCityWeekTemperature/{cityName}")]
        public async Task<ActionResult<string>> GetCityWeekTemperature(string cityName)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_configuration.GetConnectionString("WeekCityWeatherURL")}{HttpUtility.UrlEncode(cityName)}"),
                Headers =
                {
                    { "x-rapidapi-key", "a82516bbcemsh6f2d7a955d1b58ep1bd3c1jsnf280cc956ebd" },
                    { "x-rapidapi-host", "community-open-weather-map.p.rapidapi.com" },
                },
            };

            using (var response = await _client.SendAsync(request))
            {
                try
                {
                    response.EnsureSuccessStatusCode();

                    JObject body = JObject.Parse(await response.Content.ReadAsStringAsync());
                    JArray list = (JArray)body["list"];
                    JToken city = body["city"];


                    return JsonSerializer.Serialize(Enumerable.Range(0, 5).Select(i =>
                    {
                        return new DaySnapshot
                        {
                            Snapshots = Enumerable.Range(0, 8).Select(k =>
                            {
                                JToken tempSnap = list[i * 8 + k];
                                return new TemperatureSnapshot
                                {
                                    TempValue = tempSnap["main"]["temp"].Value<float>(),
                                    Humidity = tempSnap["main"]["humidity"].Value<float>(),
                                    Pressure = tempSnap["main"]["pressure"].Value<float>(),
                                    Clouds = tempSnap["clouds"]["all"].Value<int>(),
                                    WindSpeed = tempSnap["wind"]["speed"].Value<float>(),
                                    Time = DateTime.ParseExact(tempSnap["dt_txt"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                    Icon = tempSnap["weather"][0]["icon"].ToString()
                                };
                            }),
                            City = city["name"].ToString(),
                            Region = city["country"].ToString(),
                            Date = DateTime.ParseExact(list[i * 8]["dt_txt"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                        };
                    }));
                }
                catch
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
        }


        [HttpGet("GetCityTemperature/{cityName}")]
        public async Task<ActionResult<string>> GetCityTemperature(string cityName)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_configuration.GetConnectionString("CityWeatherURL")}{HttpUtility.UrlEncode(cityName)}"),
                Headers =
                {
                    { "x-rapidapi-key", "a82516bbcemsh6f2d7a955d1b58ep1bd3c1jsnf280cc956ebd" },
                    { "x-rapidapi-host", "community-open-weather-map.p.rapidapi.com" },
                },
            };
            using (var response = await _client.SendAsync(request))
            {
                try
                {
                    response.EnsureSuccessStatusCode();

                    JObject body = JObject.Parse(await response.Content.ReadAsStringAsync());
                    JToken weather = body["weather"][0];
                    JToken main = body["main"];
                    JToken clouds = body["clouds"];

                    return JsonSerializer.Serialize(new TemperatureSnapshot
                    {
                        TempValue = main["temp"].Value<float>(),
                        Humidity = main["humidity"].Value<float>(),
                        Pressure = main["pressure"].Value<float>(),
                        Clouds = clouds["all"].Value<int>(),
                        City = body["name"].ToString(),
                        Region = body["sys"]["country"].ToString(),
                        WindSpeed = body["wind"]["speed"].Value<float>(),
                        Icon = weather["icon"].ToString()
                    });
                }
                catch
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
        }
    }
}
