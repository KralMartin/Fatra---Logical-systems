using System;
using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using CP_Engine;
using Microsoft.Xna.Framework;
using Utilties_Mono;

namespace CP_v1
{
    class HalfScreen
    {
        const int buttonMargin = 10;
        const int buttonHeight = 20;
        const int buttonPanelHeight = 30;

        internal MenuPanel ContentPanel { get; private set; }

        protected SelectCodeScreen screen;
        protected Point itemSize;
        protected CheckMenuPanelGroup checkGroup;
        private MenuPanel mainPanel;
        MenuPanel buttonPanel;

        internal HalfScreen(SelectCodeScreen screen, HorizontalAligment halign, string title)
        {
            this.screen = screen;
            MenuPanelSettings s = new MenuPanelSettings();
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.Gray());
            s.Size = new Point(ImportantClassesCollection.ScreenSize.X / 2, ImportantClassesCollection.ScreenSize.Y);
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.Halign = halign;
            s.IgnoreEffects = true;
            s.TextValign = VerticalAligment.Top;
            s.TextHalign = HorizontalAligment.Center;
            s.TextMargin = new Point(0,8);
            mainPanel = new MenuPanel(s);
            mainPanel.Text = title;
            if (halign == HorizontalAligment.Right)
                CreateExitButton();
            mainPanel.Changed();

            s.Size = new Point(mainPanel.Settings.Size.X, mainPanel.Settings.Size.Y - 2 * buttonPanelHeight);
            s.Margin = new Point(0, buttonPanelHeight);
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.DarkGray());
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Blue);
            s.BorderWidth = 1;

            ContentPanel = new MenuPanel(s);
            mainPanel.Children.Add(ContentPanel);
            this.itemSize = new Point(this.ContentPanel.Settings.Size.X - 20, 50);
            this.checkGroup = new CheckMenuPanelGroup();
        }

        internal virtual void Resize()
        {
            MenuPanelSettings s = mainPanel.Settings;
            s.Size = new Point(ImportantClassesCollection.ScreenSize.X / 2, ImportantClassesCollection.ScreenSize.Y);
            mainPanel.Settings = s;
            mainPanel.Changed(new Rectangle(new Point(), ImportantClassesCollection.ScreenSize));

            s = ContentPanel.Settings;
            s.Size = new Point(mainPanel.Settings.Size.X, mainPanel.Settings.Size.Y - 2 * buttonPanelHeight);
            ContentPanel.Settings = s;
            this.itemSize = new Point(this.ContentPanel.Settings.Size.X - 20, 50);

            foreach (MenuPanel btn in buttonPanel.Children)
            {
                s = btn.Settings;
                s.Size = new Point((mainPanel.Settings.Size.X - buttonMargin * (buttonPanel.Children.Count + 1)) / buttonPanel.Children.Count, buttonHeight);
                s.Margin = new Point(buttonMargin, 0);
                btn.Settings = s;
            }
            mainPanel.Changed(new Rectangle(new Point(), ImportantClassesCollection.ScreenSize));
        }

        private void CreateExitButton()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Red);
            s.Size = new Point(buttonPanelHeight-2, buttonPanelHeight-2);
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.Halign = HorizontalAligment.Right;
            s.Margin = new Point(5, 0);
            s.TextValign = VerticalAligment.Center;
            s.TextHalign = HorizontalAligment.Center;
            MenuPanel panel = new MenuPanel(s);
            panel.Text = "X";
            panel.Clicked += Panel_Clicked;
            mainPanel.Children.Add(panel);
        }

        private void Panel_Clicked(MenuPanel sender)
        {
            if (screen.Workplace.Project.Programmability.RecompileFunctions())
            {
                screen.GoToGameScreen();
                return;
            }
            Form form = DefaultUI.CreateFormQuestion("Error", "Compiler functions contain errors. Do you still want to exit?");
            form.AfterClose += Form_AfterClose;            
        }

        private void Form_AfterClose(Form sender, bool result)
        {
            if (result)
                screen.GoToGameScreen();
        }

        protected void CreateButtons(string text1, string text2, string text3, string text4, int buttonCount)
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.IgnoreEffects = true;
            s.Valign = VerticalAligment.Bottom;
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.Gray());
            s.ChildrenLayout = ChildrenLayouts.HorizontalStack;
            s.Size = new Point(mainPanel.Settings.Size.X, buttonPanelHeight);
            buttonPanel = new MenuPanel(s);
            mainPanel.Children.Add(buttonPanel);

            if (buttonCount > 0)
                buttonPanel.Children.Add(CreateButton(text1, buttonCount, 0));
            if (buttonCount > 1)
                buttonPanel.Children.Add(CreateButton(text2, buttonCount, 1));
            if (buttonCount > 2)
                buttonPanel.Children.Add(CreateButton(text3, buttonCount, 2));
            if (buttonCount > 3)
                buttonPanel.Children.Add(CreateButton(text4, buttonCount, 3));
        }

        protected CheckMenuPanel DefaultCheckBox()
        {
            //General settings.
            MenuPanelSettings s = new MenuPanelSettings();
            s.Size = itemSize;
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Gray);
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            s.BorderWidth = 2;
            CheckMenuPanel check = new CheckMenuPanel(s, checkGroup, CheckTypes.Border);

            //Checked settings.
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Purple);
            s.BorderWidth = 4;
            check.CheckedSettings = s;
            return check;
        }

        protected virtual void ButtonClicked(int buttonIndex) { }

        private MenuPanel CreateButton(string text1, int buttonCount, int buttonIndex)
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.Size = new Point((mainPanel.Settings.Size.X - buttonMargin * (buttonCount + 1)) / buttonCount, buttonHeight);
            s.Margin = new Point(buttonMargin, 0);
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.Valign = VerticalAligment.Center;
            s.Halign = HorizontalAligment.Left;
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Gray);
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            s.BorderWidth = 1;

            MenuPanel btn = new MenuPanel(s);
            btn.Tag = buttonIndex;
            btn.Text = text1;
            btn.Clicked += Btn_Clicked;
            return btn;
        }

        private void Btn_Clicked(MenuPanel sender)
        {
            ButtonClicked((int)sender.Tag);
        }

        internal MenuPanel GetPanel()
        {
            mainPanel.Changed(new Rectangle(new Point(), ImportantClassesCollection.ScreenSize));
            return mainPanel;
        }
    }
}
