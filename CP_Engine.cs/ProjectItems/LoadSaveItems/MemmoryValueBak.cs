using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP_Engine.SchemeItems;
using CP_Engine.MapItems;
using CP_Engine.BugItems;

namespace CP_Engine
{
    class MemmoryValueBak
    {
        bool value;
        List<Point> position;

        internal MemmoryValueBak(string text)
        {
            position = new List<Point>();
            string[] values = text.Split(',');
            value = Convert.ToBoolean(values[0]);
            for (int i = 1; i < values.Length; i += 2)
                position.Add(new Point(Convert.ToInt32(values[i]), Convert.ToInt32(values[i + 1])));
        }

        internal static void ChangeValue(Simulation sim, SpecialPhysScheme specialPscheme, bool newValue)
        {
            if (newValue != specialPscheme.Values[0])
            {
                //Change value.
                specialPscheme.Values[0] = newValue;
                //Get outer SSource.
                SchemeSource source = specialPscheme.PlacedBug.SchemeSources[MemmoryBug.OutputIdentifier];
                //Get path leading from outer SSource.
                PhysPath pPath = specialPscheme.ParentPScheme.Paths[source.IsInputIn.ID];
                //Create event. 
                sim.Events.AddPath(pPath);
            }
        }
        
        /// <summary>
        /// Insert stored memmory value to scheme.
        /// </summary>
        /// <param name="pScheme">Top scheme of project.</param>
        internal void InsertValue(PhysScheme pScheme, WorkPlace workplace)
        {
            int index = 0;
            foreach (Point coords in position)
            {
                TileData data = pScheme.PlacedBug.Bug.Scheme.Get_TileData(coords);
                PlacedBug pBug = pScheme.PlacedBug.Bug.Scheme.PlacedBugs.Get(data.HorzWidth);
                index++;
                if (index == position.Count)
                {
                    SpecialPhysScheme specialPscheme = pScheme.SpecialChildren[pBug.ID];
                    ChangeValue(workplace.Simulation, specialPscheme, this.value);
                    return;
                }
                pScheme = pScheme.Children[pBug.ID];
            }
        }

        internal static string GetPath(SpecialPhysScheme specialPscheme)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(specialPscheme.Values[0].ToString());
            Stack<PlacedBug> stack = GetParentPlacedBugs(specialPscheme.ParentPScheme);
            while (stack.Count > 0)
            {
                PlacedBug pBug = stack.Pop();
                sb.Append("," + pBug.Coords.X + "," + pBug.Coords.Y);
            }
            sb.Append("," + specialPscheme.PlacedBug.Coords.X + "," + specialPscheme.PlacedBug.Coords.Y);
            return sb.ToString();
        }

        private static Stack<PlacedBug> GetParentPlacedBugs(PhysScheme pScheme)
        {
            Stack<PlacedBug> stack = new Stack<PlacedBug>();
            while (pScheme != null)
            {
                stack.Push(pScheme.PlacedBug);
                pScheme = pScheme.ParentPScheme;
            }
            stack.Pop();
            return stack;
        }
    }
}
