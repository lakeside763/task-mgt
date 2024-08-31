using Microsoft.AspNetCore.Mvc;
using TaskMgt.Dtos.ListDto;
using TaskMgt.Services.ListService;

namespace TaskMgt.Routes
{
    public static class ListRoute
    {
        public static void MapListRoutes(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/groups/{id}/lists", async ([FromServices] IListService listService, [FromRoute] string id) => 
            {
                var group = await listService.GetListByGroupId(id);
                return Results.Ok(group);
            });

            routes.MapGet("/lists/{id}", async ([FromServices] IListService listService, [FromRoute] string id) => 
            {
                var lists = await listService.GetListById(id);
                return Results.Ok(lists);
            });

            routes.MapPost("/lists", async ([FromServices] IListService listService, [FromBody] CreateListDto newListDto) => 
            {
                var list = await listService.CreateList(newListDto);
                return Results.Ok(list);
            });

            routes.MapPatch("/lists/{id}", async ([FromServices] IListService listService, [FromBody] UpdateListDto updateListDto, [FromRoute] string id) => 
            {
                var list = await listService.UpdateList(updateListDto, id);
                return Results.Ok(list);
            });

            routes.MapDelete("lists/{id}", async ([FromServices] IListService listService, [FromRoute] string id) =>
            {
                var list = await listService.DeleteList(id);
                return Results.Ok(list);
            });

            routes.MapGet("lists", async ([FromServices] IListService listService) => 
            {
                var lists = await listService.GetLists();
                return Results.Ok(lists);
            });
        } 
    }
}