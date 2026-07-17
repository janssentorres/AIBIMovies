
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Controllers;

public class MoviesController : Controller
{
    private readonly MvcMovieContext _context;

    public MoviesController(MvcMovieContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? movieGenre, string? searchString, DateTime? releaseDateFrom, DateTime? releaseDateTo,
        string sortBy = "Title", string sortDirection = "asc", int pageNumber = 1, int pageSize = 10)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 10, 200);

        var genreQuery = _context.Movie
            .Select(movie => movie.Genre)
            .Distinct()
            .OrderBy(genre => genre);

        var movies = _context.Movie.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            movies = movies.Where(movie => movie.Title.Contains(searchString));
        }

        if (!string.IsNullOrWhiteSpace(movieGenre))
        {
            movies = movies.Where(movie =>
                movie.Genre == movieGenre);
        }

        if (releaseDateFrom.HasValue)
        {
            movies = movies.Where(movie =>
                movie.ReleaseDate >= releaseDateFrom.Value);
        }

        if (releaseDateTo.HasValue)
        {
            movies = movies.Where(movie =>
                movie.ReleaseDate <= releaseDateTo.Value);
        }

        movies = sortBy switch
        {
            "Genre" when sortDirection == "desc" =>
                movies.OrderByDescending(movie => movie.Genre).ThenBy(movie => movie.Id),

            "Genre" => movies.OrderBy(movie => movie.Genre).ThenBy(movie => movie.Id),

            "ReleaseDate" when sortDirection == "desc" =>
                movies.OrderByDescending(movie => movie.ReleaseDate).ThenBy(movie => movie.Id),

            "ReleaseDate" =>
                movies.OrderBy(movie => movie.ReleaseDate).ThenBy(movie => movie.Id),

            "Title" when sortDirection == "desc" =>
                movies.OrderByDescending(movie => movie.Title).ThenBy(movie => movie.Id),

            _ =>
                movies.OrderBy(movie => movie.Title).ThenBy(movie => movie.Id)
        };

        var totalRecords = await movies.CountAsync();

        var totalPages = (int)Math.Ceiling(
            totalRecords / (double)pageSize);

        if (totalPages > 0)
        {
            pageNumber = Math.Min(pageNumber, totalPages);
        }
        else
        {
            pageNumber = 1;
        }

        var movieList = await movies
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var genres = await genreQuery.ToListAsync();

        var viewModel = new MovieGenreViewModel
        {
            Movies = movieList,
            Genres = new SelectList(genres, selectedValue: movieGenre),
            MovieGenre = movieGenre,
            SearchString = searchString,
            ReleaseDateFrom = releaseDateFrom,
            ReleaseDateTo = releaseDateTo,
            SortBy = sortBy,
            SortDirection = sortDirection,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };

        return View(viewModel);
    }

    // GET: MOVIES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // GET: MOVIES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MOVIES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
    {
        if (ModelState.IsValid)
        {
            _context.Add(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(movie);
    }

    // GET: MOVIES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }

    // POST: MOVIES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(movie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(movie.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(movie);
    }

    // GET: MOVIES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: MOVIES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var movie = await _context.Movie.FindAsync(id);
        if (movie != null)
        {
            _context.Movie.Remove(movie);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MovieExists(int? id)
    {
        return _context.Movie.Any(e => e.Id == id);
    }
}
