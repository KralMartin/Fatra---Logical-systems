using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilties_Mono;
using Microsoft.Xna.Framework.Graphics;

namespace CP_Engine
{
    class ValueWatchForm
    {
        const int formWidth = 100;
        const int borderWidth = 2;
        const int rowHeight = 20;

        MenuPanel titleBar, mainPanel;

        internal ValueWatchForm()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.BackGroundTexture = DefaultUI.GetFormBackGroundColor();
            s.BorderColor = s.BackGroundTexture;
            s.BorderWidth = borderWidth;
            s.Size = new Point(formWidth+ 2* borderWidth, 2*borderWidth+3*rowHeight);
            s.Margin = new Point(300, 300);
            //s.IgnoreEffects = true;
            mainPanel = new MenuPanel(s);
            mainPanel.DebugValue = "WTF";
            mainPanel.Clicked += MainPanel_Clicked;
            CreateTitleBar();
            mainPanel.Children.Add(titleBar);

            mainPanel.Changed();
            //ImportantClassesCollection.MainControler.Push(mainPanel);
            WorkPlace.Instance.controler.Push(mainPanel);
        }

        private void MainPanel_Clicked(MenuPanel sender)
        {

        }

        private void CreateTitleBar()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.BackGroundTexture = DefaultUI.GetFormBorderColor();
            s.Size = new Point(formWidth, rowHeight);
            s.Margin = new Point(borderWidth, borderWidth);
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.IgnoreEffects = false;
            s.ChildrenLayout = ChildrenLayouts.HorizontalStack;
            titleBar = new MenuPanel(s);

            //Buttons settings
            s.Size = new Point(rowHeight - 4, rowHeight - 4);
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.IgnoreEffects = false;
            s.BackGroundTexture = DefaultUI.GetFormBackGroundColor();

            //X button
            MenuPanel panel = new MenuPanel(s);
            panel.Text = "X";
            titleBar.Children.Add(panel);

            //X button
            panel = new MenuPanel(s);
            panel.Text = "T";
            titleBar.Children.Add(panel);
        }
    }
}
