using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace notifyApi.Data {
    public class Members {
        [Key]
        public int counter { get; set; }
        public string LineToken { get; set; }
        public string name { get; set; }

        public string account { get; set; }
        public string password { get; set; }
        public int deleted { get; set; }

    }
    public interface IMemberService {
        Task addLineToken (int counter, string Token);
        Task<Members> accountLogin (string account, string password);

    }
    //         MemberService是物件名稱 繼承 IMemberService
    public class MemberService : IMemberService {
        private NotifyContext _dbconn;
        public MemberService (NotifyContext dbconn) {
            this._dbconn = dbconn;
        }
        public async Task addLineToken (int counter, string Token) {
            NotifyContext db = this._dbconn;
            var _member = db.Members.Find (counter);
            db.Members.Attach (_member);
            _member.LineToken = Token;
            await db.SaveChangesAsync ();

        }
        public async Task<Members> accountLogin (string account, string password) {
            NotifyContext db = this._dbconn;
            var q = await (from p in db.Members where p.deleted == 0 && account == p.account select p).ToAsyncEnumerable ().FirstOrDefault ();
            return q;
            // if (q == null) {
            //     // 沒有資料列
            //     Members _member = new Members();
            //     _member.counter = -1;
            //     return _member;
            // } else if (q.password == password) {
            //     // 登入成功
            //     return q;

            // } else {
            //     // 密碼錯誤
            //     q.counter = 0;
            //     return q;

            // }

        }

    }

}