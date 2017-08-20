using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using System;
using Utilties_Mono;

namespace ContextMenu_Mono.Advanced
{
    public class Form
    {
        public delegate void AfterCloseEventHandler(Form sender, bool result);
        public event AfterCloseEventHandler AfterClose;

        public delegate void BeforeCloseEventHandler(Form sender, bool result, ref bool closeForm);
        public event BeforeCloseEventHandler BeforeClose;

        FormSettings settings;
        Controler menuLayer;
        MenuPanel block;
        MenuPanel titleBar;
        MenuPanel buttonExit;
        MenuPanel formMainContainer;
        MenuPanel buttonOK;
        MenuPanel buttonCancel;
        MenuPanel content;
        MenuPanel bottomBar;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings">Settings of form.</param>
        /// <param name="menuLayer">Parent controler, that is drawn.</param>
        /// <param name="content">Content of form. It also determines size of form.</param>
        public Form(FormSettings settings, Controler menuLayer, MenuPanel content)
        {
            this.settings = settings;
            this.menuLayer = menuLayer;
            this.content = content;
            this.content.Changed(new Rectangle());
            
            CreateBlockingPanel();
            CreateWindow();
            formMainContainer.Children.Add(titleBar);
            formMainContainer.Children.Add(content);
            formMainContainer.Children.Add(bottomBar);
            block.Children.Add(formMainContainer);
            block.Changed(new Rectangle(0, 0, settings.ScreenSize.X, settings.ScreenSize.Y));
            menuLayer.Push(block);
            block.DebugValue = "blocking panel";
        }

        #region Getters
        public MenuPanel Get_TitleBar()
        {
            return titleBar;
        }

        public MenuPanel Get_ButtonExit()
        {
            return buttonExit;
        }

        public MenuPanel Get_MainContainer()
        {
            return formMainContainer;
        }

        public MenuPanel Get_ButtonOK()
        {
            return buttonOK;
        }

        public MenuPanel Get_ButtonCancel()
        {
            return buttonCancel;
        }

        public MenuPanel Get_ContentMenuPanlel()
        {
            return content;
        }

        public MenuPanel Get_BottomBar()
        {
            return bottomBar;
        }
        #endregion

        private void CreateWindow()
        {
            int borderWidth = 4;

            MenuPanelSettings s = new MenuPanelSettings();
            Point contentSize= content.GetSize();
            s.Size = new Point(contentSize.X , contentSize.Y + 2 * GetBarHeight());
            s.Font = settings.Font;
            s.ChildrenLayout = ChildrenLayouts.Normal;
            s.BackGroundTexture = settings.FormBackgroundTexture;
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;
            s.Halign = HorizontalAligment.Center;
            s.Valign = VerticalAligment.Center;
            s.BorderWidth = borderWidth;
            s.IgnoreEffects = true;
            s.BorderColor = settings.FormBorderTexture;
            formMainContainer = new MenuPanel(s);

            s.BorderWidth = 0;
            s.Size = new Point(contentSize.X, GetBarHeight());
            s.Halign = HorizontalAligment.Left;
            s.Valign = VerticalAligment.Top;
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.BackGroundTexture = settings.FormBorderTexture;
            titleBar = new MenuPanel(s);
            titleBar.Text = settings.TitleBarText;

            s.ChildrenLayout = ChildrenLayouts.HorizontalStack;
            s.BackGroundTexture = settings.FormBackgroundTexture;
            bottomBar = new MenuPanel(s);
            CreteButtons();

            s.Size = new Point(s.Size.Y - 8, s.Size.Y - 8);
            s.IgnoreEffects = false;
            s.Margin = new Point(4 + borderWidth, 0);
            s.Halign = HorizontalAligment.Right;
            s.Valign = VerticalAligment.Center;
            s.BackGroundTexture = settings.ExitButtonTexture;
            buttonExit = new MenuPanel(s);
            buttonExit.Text = "X";
            buttonExit.Clicked += XButton_Clicked;
            titleBar.Children.Add(buttonExit);

            formMainContainer.Mover = titleBar;
        }

        private void CreteButtons()
        {
            int buttonWidth =(int) Math.Max(settings.Font.MeasureString(settings.ButtonOKText).X, settings.Font.MeasureString(settings.ButtonCancelText).X);
            buttonWidth += 16;

            MenuPanelSettings s = GetButtonSettings();
            s.Size = new Point(buttonWidth, 0);
            s.AdjustWidthToContent = false;

            if (settings.ButtonCancelText.Length > 0)
            {
                buttonCancel = new MenuPanel(s);
                buttonCancel.Text = settings.ButtonCancelText;
                buttonCancel.Clicked += ButtonCancel_Clicked;
                bottomBar.Children.Add(buttonCancel);
            }
            if (settings.ButtonOKText.Length > 0)
            {
                buttonOK = new MenuPanel(s);
                buttonOK.Text = settings.ButtonOKText;
                buttonOK.Clicked += ButtonOK_Clicked;
                bottomBar.Children.Add(buttonOK);
            }
        }

        private MenuPanelSettings GetButtonSettings()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.BorderWidth = 2;
            s.Size = new Point(0, 0);
            s.BorderColor = settings.FormBorderTexture;
            s.AdjustWidthToContent = true;
            s.AdjustHeightToContent = true;
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.TextMargin = new Point(0, settings.Font.LineSpacing / 4);
            s.Valign = VerticalAligment.Top;
            s.Halign = HorizontalAligment.Left;
            s.Margin = new Point(8, 2);
            s.Font = settings.Font;
            s.BackGroundTexture = settings.OKCancelButtonsTexture;
            return s;
        }

        private void CreateBlockingPanel()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.Size = settings.ScreenSize;
            s.ChildrenLayout = ChildrenLayouts.Normal;
            s.IgnoreEffects = true;
            s.AllowChildrenMovement = true;
            if(ImportantClassesCollection.AddBlockingPanel())
                s.BackGroundTexture = settings.BackgroundTexture;
            block = new MenuPanel(s);
        }

        private void ButtonOK_Clicked(MenuPanel sender)
        {
            Close(true);
        }

        private void ButtonCancel_Clicked(MenuPanel sender)
        {
            Close(false);
        }

        private void XButton_Clicked(MenuPanel sender)
        {
            Close(false);
        }

        private int GetBarHeight()
        {
            return (int)settings.Font.LineSpacing * 2;
        }

        public void Close(bool result)
        {
            bool closeForm = true;
            if (BeforeClose != null)
                BeforeClose(this, result, ref closeForm);
            if (closeForm)
            {
                this.block.Pop();
                ImportantClassesCollection.RemoveBlockingPanel();
                if (AfterClose != null)
                    AfterClose(this, result);
            }
        }
    }
}
