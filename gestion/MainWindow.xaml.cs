using Google.Api;
using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
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
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;
using System.Security.RightsManagement;
using System.Security.Policy;
using System.Xml.Linq;
using Google.Api.Gax.ResourceNames;
using System.ComponentModel;






namespace gestion
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> qualificationLevels = new List<string>();
      
        FirestoreDb database;
        public event EventHandler FilterApplied;

        
       

        public MainWindow()
        {


            InitializeComponent();
           


            this.Loaded += MainWindow_Loaded;
           

        }
       


        private void Qualifications(object sender, RoutedEventArgs e)
        {
            MainFrame.Visibility = System.Windows.Visibility.Visible;
            MainFrame.Navigate(new Secondpage());
           
        }




        private void employees_list_button(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string cin = CINTextBox.Text;
            string project = ProjectTextBox.Text;
            string famille = FamilleTextBox.Text;
            string zone = ZoneTextBox.Text;
            string qualificationName = QualificationNameTextBox.Text;

            List<string> levels = qualificationLevels;
            MainFrame.Visibility = System.Windows.Visibility.Visible;
            Employeetable table=  new Employeetable(name, cin, project, famille, zone, qualificationName, levels);
            
                MainFrame.Navigate(table);
            

           
        }

      /*  void Add_Employee(Employee employee1)
        {
            CollectionReference coll = database.Collection("Employees");

            // Firestore handles the serialization of custom objects as long as they have a parameterless constructor
            // Since your Employee class does not have a parameterless constructor, consider adding one or use a factory method to create employees
            coll.AddAsync(employee1).ContinueWith(task =>
            {
                if (task.Exception != null)
                    MessageBox.Show($"Error adding document: {task.Exception}");
                else
                    MessageBox.Show("Employee added successfully.");
            });
        }
      */
        


        public void ApplyFilter_Button(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string cin = CINTextBox.Text;
            string project = ProjectTextBox.Text;
            string famille = FamilleTextBox.Text;
            string zone = ZoneTextBox.Text;
            string qualificationName = QualificationNameTextBox.Text;
            List<string> levels = qualificationLevels;



            // Navigate to the Employeetable page and pass the text as parameters
            Employeetable employeetable = new Employeetable(name, cin, project, famille, zone, qualificationName, levels);
            MainFrame.Content = employeetable;



        }
        
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                string level = checkBox.Content.ToString();
                qualificationLevels.Add(level);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                string level = checkBox.Content.ToString();
                qualificationLevels.Remove(level);
            }
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            FirebaseUI.Instance.Client.SignOut();
           
        }

        async private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
          
            // Load the LoginPage when the main window is loaded
            MainFrame.Visibility = System.Windows.Visibility.Collapsed;
            login.Visibility=System.Windows.Visibility.Visible;
            login.Navigate(new Login());
           
            FirebaseUI.Instance.Client.AuthStateChanged += AuthStateChanged;
        } // Subscribe to auth state changes to switch to the main content when the user is authenticated
        private void AuthStateChanged(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e.User != null)
                {
                    login.Visibility= System.Windows.Visibility.Collapsed;
                    MainFrame.Visibility=System.Windows.Visibility.Visible;
                    MainFrame.Navigate(new Employeetable());
                }
                else
                {
                    MainFrame.Visibility = System.Windows.Visibility.Collapsed;
                    login.Visibility = System.Windows.Visibility.Visible;
                    login.Navigate(new Login());

                }
            });
        }
    }
}

