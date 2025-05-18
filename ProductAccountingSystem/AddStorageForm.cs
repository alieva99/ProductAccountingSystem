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
    public partial class AddStorageForm : Form
    {
        public event Action storageAdded; // Событие для уведомления о добавлении 

        public AddStorageForm()
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
            textBox2.Text = "Введите...";
            textBox2.ForeColor = System.Drawing.Color.Gray;
            textBox2.Enter += RemovePlaceholder;
            textBox2.Leave += AddPlaceholder;
            textBox3.Text = "Введите...";
            textBox3.ForeColor = System.Drawing.Color.Gray;
            textBox3.Enter += RemovePlaceholder;
            textBox3.Leave += AddPlaceholder;
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
            string storageName = textBox1.Text;
            string address = textBox2.Text;
            string storage_phone_number = textBox3.Text;

            if (string.IsNullOrWhiteSpace(storageName) || storageName == "Введите..." || string.IsNullOrWhiteSpace(address) || address == "Введите..." || string.IsNullOrWhiteSpace(storage_phone_number) || storage_phone_number == "Введите...")
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            // Сохраняем товар в базу данных
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO storages (storage_name, storage_address, storage_phone_number) VALUES (@storage_name, @storage_address, @storage_phone_number)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@storage_name", storageName);
                cmd.Parameters.AddWithValue("@storage_address", address);
                cmd.Parameters.AddWithValue("@storage_phone_number", storage_phone_number);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Склад успешно добавлен!");
                    storageAdded?.Invoke(); // Уведомляем об добавлении продукта
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении склада: {ex.Message}");
                }
            }

            this.Close(); // Закрываем форму
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем форму
        }
    }
}
