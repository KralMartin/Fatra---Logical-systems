using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ContextMenu_Mono.Advanced
{
    public struct FormSettings
    {
        /// <summary>
        /// Color of form background.
        /// </summary>
        public Texture2D FormBackgroundTexture { get; set; }

        /// <summary>
        /// Color of form borders and button borders...
        /// </summary>
        public Texture2D FormBorderTexture { get; set; }

        /// <summary>
        /// Color of blocking panel.
        /// </summary>
        public Texture2D BackgroundTexture { get; set; }

        /// <summary>
        /// Color of X button in top right corner.
        /// </summary>
        public Texture2D ExitButtonTexture { get; set; }

        /// <summary>
        /// Color of OK and Cancel buttons
        /// </summary>
        public Texture2D OKCancelButtonsTexture { get; set; }

        public SpriteFont Font { get; set; }

        public Point ScreenSize { get; set; }
        public string TitleBarText { get; set; }
        public string ButtonOKText { get; set; }
        public string ButtonCancelText { get; set; }
    }
}
