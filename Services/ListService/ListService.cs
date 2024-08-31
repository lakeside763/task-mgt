using AutoMapper;
using MongoDB.Driver;
using TaskMgt.Dtos.ListDto;
using TaskMgt.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Dtos.GroupDto;

namespace TaskMgt.Services.ListService
{
    public class ListService : IListService
    {
        private readonly MongoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

        public ListService(MongoDbContext context, IMapper mapper, IDistributedCache cache)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ServiceResponse<GetListDto>> CreateList(CreateListDto newList)
        {
            var serviceResponse = new ServiceResponse<GetListDto>();
            try
            {
                var existingList = await _context.Lists
                    .Find(l => l.Name == newList.Name && l.GroupId == newList.GroupId)
                    .FirstOrDefaultAsync();

                if (existingList != null)
                {
                    throw new InvalidOperationException("List already created");
                }

                var list = _mapper.Map<List>(newList);
                await _context.Lists.InsertOneAsync(list);

                var createdList = await _context.Lists
                    .Find(l => l.Name == list.Name && l.GroupId == list.GroupId)
                    .FirstOrDefaultAsync();

                var listDto = _mapper.Map<GetListDto>(createdList);

                // Invalidate cache for the group lists
                await _cache.RemoveAsync($"GroupLists_{list.GroupId}");
                await _cache.RemoveAsync("AllListsWithGroup");

                serviceResponse.Data = listDto;
                serviceResponse.Message = "List created successfully";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while creating list: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetListDto>> DeleteList(string id)
        {
            var serviceResponse = new ServiceResponse<GetListDto>();
            try
            {
                var list = await _context.Lists
                    .Find(l => l.Id == id)
                    .FirstOrDefaultAsync();

                if (list == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "List not found with the provided ID";
                    return serviceResponse;
                }

                await _context.Lists.DeleteOneAsync(l => l.Id == id);

                // Invalidate cache for the group lists
                await _cache.RemoveAsync($"GroupLists_{list.GroupId}");
                await _cache.RemoveAsync("AllListsWithGroup");

                serviceResponse.Success = true;
                serviceResponse.Message = "List deleted successfully";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while deleting the list: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetGroupListsDto>> GetListByGroupId(string groupId)
        {
            var serviceResponse = new ServiceResponse<GetGroupListsDto>();
            try
            {
                // Check cache first
                var cacheKey = $"GroupLists_{groupId}";
                var cachedData = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    serviceResponse.Data = JsonSerializer.Deserialize<GetGroupListsDto>(cachedData);
                    return serviceResponse;
                }

                var group = await _context.Groups.Find(g => g.Id == groupId).FirstOrDefaultAsync();
                if (group == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "List Group ID provided was not found";
                    return serviceResponse;
                }
                var lists = await _context.Lists.Find(l => l.GroupId == groupId).ToListAsync();

                var groupList = _mapper.Map<GetGroupListsDto>(group);
                groupList.Lists = _mapper.Map<List<GetListDto>>(lists);

                // Cache the result
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(groupList), options);

                serviceResponse.Data = groupList;
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while fetching group lists: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetListDto>> GetListById(string id)
        {
            var serviceResponse = new ServiceResponse<GetListDto>();
            try
            {
                // Check cache first
                var cacheKey = $"List_{id}";
                var cachedData = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    serviceResponse.Data = JsonSerializer.Deserialize<GetListDto>(cachedData);
                    return serviceResponse;
                }

                var list = await _context.Lists
                    .Find(l => l.Id == id)
                    .FirstOrDefaultAsync();

                if (list == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "List not found";
                    return serviceResponse;
                }

                var listDto = _mapper.Map<GetListDto>(list);

                // Cache the result
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(listDto), options);

                serviceResponse.Data = listDto;
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while fetching the list: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetListDto>> UpdateList(UpdateListDto list, string id)
        {
            var serviceResponse = new ServiceResponse<GetListDto>();
            try
            {
                var existingList = await _context.Lists
                    .Find(l => l.Id == id)
                    .FirstOrDefaultAsync();

                if (existingList == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "List ID provided was not found.";
                    return serviceResponse;
                }

                existingList.Name = list.Name ?? existingList.Name;
                existingList.Description = list.Description ?? existingList.Description;
                existingList.Update();

                await _context.Lists.ReplaceOneAsync(l => l.Id == id, existingList);

                var updatedListDto = _mapper.Map<GetListDto>(existingList);

                // Invalidate cache for this list and group lists
                await _cache.RemoveAsync($"List_{id}");
                await _cache.RemoveAsync($"GroupLists_{existingList.GroupId}");
                await _cache.RemoveAsync("AllListsWithGroup");

                serviceResponse.Data = updatedListDto;
                serviceResponse.Message = "List updated successfully";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occurred while updating the list: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<GetListWithGroupDto>>> GetLists()
        {
            var serviceResponse = new ServiceResponse<IEnumerable<GetListWithGroupDto>>();
            try
            {
                var cacheKey = "AllListsWithGroup";
                var cachedListsWithGroup = await _cache.GetStringAsync(cacheKey);

                if (string.IsNullOrEmpty(cachedListsWithGroup))
                {
                    var lists = await _context.Lists.Find(_ => true).ToListAsync();

                    // Fetch group details in parallel
                    var listWithGroupTasks = lists.Select(async list => 
                    {
                        var listWithGroupDto = _mapper.Map<GetListWithGroupDto>(list);

                        if (!string.IsNullOrEmpty(list.GroupId))
                        {
                            var group = await _context.Groups
                                .Find(g => g.Id == list.GroupId)
                                .FirstOrDefaultAsync();
                            listWithGroupDto.Group = _mapper.Map<GetGroupDto>(group);
                        }
                        return listWithGroupDto;
                    });
                    var listWithGroup = await Task.WhenAll(listWithGroupTasks);

                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheExpiration
                    };
                    cachedListsWithGroup = JsonSerializer.Serialize(listWithGroup);
                    await _cache.SetStringAsync(cacheKey, cachedListsWithGroup, cacheOptions);
                }
                serviceResponse.Data = JsonSerializer.Deserialize<IEnumerable<GetListWithGroupDto>>(cachedListsWithGroup);
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error occured while fetching lists: {ex.Message}";
            }
            return serviceResponse;
        }
    }
}
