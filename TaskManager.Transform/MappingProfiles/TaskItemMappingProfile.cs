using Mapster;
using TaskManager.Contracts.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Transform.MappingProfiles;

public class TaskItemMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TaskItem, TaskItemResponse>()
            .Map(dest => dest.IsOverdue, src => src.IsOverdue());
    }
}
