using System;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilties_Mono;

namespace ContextMenu_Mono.Advanced
{
    public class DefaultUI
    {
        public static Texture2D GetFormBorderColor()
        {
            return ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Blue); 
        }
        
        public static SpriteFont GetDefaultFont()
        {
            return ImportantClassesCollection.TextureLoader.GetFont("f1");
        }

        public static Texture2D GetButtonColor()
        {
            return ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.DimGray);
        }

        public static Texture2D GetFormBackGroundColor()
        {
            return ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Gray);
        }
        
        public static Form CreateFormText(string titleText, string text)
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.AdjustHeightToContent = true;
            s.AdjustWidthToContent = true;
            s.TextMargin = new Point(30, 30);
            s.Size = new Point(100, 50);

            MenuPanel textPanel = new MenuPanel(s);
            textPanel.Text = text;

            FormSettings fs = DefaultFormSettings(titleText);
            fs.ButtonCancelText = "";
            Form form = new Form(fs, ImportantClassesCollection.MenuLayer, textPanel);
            return form;
        }

        public static FormSettings DefaultFormSettings(string titleText)
        {
            FormSettings formSettings = new FormSettings();
            formSettings.BackgroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(Color.Gray, 0.3f));
            formSettings.ExitButtonTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Red);
            formSettings.FormBackgroundTexture = GetFormBackGroundColor();
            formSettings.FormBorderTexture = GetFormBorderColor();
            formSettings.OKCancelButtonsTexture = GetButtonColor();
            formSettings.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            formSettings.ScreenSize = ImportantClassesCollection.ScreenSize;
            formSettings.TitleBarText = titleText;
            formSettings.ButtonCancelText = "cancel";
            formSettings.ButtonOKText = "OK";
            return formSettings;
        }
        
        public static Form CreateDefaultForm(string titleText, MenuPanel content)
        {
            Form form = new Form(DefaultFormSettings(titleText), ImportantClassesCollection.MenuLayer, content);
            return form;
        }

        public static Form CreateFormQuestion(string caption, string question)
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.TextMargin = new Point(20, 20);
            s.Size = new Point(256, 0);
            s.AdjustHeightToContent = true;

            MenuPanel content = new MenuPanel(s);
            content.Text = question;
            content.Changed();

            FormSettings fs = DefaultUI.DefaultFormSettings(caption);
            fs.ButtonOKText = "Yes";
            fs.ButtonCancelText = "No";
            Form form = new Form(fs, ImportantClassesCollection.MenuLayer, content);
            return form;
        }

        public static MenuPanelSettings DefaultLabelSettings()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.Margin = new Point(20, 20);
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Transparent);
            s.Size = new Point(200, 20);
            s.TextValign = VerticalAligment.Center;
            s.TextHalign = HorizontalAligment.Left;
            s.TextMargin = new Point(10, 0);
            s.ChildrenLayout = ChildrenLayouts.Normal;
            return s;
        }

        public static MenuPanelSettings DefaultNumericInput()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.TextValign = VerticalAligment.Center;
            s.TextHalign = HorizontalAligment.Center;
            s.TextMargin = new Point(10, 0);

            s.Margin = new Point(10, 0);
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            s.BorderWidth = 1;
            s.Size = new Point(250, s.Font.LineSpacing + 10);
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.LightGray);
            return s;
        }

        public static MenuPanelSettings DefaultButtonSettings()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.Font = GetDefaultFont();
            s.BorderWidth = 2;
            s.Size = new Point(0, 0);
            s.BorderColor = GetFormBorderColor();
            s.AdjustWidthToContent = true;
            s.AdjustHeightToContent = true;
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.TextMargin = new Point(16, 4);
            s.Valign = VerticalAligment.Top;
            s.Halign = HorizontalAligment.Left;
            s.Margin = new Point(8, 2);
            s.BackGroundTexture = GetButtonColor();
            return s;
        }

        public static MenuPanelSettings DefaultComboBoxSettings()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.Font = GetDefaultFont();
            s.Size = new Point(128, 0);
            s.AdjustWidthToContent = false;
            s.AdjustHeightToContent = true;
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.TextMargin = new Point(16, 4);
            s.Valign = VerticalAligment.Top;
            s.Halign = HorizontalAligment.Left;
            s.Margin = new Point(8, 2);
            s.BackGroundTexture = GetButtonColor();
            return s;
        }

        public static MenuPanelSettings DefaultMultilineInput(int lines)
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.TextValign = VerticalAligment.Center;
            s.TextHalign = HorizontalAligment.Left;
            s.TextMargin = new Point(10, 0);

            s.Margin = new Point(10, 0);
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            s.BorderWidth = 1;
            s.Size = new Point(250, lines * s.Font.LineSpacing + 10);
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.LightGray);
            return s;
        }

    }
}
