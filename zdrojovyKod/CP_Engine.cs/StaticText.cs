using ContextMenu_Mono;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilties_Mono;

namespace CP_Engine
{
    class StaticText
    {
        public static int Number { get; private set; }
        static MenuPanel panel;

        public static void Set_Number(int value)
        {
            Number = value;
            panel.Text = Number.ToString();
            panel.Changed();
        }

        public static void Inc_Number()
        {
            Number++;
            panel.Text = Number.ToString();
            panel.Changed();
        }

        public static void Reset_Number()
        {
            Number = 0;
            panel.Text = Number.ToString();
            panel.Changed();
        }

        internal static void Create()
        {
            MenuPanelSettings s = new MenuPanelSettings();
            s.Margin = new Point(800, 50);
            s.Size = new Point(100, 50);
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            panel = new MenuPanel(s);
            Reset_Number();
        }

        internal static void Draw(SpriteBatch sb)
        {
            return;
            panel.ControlerDraw(sb);
        }
    }
}
