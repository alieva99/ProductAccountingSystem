using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Defaults;
//using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using LiveCharts.Wpf;
using ZstdSharp.Unsafe;
using System.Drawing.Drawing2D;
using System.IO;

namespace ProductAccountingSystem
{
    public partial class MainForm : Form
    {
        private DataTable dt;
        private DataView dv; // Храним DataView для фильтрации
        private ToolStripMenuItem currentActiveMenuItem; // Текущий активный элемент меню
        private LiveCharts.WinForms.CartesianChart salesChart;
        private Panel chartPanel;

        // Добавьте эти свойства
        public bool IsProductNameAscendingChecked { get; set; }
        public bool IsProductNameDescendingChecked { get; set; }
        public bool IsPriceAscendingChecked { get; set; }
        public bool IsPriceDescendingChecked { get; set; }

        public int LastSelectedSubcategoryId { get; set; }
        public int LastSelectedSupplierId { get; set; }
        public decimal LastPriceFrom { get; set; } = -1; // -1 будет означать, что значение не установлено
        public decimal LastPriceTo { get; set; } = -1;
        public string currentSortCriteria; // Текущий критерий сортировки

        public enum ActiveTab
        {
            Products,
            Categories,
            Subcategories,
            Suppliers,
            Storages,
            Stores,
            Leftovers,
            Sales,
            Statistics,
            LK,
            Users
        }

        private ActiveTab currentActiveTab;
        // Создаем новую панель
        Panel buttonsPanel = new Panel
        {
            Size = new Size(924, 757),    // Устанавливаем размер панели
            Location = new Point(230, 0),  // Устанавливаем расположение панели
            BackColor = Color.LightGray     // Можно установить цвет фона для визуализации
        };

        public MainForm()
        {
            InitializeComponent();
            textBox1.Enter += new EventHandler(TextBox1_Enter);
            textBox1.Leave += new EventHandler(TextBox1_Leave);
            textBox1.TextChanged += new EventHandler(TextBox1_TextChanged); // Подписка на событие
            // Устанавливаем начальные тексты
            textBox1.Text = "Поиск";
            // Добавляем панель на форму
            this.Controls.Add(buttonsPanel);
            // Изменяем стиль текстов для плейсхолдеров
            textBox1.ForeColor = System.Drawing.Color.Gray;
            // Настройка панели для графика
            chartPanel = new Panel
            {
                Dock = DockStyle.Fill // Занимает все доступное пространство
            };
            this.Controls.Add(chartPanel); // Добавьте панель на форму

            // Создаем и настраиваем диаграмму
            salesChart = new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Fill // Занимает доступное пространство в этой панели
            };
            chartPanel.Controls.Add(salesChart); // Добавляем диаграмму на панель

            // Скрываем панель с графиком по умолчанию
            chartPanel.Visible = false;
            // Скрываем панель при загрузке формы
            panel1.Visible = false;
            buttonsPanel.Visible = false;
        }

        private string firstName;
        private string lastName;

        // Изменяем конструктор
        public MainForm(string firstName, string lastName, string position)
        {
            InitializeComponent();
            this.firstName = firstName;
            this.lastName = lastName;

            // Обновляем текст кнопки "Личный кабинет"
            личныйКабинетToolStripMenuItem.Text = $"{firstName} {lastName}";
            // Проверяем позицию и скрываем элементы, если это «аналитик»
            if (position.Equals("Аналитик", StringComparison.OrdinalIgnoreCase))
            {
                пользователиToolStripMenuItem.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
                button5.Visible = false;
            }
            if (position.Equals("Кассир", StringComparison.OrdinalIgnoreCase))
            {
                пользователиToolStripMenuItem.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
                button5.Visible = false;
                button2.Visible = false;
                общаяСтатистикаToolStripMenuItem.Visible = false;

            }
            if (position.Equals("Сотрудник склада", StringComparison.OrdinalIgnoreCase))
            {
                пользователиToolStripMenuItem.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
                button5.Visible = false;
                button2.Visible = false;
                общаяСтатистикаToolStripMenuItem.Visible = false;
            }
            InitializePersonalCabinet(firstName, lastName);

            textBox1.Enter += new EventHandler(TextBox1_Enter);
            textBox1.Leave += new EventHandler(TextBox1_Leave);
            textBox1.TextChanged += new EventHandler(TextBox1_TextChanged); // Подписка на событие
            // Устанавливаем начальные тексты
            textBox1.Text = "Поиск";
            // Добавляем панель на форму
            this.Controls.Add(buttonsPanel);
            // Изменяем стиль текстов для плейсхолдеров
            textBox1.ForeColor = System.Drawing.Color.Gray;
            // Настройка панели для графика
            chartPanel = new Panel
            {
                Dock = DockStyle.Fill // Занимает все доступное пространство
            };
            this.Controls.Add(chartPanel); // Добавьте панель на форму

            // Создаем и настраиваем диаграмму
            salesChart = new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Fill // Занимает доступное пространство в этой панели
            };
            chartPanel.Controls.Add(salesChart); // Добавляем диаграмму на панель

