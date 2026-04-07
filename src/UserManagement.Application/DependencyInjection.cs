using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.UseCases;
using UserManagement.Application.Validators;

namespace UserManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginUseCase>();
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<GetAllUsersUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();

        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        return services;
    }
}

