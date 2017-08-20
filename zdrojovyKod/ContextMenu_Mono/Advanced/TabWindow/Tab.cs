using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ContextMenu_Mono.TabWindow
{
    public class Tab
    {
        public Rectangle TabBounds { get; private set; }

        internal CheckMenuPanel Button { get; set; }

        internal void Set_Bounds(Rectangle value)
        {
            this.TabBounds = value;
            Resize();
        }

        protected virtual void Resize() { }

        public virtual void Draw(SpriteBatch sb) { }

        public virtual void TabVisibilityChanged(bool visible) { }
    }
}
