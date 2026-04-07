using Moq;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Application.UseCases;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Tests.UseCases;

public sealed class UpdateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UpdateUserUseCase _useCase;

    public UpdateUserUseCaseTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _useCase = new UpdateUserUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateUser_WhenRequestIsValid()
    {
        // Arrange
        var user = User.Create("Joao Silva", "joao@email.com", "hashed_password");
        var request = new UpdateUserRequest("Joao Santos", "joao.santos@email.com");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(user.Id, request);

        // Assert
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email, result.Email);
        Assert.NotNull(result.UpdatedAt);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldAllowSameEmail_WhenEmailBelongsToSameUser()
    {
        // Arrange
        var user = User.Create("Joao Silva", "joao@email.com", "hashed_password");
        var request = new UpdateUserRequest("Joao Santos", "joao@email.com");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user); // retorna o próprio usuário

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(user.Id, request);

        // Assert
        Assert.Equal(request.Name, result.Name);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateUserRequest("Joao Santos", "joao.santos@email.com");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _useCase.ExecuteAsync(nonExistentId, request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowDomainException_WhenEmailBelongsToAnotherUser()
    {
        // Arrange
        var user = User.Create("Joao Silva", "joao@email.com", "hashed_password");
        var otherUser = User.Create("Maria Lima", "maria@email.com", "hashed_password");
        var request = new UpdateUserRequest("Joao Santos", "maria@email.com");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherUser); // email pertence a outro usuário

        // Act
        var act = () => _useCase.ExecuteAsync(user.Id, request);

        // Assert
        var exception = await Assert.ThrowsAsync<DomainException>(act);
        Assert.Contains(request.Email, exception.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
