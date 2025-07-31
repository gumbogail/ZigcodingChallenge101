using System;
using System.Collections.Generic;

namespace serverside.Data.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string Homepage { get; set; }
        public double Popularity { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    // For TMDB API response structure
    public class TmdbResponse
    {
        public List<TmdbMovie> Results { get; set; }
    }

    public class TmdbMovie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string Homepage { get; set; }
        public double Popularity { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}