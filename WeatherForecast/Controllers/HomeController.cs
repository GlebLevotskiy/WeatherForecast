using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TemperatureProjectLibrary;
using WeatherForecast.Models;
using Newtonsoft.Json;

namespace WeatherForecast.Controllers
{
    public class HomeController : Controller
    {
        IConfiguration Configuration;
        int statusCode;

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View(GetCityInfo("Minsk"));
        }

        public IActionResult GetCityTemperature(string cityName = "Minsk")
        {
            try
            {
                return View("Index", GetCityInfo(cityName));
            }
            catch (HttpRequestException ex)
            {
                return View("ConnectionError", new ErrorModel {StatusCode = statusCode, Message = ex.Message});
            }
            //If AggregateException
            catch
            {
                return View("ConnectionError", new ErrorModel { StatusCode = 404, Message = "Something go wrong"});
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private TemperatureInfo GetCityInfo(string city)
        {
            try
            {
                var temperature = new TemperatureInfo
                {
                    DaySnapshots = CallWeekTemperatureApiAsync(city).Result,
                    Snapshot = CallTemperatureApiAsync(city).Result
                };
                return temperature;
            }
            catch (HttpRequestException ex)
            {
                throw ex;
            }
            catch
            {
                throw;
            }
        }

        private async Task<IList<DaySnapshot>> CallWeekTemperatureApiAsync(string city)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Configuration.GetConnectionString("WeekCityWeatherURL")}{city}")
            };

            using (var response = await client.SendAsync(request))
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    var weatherForecast = System.Text.Json.JsonSerializer.Deserialize<List<DaySnapshot>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return weatherForecast;
                }
                catch (HttpRequestException ex)
                {
                    statusCode = (int)response.StatusCode;
                    throw ex;
                }
                catch
                {
                    throw;
                }
            }
        }

        private async Task<TemperatureSnapshot> CallTemperatureApiAsync(string city)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Configuration.GetConnectionString("CityWeatherURL")}{city}")
            };

            using (var response = await client.SendAsync(request))
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    var weatherForecast = System.Text.Json.JsonSerializer.Deserialize<TemperatureSnapshot>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return weatherForecast ?? throw new ArgumentNullException();
                }
                catch (HttpRequestException ex)
                {
                    statusCode = (int)response.StatusCode;
                    throw ex;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}