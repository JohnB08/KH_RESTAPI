using Microsoft.AspNetCore.Mvc;
using MovieAPI.DatabaseContext;
using MovieAPI.Model;

namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("tvshows")]
    public class TVShowsController(AppDbContext context) : ControllerBase
    {


        [HttpGet]
        public IEnumerable<TVShows> Get([FromQuery] QueryBody body)
        {
            return [.. context.TVShows.Where(body.ToPredicate<TVShows>())];
        }

        // Lager Overloads av Get ved å introdusere varianter med parametere fra route. 
        [HttpGet("{id}")]
        public IEnumerable<TVShows> Get(int id)
        {
            return [.. context.TVShows.Where(t => t.Id == id)];
        }

        [HttpPost]
        public IActionResult Post([FromBody] TVShows tvShows)
        {
            context.TVShows.Add(tvShows);
            context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = tvShows.Id, title = tvShows.Title, creator = tvShows.Creator, director = tvShows.Director, releaseYear = tvShows.ReleaseYear, numberOfSeasons = tvShows.NumberOfSeasons }, tvShows);
        }

    }
}