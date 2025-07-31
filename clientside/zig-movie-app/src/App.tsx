import React, { useEffect, useState } from 'react';
import { Link, BrowserRouter as Router, Routes, Route, useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
 import MovieDetail from './MovieDetail';
 



interface Movie {
  id: number;
  title: string;
  overview: string;
  poster_path?: string;
  homepage?: string;
  release_date?: string;
  vote_average?: number;
}

const MoviesList: React.FC = () => {
  const [movies, setMovies] = useState<Movie[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const navigate = useNavigate();

  // Fetch popular movies on initial load
  useEffect(() => {
    const fetchPopularMovies = async () => {
      setLoading(true);
      try {
        const response = await fetch('https://localhost:5001/api/Movies/popular');

        const data = await response.json();
        if (data.success) {
          setMovies(data.data);
        } else {
          setError(data.error || 'Failed to fetch movies');
        }
      } catch (err) {
        setError('Failed to connect to the server');
      } finally {
        setLoading(false);
      }
    };

    fetchPopularMovies();
  }, []);

  // Handle search functionality
  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!searchTerm.trim()) return;

    setLoading(true);
    try {
      const response = await fetch(`https://localhost:5001/api/Movies/search?query=${encodeURIComponent(searchTerm)}`);

      const data = await response.json();
      if (data.success) {
        setMovies(data.data);
      } else {
        setError(data.error || 'No movies found');
      }
    } catch (err) {
      setError('Failed to search movies');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-4">
      <h1 className="mb-4">Popular Movies</h1>
      
      {/* Search Form */}
      <form onSubmit={handleSearch} className="mb-4">
        <div className="input-group">
          <input
            type="text"
            placeholder="Search movies by title..."
            className="form-control"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          <button className="btn btn-primary" type="submit">
            Search
          </button>
          {searchTerm && (
            <button 
              className="btn btn-outline-secondary" 
              type="button"
              onClick={() => {
                setSearchTerm('');
                navigate(0); // Refresh to show popular movies
              }}
            >
              Clear
            </button>
          )}
        </div>
      </form>

      {/* Loading and Error States */}
      {loading && (
        <div className="text-center my-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      )}

      {error && (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      )}

      {/* Movies Grid */}
      {!loading && !error && (
        <div className="row row-cols-1 row-cols-md-3 g-4">
          {movies.map((movie) => (
            <div className="col" key={movie.id}>
              <div className="card h-100">
                {movie.poster_path && (
                  <img 
                    src={`https://image.tmdb.org/t/p/w500${movie.poster_path}`} 
                    className="card-img-top" 
                    alt={movie.title}
                  />
                )}
                <div className="card-body">
                  <h5 className="card-title">
                    <Link to={`/movie/${movie.id}`} className="text-decoration-none">
                      {movie.title}
                    </Link>
                  </h5>
                  <p className="card-text">
                    {movie.overview.slice(0, 150)}...
                  </p>
                  {movie.release_date && (
                    <p className="text-muted">
                      Released: {new Date(movie.release_date).toLocaleDateString()}
                    </p>
                  )}
                  {movie.vote_average && (
                    <p className="text-muted">
                      Rating: {movie.vote_average}/10
                    </p>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Empty State */}
      {!loading && !error && movies.length === 0 && (
        <div className="alert alert-info" role="alert">
          No movies found. Try a different search.
        </div>
      )}
    </div>
  );
};

function App() {
  return (
    <Router>
      
  
        <Routes>
        <Route path="/" element={<MoviesList />} />
        <Route path="/movie/:id" element={<MovieDetail />} />
      </Routes> 
    </Router>
  );
}

export default App;