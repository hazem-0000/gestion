using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gestion
{
    [FirestoreData]
    public class Qualification
    {
        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Level { get; set; }
        [FirestoreProperty]
        public string Date {  get; set; }
        [FirestoreProperty]
        public bool State {  get; set; }

        // Use DateTime but only utilize the date component
        
        

        public Qualification() { }

        // Constructor that takes a DateTime, ensuring time is set to midnight
        public Qualification(string name, string level,string date)
        {
            Name = name;
            Level = level;
            Date = date;
            
        }
    }
}
