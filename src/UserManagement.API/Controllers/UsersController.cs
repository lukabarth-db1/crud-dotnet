using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.UseCases;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] GetAllUsersUseCase useCase,
        CancellationToken cancellationToken)
    {
        var users = await useCase.ExecuteAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] GetUserByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var user = await useCase.ExecuteAsync(id, cancellationToken);
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        [FromServices] CreateUserUseCase useCase,
        [FromServices] IValidator<CreateUserRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

        var user = await useCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        [FromServices] UpdateUserUseCase useCase,
        [FromServices] IValidator<UpdateUserRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

        var user = await useCase.ExecuteAsync(id, request, cancellationToken);
        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] DeleteUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}

