using TaskMgt.Services;

namespace TaskMgt.Routes
{
  public static class BookRoutes
  {
    public static void MapBookRoutes(this IEndpointRouteBuilder routes)
    {
      routes.MapGet("/books", (BookService bookService) => 
      {
        return Results.Ok(bookService.GetBooks());
      });
    }
  }
}