using Microsoft.AspNetCore.Mvc;
using TaskMgt.Dtos.TaskDto;
using TaskMgt.Services.TaskService;

namespace TaskMgt.Routes
{
    public static class TaskRoute
    {
        public static void MapTaskRoutes(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/tasks", async ([FromServices] ITaskService taskService) => 
            {
                var tasks = await taskService.GetTasks();
                return Results.Ok(tasks);
            });
            routes.MapGet("/lists/{id}/tasks", async ([FromServices] ITaskService taskService, [FromRoute] string id) => 
            {
                var list = await taskService.GetTasksByListId(id);
                return Results.Ok(list);
            });

            routes.MapGet("/tasks/{id}", async ([FromServices] ITaskService taskService, [FromRoute] string id) => 
            {
                var tasks= await taskService.GetTaskById(id);
                return Results.Ok(tasks);
            });

            routes.MapPost("/tasks", async ([FromServices] ITaskService taskService, [FromBody] CreateTaskDto newTaskDto) => 
            {
                var task = await taskService.CreateTask(newTaskDto);
                return Results.Ok(task);
            });

            routes.MapPatch("/tasks/{id}", async ([FromServices] ITaskService taskService, [FromBody] UpdateTaskDto updateTaskDto, [FromRoute] string id) => 
            {
                var task = await taskService.UpdateTask(updateTaskDto, id);
                return Results.Ok(task);
            });

            routes.MapDelete("tasks/{id}", async ([FromServices] ITaskService taskService, [FromRoute] string id) =>
            {
                var task = await taskService.DeletTask(id);
                return Results.Ok(task);
            });
        }
    }
}