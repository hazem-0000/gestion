using Firebase.Auth;
using Firebase.Auth.UI;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Grpc.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

namespace gestion
{
    public class FirestoreService
    {
        private FirestoreDb db;




        public FirestoreService(string projectId, string token)
        {


            // Initialize FirestoreDb with authentication token
            //string path = AppDomain.CurrentDomain.BaseDirectory + @"key.json";
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            var credential =  GoogleCredential.FromAccessToken(token);

            // Create a ChannelCredentials using the GoogleCredential
              var channelCredentials = credential.ToChannelCredentials();

            // Create a FirestoreClient using the ChannelCredentials
              var client = new FirestoreClientBuilder
               {
                   ChannelCredentials = channelCredentials
               }.Build();
               db = FirestoreDb.Create(projectId,client);
               
        }



        public void Add_Employee2(Employee employee)
        {
            CollectionReference coll = db.Collection("Employees");

            try
            {
                // Serialize the Qualifications list into a list of dictionaries
                List<Dictionary<string, object>> qualificationList = new List<Dictionary<string, object>>();
                foreach (var qualification in employee.Qualifications)
                {
                    Dictionary<string, object> qualificationData = new Dictionary<string, object>()
            {
                { "Name", qualification.Name },
                { "Level", qualification.Level.ToString() },
                { "Date", qualification.Date.ToString() },
                { "State", qualification.State }
            };
                    qualificationList.Add(qualificationData);
                }

                // Create the employee data dictionary
                Dictionary<string, object> employeeData = new Dictionary<string, object>()
        {
            { "Name", employee.Name },
            { "CIN", employee.CIN },
            { "Project", employee.Project },
            { "Zone", employee.Zone },
            { "Famille", employee.Famille },
            { "Qualifications", qualificationList } // Add the serialized qualifications list
        };

                // Add employee data to Firestore collection
                coll.AddAsync(employeeData).Wait(); // Wait for the task to complete to catch exceptions
                MessageBox.Show("Employé ajouté avec succès");
            }
            catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
            {
                MessageBox.Show("L'opération a échoué : Vous n'avez pas la permission d'effectuer cette action.", "Erreur d'autorisation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"L'opération a échoué : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }






        public async Task GetQualifications(ObservableCollection<string> quals)
        {
            try
            {
                DocumentReference docref = db.Collection("Qualifications").Document("Qualifications");
                DocumentSnapshot snap = await docref.GetSnapshotAsync();
                if (snap.Exists)
                {
                    Dictionary<string, object> data = snap.ToDictionary();
                    if (data.TryGetValue("Qualifications", out object arrayObject) && arrayObject is List<object> array)
                    {
                        foreach (var item in array)
                        {
                            string value = item.ToString(); // Convert array element to string
                            quals.Add(value);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Array field does not exist in the document or is not of the expected type.");
                    }
                }
                else
                {
                    MessageBox.Show("Document does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public async Task AddStringToArrayInDocument(string documentId, string fieldName, string stringValue)
        {
            try
            {
                DocumentReference docRef = db.Collection("Qualifications").Document(documentId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    // Retrieve existing array from the document
                    List<object> existingArray = snapshot.GetValue<List<object>>(fieldName) ?? new List<object>();

                    if (existingArray.Contains(stringValue))
                    {
                        MessageBox.Show("L'élément existe déjà dans la base de données");
                        return; // Exit method if item already exists
                    }

                    // Add the new string value to the array
                    existingArray.Add(stringValue);

                    // Update the document with the modified array
                    await docRef.UpdateAsync(fieldName, existingArray);
                    MessageBox.Show("Élément ajouté !");
                }
                else
                {
                    MessageBox.Show("Document does not exist.");
                }
            }
            catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
            {
                MessageBox.Show("L'opération a échoué : Vous n'avez pas la permission d'effectuer cette action", "Erreur d'autorisation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur: " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public async Task RemoveStringFromArrayInDocument(string documentId, string fieldName, string stringValue)
        {
            try
            {
                DocumentReference docRef = db.Collection("Qualifications").Document(documentId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    // Retrieve existing array from the document
                    List<object> existingArray = snapshot.GetValue<List<object>>(fieldName) ?? new List<object>();

                    // Check if the string exists in the array
                    if (existingArray.Contains(stringValue))
                    {
                        // Remove the string value from the array
                        existingArray.Remove(stringValue);

                        // Update the document with the modified array
                        await docRef.UpdateAsync(fieldName, existingArray);
                    }
                    else
                    {
                        MessageBox.Show("Item does not exist in the database.");
                    }
                }
                else
                {
                    MessageBox.Show("Document does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        public async Task GetEmployeesWithQualifications(ObservableCollection<Employee> employee1)
        { 
                CollectionReference collectionRef = db.Collection("Employees");
                QuerySnapshot snap = await collectionRef.GetSnapshotAsync();

            foreach(DocumentSnapshot docsnap in snap.Documents)
            {
              
                
              Employee employee = docsnap.ConvertTo<Employee>();
                if (docsnap.Exists)
                {
                    employee1.Add(employee);
                }

             }


        }


        public async Task RemoveSelectedEmployee(DataGrid grid, ObservableCollection<Employee> employees)
        {
            var selectedEmployees = grid.SelectedItems.Cast<Employee>().ToList();

            if (selectedEmployees.Count == 0)
            {
                MessageBox.Show("Aucun employé sélectionné");
                return;
            }

            // Show a confirmation message box with the employee details
            var employeeDetails = string.Join("\n", selectedEmployees.Select(e => $"{e.Name} (CIN: {e.CIN})"));
            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer les employés suivants ?\n\n{employeeDetails}",
                "Confirmer",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            // If the user confirms the deletion
            if (result == MessageBoxResult.Yes)
            {
                bool deletionSuccessful = false; // Flag to track if any deletions were successful

                foreach (var selectedEmployee in selectedEmployees)
                {
                    try
                    {
                        // Reference to the Employees collection
                        CollectionReference collectionRef = db.Collection("Employees");

                        // Create a query to find the document with the selected employee's CIN (or other unique identifier)
                        Query query = collectionRef.WhereEqualTo("CIN", selectedEmployee.CIN);
                        QuerySnapshot snap = await query.GetSnapshotAsync();

                        // Check if any documents match the query
                        if (snap.Documents.Count > 0)
                        {
                            // Assume CIN is unique, so only one document should match
                            DocumentSnapshot docToDelete = snap.Documents[0];

                            // Delete the document
                            await docToDelete.Reference.DeleteAsync();

                            // Remove the selected employee from the ObservableCollection
                            employees.Remove(selectedEmployee);

                            deletionSuccessful = true; // Set flag to true since deletion was successful
                        }
                    }
                    catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
                    {
                        MessageBox.Show("L'opération a échoué : Vous n'avez pas la permission d'effectuer cette action", "Erreur d'autorisation", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur: " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (deletionSuccessful)
                {
                    MessageBox.Show("Les employés sélectionnés ont été supprimés avec succès");

                    // Refresh the DataGrid
                    grid.Items.Refresh();
                }
            }
        }

        public async Task UpdateEmployeeByCINAsync(Employee updatedEmployee)
        {
            bool updateSuccessful = false; // Flag to track if the update was successful

            try
            {
                CollectionReference collectionRef = db.Collection("Employees");
                QuerySnapshot snapshot = await collectionRef.WhereEqualTo("CIN", updatedEmployee.CIN).GetSnapshotAsync();

                if (snapshot.Documents.Count > 0)
                {
                    DocumentSnapshot document = snapshot.Documents[0];
                    DocumentReference docRef = document.Reference;

                    Dictionary<string, object> updatedData = new Dictionary<string, object>
            {
                {"Name", updatedEmployee.Name},
                {"CIN", updatedEmployee.CIN},
                {"Project", updatedEmployee.Project},
                {"Famille", updatedEmployee.Famille},
                {"Zone", updatedEmployee.Zone},
                {"Qualifications", updatedEmployee.Qualifications.Select(q => new Dictionary<string, object>
                    {
                        {"Name", q.Name},
                        {"Level", q.Level},
                        {"Date", q.Date},
                        {"State", q.State}
                    }).ToList()}
            };

                    await docRef.UpdateAsync(updatedData);

                    updateSuccessful = true; // Set flag to true since update was successful
                }
                else
                {
                    throw new Exception("Employé non trouvé");
                }
            }
            catch (Grpc.Core.RpcException ex) when (ex.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
            {
                MessageBox.Show("L'opération a échoué : Vous n'avez pas la permission d'effectuer cette action", "Erreur d'autorisation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour de l'employé: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (updateSuccessful)
            {
                MessageBox.Show("Employé mis à jour avec succès");
            }
        }





    }

}







            