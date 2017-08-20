using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities_Mono;
using Utilties_Mono;

namespace ContextMenu_Mono.Menu
{
    public class TextInputMenuPanel : MenuPanel
    {
        protected TextInputEditor editor;
        bool allowEnter;

        public TextInputMenuPanel(MenuPanelSettings settings, bool allowEnter = false) : base(settings)
        {
            this.allowEnter = allowEnter;
            DrawnText.ForeColor = Color.Black;
        }

        internal override void ExecuteAction()
        {
            if (Settings.IgnoreEffects == false)
            {
                this.DrawnText.HasCursor = true;
                editor = new TextInputEditor(this, Settings.Font, allowEnter);
            }
        }

        internal Vector2 Get_RelativeTextOffset()
        {
            return this.relativeTextOffset;
        }

        internal void SetEditorNull()
        {
            editor = null;
            this.DrawnText.HasCursor = false;
        }
        
    }
}
