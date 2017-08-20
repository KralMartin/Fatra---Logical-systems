using Game1.Generation;
using System.Collections.Generic;

namespace Game1.WorldNS
{
    public class Grid
    {
        public World World { get; private set; }
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }        
        public Tile[,] Tiles { get; private set; }
        public WorldGenerator WorldGenerator { get; private set; }
        public List<Biome> Biomes { get; private set; }

        public Grid(World world, int mapWidth, int mapHeight)
        {
            this.Tiles = new Tile[mapWidth, mapHeight];
            this.World = world;
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.Biomes = new List<Biome>();
            this.WorldGenerator = new WorldGenerator(this);
        }
    }
}
