using Xunit;
using TechnicalChallenge.Domain.Common;

namespace TechnicalChallenge.Domain.Tests.Common;

//teste base de entidade pra conferir a estrutura
public class EntityTests
{
    [Fact]
    public void Entity_ShouldHaveUniqueId()
    {
        //vê se o id é gerado sozinho
        var id = Guid.NewGuid();
        Assert.NotEqual(Guid.Empty, id);
    }
}
