using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace ProductAccountingSystem
{
    public partial class SortingProductsForm : Form
    {
        private MainForm _mainForm;

        public string currentSortCriteria; // Текущий критерий сортировки

        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }

        public SortingProductsForm(MainForm mainForm, string sortCriteria, decimal? priceFrom, decimal? priceTo)
        {
            InitializeComponent();
            _mainForm = mainForm;
            currentSortCriteria = sortCriteria;
            PriceFrom = priceFrom;
            PriceTo = priceTo;

            // Разрешаем выбор только одного элемента
            checkedListBox1.ItemCheck += CheckedListBox1_ItemCheck;

            // Загружаем текущее состояние сортировки
            this.LoadCurrentSortCriteria();

            // Устанавливаем значения диапазона цен в текстбоксы
            if (PriceFrom.HasValue)
                textBox2.Text = PriceFrom.Value.ToString();
            if (PriceTo.HasValue)
                textBox1.Text = PriceTo.Value.ToString();
        }

        private void CheckedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Снимаем отметку с других элементов, если выбран один
            if (e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBox1.SetItemChecked(i, false);
                    }
                }
            }
        }


        public void LoadCurrentSortCriteria()
        {
            if (currentSortCriteria != null)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.Items[i].ToString() == currentSortCriteria)
                    {
                        checkedListBox1.SetItemChecked(i, true);
                        break;
                    }
                }
            }
        }


        //кнопка сохранить 
        private void button1_Click(object sender, EventArgs e)
        {
            string sortCriteria = null;

            if (checkedListBox1.CheckedItems.Count > 0)
            {
                // Получаем выбранный элемент
                sortCriteria = checkedListBox1.CheckedItems[0].ToString();
            }

            decimal priceFrom = -1;
            decimal priceTo = -1;

            // Проверяем введенные значения
            if (decimal.TryParse(textBox2.Text, out decimal tmpPriceFrom))
            {
                priceFrom = tmpPriceFrom;
            }

            if (decimal.TryParse(textBox1.Text, out decimal tmpPriceTo))
            {
                priceTo = tmpPriceTo;
            }

            // Проверяем, чтобы цена "от" была меньше или равна цене "до"
            if (priceFrom > priceTo && priceFrom != -1 && priceTo != -1)
            {
                MessageBox.Show("Ошибка: 'Цена от' не может быть больше 'Цены до'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Сохраняем значения диапазона цен в MainForm
            _mainForm.LastPriceFrom = priceFrom;
            _mainForm.LastPriceTo = priceTo;

            // Передаем выбранные значения сортировки и цену в MainForm
            _mainForm.SortProducts(sortCriteria, priceFrom, priceTo);
            this.Close(); // Закрываем форму сортировки
        }


        //кнопка отмена
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Просто закрываем форму
        }


    }
}
