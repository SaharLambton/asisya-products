using Asisya.Products.Domain.Interfaces;
using Moq;

namespace Asisya.Products.Tests.Unit.Helpers;

public static class MockFactory
{
    public static Mock<IUnitOfWork> CreateUnitOfWork(
        Mock<IProductRepository>? products = null,
        Mock<ICategoryRepository>? categories = null,
        Mock<IUserRepository>? users = null)
    {
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.Products).Returns((products ?? new Mock<IProductRepository>()).Object);
        uow.Setup(u => u.Categories).Returns((categories ?? new Mock<ICategoryRepository>()).Object);
        uow.Setup(u => u.Users).Returns((users ?? new Mock<IUserRepository>()).Object);
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        return uow;
    }
}
