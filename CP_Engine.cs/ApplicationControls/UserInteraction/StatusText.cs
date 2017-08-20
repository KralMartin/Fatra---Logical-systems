using ContextMenu_Mono;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Utilties_Mono;

namespace CP_Engine
{
    public class StatusText
    {
        MenuPanel panel, left, right, center;
        Point toolTipOffset;
        DrawnText toolTip;

        public StatusText(WorkPlace workplace, Rectangle bounds)
        {
            MenuPanelSettings settings = new MenuPanelSettings();
            settings.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            settings.TextMargin = new Point(32, 0);
            settings.Size = new Point(bounds.Width, bounds.Height);
            settings.Margin = new Point(bounds.X, bounds.Y);
            settings.Halign = HorizontalAligment.Left;
            settings.Valign = VerticalAligment.Top;
            settings.ChildrenLayout = ChildrenLayouts.HorizontalStack;
            settings.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.Gray());
            settings.BorderWidth = 2;
            settings.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.DarkGray());

            panel = new MenuPanel(settings);
            
            settings.TextHalign = HorizontalAligment.Left;
            settings.TextValign = VerticalAligment.Center;
            settings.Margin = new Point();
            settings.Size = new Point(bounds.Width / 2, bounds.Height);
            left = new MenuPanel(settings);
            left.Text = "lavy";
            panel.Children.Add(left);

            settings.Size = new Point(bounds.Width / 4, bounds.Height);
            settings.TextHalign = HorizontalAligment.Center;
            center = new MenuPanel(settings);
            center.Text = "center";
            panel.Children.Add(center);

            settings.TextHalign = HorizontalAligment.Right;
            right = new MenuPanel(settings);
            right.Text = "pravy";
            panel.Children.Add(right);

            //Create tooltip
            toolTip = new DrawnText(ImportantClassesCollection.TextureLoader.GetFont("f1"), "QWE\n ASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXCASD ZXC", HorizontalAligment.Left, VerticalAligment.Bottom);
            toolTip.Background = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Teal);
            toolTip.TextMargin = new Point(3, 3);
            
            Resize(bounds);
        }

        internal void Resize(Rectangle bounds)
        {
            MenuPanelSettings s = panel.Settings;
            s.Margin = new Point(bounds.X, bounds.Y);
            s.Size = new Point(bounds.Width, bounds.Height);
            panel.Settings = s;

            int quarter = 200;
            ResizeInner(right, new Point(quarter, bounds.Height));
            ResizeInner(center, new Point(quarter, bounds.Height));
            ResizeInner(left, new Point(bounds.Width - quarter * 2, bounds.Height));
            panel.Changed();

            toolTip.MaxSize = new Point(bounds.Width, 80);
            toolTip.MinSize = new Point(bounds.Width, 0);
            toolTipOffset = new Point(0, bounds.Y);
            toolTip.Changed();
        }

        private void ResizeInner(MenuPanel toResize, Point size)
        {
            MenuPanelSettings s = toResize.Settings;
            s.Size = size;
            toResize.Settings = s;
        }

        public void SetTextLeft(string text)
        {
            left.Text = text;
            panel.Changed(new Rectangle());
        }
        
        public void SetTextCenter(string text)
        {
            center.Text = text;
            panel.Changed(new Rectangle());
        }

        public void SetTextRight(string text)
        {
            right.Text = text;
            panel.Changed(new Rectangle());
        }

        public void SetToolTipText(string text)
        {
            toolTip.Text = text;
            toolTip.Changed();
        }

        public void Draw(SpriteBatch sb)
        {
            panel.ControlerDraw(sb);
            if(toolTip.Text!="")
                toolTip.Draw(sb, toolTipOffset);
        }
    }
}
