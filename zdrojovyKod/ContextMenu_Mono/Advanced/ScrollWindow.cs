using ContextMenu_Mono.Menu;
using ContextMenu_Mono.Menu.Inputs.Scroll;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilties_Mono;

namespace ContextMenu_Mono.ScrollWindowItems
{
    public class ScrollWindow
    {   
        public List<MenuPanel> MenuPanelItems { get; private set; }

        Point margin;
        int maxItemsShown;
        Point itemSize;
        MenuPanel mainPanel, stackPanel;
        ScrollBar scrollBar;
        int showFromIndex;

        public ScrollWindow(MenuPanelSettings mainPanelSettings, Point itemSize)
        {
            mainPanelSettings.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            this.itemSize = itemSize;
            this.MenuPanelItems = new List<MenuPanel>();
            //Create main panel.
            mainPanelSettings.IgnoreEffects = true;
            mainPanelSettings.ChildrenLayout = ChildrenLayouts.Normal;
            mainPanel = new MenuPanel(mainPanelSettings);

            //Split main panel to two panels.
            MenuPanelSettings s = mainPanelSettings;
            s.BorderWidth = 0;
            s.BackGroundTexture = mainPanelSettings.BackGroundTexture;
            s.Valign = VerticalAligment.Top;

            //Stack panel.
            s.Halign = HorizontalAligment.Left;
            s.Margin = new Point(mainPanelSettings.BorderWidth);
            s.Size = new Point(itemSize.X, mainPanelSettings.Size.Y - 2 * mainPanelSettings.BorderWidth);
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;
            stackPanel = new MenuPanel(s);
            mainPanel.Children.Add(stackPanel);

            //Calculate stack panel variabiles.
            maxItemsShown = s.Size.Y / itemSize.Y;
            int marginY = s.Size.Y - maxItemsShown * itemSize.Y;
            marginY = marginY / (maxItemsShown + 1);
            this.margin = new Point(0, marginY);

            //Scroll panel.
            s.BackGroundTexture = mainPanelSettings.BackGroundTexture;
            s.Margin = new Point(mainPanelSettings.BorderWidth);
            s.Halign = HorizontalAligment.Right;
            s.Size = new Point(mainPanelSettings.Size.X - 2 * mainPanelSettings.BorderWidth - itemSize.X, mainPanelSettings.Size.Y - 2 * mainPanelSettings.BorderWidth);
            scrollBar = new ScrollBar(s, 0);
            scrollBar.IndexChanged += ScrollBar_IndexChanged;
            mainPanel.Children.Add(scrollBar);

            mainPanel.Changed();
        }

        /// <summary>
        /// Not working OK
        /// </summary>
        /// <param name="mainPanelSettings"></param>
        /// <param name="itemSize"></param>
        public void Resize(MenuPanelSettings mainPanelSettings, Point itemSize)
        {
            mainPanelSettings.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            this.itemSize = itemSize;
            //Create main panel.
            mainPanelSettings.IgnoreEffects = true;
            mainPanelSettings.ChildrenLayout = ChildrenLayouts.Normal;
            mainPanel.Settings = mainPanelSettings;

            //Split main panel to two panels.
            MenuPanelSettings s = mainPanelSettings;
            s.BorderWidth = 0;
            s.BackGroundTexture = mainPanelSettings.BackGroundTexture;
            s.Valign = VerticalAligment.Top;

            //Stack panel.
            s.Halign = HorizontalAligment.Left;
            s.Margin = new Point(mainPanelSettings.BorderWidth);
            s.Size = new Point(itemSize.X, mainPanelSettings.Size.Y - 2 * mainPanelSettings.BorderWidth);
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;
            stackPanel.Settings = s;

            //Calculate stack panel variabiles.
            maxItemsShown = s.Size.Y / itemSize.Y;
            int marginY = s.Size.Y - maxItemsShown * itemSize.Y;
            marginY = marginY / (maxItemsShown + 1);
            this.margin = new Point(0, marginY);

            //Scroll panel.
            s.BackGroundTexture = mainPanelSettings.BackGroundTexture;
            s.Margin = new Point(mainPanelSettings.BorderWidth);
            s.Halign = HorizontalAligment.Right;
            s.Size = new Point(mainPanelSettings.Size.X - 2 * mainPanelSettings.BorderWidth - itemSize.X, mainPanelSettings.Size.Y - 2 * mainPanelSettings.BorderWidth);
            scrollBar.Resize(s);


            foreach (MenuPanel panel in this.MenuPanelItems)
            {
                s = panel.Settings;
                s.Size = this.itemSize;
                panel.Settings = s;
            }
            IndexChanged(this.showFromIndex);
            mainPanel.Changed();
        }

        public void Changed()
        {
            this.scrollBar.Set_ItemsCount(this.MenuPanelItems.Count - maxItemsShown);
        }

        private void ScrollBar_IndexChanged(MenuPanel sender, int showFromIndex)
        {
            IndexChanged(showFromIndex);
        }

        public void IndexChanged(int showFromIndex)
        {
            this.showFromIndex = showFromIndex;
            this.stackPanel.Children.Clear();
            //Fill stack panel with items.
            for (int i = showFromIndex; i < Math.Min(this.MenuPanelItems.Count, showFromIndex + maxItemsShown); i++)
            {
                this.MenuPanelItems[i].SetMargin(margin);
                this.stackPanel.Children.Add(this.MenuPanelItems[i]);
            }
            stackPanel.Changed();
        }

        public MenuPanel GetMainPanel()
        {
            return mainPanel;
        }
    }
}
