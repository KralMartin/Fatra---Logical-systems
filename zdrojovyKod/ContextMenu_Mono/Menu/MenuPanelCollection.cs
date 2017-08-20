using System.Collections;
using System.Collections.Generic;

namespace ContextMenu_Mono.Menu
{
    public class MenuPanelCollection : IEnumerable
    {
        List<MenuPanel> items;
        MenuPanel owner;

        internal MenuPanelCollection(MenuPanel owner)
        {
            this.owner = owner;
            items = new List<MenuPanel>();
        }

        public MenuPanel this[int i]
        {
            get
            {
                return items[i];
            }
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        public void Remove(MenuPanel panel)
        {
            if (items.Remove(panel))
                panel.Parent = null;
        }

        public void Add(MenuPanel panel)
        {
            panel.Parent = owner;
            items.Add(panel);
        }

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }

    }
}
