using Game1.WorldNS;
using System;
using System.Diagnostics;

namespace Game1.Generation
{
    class ElevationGenerator
    {
        Grid grid;
        Random rnd;
        Tile[,] tiles;
        double minAccept;
        double maxNumber;

        public ElevationGenerator(Grid grid, int seed)
        {
            this.grid = grid;
            rnd = new Random();
            tiles = grid.Tiles;
        }

        public void Generate()
        {
            minAccept = Default.MaxElevation / Default.TriangularRandomCoef;
            maxNumber = Default.MaxElevation + minAccept;
            double average = 0;
            for (int x = 0; x < grid.MapWidth; x++)
            {
                for (int y = 0; y < grid.MapHeight; y++)
                {
                    double generated;
                    do
                        generated = TriangularRandom();
                    while (generated<minAccept);
                    tiles[x, y].Elevation = generated - minAccept;
                    average += tiles[x, y].Elevation;
                }
            }
            average = (average / grid.MapHeight / grid.MapWidth);
        }

        public void Smooth(int smoothRadius)
        {
            for (int x = smoothRadius; x < grid.MapWidth- smoothRadius; x++)
            {
                for (int y = smoothRadius; y < grid.MapHeight-smoothRadius; y++)
                {
                    tiles[x, y].Elevation = AverageElevation(smoothRadius, x, y);
                }
            }
        }

        private double AverageElevation(int smoothRadius, int x, int y)
        {
            double toReturn = tiles[x, y].Elevation;
            for (x = x - smoothRadius; x < 1 + 2 * smoothRadius; x++)
            {
                for (y = y - smoothRadius; y < 1 + 2 * smoothRadius; y++)
                {
                    toReturn += tiles[x, y].Elevation;
                }
            }
            return toReturn / ((smoothRadius * 2 + 1) * (smoothRadius * 2 + 1));
        }

        private double TriangularRandom()
        {
            double n1 = rnd.NextDouble();
            double n2 = rnd.NextDouble();
            n1 = (n1 + n2) / 2;
            return (n1 * maxNumber);
        }
    }
}
