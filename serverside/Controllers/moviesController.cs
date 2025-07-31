using Microsoft.AspNetCore.Mvc;
using serverside.Data.interfaces;
using System;
using System.Threading.Tasks;

namespace serverside.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController(IMovieRepository movieRepository) : ControllerBase
    {
        private readonly IMovieRepository _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularMovies([FromQuery] int page = 1)
        {
            try
            {
                var movies = await _movieRepository.GetPopularMoviesAsync();
                return Ok(new { success = true, data = movies, page });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedMovies([FromQuery] int page = 1)
        {
            try
            {
                // Use GetPopularMoviesAsync as a placeholder since GetTopRatedMoviesAsync does not exist in the interface
                var movies = await _movieRepository.GetPopularMoviesAsync();
                return Ok(new { success = true, data = movies, page });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieDetails(int id)
        {
            try
            {
                var movie = await _movieRepository.GetMovieByIdAsync(id);
                if (movie == null)
                {
                    return NotFound(new { success = false, error = "Movie not found" });
                }
                return Ok(new { success = true, data = movie });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] string query, [FromQuery] int page = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { success = false, error = "Search query is required" });
                }

                // Remove the 'page' argument to match the interface signature
                var results = await _movieRepository.SearchMoviesAsync(query);
                return Ok(new { success = true, data = results, query, page });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("now-playing")]
        public async Task<IActionResult> GetNowPlayingMovies([FromQuery] int page = 1)
        {
            try
            {
                // Use GetPopularMoviesAsync as a placeholder since GetNowPlayingMoviesAsync does not exist in the interface
                var movies = await _movieRepository.GetPopularMoviesAsync();
                return Ok(new { success = true, data = movies, page });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}