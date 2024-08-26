using AutoMapper;
using Dtos.GroupDto;
using TaskMgt.Dtos.ListDto;
using TaskMgt.Dtos.TaskDto;
using TaskMgt.Models;

namespace TaskMgt
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Group, GetGroupDto>();
            CreateMap<CreateGroupDto, Group>();
            CreateMap<List, GetListDto>();
            CreateMap<CreateListDto, List>();
            CreateMap<Group, GetGroupListsDto>();
            CreateMap<TodoTask, GetTaskDto>();
            CreateMap<CreateTaskDto, TodoTask>();
            CreateMap<List, GetListTasksDto>();
        }
    }
}