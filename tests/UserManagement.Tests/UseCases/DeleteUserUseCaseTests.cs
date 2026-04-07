using Moq;
using UserManagement.Application.Interfaces;
using UserManagement.Application.UseCases;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Tests.UseCases;

public sealed class DeleteUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly DeleteUserUseCase _useCase;

    public DeleteUserUseCaseTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _useCase = new DeleteUserUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var user = User.Create("Joao Silva", "joao@email.com", "hashed_password");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.DeleteAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(user.Id);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
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
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

