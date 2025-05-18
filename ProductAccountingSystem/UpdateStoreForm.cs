using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductAccountingSystem
{
    public partial class UpdateStoreForm : Form
    {
        private string storeId; // Идентификатор продукта
        private string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores"; // Строка подключения
        public event Action storeUpdated; // Событие при обновлении категории

        public UpdateStoreForm()
        {
            InitializeComponent();
        }

        public UpdateStoreForm(string id, string storageName, string id_storage, string address, string phone)
        {
            InitializeComponent();
            storeId = id;

            // Загружаем данные в комбобоксы
            LoadStorages();

            // Заполняем текстовые поля данными о продукте
            textBox2.Text = storageName;
            textBox3.Text = address;
            textBox1.Text = phone;
            //comboBox1.SelectedValue = categoryId;
        }

        private void LoadStorages()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_storage, storage_name FROM storages";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dtSubcategories = new DataTable();
                    adapter.Fill(dtSubcategories);
                    comboBox1.DataSource = dtSubcategories;
                    comboBox1.DisplayMember = "storage_name";  // Имя для отображения в комбобоксе
                    comboBox1.ValueMember = "id_storage";      // Значение для передачи
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //кнопка сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            // Получение значений из текстовых полей
            string storeName = textBox2.Text;
            string address = textBox3.Text;
            string phone = textBox1.Text;
            string storageId = comboBox1.SelectedValue.ToString();

            // Проверка на корректность введенных данных
            if (string.IsNullOrWhiteSpace(storeName) || storageId == null || string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            // Обновляем продукт в базе данных
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE stores SET store_name = @store_name, id_storage = @id_storage, store_address = @store_address, store_phone_number = @store_phone_number WHERE id_store = @id_store";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@store_name", storeName);
                    cmd.Parameters.AddWithValue("@id_storage", storageId);
                    cmd.Parameters.AddWithValue("@store_address", address);
                    cmd.Parameters.AddWithValue("@store_phone_number", phone);
                    cmd.Parameters.AddWithValue("@id_store", storeId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные о магазине обновлены!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
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
