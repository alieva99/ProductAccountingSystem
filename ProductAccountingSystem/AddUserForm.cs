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
    public partial class AddUserForm : Form
    {
        public event Action userAdded; // Событие для уведомления о добавлении 

        public AddUserForm()
        {
            InitializeComponent();

            LoadPositions();

            // Устанавливаем плейсхолдеры
            SetPlaceholders();
        }

        private void SetPlaceholders()
        {
            comboBox2.Text = "Выбрать";
            comboBox2.ForeColor = System.Drawing.Color.Gray;
            comboBox2.Enter += RemovePlaceholder;
            comboBox2.Leave += AddPlaceholder;

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

            textBox4.Text = "Введите...";
            textBox4.ForeColor = System.Drawing.Color.Gray;
            textBox4.Enter += RemovePlaceholder;
            textBox4.Leave += AddPlaceholder;

            textBox5.Text = "Введите...";
            textBox5.ForeColor = System.Drawing.Color.Gray;
            textBox5.Enter += RemovePlaceholder;
            textBox5.Leave += AddPlaceholder;

            textBox6.Text = "Введите...";
            textBox6.ForeColor = System.Drawing.Color.Gray;
            textBox6.Enter += RemovePlaceholder;
            textBox6.Leave += AddPlaceholder;
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.ComboBox comboBox)
            {
                if (comboBox.ForeColor == System.Drawing.Color.Gray)
                {
                    comboBox.Text = "";
                    comboBox.ForeColor = System.Drawing.Color.Black;
                }
            }
            else if (sender is System.Windows.Forms.TextBox textBox)
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
            if (sender is System.Windows.Forms.ComboBox comboBox && string.IsNullOrWhiteSpace(comboBox.Text))
            {
                if (comboBox == comboBox2)
                {
                    comboBox.Text = "Выбрать";
                }
                comboBox.ForeColor = System.Drawing.Color.Gray;
            }
            else if (sender is System.Windows.Forms.TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == textBox1)
                {
                    textBox.Text = "Введите...";
                }
                if (textBox == textBox2)
                {
                    textBox.Text = "Введите...";
                }
                if (textBox == textBox3)
                {
                    textBox.Text = "Введите...";
                }
                if (textBox == textBox4)
                {
                    textBox.Text = "Введите...";
                }
                if (textBox == textBox5)
                {
                    textBox.Text = "Введите...";
                }
                if (textBox == textBox6)
                {
                    textBox.Text = "Введите...";
                }
                textBox.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void LoadPositions()
        {
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_position, position_name FROM positions"; // Предполагается, что есть таблица subcategories
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox2.Items.Add(new { Text = reader["position_name"].ToString(), Value = reader["id_position"].ToString() }); // Добавляем элемент
                }
                comboBox2.DisplayMember = "Text"; // Указываем, что для отображения берем свойство Text
                comboBox2.ValueMember = "Value"; // Указываем, что для выбора берем свойство Value
            }
        }

        //кнопка сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            string selectedPositionId = (comboBox2.SelectedItem as dynamic)?.Value;
            string name = textBox4.Text;
            string surname = textBox5.Text;
            string email = textBox1.Text;
            string phone = textBox3.Text;
            string login = textBox2.Text;
            string password = textBox6.Text;

            if (selectedPositionId == null || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            // Сохраняем товар в базу данных
            string connectionString = "server=localhost;uid=root;pwd=cZ_3ip9q1rL8[EVD;database=chainofstores";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO system_users (id_position, user_name, user_surname, user_email, user_phone, login, password) VALUES (@id_position, @user_name, @user_surname, @user_email, @user_phone, @login, @password)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id_position", selectedPositionId);
                cmd.Parameters.AddWithValue("@user_name", name);
                cmd.Parameters.AddWithValue("@user_surname", surname);
                cmd.Parameters.AddWithValue("@user_email", email);
                cmd.Parameters.AddWithValue("@user_phone", phone);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Пользователь успешно добавлен!");
            userAdded?.Invoke(); // Уведомляем об добавлении 
            this.Close(); // Закрываем форму
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Закрываем форму
        }
    }
}