            // Скрываем панель с графиком по умолчанию
            chartPanel.Visible = false;
            // Скрываем панель при загрузке формы
            panel1.Visible = false;
            buttonsPanel.Visible = false;
        }


        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            string filter = textBox1.Text.Trim().ToLower();
            if (dv != null)
            {
                if (!string.IsNullOrEmpty(filter) && filter != "поиск")
                {
                    dv.RowFilter = string.Join(" OR ", dt.Columns.Cast<DataColumn>()
                        .Where(col => col.DataType == typeof(string))
                        .Select(col => string.Format("{0} LIKE '%{1}%'", col.ColumnName, filter)));
                }
                else
                {
                    dv.RowFilter = string.Empty; // Если фильтр пустой, показываем все строки
                }
                UpdateFilteredRowCount();  // Обновляем количество строк
            }
        }

        private void TextBox1_Enter(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Поиск";
                textBox1.ForeColor = System.Drawing.Color.Gray; // Устанавливаем цвет для плейсхолдера

                // Сбрасываем фильтрацию
                dv.RowFilter = string.Empty; // Показываем все строки
                dataGridView1.DataSource = dv; // Обновляем источник данных
                UpdateRowCount(); // Обновляем количество строк
            }
            if (textBox1.Text == "Поиск")
            {
                textBox1.Text = ""; // Убираем текст "Поиск"
                textBox1.ForeColor = System.Drawing.Color.Black; // Устанавливаем цвет текста
            }
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Поиск";
                textBox1.ForeColor = System.Drawing.Color.Gray; // Устанавливаем цвет для плейсхолдера
            }
        }

        private void продкутыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Products; // Устанавливаем активную вкладку как товары
            // Показать панель с элементами управления
            panel1.Visible = true;
            button1.Visible = true;
            LoadProductsData();
        }

        private void SetActiveMenuItem(ToolStripMenuItem menuItem)
        {
            if (currentActiveMenuItem != null)
            {
                // Сбрасываем цвет предыдущего активного элемента
                currentActiveMenuItem.BackColor = Color.White; // Или любой другой цвет по умолчанию
                currentActiveMenuItem.ForeColor = SystemColors.ControlText; // Цвет текста по умолчанию
            }

            // Устанавливаем новый активный элемент
            currentActiveMenuItem = menuItem;
            currentActiveMenuItem.BackColor = Color.LightBlue; // Цвет для выделенной вкладки
            currentActiveMenuItem.ForeColor = Color.Black; // Изменяем цвет текста выделенной вкладки
        }

        public void LoadProductsData()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                //string queryProducts = "SELECT barcode, product_name, id_subcategory, id_supplier, price, id_product FROM products";
                    string queryProducts = @"
                SELECT p.barcode, p.product_name, s.subcategory_name AS subcategory, su.supplier_name AS supplier, p.price, p.id_product 
                FROM products p 
                JOIN subcategories s ON p.id_subcategory = s.id_subcategory 
                JOIN suppliers su ON p.id_supplier = su.id_supplier"; // Измените "suppliers" на фактическое имя таблицы

                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                dt = new DataTable();
                dt.Load(readerProducts);
                dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных

               // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Артикул";
                dataGridView1.Columns[1].HeaderText = "Товар";
                dataGridView1.Columns[2].HeaderText = "Подкатегория";
                dataGridView1.Columns[3].HeaderText = "Поставщик";
                dataGridView1.Columns[4].HeaderText = "Цена";

                UpdateRowCount(); // Обновляем количество строк
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            HideFirstColumn();
        }

        public void SortProducts(string sortCriteria, decimal priceFrom, decimal priceTo)
        {
            if (!string.IsNullOrEmpty(sortCriteria))
            {
                currentSortCriteria = sortCriteria; // Сохраняем текущий критерий сортировки
                switch (sortCriteria)
                {
                    case "Названия товаров по алфавиту":
                        dv.Sort = "product_name ASC"; // Сортировка по алфавиту
                        break;
                    case "Цена по возрастанию":
                        dv.Sort = "price ASC"; // Сортировка по цене (возрастание)
                        break;
                    case "Цена по убыванию":
                        dv.Sort = "price DESC"; // Сортировка по цене (убывание)
                        break;
                }
                // Применяем фильтр по ценам
                ApplyPriceFilter(priceFrom, priceTo);
                UpdateFilteredRowCount(); // Обновляем количество строк после сортировки
            }
        }

        private void ApplyPriceFilter(decimal priceFrom, decimal priceTo)
        {
            List<string> filters = new List<string>();

            if (priceFrom >= 0)
            {
                filters.Add($"price >= {priceFrom}");
            }

            if (priceTo >= 0)
            {
                filters.Add($"price <= {priceTo}");
            }

            // Применить фильтр к DataView
            if (filters.Count > 0)
            {
                dv.RowFilter = string.Join(" AND ", filters);
            }
            else
            {
                dv.RowFilter = string.Empty; // Сбросить фильтры
            }
            UpdateFilteredRowCount();
        }

        private void HideFirstColumn()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[5].Visible = false; // Скрываем первый столбец
            }
        }

        private void UpdateRowCount()
        {
            if (dt != null)
            {
                // Получаем количество строк
                int rowCount = dt.Rows.Count;

                // Обновляем текст label1
                label1.Text = $"Количество: {rowCount}";
            }
        }
        
        //удалить товар
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) // Проверяем, выбрана ли строка
            {
                switch (currentActiveTab)
                {
                    case ActiveTab.Products:
                        // Получаем данные выбранной строки
                        string productName = dataGridView1.SelectedRows[0].Cells["product_name"].Value.ToString();
                        string productId = dataGridView1.SelectedRows[0].Cells["id_product"].Value.ToString();

                        // Подтверждение удаления
                        DialogResult dialogResult = MessageBox.Show($"Вы действительно хотите удалить товар \"{productName}\"?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            DeleteProduct(productId); // Удаляем продукт из базы данных
                        }
                        break;
                    case ActiveTab.Categories:
                        // Получаем данные выбранной строки
                        string categoryName = dataGridView1.SelectedRows[0].Cells["category_name"].Value.ToString();
                        string categoryId = dataGridView1.SelectedRows[0].Cells["id_category"].Value.ToString();

                        // Подтверждение удаления
                        DialogResult dialogResult1 = MessageBox.Show($"Вы действительно хотите удалить категорию \"{categoryName}\"?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult1 == DialogResult.Yes)
                        {
                            DeleteCategory(categoryId); // Удаляем продукт из базы данных
                        }
                        break;
                    case ActiveTab.Suppliers:
                        // Получаем данные выбранной строки
                        string supplierName = dataGridView1.SelectedRows[0].Cells["supplier_name"].Value.ToString();
                        string supplierId = dataGridView1.SelectedRows[0].Cells["id_supplier"].Value.ToString();

                        // Подтверждение удаления
                        DialogResult dialogResult2 = MessageBox.Show($"Вы действительно хотите удалить поставщика \"{supplierName}\"?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult2 == DialogResult.Yes)
                        {
                            DeleteSupplier(supplierId); // Удаляем продукт из базы данных
                        }
                        break;
                    case ActiveTab.Storages:
                        // Получаем данные выбранной строки
                        string storageName = dataGridView1.SelectedRows[0].Cells["storage_name"].Value.ToString();
                        string storageId = dataGridView1.SelectedRows[0].Cells["id_storage"].Value.ToString();

                        // Подтверждение удаления
                        DialogResult dialogResult3 = MessageBox.Show($"Вы действительно хотите удалить склад \"{storageName}\"?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult3 == DialogResult.Yes)
                        {
                            DeleteStorage(storageId); // Удаляем продукт из базы данных
                        }
                        break;
                    case ActiveTab.Subcategories:
                        // Получаем данные выбранной строки
                        string subcategoryName = dataGridView1.SelectedRows[0].Cells["subcategory_name"].Value.ToString();
                        string subcategoryId = dataGridView1.SelectedRows[0].Cells["id_subcategory"].Value.ToString();

                        // Подтверждение удаления
                        DialogResult dialogResult4 = MessageBox.Show($"Вы действительно хотите удалить подкатегорию \"{subcategoryName}\"?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult4 == DialogResult.Yes)
                        {
                            DeleteSubcategory(subcategoryId); // Удаляем продукт из базы данных
                        }
                        break;
                    case ActiveTab.Stores:
                        // Получаем данные выбранной строки
                        string storeName = dataGridView1.SelectedRows[0].Cells["store_name"].Value.ToString();
                        string storeId = dataGridView1.SelectedRows[0].Cells["id_store"].Value.ToString();

                        // Подтверждение удаления
                        DialogResult dialogResult5 = MessageBox.Show($"Вы действительно хотите удалить магазин \"{storeName}\"?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult5 == DialogResult.Yes)
                        {
                            DeleteStore(storeId); // Удаляем продукт из базы данных
                        }
                        break;
                    case ActiveTab.Leftovers:
                        // Получаем данные выбранной строки
                        string leftoverId = dataGridView1.SelectedRows[0].Cells["id_leftover"].Value.ToString();

                        // Подтверждение удаления
                        DialogResult dialogResult6 = MessageBox.Show($"Вы действительно хотите удалить запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult6 == DialogResult.Yes)
                        {
                            DeleteLeftover(leftoverId); // Удаляем продукт из базы данных
                        }
                        break;
                    case ActiveTab.Sales:
                        // Получаем данные выбранной строки
                        string saleId = dataGridView1.SelectedRows[0].Cells["id_sale"].Value.ToString();

                        // Подтверждение удаления
                        DialogResult dialogResult7 = MessageBox.Show($"Вы действительно хотите удалить продажу?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult7 == DialogResult.Yes)
                        {
                            DeleteSale(saleId); // Удаляем продукт из базы данных
                        }
                        break;
                    case ActiveTab.Users:
                        // Получаем данные выбранной строки
                        string userId = dataGridView1.SelectedRows[0].Cells["id_user"].Value.ToString();
                        string userName = dataGridView1.SelectedRows[0].Cells["user_name"].Value.ToString();

                        // Подтверждение удаления
                        DialogResult dialogResult8 = MessageBox.Show($"Вы действительно хотите удалить пользователя \"{userName}\"?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult8 == DialogResult.Yes)
                        {
                            DeleteUser(userId); // Удаляем продукт из базы данных
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для удаления.");
            }
        }

        private void DeleteUser(string userId)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM system_users WHERE id_user = @id_user";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_user", userId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Пользователь был успешно удален!");

                    LoadUsersData(); // Обновляем список после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DeleteSale(string saleId)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM sales WHERE id_sale = @id_sale";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_sale", saleId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Продажа была успешно удалена!");

                    LoadSalesData(); // Обновляем список после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DeleteLeftover(string leftoverId)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM leftovers_in_storages WHERE id_leftover = @id_leftover";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_leftover", leftoverId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись была успешно удалена!");

                    LoadLeftoversData(); // Обновляем список после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DeleteStore(string storeId)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM stores WHERE id_store = @id_store";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_store", storeId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Магазин был успешно удален!");

                    LoadStoresData(); // Обновляем список после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DeleteSubcategory(string subcategoryId)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM subcategories WHERE id_subcategory = @id_subcategory";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_subcategory", subcategoryId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Подкатегория была успешно удалена!");

                    LoadSubcategoriesData(); // Обновляем список после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DeleteStorage(string storageId)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM storages WHERE id_storage = @id_storage";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_storage", storageId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Склад был успешно удален!");

                    LoadStoragesData(); // Обновляем список после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DeleteSupplier(string supplierId)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM suppliers WHERE id_supplier = @id_supplier";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_supplier", supplierId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Поставщик был успешно удален!");

                    LoadSuppliersData(); // Обновляем список после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DeleteCategory(string categoryId)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM categories WHERE id_category = @id_category";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_category", categoryId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Категория успешно удалёна!");

                    LoadCategoriesData(); // Обновляем список после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DeleteProduct(string productId)
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM products WHERE id_product = @id_product";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id_product", productId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Товар успешно удалён!");

                    LoadProductsData(); // Обновляем список товаров после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //добавить товар
        private void button3_Click(object sender, EventArgs e)
        {
            switch (currentActiveTab)
            {
                case ActiveTab.Products:
                    AddProductForm addProductForm = new AddProductForm();
                    addProductForm.ProductAdded += LoadProductsData; // Подписка на событие
                    addProductForm.Show();
                    break;
                case ActiveTab.Categories:
                    AddCategoryForm addCategoryForm = new AddCategoryForm();
                    addCategoryForm.categoryAdded += LoadCategoriesData; // Подписка на событие
                    addCategoryForm.Show();
                    break;
                case ActiveTab.Suppliers:
                    AddSupplierForm addSupplierForm = new AddSupplierForm();
                    addSupplierForm.supplierAdded += LoadSuppliersData; // Подписка на событие
                    addSupplierForm.Show();
                    break;
                case ActiveTab.Storages:
                    AddStorageForm addStorageForm = new AddStorageForm();
                    addStorageForm.storageAdded += LoadStoragesData; // Подписка на событие
                    addStorageForm.Show();
                    break;
                case ActiveTab.Subcategories:
                    AddSubcategoryForm addSubcategoryForm = new AddSubcategoryForm();
                    addSubcategoryForm.subcategoryAdded += LoadSubcategoriesData; // Подписка на событие
                    addSubcategoryForm.Show();
                    break;
                case ActiveTab.Stores:
                    AddStoreForm addStoreForm = new AddStoreForm();
                    addStoreForm.storeAdded += LoadStoresData; // Подписка на событие
                    addStoreForm.Show();
                    break;
                case ActiveTab.Leftovers:
                    AddLeftoverForm addLeftoverForm = new AddLeftoverForm();
                    addLeftoverForm.leftoverAdded += LoadLeftoversData; // Подписка на событие
                    addLeftoverForm.Show();
                    break;
                case ActiveTab.Sales:
                    AddSaleForm addSaleForm = new AddSaleForm();
                    addSaleForm.saleAdded += LoadSalesData; // Подписка на событие
                    addSaleForm.Show();
                    break;
                case ActiveTab.Users:
                    AddUserForm addUserForm = new AddUserForm();
                    addUserForm.userAdded += LoadUsersData; // Подписка на событие
                    addUserForm.Show();
                    break;
            }
        }


        //редактирование товара
        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) // Проверяем, выбрана ли строка
            {
                switch (currentActiveTab)
                {
                    case ActiveTab.Products:
                        // Получаем данные выбранной строки
                        string productId = dataGridView1.SelectedRows[0].Cells["id_product"].Value.ToString();
                        string barcode = dataGridView1.SelectedRows[0].Cells["barcode"].Value.ToString();
                        string productName = dataGridView1.SelectedRows[0].Cells["product_name"].Value.ToString();
                        //string subcategoryId = dataGridView1.SelectedRows[0].Cells["subcategory"].Value.ToString(); // Используем id_subcategory
                        string subcategoryId = dataGridView1.SelectedRows[0].Cells["subcategory"].Value.ToString();

                        string supplierId = dataGridView1.SelectedRows[0].Cells["supplier"].Value.ToString(); // Используем id_supplier
                        decimal price = Convert.ToDecimal(dataGridView1.SelectedRows[0].Cells["price"].Value);

                        // Открываем форму редактирования и передаем параметры
                        UpdateProductForm updateProductForm = new UpdateProductForm(productId, barcode, productName, subcategoryId, supplierId, price);
                        updateProductForm.ShowDialog(); // Показываем форму как модальную

                        LoadProductsData(); // Обновляем данные после закрытия формы
                        break;
                    case ActiveTab.Categories:
                        // Получаем данные выбранной строки
                        string categoryId = dataGridView1.SelectedRows[0].Cells["id_category"].Value.ToString();
                        string categoryName = dataGridView1.SelectedRows[0].Cells["category_name"].Value.ToString();

                        // Открываем форму редактирования и передаем параметры
                        UpdateCategoryForm updateCategoryForm = new UpdateCategoryForm(categoryId, categoryName);
                        updateCategoryForm.CategoryUpdated += LoadCategoriesData; // Подписка на событие обновления
                        updateCategoryForm.ShowDialog();
                        break;
                    case ActiveTab.Suppliers:
                        // Получаем данные выбранной строки
                        string id_supplier = dataGridView1.SelectedRows[0].Cells["id_supplier"].Value.ToString();
                        string supplierName = dataGridView1.SelectedRows[0].Cells["supplier_name"].Value.ToString();
                        string email = dataGridView1.SelectedRows[0].Cells["email"].Value.ToString();
                        string phone_number = dataGridView1.SelectedRows[0].Cells["phone_number"].Value.ToString();
                        // Открываем форму редактирования и передаем параметры
                        UpdateSupplierForm updateSupplierForm = new UpdateSupplierForm(id_supplier, supplierName, email, phone_number);
                        updateSupplierForm.supplierUpdated += LoadSuppliersData; // Подписка на событие обновления
                        updateSupplierForm.ShowDialog();
                        break;
                    case ActiveTab.Storages:
                        // Получаем данные выбранной строки
                        string id_storage = dataGridView1.SelectedRows[0].Cells["id_storage"].Value.ToString();
                        string storageName = dataGridView1.SelectedRows[0].Cells["storage_name"].Value.ToString();
                        string address = dataGridView1.SelectedRows[0].Cells["storage_address"].Value.ToString();
                        string storage_phone_number = dataGridView1.SelectedRows[0].Cells["storage_phone_number"].Value.ToString();
                        // Открываем форму редактирования и передаем параметры
                        UpdateStorageForm updateStorageForm = new UpdateStorageForm(id_storage, storageName, address, storage_phone_number);
                        updateStorageForm.storageUpdated += LoadStoragesData; // Подписка на событие обновления
                        updateStorageForm.ShowDialog();
                        break;
                    case ActiveTab.Subcategories:
                        // Получаем данные выбранной строки
                        string id_subcategory = dataGridView1.SelectedRows[0].Cells["id_subcategory"].Value.ToString();
                        string subcategoryName = dataGridView1.SelectedRows[0].Cells["subcategory_name"].Value.ToString();
                        string id_category = dataGridView1.SelectedRows[0].Cells["category_name"].Value.ToString();
                        // Открываем форму редактирования и передаем параметры
                        UpdateSubcategoryForm updateSubcategoryForm = new UpdateSubcategoryForm(id_subcategory, subcategoryName, id_category);
                        updateSubcategoryForm.ShowDialog(); // Показываем форму как модальную

                        LoadSubcategoriesData(); // Обновляем данные после закрытия формы
                        break;
                    case ActiveTab.Stores:
                        // Получаем данные выбранной строки
                        string id_sutore = dataGridView1.SelectedRows[0].Cells["id_store"].Value.ToString();
                        string storeName = dataGridView1.SelectedRows[0].Cells["store_name"].Value.ToString();
                        string storageID = dataGridView1.SelectedRows[0].Cells["storage_name"].Value.ToString();
                        string storeAddress = dataGridView1.SelectedRows[0].Cells["store_address"].Value.ToString();
                        string number = dataGridView1.SelectedRows[0].Cells["store_phone_number"].Value.ToString();
                        // Открываем форму редактирования и передаем параметры
                        UpdateStoreForm updateStoreForm = new UpdateStoreForm(id_sutore, storeName, storageID, storeAddress, number);
                        updateStoreForm.ShowDialog(); // Показываем форму как модальную

                        LoadStoresData(); // Обновляем данные после закрытия формы
                        break;
                    case ActiveTab.Leftovers:
                        // Получаем данные выбранной строки
                        string id_leftover = dataGridView1.SelectedRows[0].Cells["id_leftover"].Value.ToString();
                        string idStorage = dataGridView1.SelectedRows[0].Cells["storage_name"].Value.ToString();
                        string idProduct = dataGridView1.SelectedRows[0].Cells["product_name"].Value.ToString();
                        string data = dataGridView1.SelectedRows[0].Cells["update_data"].Value.ToString();
                        // Открываем форму редактирования и передаем параметры
                        UpdateLeftoverForm updateLeftoverForm = new UpdateLeftoverForm(id_leftover, idStorage, idProduct, data);
                        updateLeftoverForm.ShowDialog(); // Показываем форму как модальную

                        LoadLeftoversData(); // Обновляем данные после закрытия формы
                        break;
                    case ActiveTab.Sales:
                        // Получаем данные выбранной строки
                        string id_sale = dataGridView1.SelectedRows[0].Cells["id_sale"].Value.ToString();
                        string idStore = dataGridView1.SelectedRows[0].Cells["store_name"].Value.ToString();
                        string idProduct1 = dataGridView1.SelectedRows[0].Cells["product_name"].Value.ToString();
                        string dataSale = dataGridView1.SelectedRows[0].Cells["sale_date"].Value.ToString();
                        string priceSale = dataGridView1.SelectedRows[0].Cells["sale_price"].Value.ToString();
                        // Открываем форму редактирования и передаем параметры
                        UpdateSaleForm updateSaleForm = new UpdateSaleForm(id_sale, idStore, idProduct1, dataSale, priceSale);
                        updateSaleForm.ShowDialog(); // Показываем форму как модальную

                        LoadSalesData(); // Обновляем данные после закрытия формы
                        break;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите строку для изменения.");
            }
        }


        private void UpdateFilteredRowCount()
        {
            if (dv != null)
            {
                // Получаем количество строк в DataView
                int rowCount = dv.Count;

                // Обновляем текст label1
                label1.Text = $"Количество: {rowCount}";
            }
        }

        //кнопка сортировки
        private void button1_Click(object sender, EventArgs e)
        {
            switch (currentActiveTab)
            {
                case ActiveTab.Products:
                    SortingProductsForm sortingForm = new SortingProductsForm(this, currentSortCriteria, LastPriceFrom >= 0 ? LastPriceFrom : (decimal?)null, LastPriceTo >= 0 ? LastPriceTo : (decimal?)null);
                    sortingForm.ShowDialog(); // Показываем форму как модальную
                    break;
                case ActiveTab.Categories:
                    SortCategoriesAlphabetically();
                    break;
                case ActiveTab.Suppliers:
                    SortSuppliersAlphabetically();
                    break;
                case ActiveTab.Storages:
                    SortStoragesAlphabetically();
                    break;
                case ActiveTab.Subcategories:
                    SortSubcategoriesAlphabetically();
                    break;
                case ActiveTab.Stores:
                    SortStoresAlphabetically();
                    break;
                case ActiveTab.Leftovers:
                    SortLeftoversAlphabetically();
                    break;
                case ActiveTab.Sales:
                    SortSalesAlphabetically();
                    break;
            }
        }

        // Метод для сортировки продаж по алфавиту
        private void SortSalesAlphabetically()
        {
            if (dv != null)
            {
                // Сортируем DataView по имени категории
                dv.Sort = "sale_date DESC"; // Сортировка по алфавиту по столбцу

                // Обновляем источник данных для отображения в dataGridView
                dataGridView1.DataSource = dv;

                UpdateFilteredRowCount(); // Обновляем количество строк после сортировки
            }
        }

        // Метод для сортировки магазинов по алфавиту
        private void SortLeftoversAlphabetically()
        {
            if (dv != null)
            {
                // Сортируем DataView по имени категории
                dv.Sort = "product_name ASC"; // Сортировка по алфавиту по столбцу

                // Обновляем источник данных для отображения в dataGridView
                dataGridView1.DataSource = dv;

                UpdateFilteredRowCount(); // Обновляем количество строк после сортировки
            }
        }

        // Метод для сортировки магазинов по алфавиту
        private void SortStoresAlphabetically()
        {
            if (dv != null)
            {
                // Сортируем DataView по имени категории
                dv.Sort = "store_name ASC"; // Сортировка по алфавиту по столбцу

                // Обновляем источник данных для отображения в dataGridView
                dataGridView1.DataSource = dv;

                UpdateFilteredRowCount(); // Обновляем количество строк после сортировки
            }
        }

        // Метод для сортировки подкатегорий по алфавиту
        private void SortSubcategoriesAlphabetically()
        {
            if (dv != null)
            {
                // Сортируем DataView по имени категории
                dv.Sort = "subcategory_name ASC"; // Сортировка по алфавиту по столбцу

                // Обновляем источник данных для отображения в dataGridView
                dataGridView1.DataSource = dv;

                UpdateFilteredRowCount(); // Обновляем количество строк после сортировки
            }
        }

        // Метод для сортировки складов по алфавиту
        private void SortStoragesAlphabetically()
        {
            if (dv != null)
            {
                // Сортируем DataView по имени категории
                dv.Sort = "storage_name ASC"; // Сортировка по алфавиту по столбцу

                // Обновляем источник данных для отображения в dataGridView
                dataGridView1.DataSource = dv;

                UpdateFilteredRowCount(); // Обновляем количество строк после сортировки
            }
        }

        // Метод для сортировки поставщиков по алфавиту
        private void SortSuppliersAlphabetically()
        {
            if (dv != null)
            {
                // Сортируем DataView по имени категории
                dv.Sort = "supplier_name ASC"; // Сортировка по алфавиту по столбцу

                // Обновляем источник данных для отображения в dataGridView
                dataGridView1.DataSource = dv;

                UpdateFilteredRowCount(); // Обновляем количество строк после сортировки
            }
        }

        // Метод для сортировки категорий по алфавиту
        private void SortCategoriesAlphabetically()
        {
            if (dv != null)
            {
                // Сортируем DataView по имени категории
                dv.Sort = "category_name ASC"; // Сортировка по алфавиту по столбцу "category_name"

                // Обновляем источник данных для отображения в dataGridView
                dataGridView1.DataSource = dv;

                UpdateFilteredRowCount(); // Обновляем количество строк после сортировки
            }
        }



        //КАТЕГОРИИ
        private void категорииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Categories; // Устанавливаем активную вкладку как товары
            // Показать панель с элементами управления
            panel1.Visible = true;
            button1.Visible = true;
            LoadCategoriesData();
        }

        public void LoadCategoriesData()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string queryProducts = "SELECT category_name, id_category FROM categories";
                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                dt = new DataTable();
                dt.Load(readerProducts);
                dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных

                // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Название категории";

                // Устанавливаем AutoSizeMode для первого столбца
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                // Скрываем второй столбец
                if (dataGridView1.Columns.Count > 1)
                {
                    dataGridView1.Columns[1].Visible = false; // Скрываем столбец
                }

                UpdateRowCount(); // Обновляем количество строк
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

        private void поставщикиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Suppliers; // Устанавливаем активную вкладку как товары
            // Показать панель с элементами управления
            panel1.Visible = true;
            button1.Visible = true;
            LoadSuppliersData();
        }

        public void LoadSuppliersData()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string queryProducts = "SELECT supplier_name, email, phone_number, id_supplier FROM suppliers";
                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                dt = new DataTable();
                dt.Load(readerProducts);
                dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных

                // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Поставщик";
                dataGridView1.Columns[1].HeaderText = "Почта";
                dataGridView1.Columns[2].HeaderText = "Телефон";

                // Устанавливаем режим авторазмера для первых трех столбцов
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (dataGridView1.Columns.Count > 1)
                {
                    dataGridView1.Columns[3].Visible = false; // Скрываем столбец
                }

                UpdateRowCount(); // Обновляем количество строк
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

        private void складыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Storages; // Устанавливаем активную вкладку как товары
            // Показать панель с элементами управления
            panel1.Visible = true;
            button1.Visible = true;
            LoadStoragesData();
        }

        public void LoadStoragesData()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string queryProducts = "SELECT storage_name, storage_address, storage_phone_number, id_storage FROM storages";
                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                dt = new DataTable();
                dt.Load(readerProducts);
                dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных

                // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Склад";
                dataGridView1.Columns[1].HeaderText = "Адрес";
                dataGridView1.Columns[2].HeaderText = "Телефон";

                // Устанавливаем режим авторазмера для первых трех столбцов
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (dataGridView1.Columns.Count > 1)
                {
                    dataGridView1.Columns[3].Visible = false; // Скрываем столбец
                }

                UpdateRowCount(); // Обновляем количество строк
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

        private void подкатегорииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Subcategories; // Устанавливаем активную вкладку как товары
            // Показать панель с элементами управления
            panel1.Visible = true;
            button1.Visible = true;
            LoadSubcategoriesData();
        }

        public void LoadSubcategoriesData()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                //string queryProducts = "SELECT barcode, product_name, id_subcategory, id_supplier, price, id_product FROM products";
                string queryProducts = @"
                SELECT  p.subcategory_name, su.category_name, p.id_subcategory 
                FROM subcategories p 
                JOIN subcategories s ON p.id_subcategory = s.id_subcategory 
                JOIN categories su ON p.id_category = su.id_category"; 

                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                dt = new DataTable();
                dt.Load(readerProducts);
                dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных

                // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Подкатегория";
                dataGridView1.Columns[1].HeaderText = "Категория";

                // Устанавливаем режим авторазмера для первых трех столбцов
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (dataGridView1.Columns.Count > 1)
                {
                    dataGridView1.Columns[2].Visible = false; // Скрываем столбец
                }

                UpdateRowCount(); // Обновляем количество строк
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

        private void магазиныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Stores; // Устанавливаем активную вкладку как товары
            // Показать панель с элементами управления
            panel1.Visible = true;
            button1.Visible = true;
            LoadStoresData();
        }

        public void LoadStoresData()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string queryProducts = @"
                SELECT  p.store_name, p.store_address, p.store_phone_number,  su.storage_name, p.id_store 
                FROM stores p 
                JOIN stores s ON p.id_store = s.id_store
                JOIN storages su ON p.id_storage = su.id_storage";

                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                dt = new DataTable();
                dt.Load(readerProducts);
                dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных

                // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Магазин";
                dataGridView1.Columns[1].HeaderText = "Адрес магазина";
                dataGridView1.Columns[2].HeaderText = "Телефон магазина";
                dataGridView1.Columns[3].HeaderText = "Склад";

                // Устанавливаем режим авторазмера для первых трех столбцов
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (dataGridView1.Columns.Count > 1)
                {
                    dataGridView1.Columns[4].Visible = false; // Скрываем столбец
                }

                UpdateRowCount(); // Обновляем количество строк
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

        private void остаткиНаСкладахToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Leftovers; // Устанавливаем активную вкладку как товары
            // Показать панель с элементами управления
            panel1.Visible = true;
            button1.Visible = true;
            LoadLeftoversData();
        }

        public void LoadLeftoversData()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string queryProducts = @"
                SELECT   su.product_name, s.storage_name , p.update_data, p.id_leftover
                FROM leftovers_in_storages p 
                JOIN storages s ON p.id_storage = s.id_storage
                JOIN products su ON p.id_product = su.id_product";

                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                dt = new DataTable();
                dt.Load(readerProducts);
                // Явно преобразуем значения update_data в DateTime
                foreach (DataRow row in dt.Rows)
                {
                    if (row["update_data"] != DBNull.Value)
                    {
                        // Попробуем преобразовать в DateTime
                        DateTime dateValue;
                        if (DateTime.TryParse(row["update_data"].ToString(), out dateValue))
                        {
                            row["update_data"] = dateValue; // Убедитесь, что значение корректное
                        }
                        else
                        {
                            // Обработка ошибки преобразования, например, установим значение по умолчанию
                            row["update_data"] = DateTime.MinValue; // Или удалите строку
                        }
                    }
                }
                dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных

                // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Продукт";
                dataGridView1.Columns[1].HeaderText = "Склад";
                dataGridView1.Columns[2].HeaderText = "Дата изменения";

                // Устанавливаем режим авторазмера для первых трех столбцов
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (dataGridView1.Columns.Count > 1)
                {
                    dataGridView1.Columns[3].Visible = false; // Скрываем столбец
                }

                UpdateRowCount(); // Обновляем количество строк
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

        private void продажиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Sales; // Устанавливаем активную вкладку как товары
            // Показать панель с элементами управления
            panel1.Visible = true;
            button1.Visible = true;

            LoadSalesData();
        }

        public void LoadSalesData()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string queryProducts = @"
                SELECT   su.product_name, p.sale_date, p.sale_price, s.store_name , p.id_sale
                FROM sales p 
                JOIN stores s ON p.id_store = s.id_store
                JOIN products su ON p.id_product = su.id_product";

                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                dt = new DataTable();
                dt.Load(readerProducts);
                dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных

                // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Продукт";
                dataGridView1.Columns[1].HeaderText = "Дата продажи";
                dataGridView1.Columns[2].HeaderText = "Цена продажи";
                dataGridView1.Columns[3].HeaderText = "Магазин";

                // Устанавливаем режим авторазмера для первых трех столбцов
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (dataGridView1.Columns.Count > 1)
                {
                    dataGridView1.Columns[4].Visible = false; // Скрываем столбец
                }

                UpdateRowCount(); // Обновляем количество строк
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

        private void общаяСтатистикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Statistics; // Устанавливаем активную вкладку как статистика
            StatisticsForm statForm = new StatisticsForm();
            statForm.Show();
            //this.Hide(); // Скрываем текущую форму
        }

        
        //личный кабинет
        private void личныйКабинетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.LK; // Устанавливаем активную вкладку как статистика
            panel1.Visible = false; // Скрываем панель с кнопками
            chartPanel.Visible = false; // Показываем панель с графико
            buttonsPanel.Visible = true;

            InitializePersonalCabinet();

        }

        private void InitializePersonalCabinet()
        {
            // Заголовок
            Label titleLabel = new Label
            {
                Text = "Личный кабинет",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            buttonsPanel.Controls.Add(titleLabel);

            // Имя
            AddLabelAndTextBox("Имя:", "текст", new Point(10, 70));

            // Фамилия
            AddLabelAndTextBox("Фамилия:", "текст", new Point(10, 130));

            // Должность
            AddLabelAndTextBox("Должность:", "текст", new Point(10, 190));

            // Email
            AddLabelAndTextBox("Email:", "email@example.com", new Point(10, 250));

            // Телефон
            AddLabelAndTextBox("Телефон:", "+123456789", new Point(10, 310));

            // Кнопка "Выйти"
            System.Windows.Forms.Button logoutButton = new System.Windows.Forms.Button
            {
                Text = "Выйти",
                Location = new Point(100, 370),
                Size = new Size(150, 40),
                Font = new Font("Arial", 14), // Устанавливаем размер шрифта кнопки
                BackColor = Color.White // Устанавливаем цвет кнопки на белый
            };
            logoutButton.Click += LogoutButton_Click;
            buttonsPanel.Controls.Add(logoutButton);

            // Кнопка "Руководство пользователя"
            System.Windows.Forms.Button helpButton = new System.Windows.Forms.Button
            {
                Text = "?",
                Size = new Size(30, 30), // Размер обычной кнопки
                Location = new Point(300, 370), // Расположение: правее всех элементов, на уровне с заголовком
                Font = new Font("Arial", 10),
                BackColor = Color.LightBlue
            };


            helpButton.Click += HelpButton_Click;
            buttonsPanel.Controls.Add(helpButton);

        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            // Получаем путь к рабочему столу текущего пользователя
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Формируем полный путь к PDF-файлу
            string helpFilePath = Path.Combine(desktopPath, "пдфка.pdf");// Или любой другой формат файла.  Убедитесь, что файл существует!

            try
            {
                // Открываем файл с руководством
                System.Diagnostics.Process.Start(helpFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл руководства: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddLabelAndTextBox(string labelText, string textBoxText, Point location)
        {
            Label label = new Label
            {
                Text = labelText,
                Location = location,
                AutoSize = true,
                Font = new Font("Arial", 13) // Устанавливаем размер шрифта для меток
            };
            buttonsPanel.Controls.Add(label);

            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox
            {
                Text = textBoxText,
                Location = new Point(location.X + 200, location.Y),
                Width = 200,
                ReadOnly = true // Пользователь не может редактировать
            };
            buttonsPanel.Controls.Add(textBox);
        }

        // Обработчик кнопки "Выйти"
        private void LogoutButton_Click(object sender, EventArgs e)
        {
            // Вернуться на окно авторизации
            this.Hide(); // Скрываем текущее окно
            var loginForm = new AuthorizationForm(); // Создаем экземпляр формы авторизации
            loginForm.Show(); // Показываем форму авторизации
        }

        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetActiveMenuItem((ToolStripMenuItem)sender); // Устанавливаем активный элемент меню
            currentActiveTab = ActiveTab.Users; // Устанавливаем активную вкладку как товары
            // Показать панель с элементами управления
            panel1.Visible = true;
            button1.Visible = false;
            LoadUsersData();
        }

        public void LoadUsersData()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string queryProducts = @"
                SELECT    p.user_name, p.user_surname,s.position_name, p.user_email,  p.user_phone ,p.login,p.password, p.id_user
                FROM system_users p 
                JOIN positions s ON p.id_position = s.id_position";

                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                dt = new DataTable();
                dt.Load(readerProducts);
                dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных

                // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Имя";
                dataGridView1.Columns[1].HeaderText = "Фамилия";
                dataGridView1.Columns[2].HeaderText = "Должность";
                dataGridView1.Columns[3].HeaderText = "Почта";
                dataGridView1.Columns[4].HeaderText = "Телефон";
                dataGridView1.Columns[5].HeaderText = "Логин";
                dataGridView1.Columns[6].HeaderText = "Пароль";

                // Устанавливаем режим авторазмера для первых трех столбцов
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (dataGridView1.Columns.Count > 1)
                {
                    dataGridView1.Columns[7].Visible = false; // Скрываем столбец
                }

                UpdateRowCount(); // Обновляем количество строк
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

        private void InitializePersonalCabinet(string firstName, string lastName)
        {
            // Заголовок
            Label titleLabel = new Label
            {
                Text = "Личный кабинет",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            buttonsPanel.Controls.Add(titleLabel);

            // Имя
            AddLabelAndTextBox("Имя:", firstName, new Point(10, 70));

            // Фамилия
            AddLabelAndTextBox("Фамилия:", lastName, new Point(10, 130));

            // Получаем дополнительные данные пользователя
            LoadUserData(firstName, lastName);

            // Кнопка "Выйти"
            System.Windows.Forms.Button logoutButton = new System.Windows.Forms.Button
            {
                Text = "Выйти",
                Location = new Point(100, 370),
                Size = new Size(150, 40),
                Font = new Font("Arial", 14),
                BackColor = Color.White
            };
            logoutButton.Click += LogoutButton_Click;
            buttonsPanel.Controls.Add(logoutButton);
        }

        private void LoadUserData(string firstName, string lastName)
        {
            // Соединение с базой данных
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT p.position_name, u.user_email, u.user_phone 
            FROM system_users u
            JOIN positions p ON u.id_position = p.id_position
            WHERE u.user_name = @user_name AND u.user_surname = @user_surname";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_name", firstName);
                    cmd.Parameters.AddWithValue("@user_surname", lastName);

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string position = reader["position_name"].ToString();
                                string email = reader["user_email"].ToString();
                                string phone = reader["user_phone"].ToString();

                                // Добавляем данные в соответствующие текстовые поля
                                AddLabelAndTextBox("Должность:", position, new Point(10, 190));
                                AddLabelAndTextBox("Email:", email, new Point(10, 250));
                                AddLabelAndTextBox("Телефон:", phone, new Point(10, 310));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Обработка ошибок (например, отобразить сообщение пользователю)
                        MessageBox.Show("Ошибка при получении данных: " + ex.Message);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем Excel приложение
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

                // Проверяем, что Excel установлен
                if (excelApp == null)
                {
                    MessageBox.Show("Excel не установлен на этом компьютере!");
                    return;
                }

                // Создаем новую книгу Excel
                Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = null;

                try
                {
                    // Получаем активный лист
                    worksheet = workbook.Sheets[1];
                    worksheet = workbook.ActiveSheet;
                    worksheet.Name = "Отчет"; // Измените имя листа, если хотите

                    // Записываем заголовки столбцов
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        worksheet.Cells[1, j + 1] = dataGridView1.Columns[j].HeaderText;
                    }

                    // Записываем данные из DataGridView
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            if (dataGridView1.Rows[i].Cells[j].Value != null)
                            {
                                worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            }
                            else
                            {
                                worksheet.Cells[i + 2, j + 1] = ""; // Пустая ячейка
                            }
                        }
                    }

                    // Автоматическая ширина столбцов
                    Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange; // Явно указываем пространство имен
                    range.Columns.AutoFit();

                    // Сохраняем файл Excel
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Отчет.xlsx"; // Имя файла по умолчанию
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel Files|*.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(saveFileDialog.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing); // Явно указываем пространство имен
                        MessageBox.Show("Отчет успешно выгружен!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выгрузке в Excel: " + ex.Message);
                }
                finally
                {
                    // Закрываем Excel (важно!)
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    if (workbook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    excelApp = null;
                    workbook = null;
                    worksheet = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Общая ошибка: " + ex.Message);
            }
        }

    }
}


