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
    public partial class UpdateStorageForm : Form
    {
        private string storageId;
        public event Action storageUpdated; // Событие при обновлении категории

        public UpdateStorageForm()
        {
            InitializeComponent();
        }

        public UpdateStorageForm(string id, string storage_name, string storage_address, string storage_phone_number)
        {
            InitializeComponent();
            storageId = id;
            textBox1.Text = storage_name; // Устанавливаем текущее название категории в текстбокс
            textBox2.Text = storage_address;
            textBox3.Text = storage_phone_number;
        }

        //кнопка сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            string newStorageName = textBox1.Text.Trim();
            string addressNew = textBox2.Text.Trim();
            string storage_phone_numberNew = textBox3.Text.Trim();

            if (!string.IsNullOrEmpty(newStorageName))
            {
                UpdateStorageInDatabase(storageId, newStorageName, addressNew, storage_phone_numberNew); // Обновляем категорию в БД
                storageUpdated?.Invoke(); // Вызываем событие обновления
                this.Close(); // Закрываем форму
            }
            else
            {
                MessageBox.Show("Введите новое название склада.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем форму
        }

        private void UpdateStorageInDatabase(string id, string storage_name, string storage_address, string storage_phone_number)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE storages SET storage_name = @storage_name, storage_address = @storage_address, storage_phone_number = @storage_phone_number WHERE id_storage = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@storage_name", storage_name);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@storage_address", storage_address);
                    cmd.Parameters.AddWithValue("@storage_phone_number", storage_phone_number);
                    cmd.ExecuteNonQuery(); // Выполнить обновление
                    MessageBox.Show("Склад успешно обновлен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
