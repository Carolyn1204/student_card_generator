using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Globalization;
using System.Configuration;

namespace StudentCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime selectedDate = DateTime.Now;
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private void dobDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dobCalendar.SelectedDate.HasValue)
            {
                selectedDate = dobCalendar.SelectedDate.Value;
               
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string serverConnection = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=True;";
            string createDatabase = "CREATE DATABASE StudentCardDB";
            string createTableQuery = @"
                USE StudentCardDB;
                CREATE TABLE StudentCard (
                    StudentId INT PRIMARY KEY,
                    Name VARCHAR(50) NOT NULL,
                    Surname VARCHAR(50) NOT NULL,
                    Email VARCHAR(200) NOT NULL,
                    DoB Date NOT NULL
                );";

            try
            {

                //Create database
                using (SqlConnection serverCon = new SqlConnection(serverConnection))
                {
                    serverCon.Open();
                    using (SqlCommand cmd = new SqlCommand(createDatabase, serverCon))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Database was created successfully.");
                    }
                }

                // Create the table 
                string databaseConnection = @"Server=(localdb)\MSSQLLocalDB;Database=StudentCardDB;Integrated Security=True;";
                using (SqlConnection dbCon = new SqlConnection(databaseConnection))
                {
                    dbCon.Open();
                    using (SqlCommand cmd = new SqlCommand(createTableQuery, dbCon))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Table was created successfully.");
                    }

                }


                // insert Data
                SqlConnection con = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=StudentCardDB;Integrated Security=True;");
                con.Open();
                string add_data = "insert into [dbo].[StudentCard] (StudentId, Name, Surname, Email, DoB) values (@StudentId, @Name, @Surname, @Email, @DoB)";
                SqlCommand addCmd = new SqlCommand(add_data, con);

                Random random = new Random();
                int newId = 0;
                while (true)
                {
                    newId = random.Next(1000000, 10000000);
                    string check_id = "SELECT COUNT(*) FROM [dbo].[StudentCard] WHERE StudentId = @newId";
                    SqlCommand checkCmd = new SqlCommand(check_id, con);
                    checkCmd.Parameters.AddWithValue("@newId", newId);

                    int count = (int)checkCmd.ExecuteScalar();
                    if (count == 0)
                    {
                        break;
                    }
                }

                addCmd.Parameters.AddWithValue("@StudentId", newId);
                addCmd.Parameters.AddWithValue("@Name", nameTbx.Text);
                addCmd.Parameters.AddWithValue("@Surname", surnameTbx.Text);
                addCmd.Parameters.AddWithValue("@Email", emailTbx.Text);
                addCmd.Parameters.AddWithValue("@DoB", selectedDate);

                addCmd.ExecuteNonQuery();             
                MessageBox.Show("Student Info inserted successfully.");
                con.Close();


                string readConnection = @"Server=(localdb)\MSSQLLocalDB;Database=StudentCardDB;Integrated Security=True;";
                string studentInfoQuery = "SELECT * FROM [dbo].[StudentCard] WHERE StudentId = @newId";

                using (SqlConnection readCon = new SqlConnection(readConnection))
                {
                    readCon.Open();
                    using (SqlCommand cmd = new SqlCommand(studentInfoQuery, readCon))
                    {
                        
                        cmd.Parameters.AddWithValue("@newId", newId);

                      
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            
                            if (reader.Read())
                            {
      
                                    int studentId = reader.GetInt32(reader.GetOrdinal("StudentId"));
                                    string name = reader.GetString(reader.GetOrdinal("Name"));
                                    string surname = reader.GetString(reader.GetOrdinal("Surname"));
                                    string email = reader.GetString(reader.GetOrdinal("Email"));
                                    DateTime dob = reader.GetDateTime(reader.GetOrdinal("DoB"));
                                    
                                    Card card = new Card();
                                card.idTb.Text = studentId.ToString();
                                card.nameTb.Text = name;
                                card.surTb.Text = surname;
                                card.emailTb.Text = email;
                                card.dobTb.Text = dob.ToString("yyyy-MM-dd");
                                card.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Cannot generate student card!");
                            }
                        }
                    }
                }

                



            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }


       



    }
}