using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class UniqueIdentifiers
    {
        int biomeID;

        public UniqueIdentifiers()
        {
            biomeID = 0;
        }

        public int BiomeID()
        {
            biomeID++;
            return biomeID;
        }
    }
}
