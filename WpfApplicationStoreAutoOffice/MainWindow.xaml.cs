using Excel;
using IniitalDataClassLibrary;
using LogLibraryClassLibrary;
using Microsoft.Win32;
using SQL;
using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TaskClassLibrary;
using WinformsClassLibrary;
using static TaskClassLibrary.TaskOperators;

namespace WpfApplicationStoreAutoOffice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InitalData initialData = new InitalData();
        private StoreSQL storeSQL = new StoreSQL();
        private StoreEmployeeSQL storeEmployeeSQL = new StoreEmployeeSQL();
        private TaskSQL taskSQL = new TaskSQL();
        private UserSQL userSQL = new UserSQL();

        private DataTable dataPendingTask = new DataTable();

        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        public MainWindow(InitalData initialData)
        {
            InitializeComponent();
            this.initialData = initialData;
            LoadFilterStores();
            LoadEditStores();
            LoadEditUser();
            LoadEditUserStores();
            LoadAddEditUserStore();
            LoadComboboxHistoric();
            LoadComboboxStatusTask();
            LoadComboboxStatusLocal();
            GetPendingTaskFromStore();
            backgroundWorker.DoWork += worker_DoWork;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.ProgressChanged += worker_ProgressChanged;
            backgroundWorker.RunWorkerAsync();
        }

        private string GetTime()
        {
            DateTime time = DateTime.Now;
            return " Time: " + time.Hour + ":" + time.Minute + ":" + time.Second;
        }

        private void LoadFilterStores()
        {
            comboboxPendinngFilterInit.Items.Clear();
            comboboxPendinngFilterFinal.Items.Clear();
            foreach (DataRow row in initialData.StoresData.Rows)
            {
                comboboxPendinngFilterInit.Items.Add(new MyDataComboBox(row[0].ToString(), row[1].ToString()));
                comboboxPendinngFilterFinal.Items.Add(new MyDataComboBox(row[0].ToString(), row[1].ToString()));
            }
            comboboxPendinngFilterInit.SelectedIndex = 0;
            comboboxPendinngFilterFinal.SelectedIndex = comboboxPendinngFilterFinal.Items.Count - 1;
        }

        private void LoadEditStores()
        {
            comboboxEditStore.Items.Clear();
            foreach (DataRow row in initialData.StoresData.Rows)
            {
                comboboxEditStore.Items.Add(new MyDataComboBox(row[0].ToString(), row[1].ToString()));
            }
        }

        private void LoadEditUserStores()
        {
            comboboxEditUserStore.Items.Clear();
            foreach (DataRow row in initialData.StoresEmployeeData.Rows)
            {
                comboboxEditUserStore.Items.Add(new MyDataComboBox(row[0].ToString(), row[1].ToString()));
            }
        }

        private void LoadEditUser()
        {
            comboboxEditUser.Items.Clear();
            foreach (DataRow row in initialData.UsersData.Rows)
            {
                comboboxEditUser.Items.Add(new MyDataComboBox(row[0].ToString(), row[1].ToString()));
            }
        }

        private void LoadAddEditUserStore()
        {
            comboboxEditUserStoreSelectStore.Items.Clear();
            comboboxAddUserStoreSelectStore.Items.Clear();
            foreach (DataRow row in initialData.StoresData.Rows)
            {
                comboboxEditUserStoreSelectStore.Items.Add(new MyDataComboBox(row[0].ToString(), row[1].ToString()));
                comboboxAddUserStoreSelectStore.Items.Add(new MyDataComboBox(row[0].ToString(), row[1].ToString()));
            }
        }

        private void LoadComboboxHistoric()
        {
            comboboxHistoricStoreInit.Items.Clear();
            comboboxHistoricStoreFinal.Items.Clear();
            foreach (DataRow row in initialData.StoresData.Rows)
            {
                comboboxHistoricStoreInit.Items.Add(new MyDataComboBox(row[0].ToString(), row[1].ToString()));
                comboboxHistoricStoreFinal.Items.Add(new MyDataComboBox(row[0].ToString(), row[1].ToString()));
            }
            comboboxHistoricStoreInit.SelectedIndex = 0;
            comboboxHistoricStoreFinal.SelectedIndex = comboboxHistoricStoreFinal.Items.Count - 1;
        }

        private void LoadComboboxStatusTask()
        {
            comboboxHistoricStatusTask.Items.Clear();
            comboboxHistoricStatusTask.Items.Add(new MyDataComboBox(((int)TaskOperators.EnumTaskStatusTask.APROBADA).ToString(), TaskOperators.EnumTaskStatusTask.APROBADA.ToString()));
            comboboxHistoricStatusTask.Items.Add(new MyDataComboBox(((int)TaskOperators.EnumTaskStatusTask.PENDIENTE).ToString(), TaskOperators.EnumTaskStatusTask.PENDIENTE.ToString()));
            comboboxHistoricStatusTask.Items.Add(new MyDataComboBox(((int)TaskOperators.EnumTaskStatusTask.CERRADA).ToString(), TaskOperators.EnumTaskStatusTask.CERRADA.ToString()));
            comboboxHistoricStatusTask.Items.Add(new MyDataComboBox(((int)TaskOperators.EnumTaskStatusTask.DENEGADA).ToString(), TaskOperators.EnumTaskStatusTask.DENEGADA.ToString()));
            comboboxHistoricStatusTask.Items.Add(new MyDataComboBox(((int)TaskOperators.EnumTaskStatusTask.ERROR).ToString(), TaskOperators.EnumTaskStatusTask.ERROR.ToString()));
            comboboxHistoricStatusTask.Items.Add(new MyDataComboBox(((int)TaskOperators.EnumTaskStatusTask.NONE).ToString(), TaskOperators.EnumTaskStatusTask.NONE.ToString()));
            comboboxHistoricStatusTask.SelectedIndex = 5;
        }

        private void LoadComboboxStatusLocal()
        {
            comboboxHistoricStatusLocal.Items.Clear();
            comboboxHistoricStatusLocal.Items.Add(new MyDataComboBox(((int)TaskOperators.EnumTaskStatusLocal.EXITOSA).ToString(), TaskOperators.EnumTaskStatusLocal.EXITOSA.ToString()));
            comboboxHistoricStatusLocal.Items.Add(new MyDataComboBox(((int)TaskOperators.EnumTaskStatusLocal.FALLIDA).ToString(), TaskOperators.EnumTaskStatusLocal.FALLIDA.ToString()));
            comboboxHistoricStatusLocal.Items.Add(new MyDataComboBox(((int)TaskOperators.EnumTaskStatusLocal.NONE).ToString(), TaskOperators.EnumTaskStatusLocal.NONE.ToString()));
            comboboxHistoricStatusLocal.SelectedIndex = 2;
        }

        private void buttonPendingTaskApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if(comboboxPendinngFilterInit.SelectedIndex < 0 || comboboxPendinngFilterFinal.SelectedIndex < 0)
            {
                MessageBox.Show("Error debe seleccionar una tienda en el filtro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (Int32.Parse(((MyDataComboBox)comboboxPendinngFilterInit.SelectedItem).DisplayValue) <= Int32.Parse(((MyDataComboBox)comboboxPendinngFilterFinal.SelectedItem).DisplayValue))
                {
                    if (((Button)sender).Content.ToString() == "Aplicar Filtro")
                    {
                        ((Button)sender).Content = "Desactivar Filtro";
                    }
                    else
                    {
                        ((Button)sender).Content = "Aplicar Filtro";
                    }

                    GetPendingTaskFromStore();
                    dataGridPendingTask.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show("Error el codigo de la tienda inicial no puede ser mayor que la tienda final.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }

        private async void GetPendingTaskFromStore()
        {
            Tuple<bool, string, DataTable> result = await storeSQL.GetPendingStoresTaskAsync();
            if(result.Item1)
            {
                UpdateLabelStatus(labelStatusPendingTask, result.Item2, Brushes.Green);
                FillListboxPendingTask(result.Item3);
            }
            else
            {
                UpdateLabelStatus(labelStatusPendingTask, result.Item2, Brushes.Red);
                MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Tuple<bool, string, int> result2;

            if (buttonPendingTaskApplyFilter.Content.ToString() == "Desactivar Filtro")
            {
                result2 = await storeSQL.GetNumberPendingTaskAsync(true, ((MyDataComboBox)comboboxPendinngFilterInit.SelectedItem).DisplayValue, ((MyDataComboBox)comboboxPendinngFilterFinal.SelectedItem).DisplayValue);
            }
            else
            {
                result2 = await storeSQL.GetNumberPendingTaskAsync(false, string.Empty, string.Empty);
            }
            
            if(result2.Item1)
            {
                UpdateLabelStatus(labelStatusPendingTask, result2.Item2, Brushes.Green);
                UpdateCounterOfPendigTask(result2.Item3);
                if(result2.Item3 > 0)
                {
                    Notifications.ShowNotification("Hay " + result2.Item3 + " tareas pendientes.", TaskOperators.EnumTaskStatusTask.PENDIENTE);
                }
            }
            else
            {
                UpdateLabelStatus(labelStatusPendingTask, result2.Item2, Brushes.Red);
                MessageBox.Show(result2.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateCounterOfPendigTask(int newValue)
        {
            tabNumberPendingTask.Text = "(" + newValue.ToString() + ")";
        }

        private void UpdateLabelStatus(Label label, string text, Brush color)
        {         
            label.Content = text + GetTime();
            label.Foreground = color;
        }

        private void FillListboxPendingTask(DataTable dataTable)
        {
            listboxPendingTask.Items.Clear();
            foreach (DataRow row in dataTable.Rows)
            {
                if (buttonPendingTaskApplyFilter.Content.ToString() == "Desactivar Filtro" && Int32.Parse(row[0].ToString()) >= Int32.Parse(((MyDataComboBox)comboboxPendinngFilterInit.SelectedItem).DisplayValue)
                    && Int32.Parse(row[0].ToString()) <= Int32.Parse(((MyDataComboBox)comboboxPendinngFilterFinal.SelectedItem).DisplayValue))
                {
                    listboxPendingTask.Items.Add(CreateButtonPendigTask(row[0].ToString()));
                }
                else if (buttonPendingTaskApplyFilter.Content.ToString() == "Aplicar Filtro")
                {
                    listboxPendingTask.Items.Add(CreateButtonPendigTask(row[0].ToString()));
                }
            }

            if (listboxPendingTask.Items.Count <= 0)
            {
                dataGridPendingTask.ItemsSource = null;
            }
        }

        private Button CreateButtonPendigTask(string codeStore)
        {
            Button button = new Button();
            button.Content = codeStore;
            button.Background = Brushes.WhiteSmoke;
            button.Foreground = Brushes.DarkOrange;
            button.Width = 75;
            button.Height = 30;
            button.Margin = new Thickness(8);
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.FontSize = 12;
            button.FontWeight = FontWeights.Bold;
            button.Click += button_Click;
            return button;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(Int32.Parse(initialData.TIME_INTERVAL_CHECKING_TASK));
                backgroundWorker.ReportProgress(0, null);
            }
        }

        private void buttonRefreshPendigTask_Click(object sender, RoutedEventArgs e)
        {
            GetPendingTaskFromStore();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            GetPendingTaskFromStore();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            Tuple<bool, string, DataTable> result = await taskSQL.GetPendingTaskFromStoreAsync(Int32.Parse(((Button)sender).Content.ToString()));
            dataPendingTask = result.Item3;
            dataPendingTask.Columns.Add("SELECCION", typeof(bool)).SetOrdinal(0);
            foreach (DataRow row in dataPendingTask.Rows)
            {
                row[0] = false;
            }
            dataGridPendingTask.ItemsSource = dataPendingTask.DefaultView;
            dataGridPendingTask.CanUserAddRows = false;
            StyleDataGridPendingTask();
        }

        private void StyleDataGridPendingTask()
        {
            try
            {
                dataGridPendingTask.Columns[0].Width = 80;
                dataGridPendingTask.Columns[0].IsReadOnly = false;
                dataGridPendingTask.Columns[1].Visibility = Visibility.Hidden;
                dataGridPendingTask.Columns[1].IsReadOnly = true;
                dataGridPendingTask.Columns[2].Width = 80;
                dataGridPendingTask.Columns[2].IsReadOnly = true;
                dataGridPendingTask.Columns[3].Width = 100;
                dataGridPendingTask.Columns[3].IsReadOnly = true;
                dataGridPendingTask.Columns[4].Width = 100;
                dataGridPendingTask.Columns[4].IsReadOnly = true;
                dataGridPendingTask.Columns[5].Width = new DataGridLength(1, DataGridLengthUnitType.SizeToCells);
                dataGridPendingTask.Columns[5].IsReadOnly = true;
                dataGridPendingTask.Columns[6].Width = 220;
                dataGridPendingTask.Columns[6].IsReadOnly = false;
                dataGridPendingTask.Columns[7].Width = 140;
                dataGridPendingTask.Columns[7].IsReadOnly = true;
            }
            catch
            {
                ; //Do nothing
            }
        }

        private async void SetNewStatusToPendingTask(EnumTaskStatusTask newStatus)
        {
            for (int i = dataPendingTask.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dataPendingTask.Rows[i];
                if ((bool)row[0] == true)
                {
                    Tuple<bool, string> result = await taskSQL.SetTaskStatusAsync(Int32.Parse(row[1].ToString()), row[6].ToString(), intgerUpDownTokenTimeAlive.Value.Value, newStatus, initialData.UserIdAudit);
                    if (result.Item1)
                    {
                        row.Delete();
                        UpdateLabelStatus(labelStatusPendingTask, result.Item2, Brushes.Green);
                    }
                    else
                    {
                        UpdateLabelStatus(labelStatusPendingTask, result.Item2, Brushes.Red);
                        MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            dataPendingTask.AcceptChanges();

            if (dataPendingTask.Rows.Count <= 0)
            {
                GetPendingTaskFromStore();
            }
        }

        private void buttonAproveTasks_Click(object sender, RoutedEventArgs e)
        {
            SetNewStatusToPendingTask(EnumTaskStatusTask.APROBADA);
        }

        private void buttonDenyTasks_Click(object sender, RoutedEventArgs e)
        {
            SetNewStatusToPendingTask(EnumTaskStatusTask.DENEGADA);
        }

        private void buttonCloseTasks_Click(object sender, RoutedEventArgs e)
        {
            SetNewStatusToPendingTask(EnumTaskStatusTask.CERRADA);
        }

        private async void buttonDenyAllTasks_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultBox = MessageBox.Show("¿Esta seguro de denegar todas las tareas?", "Informacion", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (resultBox == MessageBoxResult.Yes)
            {
                Tuple<bool, string, DataTable> result = await taskSQL.GetAllPendingTaskAsync();
                if (result.Item1)
                {
                    foreach (DataRow row in result.Item3.Rows)
                    {
                        Tuple<bool, string> result2 = await taskSQL.SetTaskStatusAsync(Int32.Parse(row[0].ToString()), string.Empty, intgerUpDownTokenTimeAlive.Value.Value, TaskOperators.EnumTaskStatusTask.DENEGADA, initialData.UserIdAudit);
                        if (result2.Item1)
                        {
                            UpdateLabelStatus(labelStatusPendingTask, result.Item2, Brushes.Green);
                        }
                        else
                        {
                            UpdateLabelStatus(labelStatusPendingTask, result.Item2, Brushes.Red);
                            MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    dataGridPendingTask.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                GetPendingTaskFromStore();
            }
            
        }

        private void comboboxEditStore_DropDownClosed(object sender, EventArgs e)
        {
            foreach (DataRow row in initialData.StoresData.Rows)
            {
                if(comboboxEditStore.SelectedIndex != -1 && row[1].ToString() == ((MyDataComboBox)comboboxEditStore.SelectedItem).DisplayValue)
                {
                    DataTable aux = new DataTable();
                    aux.Columns.Add("ID", typeof(string));
                    aux.Columns.Add("CODIGO_TIENDA", typeof(string));
                    aux.Rows.Add(row[0].ToString(), row[1].ToString());
                }
            }             
        }

        private async void buttonDeleteStore_Click(object sender, RoutedEventArgs e)
        {
            if(comboboxEditStore.SelectedIndex != -1)
            {
                Tuple<bool, string> result = await storeEmployeeSQL.StoreHasEmployeesAsync(Int32.Parse(((MyDataComboBox)comboboxEditStore.SelectedItem).Value),
                                                                                           Int32.Parse(((MyDataComboBox)comboboxEditStore.SelectedItem).DisplayValue));

                if (!result.Item1)
                {
                    Tuple<bool, string> result2 = await storeSQL.EraseStoreAsync(Int32.Parse(((MyDataComboBox)comboboxEditStore.SelectedItem).Value), initialData.UserIdAudit);
                    if (result2.Item1)
                    {
                        Tuple<bool, string, DataTable> result3 = await storeSQL.GetStoresAsync();
                        {
                            initialData.StoresData = result3.Item3;
                            LoadEditStores();
                            comboboxEditStore.SelectedIndex = -1;
                            LoadFilterStores();
                            LoadAddEditUserStore();
                            LoadComboboxHistoric();
                        }
                        MessageBox.Show(result2.Item2, "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(result2.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }  
            }
            else
            {
                MessageBox.Show("Error debe seleccionar una tienda.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void buttonAddStore_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(textboxAddStore.Text))
            {

                try
                {
                    Tuple<bool, string> result = await storeSQL.InsertStoreAsync(Int32.Parse(textboxAddStore.Text.Trim()), initialData.UserIdAudit);

                    if (result.Item1)
                    {
                        Tuple<bool, string, DataTable> result2 = await storeSQL.GetStoresAsync();
                        {
                            initialData.StoresData = result2.Item3;
                            LoadEditStores();
                            LoadFilterStores();
                            LoadAddEditUserStore();
                            LoadComboboxHistoric();
                        }
                        MessageBox.Show(result.Item2, "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch(Exception ex)
                {
                    Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                    MessageBox.Show("Error: " + ex.Message.ToLower(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Error debe ingresar un campo de tienda valido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void comboboxEditUserStore_DropDownClosed(object sender, EventArgs e)
        {
            if(comboboxEditUserStore.SelectedIndex != -1)
            {
                textboxEditUserStore.Text = ((MyDataComboBox)comboboxEditUserStore.SelectedItem).DisplayValue;

                foreach(DataRow row in initialData.StoresEmployeeData.Rows)
                {
                    if(((MyDataComboBox)comboboxEditUserStore.SelectedItem).DisplayValue == row[1].ToString())
                    {
                        foreach (MyDataComboBox item in comboboxEditUserStoreSelectStore.Items)
                        {
                            if (item.DisplayValue.ToString() == row[2].ToString())
                            {
                                comboboxEditUserStoreSelectStore.SelectedValue = item;
                                break;
                            }
                        }
                        break;   
                    }
                }
            }
        }
        
        private async void buttonEditUserStore_Click(object sender, RoutedEventArgs e)
        {
            if (comboboxEditUserStore.SelectedIndex != -1)
            {
                if(!string.IsNullOrWhiteSpace(textboxEditUserStore.Text))
                {
                    Tuple<bool, string> result = await storeEmployeeSQL.EditUserStoreAsync(
                        Int32.Parse(((MyDataComboBox)comboboxEditUserStore.SelectedItem).Value), 
                        textboxEditUserStore.Text,
                        Int32.Parse(((MyDataComboBox)comboboxEditUserStoreSelectStore.SelectedItem).Value), 
                        initialData.UserIdAudit);

                    if (result.Item1)
                    {
                        Tuple<bool, string, DataTable> result2 = await storeEmployeeSQL.GetStoresEmployeeAsync();
                        {
                            initialData.StoresEmployeeData = result2.Item3;
                            textboxEditUserStore.Text = string.Empty;
                            comboboxEditUserStore.SelectedIndex = -1;
                            comboboxEditUserStoreSelectStore.SelectedIndex = -1;
                            LoadEditUserStores();
                        }
                        MessageBox.Show(result.Item2, "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Error debe ingresar un campo de usuario de tienda valido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
            else
            {
                MessageBox.Show("Error debe seleccionar un usuario de tienda.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void buttonDeleteUserStore_Click(object sender, RoutedEventArgs e)
        {
            if (comboboxEditUserStore.SelectedIndex != -1)
            {
                Tuple<bool, string> result = await storeEmployeeSQL.EraseUserStoreAsync(
                        Int32.Parse(((MyDataComboBox)comboboxEditUserStore.SelectedItem).Value),
                        initialData.UserIdAudit);
                if (result.Item1)
                {
                    Tuple<bool, string, DataTable> result2 = await storeEmployeeSQL.GetStoresEmployeeAsync();
                    {
                        initialData.StoresEmployeeData = result2.Item3;
                        textboxEditUserStore.Text = string.Empty;
                        comboboxEditUserStore.SelectedIndex = -1;
                        comboboxEditUserStoreSelectStore.SelectedIndex = -1;
                        LoadEditUserStores();
                    }
                    MessageBox.Show(result.Item2, "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Error debe seleccionar un usuario de tienda.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void buttonAddUserStore_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(textboxAddUserStore.Text))
            {
                if(comboboxAddUserStoreSelectStore.SelectedIndex != -1)
                {
                    Tuple<bool, string> result = await storeEmployeeSQL.InsertEmployStoreAsync(
                        textboxAddUserStore.Text,
                        Int32.Parse(((MyDataComboBox)comboboxAddUserStoreSelectStore.SelectedItem).Value),
                         initialData.UserIdAudit);
                    if (result.Item1)
                    {
                        Tuple<bool, string, DataTable> result2 = await storeEmployeeSQL.GetStoresEmployeeAsync();
                        {
                            initialData.StoresEmployeeData = result2.Item3;
                            textboxEditUserStore.Text = string.Empty;
                            comboboxEditUserStore.SelectedIndex = -1;
                            comboboxEditUserStoreSelectStore.SelectedIndex = -1;
                            textboxAddUserStore.Text = string.Empty;
                            comboboxAddUserStoreSelectStore.SelectedIndex = -1;
                            LoadEditUserStores();
                        }
                        MessageBox.Show(result.Item2, "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Error debe seleccionar una tienda.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Error debe ingresar un campo de usuario de tienda valido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void buttonEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (comboboxEditUser.SelectedIndex != -1)
            {
                if (!string.IsNullOrWhiteSpace(textboxEditUser.Text))
                {
                    Tuple<bool, string> result = await userSQL.EditUserAsync(Int32.Parse(((MyDataComboBox)comboboxEditUser.SelectedItem).Value), textboxEditUser.Text, initialData.UserIdAudit);

                    if (result.Item1)
                    {
                        Tuple<bool, string, DataTable> result2 = await userSQL.GetUsersAsync();
                        {
                            initialData.UsersData = result2.Item3;
                            LoadEditUser();
                            textboxEditUser.Text = string.Empty;
                            comboboxEditUserStore.SelectedIndex = -1;
                        }
                        MessageBox.Show(result.Item2, "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Error el campo es invalido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Error debe seleccionar un usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void comboboxEditUser_DropDownClosed(object sender, EventArgs e)
        {
            if(comboboxEditUser.SelectedIndex != -1)
            {
                textboxEditUser.Text = ((MyDataComboBox)comboboxEditUser.SelectedItem).DisplayValue;
            }
        }

        private async void buttonDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (comboboxEditUser.SelectedIndex != -1)
            {
                if(Int32.Parse(((MyDataComboBox)comboboxEditUser.SelectedItem).Value) != initialData.UserIdAudit)
                {
                    Tuple<bool, string> result = await userSQL.EraseUserAsync(Int32.Parse(((MyDataComboBox)comboboxEditUser.SelectedItem).Value), initialData.UserIdAudit);
                    if (result.Item1)
                    {
                        Tuple<bool, string, DataTable> result2 = await userSQL.GetUsersAsync();
                        {
                            initialData.UsersData = result2.Item3;
                            LoadEditUser();
                            textboxEditUser.Text = string.Empty;
                            comboboxEditUserStore.SelectedIndex = -1;
                        }
                        MessageBox.Show(result.Item2, "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Usuario en uso no se puede eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Error debe seleccionar un usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void buttonAddUser_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(textboxAddUser.Text))
            {
                Tuple<bool, string> result = await userSQL.InsertUserAsync(textboxAddUser.Text, initialData.UserIdAudit);
                if (result.Item1)
                {
                    Tuple<bool, string, DataTable> result2 = await userSQL.GetUsersAsync();
                    {
                        initialData.UsersData = result2.Item3;
                        LoadEditUser();
                        textboxEditUser.Text = string.Empty;
                        comboboxEditUserStore.SelectedIndex = -1;
                    }
                    MessageBox.Show(result.Item2, "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(result.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Error el campo es invalido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DataTable historicToExcelDatatTable = new DataTable();

        private async void buttonHistoricConsult_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.Parse(((MyDataComboBox)comboboxHistoricStoreInit.SelectedItem).DisplayValue) <= Int32.Parse(((MyDataComboBox)comboboxHistoricStoreFinal.SelectedItem).DisplayValue))
            {
                Tuple<bool, string, DataTable> result = await taskSQL.GetHistoricTaskAsync(Int32.Parse(((MyDataComboBox)comboboxHistoricStoreInit.SelectedItem).DisplayValue),
                    Int32.Parse(((MyDataComboBox)comboboxHistoricStoreFinal.SelectedItem).DisplayValue),
                    (TaskOperators.EnumTaskStatusTask)(Int32.Parse(((MyDataComboBox)comboboxHistoricStatusTask.SelectedItem).Value)),
                    (TaskOperators.EnumTaskStatusLocal)(Int32.Parse(((MyDataComboBox)comboboxHistoricStatusLocal.SelectedItem).Value)),
                    datePickerHistoric.SelectedDate.Value
                    );
                if(result.Item1)
                {
                    historicToExcelDatatTable = result.Item3;
                    dataGridHistoric.ItemsSource = result.Item3.DefaultView;
                    MessageBox.Show(result.Item2, "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    dataGridHistoric.ItemsSource = null;
                   MessageBox.Show(result.Item2, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Error el codigo de la tienda inicial no puede ser mayor que la tienda final.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void buttonHistoricExportExcel_Click(object sender, RoutedEventArgs e)
        {
            buttonHistoricExportExcel.IsEnabled = false;
            buttonHistoricConsult.IsEnabled = false;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save XLS File";
            saveFileDialog1.FileName = "Autogestion-" + datePickerHistoric.SelectedDate.Value.ToString("dd-MM-yyyy");
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "XLS";
            saveFileDialog1.Filter = "XLS files (*.xls)|*.xls";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == true)
            {
                if(await ManageExcel.Create(historicToExcelDatatTable, saveFileDialog1.FileName) == true)
                {
                    MessageBox.Show("Archivo guardado con exito.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error al guardar archivo excel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            buttonHistoricExportExcel.IsEnabled = true;
            buttonHistoricConsult.IsEnabled = true;
        }
    }
}
