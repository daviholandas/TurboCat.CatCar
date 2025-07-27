using CatCar.SharedKernel.Common;

namespace CatCar.SharedKernel.Tests;

public class EntityTests
{
    [Fact]
    public void Entity_CanBeInstantiated()
    {
        // Arrange & Act
        var entity = new Entity();
        
        // Assert
        Assert.NotNull(entity);
    }
}