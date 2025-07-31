using Microsoft.Extensions.Configuration;
using serverside.Data.interfaces;
using serverside.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace serverside.Data.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public MovieRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = configuration["TmdbApiSettings:ApiKey"];
            _baseUrl = configuration["TmdbApiSettings:BaseUrl"];
        }

        public async Task<IEnumerable<Movie>> GetPopularMoviesAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}movie/popular?api_key={_apiKey}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<TmdbResponse>();
            return content?.Results.Select(m => MapToMovie(m)).Take(20);
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string query)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}search/movie?api_key={_apiKey}&query={query}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<TmdbResponse>();
            return content?.Results.Select(MapToMovie);
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_baseUrl}movie/{id}?api_key={_apiKey}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<TmdbMovie>();
            return MapToMovie(content);
        }

        private Movie MapToMovie(TmdbMovie tmdbMovie)
        {
            return new Movie
            {
                Id = tmdbMovie.Id,
                Title = tmdbMovie.Title,
                Overview = tmdbMovie.Overview,
                PosterPath = tmdbMovie.PosterPath,
                Homepage = tmdbMovie.Homepage,
                Popularity = tmdbMovie.Popularity,
                ReleaseDate = tmdbMovie.ReleaseDate
            };
        }
    }
}