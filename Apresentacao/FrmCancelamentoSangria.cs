using Apresentacao.Formulas;
using Negocio;
using ObjetoTransferencia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Apresentacao
{
    public partial class FrmCancelamentoSangria : Form
    {
        Sangria objSangriaAlterada = new Sangria();
        Sangria sangriaSelecionada = new Sangria();
        SangriaLista sangriaLista = new SangriaLista();//Lista para preencher o gride
        SangriaLista sangriaListaAlt = new SangriaLista();//Lista para cadastrar
        NegSangria nSangria = new NegSangria();

        ListaFormaPagamento listaFormaPagamento = new ListaFormaPagamento();
        NegFormaPagamento nFormaPagamento = new NegFormaPagamento();

        NegFuncionario nFuncionario = new NegFuncionario();

        //-----------------Metodos
        Metodos metodos = new Metodos();
        int quantidadeItem = 0;
        double totalCancelar = 0;
        double totalRestoCaixa = 0;
        double valorSugestao = 0;
        string FormularioCancelado = "";
        //Objetos Gride
        TextBox caixaTextoGride;

        //Objetos para o cancelamento
        SangriaListaCancelamento listaSangriaCancelada = new SangriaListaCancelamento();
        ItemVendaLista itemVendaSelecionado = new ItemVendaLista();
        ParcialVendaListaSangria parcialVendaSelecionado = new ParcialVendaListaSangria();
        ItemCrediarioLista itemCrediarioPagoSelecionado = new ItemCrediarioLista();
        ItemCrediarioParcialLista itemCrediarioParcialSelecionado = new ItemCrediarioParcialLista();
        CaixaLista caixaSelecionado = new CaixaLista();

        public FrmCancelamentoSangria(Sangria sangria,CaixaLista caixa, [Optional] ParcialVendaListaSangria parcialVenda, [Optional]  ItemVendaLista itemVenda, [Optional] ItemCrediarioLista itemCrediario, [Optional]  ItemCrediarioParcialLista itemCrediarioParcial)
        {
            InitializeComponent();

            sangriaSelecionada = sangria;
            
            if (caixa != null)
            {
                caixaSelecionado = caixa;
            }
            //Objetos a serem cancelados
            if (parcialVenda != null) {
                parcialVendaSelecionado = parcialVenda;
            }
            if (itemVenda != null)
            {
                itemVendaSelecionado = itemVenda;
            }
            if (itemCrediario != null)
            {
                itemCrediarioPagoSelecionado = itemCrediario;
            }
            if (itemCrediarioParcial != null)
            {
                itemCrediarioParcialSelecionado = itemCrediarioParcial;
            }
            
        }

        //----------------------------------------Metodos

        //Inicia o formulario
        private void metodoIniciaFormulario()
        {

            dtpDataSangria.Value = sangriaSelecionada.dataSangria;
            sangriaLista = nSangria.BuscarSangriaParaCancelamento(sangriaSelecionada.dataSangria);
            metodoPreencheCombobox();

            if (parcialVendaSelecionado.Count > 0) {
                FormularioCancelado = "ParcialVenda";
                AtualizarDataGridCancelamentoVendaParcial();
                metodoValidaCalculaGrideAtualizacoes();
            }
            else if (itemVendaSelecionado.Count > 0) {
                FormularioCancelado = "ItemVenda";
                AtualizarDataGridCancelamentoItemVenda();
                metodoValidaCalculaGrideAtualizacoes();
            }
            else if (itemCrediarioPagoSelecionado.Count > 0) {

                FormularioCancelado = "ItemCrediarioPago";
                sangriaLista = metodoSangriaCrediario(itemCrediarioPagoSelecionado);
                AtualizarDataGridCancelamentoItemCrediarioPago();
                metodoValidaCalculaGrideAtualizacoes();
                //Sangria deve mostrar apenas formas de pagamento que devem ser atualizadas no gride
            }

            AtualizarDataGrid();         
            dgvSangria.Focus();
            metodoCalculaTotais();
            //Fazer um método para percorrer o gride e verficar quais valores devem ser atualizado
            //Caso não consiga fazer o mesmo no gride
            metodoAtualizaCancelamentoGrid();
        }

        private void AtualizarDataGrid()
        {
            this.dgvSangria.Rows.Clear(); // Limpa todos os registros atuais no grid de funcionários.

            int indice = 0;

            foreach (Sangria sangCancelado in listaSangriaCancelada)
            {

                foreach (Sangria sang in this.sangriaLista)
                {
                    if (sangCancelado.pagamentoSangria.codigoFormaPagamento == sang.pagamentoSangria.codigoFormaPagamento)
                    {
                        
                        this.dgvSangria.Rows.Add(1);

                        //----------------------------Exibindo valor a ser cancelado
                        if (sangCancelado.valorSangria > 0)
                        {
                            dgvSangria.Rows[indice].Cells["valorParcialVenda"].Style.ForeColor = Color.Red;
                        }
                        //Quantidade itens sangria 3                        
                        quantidadeItem = sangriaLista.Where(t => t.pagamentoSangria.codigoFormaPagamento == sang.pagamentoSangria.codigoFormaPagamento).Count();
                        
                        //Valor total cancelado de itens R$60
                        totalCancelar = sangriaLista.Where(t => t.pagamentoSangria.codigoFormaPagamento == sang.pagamentoSangria.codigoFormaPagamento).Select(s => s.valorSangria).Sum();
                        
                        //Total restante R$ 10,16  Débito + 19,84 PIX
                        totalRestoCaixa = parcialVendaSelecionado.Where(p => p.formaPagamentoVenda.codigoFormaPagamento == sang.pagamentoSangria.codigoFormaPagamento).Select(s => s.valorParcialVenda).Sum();
                        
                         valorSugestao = (sang.valorSangria)-(totalRestoCaixa/quantidadeItem);


                         this.dgvSangria[0, indice].Value = sang.ordemSangra;
                         this.dgvSangria[1, indice].Value = sang.valorSangria;//Valor da sangria para atualização e liberação cancelamento
                        //------------------------------------------------
                        this.dgvSangria[2, indice].Value = sang.valorSangria - valorSugestao;
                        this.dgvSangria[3, indice].Value = sang.valorSangria;//Valor atual da sangria FIXO
                        this.dgvSangria[4, indice].Value = sang.funcionarioSangria.codigoFuncionario;
                        this.dgvSangria[5, indice].Value = sang.funcionarioSangria.nomeFuncionario + " " + sang.funcionarioSangria.sobrenomeFuncionario;
                        this.dgvSangria[6, indice].Value = sang.pagamentoSangria.codigoFormaPagamento;
                        this.dgvSangria[7, indice].Value = sang.pagamentoSangria.formaPagamento;
                        this.dgvSangria[8, indice].Value = 0;
                        this.dgvSangria[9, indice].Value = sang.estatusSangria;
                        this.dgvSangria[10, indice].Value = sang.dataSangria;
                        indice++;
                    }
                }
            }

            dgvSangria.Update();
            this.dgvSangria.ClearSelection();
        }

        //Método Adiciona a lista de sangria os Valores De acordo com as datas de cancelamento
        public SangriaLista metodoSangriaCrediario(ItemCrediarioLista lista)
        {
            sangriaLista = new SangriaLista();


            int contador = 0;
            foreach (ItemCrediario item in lista)
            {
                if (sangriaLista.Count == 0)
                {

                    sangriaLista = nSangria.BuscarSangriaParaCancelamento(item.dataItemCrediario);
                }
                else if (item.dataItemCrediario != sangriaLista[contador].dataSangria)
                {
                    //Toda vez que a data for diferente busca os itens do caixa 
                    //E os adiciona na lista antiga
                    SangriaLista sangriaListaNova = new SangriaLista();
                    sangriaListaNova = nSangria.BuscarSangriaParaCancelamento(item.dataItemCrediario);

                    foreach (Sangria sangAdd in sangriaListaNova)
                    {

                        sangriaLista.Add(sangAdd);

                    }

                }

            }

            return sangriaLista;


        }
       
        //ITEMVENDA
        private void AtualizarDataGridCancelamentoItemVenda()
        {
            try
            {

                this.dgvCancelamento.Rows.Clear(); // Limpa todos os registros atuais no grid de funcionários.

                //Adiciona Valores e forma de pagamentos a serem cancelados
                Sangria sangriaCancelada = new Sangria();
                listaSangriaCancelada = new SangriaListaCancelamento();
                int contador = 0;

                foreach (ItemVenda item in itemVendaSelecionado.OrderBy(o => o.Venda.formaPagamento.codigoFormaPagamento))
                {

                    if (listaSangriaCancelada.Count() > 0)
                    {

                        if (listaSangriaCancelada[contador].pagamentoSangria.codigoFormaPagamento == item.Venda.formaPagamento.codigoFormaPagamento)
                        {

                            listaSangriaCancelada[contador].valorSangria = listaSangriaCancelada[contador].valorSangria + (item.quantidadeVenda * item.precoVenda);
                            listaSangriaCancelada[contador].retiradaSangria = listaSangriaCancelada[contador].retiradaSangria + (item.quantidadeVenda * item.precoVenda);

                        }
                        else
                        {

                            sangriaCancelada = new Sangria();
                            sangriaCancelada.pagamentoSangria = new FormaPagamento();

                            sangriaCancelada.valorSangria = item.quantidadeVenda * item.precoVenda;
                            sangriaCancelada.retiradaSangria = item.quantidadeVenda * item.precoVenda;
                            sangriaCancelada.pagamentoSangria.codigoFormaPagamento = item.Venda.formaPagamento.codigoFormaPagamento;
                            sangriaCancelada.pagamentoSangria.formaPagamento = item.Venda.formaPagamento.formaPagamento;
                            sangriaCancelada.dataSangria = item.dataItemVenda;
                            listaSangriaCancelada.Add(sangriaCancelada);

                            contador++;
                        }
                    }
                    else
                    {

                        sangriaCancelada = new Sangria();
                        sangriaCancelada.pagamentoSangria = new FormaPagamento();

                        sangriaCancelada.valorSangria = item.quantidadeVenda * item.precoVenda;
                        sangriaCancelada.pagamentoSangria.codigoFormaPagamento = item.Venda.formaPagamento.codigoFormaPagamento;
                        sangriaCancelada.pagamentoSangria.formaPagamento = item.Venda.formaPagamento.formaPagamento;
                        sangriaCancelada.retiradaSangria = item.quantidadeVenda * item.precoVenda;
                        sangriaCancelada.dataSangria = item.dataItemVenda;
                        listaSangriaCancelada.Add(sangriaCancelada);
                    }

                }

                if (this.listaSangriaCancelada.Count > 0)
                {
                    this.dgvCancelamento.Rows.Add(this.listaSangriaCancelada.Count);
                }
                else
                {
                    return;
                }

                int indice = 0;
                foreach (Sangria sang in this.listaSangriaCancelada)
                {
                    this.dgvCancelamento[0, indice].Value = sang.pagamentoSangria.codigoFormaPagamento;
                    this.dgvCancelamento[1, indice].Value = sang.pagamentoSangria.formaPagamento;//forma de pagamento
                    this.dgvCancelamento[2, indice].Value = caixaSelecionado.Where(c => c.formaPagamento.codigoFormaPagamento == sang.pagamentoSangria.codigoFormaPagamento).Select(s => s.valorCaixa).Sum() - sang.valorSangria;//Valor a ser cancelado
                    this.dgvCancelamento[3, indice].Value = sang.retiradaSangria;
                    this.dgvCancelamento[4, indice].Value = sang.dataSangria;
                    indice++;
                }
                dgvCancelamento.Update();

                this.dgvCancelamento.ClearSelection();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        //VENDAPARCIAL
        private void AtualizarDataGridCancelamentoVendaParcial() {
          try
          { 
            this.dgvCancelamento.Rows.Clear(); // Limpa todos os registros atuais no grid de funcionários.

            //Adiciona Valores e forma de pagamentos a serem cancelados
            Sangria sangriaCancelada = new Sangria();
            listaSangriaCancelada = new SangriaListaCancelamento();

            #region ADD PARCIALVENDA 
            int contador = 0;
            foreach (ParcialVenda item in parcialVendaSelecionado.OrderBy(o => o.formaPagamentoVenda.codigoFormaPagamento))
            {

                if (listaSangriaCancelada.Count() > 0)
                {

                    if (listaSangriaCancelada[contador].pagamentoSangria.codigoFormaPagamento == item.formaPagamentoVenda.codigoFormaPagamento)
                    {

                        listaSangriaCancelada[contador].valorSangria = listaSangriaCancelada[contador].valorSangria + (item.valorParcialVenda);
                        listaSangriaCancelada[contador].retiradaSangria = listaSangriaCancelada[contador].retiradaSangria + (item.valorParcialVenda);
                    }
                    else
                    {

                        sangriaCancelada = new Sangria();
                        sangriaCancelada.pagamentoSangria = new FormaPagamento();

                        sangriaCancelada.valorSangria = item.valorParcialVenda;
                        sangriaCancelada.retiradaSangria = item.valorParcialVenda;
                        sangriaCancelada.pagamentoSangria.codigoFormaPagamento = item.formaPagamentoVenda.codigoFormaPagamento;
                        sangriaCancelada.pagamentoSangria.formaPagamento = item.formaPagamentoVenda.formaPagamento;
                        sangriaCancelada.dataSangria = sangriaSelecionada.dataSangria;
                        listaSangriaCancelada.Add(sangriaCancelada);

                        contador++;
                    }
                }
                else
                {

                    sangriaCancelada = new Sangria();
                    sangriaCancelada.pagamentoSangria = new FormaPagamento();

                    sangriaCancelada.valorSangria = item.valorParcialVenda;
                    sangriaCancelada.pagamentoSangria.codigoFormaPagamento = item.formaPagamentoVenda.codigoFormaPagamento;
                    sangriaCancelada.pagamentoSangria.formaPagamento = item.formaPagamentoVenda.formaPagamento;
                    sangriaCancelada.retiradaSangria = item.valorParcialVenda;
                    sangriaCancelada.dataSangria = sangriaSelecionada.dataSangria;
                    listaSangriaCancelada.Add(sangriaCancelada);
                }

            }
            #endregion

            if (this.listaSangriaCancelada.Count > 0)
            {
                this.dgvCancelamento.Rows.Add(this.listaSangriaCancelada.Count);
            }
            else
            {
                return;
            }

            int indice = 0;
            foreach (Sangria sang in this.listaSangriaCancelada)
            {
                this.dgvCancelamento[0, indice].Value = sang.pagamentoSangria.codigoFormaPagamento;
                this.dgvCancelamento[1, indice].Value = sang.pagamentoSangria.formaPagamento;//forma de pagamentoo
                this.dgvCancelamento[2, indice].Value = sang.valorSangria;//Valor a ser cancelado      
                this.dgvCancelamento[3, indice].Value = sang.retiradaSangria;
                this.dgvCancelamento[4, indice].Value = sang.dataSangria;
                indice++;
            }
            dgvCancelamento.Update();

            this.dgvCancelamento.ClearSelection();
          }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        //ItemCrediarioPago
        private void AtualizarDataGridCancelamentoItemCrediarioPago() {
            try
            {

                this.dgvCancelamento.Rows.Clear(); // Limpa todos os registros atuais no grid de funcionários.

                //Adiciona Valores e forma de pagamentos a serem cancelados
                Sangria sangriaCancelada = new Sangria();
                listaSangriaCancelada = new SangriaListaCancelamento();
                
                int contador = 0;
                foreach (ItemCrediario item in itemCrediarioPagoSelecionado)
                {
                     if (listaSangriaCancelada.Count() > 0)
                    {

                        if (listaSangriaCancelada[contador].pagamentoSangria.codigoFormaPagamento == item.formaPagamento.codigoFormaPagamento &&
                            listaSangriaCancelada[contador].dataSangria == item.dataItemCrediario)
                        {

                            listaSangriaCancelada[contador].valorSangria = listaSangriaCancelada[contador].valorSangria + (item.quantidadeItemCrediario * item.valorItemCrediario);
                            listaSangriaCancelada[contador].retiradaSangria = listaSangriaCancelada[contador].retiradaSangria + (item.quantidadeItemCrediario * item.valorItemCrediario);

                        }
                        else
                        {

                            sangriaCancelada = new Sangria();
                            sangriaCancelada.pagamentoSangria = new FormaPagamento();

                            sangriaCancelada.valorSangria = item.quantidadeItemCrediario * item.valorItemCrediario;
                            sangriaCancelada.retiradaSangria = item.quantidadeItemCrediario * item.valorItemCrediario;
                            sangriaCancelada.pagamentoSangria.codigoFormaPagamento = item.Venda.formaPagamento.codigoFormaPagamento;
                            sangriaCancelada.pagamentoSangria.formaPagamento = item.Venda.formaPagamento.formaPagamento;
                            sangriaCancelada.dataSangria = item.dataItemCrediario;
                            listaSangriaCancelada.Add(sangriaCancelada);

                            contador++;
                        }
                    }
                    else
                    {

                        sangriaCancelada = new Sangria();
                        sangriaCancelada.pagamentoSangria = new FormaPagamento();

                        sangriaCancelada.valorSangria = item.quantidadeItemCrediario * item.valorItemCrediario;
                        sangriaCancelada.pagamentoSangria.codigoFormaPagamento = item.Venda.formaPagamento.codigoFormaPagamento;
                        sangriaCancelada.pagamentoSangria.formaPagamento = item.Venda.formaPagamento.formaPagamento;
                        sangriaCancelada.retiradaSangria = item.quantidadeItemCrediario * item.valorItemCrediario;
                        sangriaCancelada.dataSangria = item.dataItemCrediario;
                        listaSangriaCancelada.Add(sangriaCancelada);
                    }
                }//Foreach

                     //Adiciona colunas ao gride
                     if (this.listaSangriaCancelada.Count > 0)
                     {
                       this.dgvCancelamento.Rows.Add(this.listaSangriaCancelada.Count);
                     }
                     else
                     {
                    return;
                     }

                int indice = 0;
                foreach (Sangria sang in this.listaSangriaCancelada)
                {
                    this.dgvCancelamento[0, indice].Value = sang.pagamentoSangria.codigoFormaPagamento;
                    this.dgvCancelamento[1, indice].Value = sang.pagamentoSangria.formaPagamento;//forma de pagamento
                    this.dgvCancelamento[2, indice].Value = caixaSelecionado.Where(c => c.formaPagamento.codigoFormaPagamento == sang.pagamentoSangria.codigoFormaPagamento && c.dataCaixa == sang.dataSangria).Select(s => s.valorCaixa).Sum() - sang.valorSangria;//Valor a ser cancelado
                    this.dgvCancelamento[3, indice].Value = sang.retiradaSangria;
                    this.dgvCancelamento[4, indice].Value = sang.dataSangria;
                    indice++;
                }
                dgvCancelamento.Update();

                this.dgvCancelamento.ClearSelection();

            }//Try
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        
        }
       
        //Preencher ComboBox
        public void metodoPreencheCombobox()
        {

            this.formaPagamentoParcial.Items.Clear();
            this.formaPagamentoCancelado.Items.Clear();

            this.listaFormaPagamento = nFormaPagamento.BuscarFormaPagamentoPorNome("");

            foreach (FormaPagamento pag in this.listaFormaPagamento)
            {
                if (pag.formaPagamento != "CREDIARIO")
                {
                    {
                        this.formaPagamentoParcial.Items.IndexOf(pag.codigoFormaPagamento);
                        this.formaPagamentoParcial.Items.Add(pag.formaPagamento);

                        this.formaPagamentoCancelado.Items.IndexOf(pag.codigoFormaPagamento);
                        this.formaPagamentoCancelado.Items.Add(pag.formaPagamento);
                    }
                }
            }
        }

        //Calcula Totais
        private void metodoCalculaTotais()
        {
            //Caixa
            double valorSangria = 0;
            double valorSangriaNova = 0;
            double valorCancelado = 0;//VAlor do caixa
            double valorRetirada = 0;
            //-----------------------------------------Calcula Totais do Datagride
            //faz a soma dos totais dos valores do gride
            #region dgvSanngria
            foreach (DataGridViewRow col in dgvSangria.Rows)
            {
                //Valor da Parcial
                valorSangriaNova = valorSangriaNova + Convert.ToDouble(col.Cells[1].Value);
                valorSangria = valorSangria + Convert.ToDouble(col.Cells[3].Value);//Valor fixo da sangria
            }
            #endregion

            #region dgvCancelamento
            foreach (DataGridViewRow col in dgvCancelamento.Rows)
            {
                    //Valor da Parcial
                    valorCancelado = valorCancelado +Convert.ToDouble(col.Cells[2].Value);
                    valorRetirada = valorRetirada + Convert.ToDouble(col.Cells[3].Value);           

                if (Convert.ToDouble(col.Cells[2].Value) >= 0)
                {
                    col.DefaultCellStyle.ForeColor = Color.SkyBlue;
                    col.ErrorText = "";
                }
                else {
                    col.DefaultCellStyle.ForeColor = Color.Red;
                    col.ErrorText = "Atualizar Valor: " + col.Cells[1].Value.ToString();
                }
            }
            #endregion

            
            //-----------Venda
            if (FormularioCancelado == "ParcialVenda")
            {
                lbCaixaTotal.Text = "+ " + String.Format("{0:C2}", (listaSangriaCancelada.Sum(p => p.valorSangria)) + sangriaLista.Sum(p => p.valorSangria * quantidadeItem));//TotalCaixa
            }
            else {
                lbCaixaTotal.Text = "+ " + String.Format("{0:C2}", (listaSangriaCancelada.Sum(p => p.valorSangria)) + valorCancelado);//TotalCaixa          
            }
            lbSangriaTotal.Text = "+" + String.Format("{0:C2}", valorSangriaNova);//TotalNovaSangria
            lbTotalCancelado.Text = "- " + String.Format("{0:C2}",itemVendaSelecionado.Sum(i => i.quantidadeVenda*i.precoVenda));//Soma dos Itens Cancelados
            lbCaixaRestante.Text = "+ " + String.Format("{0:C2}", (valorCancelado));//ValorRestanteCaixa           
            
            }

        //Valida se há algum valor lançado
        public Boolean metodoValidaSangria()
        {
            double ValidaSangria = 0;

            //verifica se campos vazios
            foreach (DataGridViewRow col in dgvCancelamento.Rows)
            {

             ValidaSangria = ValidaSangria + Convert.ToDouble(col.Cells[2].Value);//Grid Cancelamento o valor for zeradoou positivo

            }
            if (ValidaSangria >= 0)
            {

                return true;
            }
            else { return false; }

        }

        //Retorna Lista de Parcias Atualizadas
        private void metodoAddSangriaLista()
        {

            sangriaListaAlt = new SangriaLista();

            foreach (DataGridViewRow col in dgvSangria.Rows)
            {
                Sangria newSangria = new Sangria();
                newSangria.pagamentoSangria = new FormaPagamento();
                newSangria.funcionarioSangria = new Funcionario();

                newSangria.codigoSangria = objSangriaAlterada.codigoSangria;
                newSangria.valorSangria = Convert.ToDouble(col.Cells[0].Value);
                newSangria.pagamentoSangria.codigoFormaPagamento = Convert.ToInt32(col.Cells[3].Value);
                newSangria.pagamentoSangria.formaPagamento = (col.Cells[4].Value).ToString();
                newSangria.funcionarioSangria = objSangriaAlterada.funcionarioSangria;
                newSangria.descontoItem = Convert.ToDouble(col.Cells[5].Value);
                newSangria.JurosItem = Convert.ToDouble(col.Cells[6].Value);
                newSangria.dataSangria = objSangriaAlterada.dataSangria;
                newSangria.ordemSangra = objSangriaAlterada.ordemSangra;

                if (newSangria.valorSangria > 0)//Só adiciona valores lançados
                {
                    sangriaListaAlt.Add(newSangria);
                }
            }
        }

        //Metodo atualiza valores validação
        private void metodoAtualizaCancelamentoGrid() {

            double valorCanceladoAtualizar = 0;
            double totalCancelarAtualiza = listaSangriaCancelada.Where(t => t.pagamentoSangria.codigoFormaPagamento == Convert.ToInt32(dgvSangria.CurrentRow.Cells[6].Value)).Select(s => s.valorSangria).Sum();      
            
            if(FormularioCancelado =="ItemVenda"){
            totalCancelarAtualiza =  (caixaSelecionado.Where(c => c.formaPagamento.codigoFormaPagamento == Convert.ToInt32(dgvSangria.CurrentRow.Cells[6].Value)).Select(s => s.valorCaixa).Sum() - totalCancelarAtualiza);
           }
            foreach (DataGridViewRow col in dgvSangria.Rows)
            {
                if (col.Cells[6].Value.ToString() == dgvSangria.CurrentRow.Cells[6].Value.ToString()) {

                    valorCanceladoAtualizar = valorCanceladoAtualizar + Convert.ToDouble(col.Cells[1].Value);
                
                }
            }
            foreach (DataGridViewRow col in dgvSangria.Rows)
            {
                if (valorCanceladoAtualizar <= totalCancelarAtualiza)
                {
                    if (col.Cells[6].Value.ToString() == dgvSangria.CurrentRow.Cells[6].Value.ToString())
                    {
                        col.Cells["valorParcialVenda"].Style.ForeColor = Color.SkyBlue;
                    }
                }
                else {

                    if (col.Cells[6].Value.ToString() == dgvSangria.CurrentRow.Cells[6].Value.ToString())
                    {
                        col.Cells["valorParcialVenda"].Style.ForeColor = Color.Red;
                    }
                }

            }

            this.dgvSangria.ClearSelection();                   
        }

        //Método valida atualização da sangria
        private Boolean metodoValidaAtualizacao() {

            double valorSangria = 0;
            double valorCaixa = 0;
            
            foreach (DataGridViewRow col in dgvSangria.Rows) { 
            
            valorSangria = valorSangria + Convert.ToDouble(col.Cells[1].Value);
            
            }
            foreach (DataGridViewRow col in dgvCancelamento.Rows)
            {

                valorCaixa = valorCaixa + Convert.ToDouble(col.Cells[2].Value);

            }

            if (valorCaixa >= valorSangria) { return true; } 
            else {
                FrmCaixaDialogo frmCaixaCad = new FrmCaixaDialogo("Sangria maior que o Caixa",
                "Sangria maior que valor do Caixa: \n" + (valorSangria - valorCaixa).ToString(),
                Properties.Resources.DialogErro,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Ok", "",
                false);
                frmCaixaCad.ShowDialog();
                return false; 
            }
        
        }

        //Percorre os grides para validar os valores que devem ser atualizados na sangria
        //Testar
        //testar
        //testar
        //testar
        //testar
        //testar
        //testar
        private void metodoValidaCalculaGrideAtualizacoes() {
            
            double valorCanceladoAtualizar = 0;
            double totalCancelarAtualiza = 0;
            
            //Percorre as formas de pagamenta do caixa para verificar valores a serem cancelados 
            foreach (DataGridViewRow colCaixa in dgvCancelamento.Rows)
            {
                valorCanceladoAtualizar = 0;
                totalCancelarAtualiza = listaSangriaCancelada.Where(t => t.pagamentoSangria.codigoFormaPagamento == Convert.ToInt32(colCaixa.Cells[0].Value)).Select(s => s.valorSangria).Sum();      
               
                //Soma total dos itens cancelados de acordo com a forma de pagamente 
                foreach (DataGridViewRow colSangria in dgvSangria.Rows)
                {
                    if (colCaixa.Cells[0].Value.ToString() == colSangria.Cells[6].Value.ToString() &&
                    colCaixa.Cells[4].Value == colSangria.Cells[10].Value)
                    {
                        valorCanceladoAtualizar = valorCanceladoAtualizar + Convert.ToDouble(colSangria.Cells[1].Value);
                    }
                }
                
                //Caso o Valor a ser cancelado for <= ao valor restante do caixa muda a cor dos valores para Azul SENÃO Vermelho
                if (valorCanceladoAtualizar <= totalCancelarAtualiza)
                {
                    foreach (DataGridViewRow colValores in dgvSangria.Rows)
                    {
                        if (colValores.Cells[6].Value.ToString() == colCaixa.Cells[0].Value.ToString())
                        {
                            colValores.Cells["valorParcialVenda"].Style.ForeColor = Color.SkyBlue;
                        }
                    }

                }
                //Soma dos itens forem maior doque valor restante do caixa cor fica vermelha
                else {

                    foreach (DataGridViewRow colValores in dgvSangria.Rows)
                    {
                        if (colValores.Cells[6].Value.ToString() == colCaixa.Cells[0].Value.ToString())
                        {
                            colValores.Cells["valorParcialVenda"].Style.ForeColor = Color.Red;
                        }
                    }
                
                }
             }
            
           
          this.dgvSangria.ClearSelection(); 
        }

        //-------------------------Buscar Funcionário da Sangria
        private void btBuscar_Click(object sender, EventArgs e)
        {
            int n;
            bool ehUmNumero = int.TryParse(tbBuscarFuncionario.Text, out n);
            if (ehUmNumero == true)
            {
                objSangriaAlterada.funcionarioSangria = nFuncionario.BuscarFuncionarioPorCodigo(n);
                if (objSangriaAlterada.funcionarioSangria != null)
                {
                    this.tbBuscarFuncionario.Text = objSangriaAlterada.funcionarioSangria.nomeFuncionario; ;
                    dgvSangria.Focus();
                }
                else
                    tbBuscarFuncionario.Clear();
            }
            else
            {
                FrmSelecionarFuncionario frmSelecionarFuncionario = new FrmSelecionarFuncionario(tbBuscarFuncionario.Text);
                DialogResult resultado = frmSelecionarFuncionario.ShowDialog();

                if (resultado == DialogResult.OK)
                {

                    this.objSangriaAlterada.funcionarioSangria = frmSelecionarFuncionario.FuncionarioSelecionado;
                    this.tbBuscarFuncionario.Text = objSangriaAlterada.funcionarioSangria.nomeFuncionario;
                    dgvSangria.Focus();
                }

            }
        }

        private void tbBuscarFuncionario_Leave(object sender, EventArgs e)
        {
            if (tbBuscarFuncionario.Text == "")
            {
                tbBuscarFuncionario.Text = "Digite o nome do funcionário ...";
                pbFuncionario.Image = Properties.Resources.FuncionarioAzul;
                panelBuscarFuncionario.BackColor = Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76)))));
                tbBuscarFuncionario.ForeColor = Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76)))));

            }
        }

        private void tbBuscarFuncionario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btBuscar.PerformClick();
                e.Handled = true;
            }
        }

        private void tbBuscarFuncionario_Enter(object sender, EventArgs e)
        {
            tbBuscarFuncionario.Clear();
            pbFuncionario.Image = Properties.Resources.FuncionarioRosa;
            panelBuscarFuncionario.BackColor = Color.DeepPink;
        }

        //--------------------------Formulário
        private void FrmCancelamentoSangria_Load(object sender, EventArgs e)
        {
            //Pega no formulario da venda o UsuarioLogado
            if (FrmMenuPrincipal.userLogado != null)
            {
                objSangriaAlterada.funcionarioSangria = FrmMenuPrincipal.userLogado;
                tbBuscarFuncionario.Text = objSangriaAlterada.funcionarioSangria.nomeFuncionario;
            }

            metodoIniciaFormulario();
        }

        private void dgvSangria_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                //-----------Adiciona o formato apenas na caixa de texto 
                if (dgvSangria.CurrentCell.ColumnIndex == 1)
                {
                    {
                        caixaTextoGride = e.Control as TextBox;
                        caixaTextoGride.TextChanged -= new EventHandler(caixaTextoGride_TextChanged);
                        caixaTextoGride.TextChanged += caixaTextoGride_TextChanged;

                        caixaTextoGride.Leave -= new EventHandler(caixaTextoGride_Leave);
                        caixaTextoGride.Leave += caixaTextoGride_Leave;
                    }
                }
                else
                {

                    caixaTextoGride = new TextBox();

                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void dgvSangria_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //Valor da Parcial
            if (dgvSangria.Columns[e.ColumnIndex].Name == "valorParcialVenda")
            {
                dgvSangria.Rows[e.RowIndex].ErrorText = "";
                double newDouble;

                if (dgvSangria.Rows[e.RowIndex].IsNewRow) { return; }
                if (!double.TryParse(e.FormattedValue.ToString(),
                    out newDouble) || newDouble < 0)
                {
                    dgvSangria.Rows[e.RowIndex].ErrorText = "Informe o valor da Sangria";
                }

            }
        }

        //-----------------------------Data Gride Caixa de Texto
        //Evento TextoChanged do Gride
        private void caixaTextoGride_TextChanged(object sender, EventArgs e)
        {
            metodos.metodoMoedaTB(ref caixaTextoGride);
        }

        private void caixaTextoGride_Leave(object sender, EventArgs e)
        {
            //Pega valor da caixa de testo para atualizar juros
            double valorCaixaTexto = Convert.ToDouble(caixaTextoGride.Text);
            double valorCaixa = Convert.ToDouble(dgvSangria.CurrentRow.Cells[3].Value);

            if (valorCaixaTexto > valorCaixa)
            {
                FrmCaixaDialogo frmCaixaCad = new FrmCaixaDialogo("Sangria maior que Cancelamento",
                "Novo valor maior que valor já registrado: \n" + valorCaixa.ToString(),
                Properties.Resources.DialogParcial,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Ok", "",
                false);
                frmCaixaCad.ShowDialog();

                dgvSangria.CurrentRow.Cells[1].Value = valorCaixaTexto;
            }

            metodoAtualizaCancelamentoGrid();
            metodoCalculaTotais();

        }

        private void FrmCancelamentoSangria_KeyDown(object sender, KeyEventArgs e)
        {
            //atalho da tecla de atalho ESC
            if (e.KeyCode.Equals(Keys.Escape) == true)
            {
                btSair.PerformClick();
            }
            //atalho para o botão cadastrar
            else if (e.KeyCode.Equals(Keys.F10) == true)
            {
                btAlterar.PerformClick();
            }
            else if (e.KeyCode.Equals(Keys.F2) == true)
            {
                btExcluir.PerformClick();
            }
            else if (e.KeyCode.Equals(Keys.F5) == true)
            {
                btBuscar.PerformClick();
            }
        }
        //----------------------------Botões
        private void btExcluir_Click(object sender, EventArgs e)
        {
            if (nSangria.AlterarSangria(sangriaListaAlt) == true)
            {
                try
                {
                    DialogResult resposta;
                    //Criando Caixa de dialogo
                    FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Exclusão",
                    "Exclusão deseja excluir a sangria?",
                     Properties.Resources.DialogOK,
                     System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                     Color.White,
                     "Sim", "Não",
                     false);
                    resposta = frmCaixa.ShowDialog();
                    if (resposta == DialogResult.Yes)
                    {
                        if (nSangria.ExcluirSangria(sangriaLista, objSangriaAlterada.funcionarioSangria.codigoFuncionario, objSangriaAlterada.ordemSangra) == true)
                        {

                            resposta = new DialogResult();
                            //Criando Caixa de dialogo
                            frmCaixa = new FrmCaixaDialogo("Confirmação",
                            "Alteração Realizada com Sucesso! \r\n" +
                            "Deseja realizar impressão comprovante?",
                            Properties.Resources.DialogOK,
                            System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                            Color.White,
                            "Sim", "Não",
                            false);

                            resposta = frmCaixa.ShowDialog();
                            if (resposta == DialogResult.Yes)
                            {

                                //Imprimi o comprovante
                                this.DialogResult = DialogResult.Yes;
                            }
                            else
                            {

                                this.DialogResult = DialogResult.Yes;

                            }

                        }
                        else
                        {
                            MessageBox.Show("Erro ao excluir Sangria!", "Erro Exclusão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir Sangria!: " + ex.Message, "Erro Exclusão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void btAlterar_Click(object sender, EventArgs e)
        {
            try
            {
                if (objSangriaAlterada.funcionarioSangria == null)
                {

                    MessageBox.Show("Selecione o Funcionário que vai alterar a Sangria!", "Aviso Funcionário", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    tbBuscarFuncionario.Focus();
                }
                else
                {
                    if (dgvSangria.RowCount > 0)
                    {
                        if (metodoValidaSangria() == true)
                        {
                            metodoAddSangriaLista();//Adiciona itens a lista da sangria
                            //Lista Sangria é maior que zero realiza o cadastro
                            if (sangriaListaAlt.Count > 0)
                            {
                                if (nSangria.AlterarSangria(sangriaListaAlt) == true)
                                {
                                    DialogResult resposta;
                                    //Criando Caixa de dialogo
                                    FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Confirmação",
                                    "Alteração Realizada com Sucesso! \r\n" +
                                    "Deseja realizar impressão comprovante?",
                                    Properties.Resources.DialogOK,
                                    System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                                    Color.White,
                                    "Sim", "Não",
                                    false);

                                    resposta = frmCaixa.ShowDialog();
                                    if (resposta == DialogResult.Yes)
                                    {

                                        //Imprimi o comprovante
                                        this.DialogResult = DialogResult.OK;
                                    }

                                    this.DialogResult = DialogResult.OK;
                                    this.Close();

                                }
                                else//Cadastro Erro
                                {
                                    MessageBox.Show("Erro ao alterar Sangria!", "Erro Alteração", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }


                            }
                            else//Erro Lista de Sangrias
                            {
                                MessageBox.Show("Não existem items na Sangria!", "Erro Lista", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else//Erro Validação
                        {


                            FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Valores Sangria",
                            "Não existem valores para realizar a Sangria!",
                            Properties.Resources.DialogQuestion,
                            System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                            Color.White,
                            "Ok", "",
                            false);
                            frmCaixa.ShowDialog();

                            btExcluir.PerformClick();
                        }


                    }//Gride Vazio
                }//Funcionário Selecionado com Sucesso
            }//Captura de EXceção

            catch (Exception ex)
            {
                FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro",
                "Erro ao alterar a Sangria \r\n" + ex.Message,
                Properties.Resources.DialogErro,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Ok", "",
                false);

                frmCaixa.ShowDialog();
            }
        }

    }
}
