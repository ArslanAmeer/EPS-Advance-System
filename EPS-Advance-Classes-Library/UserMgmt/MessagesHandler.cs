using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EPS_Advance_Classes_Library.UserMgmt
{
    public class MessagesHandler
    {
        private readonly DBContextClass _db = new DBContextClass();

        public List<Converstion> Conversations(int senderId, int receiverId)
        {
            using (_db)
            {
                return (from c in _db.Converstions
                    .Where((x => (x.User1 == senderId && x.User2 == receiverId) || (x.User1 == receiverId && x.User2 == senderId)))
                        select c).ToList();
            }
        }

        public List<Converstion> UserConversations(int currentUserId)
        {
            using (_db)
            {
                return (from c in _db.Converstions
                        .Where(x => x.User1 == currentUserId || x.User2 == currentUserId)
                        select c).ToList();
            }
        }

        public void AddNewConverstion(Converstion convo)
        {
            using (_db)
            {
                _db.Converstions.Add(convo);
                _db.SaveChanges();
            }
        }

        public List<Messages> GetAllSendMessages(int send, int recv)
        {
            using (_db)
            {
                List<Messages> msgs =
                    (from m in _db.Messages.Where(u => u.SenderId == send && u.ReceiverId == recv) select m).ToList();
                return msgs;
            }
        }

        public List<Messages> GetAllReceivedMessages(int send, int recv)
        {
            using (_db)
            {
                List<Messages> msgs =
                    (from m in _db.Messages.Where(u => u.SenderId == recv && u.ReceiverId == send) select m).ToList();
                return msgs;
            }
        }

        public void AddMessage(Messages msg)
        {
            using (_db)
            {
                _db.Entry(msg.Converstion).State = EntityState.Unchanged;
                _db.Messages.Add(msg);
                _db.SaveChanges();
            }
        }

    }
}
