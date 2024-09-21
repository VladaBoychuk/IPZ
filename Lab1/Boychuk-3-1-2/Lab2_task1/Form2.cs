using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2_task1
{
    public partial class Form2 : Form
    {
        private string optionSelected = "";
        private string tableName = "CosmeticsTypes";
        private SqlConnection connection;
        private string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=BOYCHUK-LAB2;";


        public Form2()
        {
            InitializeComponent();
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SQL connection open error");
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            optionSelected = "SELECT";
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form1 = new Form1();
            form1.Show();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            optionSelected = "ADD";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            optionSelected = "UPDATE";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            optionSelected = "DELETE";
        }

        private void ExecuteSelect()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            var request = $"SELECT * FROM {tableName}";

            var command = new SqlCommand(request, connection);
            var reader = command.ExecuteReader();
            dataGridView1.Columns.Add("CosmeticsTypeId", "CosmeticsTypeId");
            dataGridView1.Columns.Add("Name", "Name");
            dataGridView1.Columns.Add("Description", "Description");
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
            while (reader.Read())
            {
                string[] data = new string[9];
                for (int i = 0; i < 3; i++)
                {
                    data[i] = reader[i].ToString();
                }
                dataGridView1.Rows.Add(data);

            }
            reader.Close();
        }

        private void ExecuteAdd()
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }
            string request = "INSERT INTO CosmeticsTypes (TypeName, Description) " +
                             "VALUES (@TypeName, @Description)";
            using (var command = new SqlCommand(request, connection))
            {
                command.Parameters.AddWithValue("@TypeName", textBox2.Text);
                command.Parameters.AddWithValue("@Description", textBox3.Text);
                command.ExecuteNonQuery();
            }

            MessageBox.Show("Record added successfully.");



        }
        private void ExecuteUpdate()
        {
            // Ensure that necessary fields are filled out
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||  // Assuming textBox1 is used for Id
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            // Construct the SQL update command
            string request = "UPDATE CosmeticsTypes SET TypeName = @TypeName, Description = @Description " +
                             "WHERE CosmeticsTypeId = @CosmeticsTypeId";

            using (var command = new SqlCommand(request, connection))
            {
                // Add parameters with values
                command.Parameters.AddWithValue("@TypeName", textBox2.Text);
                command.Parameters.AddWithValue("@Description", textBox3.Text);
                command.Parameters.AddWithValue("@CosmeticsTypeId", textBox1.Text);  // Assuming textBox1 holds the Id

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Record updated successfully.");
                }
                else
                {
                    MessageBox.Show("No record found with the provided ID.");
                }
            }
        }


        private void ExecuteDelete()
        {
            // Ensure that the ID field is filled out
            if (string.IsNullOrWhiteSpace(textBox1.Text))  // Assuming textBox1 is used for Id
            {
                MessageBox.Show("ID field must be filled.");
                return;
            }

            // Construct the SQL delete command
            string request = "DELETE FROM CosmeticsTypes WHERE CosmeticsTypeId = @CosmeticsTypeId";

            using (var command = new SqlCommand(request, connection))
            {
                // Add parameters with values
                command.Parameters.AddWithValue("@CosmeticsTypeId", textBox1.Text);  // Assuming textBox1 holds the Id

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Record deleted successfully.");
                }
                else
                {
                    MessageBox.Show("No record found with the provided ID.");
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            switch (optionSelected)
            {
                case "SELECT":
                    ExecuteSelect();
                    break;
                case "ADD":
                    ExecuteAdd();
                    break;
                case "UPDATE":
                    ExecuteUpdate();
                    break;
                case "DELETE":
                    ExecuteDelete();
                    break;  
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
