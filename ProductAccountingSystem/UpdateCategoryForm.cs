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

namespace ProductAccountingSystem
{
    public partial class UpdateCategoryForm : Form
    {
        private string categoryId;

        public event Action CategoryUpdated; // Событие при обновлении категории

        public UpdateCategoryForm()
        {
            InitializeComponent();
        }

        public UpdateCategoryForm(string id, string name)
        {
            InitializeComponent();
            categoryId = id;
            textBox1.Text = name; // Устанавливаем текущее название категории в текстбокс
        }

        //кнопка сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            string newCategoryName = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(newCategoryName))
            {
                UpdateCategoryInDatabase(categoryId, newCategoryName); // Обновляем категорию в БД
                CategoryUpdated?.Invoke(); // Вызываем событие обновления
                this.Close(); // Закрываем форму
            }
            else
            {
                MessageBox.Show("Введите новое название категории.");
            }
        }

        private void UpdateCategoryInDatabase(string id, string name)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE categories SET category_name = @name WHERE id_category = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery(); // Выполнить обновление
                    MessageBox.Show("Категория успешно обновлена!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем форму
        }
    }
}
