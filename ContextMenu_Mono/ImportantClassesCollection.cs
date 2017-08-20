using System;
using ContextMenu_Mono.ContextMenu;
using Microsoft.Xna.Framework;
using Utilities_Mono;
using Utilties_Mono;

namespace ContextMenu_Mono
{
    public class ImportantClassesCollection
    {
        public static TextureLoader TextureLoader { get; private set; }
        public static Controler MenuLayer { get; private set; }
        public static Controler MainControler { get; private set; }
        public static ContextMenuClass ContextMenu { get; private set; }
        public static Point ScreenSize { get; set; }

        private static int blockingPanelCount;

        internal static bool AddBlockingPanel()
        {
            blockingPanelCount++;
            return (blockingPanelCount <= 1);
        }

        internal static void RemoveBlockingPanel()
        {
            blockingPanelCount--;
        }

        public static void Init(TextureLoader loader, Controler menuLayer, ContextMenuClass contextMenu, Controler mainControler, Point screenSize)
        {
            TextureLoader = loader;
            MenuLayer = menuLayer;
            ContextMenu = contextMenu;
            MainControler = mainControler;
            ScreenSize = screenSize;
            blockingPanelCount = 0;
        }
    }
}
