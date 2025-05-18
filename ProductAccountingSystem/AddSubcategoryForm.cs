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
    public partial class AddSubcategoryForm : Form
    {
        public event Action subcategoryAdded; // Событие для уведомления о добавлении 

        public AddSubcategoryForm()
        {
            InitializeComponent();

            LoadCategories();

            // Устанавливаем плейсхолдеры
            SetPlaceholders();
        }

        private void SetPlaceholders()
        {
            textBox2.Text = "Введите...";
            textBox2.ForeColor = System.Drawing.Color.Gray;
            textBox2.Enter += RemovePlaceholder;
            textBox2.Leave += AddPlaceholder;

            comboBox1.Text = "Выбрать";
            comboBox1.ForeColor = System.Drawing.Color.Gray;
            comboBox1.Enter += RemovePlaceholder;
            comboBox1.Leave += AddPlaceholder;
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
            else if (sender is System.Windows.Forms.ComboBox comboBox)
            {
                if (comboBox.ForeColor == System.Drawing.Color.Gray)
                {
                    comboBox.Text = "";
                    comboBox.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        private void AddPlaceholder(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == textBox2)
                {
                    textBox.Text = "Введите...";
                }
                textBox.ForeColor = System.Drawing.Color.Gray;
            }
            else if (sender is System.Windows.Forms.ComboBox comboBox && string.IsNullOrWhiteSpace(comboBox.Text))
            {
                if (comboBox == comboBox1)
                {
                    comboBox.Text = "Выбрать";
                }
                comboBox.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void LoadCategories()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_category, category_name FROM categories"; // Предполагается, что есть таблица subcategories
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(new { Text = reader["category_name"].ToString(), Value = reader["id_category"].ToString() }); // Добавляем элемент
                }
                comboBox1.DisplayMember = "Text"; // Указываем, что для отображения берем свойство Text
                comboBox1.ValueMember = "Value"; // Указываем, что для выбора берем свойство Value
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string subcategoryName = textBox2.Text;
            string selectedCategoryId = (comboBox1.SelectedItem as dynamic)?.Value;

            if (string.IsNullOrWhiteSpace(subcategoryName) || selectedCategoryId == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            // Сохраняем товар в базу данных
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO subcategories (subcategory_name, id_category) VALUES (@subcategory_name, @id_category)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@subcategory_name", subcategoryName);
                cmd.Parameters.AddWithValue("@id_category", selectedCategoryId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Подкатегория успешно добавлена!");
            subcategoryAdded?.Invoke(); // Уведомляем об добавлении 
            this.Close(); // Закрываем форму
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем форму
        }
    }
}
