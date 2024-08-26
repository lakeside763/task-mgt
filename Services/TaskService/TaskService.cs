using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using TaskMgt.Dtos.TaskDto;
using TaskMgt.Models;

namespace TaskMgt.Services.TaskService
{
    public class TaskService : ITaskService
    {
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

        public TaskService(MongoDbContext context, IMapper mapper, IDistributedCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<ServiceResponse<GetTaskDto>> CreateTask(CreateTaskDto newTask)
        {
            var serviceResponse = new ServiceResponse<GetTaskDto>();
            try
            {
                var existingTask = await _context.Tasks
                    .Find(t => t.Name == newTask.Name && t.ListId == newTask.ListId)
                    .FirstOrDefaultAsync();

                if (existingTask != null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Task already created";
                    return serviceResponse;
                }
                var task = _mapper.Map<TodoTask>(newTask);
                await _context.Tasks.InsertOneAsync(task);

                var createdTask = await _context.Tasks
                    .Find(t => t.Name == task.Name && t.ListId == task.ListId)
                    .FirstOrDefaultAsync();

                var taskDto = _mapper.Map<GetTaskDto>(createdTask);

                // Invalidate cache for the list tasks
                await _cache.RemoveAsync($"ListTasks_{task.ListId}");

                serviceResponse.Data = taskDto;
                serviceResponse.Message = "Task created successfully";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occured whild creating list: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetTaskDto>> DeletTask(string id)
        {
            var serviceResponse = new ServiceResponse<GetTaskDto>();
            try
            {
                var task = await _context.Tasks
                    .Find(t => t.Id == id)
                    .FirstOrDefaultAsync();
                
                if (task == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Task not found with the provided ID";
                    return serviceResponse;
                }
                await _context.Tasks.DeleteOneAsync(t => t.Id == id);

                // Invalidate cache for the list tasks
                await _cache.RemoveAsync($"ListTasks_{task.ListId}");

                serviceResponse.Success = true;
                serviceResponse.Message = "List deleted successfully";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occured while deleting the task: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetTaskDto>> GetTaskById(string id)
        {
            var serviceResponse = new ServiceResponse<GetTaskDto>();
            try
            {
                // check cache first
                var cacheKey = $"Task_{id}";
                var cacheData = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cacheData))
                {
                    serviceResponse.Data = JsonSerializer.Deserialize<GetTaskDto>(cacheData);
                    return serviceResponse;
                }

                var task = await _context.Tasks
                    .Find(t => t.Id == id)
                    .FirstOrDefaultAsync();

                if (task == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Task not found";
                    return serviceResponse;
                }

                var taskDto = _mapper.Map<GetTaskDto>(task);

                // Cache the result
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(taskDto), options);

                serviceResponse.Data = taskDto;
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while fetching the task: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetListTasksDto>> GetTasksByListId(string listId)
        {
            var serviceResponse = new ServiceResponse<GetListTasksDto>();
            try
            {
                // Check cache first
                var cacheKey = $"ListTasks_{listId}";
                var cacheData = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cacheData))
                {
                    serviceResponse.Data = JsonSerializer.Deserialize<GetListTasksDto>(cacheData);
                    return serviceResponse;
                }

                var list = await _context.Lists.Find(l => l.Id == listId).FirstOrDefaultAsync();
                if (list == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Task List ID provided was not found";
                    return serviceResponse;
                }
                var tasks = await _context.Tasks.Find(t => t.ListId == listId).ToListAsync();
                var listTasks = _mapper.Map<GetListTasksDto>(list);
                listTasks.Tasks = _mapper.Map<List<GetTaskDto>>(tasks);

                // Cache the result;
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration
                };

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(listTasks), options);

                serviceResponse.Data = listTasks;
                return serviceResponse;
            }
            catch(Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while fetching group lists: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetTaskDto>> UpdateTask(UpdateTaskDto task, string id)
        {
            var serviceResponse = new ServiceResponse<GetTaskDto>();
            try
            {
                var existingTask = await _context.Tasks
                    .Find(t => t.Id == id)
                    .FirstOrDefaultAsync();
                
                if (existingTask == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Task ID provided was not found.";
                    return serviceResponse;
                }
                existingTask.Name = task.Name ?? existingTask.Name;
                existingTask.Description = task.Description ?? existingTask.Description;
                existingTask.Update();

                await _context.Tasks.ReplaceOneAsync(t => t.Id == id, existingTask);

                var updatedTaskDto = _mapper.Map<GetTaskDto>(existingTask);

                // Invalidate cache for this task and list tasks
                await _cache.RemoveAsync($"Task_{id}");
                await _cache.RemoveAsync($"ListTasks_{existingTask.ListId}");

                serviceResponse.Data = updatedTaskDto;
                serviceResponse.Message = "Task updated successfully";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while updating the task: {ex.Message}";
            }
            return serviceResponse;
        }
    }
}