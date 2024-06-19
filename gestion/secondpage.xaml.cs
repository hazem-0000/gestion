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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace gestion
{
    /// <summary>
    /// Interaction logic for secondpage.xaml
    /// </summary>
    public partial class Secondpage : Page

    {
        private FirestoreService firestoreService;

        ObservableCollection<string> quals = new ObservableCollection<string>();
        public Secondpage()
        {

            firestoreService = new FirestoreService("cofat-41b6e", GlobalState.AuthToken);

            // firestoreService = new FirestoreService("cofat-832c3");
            InitializeComponent();
            firestoreService.GetQualifications(quals);
            QualificationsList.ItemsSource = quals;


            /* Task.Run(async () =>
             {
                 ObservableCollection<string> quals = await firestoreService.GetAllQualificationsAsync();
                 // Update UI with the retrieved employees
             }).Wait();

            DataContext = quals;*/

        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            await firestoreService.AddStringToArrayInDocument("Qualifications", "Qualifications", QualificationInput.Text);
            quals.Clear();
            firestoreService.GetQualifications(quals);
            QualificationsList.ItemsSource= quals;
            
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string selected = QualificationsList.SelectedItem.ToString();
            await firestoreService.RemoveStringFromArrayInDocument("Qualifications", "Qualifications",selected );
            quals.Clear();
            firestoreService.GetQualifications(quals);
            QualificationsList.ItemsSource = quals;

        }

        

        
    }
}
    

