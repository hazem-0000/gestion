using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace gestion
{
    /// <summary>
    /// Interaction logic for Attention.xaml
    /// </summary>
    public partial class Attention : Window
    {   
      
        private Employeetable here = new Employeetable();
        public ObservableCollection<Employee> employees = new ObservableCollection<Employee>();
        public FirestoreService firestoreService;
        public Attention(ObservableCollection<Employee> employees)
        {
            InitializeComponent();
            dataGrid.ItemsSource = employees;


        
        }
    
    private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                // Check if all items are currently selected
                if (dataGrid.SelectedItems.Count == dataGrid.Items.Count)
                {
                    // Deselect all items if all are currently selected
                    dataGrid.SelectedItems.Clear();
                }
                else
                {
                    // Select all items if not all are currently selected
                    dataGrid.SelectAll();
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Employee selectedEmployee)
            {
                var editWindow = new EditWindow(selectedEmployee);
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    // Handle the case where the user saved changes
                    dataGrid.Items.Refresh(); // Refresh the DataGrid to reflect changes
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un employé.");
            }
        }

       
        
        private void DeleteEmployee_Button(object sender, RoutedEventArgs e)
        {
           here.firestoreService.RemoveSelectedEmployee(dataGrid,employees);
        }
      



    }
}
