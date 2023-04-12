using NerdStore.Core.DomainObjects;
using Xunit;
namespace NerdStore.Vendas.Domain.Tests;

public class PedidoTests
{
    [Fact(DisplayName = "Adicionar Item Novo Pedido")]
    [Trait("Categoria", "Vendas - Pedidos")]
    public void AdicionarItemPedido_NovoPedido_DeveActualizarValor()
    {
        // Arrange
        var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
        var pedidoItem = new PedidoItem(Guid.NewGuid(), "Produto Test", 2, 100);


        // Act
        pedido.AdicionarItem(pedidoItem);

        // Assert
        Assert.Equal(200, pedido.ValorTotal);
    }

    [Fact(DisplayName = "Mudar")]
    [Trait("Categoria", "Mudar")]
    public void AdicionarItem_ItemExistente_DeveIncrementarUnidadesSomarValores()
    {
        // Arrange
        // Arrange
        var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
        var produtoId = Guid.NewGuid();
        var pedidoItem = new PedidoItem(produtoId, "Produto Test", 2, 100);
        pedido.AdicionarItem(pedidoItem);

        var pedidoItem2 = new PedidoItem(produtoId, "Produto Test", 1, 100);

        // Act
        pedido.AdicionarItem(pedidoItem2);

        // Assert
        Assert.Equal(300, pedido.ValorTotal);
        Assert.Equal(1, pedido.PedidoItems.Count);
        Assert.Equal(3, pedido.PedidoItems.FirstOrDefault(i => i.ProdutoId == produtoId).Quantidade);
    }

    [Fact(DisplayName = "Adicionar Item Pedido acima do permitido")]
    [Trait("Categoria", "Vendas - Pedidos")]
    public void AdicionarItemPedido_UnidadesItemAcimaDoPermitido_DeveRetornarException()
    {
        // Arrange
        var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
        var produtoId = Guid.NewGuid();
        var pedidoItem = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM + 1, 100);

        // Act & Assert
        Assert.Throws<DomainException>(() => pedido.AdicionarItem(pedidoItem));
    }

    [Fact(DisplayName = "Adicionar Item Pedido Existente acima do permitido")]
    [Trait("Categoria", "Vendas - Pedidos")]
    public void AdicionarItemPedido_ItemExistenteSomaUnidadesAcimaDoPermitido_DeveRetornarException()
    {
        // Arrange
        var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
        var produtoId = Guid.NewGuid();
        var pedidoItem = new PedidoItem(produtoId, "Produto Teste", 1, 100);
        var pedidoItem2 = new PedidoItem(produtoId, "Produto Teste", Pedido.MAX_UNIDADES_ITEM, 100);
        pedido.AdicionarItem(pedidoItem);

        // Act & Assert
        Assert.Throws<DomainException>(() => pedido.AdicionarItem(pedidoItem2));
    }
}
