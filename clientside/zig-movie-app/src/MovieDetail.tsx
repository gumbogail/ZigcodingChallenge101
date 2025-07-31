import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

interface MovieDetails {
  id: number;
  title: string;
  overview: string;
  poster_path: string;
  homepage: string;
  release_date: string;
  vote_average: number;
  runtime: number;
  genres: { id: number; name: string }[];
}

const MovieDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [movie, setMovie] = useState<MovieDetails | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchMovieDetails = async () => {
      try {
        const response = await fetch(`https://localhost:5001/api/Movies/${id}`);
        const data = await response.json();
        if (data.success) {
          setMovie(data.data);
        } else {
          setError(data.error || 'Movie not found');
        }
      } catch (err) {
        setError('Failed to fetch movie details');
      } finally {
        setLoading(false);
      }
    };

    fetchMovieDetails();
  }, [id]);

  if (loading) {
    return (
      <div className="text-center my-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="alert alert-danger" role="alert">
        {error}
      </div>
    );
  }

  if (!movie) {
    return <div>No movie data available</div>;
  }

  return (
    <div className="container mt-4">
      <div className="row">
        <div className="col-md-4">
          <img 
            src={`https://image.tmdb.org/t/p/w500${movie.poster_path}`} 
            className="img-fluid rounded" 
            alt={movie.title}
          />
        </div>
        <div className="col-md-8">
          <h1>{movie.title}</h1>
          
          {movie.homepage && (
            <p>
              <a href={movie.homepage} target="_blank" rel="noopener noreferrer">
                Official Website
              </a>
            </p>
          )}

          <p className="lead">{movie.overview}</p>
          
          <ul className="list-unstyled">
            <li>
              <strong>Release Date:</strong> {new Date(movie.release_date).toLocaleDateString()}
            </li>
            <li>
              <strong>Rating:</strong> {movie.vote_average}/10
            </li>
            {movie.runtime && (
              <li>
                <strong>Runtime:</strong> {Math.floor(movie.runtime / 60)}h {movie.runtime % 60}m
              </li>
            )}
            {movie.genres && movie.genres.length > 0 && (
              <li>
                <strong>Genres:</strong> {movie.genres.map(g => g.name).join(', ')}
              </li>
            )}
          </ul>

          <Link to="/" className="btn btn-primary mt-3">
            Back to Movies
          </Link>
        </div>
      </div>
    </div>
  );
};

export default MovieDetail;