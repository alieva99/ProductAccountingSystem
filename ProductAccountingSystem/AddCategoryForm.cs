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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProductAccountingSystem
{
    public partial class AddCategoryForm : Form
    {
        public event Action categoryAdded; // Событие для уведомления о добавлении 

        public AddCategoryForm()
        {
            InitializeComponent();

            // Устанавливаем плейсхолдеры
            SetPlaceholders();
        }

        private void SetPlaceholders()
        {
            textBox1.Text = "Введите...";
            textBox1.ForeColor = System.Drawing.Color.Gray;
            textBox1.Enter += RemovePlaceholder;
            textBox1.Leave += AddPlaceholder;
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.TextBox textBox)
            {
                if (textBox.ForeColor == System.Drawing.Color.Gray)
                {
                    textBox.Text = "";
                    textBox.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        private void AddPlaceholder(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == textBox1)
                {
                    textBox.Text = "Введите...";
                }
                textBox.ForeColor = System.Drawing.Color.Gray;
            }
        }

        //кнопка сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            string categoryName = textBox1.Text;

            if (string.IsNullOrWhiteSpace(categoryName) || categoryName == "Введите...")
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            // Сохраняем товар в базу данных
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO categories (category_name) VALUES (@category_name)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@category_name", categoryName);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Категория успешно добавлена!");
                    categoryAdded?.Invoke(); // Уведомляем об добавлении продукта
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении категории: {ex.Message}");
                }
            }

            this.Close(); // Закрываем форму
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Просто закрываем форму
        }
    }
}
