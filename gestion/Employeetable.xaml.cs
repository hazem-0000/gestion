using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace gestion
{
    /// <summary>
    /// Interaction logic for employeetable.xaml
    /// </summary>
    public partial class Employeetable : Page

    {
       public bool Exist;
       
        //public bool ShouldApplyFilter { get; set; }
        public ObservableCollection<Employee> employee = new ObservableCollection<Employee>();
        
        public FirestoreService firestoreService;
        private ICollectionView FilteredEmployees;
        private DispatcherTimer timer;

        public Employeetable()
        {
            
            InitializeComponent();
           
            firestoreService = new FirestoreService("cofat-41b6e", GlobalState.AuthToken);
            firestoreService.GetEmployeesWithQualifications(employee);
            dataGrid.ItemsSource = employee;
            

            FilteredEmployees = CollectionViewSource.GetDefaultView(employee);
            dataGrid.ItemsSource = FilteredEmployees;
            
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(3);
                        timer.Tick += Timer_Tick;

                        timer.Start();
           


        }
        public Employeetable(string name, string cin, string project, string famille, string zone,string qualificationName,List<string> levels)
        {

           
            InitializeComponent();
            
            firestoreService = new FirestoreService("cofat-41b6e", GlobalState.AuthToken);
           firestoreService.GetEmployeesWithQualifications(employee);
            dataGrid.ItemsSource = employee;
            

            FilteredEmployees = CollectionViewSource.GetDefaultView(employee);
        dataGrid.ItemsSource = FilteredEmployees;

           

            // Perform filtering or other operations with the received data
           ApplyFilter(name, cin, project, famille, zone,qualificationName,levels);



            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;

            timer.Start();
         


        }



        public void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            // Ensure the DataGrid is not null
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

        public string EmployeeCountText
        {
            get { return $"{dataGrid.Items.Count} Employés"; }
        }


        public void DeleteEmployee_Button(object sender, RoutedEventArgs e)
        {
            firestoreService.RemoveSelectedEmployee(dataGrid, employee);



        }






        public void Ajout(object sender, RoutedEventArgs e)
        {
            AddEmployee addEmployee = new AddEmployee(employee);
            addEmployee.Show();
         
        }

        public void EditButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Veuillez sélectionner un employé à modifier");
            }
        }


        public void ApplyFilter(string name, string cin, string project, string famille, string zone, string qualificationName, List<string> qualificationLevels)
        {
            if (FilteredEmployees == null)
                return;

            // Parse CIN filter input to integer, if it's not empty and a valid integer
            bool cinParsed = int.TryParse(cin, out int cinInt);

            FilteredEmployees.Filter = item =>
            {
                if (item is Employee employee)
                {
                    bool nameMatches = string.IsNullOrEmpty(name) || employee.Name.ToLower().Contains(name.ToLower());
                    bool cinMatches = string.IsNullOrEmpty(cin) || (cinParsed && employee.CIN == cinInt);
                    bool projectMatches = string.IsNullOrEmpty(project) || employee.Project.ToLower().Contains(project.ToLower());
                    bool familleMatches = string.IsNullOrEmpty(famille) || employee.Famille.ToLower().Contains(famille.ToLower());
                    bool zoneMatches = string.IsNullOrEmpty(zone) || employee.Zone.ToLower().Contains(zone.ToLower());

                    bool qualificationNameMatches = string.IsNullOrEmpty(qualificationName) ||
                        employee.Qualifications.Any(q => q.Name.ToLower().Contains(qualificationName.ToLower()));

                    bool qualificationLevelMatches = qualificationLevels.Count == 0 ||
                        employee.Qualifications.Any(q => qualificationLevels.Contains(q.Level));

                    return nameMatches && cinMatches && projectMatches && familleMatches && zoneMatches &&
                        qualificationNameMatches && qualificationLevelMatches;
                }

                return false;
            };
        }



        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Employee> filteredEmployees = GetAttention();
            Attention attentionWindow = new Attention(filteredEmployees);
            attentionWindow.Show();
        }

        public ObservableCollection<Employee> GetAttention()
        {
            ObservableCollection<Employee> filteredEmployees = new ObservableCollection<Employee>();
            DateTime currentDate = DateTime.Today;

            foreach (Employee employee in dataGrid.ItemsSource)
            {
                bool hasYellowQualification = false;
                bool hasRedQualification = false;
                bool hasSCGreenQualification = false;
                bool hasZRGreenQualification = false;

                foreach (Qualification qualification in employee.Qualifications)
                {
                    // Check for SC qualification first
                    if (qualification.Name == "S.C assemblage" && qualification.Level == "Vert")
                    {
                        // Check if the date is at least 174 days away from today
                        if (IsQualificationDateWithinRange(qualification.Date, currentDate, 174))
                        {
                            hasSCGreenQualification = true;
                            break; // No need to check other qualifications for this employee
                        }
                    }
                    // Check for ZR qualification next
                    else if (qualification.Name == "Retouche cablage en ligne" && qualification.Level == "Vert")
                    {
                        // Check if the date is at least 83 days away from today
                        if (IsQualificationDateWithinRange(qualification.Date, currentDate, 83))
                        {
                            hasZRGreenQualification = true;
                            break; // No need to check other qualifications for this employee
                        }
                    }
                    // Check for Yellow qualification
                    else if (qualification.Level == "Jaune")
                    {
                        // Check if the state is true and date is within the specified range
                        if (qualification.State && IsQualificationDateWithinRange(qualification.Date, currentDate, 55))
                        {
                            hasYellowQualification = true;
                            break; // No need to check other qualifications for this employee
                        }
                        // Check if the state is false and date is within the specified range
                        else if (!qualification.State && IsQualificationDateWithinRange(qualification.Date, currentDate, 10))
                        {
                            hasYellowQualification = true;
                            break; // No need to check other qualifications for this employee
                        }
                    }
                    // Check for Red qualification
                    else if (qualification.Level == "Rouge")
                    {
                        // Check if the state is true and date is within the specified range
                       
                        
                            hasRedQualification = true;
                            break; // No need to check other qualifications for this employee
                        
                    }
                }

                // Add the employee to the filtered list if it has the required qualifications
                if (hasSCGreenQualification || hasZRGreenQualification || hasYellowQualification || hasRedQualification)
                {
                    filteredEmployees.Add(employee);
                }
            
            

            }
           Array state = filteredEmployees.ToArray(); 
            int T = state.Length;
            if (T != 0)
            {
                Alert.Background = new SolidColorBrush(Colors.Yellow);

            }
            else Alert.Background = new SolidColorBrush(Colors.Gray);

            return filteredEmployees;
           
        }

        private bool IsQualificationDateWithinRange(string dateString, DateTime currentDate, int days)
        {
            DateTime qualificationDate;
            if (DateTime.TryParse(dateString, out qualificationDate))
            {
                TimeSpan difference = currentDate - qualificationDate;
                return difference.TotalDays >= days;
            }
            return false; // Return false if parsing fails
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // This event handler is invoked after the specified interval (3 seconds)
            // Invoke the GetAttention method
            GetAttention();
            timer.Stop();
        }
    }
}
