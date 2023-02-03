using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Data.SQLite
{
    internal class PalantirDatabase
    {
        private string path;

        public PalantirDatabase(string path) { 
            this.path = path;
        }

        public PalantirDatabaseContext Open()
        {
            return new PalantirDatabaseContext(this.path);
        }

        public async Task Close(PalantirDatabaseContext context, bool save = false)
        {
            if (save) await context.SaveChangesAsync();
            await context.DisposeAsync();
        }

        public async Task<TResult> Query<TResult>(Func<PalantirDatabaseContext, TResult> query, bool save = false)
        {
            var context = Open();
            var result = query.Invoke(context);
            await Close(context, save);
            return result;
        }
    }
}
