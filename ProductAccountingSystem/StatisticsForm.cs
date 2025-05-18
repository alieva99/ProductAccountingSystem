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
    public partial class StatisticsForm : Form
    {
        private DataTable dt;
        private DataView dv; // Храним DataView для фильтрации

        public StatisticsForm()
        {
            InitializeComponent();

            LoadTopProducts();
            LoadStockLevels();
            LoadSalesByStore();
            LoadSalesByMonth();
        }
        //назад
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide(); // Скрываем текущую форму
        }

        public void LoadTopProducts()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string queryProducts = @"
            SELECT 
                p.barcode, 
                p.product_name, 
                s.subcategory_name, 
                p.price, 
                sup.supplier_name, 
                COUNT(sa.id_product) AS count
            FROM 
                products p
            JOIN 
                subcategories s ON p.id_subcategory = s.id_subcategory
            JOIN 
                suppliers sup ON p.id_supplier = sup.id_supplier
            LEFT JOIN 
                sales sa ON p.id_product = sa.id_product
            GROUP BY 
                p.id_product
            ORDER BY 
                count DESC
            LIMIT 10;";

                MySqlCommand cmdProducts = new MySqlCommand(queryProducts, conn);
                MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(readerProducts);
                DataView dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView1.DataSource = dv; // Установка источника данных


                // Изменяем заголовки столбцов на русском языке
                dataGridView1.Columns[0].HeaderText = "Артикул";
                dataGridView1.Columns[1].HeaderText = "Продукт";
                dataGridView1.Columns[2].HeaderText = "Подкатегория";
                dataGridView1.Columns[3].HeaderText = "Цена";
                dataGridView1.Columns[4].HeaderText = "Поставщик";
                dataGridView1.Columns[5].HeaderText = "Количество";

                // Устанавливаем AutoSizeMode для первого столбца
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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

        public void LoadStockLevels()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();

                // Получаем топ продаж
                var topProductsBarcodes = new HashSet<string>();
                string topProductsQuery = @"
            SELECT barcode 
            FROM products p 
            JOIN sales sa ON p.id_product = sa.id_product 
            GROUP BY p.id_product 
            ORDER BY COUNT(sa.id_product) DESC 
            LIMIT 10;";

                MySqlCommand cmdTopProducts = new MySqlCommand(topProductsQuery, conn);
                using (var readerTop = cmdTopProducts.ExecuteReader())
                {
                    while (readerTop.Read())
                    {
                        topProductsBarcodes.Add(readerTop.GetString("barcode"));
                    }
                }

                // Получаем уровень запасов
                string queryStockLevels = @"
            SELECT 
                p.barcode, 
                p.product_name, 
                COUNT(ls.id_product) AS count
            FROM 
                products p
            LEFT JOIN 
                leftovers_in_storages ls ON p.id_product = ls.id_product
            GROUP BY 
                p.id_product
            HAVING 
                COUNT(ls.id_product) < 10
            ORDER BY 
                count ASC
            LIMIT 10;";

                MySqlCommand cmdStockLevels = new MySqlCommand(queryStockLevels, conn);
                MySqlDataReader readerStockLevels = cmdStockLevels.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(readerStockLevels);

                // Добавляем столбец priority
                dt.Columns.Add("priority", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    if (topProductsBarcodes.Contains(row["barcode"].ToString()))
                    {
                        row["priority"] = "10 самых продаваемых";
                    }
                    else
                    {
                        row["priority"] = string.Empty; // Пустое значение
                    }
                }

                DataView dv = new DataView(dt); // Создаем новый DataView после загрузки данных
                dataGridView2.DataSource = dv; // Установка источника данных

                // Изменяем заголовки столбцов на русском языке
                dataGridView2.Columns[0].HeaderText = "Артикул";
                dataGridView2.Columns[1].HeaderText = "Продукт";
                dataGridView2.Columns[2].HeaderText = "Количество";
                dataGridView2.Columns[3].HeaderText = "Приоритет";

                // Устанавливаем AutoSizeMode для первого столбца
                dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView2.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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

        public void LoadSalesByStore()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string querySales = @"
    SELECT 
        st.store_name, 
        COUNT(s.id_sale) AS sales_count
    FROM 
        stores st
    LEFT JOIN 
        sales s ON st.id_store = s.id_store
    GROUP BY 
        st.id_store
    ORDER BY 
        sales_count DESC;";

                MySqlCommand cmdSales = new MySqlCommand(querySales, conn);
                MySqlDataReader readerSales = cmdSales.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(readerSales);

                // Очищаем предыдущие данные диаграммы (всегда очищаем перед добавлением новых)
                chart1.Series["Размер продаж"].Points.Clear();

                // Заполняем диаграмму данными
                foreach (DataRow row in dt.Rows)
                {
                    string storeName = row["store_name"].ToString();
                    int salesCount = Convert.ToInt32(row["sales_count"]);
                    chart1.Series["Размер продаж"].Points.AddXY(storeName, salesCount);
                }
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

        public void LoadSalesByMonth()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();

                // Получаем текущую дату
                DateTime currentDate = DateTime.Now;

                // Создаем список названий последних 12 месяцев
                List<string> last12Months = new List<string>();
                for (int i = 0; i < 12; i++)
                {
                    last12Months.Add(currentDate.AddMonths(-i).ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")));
                }
                last12Months.Reverse(); // Оборачиваем для правильного порядка

                // Извлекаем продажи из базы данных
                string querySales = @"
SELECT 
    DATE_FORMAT(s.sale_date, '%Y-%m') AS sale_month, 
    COUNT(s.id_sale) AS sales_count
FROM 
    sales s
WHERE 
    s.sale_date >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)
GROUP BY 
    sale_month
ORDER BY 
    sale_month;";

                MySqlCommand cmdSales = new MySqlCommand(querySales, conn);
                MySqlDataReader readerSales = cmdSales.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(readerSales);

                // Очищаем предыдущие данные диаграммы
                chart2.Series["Количество продаж"].Points.Clear();

                // Создаем словарь для хранения количества продаж с инициализацией
                Dictionary<string, int> salesData = new Dictionary<string, int>();
                foreach (var month in last12Months)
                {
                    salesData[month] = 0; // Изначально устанавливаем количество продаж в 0
                }

                // Заполняем данные из DataTable
                foreach (DataRow row in dt.Rows)
                {
                    string saleMonth = DateTime.ParseExact(row["sale_month"].ToString(), "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture)
                        .ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU"));

                    // Устанавливаем количество продаж для соответствующего месяца
                    if (salesData.ContainsKey(saleMonth))
                    {
                        salesData[saleMonth] = Convert.ToInt32(row["sales_count"]);
                    }
                }

                // Заполняем диаграмму данными
                foreach (var month in last12Months)
                {
                    chart2.Series["Количество продаж"].Points.AddXY(month, salesData[month]);
                }
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








    }
}
