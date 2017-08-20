using ContextMenu_Mono;
using ContextMenu_Mono.ContextMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilties_Mono;

namespace CP_v1
{
    class Screen: Controler
    {

        protected Engine engine;

        internal Screen(Engine engine)
        {
            this.engine = engine;
        }

        internal virtual void Update(GameTime gameTime) { }

        internal virtual void Draw(SpriteBatch sb) { }

        internal virtual void Close() { }

        internal virtual void Resize() { }
    }
}
