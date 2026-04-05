using Xunit;

namespace TechnicalChallenge.Domain.Tests.Common;

///<summary>
///Placeholder test file to ensure the test project structure is correct.
///</summary>
public class EntityTests
{
    [Fact]
    public void Entity_ShouldHaveUniqueId()
    {
        //This is a placeholder test demonstrating the test structure
        var id = Guid.NewGuid();
        Assert.NotEqual(Guid.Empty, id);
    }
}
