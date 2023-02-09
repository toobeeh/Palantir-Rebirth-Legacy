using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Data.Cache
{
    internal class DependencyCache<TItem, TDependency>
    {
        private Func<TDependency> dependencyRead;
        private Func<TDependency, TItem> dependencySync;
        private TDependency? lastDependencyValue;
        private TItem? cache;

        public DependencyCache(Func<TDependency> dependencyRead, Func<TDependency, TItem> dependencySync)
        {
            this.dependencyRead = dependencyRead;
            this.dependencySync = dependencySync;
        }

        public TItem Item {
            get
            {
                var newDependencyValue = dependencyRead.Invoke();
                if (cache is null || lastDependencyValue is null || lastDependencyValue.Equals(newDependencyValue))
                {
                    var newCache = dependencySync.Invoke(newDependencyValue);
                    cache = newCache;
                    return cache;
                }
                else return cache;
            }
        }
    }
}
