using ContextMenu_Mono;
using ContextMenu_Mono.TabWindow;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CP_Engine.WorkplaceAssistants;
using CP_Engine.MapItems;

namespace CP_Engine
{
    public enum Actions { Selecting, DrawingLines, Removing, Inserting_Konveror, Inserting_Diode, Inserting_Bug, Inserting_Input, Inserting_Output, BreakPoint }

    public class WorkPlace
    {
        public SchemeEventHistory SchemeEventHistory { get; private set; }
        public Actions CurrentAction { get; private set; }
        public Project Project { get; private set; }
        public Simulation Simulation { get; private set; }
        public Window CurrentWindow { get; private set; }
        public StatusText StatusText { get; private set; }

        public CoppyAssistant CoppyAssistant { get; private set; }
        internal DrawingAssistant DrawingAssistant { get; private set; }
        internal RemovingAssistant RemovingAssistant { get; private set; }
        internal WidthAssistant WidthAssistant { get; private set; }
        internal InsertAssistant InsertAssistant { get; private set; }
        internal OffsetAssistant OffsetAssistant { get; private set; }
        internal BugInsertAssistant BugInsertAssistant { get; private set; }
        internal BugAssistant BugAssistant { get; private set; }
        internal BreakPointAssistant BreakPointAssistant { get; private set; }

        internal PopupMenuGenerator PopupMenuGenerator { get; private set; }
        internal GlobalSettings GlobalSettings { get; private set; }
        
        private TabManager tabManager;
        internal MyControler controler;
        private Rectangle bounds;
        private int statusTextHeight;
        
