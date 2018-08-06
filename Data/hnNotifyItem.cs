using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace notifyApi.Data {
    public class hnNotifyItem {
        [Key]
        public int counter { get; set; }
        public string name { get; set; }
        public int deleted { get; set; }
    }

    public interface IhnNotifyItemService {
        Task<List<hnNotifyItem>> ShowItem ();
    }
    public class hnNotifyItemService : IhnNotifyItemService {
        private NotifyContext _dbconn;
        public hnNotifyItemService (NotifyContext dbconn) {
            this._dbconn = dbconn;
        }
        public async Task<List<hnNotifyItem>> ShowItem () {
            NotifyContext db = this._dbconn;
            var q = await (from p in db.hnNotifyItem where p.deleted == 0 select p).ToAsyncEnumerable ().ToList ();
            return q;
        }

       

    }
    
}