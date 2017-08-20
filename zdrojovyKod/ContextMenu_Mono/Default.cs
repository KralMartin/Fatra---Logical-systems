using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilties_Mono;

namespace ContextMenu_Mono
{
    public enum ChildrenLayouts { Normal, HorizontalStack, VerticalStack }

    class Default
    {
        public static int MenuPanelMargin = 2;
        public static int ContextMenuDelay = 500;
        public static int CursorIndicatorDelay = 500;
        public static HorizontalAligment TextHaligment = HorizontalAligment.Center;

        public static int ContextMenu_ButtonGlyph = 16;
        public static int ContextMenu_ButtonMargin = 2;
        public static int ContextMenu_ButtonTextMargin = 2;
        
    }
}