        public WorkPlace(Rectangle bounds)
        {
            this.bounds = bounds;
            StaticText.Create();
            statusTextHeight = 32;

            //Create tab manager.
            Rectangle tabManagerBounds = bounds;
            tabManagerBounds.Height -= statusTextHeight;
            tabManager = new TabManager(tabManagerBounds, 16, 4,
                ImportantClassesCollection.TextureLoader.GetFont("f1"), ImportantClassesCollection.MainControler,
                ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.DarkGray()),
                ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.Gray()),
                ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Red),
                ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Blue));
            tabManager.TabChanged += TabManager_TabChanged;

            //Create bounds of status text.
            Rectangle statusTextBounds = bounds;
            statusTextBounds.Y += statusTextBounds.Height - statusTextHeight;
            statusTextBounds.Height = statusTextHeight;
            
            //Initialize other variabiles.
            TilesInfo.Initialize();
            GlobalSettings.Init(this);
            this.StatusText = new StatusText(this, statusTextBounds);
            this.SchemeEventHistory = new SchemeEventHistory(this);
            this.Simulation = new Simulation(this);
            this.Project = new Project(this);


            this.InsertAssistant = new InsertAssistant(this);
            this.DrawingAssistant = new DrawingAssistant(this);
            this.WidthAssistant = new WidthAssistant(this);
            this.OffsetAssistant = new OffsetAssistant(this);
            this.RemovingAssistant = new RemovingAssistant(this);
            this.BugInsertAssistant = new BugInsertAssistant(this);
            this.BugAssistant = new BugAssistant(this);
            this.CoppyAssistant = new CoppyAssistant(this);
            this.BreakPointAssistant = new BreakPointAssistant(this);

            //Register controler, that allows user to control this whole dll.
            controler = new MyControler(this, tabManager.TabContentBounds, ImportantClassesCollection.ContextMenu);
            ImportantClassesCollection.MainControler.Push(controler);
            
        }
        
        public void Update(GameTime gameTime)
        {
            if (this.Simulation.BreakPointResults.Count > 0)
            {
                BreakPointForm form = new BreakPointForm(this);
                //OpenWindow(this.Simulation.BreakPointResults[0].BreakOccourances[0]);
                this.Simulation.BreakPointResults.Clear();
            }
        }
        
        public void Resize(Rectangle bounds)
        {
            this.bounds = bounds;
            Rectangle tabManagerBounds = bounds;
            tabManagerBounds.Height -= statusTextHeight;
            tabManager.Resize(tabManagerBounds);
            controler.Resize(tabManager.TabContentBounds);

            //Create bounds of status text.
            Rectangle statusTextBounds = bounds;
            statusTextBounds.Y += statusTextBounds.Height - statusTextHeight;
            statusTextBounds.Height = statusTextHeight;
            this.StatusText.Resize(statusTextBounds);
        }

        /// <summary>
        /// Closes windoes that show PhysSchemes that are no longer in use.
        /// </summary>
        internal void CloseWindows()
        {
            Tab[] tabs = this.tabManager.GetTabs();
            foreach (Window window in tabs)
            {
                if (this.Project.SchemeStructure.Get_PhysSchemes(window.Scheme).Contains(window.PhysScheme) == false)
                    tabManager.RemoveTab(window);
            }
        }

        /// <summary>
        /// Opens first window, that shows provided scheme.
        /// If CurrentWindow already shows provided scheme, do nothing.
        /// If there is no such a window, craete one.
        /// </summary>
        /// <param name="scheme"></param>
        internal void OpenWindow(Scheme scheme)
        {
            if (this.CurrentWindow!=null && object.ReferenceEquals(this.CurrentWindow.Scheme, scheme))
                return;
            Tab[] tabs = this.tabManager.GetTabs();
            foreach (Window window in tabs)
            {
                if (object.ReferenceEquals(window.Scheme, scheme))
                {
                    tabManager.SelectTab(window, true);
                    return;
                }
            }
            if(this.Project.SchemeStructure.Get_PhysSchemes(scheme).Count>0)
                OpenWindow(this.Project.SchemeStructure.Get_PhysSchemes(scheme)[0]);
        }

        /// <summary>
        /// Safely closes this component.
        /// </summary>
        public void Close()
        {
            this.Simulation.Stop();
            tabManager.Pop();
            controler.Pop();
        }
        
        /// <summary>
        /// Sets action. Affects what happens if user is clicking/moving over this component.
        /// </summary>
        /// <param name="value"></param>
        public void Set_CurrentAction(Actions value)
        {
            this.CurrentAction = value;
            this.CurrentWindow.Selection.Items.Clear();
            this.CurrentWindow.Selection.IsValid = true;
            this.StatusText.SetTextCenter(GlobalSettings.GetActionText(CurrentAction));
        }

        /// <summary>
        /// Creates new top bug.
        /// </summary>
        public void CreateNewTopBug(bool createNewTopScheme)
        {
            NewSchemeForm form = new NewSchemeForm(this, null, createNewTopScheme);
        }

        /// <summary>
        /// Changes current action to Actions.Inserting_Bug and sets what Bug will be user inserting to scheme.
        /// </summary>
        /// <param name="bug"></param>
        public void StartInserting(Bug bug)
        {
            this.BugInsertAssistant.StartInserting(bug);
            Set_CurrentAction(Actions.Inserting_Bug);
        }
        
        /// <summary>
        /// Open new window showing provided PhysScheme.
        /// </summary>
        /// <param name="physScheme"></param>
        internal void OpenWindow(PhysScheme physScheme)
        {
            Tab[] tabs = this.tabManager.GetTabs();
            foreach (Window tab in tabs)
            {
                if (object.ReferenceEquals(tab.PhysScheme, physScheme))
                {
                    tabManager.SelectTab(tab, true);
                    return;
                }
            }
            Window window = new Window(this, tabManager.TabContentBounds, physScheme);
            tabManager.Add(physScheme.PlacedBug.Bug.Title, window);
            tabManager.SelectTab(window, true);
        }

        /// <summary>
        /// Prevent hiding all tabs.
        /// When all tabs are closed, open window showing TopScheme.
        /// </summary>
        /// <param name="newTab"></param>
        private void TabManager_TabChanged(Tab newTab)
        {
            CurrentWindow = (Window)newTab;
            if (this.CurrentWindow == null)
            {
                this.OpenWindow(this.Project.TopPScheme);
            }
            this.StatusText.SetTextLeft(this.CurrentWindow.PhysScheme.GetPath());
            
        }

        /// <summary>
        /// Draw this component.
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            tabManager.Draw(sb);
            this.StatusText.Draw(sb);
            StaticText.Draw(sb);
        }
    }
}
