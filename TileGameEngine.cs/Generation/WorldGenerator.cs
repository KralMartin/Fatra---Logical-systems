using Game1.WorldNS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Game1.Generation
{
    public class WorldGenerator
    {
        ElevationGenerator gElevation;
        BiomeGenerator gBiome;

        public WorldGenerator(Grid grid)
        {
            Random rnd = new Random();

            //Create grid
            Texture2D blank = grid.World.TextureLoader.CreateSimpleTexture(Color.Black);
            for (int x = 0; x < grid.MapWidth; x++)
            {
                for (int y = 0; y < grid.MapHeight; y++)
                {
                    grid.Tiles[x, y] = new Tile(x, y);
                    grid.Tiles[x, y].GroundTexture = blank;
                }
            }

            gElevation = new ElevationGenerator(grid, rnd.Next());
            //gElevation.Generate();
            //for (int i = 0; i < 1; i++)
            //    gElevation.Smooth(Default.SmoothingRadius);

            gBiome = new BiomeGenerator(grid, rnd.Next());
            gBiome.Generate();
        }
    }
}
