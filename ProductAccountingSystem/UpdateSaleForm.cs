using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Collections;
using ProductAccountingSystem;
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
    public partial class UpdateSaleForm : Form
    {
        private string saleId; // Идентификатор продукта
        private string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores"; // Строка подключения
        public event Action saleUpdated; // Событие при обновлении категории

        public UpdateSaleForm()
        {
            InitializeComponent();
        }

        public UpdateSaleForm(string id, string idStore, string idProduct1, string dataSale, string priceSale)
        {
            InitializeComponent();
            saleId = id;

            // Загружаем данные в комбобоксы
            LoadStores();
            LoadProducts();

            textBox1.Text = priceSale;
            // Заполняем текстовые поля данными о продукте
            dateTimePicker1.Text = dataSale;
            //comboBox1.SelectedValue = categoryId;
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

        private void LoadStores()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_store, store_name FROM stores";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dtSubcategories = new DataTable();
                    adapter.Fill(dtSubcategories);
                    comboBox1.DataSource = dtSubcategories;
                    comboBox1.DisplayMember = "store_name";  // Имя для отображения в комбобоксе
                    comboBox1.ValueMember = "id_store";      // Значение для передачи
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
            string storeId = comboBox1.SelectedValue.ToString();
            string productId = comboBox2.SelectedValue.ToString();
            DateTime selectedDate = dateTimePicker1.Value;
            string price = textBox1.Text.Trim();

            // Проверка на корректность введенных данных
            if (productId == null || storeId == null || string.IsNullOrEmpty(price))
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
                    string query = "UPDATE sales SET id_store = @id_store, id_product = @id_product, sale_date = @sale_date, sale_price = " +
                        "@sale_price WHERE id_sale = @id_sale";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_store", storeId);
                    cmd.Parameters.AddWithValue("@id_product", productId);
                    cmd.Parameters.AddWithValue("@sale_date", selectedDate);
                    cmd.Parameters.AddWithValue("@id_sale", saleId);
                    cmd.Parameters.AddWithValue("@sale_price", price);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные о продаже обновлены!");
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

