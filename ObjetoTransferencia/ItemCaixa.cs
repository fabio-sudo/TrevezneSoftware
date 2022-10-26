using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjetoTransferencia
{
   public class ItemCaixa
    {
     public string tipoVenda { get; set; }
     public int codigoItem { get; set; }
     public Venda Venda { get; set; }
     public ProdutoCor ProdutoCor { get; set; }
     public Tamanho Tamanho { get; set; }
     public double totalItem { get; set; }
     public double valorPago { get; set; }
     public double descontoItem { get; set; }
     public string estatusItem { get; set; }
     public DateTime dataItem { get; set; }

   }
       public class ItemCaixaLista : List<ItemCaixa> { }

}
