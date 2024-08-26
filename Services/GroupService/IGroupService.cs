using Dtos.GroupDto;
using TaskMgt.Dtos.GroupDto;
using TaskMgt.Models;

namespace TaskMgt.GroupService
{
  public interface IGroupService
  {
    Task<ServiceResponse<IEnumerable<GetGroupDto>>> GetAllGroups();
    Task<ServiceResponse<GetGroupDto>> GetGroupById(string id);
    Task<ServiceResponse<GetGroupDto>> CreateGroup(CreateGroupDto group);
    Task<ServiceResponse<GetGroupDto>> UpdateGroup(UpdateGroupDto group, string id);
    Task<ServiceResponse<GetGroupDto>> DeleteGroup(string id);
  }
}