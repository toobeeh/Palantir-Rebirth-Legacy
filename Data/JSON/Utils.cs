using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Data.JSON
{
    internal static class Utils
    {
        public static T FromString<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T FromFile<T>(string path)
        {
            return FromString<T>(File.ReadAllText(path));
        }
    }
}
