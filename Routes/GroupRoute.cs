using Dtos.GroupDto;
using Microsoft.AspNetCore.Mvc;
using TaskMgt.Dtos.GroupDto;
using TaskMgt.GroupService;

namespace TaskMgt.Routes
{
  public static class GroupRoutes
  {
    public static void MapGroupRoutes(this IEndpointRouteBuilder routes)
    {
      routes.MapGet("/groups", async ([FromServices] IGroupService groupService) => 
      {
        var groups = await groupService.GetAllGroups();
        return Results.Ok(groups);
      }); 

      routes.MapPost("/groups", async ([FromServices] IGroupService groupService, [FromBody] CreateGroupDto newGroup) => 
      {
        var result = await groupService.CreateGroup(newGroup);
        return Results.Ok(result);
      });

      routes.MapGet("/groups/{id}", async ([FromServices] IGroupService groupService, [FromRoute] string id) => 
      {
        var group = await groupService.GetGroupById(id);
        return Results.Ok(group);
      });

      routes.MapPatch("/groups/{id}", async ([FromServices] IGroupService groupService, [FromBody] UpdateGroupDto group, [FromRoute] string id) => 
      { 
        var updatedGroup = await groupService.UpdateGroup(group, id);
        return Results.Ok(updatedGroup);
      });

      routes.MapDelete("/groups/{id}", async ([FromServices] IGroupService groupService, [FromRoute] string id) => 
      {
        var deletedGroup = await groupService.DeleteGroup(id);
        return Results.Ok(deletedGroup);
      });
    }
  }
}