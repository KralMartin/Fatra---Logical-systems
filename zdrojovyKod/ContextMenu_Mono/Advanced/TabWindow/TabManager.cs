using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Utilities_Mono;
using Utilties_Mono;
using System;

namespace ContextMenu_Mono.TabWindow
{
    public class TabManager
    {
        /// <summary>
        /// Fired when CurrentTab property is changed.
        /// </summary>
        public event TabChangedEventHandler TabChanged;
        public delegate void TabChangedEventHandler(Tab newTab);

        /// <summary>
        /// Currently visible tab.
        /// </summary>
        public Tab CurrentTab { get; private set; }

        /// <summary>
        /// Bounds of tab.
        /// </summary>
        public Rectangle TabContentBounds { get; private set; }

        MenuPanel menu;
        CheckMenuPanelGroup group;

        int tabHeight;                      //Height of tab-name panel.
        Texture2D tabPanelTexture;          //Texture of tab-name panel. 
        Texture2D removeButtonTexture;      //Texture of button, that removes Tab.
        List<Tab> tabs;                     //List of all tabs.
        RectBorder border;                  //Border of whole tab manager (tab-name panel + tab-content panel).
        SpriteFont font;
        Texture2D borderTexture;
        Texture2D checkedTexture;

        public TabManager(Rectangle globalBounds, int tabHeight, int borderWidth, SpriteFont font, Controler parentControler, Texture2D tabPanelTexture, Texture2D borderTexture, Texture2D removeButtonTexture, Texture2D checkedTexture) 
        {
            this.tabHeight = tabHeight;
            this.tabPanelTexture = tabPanelTexture;
            this.font = font;
            this.borderTexture = borderTexture;
            this.removeButtonTexture = removeButtonTexture;
            this.checkedTexture = checkedTexture;
            border = new RectBorder(borderWidth, globalBounds, borderTexture);      
                  
            Rectangle modifiedRect = border.GetInnerRectange();
            modifiedRect.Y += tabHeight;
            modifiedRect.Height -= tabHeight;
            this.TabContentBounds = modifiedRect;
            
            group = new CheckMenuPanelGroup();

            Rectangle menuGlobalAllowedArea = border.GetInnerRectange();
            menuGlobalAllowedArea.Height = tabHeight;

            MenuPanelSettings settings = new MenuPanelSettings();
            settings.BackGroundTexture = borderTexture;
            settings.ChildrenLayout = ChildrenLayouts.HorizontalStack;
            settings.IgnoreEffects = true;
            settings.Size = new Point(menuGlobalAllowedArea.Width, menuGlobalAllowedArea.Height);
            settings.Margin = new Point(menuGlobalAllowedArea.X, menuGlobalAllowedArea.Y);

            menu = new MenuPanel(settings);
            menu.Changed(new Rectangle());
            parentControler.Push(menu);
            menu.DebugValue = "tab manager";

            tabs = new List<Tab>();
        }

        public void Pop()
        {
            this.menu.Pop();
        }

        public void Resize(Rectangle bounds)
        {
            border = new RectBorder(border.Get_BorderWidth(), bounds, borderTexture);

            Rectangle modifiedRect = border.GetInnerRectange();
            modifiedRect.Y += tabHeight;
            modifiedRect.Height -= tabHeight;
            this.TabContentBounds = modifiedRect;

            Rectangle menuGlobalAllowedArea = border.GetInnerRectange();
            menuGlobalAllowedArea.Height = tabHeight;

            MenuPanelSettings settings = menu.Settings;
            settings.Size = new Point(menuGlobalAllowedArea.Width, menuGlobalAllowedArea.Height);
            settings.Margin = new Point(menuGlobalAllowedArea.X, menuGlobalAllowedArea.Y);
            menu.Settings = settings;
            menu.Changed(new Rectangle());

            foreach (Tab tab in tabs)
            {
                tab.Set_Bounds(this.TabContentBounds);

            }
        }

        public void Add(string tabName, Tab tab)
        {
            this.tabs.Add(tab);
            tab.Set_Bounds(this.TabContentBounds);
            int textWidth =(int) font.MeasureString(tabName).X;
            MenuPanelSettings settings = new MenuPanelSettings();
            settings.Margin = new Point(10, 0);
            settings.TextMargin = new Point(4, 0);
            settings.Size = new Point(3 * 4 + textWidth + tabHeight * 2 / 3, menu.GetSize().Y);
            settings.TextHalign = HorizontalAligment.Left;
            settings.TextValign = VerticalAligment.Center;
            settings.Font = font;
            settings.BackGroundTexture = this.tabPanelTexture;

            CheckMenuPanel buttonTab = new CheckMenuPanel(settings, group, CheckTypes.Background);
            buttonTab.CheckedSettings = GetCheckedSettings();
            buttonTab.Text = tabName;
            buttonTab.Tag = tab;
            buttonTab.CheckedChanged += CheckedChanged;
            menu.Children.Add(buttonTab);
            tab.Button = buttonTab;            

            settings = new MenuPanelSettings();
            settings.AdjustWidthToContent = false;
            settings.Size = new Point(this.tabHeight * 2 / 3, this.tabHeight * 2 / 3);
            settings.Valign = VerticalAligment.Center;
            settings.Halign = HorizontalAligment.Right;
            settings.Margin = new Point(4, 0);
            settings.Font = font;
            settings.BackGroundTexture = this.removeButtonTexture;

            MenuPanel removeButton = new MenuPanel(settings);
            removeButton.Tag = buttonTab;
            removeButton.Clicked += RemoveButton_Clicked;
            buttonTab.Children.Add(removeButton);

            menu.Changed(new Rectangle());
        }

        private MenuPanelSettings GetCheckedSettings()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.BackGroundTexture = checkedTexture;
            return s;
        }

        public Tab[] GetTabs()
        {
            return this.tabs.ToArray();
        }

        public void SelectTab(Tab newTab, bool fireEvent)
        {
            newTab.Button.Set_Checked(true, true);
        }

        internal void SwitchTab(Tab newTab, bool fireEvent)
        {
            if (object.ReferenceEquals(newTab, this.CurrentTab) == false)
            {
                if (this.CurrentTab != null)
                    this.CurrentTab.TabVisibilityChanged(false);
                this.CurrentTab = newTab;
                if(this.CurrentTab!=null)
                    this.CurrentTab.TabVisibilityChanged(true);
                if (TabChanged != null)
                    TabChanged(this.CurrentTab);
            }
        }

        public void RemoveTab(Tab tab)
        {
            if (this.tabs.Remove(tab) == false)
                return;
            menu.Children.Remove(tab.Button);

            Tab newTab = null;
            if (menu.Children.Count > 0)
                newTab = (Tab)menu.Children[menu.Children.Count - 1].Tag;
            SwitchTab(newTab, true);
            this.CurrentTab.Button.Set_Checked(true, false);
            menu.Changed(new Rectangle());
        }

        private void RemoveButton_Clicked(MenuPanel sender)
        {
            MenuPanel removeButton = (MenuPanel)sender;
            MenuPanel tabButton = (MenuPanel)removeButton.Tag;
            RemoveTab((Tab)tabButton.Tag);
        }

        private void CheckedChanged(CheckMenuPanel sender)
        {
            if(sender.Checked)
                SwitchTab((Tab)sender.Tag, true);
        }

        public void Draw(SpriteBatch sb)
        {
            if (CurrentTab != null)
                CurrentTab.Draw(sb);
            menu.ControlerDraw(sb);
            border.DrawStatic(sb);
        }
    }
}
