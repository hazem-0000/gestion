using gestion;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gestion
 {
    [FirestoreData]
    public class Employee
    {
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public int CIN { get; set; }
        [FirestoreProperty]
        public string Project { get; set; }
        [FirestoreProperty]
        public string Zone { get; set; }
        [FirestoreProperty]
        public string Famille {get;set ;}
        [FirestoreProperty]
        public ObservableCollection<Qualification> Qualifications { get; set; }
       public Employee() { }
        public Employee(string name, int cin, string project, string zone,string famille, ObservableCollection <Qualification> qualifications)
        {
            Name = name;
            CIN = cin;
            Project = project;
            Zone = zone;
            Famille = famille;
            Qualifications = qualifications;
        }
        
    }
   


}