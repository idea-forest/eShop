﻿using YourBrand.Portal.Application.Todos.Dtos;
using YourBrand.Portal.Application.Users;

namespace YourBrand.Portal.Application;

public static class Mappings
{
    public static TodoDto ToDto(this Todo todo) => new TodoDto(todo.Id, todo.Title, todo.Description, (TodoStatusDto)todo.Status, todo.AssigneeId?.ToDto(), todo.EstimatedHours, todo.RemainingHours, todo.Created, todo.CreatedBy.ToDto(), todo.LastModified, todo.LastModifiedBy?.ToDto());

    public static UserDto ToDto(this User user) => new UserDto(user.Id, user.Name);

    public static UserInfoDto ToDto2(this User user) => new UserInfoDto(user.Id, user.Name);
}
