using System.Collections.Generic;
using TaskMgt.Dtos.TaskDto;
using TaskMgt.Models;

namespace TaskMgt.Services.TaskService
{
    public interface ITaskService
    {
        Task<ServiceResponse<GetListTasksDto>> GetTasksByListId(string listId);

        Task<ServiceResponse<GetTaskDto>> GetTaskById(string id);

        Task<ServiceResponse<GetTaskDto>> CreateTask(CreateTaskDto task);

        Task<ServiceResponse<GetTaskDto>> UpdateTask(UpdateTaskDto task, string id);

        Task<ServiceResponse<GetTaskDto>> DeletTask(string id);

        Task<ServiceResponse<IEnumerable<GetTaskDto>>> GetTasks();
    }
}