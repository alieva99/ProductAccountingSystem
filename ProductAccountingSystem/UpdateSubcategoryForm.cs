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
    public partial class UpdateSubcategoryForm : Form
    {
        private string subcategoryId; // Идентификатор продукта
        private string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores"; // Строка подключения
        public event Action subcategoryUpdated; // Событие при обновлении категории

        public UpdateSubcategoryForm()
        {
            InitializeComponent();
        }

        public UpdateSubcategoryForm(string id, string subcategoryName, string id_category)
        {
            InitializeComponent();
            subcategoryId = id;

            // Загружаем данные в комбобоксы
            LoadCategories();

            // Заполняем текстовые поля данными о продукте
            textBox2.Text = subcategoryName;
            //comboBox1.SelectedValue = categoryId;
        }

        private void LoadCategories()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_category, category_name FROM categories";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dtSubcategories = new DataTable();
                    adapter.Fill(dtSubcategories);
                    comboBox1.DataSource = dtSubcategories;
                    comboBox1.DisplayMember = "category_name";  // Имя для отображения в комбобоксе
                    comboBox1.ValueMember = "id_category";      // Значение для передачи
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Получение значений из текстовых полей
            string subcategory_name = textBox2.Text;
            string categoryId = comboBox1.SelectedValue.ToString();

            // Проверка на корректность введенных данных
            if (string.IsNullOrWhiteSpace(subcategory_name) || categoryId == null )
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
                    string query = "UPDATE subcategories SET subcategory_name = @subcategory_name, id_category = @id_category WHERE id_subcategory = @id_subcategory";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@subcategory_name", subcategory_name);
                    cmd.Parameters.AddWithValue("@id_category", categoryId);
                    cmd.Parameters.AddWithValue("@id_subcategory", subcategoryId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные о подкатегории обновлены!");
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
