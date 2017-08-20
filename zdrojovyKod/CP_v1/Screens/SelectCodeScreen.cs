using ContextMenu_Mono;
using CP_Engine;

namespace CP_v1
{
    class SelectCodeScreen : Screen
    {
        HalfScreen leftScreen;
        HalfScreen rightScreen;
        GameScreen gameScreen;
        internal WorkPlace Workplace { get; private set; }

        public SelectCodeScreen(Engine engine, GameScreen gameScreen) : base(engine)
        {
            this.gameScreen = gameScreen;
            this.Workplace = this.gameScreen.GetWorkplace();
            rightScreen = new CodeSelectHalfScreen(this, null);
            leftScreen = new FunctionSelectHalfScreen(this, null);
            ImportantClassesCollection.MenuLayer.Push(rightScreen.GetPanel());
            ImportantClassesCollection.MenuLayer.Push(leftScreen.GetPanel());            
        }

        internal override void Resize()
        {
            leftScreen.Resize();
            rightScreen.Resize();
        }

        internal override void Close()
        {
            ClearMenuPanels();
            this.gameScreen.Close();
        }

        /// <summary>
        /// Removes all menupanels from application.
        /// </summary>
        private void ClearMenuPanels()
        {
            leftScreen.GetPanel().Pop();
            rightScreen.GetPanel().Pop();
        }

        internal void GoToGameScreen()
        {
            this.ClearMenuPanels();
            this.engine.CurrentScreen = this.gameScreen;
        }
        
        internal Engine GetEngine()
        {
            return this.engine;
        }

        internal void ChangeHalfScreenLeft(HalfScreen halfScreen)
        {
            leftScreen.GetPanel().Pop();
            this.leftScreen = halfScreen;
            ImportantClassesCollection.MenuLayer.Push(leftScreen.GetPanel());
        }

        internal void ChangeHalfScreenRight(HalfScreen halfScreen)
        {
            rightScreen.GetPanel().Pop();
            this.rightScreen = halfScreen;
            ImportantClassesCollection.MenuLayer.Push(rightScreen.GetPanel());
        }

    }
}
