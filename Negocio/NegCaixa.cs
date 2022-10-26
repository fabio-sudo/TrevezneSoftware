using AcessoDados;
using ObjetoTransferencia;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Negocio
{
   public class NegCaixa
    {
        private ConexaoSqlServer sqlServer = new ConexaoSqlServer();

        //Busca os valores que tem movimentação no caixa para realizar o cadastro da sangria
        public CaixaLista BuscarCaixaValores(DateTime data)
        {
            try
            {
                sqlServer.LimparParametros();
                sqlServer.AdicionarParametro(new SqlParameter("@data", data));

                string comandoSql = "exec uspBuscarCaixaValores @data";

                DataTable tabelaRetorno = this.sqlServer.ExecutarConsulta(comandoSql, CommandType.Text);

                CaixaLista listaCaixa = new CaixaLista();
                Caixa caixa;

                foreach (DataRow registro in tabelaRetorno.Rows)
                {

                    caixa = new Caixa();
                    caixa.formaPagamento = new FormaPagamento();

                    caixa.valorCaixa = Convert.ToDouble(registro[0]);
                    caixa.jurosCaixa = Convert.ToDouble(registro[1]);
                    caixa.descontoCaixa = Convert.ToDouble(registro[2]);

                    caixa.formaPagamento.codigoFormaPagamento = Convert.ToInt32(registro[3]);
                    caixa.formaPagamento.formaPagamento = (registro[4]).ToString();
                    caixa.dataCaixa = Convert.ToDateTime(registro[5]);
                    caixa.estatusCaixa = (registro[6]).ToString();

                    listaCaixa.Add(caixa);
                }
                return listaCaixa;

            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível buscar dados do Caixa. [Negócios]. Motivo: " + ex.Message);
            }

        }

     
    
    }
}
