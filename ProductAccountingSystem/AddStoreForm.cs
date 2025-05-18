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
    public partial class AddStoreForm : Form
    {
        public event Action storeAdded; // Событие для уведомления о добавлении 

        public AddStoreForm()
        {
            InitializeComponent();
            LoadStorages();

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

        private void LoadStorages()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_storage, storage_name FROM storages"; // Предполагается, что есть таблица subcategories
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(new { Text = reader["storage_name"].ToString(), Value = reader["id_storage"].ToString() }); // Добавляем элемент
                }
                comboBox1.DisplayMember = "Text"; // Указываем, что для отображения берем свойство Text
                comboBox1.ValueMember = "Value"; // Указываем, что для выбора берем свойство Value
            }
        }

        //кнопка сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            string storeName = textBox2.Text;
            string address = textBox3.Text;
            string phone_naum = textBox1.Text;
            string selectedStorageId = (comboBox1.SelectedItem as dynamic)?.Value;

            if (string.IsNullOrWhiteSpace(storeName) || selectedStorageId == null || string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phone_naum))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            // Сохраняем товар в базу данных
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO stores (store_name, id_storage, store_address, store_phone_number) VALUES (@store_name, @id_storage, @store_address, @store_phone_number)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@store_name", storeName);
                cmd.Parameters.AddWithValue("@id_storage", selectedStorageId);
                cmd.Parameters.AddWithValue("@store_address", address);
                cmd.Parameters.AddWithValue("@store_phone_number", phone_naum);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Магазин успешно добавлен!");
            storeAdded?.Invoke(); // Уведомляем об добавлении 
            this.Close(); // Закрываем форму
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем форму
        }
    }
}
