using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Employees
{
    public partial class Work : Form
    {
        SqlConnection sqlConnection;//для того чтобы законектится к бд, объявляем его как поле класса
                                     //чтобы получать доступ из методов
        public Work()
        {
            InitializeComponent();
        }

        

        private async void Work_Load(object sender, EventArgs e)
        {
            string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\kapustina0906\source\repos\Employees\Employees\Database.mdf; Integrated Security = True";
                                //получаем физическое расположение нашей бд
            sqlConnection = new SqlConnection(connectionString);//создаем экземпляр класса
            await sqlConnection.OpenAsync();//позволит не тормозить пользовательский интерфейс
                                            //делаем наше соединение открытым
            SqlDataReader sqlReader = null;//позволяет получать таблицу в табличном представлении
            SqlCommand command = new SqlCommand("SELECT * FROM [Work]", sqlConnection);//выбираем все из таблицы ворк
                                                                          //делаем запрос через соманд
            try
            {
                sqlReader = await command.ExecuteReaderAsync();//считывает таблицу
                                                      //мы эксюлредару присваиваем возвращаемое значение  из метода которые вызовем
                                                      //позволяет выполнить команды возращаемые табличное представление данных
                while (await sqlReader.ReadAsync())//проходимся по всей считаной инфе
                {                                      //в этом цикле заолняем наш ворк лист
                    workList.Items.Add(Convert.ToString(sqlReader["Id"]) + "    " + Convert.ToString(sqlReader["Name"]) + "    " + Convert.ToString(sqlReader["Wages"]));
                }            //в первой строке айди потом имя и зп
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);//в качесве заголовка будет решение
            }
            finally//что мы должны сделать в любом из случаем когда пройдет код, либо не пройдет в блоке трай
            {
                if (sqlReader != null)//мы должны закрыть наш реадер иначе будет ошибка и прога вылетит
                {
                    sqlReader.Close();
                }

            }
        }

        private void exit_Clich(object sender, EventArgs e)//для того чтобы не было утечки памяти, нужно закрывать соединение
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)//если конекшен не пустой  и проверим не закрыто ли оно уже
                sqlConnection.Close();//закрываем
        }

        private void workClosed(object sender, FormClosedEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                sqlConnection.Close();
        }

        private async void insert(object sender, EventArgs e)
        {
            if (warning1.Visible)
                warning1.Visible = false;
            if (!string.IsNullOrEmpty(nameInsert.Text) && !string.IsNullOrWhiteSpace(nameInsert.Text) && //чтобы имя и зп не отсутствовали
                !string.IsNullOrEmpty(wagesInsert.Text) && !string.IsNullOrWhiteSpace(wagesInsert.Text))
            {
                SqlCommand command = new SqlCommand("INSERT INTO [Work](Name,Wages)VALUES(@Name,@Wages)", sqlConnection);
                       //для добавления
                command.Parameters.AddWithValue("Name", nameInsert.Text);//
                command.Parameters.AddWithValue("Wages", wagesInsert.Text);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                warning1.Visible = true;
                warning1.Text = "Поля 'Имя' и 'Заработная плата' должны быть заполнены";
            }
        }

        private async void list_Update(object sender, EventArgs e)
        {
            workList.Items.Clear();
            SqlDataReader sqlReader = null;//позволяет получать таблицу в табличном представлении
            SqlCommand command = new SqlCommand("SELECT * FROM [Work]", sqlConnection);//выбираем все из таблицы ворк
            try
            {
                sqlReader = await command.ExecuteReaderAsync();//считывает таблицу
                while (await sqlReader.ReadAsync())
                {
                    workList.Items.Add(Convert.ToString(sqlReader["Id"]) + "    " + Convert.ToString(sqlReader["Name"]) + "    " + Convert.ToString(sqlReader["Wages"]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }

            }
        }

        private async void upgate(object sender, EventArgs e)
        {
            if (warning1.Visible)
                warning1.Visible = false;
            if (!string.IsNullOrEmpty(wagesUpdate.Text) && !string.IsNullOrWhiteSpace(wagesUpdate.Text) &&
                !string.IsNullOrEmpty(nameUpdate.Text) && !string.IsNullOrWhiteSpace(nameUpdate.Text) &&
                  !string.IsNullOrEmpty(idUpdate.Text) && !string.IsNullOrWhiteSpace(idUpdate.Text))
            {
                SqlCommand command = new SqlCommand("UPDATE [Work] SET [Nane] = @Name,[Wages]=@Wages WHERE [Id]=@Id", sqlConnection);
                command.Parameters.AddWithValue("Id",idUpdate.Text );
                command.Parameters.AddWithValue("Name",nameUpdate.Text );
                command.Parameters.AddWithValue("Wages",wagesUpdate.Text );
                await command.ExecuteNonQueryAsync();
            }
            else if (!string.IsNullOrEmpty(idUpdate.Text) && !string.IsNullOrWhiteSpace(idUpdate.Text))
            {
                warning2.Visible = true;
                warning2.Text = "Id должен быть заполнен";

            }
            else
            {

                warning2.Visible = true;
                warning2.Text = "Поля 'Id', 'Имя' и 'Заработная плата' должны быть заполнены";
            }
        }

        private async void delete(object sender, EventArgs e)
        {

            if (warning1.Visible)
                warning1.Visible = false;
            if (!string.IsNullOrEmpty(idDelete.Text) && !string.IsNullOrWhiteSpace(idDelete.Text)) {
                SqlCommand command = new SqlCommand("DELETE FROM [Work] wHERE [Id]=@Id",sqlConnection);
                command.Parameters.AddWithValue("Id", idDelete.Text);
                await command.ExecuteNonQueryAsync();
            }
            else
            {

                warning3.Visible = true;
                warning3.Text = "Id должен быть заполнен";
            }
               
        }
    }
}
