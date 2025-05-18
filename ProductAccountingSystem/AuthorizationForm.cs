using MySql.Data.MySqlClient;
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

namespace ProductAccountingSystem
{
    public partial class AuthorizationForm : Form
    {
        public AuthorizationForm()
        {
            InitializeComponent();
            // Добавляем обработчики событий
            textBox1.Enter += new EventHandler(TextBoxLogin_Enter);
            textBox1.Leave += new EventHandler(TextBoxLogin_Leave);
            textBox2.Enter += new EventHandler(TextBoxPassword_Enter);
            textBox2.Leave += new EventHandler(TextBoxPassword_Leave);

            // Устанавливаем начальные тексты
            textBox1.Text = "Введите логин...";
            textBox2.Text = "Введите пароль...";

            // Изменяем стиль текстов для плейсхолдеров
            textBox1.ForeColor = System.Drawing.Color.Gray;
            textBox2.ForeColor = System.Drawing.Color.Gray;

            // Инициализируем метку для ошибок
            label5.ForeColor = System.Drawing.Color.Red;
            label5.Text = ""; // Стираем текст ошибки
        }

        private void TextBoxLogin_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Введите логин...")
            {
                textBox1.Text = "";
                textBox1.ForeColor = System.Drawing.Color.Black; // Устанавливаем цвет текста
            }
        }

        private void TextBoxLogin_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Введите логин...";
                textBox1.ForeColor = System.Drawing.Color.Gray; // Устанавливаем цвет для плейсхолдера
            }
        }

        private void TextBoxPassword_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Введите пароль...")
            {
                textBox2.Text = "";
                textBox2.ForeColor = System.Drawing.Color.Black; // Устанавливаем цвет текста
                textBox2.UseSystemPasswordChar = true; // Устанавливаем маску для пароля
            }
        }

        private void TextBoxPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.Text = "Введите пароль...";
                textBox2.ForeColor = System.Drawing.Color.Gray; // Устанавливаем цвет для плейсхолдера
                textBox2.UseSystemPasswordChar = false; // Убираем маску для плейсхолдера
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Стираем текст ошибки
            label5.Text = "";

            // Проверяем заполнение полей
            if (string.IsNullOrWhiteSpace(textBox1.Text) || textBox1.Text == "Введите логин..." ||
                string.IsNullOrWhiteSpace(textBox2.Text) || textBox2.Text == "Введите пароль...")
            {
                label5.Text = "Заполните все необходимые данные.";
                return;
            }

            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string queryProducts = @"
            SELECT u.user_name, u.user_surname, p.position_name 
            FROM system_users u
            JOIN positions p ON u.id_position = p.id_position
            WHERE u.login = @login AND u.password = @password";

                using (MySqlCommand cmd = new MySqlCommand(queryProducts, conn))
                {
                    cmd.Parameters.AddWithValue("@login", textBox1.Text);
                    cmd.Parameters.AddWithValue("@password", textBox2.Text);

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstName = reader["user_name"].ToString();
                                string lastName = reader["user_surname"].ToString();
                                string position = reader["position_name"].ToString();

                                MainForm mainForm = new MainForm(firstName, lastName, position);
                                mainForm.Show();
                                this.Hide(); // Скрываем текущую форму
                            }
                            else
                            {
                                label5.Text = "Неверный логин или пароль. Попробуйте снова.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        label5.Text = "Ошибка при подключении к базе данных: " + ex.Message;
                    }
                }
            }
        }


    }
}

