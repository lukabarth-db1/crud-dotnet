using Moq;
using UserManagement.Application.Interfaces;
using UserManagement.Application.UseCases;
using UserManagement.Domain.Entities;

namespace UserManagement.Tests.UseCases;

public sealed class GetAllUsersUseCaseTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly GetAllUsersUseCase _useCase;

    public GetAllUsersUseCaseTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _useCase = new GetAllUsersUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnAllUsers_WhenUsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            User.Create("Ana Silva", "ana@email.com"),
            User.Create("Bruno Costa", "bruno@email.com"),
            User.Create("Carlos Lima", "carlos@email.com"),
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        Assert.Equal(users.Count, result.Count);
        Assert.Equal(users[0].Name.Value, result[0].Name);
        Assert.Equal(users[1].Email.Value, result[1].Email);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        Assert.Empty(result);
    }
}

