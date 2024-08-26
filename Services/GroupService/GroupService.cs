using AutoMapper;
using Dtos.GroupDto;
using MongoDB.Driver;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskMgt.Dtos.GroupDto;
using TaskMgt.Models;

namespace TaskMgt.GroupService
{
    public class GroupService : IGroupService
    {
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

        public GroupService(MongoDbContext context, IMapper mapper, IDistributedCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ServiceResponse<GetGroupDto>> CreateGroup(CreateGroupDto newGroup)
        {
            var serviceResponse = new ServiceResponse<GetGroupDto>();
            try
            {
                var existingGroup = await _context.Groups
                    .Find(g => g.Name == newGroup.Name)
                    .FirstOrDefaultAsync();

                if (existingGroup != null)
                {
                    throw new InvalidOperationException("Group already created");
                }

                var group = _mapper.Map<Group>(newGroup);
                await _context.Groups.InsertOneAsync(group);

                // Invalidate cache after creating a new group
                await _cache.RemoveAsync("AllGroups");

                var createdGroup = await _context.Groups
                    .Find(g => g.Name == group.Name)
                    .FirstOrDefaultAsync() ?? throw new Exception("Group creation failed");

                var groupDto = _mapper.Map<GetGroupDto>(createdGroup);

                serviceResponse.Data = groupDto;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while creating group: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetGroupDto>> DeleteGroup(string id)
        {
            var serviceResponse = new ServiceResponse<GetGroupDto>();
            try
            {
                var group = await _context.Groups
                    .Find(g => g.Id == id)
                    .FirstOrDefaultAsync();

                if (group == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Group not found with the provided ID";
                    return serviceResponse;
                }

                await _context.Groups.DeleteOneAsync(g => g.Id == id);

                // Invalidate cache after deleting a group
                await _cache.RemoveAsync("AllGroups");
                await _cache.RemoveAsync($"Group_{id}");

                serviceResponse.Success = true;
                serviceResponse.Message = "Group deleted successfully";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while deleting the group: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<GetGroupDto>>> GetAllGroups()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetGroupDto>>();
            try
            {
                var cacheKey = "AllGroups";
                var cachedGroups = await _cache.GetStringAsync(cacheKey);

                if (string.IsNullOrEmpty(cachedGroups))
                {
                    var groups = await _context.Groups.Find(_ => true).ToListAsync();
                    var groupDtos = _mapper.Map<IEnumerable<GetGroupDto>>(groups);

                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheExpiration
                    };

                    cachedGroups = JsonSerializer.Serialize(groupDtos);
                    await _cache.SetStringAsync(cacheKey, cachedGroups, cacheOptions);
                }

                serviceResponse.Data = JsonSerializer.Deserialize<IEnumerable<GetGroupDto>>(cachedGroups);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while fetching groups: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetGroupDto>> GetGroupById(string id)
        {
            var serviceResponse = new ServiceResponse<GetGroupDto>();
            try
            {
                var cacheKey = $"Group_{id}";
                var cachedGroup = await _cache.GetStringAsync(cacheKey);

                if (string.IsNullOrEmpty(cachedGroup))
                {
                    var group = await _context.Groups
                        .Find(g => g.Id == id)
                        .FirstOrDefaultAsync();

                    if (group == null)
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = "Group not found";
                        return serviceResponse;
                    }

                    cachedGroup = JsonSerializer.Serialize(_mapper.Map<GetGroupDto>(group));
                    
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheExpiration
                    };
                    
                    await _cache.SetStringAsync(cacheKey, cachedGroup, cacheOptions);
                }

                serviceResponse.Data = JsonSerializer.Deserialize<GetGroupDto>(cachedGroup);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while fetching the group: {ex.Message}";

            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetGroupDto>> UpdateGroup(UpdateGroupDto group, string id)
        {
            var serviceResponse = new ServiceResponse<GetGroupDto>();

            try
            {
                var existingGroup = await _context.Groups
                    .Find(g => g.Id == id)
                    .FirstOrDefaultAsync();

                if (existingGroup == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Group ID provided was not found.";
                    return serviceResponse;
                }

                existingGroup.Name = group.Name ?? existingGroup.Name;
                existingGroup.Description = group.Description ?? existingGroup.Description;
                existingGroup.Update();

                await _context.Groups.ReplaceOneAsync(g => g.Id == id, existingGroup);

                // Invalidate cache after updating a group
                await _cache.RemoveAsync("AllGroups");
                await _cache.RemoveAsync($"Group_{id}");

                var updatedGroupDto = _mapper.Map<GetGroupDto>(existingGroup);
                serviceResponse.Data = updatedGroupDto;
                serviceResponse.Message = "Group updated successfully.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while updating the group: {ex.Message}";
            }

            return serviceResponse;
        }
    }
}
