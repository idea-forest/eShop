﻿using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YourBrand.Portal.Todos.Dtos;

namespace YourBrand.Portal.Todos.Commands;

public sealed record CreateTodo(string Title, string? Description, TodoStatusDto Status, string? AssigneeId, double? EstimatedHours, double? RemainingHours) : IRequest<Result<TodoDto>>
{
    public sealed class Validator : AbstractValidator<CreateTodo>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Description).MaximumLength(240);
        }
    }

    public sealed class Handler : IRequestHandler<CreateTodo, Result<TodoDto>>
    {
        private readonly ITodoRepository todoRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDomainEventDispatcher domainEventDispatcher;

        public Handler(ITodoRepository todoRepository, IUnitOfWork unitOfWork, IDomainEventDispatcher domainEventDispatcher)
        {
            this.todoRepository = todoRepository;
            this.unitOfWork = unitOfWork;
            this.domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<Result<TodoDto>> Handle(CreateTodo request, CancellationToken cancellationToken)
        {
            var todo = new Todo(request.Title, request.Description, (Domain.Enums.TodoStatus)request.Status);

            todo.UpdateEstimatedHours(request.EstimatedHours);
            todo.UpdateRemainingHours(request.RemainingHours);

            todoRepository.Add(todo);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.AssigneeId is not null)
            {
                todo.UpdateAssigneeId(request.AssigneeId);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                todo.ClearDomainEvents();
            }

            await domainEventDispatcher.Dispatch(new TodoCreated(todo.Id), cancellationToken);

            todo = await todoRepository.GetAll()
                .OrderBy(i => i.Id)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .LastAsync(cancellationToken);

            return Result.Success(todo!.ToDto());
        }
    }
}
