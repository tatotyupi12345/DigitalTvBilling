using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Utils
{
    public class ReturnedCardDelete
    {
        public ReturnedCardDelete(DataContext _db, int id)
        {
            var card_id = _db.Database.SqlQuery<int>("SELECT c.card_id FROM dbo.ReturnedCards c where id=" + id).FirstOrDefault();
            DateTime date = _db.Database.SqlQuery<DateTime>("SELECT c.tdate FROM dbo.ReturnedCards c where id=" + id).FirstOrDefault();
            var dateForm = date.ToString("yyyy-MM-dd HH:mm:ss");
            _db.Database.ExecuteSqlCommand("DELETE FROM doc.CardCharges where card_id=" + card_id + " and convert(nvarchar(MAX), tdate , 20)='" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            _db.Database.ExecuteSqlCommand("DELETE FROM doc.Payments where card_id=" + card_id + " and convert(nvarchar(MAX), tdate , 20)='" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            _db.Database.ExecuteSqlCommand("DELETE FROM dbo.ReturnedCardAttachments where ReturnedCardsID=" + id + "");
            _db.Database.ExecuteSqlCommand("DELETE FROM dbo.ReturnedCards where id=" + id);
            _db.Database.ExecuteSqlCommand("UPDATE book.Cards SET  [status] = 0 WHERE id =" + card_id);
        }
    }
}