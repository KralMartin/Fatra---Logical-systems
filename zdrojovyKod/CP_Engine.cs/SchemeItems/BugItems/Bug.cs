using CP_Engine.BugItems;
using CP_Engine.SchemeItems;
using CP_Engine.SimulationItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CP_Engine
{
    public class Bug
    {
        public string Title { get; set; }

        internal int ID { get; private set; }
        internal int ColorID { get; set; }
        internal string Description { get; set; }
        internal DisplayTextConvertor DisplayTextContertor { get; set; }
        internal IODescriptionCollection IODecription { get; private set; }
        internal Scheme Scheme { get; set; }

        internal Bug(int id)
        {
            this.ID = id;
            this.Title = "";
            this.Description = "";
            this.DisplayTextContertor = new DisplayTextConvertor();
            this.IODecription = new IODescriptionCollection();
        }

        public bool IsUserCreated()
        {
            return Scheme != null;
        }

        /// <summary>
        /// Returns count of tiles, bug takes on horizontal side.
        /// Return count of tiles from left to right side fo bug.
        /// Calling this.GetBugWidth do not have to be same as calling this.Bug.GetBugWidth.
        /// </summary>
        /// <returns></returns>
        internal int GetBugWidth()
        {
            return Math.Max(2, Math.Max(this.IODecription.Get_Inputs().Count, this.IODecription.Get_Outputs().Count));
        }

        /// <summary>
        /// Copies Bug propherties, exclude scheme.
        /// </summary>
        /// <returns></returns>
        internal Bug Export()
        {
            Bug toReturn = new Bug(this.ID);
            toReturn.Title = this.Title;
            toReturn.ColorID = this.ColorID;
            toReturn.Description = this.Description;
            toReturn.DisplayTextContertor.Text = this.DisplayTextContertor.Text;
            toReturn.IODecription = this.IODecription.Export();
            return toReturn;
        }

        /// <summary>
        /// Sets properties to same values as provided Bug.
        /// Excludes scheme.
        /// </summary>
        /// <param name="source"></param>
        internal void Inport(Bug source, Point offset)
        {
            this.Title = source.Title;
            this.Description = source.Description;
            this.ColorID = source.ColorID;
            this.DisplayTextContertor.Text = source.DisplayTextContertor.Text;
            this.DisplayTextContertor.Changed();
            this.IODecription.Inport(source.IODecription, offset);
        }

        internal void SetMemmoryValue(Queue<bool> values, PhysScheme pScheme, Simulation sim)
        {
            foreach (PlacedBug pBug in Scheme.PlacedBugs.OrderedItems)
            {
                if (values.Count <= 0)
                    return;
                if (pBug.Bug.Scheme != null)
                {
                    //User created scheme.
                    pBug.Bug.SetMemmoryValue(values, pScheme.Children[pBug.ID], sim);
                }
                else if (pBug.Bug is MemmoryBug)
                {
                    //if (ecv.PhysScheme.Values[0] == ecv.NewValue)
                    //    return;
                    //Get special child scheme.
                    SpecialPhysScheme specialPscheme = pScheme.SpecialChildren[pBug.ID];
                    //Deque value 
                    bool newValue = values.Dequeue();
                    MemmoryValueBak.ChangeValue(sim, specialPscheme, newValue);
                }
            }
        }
        
        internal void GetMemmoryValue(List<bool> values, PhysScheme pScheme)
        {
            foreach (PlacedBug pBug in Scheme.PlacedBugs.OrderedItems)
            {
                if (pBug.Bug.Scheme != null)
                {
                    //User created scheme.
                    pBug.Bug.GetMemmoryValue(values, pScheme.Children[pBug.ID]);
                }
                else if (pBug.Bug is MemmoryBug)
                {
                    //Memmory bug
                    SpecialPhysScheme specialPscheme = pScheme.SpecialChildren[pBug.ID];
                    values.Add(specialPscheme.Values[0]);
                }
            }
        }

        internal virtual void ValueChanged(EventChangeValueSpecialBug ecv, Simulation sim) { }

        internal virtual void OuterInputChanged(bool inputValue, PhysScheme parentPScheme, OuterSchemeSource outerSSource, SpecialInnerSchemeSource innerSSource, Simulation sim) { }

        internal virtual bool GetInnerValue(PhysScheme parentPScheme, SchemeSource outerSchemeSource, SchemeSource innerSchemeSource) { return false; }

        internal virtual string GetDisplayText(PhysScheme parentScheme, PlacedBug pBug)
        {
            return this.DisplayTextContertor.GetText(parentScheme.Children[pBug.ID]);
        }

        /// <summary>
        /// When Bug is SpecialBug, returns how many bool values needs to be memmorized, to fully recover its current state.
        /// </summary>
        /// <returns></returns>
        internal virtual int GetValueSize() { return 0; }

        internal virtual void SpecialPhysSchemeCreated(SpecialPhysScheme pScheme, Simulation sim) { }
    }
}
