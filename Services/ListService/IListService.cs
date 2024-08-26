using TaskMgt.Dtos.ListDto;
using TaskMgt.Models;

namespace TaskMgt.Services.ListService
{
    public interface IListService
    {
        Task<ServiceResponse<GetGroupListsDto>> GetListByGroupId(string groupId);

        Task<ServiceResponse<GetListDto>> GetListById(string id);

        Task<ServiceResponse<GetListDto>> CreateList(CreateListDto list);

        Task<ServiceResponse<GetListDto>> UpdateList(UpdateListDto list, string id);

        Task<ServiceResponse<GetListDto>> DeleteList(string id);
    }
}