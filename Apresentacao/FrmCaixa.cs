using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Apresentacao
{
    public partial class FrmCaixa : Form
    {
        public FrmCaixa()
        {
            InitializeComponent();
        }

        private void btVenda_Click(object sender, EventArgs e)
        {
            FrmVendaPendente frmVendaPendente = new FrmVendaPendente();
            frmVendaPendente.ShowDialog();

        }

        private void pbImagemProduto_Click(object sender, EventArgs e)
        {

        }

        private void FrmCaixa_Load(object sender, EventArgs e)
        {

        }

    }
}
