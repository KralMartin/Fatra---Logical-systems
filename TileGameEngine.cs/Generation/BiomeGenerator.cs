using Game1.WorldNS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Generation
{
    class BiomeGenerator
    {
        Grid grid;
        Tile[,] tiles;
        Random rnd;
        List<Qwe> biomes;

        public BiomeGenerator(Grid grid, int seed)
        {
            this.grid = grid;
            rnd = new Random();
            tiles = grid.Tiles;
        }

        public void Generate()
        {
            int biomesTypesCount= Enum.GetValues(typeof(BiomeTypes)).Length;
            biomes = new List<Qwe>();
            for (int i = 0; i < grid.MapHeight * grid.MapWidth / 2000; i++)
            {
                Qwe q = new Qwe();
                q.Biome = CreateRandomBiome(biomesTypesCount);
                q.point = new Vector2(rnd.Next(grid.MapWidth), rnd.Next(grid.MapHeight));
                biomes.Add(q);
                grid.Biomes.Add(q.Biome);
                grid.Tiles[(int)q.point.X, (int)q.point.Y].Biome = q.Biome;
            }
            for (int x = 0; x < grid.MapWidth; x++)
            {
                for (int y = 0; y < grid.MapHeight; y++)
                {
                    if (tiles[x, y].Biome == null)
                    {
                        tiles[x, y].Biome = FindClosestBiomePoint(x, y);
                    }
                }
            }
        }

        private Biome FindClosestBiomePoint(int x, int y)
        {
            Biome toReturn = null;
            float minDistance = float.MaxValue;
            float distance;
            Vector2 v1 = new Vector2(x, y);
            foreach (Qwe current in biomes)
            {
                distance = Vector2.Distance(v1, current.point);
                if (distance < minDistance)
                {
                    toReturn = current.Biome;
                    minDistance = distance;
                }
            }
            return toReturn;
        }

        private Biome CreateRandomBiome(int biomesTypesCount)
        {
            Biome toReturn = new Biome(this.grid.World);
            toReturn.Init(grid.World, (BiomeTypes)rnd.Next(biomesTypesCount));
            return toReturn;
        }
    }
}
