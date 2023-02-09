using Palantir_Rebirth.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Data.Cache
{
    internal class DatabaseCache<TItem>
    {
        private Func<PalantirDatabaseContext, TItem> bindingRead;
        private Action<PalantirDatabaseContext, TItem> bindingWrite;

        private long lastUpdate;
        private readonly int expiry;
        private TItem? item;

        public DatabaseCache(Func<PalantirDatabaseContext, TItem> bindingRead, Action<PalantirDatabaseContext, TItem> bindingWrite, int expiry = 60 * 1000)
        {
            lastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.expiry = expiry;
            this.bindingRead = bindingRead;
            this.bindingWrite = bindingWrite;
            lastUpdate = 0;
        }
        public Action<TItem> OnRead { private get; set; } = delegate { };
        public Action<TItem> OnUpdate { private get; set; } = delegate { };

        public TItem Item 
        { 
            get
            {
                long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (now - lastUpdate > expiry || item is null)
                {
                    var db = Program.PalantirDb.Open();
                    item = bindingRead.Invoke(db);
                    Program.PalantirDb.Close(db);

                    lastUpdate = now;
                    OnRead(item);
                }

                return item;
            } 

            set
            {
                var db = Program.PalantirDb.Open();
                bindingWrite.Invoke(db, value);
                Console.WriteLine("no changes in debug mode");
                Program.PalantirDb.Close(db, false);
                MarkDirty();
                OnUpdate(value);
            }
        } 

        public void MarkDirty()
        {
            lastUpdate = 0;
        }
    }
}
