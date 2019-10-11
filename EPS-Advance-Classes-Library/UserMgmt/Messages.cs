using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS_Advance_Classes_Library.UserMgmt
{
    public class Messages
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public DateTime TimeOfMsg { get; set; }

        public Converstion Converstion { get; set; }

        public bool ReadStatus { get; set; }
    }
}
