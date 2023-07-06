using IniitalDataClassLibrary;
using LogLibraryClassLibrary;
using SQL;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace WpfApplicationStoreAutoOffice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly string TIME_INTERVAL_CHECKING_TASK;

        private DataTable usersData = new DataTable();
        private DataTable storesData = new DataTable();
        private DataTable storesEmployeeData = new DataTable();

        private InitalData initialData = new InitalData();

        private UserSQL userSQL = new UserSQL();
        private StoreSQL storeSQL = new StoreSQL();
        private StoreEmployeeSQL storeEmployeeSQL = new StoreEmployeeSQL();

        public LoginWindow()
        {
            InitializeComponent();
            Logger.CreateLog();
            try
            {
                TIME_INTERVAL_CHECKING_TASK = ConfigurationManager.AppSettings["TIME_INTERVAL_CHECKING_TASK"].ToString();
            }
            catch(Exception ex)
            {
                if(Logger.LoggerExists())
                {
                    Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                }
                System.Windows.MessageBox.Show("Error al obtener el timepo de intervalo de chequeo de las tareas del app.config, " + ex.Message.ToLower(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TIME_INTERVAL_CHECKING_TASK = "30000";
            }

            Load();
        }

        private async void Load()
        {
            await LoadInitialData();
        }

        private async System.Threading.Tasks.Task LoadInitialData()
        {
            UpdateUiFromLoadStore(new Tuple<bool, string>(false, "Cargando data inicial."));
            Tuple<bool, string, DataTable> result = await userSQL.GetUsersAsync();

            if (result.Item1)
            {
                usersData = result.Item3;

                Tuple<bool, string, DataTable> result2 = await storeSQL.GetStoresAsync();

                if (result2.Item1)
                {
                    storesData = result2.Item3;

                    Tuple<bool, string, DataTable> result3 = await storeEmployeeSQL.GetStoresEmployeeAsync();

                    if (result3.Item1)
                    {
                        storesEmployeeData = result3.Item3;
                        UpdateUiFromLoadStore(new Tuple<bool, string>(true, "Data inicial obtenida con exito."));
                    }
                    else
                    {
                        UpdateUiFromLoadStore(new Tuple<bool, string>(false, result3.Item2));
                    }
                }
                else
                {
                    UpdateUiFromLoadStore(new Tuple<bool, string>(false, result2.Item2));
                }
            }
            else
            {
                UpdateUiFromLoadStore(new Tuple<bool, string>(false, result.Item2));
            }
        }

        private void TextBoxStatusUpdate(string text, Brush colorBrush)
        {
            textBoxStatus.Dispatcher.Invoke(() =>
            {
                textBoxStatus.Foreground = colorBrush;
                textBoxStatus.Text = text;
            });
        }

        private void UpdateUiFromLoadStore(Tuple<bool, string> result)
        {
            if (result.Item1)
            {
                TextBoxStatusUpdate(result.Item2, Brushes.Green);
                buttonLogin.ToolTip = "Entrar a la aplicación";
                buttonLogin.IsEnabled = true;
            }
            else
            {
                TextBoxStatusUpdate(result.Item2, Brushes.Red);
                buttonLogin.ToolTip = "Boton deshabilitado";
                buttonLogin.IsEnabled = false;
            }
        }

        private Tuple<bool, string> IsUserInDataBase()
        {
            foreach(DataRow row in usersData.Rows)
            {
                if(row[1].ToString() == textBoxUser.Text)
                {
                    return new Tuple<bool, string>(true, "Usuario existe en la base de datos.");
                }
            }
            return new Tuple<bool, string>(false, "Usuario no existe en la base de datos.");
        }

        private async void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(textBoxUser.Text))
            {
                TextBoxStatusUpdate("Error nombre de ususario vacio.", Brushes.Red);
            }
            else if (string.IsNullOrEmpty(textBoxUserPassword.Password))
            {
                TextBoxStatusUpdate("Error contraseña vacia.", Brushes.Red);
            }
            else
            {
                Tuple<bool, string> result = IsUserInDataBase();
                if (!result.Item1)
                {
                    TextBoxStatusUpdate(result.Item2, Brushes.Red);
                }
                else
                {
                    int userAuditId = 0;
                    foreach(DataRow row in usersData.Rows)
                    {
                        if (row[1].ToString() == textBoxUser.Text)
                        {
                            userAuditId = Int32.Parse(row[0].ToString());
                            break;
                        }
                    }
                        
                    Tuple<bool, string> result2 = ActiveDirectory.AuthenticateActiveDirectory(textBoxUser.Text, textBoxUserPassword.Password);
                    if (!result2.Item1)
                    {
                        userAuditId = 0;
                        TextBoxStatusUpdate(result2.Item2, Brushes.Red);
                    }
                    else
                    {
                        await System.Threading.Tasks.Task.Run(() => TextBoxStatusUpdate("Entrando al sistema espere.", Brushes.Green));
                        this.Hide();
                        textBoxUser.Text = string.Empty;
                        textBoxUserPassword.Password = string.Empty;
                        TextBoxStatusUpdate(string.Empty, Brushes.White);
                        MainWindow mainForm = new MainWindow(new InitalData(TIME_INTERVAL_CHECKING_TASK, usersData,storesData, storesEmployeeData, userAuditId));
                        mainForm.Closed += (s, args) => this.Show();
                        mainForm.Show();
                    }
                        
                }
            }
            
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void buttonRefreshData_Click(object sender, RoutedEventArgs e)
        {
            await LoadInitialData();
        }
    }
}
