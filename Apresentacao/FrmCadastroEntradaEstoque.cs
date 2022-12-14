using Apresentacao.Formulas;
using Negocio;
using ObjetoTransferencia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Apresentacao
{
    public partial class FrmCadastroEntradaEstoque : Form
    {
        EntradaEstoque entradaEstoque = new EntradaEstoque();
        EntradaEstoque entradaEstoqueValida = new EntradaEstoque();
        NegEntradaEstoque nEntrada = new NegEntradaEstoque();

        ItemEntrada item = new ItemEntrada();
        ItemEntrada itemCodigoBarras = new ItemEntrada();
        NegItemEntrada nItem = new NegItemEntrada();
        ItemEntradaLista listaItems = new ItemEntradaLista();

        NegProduto nProduto = new NegProduto();
        NegProdutoCor nProdutoCor = new NegProdutoCor();
        NegFornecedor nFornecedor = new NegFornecedor();

        Metodos metodos = new Metodos();
        String LocalSolution;
        int codEntradaEstoque;

        //Variavel Booleana para idicar selecionado ou não selecionado  
        Boolean chkSelecao = false;
        Boolean ExcEnt = false;


        public FrmCadastroEntradaEstoque(EntradaEstoque objEntradaEstoque)
        {
            InitializeComponent();

          entradaEstoque = objEntradaEstoque;

        }

        //---------------------------Metodos
        private void AtualizarDataGrid()
        {
            this.dgvItemEntrada.Rows.Clear(); // Limpa todos os registros atuais no grid de funcionários.

            if (this.listaItems.Count > 0)
            {
                this.dgvItemEntrada.Rows.Add(this.listaItems.Count);
            }
            else
            {
                return;
            }

            int indice = 0;
            foreach (ItemEntrada item in this.listaItems)
            {
                this.dgvItemEntrada[1, indice].Value = item.codigoItemEntrada;
                this.dgvItemEntrada[2, indice].Value = item.ProdutoCor.Produto.codigoProduto;
                this.dgvItemEntrada[3, indice].Value = item.ProdutoCor.codigoProdutoCor;
                this.dgvItemEntrada[4, indice].Value = item.ProdutoCor.Produto.descricaoProduto;
                this.dgvItemEntrada[5, indice].Value = item.ProdutoCor.Produto.referenciaProduto;
                this.dgvItemEntrada[6, indice].Value = item.ProdutoCor.Produto.sexoProduto;
                this.dgvItemEntrada[7, indice].Value = item.ProdutoCor.Cor.nomeCor;
                this.dgvItemEntrada[8, indice].Value = item.ProdutoCor.Produto.Genero.nomeGenero;
                this.dgvItemEntrada[9, indice].Value = item.Tamanho.codigoTamanho;
                this.dgvItemEntrada[10, indice].Value = item.Tamanho.nometamanho;
                this.dgvItemEntrada[11, indice].Value = item.quantidadeItem;
                this.dgvItemEntrada[12, indice].Value = item.precoCustoItem;
                this.dgvItemEntrada[13, indice].Value = item.precoVendaItem;
                this.dgvItemEntrada[14, indice].Value = item.codigoBarrasItem;
                this.dgvItemEntrada[15, indice].Value = item.ProdutoCor.ImagemProduto;

                indice++;
            }

            dgvItemEntrada.Update();

        }

        public void metodoExibeImagemProduto()
        {
            if (dgvItemEntrada.Rows.Count > 0)
            {
                String imgProduto = (dgvItemEntrada.CurrentRow.Cells[15].Value).ToString();
                //Verifica se a imagem existe
                if (System.IO.File.Exists(LocalSolution + "\\Imagens\\" + imgProduto + ".jpeg") == true)
                {
                    //----------------Cria imagem para exibir 
                    Image img;
                    Bitmap img2;
                    img = (System.Drawing.Image.FromFile(LocalSolution + "\\Imagens\\" + imgProduto + ".jpeg"));
                    img2 = new Bitmap(img);
                    img.Dispose();
                    pbImagemProduto.Image = img2;
                }
                else
                {
                    pbImagemProduto.Image = global::Apresentacao.Properties.Resources.imgDefaut;
                }
            }

        }

        public void metodoIniciaFormulario()
        {
            //Buscal Local que a Solution Está sendo Executada no Computador
            LocalSolution = Environment.CurrentDirectory;
            // This will get the current PROJECT directory
            LocalSolution = Directory.GetParent(LocalSolution).Parent.Parent.FullName;

            
            listaItems = nItem.BuscarEntradaItemEntradaTemp(codEntradaEstoque);//CodigoEntradaEstoque          
            AtualizarDataGrid();

            if (listaItems.Count > 0)
            {
                metodoCalculaEntradaEstoque();
            }

        }

        public Boolean metodoValidaEntrada()
        {

            if (tbNotaFiscal.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Informe a Nota Fiscal!",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbNotaFiscal.Clear();
                tbNotaFiscal.Focus();

                return false;
            }
            else if (tbFornecedor.Text.Trim() == String.Empty || entradaEstoque.Fornecedor == null)
            {
                MessageBox.Show("Informe o Fornecedor do Produto!",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbFornecedor.Clear();
                tbFornecedor.Focus();

                return false;
            }
            else if (tbVolumeNotaFiscal.Text == String.Empty)
            {
                MessageBox.Show("Informe o volume de Produtos!",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbVolumeNotaFiscal.Clear();
                tbVolumeNotaFiscal.Focus();

                return false;
            }
            else if (mtbTotalNota.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Informe o Valor Total dos Produto!",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mtbTotalNota.Clear();
                mtbTotalNota.Focus();

                return false;
            }

            else
                return true;
        }

        public void metodoAtivaEntradaEstoque()
        {
            btEntrada.Text = "&F12 Voltar";
            tbProduto.Enabled = true;
            dgvItemEntrada.Enabled = true;
            pbProduto.Enabled = true;
            cbAtivarBarras.Enabled = true;
            btSelecionar.Enabled = true;
            btCadastrar.Enabled = true;

            tbNotaFiscal.Enabled = false;
            tbFornecedor.Enabled = false;
            pbFornecedor.Enabled = false;
            tbVolumeNotaFiscal.Enabled = false;
            mtbTotalNota.Enabled = false;
            dtpDataNotaFiscal.Enabled = false;
        }

        public void metodoDesativaEntradaEstoque()
        {
            tbProduto.Enabled = false;
            dgvItemEntrada.Enabled = false;
            pbProduto.Enabled = false;
            cbAtivarBarras.Enabled = false;
            cbAtivarBarras.Checked = false;
            tbQuantidadeBarras.Enabled = false;
            mtbPrecoBarras.Enabled = false;
            pbBuscarBarras.Enabled = false;
            pbLancar.Enabled = false;
            mtbPrecoVendaBarras.Enabled = false;
            btSelecionar.Enabled = false;
            btCadastrar.Enabled = false;

            tbProduto.Focus();
            tbCodigoBarras.Clear();
            tbQuantidadeBarras.Clear();
            mtbPrecoVendaBarras.Clear();
            tbProduto.Clear();
            tbCodigoBarras.Clear();


            tbNotaFiscal.Enabled = true;
            tbFornecedor.Enabled = true;
            pbFornecedor.Enabled = true;
            tbVolumeNotaFiscal.Enabled = true;
            mtbTotalNota.Enabled = true;
            dtpDataNotaFiscal.Enabled = true;
            pbImagemProduto.Image = global::Apresentacao.Properties.Resources.imgDefaut;
        }

        public void metodoBarrasAtivado()
        {
            itemCodigoBarras = new ItemEntrada();

            tbCodigoBarras.Enabled = true;
            tbQuantidadeBarras.Enabled = true;
            mtbPrecoBarras.Enabled = true;
            mtbPrecoVendaBarras.Enabled = true;
            pbLancar.Enabled = true;
            pbBuscarBarras.Enabled = true;

            tbProduto.Focus();
            tbCodigoBarras.Clear();
            tbQuantidadeBarras.Clear();
            mtbPrecoBarras.Clear();
            mtbPrecoVendaBarras.Clear();

            tbProduto.Enabled = false;
            pbProduto.Enabled = false;

            tbCodigoBarras.Focus();
        }

        public void metodoBarrasDesativado()
        {
            tbCodigoBarras.Enabled = false;
            tbQuantidadeBarras.Enabled = false;
            mtbPrecoBarras.Enabled = false;
            mtbPrecoVendaBarras.Enabled = false;

            pbLancar.Enabled = false;
            pbBuscarBarras.Enabled = false;
            tbProduto.Focus();
            tbCodigoBarras.Clear();
            tbQuantidadeBarras.Clear();
            mtbPrecoBarras.Clear();
            mtbPrecoVendaBarras.Clear();

            tbProduto.Enabled = true;
            pbProduto.Enabled = true;
        }

        public void metodoCalculaEntradaEstoque()
        {

            double valorTotalCusto = 0;
            double valorTotalVenda = 0;
            double quantidadeTotal = 0;

            //faz a soma dos totais dos valores do gride
            foreach (DataGridViewRow col in dgvItemEntrada.Rows)
            {
                quantidadeTotal = quantidadeTotal + Convert.ToDouble(col.Cells[11].Value);
                valorTotalCusto = valorTotalCusto + (Convert.ToDouble(col.Cells[12].Value) * Convert.ToDouble(col.Cells[11].Value));
                valorTotalVenda = valorTotalVenda + (Convert.ToDouble(col.Cells[13].Value) * Convert.ToDouble(col.Cells[11].Value));
            }

            tbCustoTotal.Text = (valorTotalCusto * 100).ToString();
            tbVendaTotal.Text = (valorTotalVenda * 100).ToString();
            tbQuantidadeTotal.Text = quantidadeTotal.ToString();

            metodos.metodoMoedaTB(ref tbVendaTotal);
            metodos.metodoMoedaTB(ref tbCustoTotal);
        }

        public Boolean metodoValidaTotalNF()
        {
            if (tbVolumeNotaFiscal.Text == tbQuantidadeTotal.Text && mtbTotalNota.Text == tbCustoTotal.Text)
            {

                return true;
            }
            else
            {

                DialogResult resposta;
                //Criando Caixa de dialogo
                FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Correção Nota Fiscal",
                "Valores NF e Lançamento estão diferentes! \n" +
                "Deseja ajustar os valores da NF?",
                Properties.Resources.DialogErro,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Sim", "Não",
                false);

                resposta = frmCaixa.ShowDialog();
                if (resposta == DialogResult.Yes)
                {
                    tbVolumeNotaFiscal.Text = tbQuantidadeTotal.Text;
                    mtbTotalNota.Text = tbCustoTotal.Text;
                    //-------Preenche objeto Entrada Estoque
                    entradaEstoque.quantidadeProdutosNota = Convert.ToInt32(tbVolumeNotaFiscal.Text);
                    entradaEstoque.valorTotalNota = Convert.ToDouble(mtbTotalNota.Text);
                    if (nEntrada.AtualizarEntradaEstoque(entradaEstoque) == true)
                    {

                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        //------------------------------------Caixas de Texto
        private void mtbTotalNota_TextChanged(object sender, EventArgs e)
        {
            metodos.metodoMoedaMTB(ref mtbTotalNota);
        }

        private void tbVolumeNotaFiscal_KeyPress(object sender, KeyPressEventArgs e)
        {
            metodos.metodoAllowNumber(e);
        }

        private void tbFornecedor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                pbFornecedor_Click(null, null);
                e.Handled = true;
            }
        }

        private void tbProduto_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == 13)
                {
                    int n;
                    bool ehUmNumero = int.TryParse(tbProduto.Text, out n);
                    if (ehUmNumero == true)
                    {
                        item.ProdutoCor = new ProdutoCor();
                        item.ProdutoCor.Produto = nProduto.BuscarProdutoPorCodigo(n);
                        if (item.ProdutoCor.Produto != null)
                        {
                            FrmProdutoCorEntrada frmProdutoCor = new FrmProdutoCorEntrada(item.ProdutoCor.Produto, codEntradaEstoque, "Cadastrar");
                            DialogResult resposta;


                            resposta = frmProdutoCor.ShowDialog();

                            if (resposta == DialogResult.Yes)
                            {
                                tbProduto.Clear();
                                metodoIniciaFormulario();
                            }
                            e.Handled = true;
                        }
                        else
                        {
                            pbProduto_Click(null, null);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        item.ProdutoCor = new ProdutoCor();
                        item.ProdutoCor.Produto = nProduto.BuscarProdutoPorReferencia(tbProduto.Text);
                        if (item.ProdutoCor.Produto != null)
                        {
                            FrmProdutoCorEntrada frmProdutoCor = new FrmProdutoCorEntrada(item.ProdutoCor.Produto, codEntradaEstoque, "Cadastrar");
                            DialogResult resposta;


                            resposta = frmProdutoCor.ShowDialog();

                            if (resposta == DialogResult.Yes)
                            {
                                tbProduto.Clear();
                                metodoIniciaFormulario();
                            }
                            e.Handled = true;
                        }
                        else
                        {
                            pbProduto_Click(null, null);
                            e.Handled = true;
                        }
                    }

                    if (dgvItemEntrada.RowCount > 0)
                    {
                        this.dgvItemEntrada.CurrentRow.Selected = true;
                    }
                    metodoExibeImagemProduto();
                }
            }
            catch (Exception ex)
            {
                FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro",
                "Erro ao selecionar Produto \r\n" + ex.Message,
                Properties.Resources.DialogErro,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Ok", "",
                false);
                frmCaixa.ShowDialog();
            }
        }

        private void tbQuantidadeBarras_KeyPress(object sender, KeyPressEventArgs e)
        {
            metodos.metodoAllowNumber(e);

        }

        private void mtbPrecoBarras_TextChanged(object sender, EventArgs e)
        {
            metodos.metodoMoedaMTB(ref mtbPrecoBarras);
        }

        private void mtbPrecoVendaBarras_TextChanged(object sender, EventArgs e)
        {
            metodos.metodoMoedaMTB(ref mtbPrecoVendaBarras);
        }

        private void tbCodigoBarras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (itemCodigoBarras.ProdutoCor == null)
                {
                    pbBuscarBarras_Click(null, null);
                    e.Handled = true;
                }
                else
                {
                    pbLancar_Click(null, null);
                    e.Handled = true;
                }
            }
            else if (e.KeyChar == 27)
            {

                metodoBarrasDesativado();
                cbAtivarBarras.Checked = false;
                e.Handled = true;
            }
        }

        //--------------------------------------Botões
        private void btEntrada_Click(object sender, EventArgs e)
        {
            if (metodoValidaEntrada() == true)
            {
                //---------------------------------Realiza alteração dos dados da Entrada de Estoque
                if (btEntrada.Text == "&F12 Alterar")
                {
                    DialogResult resposta;
                    //Criando Caixa de dialogo
                    FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Alteração",
                    " Deseja realmente Alterar os dados da Entrada de Estoque ?",
                    Properties.Resources.Alterar,
                    System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                    Color.White,
                    "Confirmar", "Cancelar",
                    false);

                    //Verifica se o usuário realmente quer alterar os dados do cliente.
                    resposta = frmCaixa.ShowDialog();
                    if (resposta == DialogResult.Yes)
                    {
                        //-------Preenche objeto Entrada Estoque
                        entradaEstoque.notaFiscal = tbNotaFiscal.Text;
                        entradaEstoque.quantidadeProdutosNota = Convert.ToInt32(tbVolumeNotaFiscal.Text);
                        entradaEstoque.valorTotalNota = Convert.ToDouble(mtbTotalNota.Text);
                        entradaEstoque.dataEntrada = dtpDataNotaFiscal.Value;

                        if (nEntrada.AtualizarEntradaEstoque(entradaEstoque) == true)
                        {
                            metodoAtivaEntradaEstoque();
                            metodoIniciaFormulario();//CodigoEntradaEstoque para Buscar Itens
                            metodoExibeImagemProduto();
                            tbProduto.Focus();
                        }
                    }
                    else
                    {
                        metodoAtivaEntradaEstoque();
                        metodoIniciaFormulario();//CodigoEntradaEstoque para Buscar Itens
                        metodoExibeImagemProduto();
                        tbProduto.Focus();
                    }
                }
                else if (btEntrada.Text == "&F12 Voltar")
                {

                    btEntrada.Text = "&F12 Alterar";
                    metodoDesativaEntradaEstoque();
                }
                //----------------------------------Primeira Entrada de Estoque  ou  Já Existente
                else
                {
                    //-------------Valida se já não existe a mesma entrada de estoque no banco
                    entradaEstoqueValida = nEntrada.ValidaEntradaEstoque(tbNotaFiscal.Text, entradaEstoque.Fornecedor.codigoFornecedor);
                    //----------------Se entrada já existir preenche os campos da entrada
                    if (entradaEstoqueValida != null)
                    {
                        //Criando Caixa de dialogo
                        FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Entrada de Estoque já Existe!",
                        " NF: " + entradaEstoqueValida.notaFiscal +
                        "\n Fornecedor: " + entradaEstoqueValida.Fornecedor.nomeFantasiaFornecedor +
                        "\n Data: " + entradaEstoqueValida.dataEntrada.ToShortDateString(),
                        Properties.Resources.DialogProcessando,
                        System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                        Color.White,
                        "Ok", "",
                        false);
                        frmCaixa.ShowDialog();

                        //Preenche os campos com a os dados já existentes da Entrada de Estoque
                        tbVolumeNotaFiscal.Text = entradaEstoqueValida.quantidadeProdutosNota.ToString();
                        mtbTotalNota.Text = (entradaEstoqueValida.valorTotalNota * 100).ToString();
                        dtpDataNotaFiscal.Value = entradaEstoqueValida.dataEntrada;
                        entradaEstoque = entradaEstoqueValida;

                        //Ativa os campos e preenche os dgv com os Itens
                        codEntradaEstoque = entradaEstoqueValida.codigoEntradaEstoque;
                        metodoAtivaEntradaEstoque();
                        metodoIniciaFormulario();//CodigoEntradaEstoque Buscar Itens
                        metodoExibeImagemProduto();
                        tbProduto.Focus();
                    }
                    else
                    {
                        //----------Realiza o Cadastro
                        if (btEntrada.Text == "&F12 Entrada")
                        {
                            //-------Preenche objeto Entrada Estoque
                            entradaEstoque.notaFiscal = tbNotaFiscal.Text;
                            entradaEstoque.quantidadeProdutosNota = Convert.ToInt32(tbVolumeNotaFiscal.Text);
                            entradaEstoque.valorTotalNota = Convert.ToDouble(mtbTotalNota.Text);
                            entradaEstoque.dataEntrada = dtpDataNotaFiscal.Value;

                            if (nEntrada.CadastrarEntradaEstoque(entradaEstoque) == true)
                            {
                                codEntradaEstoque = nEntrada.BuscarUltimoRegistro();
                                entradaEstoque.codigoEntradaEstoque = codEntradaEstoque;

                                metodoAtivaEntradaEstoque();
                                metodoIniciaFormulario();//CodigoEntradaEstoque para Buscar Itens
                                metodoExibeImagemProduto();
                                tbProduto.Focus();
                            }
                        }
                    }
                }
            }
        }

        private void pbProduto_Click(object sender, EventArgs e)
        {
            FrmSelecionarProdutoEstoque frmSelecionarProduto = new FrmSelecionarProdutoEstoque(codEntradaEstoque, tbProduto.Text, "Cadastrar");
            DialogResult resposta = frmSelecionarProduto.ShowDialog();

            if (resposta == DialogResult.Yes)
            {
                metodoIniciaFormulario();
            }
        }

        private void pbFornecedor_Click(object sender, EventArgs e)
        {
            int n;
            bool ehUmNumero = int.TryParse(tbFornecedor.Text, out n);
            if (ehUmNumero == true)
            {
                entradaEstoque.Fornecedor = nFornecedor.BuscarFornecedorPorCodigo(n);
                if (entradaEstoque.Fornecedor != null)
                {
                    this.tbFornecedor.Text = entradaEstoque.Fornecedor.nomeFantasiaFornecedor;
                    tbVolumeNotaFiscal.Focus();
                }
                else
                    tbFornecedor.Clear();
            }
            else
            {
                FrmSelecionarFornecedor frmSelecionarFornecedor = new FrmSelecionarFornecedor(tbFornecedor.Text);
                DialogResult resultado = frmSelecionarFornecedor.ShowDialog();

                if (resultado == DialogResult.OK)
                {

                    this.entradaEstoque.Fornecedor = frmSelecionarFornecedor.FornecedorSelecionado;
                    this.tbFornecedor.Text = entradaEstoque.Fornecedor.nomeFantasiaFornecedor;
                    tbVolumeNotaFiscal.Focus();
                }

            }
        }

        private void btSelecionar_Click(object sender, EventArgs e)
        {
            try
            {
                int n = Convert.ToInt32(dgvItemEntrada.CurrentRow.Cells[3].Value);
                item.ProdutoCor = nProdutoCor.BuscarProdutoCorPorCodigo(n);
                if (item.ProdutoCor != null)
                {
                    FrmItemEntradaTemp frmItemEntrada = new FrmItemEntradaTemp(item.ProdutoCor, codEntradaEstoque);
                    DialogResult resposta;

                    resposta = frmItemEntrada.ShowDialog();

                    if (resposta == DialogResult.Yes)
                    {
                        metodoIniciaFormulario();
                    }
                }
            }
            catch (Exception ex)
            {
                FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro",
                "Erro ao selecionar Produto \r\n" + ex.Message,
                Properties.Resources.DialogErro,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Ok", "",
                false);
                frmCaixa.ShowDialog();
            }

        }

        private void btCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvItemEntrada.RowCount > 0)
                {
                    if (metodoValidaTotalNF() == true)
                    {
                        if (nItem.CadastrarItemEntradaLista(listaItems) == true)
                        {          
                            //----------Limpa tabela temporária
                            nItem.ExcluirDadosTabelaTemporaria(codEntradaEstoque);
                            dgvItemEntrada.Rows.Clear();

                            DialogResult resposta;
                            //Criando Caixa de dialogo
                            FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Confirmação - Lançamento",
                            "Lançamento Realizado com Sucesso! \r\n",
                            Properties.Resources.DialogOK,
                            System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                            Color.White,
                            "Sim", "",
                            false);

                            resposta = frmCaixa.ShowDialog();
                            if (resposta != DialogResult.OK)
                            {
                                entradaEstoque.codigoEntradaEstoque = 0;
                                this.DialogResult = DialogResult.Yes;
                                this.Close();
                            }
                        }
                        else
                        {
                            FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro",
                            "Erro ao cadastrar Itens da Entrada de Estoque  \r\n",
                             Properties.Resources.DialogErro,
                             System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                             Color.White,
                             "Ok", "",
                             false);
                            frmCaixa.ShowDialog();

                        }
                    }
                }
                else
                {
                    FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro",
                    "Não Existem itens para o Lançamento  \r\n",
                     Properties.Resources.DialogErro,
                     System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                     Color.White,
                     "Ok", "",
                    false);
                    frmCaixa.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro",
                "Erro Entrada de Estoque: \r\n" + ex.Message,
                Properties.Resources.DialogErro,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Ok", "",
                false);

                frmCaixa.ShowDialog();
            }
        }

        private void btSair_Click(object sender, EventArgs e)
        {
            DialogResult resposta;
            //Criando Caixa de dialogo
            FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Confirmação",
            " Deseja realmente sair do Cadastro de Entrada de Estoque ?",
            Properties.Resources.DialogQuestion,
            System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
            Color.White,
            "Sim", "Não",
            false);

            resposta = frmCaixa.ShowDialog();
            if (resposta == DialogResult.Yes)
            {
                this.Close();

            }
        }

        private void cbAtivarBarras_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAtivarBarras.Checked == true)
            {
                metodoBarrasAtivado();
            }
            else
            {
                metodoBarrasDesativado();
            }
        }

        private void pbBuscarBarras_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbCodigoBarras.Text == String.Empty)
                {
                    FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro Código de Barras",
                    "Informe o código de Barras do Produto",
                    Properties.Resources.DialogErro,
                    System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                    Color.White,
                    "Ok", "",
                    false);
                    frmCaixa.ShowDialog();
                }
                else
                {
                    itemCodigoBarras = new ItemEntrada();
                    itemCodigoBarras = nItem.BuscarItemPorBarras(tbCodigoBarras.Text);
                    if (itemCodigoBarras != null)
                    {
                        tbProduto.Text = itemCodigoBarras.ProdutoCor.Produto.descricaoProduto;
                        mtbPrecoBarras.Text = (itemCodigoBarras.precoCustoItem * 100).ToString();
                        mtbPrecoVendaBarras.Text = (itemCodigoBarras.precoVendaItem * 100).ToString();
                        tbQuantidadeBarras.Text = "1";

                    }
                    else
                    {
                        FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro Código de Barras",
                        "Código de Barras não existe!",
                         Properties.Resources.DialogErro,
                         System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                         Color.White,
                         "Ok", "",
                        false);
                        frmCaixa.ShowDialog();


                    }

                }
            }
            catch (Exception) {
                FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro Código de Barras",
                "Código de Barras não existe!",
                Properties.Resources.DialogErro,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Ok", "",
                false);
                frmCaixa.ShowDialog();        
            }
        }

        private void pbLancar_Click(object sender, EventArgs e)
        {
            if (tbCodigoBarras.Text == String.Empty || tbQuantidadeBarras.Text == String.Empty || mtbPrecoBarras.Text == String.Empty || itemCodigoBarras.ProdutoCor == null)
            {
                FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro Código de Barras",
                "Informe o código de Barras do Produto",
                Properties.Resources.DialogErro,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Ok", "",
                false);
                frmCaixa.ShowDialog();
            }
            else
            {
                itemCodigoBarras.EntradaEstoque = new EntradaEstoque();
                itemCodigoBarras.EntradaEstoque.codigoEntradaEstoque = codEntradaEstoque;
                itemCodigoBarras.quantidadeItem = Convert.ToInt32(tbQuantidadeBarras.Text);
                itemCodigoBarras.precoCustoItem = Convert.ToDouble(mtbPrecoBarras.Text);
                itemCodigoBarras.precoVendaItem = Convert.ToDouble(mtbPrecoVendaBarras.Text);
                if (itemCodigoBarras.quantidadeItem <= 0)
                {
                    itemCodigoBarras.quantidadeItem = 1;
                }

                if (nItem.CadastrarItemEntradaBarrasTemp(itemCodigoBarras) == true)
                {
                    FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Confirmação Código de Barras",
                    "Produto Adicionado com sucesso!",
                    Properties.Resources.DialogOK,
                    System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                    Color.White,
                    "Ok", "",
                    false);
                    frmCaixa.ShowDialog();

                    metodoBarrasAtivado();
                    metodoIniciaFormulario();
                }
                else
                {
                    FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro Código de Barras",
                    "Erro ao Adicionar o produto!",
                     Properties.Resources.DialogErro,
                     System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                     Color.White,
                     "Ok", "",
                     false);
                    frmCaixa.ShowDialog();
                }
            }
        }

        //--------------------------------------Formulário
        private void FrmCadastroEntradaEstoque_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ExcEnt == false)
            {
                //se houver entrada de estoque ele realiza pergunta se deseja mantela ou excluila
                if (entradaEstoque.codigoEntradaEstoque != 0)
                {
                    DialogResult resposta;
                    //Criando Caixa de dialogo
                    FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Aviso - Lançamento Pendente",
                    " Deseja manter Entrada de Estoque Pendente?",
                    Properties.Resources.DialogWarning,
                    System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                    Color.White,
                    "Sim", "Não",
                    false);

                    resposta = frmCaixa.ShowDialog();
                    if (resposta == DialogResult.Yes)
                    {
                        DialogResult = DialogResult.Yes;

                    }
                    else if (resposta == DialogResult.Cancel)
                    {
                        if (dgvItemEntrada.RowCount > 0)
                        {
                            //limpa dados pendentes
                            nItem.ExcluirDadosTabelaTemporaria(codEntradaEstoque);//CodigoEntradaEstoque
                        }
                        nEntrada.ExcluirEntradaEstoque(entradaEstoque);
                        DialogResult = DialogResult.Yes;
                    }
                }
            }
        }

        private void FrmCadastroEntradaEstoque_Load(object sender, EventArgs e)
        {
            if (entradaEstoque != null)
            {
                tbNotaFiscal.Text = entradaEstoque.notaFiscal;
                tbFornecedor.Text = entradaEstoque.Fornecedor.nomeFantasiaFornecedor;
                tbVolumeNotaFiscal.Text = entradaEstoque.quantidadeProdutosNota.ToString();
                mtbTotalNota.Text = (entradaEstoque.valorTotalNota * 100).ToString();
                btEntrada.Text = "F12 Voltar";

                metodoAtivaEntradaEstoque();
                codEntradaEstoque = entradaEstoque.codigoEntradaEstoque;

                metodoIniciaFormulario();

            }
            else
            {
                entradaEstoque = new EntradaEstoque();
                metodoDesativaEntradaEstoque();
            }
        }

        private void dgvItemEntrada_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            metodoExibeImagemProduto();
        }

        private void dgvItemEntrada_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btSelecionar.PerformClick();
        }

        private void FrmCadastroEntradaEstoque_KeyDown(object sender, KeyEventArgs e)
        {
            //atalho da tecla de atalho ESC
            if (e.KeyCode.Equals(Keys.Escape) == true)
            {
                btSair.PerformClick();
            }
            //atalho para o botão cadastrar
            else if (e.KeyCode.Equals(Keys.F10) == true)
            {
                btCadastrar.PerformClick();
            }
            else if (e.KeyCode.Equals(Keys.F2) == true)
            {
                btSelecionar.PerformClick();
            }
            else if (e.KeyCode.Equals(Keys.F12) == true)
            {
                btEntrada.PerformClick();
            }
        }

        private void dgvItemEntrada_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvItemEntrada.Columns[e.ColumnIndex].Name == "marcarItem")
            {
                if (chkSelecao == false)
                {
                    chkSelecao = true;
                    foreach (DataGridViewRow dtr in dgvItemEntrada.Rows)
                    {
                        ((DataGridViewCheckBoxCell)dtr.Cells[0]).Value = true;
                    }
                }
                else
                {
                    chkSelecao = false;
                    foreach (DataGridViewRow dtr in dgvItemEntrada.Rows)
                    {
                        ((DataGridViewCheckBoxCell)dtr.Cells[0]).Value = false;
                    }

                }

                btExcluir.Focus();
            }

        }

        private void btExcluir_Click(object sender, EventArgs e)
        {
            Boolean validaSelecao = false;//Verifica se o gride tem algum item selecionado para realizar exclusão

            //percorre todas as linhas do gride
            foreach (DataGridViewRow check in dgvItemEntrada.Rows)
            {
                //pega valores cheked no gride
                if ((bool)check.Cells[0].FormattedValue)
                {
                    //Código item Entrada 
                    int codigoItem = int.Parse(check.Cells[1].Value.ToString());
                    validaSelecao = true;

                    if (nItem.ExcluirItemEntradaTemp(codigoItem) == false)
                    {
                        FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Erro - Exclusão",
                        "Descrição :" + check.Cells[2].Value.ToString() + " " + check.Cells[4].Value.ToString()
                        + "\r\n Referência :" + check.Cells[5].Value.ToString()
                        + "\r\n Não foi possível excluir, Itens possuem movimentação!",
                        Properties.Resources.DialogErro,
                        System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                        Color.White,
                        "Ok", "",
                        true);

                        //Se usuário desejar realizar exclusão da entrada de estoque
                        frmCaixa.ShowDialog();
                    }
                }
            }

            //---Atualiza o gride--\\
            metodoIniciaFormulario();

            //Se a tabela não triver itens pergunta para o usuário se ele deseja excluir a entrada de estoque
            if (dgvItemEntrada.Rows.Count > 0 && validaSelecao == false)
            {
                DialogResult resposta;
                //Criando Caixa de dialogo
                FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Confirmação - Exclusão",
                "Deseja realizar a exclusão dos Itens da Entrada de Estoque? \r\n",
                Properties.Resources.DialogProcessando,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Sim", "Não",
                false);

                //Se usuário desejar realizar exclusão da entrada de estoque
                resposta = frmCaixa.ShowDialog();
                if (resposta == DialogResult.Yes)
                {

                    if (nItem.ExcluirItemEntradaTempPorEntrada(listaItems) == true)
                    {
                        DialogResult respostaExclusao;
                        //Criando Caixa de dialogo
                        frmCaixa = new FrmCaixaDialogo("Informação - Exclusão",
                        "Itens da Entrada de Estoque Excluidos com Sucesso! \r\n",
                        Properties.Resources.DialogWarning,
                        System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                        Color.White,
                        "Sim", "",
                        false);
                        respostaExclusao = frmCaixa.ShowDialog();
                        if (respostaExclusao == DialogResult.Yes)
                        {
                            metodoIniciaFormulario();
                        }
                    }
                    else
                    {
                        DialogResult respostaExclusao;
                        //Criando Caixa de dialogo
                        frmCaixa = new FrmCaixaDialogo("Erro - Exclusão",
                        "Não foi possível excluir entrada de estoque! \r\n" +
                        "Existem itens com estoque atendido!",
                        Properties.Resources.DialogErro,
                        System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                        Color.White,
                        "OK", "",
                        false);

                        respostaExclusao = frmCaixa.ShowDialog();
                        metodoIniciaFormulario();
                    }
                }
            }

            //Se a tabela não triver itens pergunta para o usuário se ele deseja excluir a entrada de estoque
            if (dgvItemEntrada.Rows.Count == 0)
            {
                DialogResult resposta;
                //Criando Caixa de dialogo
                FrmCaixaDialogo frmCaixa = new FrmCaixaDialogo("Confirmação - Exclusão",
                "Deseja realizar a exclusão da Entrada de Estoque? \r\n",
                Properties.Resources.DialogQuestion,
                System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                Color.White,
                "Sim", "Não",
                false);

                //Se usuário desejar realizar exclusão da entrada de estoque
                resposta = frmCaixa.ShowDialog();
                if (resposta == DialogResult.Yes)
                {

                    if (nEntrada.ExcluirEntradaEstoque(entradaEstoque) == true)
                    {
                        DialogResult respostaExclusao;
                        //Criando Caixa de dialogo
                        frmCaixa = new FrmCaixaDialogo("Informação - Exclusão",
                        "Exclusão realizada com sucesso! \r\n",
                        Properties.Resources.DialogOK,
                        System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                        Color.White,
                        "Sim", "",
                        false);
                        respostaExclusao = frmCaixa.ShowDialog();
                        if (respostaExclusao == DialogResult.Yes)
                        {
                            ExcEnt = true;//Variavel indica que Entrada de estoque ja foi Excluida
                            this.DialogResult = DialogResult.Yes;
                            this.Close();
                        }
                    }
                    else
                    {
                        DialogResult respostaExclusao;
                        //Criando Caixa de dialogo
                        frmCaixa = new FrmCaixaDialogo("Erro - Exclusão",
                        "Não foi possível excluir entrada de estoque! \r\n",
                        Properties.Resources.DialogErro,
                        System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76))))),
                        Color.White,
                        "OK", "",
                        false);

                        respostaExclusao = frmCaixa.ShowDialog();
                    }
                }
            }
        }

    }
}
