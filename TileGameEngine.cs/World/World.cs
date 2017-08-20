using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities_Mono;

namespace Game1.WorldNS
{
    public class World
    {
        public UniqueIdentifiers Ids { get; private set; }
        public TextureLoader TextureLoader { get; private set; }
        public Grid Grid { get; private set; }
        
        public World(TextureLoader textureLoader)
        {
            this.TextureLoader = textureLoader;
            this.Ids = new UniqueIdentifiers();
            this.Grid = new Grid(this, 200, 200);
        }


    }
}
