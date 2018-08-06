using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace notifyApi.Data {
    public class hnNotify {
        [Key]
        public Guid counter { get; set; }
        public int hnNotifyItem_counter { get; set; }
        public int member_counter { get; set; }
        public int Line { get; set; }
        public int Mail { get; set; }
        public int Message { get; set; }
        public int deleted { get; set; }

    }
    public interface IhnNotifyService {
        Task<List<hnNotify>> ShowMember (int member_counter);
        Task<List<DTOhnNotifyItem>> ShowMemberItem (int member_counter);
        void UpdateNotifyMember (DTOhnNotifyItemUser dto);
        Task<List<USERSERVICEORDER>> ShowMemberOrderItem (int member_counter);
        Task delUserServiceOrder (int member_counter, string actionName, string serviceName);
    }
    public class hnNotifyService : IhnNotifyService {
        private NotifyContext _dbconn;
        public hnNotifyService (NotifyContext dbconn) {
            this._dbconn = dbconn;
        }
        public async Task<List<hnNotify>> ShowMember (int member_counter) {
            NotifyContext db = this._dbconn;
            var q = await (from p in db.hnNotify where p.deleted == 0 && p.member_counter == member_counter select p).ToAsyncEnumerable ().ToList ();
            return q;
        }
        public async Task updateMember (hnNotify _hnNotify) {
            NotifyContext db = this._dbconn;
            var _member = db.hnNotify.Find (_hnNotify.counter);
            db.hnNotify.Attach (_member);
            // _member = _hnNotify;
            _member.Line = _hnNotify.Line;
            _member.Mail = _hnNotify.Mail;
            _member.Message = _hnNotify.Message;
            await db.SaveChangesAsync ();
        }
        public async Task<List<USERSERVICEORDER>> ShowMemberOrderItem (int member_counter) {
            NotifyContext db = this._dbconn;
            var q = await (from p in db.hnNotify join p2 in db.hnNotifyItem on p.hnNotifyItem_counter equals p2.counter where p.deleted == 0 && p.member_counter == member_counter select new {
                Line = p.Line,
                    Mail = p.Mail,
                    Message = p.Message,
                    serviceName = p2.name
            }).ToAsyncEnumerable ().ToList ();

            List<USERSERVICEORDER> l = new List<USERSERVICEORDER> ();
            foreach (var p in q) {

                if (p.Line == 1) {
                    USERSERVICEORDER d = new USERSERVICEORDER ();
                    d.actionName = "Line";
                    d.serviceName = p.serviceName;
                    l.Add (d);
                }

                if (p.Mail == 1) {
                    USERSERVICEORDER d = new USERSERVICEORDER ();
                    d.actionName = "Mail";
                    d.serviceName = p.serviceName;
                    l.Add (d);
                }

                if (p.Message == 1) {
                    USERSERVICEORDER d = new USERSERVICEORDER ();
                    d.actionName = "Message";
                    d.serviceName = p.serviceName;
                    l.Add (d);
                }

            }

            l = l.OrderBy (c => c.actionName).ToList ();
            return l;
        }

        public async Task<List<DTOhnNotifyItem>> ShowMemberItem (int member_counter) {
            NotifyContext db = this._dbconn;
            var q = await (from p in db.hnNotify where p.deleted == 0 && p.member_counter == member_counter select p).ToAsyncEnumerable ().ToList ();
            var tmpList = new List<DTOhnNotifyItem> ();

            foreach (var p in db.hnNotifyItem.Where (c => c.deleted == 0)) {
                var DTO = new DTOhnNotifyItem ();
                DTO.counter = p.counter;
                DTO.name = p.name;
                var p2 = q.Where (c => c.hnNotifyItem_counter == p.counter).FirstOrDefault ();
                if (p2 != null) {
                    if (p2.Line == 1) {
                        DTO.Line = true;
                    } else {
                        DTO.Line = false;
                    }

                    if (p2.Mail == 1) {
                        DTO.Mail = true;
                    } else {
                        DTO.Mail = false;
                    }

                    if (p2.Message == 1) {
                        DTO.Message = true;
                    } else {
                        DTO.Message = false;
                    }

                } else {
                    DTO.Line = false;
                    DTO.Mail = false;
                    DTO.Message = false;
                }
                tmpList.Add (DTO);

            }
            return tmpList;
        }

        public void UpdateNotifyMember (DTOhnNotifyItemUser dto) {
            NotifyContext db = this._dbconn;
            var q = (from p in db.hnNotify where p.member_counter == dto.member_counter select p)
                .AsEnumerable ().ToList ();

            if (q.FirstOrDefault () == null) {

                foreach (var p in dto.DTOhnNotifyItems) {
                    var _hnNotify = new hnNotify ();
                    _hnNotify.member_counter = dto.member_counter;
                    _hnNotify.counter = Guid.NewGuid ();
                    _hnNotify.hnNotifyItem_counter = p.counter;
                    if (p.Line == true) {
                        _hnNotify.Line = 1;
                    } else {
                        _hnNotify.Line = 0;
                    }

                    if (p.Mail == true) {
                        _hnNotify.Mail = 1;
                    } else {
                        _hnNotify.Mail = 0;
                    }

                    if (p.Message == true) {
                        _hnNotify.Message = 1;
                    } else {
                        _hnNotify.Message = 0;
                    }

                    _hnNotify.deleted = 0;
                    db.hnNotify.Add (_hnNotify);
                    db.SaveChanges ();
                }

            } else {
                foreach (var p in q) {
                    var _hnNotify = db.hnNotify.Find (p.counter);
                    db.hnNotify.Attach (_hnNotify);
                    _hnNotify.deleted = 0;
                    foreach (var p1 in dto.DTOhnNotifyItems) {

                        if (p.hnNotifyItem_counter == p1.counter) {
                            if (p1.Line == true) {
                                _hnNotify.Line = 1;
                            } else {
                                _hnNotify.Line = 0;
                            }

                            if (p1.Mail == true) {
                                _hnNotify.Mail = 1;
                            } else {
                                _hnNotify.Mail = 0;
                            }

                            if (p1.Message == true) {
                                _hnNotify.Message = 1;
                            } else {
                                _hnNotify.Message = 0;
                            }

                        }
                    }

                }
                db.SaveChanges ();
            }

        }
        public async Task delUserServiceOrder (int member_counter, string actionName, string serviceName) {
            NotifyContext db = this._dbconn;

            var q1 = (from p in db.hnNotifyItem where p.name == serviceName select p).AsEnumerable ().FirstOrDefault ();
            var q = (from p in db.hnNotify where p.member_counter == member_counter && p.hnNotifyItem_counter == q1.counter select p)
                .AsEnumerable ().FirstOrDefault ();
            var a = db.hnNotify.Find (q.counter);
            db.hnNotify.Attach (a);
            if (actionName == "Line") {
                a.Line = 0;
            }
            if (actionName == "Mail") {
                a.Mail = 0;
            }
            if (actionName == "Message") {
                a.Message = 0;
            }

            await db.SaveChangesAsync ();

        }

    }

    public class DTOhnNotifyItem {
        public int counter { get; set; }
        public string name { get; set; }
        public bool Line { get; set; }
        public bool Mail { get; set; }
        public bool Message { get; set; }

    }

    public class DTOhnNotifyItemUser {
        public int member_counter { get; set; }
        public List<DTOhnNotifyItem> DTOhnNotifyItems { get; set; }
    }

    //使用者服務訂閱檔
    public class USERSERVICEORDER {
        //動作名稱
        public string actionName { get; set; }

        //服務名稱
        public string serviceName { get; set; }

    }

}