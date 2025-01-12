﻿using System;
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
    public partial class FrmHastaDetay : Form
    {
        public FrmHastaDetay()
        {
            InitializeComponent();
        }

        sqlbaglanti bgldetay = new sqlbaglanti();

        public string hastatc;

        void randevugecmislistele()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Tbl_Randevular WHERE Randevuhastatc = @p1", bgldetay.baglanti()))
                {
                    da.SelectCommand.Parameters.AddWithValue("@p1", LblTc.Text);
                    da.Fill(dt);
                }
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Randevu listesi çekilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmHastaDetay_Load(object sender, EventArgs e)
        {
            LblTc.Text = hastatc;

            // Ad Soyad çekme
            SqlCommand komut = new SqlCommand("Select Hastaad,Hastasoyad From Tbl_Hastalar where Hastatc=@d1", bgldetay.baglanti());
            komut.Parameters.AddWithValue("@d1", LblTc.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                LblAdsoyad.Text = dr[0].ToString() + " " + dr[1].ToString();
            }
            bgldetay.baglanti().Close();

            randevugecmislistele();

            // Branşları comboboxa aktarma
            SqlCommand komut2 = new SqlCommand("Select Bransad From Tbl_Branslar", bgldetay.baglanti());
            SqlDataReader dr2 = komut2.ExecuteReader();
            while (dr2.Read())
            {
                CmbBrans.Items.Add(dr2[0].ToString());
            }
            bgldetay.baglanti().Close();
        }


        // Branş seçtikten sonra combobox'a o branşdaki doktorları ekledik
        private void CmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            Txtid.Text = "";
            CmbDoktor.Text = "";
            CmbDoktor.Items.Clear();

            SqlCommand komut3 = new SqlCommand("Select Doktorad,Doktorsoyad From Tbl_Doktorlar where Doktorbrans=@b1", bgldetay.baglanti());
            komut3.Parameters.AddWithValue("@b1", CmbBrans.Text);
            SqlDataReader dr3 = komut3.ExecuteReader();
            while (dr3.Read())
            {
                CmbDoktor.Items.Add(dr3[0].ToString() + " " + dr3[1].ToString());
            }
            bgldetay.baglanti().Close();
        }

        // aktif randevular
        private void CmbDoktor_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("Select * From Tbl_Randevular where Randevubrans='" + CmbBrans.Text + "'" + " and Randevudoktor='" + CmbDoktor.Text + "' and Randevudurum=0", bgldetay.baglanti());
            da.Fill(dt);
            dataGridView2.DataSource = dt;
        }

        private void LnkGuncelle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmHastaGuncelle frmguncelle = new FrmHastaGuncelle();
            frmguncelle.tcno = LblTc.Text;
            frmguncelle.Show();
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView2.SelectedCells[0].RowIndex;

            Txtid.Text = dataGridView2.Rows[secilen].Cells[0].Value.ToString();
        }

        private void BtnRandevu_Click(object sender, EventArgs e)
        {
            if (CmbDoktor.Text == "" || CmbBrans.Text == "" || RchSikayet.Text == "")
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    SqlCommand komutrandevu = new SqlCommand("INSERT INTO Tbl_Randevular (Randevutarih, Randevusaat, Randevubrans, Randevudoktor, Randevudurum, Randevuhastatc, Randevuhastasikayet) VALUES (@tarih, @saat, @brans, @doktor, @durum, @hastatc, @sikayet); SELECT SCOPE_IDENTITY()", bgldetay.baglanti());
                    komutrandevu.Parameters.AddWithValue("@tarih", DateTime.Now.ToShortDateString());
                    komutrandevu.Parameters.AddWithValue("@saat", DateTime.Now.ToShortTimeString());
                    komutrandevu.Parameters.AddWithValue("@brans", CmbBrans.Text);
                    komutrandevu.Parameters.AddWithValue("@doktor", CmbDoktor.Text);
                    komutrandevu.Parameters.AddWithValue("@durum", 0); // Yeni randevu olduğu için durumu 0 olarak ayarlıyoruz
                    komutrandevu.Parameters.AddWithValue("@hastatc", LblTc.Text);
                    komutrandevu.Parameters.AddWithValue("@sikayet", RchSikayet.Text);

                    int yeniRandevuId = Convert.ToInt32(komutrandevu.ExecuteScalar());
                    bgldetay.baglanti().Close();

                    MessageBox.Show("Randevu başarıyla alındı. Randevu ID'si: " + yeniRandevuId.ToString(), "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Ekranı temizleme
                    CmbBrans.Text = "";
                    CmbDoktor.Text = "";
                    RchSikayet.Text = "";

                    // Veritabanını güncelleme
                    randevugecmislistele();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Randevu alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }


}

