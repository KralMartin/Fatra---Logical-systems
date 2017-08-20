using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Utilities_Mono
{
    /// <summary>
    /// Helps to retieve Textures and Fonts.
    /// </summary>
    public class TextureLoader
    {
        public static TextureLoader Instance {get; private set;}

        Dictionary<string, Texture2D> textures;
        Dictionary<Color, Texture2D> simpleTextures;
        Dictionary<string, SpriteFont> fonts;
        public ContentManager contentManager;
        GraphicsDevice graphicsDevice;

        public TextureLoader(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            this.contentManager = contentManager;
            this.graphicsDevice = graphicsDevice;
            textures = new Dictionary<string, Texture2D>();
            simpleTextures = new Dictionary<Color, Texture2D>();
            fonts = new Dictionary<string, SpriteFont>();
            Instance = this;
        }

        /// <summary>
        /// Returns 1x1 texture of certain color.
        /// </summary>
        /// <param name="textureColor">Color of texture.</param>
        /// <returns>Texture of certain color.</returns>
        public Texture2D CreateSimpleTexture(Color textureColor)
        {
            if (simpleTextures.ContainsKey(textureColor) == false)
            {
                Texture2D newTexture = new Texture2D(graphicsDevice, 1, 1);
                newTexture.SetData<Color>(new Color[] { textureColor });
                simpleTextures.Add(textureColor, newTexture);
            }
            return simpleTextures[textureColor];
        }

        /// <summary>
        /// Loads texture from ContentManager and stores it for faster acces to it.
        /// </summary>
        /// <param name="name">Name of texture.</param>
        /// <returns></returns>
        public Texture2D GetTexture(string name)
        {
            if (textures.ContainsKey(name) == false)
                textures.Add(name, contentManager.Load<Texture2D>(name));
            return textures[name];
        }

        public Texture2D ChangeColor(Texture2D texture, Color oldColor, Color newColor)
        {
            Texture2D toReturn = new Texture2D(graphicsDevice, texture.Width, texture.Height);
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == oldColor)
                    data[i] = newColor;
            }
            toReturn.SetData(data);
            return toReturn;
        }

        /// <summary>
        /// Loads font from ContentManager and stores it for faster acces to it.
        /// </summary>
        /// <param name="name">Name of font.</param>
        /// <returns></returns>
        public SpriteFont GetFont(string name)
        {
            if (fonts.ContainsKey(name) == false)
                fonts.Add(name, contentManager.Load<SpriteFont>(name));
            return fonts[name];
        }
    }
}
