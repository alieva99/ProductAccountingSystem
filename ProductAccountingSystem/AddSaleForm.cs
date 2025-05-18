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
    public partial class AddSaleForm : Form
    {
        public event Action saleAdded; // Событие для уведомления о добавлении 

        public AddSaleForm()
        {
            InitializeComponent();
            LoadStores();
            LoadProducts();

            // Устанавливаем плейсхолдеры
            SetPlaceholders();
        }

        private void SetPlaceholders()
        {
            comboBox1.Text = "Выбрать";
            comboBox1.ForeColor = System.Drawing.Color.Gray;
            comboBox1.Enter += RemovePlaceholder;
            comboBox1.Leave += AddPlaceholder;

            comboBox2.Text = "Выбрать";
            comboBox2.ForeColor = System.Drawing.Color.Gray;
            comboBox2.Enter += RemovePlaceholder;
            comboBox2.Leave += AddPlaceholder;

            textBox1.Text = "Введите...";
            textBox1.ForeColor = System.Drawing.Color.Gray;
            textBox1.Enter += RemovePlaceholder;
            textBox1.Leave += AddPlaceholder;
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.ComboBox comboBox)
            {
                if (comboBox.ForeColor == System.Drawing.Color.Gray)
                {
                    comboBox.Text = "";
                    comboBox.ForeColor = System.Drawing.Color.Black;
                }
            }
            else if (sender is System.Windows.Forms.TextBox textBox)
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
            if (sender is System.Windows.Forms.ComboBox comboBox && string.IsNullOrWhiteSpace(comboBox.Text))
            {
                if (comboBox == comboBox1)
                {
                    comboBox.Text = "Выбрать";
                }
                if (comboBox == comboBox2)
                {
                    comboBox.Text = "Выбрать";
                }
                comboBox.ForeColor = System.Drawing.Color.Gray;
            }
            else if (sender is System.Windows.Forms.TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == textBox1)
                {
                    textBox.Text = "Введите...";
                }
                textBox.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void LoadStores()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_store, store_name FROM stores"; // Предполагается, что есть таблица subcategories
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(new { Text = reader["store_name"].ToString(), Value = reader["id_store"].ToString() }); // Добавляем элемент
                }
                comboBox1.DisplayMember = "Text"; // Указываем, что для отображения берем свойство Text
                comboBox1.ValueMember = "Value"; // Указываем, что для выбора берем свойство Value
            }
        }

        private void LoadProducts()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_product, product_name FROM products"; // Предполагается, что есть таблица subcategories
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox2.Items.Add(new { Text = reader["product_name"].ToString(), Value = reader["id_product"].ToString() }); // Добавляем элемент
                }
                comboBox2.DisplayMember = "Text"; // Указываем, что для отображения берем свойство Text
                comboBox2.ValueMember = "Value"; // Указываем, что для выбора берем свойство Value
            }
        }

        //кнопка сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            string selectedStoreId = (comboBox1.SelectedItem as dynamic)?.Value;
            string selectedProductId = (comboBox2.SelectedItem as dynamic)?.Value;
            string price = textBox1.Text;
            DateTime selectedDate = dateTimePicker1.Value;

            if (selectedStoreId == null || selectedProductId == null || string.IsNullOrWhiteSpace(price))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }
            // Проверка на дату из будущего
            if (selectedDate > DateTime.Now)
            {
                MessageBox.Show("Пожалуйста, укажите корректную дату.");
                return;
            }

            // Сохраняем товар в базу данных
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO sales (id_store, id_product, sale_date, sale_price) VALUES (@id_store, @id_product, @sale_date, @sale_price)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id_store", selectedStoreId);
                cmd.Parameters.AddWithValue("@id_product", selectedProductId);
                cmd.Parameters.AddWithValue("@sale_date", selectedDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@sale_price", price);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Продажа успешно добавлена!");
            saleAdded?.Invoke(); // Уведомляем об добавлении 
            this.Close(); // Закрываем форму
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем форму
        }
    }
}
