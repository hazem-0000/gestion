using System.Configuration;
using System.Data;
using System.Windows;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static Google.Cloud.Firestore.V1.StructuredAggregationQuery.Types.Aggregation.Types;

namespace gestion
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize Firebase with your configuration
            FirebaseUI.Initialize(new FirebaseUIConfig
            {
                ApiKey = "AIzaSyD0w78QoJnsZExx9ckQVNTilbdscFGiDaM",
                AuthDomain = "cofat-41b6e.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
     {
        new GoogleProvider().AddScopes("email"),
        new EmailProvider(),
        
       
     },
                PrivacyPolicyUrl = "https://example.com/privacy-policy",
                TermsOfServiceUrl = "https://example.com/terms-of-service",
                IsAnonymousAllowed = true,
                UserRepository = new FileUserRepository("FirebaseSample") // persist data into %AppData%\FirebaseSample
            });
            
        }
    }
}
