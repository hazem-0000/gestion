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
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private FirestoreService firestoreService;
        private ObservableCollection<String> quals = new ObservableCollection<String>();
        public Employee SelectedEmployee { get; set; }
        public Qualification SelectedQualification { get; set; }

        private List<Qualification> existingQualifications = new List<Qualification>();
        private Dictionary<string, string> originalQualificationLevels = new Dictionary<string, string>();
        public EditWindow(Employee employee)
        {
            InitializeComponent();
            firestoreService = new FirestoreService("cofat-41b6e", GlobalState.AuthToken);
            firestoreService.GetQualifications(quals);
            QualificationComboBox.ItemsSource = quals;
            SelectedEmployee = employee;
            DataContext = SelectedEmployee;

           
            
        }

        private void AddQualification_Button(object sender, RoutedEventArgs e)
        {
            if (QualificationComboBox.SelectedItem != null && LevelComboBox.SelectedItem != null)
            {
                var qualificationName = QualificationComboBox.SelectedItem.ToString();
                var qualificationLevel = ((ComboBoxItem)LevelComboBox.SelectedItem).Content.ToString();
                var qualificationDate = DateOnly.FromDateTime(DateTime.Now).ToString();

                bool qualificationExists = existingQualifications.Any(q => q.Name == qualificationName);

                Qualification newQualification = new Qualification(qualificationName, qualificationLevel, qualificationDate);

                if (qualificationExists)
                {
                    var existingQualification = existingQualifications.First(q => q.Name == qualificationName);
                    // Check if the level has changed
                    if (!originalQualificationLevels.TryGetValue(qualificationName, out string originalLevel))
                    {
                        // If the original level is not found, consider it as a new qualification
                        existingQualification.State = true;
                    }
                    else
                    {
                        // Check if the level has changed
                        existingQualification.State = qualificationLevel != originalLevel;
                    }
                    existingQualification.Level = qualificationLevel;
                }
                else
                {
                    existingQualifications.Add(newQualification);
                    SelectedEmployee.Qualifications.Add(newQualification);
                    // Store the original level for this qualification
                    originalQualificationLevels.Add(qualificationName, qualificationLevel);
                }

                QualificationsDataGrid.ItemsSource = SelectedEmployee.Qualifications;

                QualificationComboBox.SelectedItem = null;
                LevelComboBox.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner à la fois la qualification et le niveau.");
            }
        }



        private void DeleteQualification_Button(object sender, RoutedEventArgs e)
        {
            if (SelectedEmployee != null && SelectedQualification != null)
            {
                // Remove the selected qualification from the employee's qualifications list
                var qualificationToRemove = SelectedQualification;
                SelectedEmployee.Qualifications.Remove(qualificationToRemove);


                // Refresh DataGrid
                QualificationsDataGrid.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une qualification à supprimer");
            }



        }

        private void SaveChanges_Button(object sender, RoutedEventArgs e) { 
         try
    {
        // Call the UpdateEmployeeByCINAsync method to update the employee
        firestoreService.UpdateEmployeeByCINAsync(SelectedEmployee);

        // If the update is successful, close the window and show a success message
        
        this.DialogResult = true; // This will close the window with DialogResult set to true
    }
    catch (Exception ex)
    {
        // Handle any exceptions that may occur during the update process
        // For example, log the exception or show an error message to the user
        MessageBox.Show($"Échec de l'enregistrement des modifications: {ex.Message}");
    }

            }
    }
}
