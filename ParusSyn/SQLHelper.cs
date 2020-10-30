using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

public class SQLHelper
{
    public Boolean result;
    public void Sp_Use(string connString,int id)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
            cmd.Connection = conn;
            conn.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "[dbo].[PARUS_NOMMODIF_INSERT_INTO_LOG]";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@prepid", id);
            cmd.ExecuteNonQuery();
            }
        }
    }
    public void Query_for_DataGridView(string connString, string query, int id, DataGridView data_grid, DataTable dt)
    {
        
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
                    }
                }
            }
        
        
    }

    public Boolean  Record_Exist_Check(string connString, string query, int id)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@prepid", id);

                if (cmd.ExecuteScalar() != null)
                        result = true;
                else    result = false;

                return result;
            }
        }
    }
}
