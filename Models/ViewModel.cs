using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wedding_Planner.Models {
    public class ViewModel {
        public User NewUser { get; set; }
        public List<User> AllUsers { get; set; }

        public LoginUser LoginUser { get; set; }

        public Wedding NewWedding { get; set; }
        public List<Wedding> AllWeddings { get; set; }

        public Association newAssoc { get; set; }
        public List<Association> AllAssociations { get; set; }
    }
}