// using Microsoft.AspNetCore.Mvc;
// using System.Collections.Generic;
// using System.Linq; 
// namespace MyFirstApi.Controllers
// {
    
//     public class WeatherForecast
//     {
//         public int Id { get; set; } 
//         public DateTime Date { get; set; }
//         public int TemperatureC { get; set; }
//         public string? Summary { get; set; }
//     }

//     [ApiController]
//     [Route("[controller]")]
//     public class WeatherForecastController : ControllerBase
//     {
//         private static readonly List<WeatherForecast> _forecasts = new List<WeatherForecast>
//         {
//             new WeatherForecast { Id = 1, Date = DateTime.Now.AddDays(1), TemperatureC = 25, Summary = "Mild" },
//             new WeatherForecast { Id = 2, Date = DateTime.Now.AddDays(2), TemperatureC = 30, Summary = "Warm" }
//         };

//         private static readonly string[] Summaries = new[]
//         {
//             "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//         };

//         [HttpGet]
//         public IEnumerable<WeatherForecast> Get()
//         {
//            return _forecasts;
//         }

//         [HttpPost]
//         public IActionResult Post([FromBody] WeatherForecast forecast)
//         {
          
//             forecast.Id = _forecasts.Any() ? _forecasts.Max(f => f.Id) + 1 : 1;
//             _forecasts.Add(forecast);
            
//             return CreatedAtAction(nameof(Get), new { id = forecast.Id }, forecast);
//         }
//         [HttpPut("{id}")]
//         public IActionResult Put(int id, [FromBody] WeatherForecast forecast)
//         { 
          
//             var existingForecast = _forecasts.FirstOrDefault(f => f.Id == id);

         
//             if (existingForecast == null)
//             {
//                 return NotFound();  
//             }

//             existingForecast.Date = forecast.Date;
//             existingForecast.TemperatureC = forecast.TemperatureC; 
//             existingForecast.Summary = forecast.Summary;
//             return NoContent();
//         }

//         [HttpDelete("{id}")]
//         public IActionResult Delete(int id)
//         { 
//            var forecastToDelete = _forecasts.FirstOrDefault(f => f.Id == id);

//            if (forecastToDelete == null)
//            {
//                return NotFound();
//            }

//            _forecasts.Remove(forecastToDelete);

//            return NoContent();
//         }
//     }
// }