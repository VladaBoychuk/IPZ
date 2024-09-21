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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Lab2_task1
{   // main form
    public partial class Form1 : Form
    {

        private SqlConnection connection;
        private string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=BOYCHUK-LAB2;";
        private string databaseName = "BOYCHUK-LAB2";
        private string mainTableName = "Cosmetics";
        private string secondaryTableName = "CosmeticsTypes";

        private string selectedOption = "";
        bool useType = false;
        bool useId = false;
        public Form1()
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
            CheckDatabase();
            PopulateComboBox();


        }

        private void CheckDatabase()
        {
            string[][] expectedTypes = new string[][]
            {
        new string[] { "SPF", "Protection from the sun" },
        new string[] { "shampoo", "Washing the head" }
            };

            foreach (var expectedType in expectedTypes)
            {
                string typeName = expectedType[0];
                string description = expectedType[1];

                string request = "SELECT * FROM CosmeticsTypes " +
                                 "WHERE TypeName = @TypeName AND Description = @Description";
                using (var command = new SqlCommand(request, connection))
                {
                    command.Parameters.AddWithValue("@TypeName", typeName);
                    command.Parameters.AddWithValue("@Description", description);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            reader.Close();
                            string insertRequest = "INSERT INTO CosmeticsTypes (TypeName, Description) " +
                                                   "VALUES (@TypeName, @Description)";
                            using (var insertCommand = new SqlCommand(insertRequest, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@TypeName", typeName);
                                insertCommand.Parameters.AddWithValue("@Description", description);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        private void PopulateComboBox()
        {
            string request = "SELECT TypeName FROM CosmeticsTypes";
            var command = new SqlCommand (request, connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var name = reader.GetString(0);
                this.comboBox1.Items.Add(name);
            }
            reader.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            selectedOption = "SELECT";
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            selectedOption = "ADD";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            selectedOption = "UPDATE";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            selectedOption = "DELETE";
        }

        private void executeSelect()
        {
            var requestBuilder = new StringBuilder();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            requestBuilder.Append($"SELECT * FROM {mainTableName} ");
            requestBuilder.Append($"INNER JOIN {secondaryTableName} ");
            requestBuilder.Append($"ON {mainTableName}.CosmeticsTypeID = {secondaryTableName}.CosmeticsTypeID ");


            if (useId && useType)
            {
                if (textBox1.Text.Length == 0 || !int.TryParse(textBox1.Text, out var _))
                {
                    MessageBox.Show("ID is in incorrect format or empty");
                    return;
                }
                if (int.TryParse(textBox1.Text, out var id))
                {
                    if (id < 1)
                    {
                        MessageBox.Show("ID must be greater 0");
                        return;
                    }
                }
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("Type is not selected");
                    return;
                }
                requestBuilder.Append($"WHERE {mainTableName}.CosmeticsID = '{textBox1.Text}' ");
                requestBuilder.Append($"AND {secondaryTableName}.TypeName = '{comboBox1.Text}'");
            }
            
            else if (useId)
            {
                if (textBox1.Text.Length == 0 || !int.TryParse(textBox1.Text, out var _))
                {
                    MessageBox.Show("ID is in incorrect format or empty");
                    return;
                }

                if (int.TryParse(textBox1.Text, out var id))
                {
                    if (id < 1)
                    {
                        MessageBox.Show("ID must be greater 0");
                        return;
                    }
                }

                requestBuilder.Append($"WHERE {mainTableName}.CosmeticsID = '{textBox1.Text}'");
            }

            else if (useType)
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("Type is not selected");
                    return;
                }

                requestBuilder.Append($"WHERE {secondaryTableName}.TypeName = '{comboBox1.Text}'");
            }
            

            var request = requestBuilder.ToString();
           
            var command = new SqlCommand(request, connection);
            var reader = command.ExecuteReader();
            dataGridView1.Columns.Add("CosmeticsId", "CosmeticsId");
            dataGridView1.Columns.Add("CosmeticsTypeId", "CosmeticsTypeId");
            dataGridView1.Columns.Add("Brand", "Brand");
            dataGridView1.Columns.Add("Name", "Name");
            dataGridView1.Columns.Add("UsedFor", "UsedFor");
            dataGridView1.Columns.Add("Color", "Color");
            dataGridView1.Columns.Add("Price", "Price");
            dataGridView1.Columns.Add("TypeName", "TypeName");
            dataGridView1.Columns.Add("Type Description", "Type Description");
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
            while (reader.Read())
            {
                string[] data = new string[9];
                for (int i = 0; i < 10; i++)
                {
                    if(i == 7)
                        continue;
                    if(i < 7)
                    {
                        data[i] = reader[i].ToString();
                    }
                    else
                    {
                        data[i-1] = reader[i].ToString();
                    }
                }
                dataGridView1.Rows.Add(data);

            }
            reader.Close();

        }

        private void ExecuteAdd()
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) 
                )
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }
            int price = 0;
            if (!int.TryParse(textBox6.Text, out price))
            {
                MessageBox.Show("Price must be an integer number");
                return;
            }
            else if (price <= 0)
            {
                MessageBox.Show("Price must be greater then 0");
                return;
            }


            int cosmeticsTypeId;
            string typeName = comboBox1.Text;
            using (var command = new SqlCommand("SELECT CosmeticsTypeID FROM CosmeticsTypes WHERE TypeName = @TypeName", connection))
            {
                command.Parameters.AddWithValue("@TypeName", typeName);
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    cosmeticsTypeId = Convert.ToInt32(result);
                }
                else
                {
                    MessageBox.Show("Selected type does not exist.");
                    return;
                }
            }

            string request = "INSERT INTO Cosmetics (CosmeticsTypeID, Brand, Name, UsedFor, Color, Price) " +
                             "VALUES (@CosmeticsTypeID, @Brand, @Name, @UsedFor, @Color, @Price)";
            using (var command = new SqlCommand(request, connection))
            {
                command.Parameters.AddWithValue("@CosmeticsTypeID", cosmeticsTypeId);
                command.Parameters.AddWithValue("@Brand", textBox2.Text);
                command.Parameters.AddWithValue("@Name", textBox3.Text);
                command.Parameters.AddWithValue("@UsedFor", textBox4.Text);
                command.Parameters.AddWithValue("@Color", textBox5.Text);
                command.Parameters.AddWithValue("@Price", textBox6.Text);


                command.ExecuteNonQuery();
            }

            MessageBox.Show("Record added successfully.");
        }


        private void ExecuteUpdate()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out var cosmeticsId))
            {
                MessageBox.Show("Cosmetics ID is in incorrect format or empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            int cosmeticsTypeId;
            string typeName = comboBox1.Text;

            // Ensure connection is open
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            try
            {
                // Fetch CosmeticsTypeID
                using (var command = new SqlCommand("SELECT CosmeticsTypeID FROM CosmeticsTypes WHERE TypeName = @TypeName", connection))
                {
                    command.Parameters.AddWithValue("@TypeName", typeName);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        cosmeticsTypeId = Convert.ToInt32(result);
                    }
                    else
                    {
                        MessageBox.Show("Selected type does not exist.");
                        return;
                    }
                }
                int price = 0;
                if (!int.TryParse(textBox6.Text, out price))
                {
                    MessageBox.Show("Price must be an integer number");
                    return;
                }
                else if (price <= 0)
                {
                    MessageBox.Show("Price must be greater then 0");
                    return;
                }

                // Update record
                string request = "UPDATE Cosmetics SET CosmeticsTypeID = @CosmeticsTypeID, " +
                                 "Brand = @Brand, Name = @Name, UsedFor = @UsedFor, Color = @Color, Price = @Price " +
                                 "WHERE CosmeticsID = @CosmeticsID";
                using (var command = new SqlCommand(request, connection))
                {
                    command.Parameters.AddWithValue("@CosmeticsID", cosmeticsId);
                    command.Parameters.AddWithValue("@CosmeticsTypeID", cosmeticsTypeId);
                    command.Parameters.AddWithValue("@Brand", textBox2.Text);
                    command.Parameters.AddWithValue("@Name", textBox3.Text);
                    command.Parameters.AddWithValue("@UsedFor", textBox4.Text);
                    command.Parameters.AddWithValue("@Color", textBox5.Text);
                    command.Parameters.AddWithValue("@Price", price);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Record not found or update failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            
        }


        private void ExecuteDelete()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || !int.TryParse(textBox1.Text, out var cosmeticsId))
            {
                MessageBox.Show("Cosmetics ID is in incorrect format or empty.");
                return;
            }

            string request = "DELETE FROM Cosmetics WHERE CosmeticsID = @CosmeticsID";
            using (var command = new SqlCommand(request, connection))
            {
                command.Parameters.AddWithValue("@CosmeticsID", cosmeticsId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Record deleted successfully.");
                }
                else
                {
                    MessageBox.Show("Record not found.");
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //processing the request
            switch (selectedOption)
            {
                case "SELECT":
                    executeSelect();
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            useId = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            useType = checkBox2.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Form2();
            form2.Show();
        }
    }
}
