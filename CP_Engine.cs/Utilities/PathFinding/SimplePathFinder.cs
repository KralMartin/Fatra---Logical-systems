using CP_Engine.MapItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    class SimplePathFinder
    {
        Queue<MemorizedPathItem> que = new Queue<MemorizedPathItem>();
        List<MemorizedPathItem> visitedItems = new List<MemorizedPathItem>();
        int width;
        Window window;

        internal static void SelectPoints(Window window, Point coords)
        {
            SimplePathFinder pf = new SimplePathFinder();
            pf.width = -1;
            pf.window = window;
            pf.que = new Queue<MemorizedPathItem>();
            pf.visitedItems = new List<MemorizedPathItem>();
            pf.SelectLineFromPoint(coords);
        }
        
        private void SelectLineFromPoint(Point coords)
        {            
            TileData data = window.Scheme.Get_TileData(coords);
            TileInfoItem info = TilesInfo.GetItem(data.Type);
            que = new Queue<MemorizedPathItem>();
            visitedItems = new List<MemorizedPathItem>();
            //Get vire-width based on where user clicked.
            width = -1;
            if (info.TileType == TileTypes.Vire)
            {
                if (info.UsesHorizontal())
                {
                    width = data.HorzWidth;
                    que.Enqueue(new MemorizedPathItem(coords, true));
                }
                else
                {
                    width = data.VertWidth;
                    que.Enqueue(new MemorizedPathItem(coords, false));
                }
            }
            else if (info.Type == 1 || info.Type == 3)
            {
                width = data.VertWidth;
                que.Enqueue(new MemorizedPathItem(coords, false));
            }
            else if (info.Type == 2 || info.Type == 4)
            {
                width = data.HorzWidth;
                que.Enqueue(new MemorizedPathItem(coords, true));
            }
            else if (TilesInfo.IsType12(data.Type))
            {
                width = data.HorzWidth;
                que.Enqueue(new MemorizedPathItem(coords, false));
            }
            if (width < 0)
            {
                window.Selection.Items.Add(coords);
                return;
            }
            //Select only those tiles, that share same width.
            while (que.Count > 0)
            {
                MemorizedPathItem current = que.Dequeue();
                if (MyContains(current) == false)
                {
                    visitedItems.Add(current);
                    data = window.Scheme.Get_TileData(current.Coords);
                    if (TilesInfo.IsBugType(data.Type))
                        continue;

                    info = TilesInfo.GetItem(data.Type);

                    if (info.TileType != TileTypes.Vire && info.TileType != TileTypes.ComposedVire)
                        continue;
                    if (current.Horz_Vert)
                    {
                        if (data.HorzWidth != width)
                            continue;
                    }
                    else
                    {
                        if (data.VertWidth != width)
                            continue;
                    }
                    window.Selection.Items.Add(current.Coords);

                    if (current.Horz_Vert)
                    {
                        GoTo(current.Coords, Sides.Left, data);
                        GoTo(current.Coords, Sides.Right, data);
                    }
                    else
                    {
                        GoTo(current.Coords, Sides.Top, data);
                        GoTo(current.Coords, Sides.Bot, data);
                    }
                    if (TilesInfo.IsType7(data.Type) == false)
                    {
                        //Is not X shaped without connection.
                        current.Horz_Vert = !current.Horz_Vert;
                        if (MyContains(current) == false)
                        {
                            visitedItems.Add(current);
                            if (current.Horz_Vert)
                            {
                                GoTo(current.Coords, Sides.Left, data);
                                GoTo(current.Coords, Sides.Right, data);
                            }
                            else
                            {
                                GoTo(current.Coords, Sides.Top, data);
                                GoTo(current.Coords, Sides.Bot, data);
                            }
                        }
                    }

                }
            }
        }

        private bool MyContains(MemorizedPathItem item)
        {
            foreach (MemorizedPathItem current in visitedItems)
            {
                if (current.Coords == item.Coords && current.Horz_Vert == item.Horz_Vert)
                    return true;
            }
            return false;
        }

        private void GoTo(Point coords, int side, TileData data)
        {
            TileInfoItem info = TilesInfo.GetItem(data.Type);
            if (info.TileSide[side].IsUsed && data.GetWidth(side) == width)
            {
                Point newCoords = Sides.Move(coords, side);
                if (window.Scheme.ValidateCoords(newCoords))
                {
                    MemorizedPathItem toAdd = new MemorizedPathItem(newCoords, Sides.IsHorizontal(side));
                    que.Enqueue(toAdd);
                }
            }
        }

    }
}
