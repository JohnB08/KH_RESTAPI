
using MovieAPI.DatabaseContext;
using MovieAPI.Model;

namespace MovieAPI.Util;

public static class SeedTvShows
{
    private static readonly List<TVShows> _tvShows = [
            new TVShows { Id = 1, Title = "The Sopranos", Creator = "David Chase", Director = "David Chase", ReleaseYear = 1999, NumberOfSeasons = 6},
            new TVShows { Id = 2, Title = "Breaking Bad", Creator = "Vince Gilligan", Director = "Vince Gilligan", ReleaseYear = 2008, NumberOfSeasons = 5},
            new TVShows {Id = 3, Title = "Game of Thrones", Creator = "David Benioff & D.B Weiss", Director = "Timothy Van Patten", ReleaseYear = 2011, NumberOfSeasons = 8},
            new TVShows {Id = 4, Title = "Twin Peaks", Creator = "David Lynch & Mark Frost", Director = "David Lynch", ReleaseYear = 1990, NumberOfSeasons = 2},
            new TVShows {Id = 5, Title = "The Wire", Creator = "David Simon", Director = "David Simon", ReleaseYear = 2002, NumberOfSeasons = 5}
        ];

    public static void SeedData(AppDbContext context)
    {
        if (context.TVShows.Any()) return;
        context.TVShows.AddRange(_tvShows);
        context.SaveChanges();
    }

}
