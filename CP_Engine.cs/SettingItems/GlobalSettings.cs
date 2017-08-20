using ContextMenu_Mono;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CP_Engine
{
    public class GlobalSettings
    {
        public static bool ShowIOputs { get; set; }
        public static NumberFormats DefaultNumberFormat { get; set; }
        public static bool DarkMode { get; set; }

        internal static Texture2D SelectedTextureValid { get; set; }
        internal static Texture2D SelectedTextureInValid { get; set; }
        internal static NamedColor[] TileColors { get; set; }
        internal static NamedColor[] BugColors { get; set; }
        
        internal static void Init(WorkPlace workplace)
        {
            SelectedTextureValid = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(0, 0, 255, 0.5f));
            SelectedTextureInValid = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(255, 0, 0, 0.5f));
            DarkMode = false;
            if (DarkMode)
            {
                //Tile colors.
                TileColors = new NamedColor[4];
                TileColors[0] = new NamedColor(0, "(none)", null);
                TileColors[1] = new NamedColor(1, "Red", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(166, 60, 60)));
                TileColors[2] = new NamedColor(2, "Green", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(60, 166, 60)));
                TileColors[3] = new NamedColor(3, "Blue", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(60, 60, 166)));

                //Bug colors.
                BugColors = new NamedColor[5];
                BugColors[0] = new NamedColor(0, "Red", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(166, 30, 30)));
                BugColors[1] = new NamedColor(1, "Green", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(30, 166, 30)));
                BugColors[2] = new NamedColor(2, "Blue", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(30, 30, 166)));
                BugColors[3] = new NamedColor(3, "Purple", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(102, 51, 153)));
                BugColors[4] = new NamedColor(4, "Brown", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(new Color(176, 140, 100)));
            }
            else
            {
                //Tile colors.
                TileColors = new NamedColor[4];
                TileColors[0] = new NamedColor(0, "(none)", null);
                TileColors[1] = new NamedColor(1, "Red", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.PaleVioletRed));
                TileColors[2] = new NamedColor(2, "Green", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.PaleGreen));
                TileColors[3] = new NamedColor(3, "Blue", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.LightBlue));

                //Bug colors.
                BugColors = new NamedColor[5];
                BugColors[0] = new NamedColor(0, "Red", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.MediumVioletRed));
                BugColors[1] = new NamedColor(1, "Green", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.MediumSeaGreen));
                BugColors[2] = new NamedColor(2, "Blue", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.LightBlue));
                BugColors[3] = new NamedColor(3, "Purple", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Magenta));
                BugColors[4] = new NamedColor(4, "Brown", ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.SandyBrown));
            }
            DefaultNumberFormat = NumberFormats.Binary;
            ShowIOputs = true;
        }


        public static Color TabManager_TabColor()
        {
            return Color.Blue;
        }

        public static Color TabManager_RemoveBtnColor()
        {
            return Color.Blue;
        }

        public static Color LightGray()
        {
            if (DarkMode)
                return new Color(91, 91, 91);
            return Color.Gray;
        }

        public static Color Gray()
        {
            if (DarkMode)
                return new Color(61, 61, 61);
            return Color.LightSkyBlue;
        }

        public static Color DarkGray()
        {
            if (DarkMode)
                return new Color(31, 31, 31);
            return Color.White;
        }

        public static string GetActionText(Actions action)
        {
            switch (action)
            {
                case Actions.BreakPoint:
                    return "Insert/Delete break-points";
                case Actions.DrawingLines:
                    return "Draw lines";
                case Actions.Inserting_Bug:
                    return "Insert bug";
                case Actions.Inserting_Diode:
                    return "Insert diode";
                case Actions.Inserting_Input:
                    return "Insert input";
                case Actions.Inserting_Konveror:
                    return "Insert konvertor";
                case Actions.Inserting_Output:
                    return "Insert output";
                case Actions.Removing:
                    return "Remove items";
                case Actions.Selecting:
                    return "Select items";
                default:
                    return "ERROR";
            }
        }
        public static string GetActionShortcut(Actions action)
        {
            switch (action)
            {
                case Actions.BreakPoint:
                    return " [B]";
                case Actions.DrawingLines:
                    return " [D]";
                case Actions.Inserting_Bug:
                    return "";
                case Actions.Inserting_Diode:
                    return " [X]";
                case Actions.Inserting_Input:
                    return " [I]";
                case Actions.Inserting_Konveror:
                    return " [C]";
                case Actions.Inserting_Output:
                    return " [O]";
                case Actions.Removing:
                    return " [R]";
                case Actions.Selecting:
                    return " [S]";
                default:
                    return "ERROR";
            }
        }
    }
}
