using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ParusSyn
{
    public partial class MainForm : Form
    {
        private readonly string LOCALDB_CONN_STRING = @"Server=DESKTOP-II0KDHO\SQL2014;Database=Test;Trusted_Connection=True;";
        //private readonly string LOCALDB_CONN_STRING = @"Data Source=192.168.0.35;Initial Catalog=rsklad;Persist Security Info=True;User ID=sa;Password=r12sql141007";

        SQLHelper s = new SQLHelper();
        public int Id = 0; //id  записи, которую надо синхронизировать
        public int number; //число для работы метода TryParse

        DataTable dt = new DataTable();// таблица для вывода запросов SELECT
        DataTable dt2 = new DataTable();

        private readonly string select_from_rsp = "SELECT [tradenameid],[lfid],[lfmass],[lfmassid],[lfconc],[lfconcid],[lfact],[lfactid],[lfsize],[lfsizeid],[lfchrid],[dospk],[pnom],[ppkmass],[ppkmassid],[ppkvol]," +
            "[ppkvolid],[setid], id, artikul, vgr_kod FROM[dbo].[ROZN_S_PREP] WHERE id = @prepid";
        private readonly string select_from_parus = "SELECT [tradenameid],[lfid],[lfmass],[lfmassid],[lfconc],[lfconcid],[lfact],[lfactid],[lfsize],[lfsizeid],[lfchrid],[dospk],[pnom],[ppkmass],[ppkmassid],[ppkvol]," +
            "[ppkvolid],[setid],[mnn_id],[zayav_price],[zayav_visible],[zayav_plist],[nomen_code],[nomen_name],[dicnomn_id],[modif_name],[nommodif],[modif_code],[umeas_main],[immun_r],[des_st],[komment],[mnn],[ngroup_code]," +
            "[prep_id],[is_jnvls],[artikul],[prep_vgr_kod],tmc_group_code,null FROM[dbo].[PARUS_NOMMODIF] WHERE prep_id = @prepid";
        public MainForm()
        {
            InitializeComponent();
        }
        private void SynButton_Click(object sender, EventArgs e)
        {
            try
            {
                ProgressLabel.Text = "Загрузка...";
                s.Sp_Use(LOCALDB_CONN_STRING, Id);
                ProgressLabel.Text = "Запись сохранена";
                using (SqlConnection conn = new SqlConnection(LOCALDB_CONN_STRING))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "dbo.PARUS_NOMMODIF_UPDATE_FROM_ROZN_S_PREP";
                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("@prepid", Id);
                        ProgressLabel.Text = "Строк затронуто обновлением: " + Convert.ToString(cmd.ExecuteNonQuery());
                        MessageBox.Show("Программа выполнена");
                        ProgressLabel.Text += "\n Введите новый Id";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Появилась ошибка: " + ex.Message);
            }
        }

        private void textBoxWithID_TextChanged(object sender, EventArgs e)
        {
            if (Int32.TryParse(textBoxWithID.Text, out number))
                Id = Convert.ToInt32(textBoxWithID.Text);
            else
                ProgressLabel.Text = "Введите только цифры";
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {

            DB_ShowSelected(LOCALDB_CONN_STRING, select_from_parus, Id, dataGridView1, dt);
            DB_ShowSelected(LOCALDB_CONN_STRING, select_from_rsp, Id, dataGridView2, dt2);
            rebuild(dataGridView1);
            rebuild(dataGridView2);
        }
        public void DB_ShowSelected(string connString, string query, int id, DataGridView data_grid, DataTable dt)
        {
            try
            {
                ProgressLabel.Text = "Загружаем записи из БД";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = query;
                            cmd.Parameters.Clear();
                            dt.Clear();
                            cmd.Parameters.AddWithValue("@prepid", id);
                            sda.Fill(dt);
                            data_grid.DataSource = dt;
                            ProgressLabel.Text = "Готово";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Появилась ошибка: " + ex.Message);
            }
        }

        private void rebuild(DataGridView data_grid) //удаляем пустые ячейки в результатах запросов
        {

            foreach (DataGridViewColumn col in data_grid.Columns)
            {
                foreach (DataGridViewRow row in data_grid.Rows)
                {
                    if (row.Cells[col.Index].Value == DBNull.Value)
                    {
                        col.Visible = false;
                    }
                }
            }
        }

        private void backup_button_Click(object sender, EventArgs e)
        {
            for (int i=0; i == 5; i++)
            {
                MessageBox.Show("Для Вас редактирование существующих записей недоступно"+i);
            }
            
           
        }

        private void recordExistButton_Click(object sender, EventArgs e)
        {
            s.Record_Exist_Check(LOCALDB_CONN_STRING, "SELECT 1 FROM Test.dbo.[PARUS_NOMMODIF] WHERE prep_id = @prepid", Id);
            MessageBox.Show(Convert.ToString(s.result));
        }
    }
}
