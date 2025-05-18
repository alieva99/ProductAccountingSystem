using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ProductAccountingSystem
{
    public partial class UpdateSupplierForm : Form
    {
        private string supplierId;

        public event Action supplierUpdated; // Событие при обновлении категории

        public UpdateSupplierForm()
        {
            InitializeComponent();
        }

        public UpdateSupplierForm(string id, string name, string email, string phone_number)
        {
            InitializeComponent();
            supplierId = id;
            textBox1.Text = name; // Устанавливаем текущее название категории в текстбокс
            textBox2.Text = email;
            textBox3.Text = phone_number; 
        }


        //кнопка сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            string newCategoryName = textBox1.Text.Trim();
            string emailNew = textBox2.Text.Trim();
            string phone_numberNew = textBox3.Text.Trim();

            if (!string.IsNullOrEmpty(newCategoryName))
            {
                UpdateCategoryInDatabase(supplierId, newCategoryName, emailNew, phone_numberNew); // Обновляем категорию в БД
                supplierUpdated?.Invoke(); // Вызываем событие обновления
                this.Close(); // Закрываем форму
            }
            else
            {
                MessageBox.Show("Введите новое название поставщика.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем форму
        }

        private void UpdateCategoryInDatabase(string id, string name, string email, string phone_number)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE suppliers SET supplier_name = @name, email = @email, phone_number = @phone_number WHERE id_supplier = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phone_number", phone_number);
                    cmd.ExecuteNonQuery(); // Выполнить обновление
                    MessageBox.Show("Поставщик успешно обновлен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
