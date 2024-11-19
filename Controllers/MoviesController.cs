using Microsoft.AspNetCore.Mvc;
using MovieAPI.Model;
using MovieAPI.DatabaseContext;
using System.Linq.Expressions;

namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("movies")]

    public class MoviesController(AppDbContext context) : ControllerBase
    {
        
        [HttpGet]
        public IEnumerable<Movies> Get([FromQuery] QueryBody<Movies> body)
        {
            return [.. context.Movies.Where(body.ToPredicate())];
        }

        // Lager Overloads av Get ved å introdusere varianter med parametere fra route. 
        [HttpGet("{id}")]
        public IEnumerable<Movies> Get(int id)
        {
            return [.. context.Movies.Where(m => m.Id == id)];
        }

        [HttpPost]
        public IActionResult Post([FromBody] Movies movies)
        {
            if (movies == null)
            {
                return BadRequest("Movie is null!");
            }

            context.Add(movies);
            context.SaveChanges();
            return CreatedAtAction(nameof(Post), new { id = movies.Id, title = movies.Title, director = movies.Director, releaseYear = movies.ReleaseYear }, movies);
        }
    }
}