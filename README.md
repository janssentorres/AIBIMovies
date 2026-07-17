# AIBIMovies
AIBI ASP .Net Core MVC Technical Test for Developers

This project is based on the Microsoft Learn ASP.NET Core MVC Movies tutorial.

The tutorial project was extended with:
- Database seeding for 1,000 movies
- Server-side filtering
- Server-side sorting
- Server-side paging
- Filtering by Title, Genre, and Release Date
- Sorting by Title, Genre, and Release Date
- Selectable page size

## Prerequisites

Before running the project, install:
- Visual Studio with .NET 10 support
- .NET 10 SDK
- SQL Server Express LocalDB
- Git

In Visual Studio Installer, include:
- ASP.NET and web development
- Data storage and processing

## Repository

https://github.com/janssentorres/AIBIMovies

## Technologies Used
- ASP.NET Core MVC
- .NET 10
- Entity Framework Core
- SQL Server LocalDB
- Bootstrap
- Visual Studio

## Main Features

### Movies Index Page

The Movies Index page supports:
- Title search
- Genre filter
- Release Date From filter
- Release Date To filter
- Sorting by Title
- Sorting by Genre
- Sorting by Release Date
- Server-side pagination
- Page size selection

The filtering, sorting, and paging are applied to the Entity Framework query before the records are loaded.

This means SQL Server only returns the records needed for the current page.

### Database Seeding

The application uses a `MovieSeedService`.

When the application starts, the service checks how many movies already exist in the database.

If the database contains fewer than 1,000 movies, the service creates the missing records.

This prevents duplicate records from being added every time the application starts.

## Clone and Run

1. Open Visual Studio.
2. Select **Clone a repository**.
3. Enter:https://github.com/janssentorres/AIBIMovies.git
4. Choose a local folder.
5. Select Clone.
6. Open MvcMovie.slnx if Visual Studio does not open it automatically.
7. In Solution Explorer, right-click the MvcMovie project.
8. Select Set as Startup Project.
9. Restore NuGet packages if needed.
10. Select Build > Build Solution.
11. Press Ctrl + F5 to run the project.

On first startup, the application will:
- Apply the Entity Framework Core migrations
- Create the SQL Server LocalDB database
- Seed the database until it contains 1,000 movies
- Open the application in the browser

Open the Movies page by adding /Movies to the current URL.
Example:
https://localhost:7200/Movies
The port may be different on another machine.