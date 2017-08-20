using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ContextMenu_Mono.ContextMenu
{
    public class ContextCheckButton : ContextButton
    {
        public delegate void CheckEventHandler(ContextCheckButton sender);
        public event CheckEventHandler CheckedChanged;

        public bool Checked { get; private set; }
        bool toogleChecked;
        Vector2 glyphOffset;

        public ContextCheckButton(string text, bool toogleChecked = false) : base(text)
        {
            this.toogleChecked = toogleChecked;
        }
        
        public void Set_Checked(bool value, bool fireEvent)
        {
            if (Checked != value)
            {
                Checked = value;
                if (fireEvent)
                {
                    if (CheckedChanged != null)
                        CheckedChanged(this);
                }
            }
        }
        
        internal override void MouseClick()
        {
            if (toogleChecked)
                Set_Checked(!Checked, true);
            else
                Set_Checked(true, true);
            base.MouseClick();
        }

        internal override int GetButtonWidth(SpriteFont font)
        {
            int toReturn = base.GetButtonWidth(font);
            return toReturn + Default.ContextMenu_ButtonGlyph;
        }

        internal override void SetPosition(Point position)
        {
            base.SetPosition(position);
            glyphOffset = this.textOffset;
            textOffset.X += Default.ContextMenu_ButtonGlyph;
        }

        internal override void Draw(SpriteBatch sb, SpriteFont font, Texture2D texture)
        {
            if (HasMouseOverEffect)
                sb.Draw(texture, bounds, Color.LightBlue);
            else
                sb.Draw(texture, bounds, Color.White);
            if (this.Checked)
                sb.DrawString(font, "(x)", glyphOffset, Color.Black);
            else
                sb.DrawString(font, "( )", glyphOffset, Color.Black);

            sb.DrawString(font, this.Text, textOffset, Color.Black);
        }
    }
}
