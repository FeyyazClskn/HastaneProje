using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Hastane_Proje
{
    public partial class FrmDoktorDetay : Form
    {
        public FrmDoktorDetay()
        {
            InitializeComponent();
        }

        sqlbaglanti bgl = new sqlbaglanti();

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public string doktortc;

        private void FrmDoktorDetay_Load(object sender, EventArgs e)
        {
            LblTc.Text = doktortc;

            // ad soyad çekme
            SqlCommand komut = new SqlCommand("Select Doktorad,Doktorsoyad From Tbl_Doktorlar where Doktortc=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", doktortc);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                LblAdsoyad.Text = dr[0].ToString() + " " + dr[1].ToString();
            }
            bgl.baglanti().Close();

            // doktorun aktif randevuları
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Tbl_Randevular WHERE Randevudoktor = @doktorAd", bgl.baglanti());
            da.SelectCommand.Parameters.AddWithValue("@doktorAd", LblAdsoyad.Text);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;

        }

        private void BtnBilgiDuzenle_Click(object sender, EventArgs e)
        {
            FrmDoktorGuncelle frmdoktorguncelle = new FrmDoktorGuncelle();
            frmdoktorguncelle.doktortc = LblTc.Text;
            frmdoktorguncelle.Show();
        }

        private void BtnDuyuru_Click(object sender, EventArgs e)
        {
            FrmDuyurular frm = new FrmDuyurular();
            frm.Show();
        }

        private void BtnCıkıs_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // şikayetleri richtextboxa çekme
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Seçilen satırın indeksini al
            int selectedRowIndex = e.RowIndex;

            // Eğer seçilen satır geçerli değilse veya hiçbir satır seçilmediyse, işlemi durdur
            if (selectedRowIndex < 0 || selectedRowIndex >= dataGridView1.Rows.Count)
                return;

            // Seçilen satırdaki hücreyi al
            DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];

            // Eğer seçilen satırın hücresi geçerli değilse, işlemi durdur
            if (selectedRow.Cells.Count < 8) // Örnek olarak, şikayetin bulunduğu hücrenin indeksi 7 olacak şekilde kabul ediyoruz
                return;

            // Seçilen satırdaki şikayet hücresinin değerini al
            object sikayetCellValue = selectedRow.Cells[7].Value;

            // Eğer hücre değeri null değilse, değeri RchSikayet.Text özelliğine ata
            if (sikayetCellValue != null)
                RchSikayet.Text = sikayetCellValue.ToString();
            else
                RchSikayet.Text = ""; // Eğer hücre değeri null ise, Richtextbox'ı boşalt
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Eğer hiçbir satır seçilmediyse, işlemi durdur
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen satırı al
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

            // Seçilen satırdaki hücreyi kontrol edin
            if (selectedRow.Cells.Count >= 8) // Örnek olarak, şikayetin bulunduğu hücrenin indeksi 7 olacak şekilde kabul ediyoruz
            {
                // Seçilen satırdaki şikayet hücresinin değerini al
                object sikayetCellValue = selectedRow.Cells[7].Value;

                // Eğer hücre değeri null değilse, değeri RchSikayet.Text özelliğine ata
                if (sikayetCellValue != null)
                    RchSikayet.Text = sikayetCellValue.ToString();
                else
                    RchSikayet.Text = ""; // Eğer hücre değeri null ise, Richtextbox'ı boşalt
            }
            else
            {
                MessageBox.Show("Seçilen satırın şikayet bilgisi bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
