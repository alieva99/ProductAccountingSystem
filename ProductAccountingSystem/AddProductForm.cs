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
    public partial class AddProductForm : Form
    {
        public event Action ProductAdded; // Событие для уведомления о добавлении продукта

        public AddProductForm()
        {
            InitializeComponent();

            LoadSubcategories();
            LoadSuppliers();

            // Устанавливаем плейсхолдеры
            SetPlaceholders();
        }

        private void SetPlaceholders()
        {
            textBox1.Text = "Введите...";
            textBox1.ForeColor = System.Drawing.Color.Gray;
            textBox1.Enter += RemovePlaceholder;
            textBox1.Leave += AddPlaceholder;

            textBox2.Text = "Введите...";
            textBox2.ForeColor = System.Drawing.Color.Gray;
            textBox2.Enter += RemovePlaceholder;
            textBox2.Leave += AddPlaceholder;

            textBox3.Text = "Введите...";
            textBox3.ForeColor = System.Drawing.Color.Gray;
            textBox3.Enter += RemovePlaceholder;
            textBox3.Leave += AddPlaceholder;

            comboBox1.Text = "Выбрать";
            comboBox1.ForeColor = System.Drawing.Color.Gray;
            comboBox1.Enter += RemovePlaceholder;
            comboBox1.Leave += AddPlaceholder;

            comboBox2.Text = "Выбрать";
            comboBox2.ForeColor = System.Drawing.Color.Gray;
            comboBox2.Enter += RemovePlaceholder;
            comboBox2.Leave += AddPlaceholder;
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.ForeColor == System.Drawing.Color.Gray)
                {
                    textBox.Text = "";
                    textBox.ForeColor = System.Drawing.Color.Black;
                }
            }
            else if (sender is ComboBox comboBox)
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
            if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == textBox1)
                {
                    textBox.Text = "Введите...";
                }
                else if (textBox == textBox2)
                {
                    textBox.Text = "Введите...";
                }
                else if (textBox == textBox3)
                {
                    textBox.Text = "Введите...";
                }
                textBox.ForeColor = System.Drawing.Color.Gray;
            }
            else if (sender is ComboBox comboBox && string.IsNullOrWhiteSpace(comboBox.Text))
            {
                if (comboBox == comboBox1)
                {
                    comboBox.Text = "Выбрать";
                }
                else if (comboBox == comboBox2)
                {
                    comboBox.Text = "Выбрать";
                }
                comboBox.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void LoadSubcategories()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_subcategory, subcategory_name FROM subcategories"; // Предполагается, что есть таблица subcategories
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox2.Items.Add(new { Text = reader["subcategory_name"].ToString(), Value = reader["id_subcategory"].ToString() }); // Добавляем элемент
                }
                comboBox2.DisplayMember = "Text"; // Указываем, что для отображения берем свойство Text
                comboBox2.ValueMember = "Value"; // Указываем, что для выбора берем свойство Value
            }
        }

        private void LoadSuppliers()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_supplier, supplier_name FROM suppliers"; // Предполагается, что есть таблица suppliers
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(new { Text = reader["supplier_name"].ToString(), Value = reader["id_supplier"].ToString() }); // Добавляем элемент
                }
                comboBox1.DisplayMember = "Text"; // Указываем, что для отображения берем свойство Text
                comboBox1.ValueMember = "Value"; // Указываем, что для выбора берем свойство Value
            }
        }

        //кнопка "сохранить"
        public void button1_Click(object sender, EventArgs e)
        {
            string barcode = textBox1.Text;
            string productName = textBox2.Text;
            string selectedSubcategoryId = (comboBox2.SelectedItem as dynamic)?.Value;
            string selectedSupplierId = (comboBox1.SelectedItem as dynamic)?.Value;
            decimal price;

            if (string.IsNullOrWhiteSpace(barcode) || string.IsNullOrWhiteSpace(productName) || selectedSubcategoryId == null || selectedSupplierId == null || !decimal.TryParse(textBox3.Text, out price))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            // Сохраняем товар в базу данных
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO products (barcode, product_name, id_subcategory, id_supplier, price) VALUES (@barcode, @product_name, @id_subcategory, @id_supplier, @price)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@barcode", barcode);
                cmd.Parameters.AddWithValue("@product_name", productName);
                cmd.Parameters.AddWithValue("@id_subcategory", selectedSubcategoryId);
                cmd.Parameters.AddWithValue("@id_supplier", selectedSupplierId);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Товар успешно добавлен!");
            ProductAdded?.Invoke(); // Уведомляем об добавлении продукта
            this.Close(); // Закрываем форму
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Просто закрываем форму
        }
    }
}
