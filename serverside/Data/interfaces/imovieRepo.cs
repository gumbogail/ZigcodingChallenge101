using serverside.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace serverside.Data.interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetPopularMoviesAsync();
        Task<IEnumerable<Movie>> SearchMoviesAsync(string query);
        Task<Movie> GetMovieByIdAsync(int id);
    }
}