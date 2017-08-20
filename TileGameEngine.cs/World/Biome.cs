using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.WorldNS
{
    public enum BiomeTypes { Woods, Plains, Barrens, Taiga }

    public class Biome
    {
        public int ID { get; private set; }
        public BiomeTypes Type { get; private set; }


        public Texture2D texture;

        public Biome(World world)
        {
            this.ID = world.Ids.BiomeID();
        }

        public void Init(World world, BiomeTypes type)
        {
            this.Type = type;
            switch (type)
            {
                case BiomeTypes.Woods:
                    texture = world.TextureLoader.CreateSimpleTexture(Color.Brown);
                    break;
                case BiomeTypes.Plains:
                    texture = world.TextureLoader.CreateSimpleTexture(Color.LightGreen);
                    break;
                case BiomeTypes.Barrens:
                    texture = world.TextureLoader.CreateSimpleTexture(Color.Wheat);
                    return;
                case BiomeTypes.Taiga:
                    texture = world.TextureLoader.CreateSimpleTexture(Color.DarkGreen);
                    return;
            }
        }

        public Texture2D GetTexture()
        {
            return texture;
        }
    }
}
