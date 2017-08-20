using CP_Engine.SchemeEvents;
using System;
using System.Collections.Generic;

namespace CP_Engine
{
    /// <summary>
    /// Holds history of changes upon schemes.
    /// </summary>
    public class SchemeEventHistory
    {
        public event CollectionChangedHandler CollectionChanged;
        public delegate void CollectionChangedHandler(bool canUndo, bool canRedo);

        WorkPlace workplace;
        int index;                          //Current position within items.
        int maxItems;                       //Maximum count of SchemeEvents in this items.
        bool eventStarted;                  //debug variabile DEBUG

        List<SchemeEvent> items;            //SchemeEvents, that user can browse.
        SchemeEvent currentEvent;           //SchemeEvent representation of currenly modified scheme.
        bool insertCurrentEventToHistory;   //Determines whether currentEvent will be inserted into history.

        internal SchemeEventHistory(WorkPlace workplace)
        {
            this.workplace = workplace;
            Reset();
        }

        /// <summary>
        /// Resets to empty state.
        /// </summary>
        internal void Reset()
        {
            this.currentEvent = null;
            this.items = new List<SchemeEvent>();
            index = -1;
            maxItems = 8;
            CallEvent();
        }

        public void Undo()
        {
            if (CanUndo())
            {
                workplace.Simulation.StopThread();
                items[index].Undo(workplace);
                index--;
                workplace.Simulation.StartThread();
            }
            CallEvent();
        }

        public void Redo()
        {
            if (CanRedo())
            {
                workplace.Simulation.StopThread();
                index++;
                items[index].Redo(workplace);
                workplace.Simulation.StartThread();
            }
            CallEvent();
        }

        public bool CanUndo()
        {
            if (index >= 0)
                return true;
            return false;
        }

        public bool CanRedo()
        {
            if (index + 1 < items.Count)
                return true;
            return false;
        }

        /// <summary>
        /// Call before making changes to scheme.
        /// This function stops simulation.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="insertToHistory"></param>
        internal void StartEvent(Scheme scheme, bool insertToHistory)
        {
            if (this.eventStarted)
                throw new Exception("Chyba  SCHEMEEVENT");
            this.insertCurrentEventToHistory = insertToHistory;
            eventStarted = true;
            workplace.Simulation.StopThread();

            currentEvent = new SchemeEvent(scheme);
        }

        /// <summary>
        /// Call when PBugs containing currenly modified scheme, 
        /// has to be replaced upon FinalizingEvent.
        /// </summary>
        internal void EventRequiresBugReplace()
        {
            if (currentEvent != null)
                currentEvent.RequiresReplace = true;
        }

        /// <summary>
        /// Finalizes changes within scheme.
        /// If changes were invalid, this functions undo them.
        /// Function starts simulation.
        /// </summary>
        internal void FinalizeEvent()
        {
            if (this.eventStarted == false)
                throw new Exception("Chyba  SCHEMEEVENT");
            eventStarted = false;

            if (currentEvent.HasChanged() && currentEvent.Validate(workplace) && this.insertCurrentEventToHistory)
            {
                workplace.CloseWindows();
                index++;
                int toRemove = items.Count - index;
                if (toRemove > 0)
                    items.RemoveRange(index, toRemove);
                items.Add(currentEvent);

                if (items.Count > maxItems)
                {
                    items.RemoveAt(0);
                    index--;
                }
                CallEvent();
            }
            workplace.Simulation.StartThread();
        }
        private void CallEvent()
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(CanUndo(), CanRedo());
            }
        }
    }
}
