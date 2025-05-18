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
    public partial class UpdateLeftoverForm : Form
    {
        private string leftoverId; // Идентификатор продукта
        private string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores"; // Строка подключения
        public event Action leftoverUpdated; // Событие при обновлении категории

        public UpdateLeftoverForm()
        {
            InitializeComponent();
        }

        public UpdateLeftoverForm(string id, string idStorage, string idProduct, string data)
        {
            InitializeComponent();
            leftoverId = id;

            // Загружаем данные в комбобоксы
            LoadStorages();
            LoadProducts();

            // Заполняем текстовые поля данными о продукте
            dateTimePicker1.Text = data;
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

        private void LoadProducts()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_product, product_name FROM products";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dtSubcategories = new DataTable();
                    adapter.Fill(dtSubcategories);
                    comboBox2.DataSource = dtSubcategories;
                    comboBox2.DisplayMember = "product_name";  // Имя для отображения в комбобоксе
                    comboBox2.ValueMember = "id_product";      // Значение для передачи
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string storageId = comboBox1.SelectedValue.ToString();
            string productId = comboBox2.SelectedValue.ToString();
            DateTime selectedDate = dateTimePicker1.Value;

            // Проверка на корректность введенных данных
            if (productId == null || storageId == null)
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
                    string query = "UPDATE leftovers_in_storages SET id_storage = @id_storage, id_product = @id_product, update_data = @update_data WHERE id_leftover = @id_leftover";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_storage", storageId);
                    cmd.Parameters.AddWithValue("@id_product", productId);
                    cmd.Parameters.AddWithValue("@update_data", selectedDate);
                    cmd.Parameters.AddWithValue("@id_leftover", leftoverId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные о записи обновлены!");
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
