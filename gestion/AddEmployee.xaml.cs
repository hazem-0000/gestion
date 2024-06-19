using Google.Cloud.Firestore;
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
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        private FirestoreService firestoreService;
        private ObservableCollection<string> quals=new ObservableCollection<string>();

        private Employee currentEmployee = new Employee();
        private  ObservableCollection<Employee> employee;
        public event Action<Employee> EmployeeAdded;

        public AddEmployee(ObservableCollection<Employee> employee)
        {
             

            InitializeComponent();
            this.employee = employee;
            firestoreService = new FirestoreService("cofat-41b6e", GlobalState.AuthToken);
            currentEmployee.Qualifications = new ObservableCollection<Qualification>();
            firestoreService.GetQualifications(quals);
            QualificationComboBox.ItemsSource = quals;

        }

        private void Add_Qualification_Click(object sender, RoutedEventArgs e)
        {
            if (QualificationComboBox.SelectedItem != null && QualificationLevelComboBox.SelectedItem != null)
            {
                var qualificationName = QualificationComboBox.SelectedItem.ToString();
                var qualificationLevel = ((ComboBoxItem)QualificationLevelComboBox.SelectedItem).Content.ToString();
                var qualificationDate = DateOnly.FromDateTime(DateTime.Now).ToString(); // Assuming this is how you obtain the date
                var qualificationState=true;
                // Create a new Qualification object
                Qualification newQualification = new Qualification
                {
                    Name = qualificationName,
                    Level = qualificationLevel,
                    Date = qualificationDate,
                    State = qualificationState,
                };

                // Add the new qualification to the currentEmployee's Qualifications collection
                currentEmployee.Qualifications.Add(newQualification);

                // Clear selection in ComboBoxes
                QualificationComboBox.SelectedItem = null;
                QualificationLevelComboBox.SelectedItem = null;

                // Optionally, you can refresh the ItemsSource of the ListBox (lista) to reflect the changes
                lista.ItemsSource = currentEmployee.Qualifications;
               
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner à la fois la qualification et le niveau.");
            }
        
    }
        private void Add_Employee_Click(object sender, RoutedEventArgs e)
        {
            // Check if all required fields are filled
            if (!string.IsNullOrEmpty(NameTextBox.Text) && !string.IsNullOrEmpty(CINTextBox.Text))
            {
                if (CINTextBox.Text.Length != 8)
                {
                    MessageBox.Show("CIN doit comporter exactement 8 chiffres.", "CIN pas valide", MessageBoxButton.OK, MessageBoxImage.Error);
                    CINTextBox.Focus();
                    return; // Exit the method early if CIN length is not 8
                }

                // Check if the employee with the same CIN already exists
                int cin = int.Parse(CINTextBox.Text);
                if (employee.Any(emp => emp.CIN == cin))
                {
                    MessageBox.Show($"Employee with CIN {cin}Déjà existant.", "Duplicate Employee", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Exit the method if employee already exists
                }

                currentEmployee.Name = NameTextBox.Text;
                currentEmployee.CIN = int.Parse(CINTextBox.Text);
                currentEmployee.Project = ProjectTextBox.Text;
                currentEmployee.Famille = FamilleTextBox.Text;
                currentEmployee.Zone = ZoneTextBox.Text;
               

                firestoreService.Add_Employee2(currentEmployee);
                // Add the new employee to the ObservableCollection
                employee.Add(currentEmployee);

                // Clear input fields for the next entry
                NameTextBox.Text = "";
                CINTextBox.Text = "";
                ProjectTextBox.Text = "";
                FamilleTextBox.Text = "";
                ZoneTextBox.Text = "";

                // Clear qualifications list
                currentEmployee.Qualifications.Clear();
            }
            else
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
            }
        }


    }
}
