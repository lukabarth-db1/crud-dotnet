using Moq;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Application.UseCases;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Tests.UseCases;

public sealed class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly CreateUserUseCase _useCase;

    public CreateUserUseCaseTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");
        _useCase = new CreateUserUseCase(_repositoryMock.Object, _passwordHasherMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateUser_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateUserRequest("Joao Silva", "joao@email.com", "senha1234");

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email.ToLowerInvariant(), result.Email);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Null(result.UpdatedAt);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowDomainException_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new CreateUserRequest("Joao Silva", "joao@email.com", "senha1234");
        var existingUser = User.Create("Maria", "joao@email.com", "hashed_password");

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<DomainException>(act);
        Assert.Contains(request.Email, exception.Message);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowDomainException_WhenNameIsTooShort()
    {
        // Arrange
        var request = new CreateUserRequest("X", "joao@email.com", "senha1234");

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await Assert.ThrowsAsync<DomainException>(act);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowDomainException_WhenEmailIsInvalid()
    {
        // Arrange
        var request = new CreateUserRequest("Joao Silva", "email-invalido", "senha1234");

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = () => _useCase.ExecuteAsync(request);

        // Assert
        await Assert.ThrowsAsync<DomainException>(act);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

