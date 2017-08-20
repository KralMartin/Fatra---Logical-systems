using Microsoft.Xna.Framework;
using System;
using Utilties_Mono;

namespace ContextMenu_Mono.Menu.Inputs.Scroll
{
    class ScrollBar : MenuPanel
    {
        public delegate void IndexChangedEventHandler(MenuPanel sender, int showFromIndex);
        public event IndexChangedEventHandler IndexChanged;

        internal int ShowFromIndex { get; private set; }
        internal int ItemsCount { get; private set; }

        MenuPanel scrollBar;
        MenuPanel scrollSpace;
        MenuPanel arrowTop,bottomArrow;

        public ScrollBar(MenuPanelSettings settings, int itemsCount) : base(settings)
        {
            MenuPanelSettings s = settings;
            this.ItemsCount = itemsCount;

            //Create scroll panel children.
            Point buttonSize = new Point(s.Size.X, Math.Min(20, s.Size.Y));
            int scrollSpaceHeight = s.Size.Y - 2 * buttonSize.Y;
            s = settings;
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.BorderWidth = 1;
            s.IgnoreEffects = false;
            s.Margin = new Point();

            //Scroll panel: Top arrow.
            s.Size = buttonSize;
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.GetTexture("Diode/North");
            arrowTop = new MenuPanel(s);
            arrowTop.Text = "I";
            arrowTop.Clicked += ArrowTop_Clicked;
            this.Children.Add(arrowTop);

            //Scroll panel: Bottom arrow.
            s.Size = buttonSize;
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.GetTexture("Diode/South");
            bottomArrow = new MenuPanel(s);
            bottomArrow.Text = "I";
            bottomArrow.Clicked += BottomArrow_Clicked;

            //Scroll panel: Scroll space.
            s.AllowChildrenMovement = true;
            s.BackGroundTexture = null;
            s.IgnoreEffects = true;
            s.Size = new Point(buttonSize.X, scrollSpaceHeight);
            scrollSpace = new MenuPanel(s);
            this.Children.Add(scrollSpace);

            this.Children.Add(bottomArrow);

            //Scroll panle: Scroll bar.
            s.AllowChildrenMovement = false;
            s.Size = new Point(s.Size.X, 44);
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.MediumAquamarine);
            scrollBar = new MenuPanel(s);
            scrollBar.Mover = scrollBar;
            scrollBar.Moved += ScrollBar_Moved;
            scrollSpace.Children.Add(scrollBar);
        }

        internal void Resize(MenuPanelSettings settings)
        {
            MenuPanelSettings s = settings;

            //Create scroll panel children.
            Point buttonSize = new Point(s.Size.X, Math.Min(20, s.Size.Y));
            int scrollSpaceHeight = s.Size.Y - 2 * buttonSize.Y;
            s = settings;
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.BorderWidth = 1;
            s.IgnoreEffects = false;
            s.Margin = new Point();

            //Scroll panel: Top arrow.
            s.Size = buttonSize;
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.GetTexture("Diode/North");
            arrowTop.Settings = s;

            //Scroll panel: Bottom arrow.
            s.Size = buttonSize;
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.GetTexture("Diode/South");
            bottomArrow.Settings = s;

            //Scroll panel: Scroll space.
            s.AllowChildrenMovement = true;
            s.BackGroundTexture = null;
            s.IgnoreEffects = true;
            s.Size = new Point(buttonSize.X, scrollSpaceHeight);
            scrollSpace.Settings = s;

            //Scroll panle: Scroll bar.
            s.AllowChildrenMovement = false;
            s.Size = new Point(s.Size.X, 44);
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.MediumAquamarine);
            scrollBar.Settings = s;
        }

        public void Set_ItemsCount(int value)
        {
            this.ItemsCount = value;
            Set_ShowFromIndex(ShowFromIndex);
        }

        private void ScrollBar_Moved(MenuPanel sender)
        {
            double totalSpaceToMove = this.scrollSpace.Settings.Size.Y - 2 * this.scrollSpace.Settings.BorderWidth - this.scrollBar.Settings.Size.Y;
            double scrollBarPosition = this.scrollBar.Settings.Margin.Y;
            double ratePosition = scrollBarPosition / totalSpaceToMove;
            double showFromIndexDouble = ratePosition * (ItemsCount - 1);
            int newIndex = (int)Math.Round(showFromIndexDouble);
            if (newIndex != ShowFromIndex)
            {
                ShowFromIndex = newIndex;
                if (ShowFromIndex > ItemsCount)
                    ShowFromIndex = ItemsCount;
                if (ShowFromIndex < 0)
                    ShowFromIndex = 0;
                if (IndexChanged != null)
                    IndexChanged(this, ShowFromIndex);
            }
        }

        private void BottomArrow_Clicked(MenuPanel sender)
        {
            Set_ShowFromIndex(this.ShowFromIndex + 1);
        }

        private void ArrowTop_Clicked(MenuPanel sender)
        {
            Set_ShowFromIndex(this.ShowFromIndex - 1);
        }

        private void Set_ShowFromIndex(int value)
        {
            ShowFromIndex = value;
            if (ShowFromIndex > ItemsCount)
                ShowFromIndex = ItemsCount;
            if (ShowFromIndex < 0)
                ShowFromIndex = 0;

            //Change scrollBar position.

            if (ShowFromIndex == ItemsCount)
                this.scrollBar.SetMargin(new Point(0, scrollSpace.Settings.Size.Y - scrollSpace.Settings.BorderWidth - scrollBar.Settings.Size.Y));
            else
            {
                double totalSpaceToMove = this.scrollSpace.Settings.Size.Y - 2 * this.scrollSpace.Settings.BorderWidth - this.scrollBar.Settings.Size.Y;
                int oneBit = (int)(totalSpaceToMove / ItemsCount);
                this.scrollBar.SetMargin(new Point(0, ShowFromIndex * oneBit));
            }
            this.Changed();
            if (IndexChanged != null)
                IndexChanged(this, ShowFromIndex);
        }
    }
}
