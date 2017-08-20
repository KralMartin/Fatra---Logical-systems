using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.ContextMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities_Mono;
using Utilties_Mono;

namespace CP_v1
{
    class Engine
    {
        internal Settings Settings { get; private set; }
        public TextureLoader TextureLoader { get; private set; }
        public UserInput UserInput { get; private set; }
        public Game1 game { get; private set; }
        internal Screen CurrentScreen { get; set; }

        public Engine(Game1 game)
        {
            this.game = game;
            this.Settings = new Settings();

            this.TextureLoader = new TextureLoader(game.Content, game.GraphicsDevice);
            this.UserInput = new UserInput();
            Controler screenControler = new Controler();
            screenControler.DebugValue = "General screen";

            Controler menuLayer = new Controler();
            menuLayer.DebugValue = "menu layer";
            ContextMenuClass contextMenuLayer = new ContextMenuClass(new Rectangle(new Point(), this.game.Get_ScreenSize()), menuLayer);
            ImportantClassesCollection.Init(this.TextureLoader, menuLayer, contextMenuLayer, screenControler, this.game.Get_ScreenSize());
            

            this.UserInput.FirstControler.Push(screenControler);
            this.UserInput.FirstControler.Push(ImportantClassesCollection.MenuLayer);

            CurrentScreen = new GameSelectScreen(this);
        }

        public void Update(GameTime gameTime)
        {
            ImportantClassesCollection.ContextMenu.Update(gameTime);
            UserInput.Update(gameTime);
            CurrentScreen.Update(gameTime);
        }

        public void Draw(SpriteBatch sb)
        {
            CurrentScreen.Draw(sb);
            ImportantClassesCollection.MenuLayer.ControlerDraw(sb);
        }
        
        internal void Close()
        {
            ImportantClassesCollection.MenuLayer.Pop();
            this.CurrentScreen.Close();
        }

        internal void Resize()
        {
            this.CurrentScreen.Resize();
        }
    }
}
