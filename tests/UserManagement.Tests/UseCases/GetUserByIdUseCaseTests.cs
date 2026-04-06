using Moq;
using UserManagement.Application.Interfaces;
using UserManagement.Application.UseCases;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Tests.UseCases;

public sealed class GetUserByIdUseCaseTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly GetUserByIdUseCase _useCase;

    public GetUserByIdUseCaseTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _useCase = new GetUserByIdUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = User.Create("Joao Silva", "joao@email.com");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _useCase.ExecuteAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name.Value, result.Name);
        Assert.Equal(user.Email.Value, result.Email);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _useCase.ExecuteAsync(nonExistentId);

        // Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Contains(nonExistentId.ToString(), exception.Message);
    }
}

