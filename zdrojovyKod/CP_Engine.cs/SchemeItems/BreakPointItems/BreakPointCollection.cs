using CP_Engine.MapItems;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    class BreakPointCollection
    {
        int currentID;
        Scheme scheme;
        List<BreakPoint> items;
        WorkPlace workplace;

        internal BreakPointCollection(WorkPlace workplace, Scheme scheme)
        {
            this.workplace = workplace;
            this.scheme = scheme;
            items = new List<BreakPoint>();
        }

        internal void CheckBreakPoints(List<BreakPointResult> results)
        {
            List<PhysScheme> pSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(scheme);
            foreach (BreakPoint bp in items)
            {
                bp.Check(scheme, pSchemes, results);
            }
        }

        internal void Add(Point coords)
        {
            BreakPoint bp = new BreakPoint(GetID(), coords);
            items.Add(bp);

            Tile tile = scheme.Get_Tile(coords);
            tile.BreakPoint = bp;
            List<PhysScheme> pSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(scheme);
            foreach (PhysScheme pScheme in pSchemes)
                pScheme.BreakPoints.Add(bp.ID, tile.GetValues(pScheme));
        }

        internal void Remove(Point coords)
        {
            //Do not forget, you have to remove it from Tile on provided coords too.
            BreakPoint toRemove = null;
            foreach (BreakPoint bp in items)
            {
                if (bp.Coords == coords)
                {
                    toRemove = bp;
                    break;
                }
            }
            items.Remove(toRemove);
            RemovePhysRepre(toRemove);
        }

        private void RemovePhysRepre(BreakPoint toRemove)
        {
            List<PhysScheme> pSchemes = workplace.Project.SchemeStructure.Get_PhysSchemes(scheme);
            foreach (PhysScheme pScheme in pSchemes)
                pScheme.BreakPoints.Remove(toRemove.ID);
        }

        private int GetID()
        {
            return currentID++;
        }

        internal void Import(List<BreakPoint> breakPoints)
        {
            foreach (BreakPoint bp in items)
            {
                scheme.Get_Tile(bp.Coords).BreakPoint = null;
                RemovePhysRepre(bp);
            }
            items.Clear();

            List<BreakPoint> toReturn = new List<BreakPoint>();
            foreach (BreakPoint bp in breakPoints)
                Add(bp.Coords);
        }

        internal List<BreakPoint> Export()
        {
            List<BreakPoint> toReturn = new List<BreakPoint>();
            foreach (BreakPoint bp in items)
                toReturn.Add(new BreakPoint(bp.ID, bp.Coords));
            return toReturn;
        }

        internal void CreatePhysRepresentation(WorkPlace workplace, PhysScheme pScheme)
        {
            foreach (BreakPoint bp in items)
            {
                Tile tile = scheme.Get_Tile(bp.Coords);
                pScheme.BreakPoints.Add(bp.ID, tile.GetValues(pScheme));
            }
        }
    }
}
