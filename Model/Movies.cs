using MovieAPI.DatabaseContext;

namespace MovieAPI.Model;

public class Movies
{
    public int Id { get; set; }
    public string? Title { get; set; }

    public string? Director { get; set; }

    public int ReleaseYear { get; set; }

}


