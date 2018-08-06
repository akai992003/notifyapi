using Microsoft.EntityFrameworkCore;

namespace notifyApi.Data {
    public class NotifyContext : DbContext {
        public NotifyContext (DbContextOptions<NotifyContext> options) : base (options) { }
        protected override void OnModelCreating (ModelBuilder ModelBuilder) {

        }
        public virtual DbSet<Members> Members { get; set; }
        public virtual DbSet<hnNotify> hnNotify { get; set; }
        public virtual DbSet<hnNotifyItem> hnNotifyItem { get; set; }
    }

}