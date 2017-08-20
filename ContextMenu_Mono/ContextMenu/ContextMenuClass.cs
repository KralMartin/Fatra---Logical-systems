using System;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities_Mono;
using Utilties_Mono;

namespace ContextMenu_Mono.ContextMenu
{
    public class ContextMenuClass
    {
        private ContextControler contextControler;

        /// <summary>
        ///
        /// </summary>
        /// <param name="allowedRectangle">Area where context menu can be shown.</param>
        /// <param name="userInput">User Input.</param>
        public ContextMenuClass(Rectangle allowedRectangle, Controler parentControler)
        {
            this.contextControler = new ContextControler(allowedRectangle, parentControler);
            this.contextControler.DebugValue = "context menu";
        }

        /// <summary>
        /// Show context menu at provided position.
        /// </summary>
        /// <param name="position">Top left corner of panel.</param>
        /// <param name="panel">Panel to be shown.</param>
        public void Show(Point position, ContextPanel panel)
        {
            contextControler.Show(position, panel);
        }

        public void Resize(Rectangle allowedRectangle)
        {
            this.contextControler.AllowedArea = allowedRectangle;
        }

        public void Update(GameTime gameTime)
        {
            if (contextControler.FirstPanel != null)
            {
                contextControler.FirstPanel.Update(gameTime);
            }
        }

        public void Show(MenuPanel sender, ContextPanel panel)
        {
            Rectangle senderBounds = sender.GetBounds();
            Show(new Point(senderBounds.X, senderBounds.Y + senderBounds.Height), panel);
        }
    }
}
