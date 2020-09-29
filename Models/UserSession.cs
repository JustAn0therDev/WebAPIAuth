using System;

namespace WebAPIAuth.Models {
    public class UserSession {
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}