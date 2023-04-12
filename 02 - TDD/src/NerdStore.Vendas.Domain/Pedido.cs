using NerdStore.Core.DomainObjects;

namespace NerdStore.Vendas.Domain;

public class Pedido
{
    public static int MAX_UNIDADES_ITEM => 15;
    public static int MIN_UNIDADES_ITEM => 1;
    
    protected Pedido()
    {
        _pedidosItens = new List<PedidoItem>();
    }

    public decimal ValorTotal { get; private set; }
    public PedidoStatus PedidoStatus { get; private set; }
    public Guid ClienteId { get; private set; }

    private List<PedidoItem> _pedidosItens;
    public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidosItens;

    private void CalcularValorPedido()
    {
        ValorTotal = _pedidosItens.Sum(i => i.CalcularValor());
    }

    private bool PedidoItemExistente(PedidoItem item)
    {
        return _pedidosItens.Any(p => p.ProdutoId == item.ProdutoId);
    }

    private void ValidarQuantidadeItemPermitida(PedidoItem item)
    {
        var quantidadeItems = item.Quantidade;
        if(PedidoItemExistente(item))
        {
            var itemExistente = _pedidosItens.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);
            quantidadeItems += itemExistente.Quantidade;
        }

        if(quantidadeItems > MAX_UNIDADES_ITEM) throw new DomainException($"Maximo de {MAX_UNIDADES_ITEM} unidades por produtos");
    }

    public void AdicionarItem(PedidoItem pedidoItem)
    {
        ValidarQuantidadeItemPermitida(pedidoItem);

        if (PedidoItemExistente(pedidoItem))
        {
            var itemExistente = _pedidosItens.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);
            itemExistente.AdicionarUnidade(pedidoItem.Quantidade);
            pedidoItem = itemExistente;

            _pedidosItens.Remove(itemExistente);
        }

        _pedidosItens.Add(pedidoItem);
        CalcularValorPedido();
    }

    public void TornarRascunho()
    {
        PedidoStatus = PedidoStatus.Rascunho;
    }

    public static class PedidoFactory
    {
        public static Pedido NovoPedidoRascunho(Guid clienteId)
        {
            var pedido =  new Pedido
            {
                ClienteId = clienteId
            };

            pedido.TornarRascunho();
            return pedido;
        }
    }
}

public enum PedidoStatus
{
    Rascunho = 0,
    Iniciado = 1,
    Pago = 2,
    Entregue = 3,
    Cancelado = 4
}

public class PedidoItem 
{
    public Guid ProdutoId { get; private set; }
    public string ProdutoNome { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }

    public PedidoItem(Guid produtoId, string produtoNome, int quantidade, decimal valorUnitario)
    {
        if (quantidade < Pedido.MIN_UNIDADES_ITEM) throw new DomainException($"Minimo de {Pedido.MIN_UNIDADES_ITEM} unidades por produtos");

        ProdutoId = produtoId;
        ProdutoNome = produtoNome;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }

    internal void AdicionarUnidade(int unidades)
    {
        Quantidade += unidades;
    }

    internal decimal CalcularValor()
    {
        return ValorUnitario * Quantidade;
    }
}


