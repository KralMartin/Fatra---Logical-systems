using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities_Mono;
using Utilties_Mono;

namespace ContextMenu_Mono.ContextMenu
{
    class ContextControler : Controler
    {
        public ContextPanel FirstPanel { get; private set; }
        internal Rectangle AllowedArea { get; set; }
        Controler parentControler;

        internal ContextControler(Rectangle allowedArea, Controler parentControler)
        {
            this.AllowedArea = allowedArea;
            this.parentControler = parentControler;
        }

        internal void Show(Point position, ContextPanel panel)
        {
            if (FirstPanel != null)
                this.Pop();
            FirstPanel = panel;
            parentControler.Push(this);
            panel.Show(position, AllowedArea, 0);
        }

        public override bool MouseMove()
        {
            if (FirstPanel != null)
                return FirstPanel.MouseMove(UserInput.GameTime, UserInput.MouseState);
            return false;
        }

        public override bool MousePressLeft()
        {
            if (FirstPanel != null)
                return true;
            return false;
        }

        public override void MouseReleaseLeft(UserInput userInput)
        {
            int result = FirstPanel.MouseRelease(userInput.MouseState.Position);
            if (result != 2)
            {
                this.Pop();
                FirstPanel = null;
            }
        }

        public override void ControlerDraw(SpriteBatch sb)
        {
            if (FirstPanel != null)
            {
                FirstPanel.Draw(sb);
            }
        }
    }
}
