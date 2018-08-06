using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using notifyApi.Data;
using notifyApi.Model;
using Newtonsoft.Json.Linq;

namespace notifyApi.Controllers {
    [Route ("api/[controller]")]
    [EnableCors ("CorsDomain")]
    [ApiController]
    public class ServiceItemController : Controller {

        // 宣告一個 IMemberService 物件給目前的controller用，取名叫 MemberService (但是他現在是空的無法使用)
        private IMemberService MemberService;

        // 宣告一個 IhnNotifyService 物件給目前的controller用，取名叫 hnNotifyService (但是他現在是空的無法使用)
        private IhnNotifyService hnNotifyService;

        // 程式啟動的時候注入實體的 IhnNotifyService 物件給controller用，取名叫 _hnNotifyService ; 注入實體的 IMemberService 物件給controller用，取名叫做 _MemberService
        public ServiceItemController (IMemberService _MemberService, IhnNotifyService _hnNotifyService) {

            // 指定本controller的 this.MemberService = 外部注入的 _MemberService 物件，接下來這整個controller都可以使用這個 this.MemberService 的這個物件(他是 IMemberService 的服務物件)
            this.MemberService = _MemberService;

            // 指定本controller的 hnNotifyService = 外部注入的 _hnNotifyService 物件
            this.hnNotifyService = _hnNotifyService;
            // 接下來這整個controller都可以使用 this.hnNotifyService 這個物件(他是IhnNotifyService服務物件)
        }

        [Route ("serviceList")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> serviceList ([FromBody] intViewModel dto) {
            // string msg = "";

            var q = await this.hnNotifyService.ShowMemberOrderItem (dto.Counter);
            // if (q == null) {
            //     // 沒有資料列
            //     msg = "沒有此帳號資料";

            //     return BadRequest (new { msg=msg });
            // } else if (q.password == dto.Pwd) {
            //     // 登入成功
            //     msg = "登入成功";

            // } else {
            //     // 密碼錯誤
            //     msg = "密碼錯誤";
            //     return BadRequest (new { msg = msg });
            // }
            var j = new JArray ();
            foreach (var p in q) {
                var j2 = new JObject {
                    {
                    "actionName",
                    p.actionName
                    }, {
                    "serviceName",
                    p.serviceName
                    }, {
                    "Line",
                    p.actionName // 有沒有訂閱LINE
                    }, {
                    "Mail",
                    p.serviceName // 有沒有訂閱MAIL
                    }
                };
                j.Add (j2);

            }

            return Ok (new {
                Data = j
            });

        }
    }
}