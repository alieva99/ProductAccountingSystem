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
    public partial class UpdateProductForm : Form
    {
        private string productId; // Идентификатор продукта
        private string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores"; // Строка подключения

        public UpdateProductForm()
        {
            InitializeComponent();

        }

        public UpdateProductForm(string id, string barcode, string productName, string subcategoryId, string supplierId, decimal price)
        {
            InitializeComponent();
            productId = id;

            // Загружаем данные в комбобоксы
            LoadSubcategories();
            LoadSuppliers();

            // Заполняем текстовые поля данными о продукте
            textBox1.Text = barcode;
            textBox2.Text = productName;
            textBox3.Text = price.ToString();
            //comboBox1.SelectedValue = subcategoryId;
            //comboBox2.SelectedValue = supplierId;
        }

        private void LoadSubcategories()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_subcategory, subcategory_name FROM subcategories";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dtSubcategories = new DataTable();
                    adapter.Fill(dtSubcategories);
                    comboBox1.DataSource = dtSubcategories;
                    comboBox1.DisplayMember = "subcategory_name";  // Имя для отображения в комбобоксе
                    comboBox1.ValueMember = "id_subcategory";      // Значение для передачи
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void LoadSuppliers()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_supplier, supplier_name FROM suppliers";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dtSuppliers = new DataTable();
                    adapter.Fill(dtSuppliers);
                    comboBox2.DataSource = dtSuppliers;
                    comboBox2.DisplayMember = "supplier_name";  // Имя для отображения в комбобоксе
                    comboBox2.ValueMember = "id_supplier";      // Значение для передачи
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
            string barcode = textBox1.Text;
            string productName = textBox2.Text;
            string subcategoryId = comboBox1.SelectedValue.ToString();
            string supplierId = comboBox2.SelectedValue.ToString();
            decimal price;

            // Проверка на корректность введенных данных
            if (string.IsNullOrWhiteSpace(barcode) || string.IsNullOrWhiteSpace(productName) ||
                subcategoryId == null || supplierId == null || !decimal.TryParse(textBox3.Text, out price))
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
                    string query = "UPDATE products SET barcode = @barcode, product_name = @product_name, id_subcategory = @id_subcategory, id_supplier = @id_supplier, price = @price WHERE id_product = @id_product";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@barcode", barcode);
                    cmd.Parameters.AddWithValue("@product_name", productName);
                    cmd.Parameters.AddWithValue("@id_subcategory", subcategoryId);
                    cmd.Parameters.AddWithValue("@id_supplier", supplierId);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@id_product", productId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные о продукте обновлены!");
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
            this.Close(); // Просто закрываем форму
        }
    }
}
