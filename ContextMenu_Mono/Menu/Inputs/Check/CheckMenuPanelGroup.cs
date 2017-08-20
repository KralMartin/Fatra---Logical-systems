using System;
using System.Collections.Generic;

namespace ContextMenu_Mono.Menu
{
    /// <summary>
    /// Groups CheckMenuPanels to a group, 
    /// where only one CheckMenuPanel can be checked at time.
    /// </summary>
    public class CheckMenuPanelGroup
    {
        internal List<CheckMenuPanel> Panels { get; private set; }

        public CheckMenuPanelGroup()
        {
            Panels = new List<CheckMenuPanel>();
        }

        public void Remove(CheckMenuPanel checkPanel)
        {
            Panels.Remove(checkPanel);
        }

        internal void CheckedChanged(CheckMenuPanel panel, bool fireEvent)
        {
            foreach (CheckMenuPanel current in Panels)
            {
                if (ReferenceEquals(current, panel))
                    current.ChangeCheckedAndFireEvent(fireEvent);
                else if (current.Checked)
                    current.ChangeCheckedAndFireEvent(fireEvent);
            }
        }

        public CheckMenuPanel GetChecked()
        {
            foreach (CheckMenuPanel panel in Panels)
            {
                if (panel.Checked)
                    return panel;
            }
            return null;
        }

        public void Clear()
        {
            this.Panels.Clear();
        }
    }
}
