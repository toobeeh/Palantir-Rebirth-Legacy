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

        public async Task<List<TResult>> QueryAsync<TResult>(Func<PalantirDatabaseContext, IEnumerable<TResult>> query, bool save = false)
        {
            var context = Open();
            try
            {
                var result = query.Invoke(context).ToList();
                await Close(context, save);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("Failed to execute async db query", e);
                await Close(context, save);
                return new List<TResult>();
            }
        }

        public List<TResult> Query<TResult>(Func<PalantirDatabaseContext, IEnumerable<TResult>> query, bool save = false)
        {
            var context = Open();
            try
            {
                var result = query.Invoke(context).ToList();
                Close(context, save).GetAwaiter().GetResult();
                return result;
            }
            catch(Exception e)
            {
                Logger.Error("Failed to execute sync db query", e);
                Close(context, save).GetAwaiter().GetResult();
                return new List<TResult>();
            }
        }
    }
}
