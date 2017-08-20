using Microsoft.Xna.Framework.Graphics;

namespace CP_Engine
{
    class NamedColor
    {
        internal int ID { get; set; }
        internal string Text { get; set; }
        internal Texture2D Texture { get; set; }

        internal NamedColor(int id, string text, Texture2D texture)
        {
            this.ID = id;
            this.Text = text;
            this.Texture = texture;
        }
    }
}
