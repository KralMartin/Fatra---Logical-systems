using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilties_Mono;

namespace ContextMenu_Mono.Menu
{
    public struct MenuPanelSettings
    {
        /// <summary>
        /// Horizontal aligment of text within MenuPanel.
        /// </summary>
        public HorizontalAligment TextHalign { get; set; }

        /// <summary>
        /// Vertical aligment of text within MenuPanel.
        /// </summary>
        public VerticalAligment TextValign { get; set; }

        /// <summary>
        /// Text margin within MenuPanel according to TextHalign and TextValigh settings.
        /// (Example: Halign = Left, Valign = Top => Margin.X = margin to left side of parent, Margin.Y = margin to top side of parent.
        /// </summary>
        public Point TextMargin { get; set; }
        
        /// <summary>
        /// Font in which Text will be shown.
        /// Can be NULL if no text is displayed.
        /// </summary>
        public SpriteFont Font { get; set; }

        /// <summary>
        /// When NULL ignore mouse-over and pressed effects.
        /// </summary>
        public Texture2D BackGroundTexture { get; set; }

        /// <summary>
        /// Size of MenuPanel. 
        /// Represents minimal size of MenuPanel when AdjustWidth/HeighToContent is set to TRUE.
        /// </summary>
        public Point Size { get; set; }

        /// <summary>
        /// When TRUE, MenuPanel's width will be calculated based on content. 
        /// When TRUE, MenuPanel's Size.X represents minimal width.
        /// </summary>
        public bool AdjustWidthToContent { get; set; }

        /// <summary>
        /// When TRUE, MenuPanel's height will be calculated based on content. 
        /// When TRUE, MenuPanel's Size.X represents minimal height.
        /// </summary>
        public bool AdjustHeightToContent { get; set; }


        /// <summary>
        /// Horizontal aligment within parental MenuPanel.
        /// </summary>
        public HorizontalAligment Halign { get; set; }

        /// <summary>
        /// Vertical aligment within parental MenuPanel.
        /// </summary>
        public VerticalAligment Valign { get; set; }

        /// <summary>
        /// Margin within parental MenuPanel according to Halign and Valigh settings.
        /// (Example: Halign = Left, Valign = Top => Margin.X = margin to left side of parent, Margin.Y = margin to top side of parent.
        /// </summary>
        public Point Margin { get; set; }

        public ChildrenLayouts ChildrenLayout { get; set; }

        /// <summary>
        /// Mouse-over and pressed effects wont be displayed.
        /// </summary>
        public bool IgnoreEffects { get; set; }

        /// <summary>
        /// Allow to move children.
        /// To move with specific child MenuPanel, that child MenuPanel has to have 
        /// Mover prophery set to any MenuPanel of its children.
        /// </summary>
        public bool AllowChildrenMovement { get; set; }
        
        /// <summary>
        /// Border color.
        /// </summary>
        public Texture2D BorderColor { get; set; }

        /// <summary>
        /// Border width. Border is inner (I is displayed within bounds of MenuPanel).
        /// </summary>
        public int BorderWidth { get; set; }
        
    }
}
