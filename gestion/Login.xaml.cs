using Firebase.Auth.UI;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Firebase.Auth;
using System.Windows;

namespace gestion
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public event Action<string> Authenticated;
        public Login()
        {
            InitializeComponent();
            FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
            
            
           
        }
        private void AuthStateChanged(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (e.User == null)
                {
                    // No user is signed in, show login UI
                    this.ShowLoginUI();
                    //var userCredential = await FirebaseUI.Instance.Client.SignInWithEmailAndPasswordAsync("hazemsaadani2014@gmail.com", "Viper20002");
                  //  var user = userCredential.User;
                   

                  
                }
                else
                {
                   
                    // User signed in, hide login UI
                    this.HideLoginUI();
                    var token=await e.User.GetIdTokenAsync();

                    GlobalState.AuthToken = token.ToString();

                }
            });
        }
            private void ShowLoginUI()
            {
                this.Visibility = Visibility.Visible;
            }

            private void HideLoginUI()
            {
                this.Visibility = Visibility.Collapsed;
            }
        
    }
}

