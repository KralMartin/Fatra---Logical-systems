using CP_Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities_Mono;
using Utilties_Mono;
using Microsoft.Xna.Framework.Input;
using ContextMenu_Mono.Menu;
using ContextMenu_Mono.ContextMenu;
using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu.Inputs.Scroll;

namespace CP_v1
{
    class GameScreen : Screen
    {
        WorkPlace workplace;
        MenuPanel topMenu;

        Texture2D enabledTexture, disabledTexture;

        MenuPanel undo, redo, sim_step, sim_start;

        public GameScreen(Engine engine) : base(engine)
        {
            enabledTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.Gray());
            disabledTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(GlobalSettings.LightGray());


            Rectangle workplaceBounds = new Rectangle(new Point(0, Default.TopMenu_Height), ImportantClassesCollection.ScreenSize);
            workplaceBounds.Height -= Default.TopMenu_Height;
            workplace = new WorkPlace(workplaceBounds);
            workplace.SchemeEventHistory.CollectionChanged += SchemeEventHistory_CollectionChanged;
            ImportantClassesCollection.MainControler.Push(this);
            CreateTopMenu();
        }

        internal override void Update(GameTime gameTime)
        {
            workplace.Update(gameTime);
        }

        internal override void Resize()
        {
            MenuPanelSettings s = topMenu.Settings;
            s.Size = new Point(ImportantClassesCollection.ScreenSize.X, Default.TopMenu_Height);
            topMenu.Settings = s;
            topMenu.Changed(new Rectangle());

            Rectangle workplaceBounds = new Rectangle(new Point(0, Default.TopMenu_Height), ImportantClassesCollection.ScreenSize);
            workplaceBounds.Height -= Default.TopMenu_Height;
            workplace.Resize(workplaceBounds);
        }

        internal override void Close()
        {
            this.Pop();
            workplace.Project.Save();
            workplace.Close();
            topMenu.Pop();            
        }

        private void SchemeEventHistory_CollectionChanged(bool canUndo, bool canRedo)
        {
            EnableButton(undo, canUndo);
            EnableButton(redo, canRedo);
        }

        private void EnableButton(MenuPanel panel, bool enable)
        {
            MenuPanelSettings s = panel.Settings;
            if (s.IgnoreEffects != enable)
                return;
            if (enable)
            {
                s.IgnoreEffects = false;
                s.BackGroundTexture = enabledTexture;
            }
            else
            {
                s.IgnoreEffects = true;
                s.BackGroundTexture = disabledTexture;
            }
            panel.Settings = s;
        }

        internal void New(string projectName, string fullProjectName)
        {
            workplace.Project.New(projectName, fullProjectName, "Content//defaultFile.xml");
            if (workplace.Simulation.IsRunning == false)
                sim_start.SimulateClick();
        }

        internal WorkPlace GetWorkplace()
        {
            return this.workplace;
        }

        private void CreateTopMenu()
        {
            //Top menu bar.
            MenuPanelSettings s = new MenuPanelSettings();
            s.Size = new Point(engine.game.Get_ScreenSize().X, Default.TopMenu_Height);
            s.BackGroundTexture = engine.TextureLoader.CreateSimpleTexture(GlobalSettings.Gray());
            s.ChildrenLayout = ChildrenLayouts.HorizontalStack;
            s.IgnoreEffects = true;
            topMenu = new MenuPanel(s);
            topMenu.DebugValue = "top menu";
            //Buttons.
            CreateActionsButton();
            CreateUndoRedo();
            CreateView();
            CreateSimulationButtons();
            CreateSchemeBugConvertor();
            CreateExpandBtn();
            CreateSchemeSettings();
            CreateCodeButn();
            CreateExitBtn();
            //CreateSimSpeed();

            workplace.Simulation.Clicked += Simulation_Changed;
            topMenu.Changed(new Rectangle());
            ImportantClassesCollection.MainControler.Push(topMenu);
            workplace.SchemeEventHistory.Undo();
        }

        private void CreateSimSpeed()
        {
        }

        private void CreateView()
        {
            MenuPanel button = new MenuPanel(GetTopMenuPanel_ButtonSettings());
            button.Text = "View";
            button.Hovered += Button_Hovered;
            button.ToolTipText = "Application settings.";
            button.Clicked += Button_Clicked2; 
            topMenu.Children.Add(button);
        }

        private void Button_Clicked2(MenuPanel sender)
        {
            ContextPanel panel = new ContextPanel(engine.TextureLoader.CreateSimpleTexture(Color.Gray), engine.TextureLoader.GetFont("f1"));

            ContextCheckButton check = new ContextCheckButton("Bug IOputs", GlobalSettings.ShowIOputs);
            check.Set_Checked(GlobalSettings.ShowIOputs, false);
            check.Clicked += Check_Clicked;
            panel.Buttons.Add(check);

            ContextLinkButton link = new ContextLinkButton(CreateNumberFormatsPanel(), "Format");
            panel.Buttons.Add(link);
            panel.Changed();
            
            Rectangle senderBounds = sender.GetBounds();
            ImportantClassesCollection.ContextMenu.Show(new Point(senderBounds.X, senderBounds.Y + senderBounds.Height), panel);
        }

        private void Check_Clicked(ContextButton sender)
        {
            GlobalSettings.ShowIOputs = !GlobalSettings.ShowIOputs;
        }

        private ContextPanel CreateNumberFormatsPanel()
        {
            ContextPanel panel = new ContextPanel(engine.TextureLoader.CreateSimpleTexture(Color.Gray), engine.TextureLoader.GetFont("f1"));
            foreach (NumberFormats current in Enum.GetValues(typeof(NumberFormats)))
            {
                if (current != NumberFormats.None)
                {
                    ContextCheckButton check = new ContextCheckButton(current.ToString());
                    check.Set_Checked(current == GlobalSettings.DefaultNumberFormat, false);
                    panel.Buttons.Add(check);
                    check.Clicked += Check_Clicked1;
                    check.Tag = current;
                }
            }
            panel.Changed();
            return panel;
        }

        private void Check_Clicked1(ContextButton sender)
        {
            NumberFormats format = (NumberFormats)sender.Tag;
            GlobalSettings.DefaultNumberFormat = format;
        }


        private void Simulation_Changed(Simulation sender)
        {
            UpdateSimButtons();
        }

        private void CreateSchemeSettings()
        {
            MenuPanel link = new MenuPanel(GetTopMenuPanel_ButtonSettings());
            link.Text = "Scheme settings";
            link.Hovered += Button_Hovered;
            link.ToolTipText = "Opens form with scheme settings.";
            link.Clicked += SchemeSettings_Clicked;
            this.topMenu.Children.Add(link);
        }

        private void SchemeSettings_Clicked(MenuPanel sender)
        {
            //Create context menu.
            ContextPanel panel = new ContextPanel(engine.TextureLoader.CreateSimpleTexture(Color.Gray), engine.TextureLoader.GetFont("f1"));
            foreach (Bug bug in workplace.Project.Bugs.GetItems())
            {
                if (bug.IsUserCreated())
                {
                    ContextButton btn = new ContextButton(bug.Title);
                    btn.Clicked += SchemeSettings_Clicked;
                    btn.Tag = bug;
                    panel.Buttons.Add(btn);
                }
            }
            panel.Changed();

            Rectangle senderBounds = sender.GetBounds();
            ImportantClassesCollection.ContextMenu.Show(new Point(senderBounds.X, senderBounds.Y + senderBounds.Height), panel);
        }

        private void SchemeSettings_Clicked(ContextButton sender)
        {
            NewSchemeForm form = new NewSchemeForm(this.workplace, (Bug)sender.Tag);
        }
        
        private void CreateExpandBtn()
        {
            ContextPanel panel = new ContextPanel(engine.TextureLoader.CreateSimpleTexture(Color.Gray), engine.TextureLoader.GetFont("f1"));
            ContextButton btn;
            string[] texts = new string[] { "Top", "Right", "Bot", "Left" };
            for (int i = 0; i < 4; i++)
            {
                btn = new ContextButton(texts[i]);
                panel.Buttons.Add(btn);
                btn.Clicked += Btn_Clicked1;
                btn.Hovered += Button_Hovered1;
                btn.ToolTipText = "Adds tiles to " + texts[i] + " side of scheme.";
                btn.Tag = i;
            }
            //Cut of scheme.
            btn = new ContextButton("Cut off scheme");
            panel.Buttons.Add(btn);
            btn.Clicked += Btn_Clicked1;
            btn.Hovered += Button_Hovered1;
            btn.ToolTipText = "Makes scheme grid as small as possible.";
            btn.Tag = -1;
            
            panel.Changed();

            MenuPanel button = new MenuPanel(GetTopMenuPanel_ButtonSettings());
            button.Text = "Expand Scheme";
            button.Hovered += Button_Hovered;
            button.ToolTipText = "Enlarges scheme grid in selected direction. You cant undo/redo this action.";
            button.Tag = panel;
            button.Clicked += OpenContextMenu;
            topMenu.Children.Add(button);
        }

        private void OpenContextMenu(MenuPanel sender)
        {
            ContextPanel panel = (ContextPanel)sender.Tag;
            Rectangle senderBounds = sender.GetBounds();
            ImportantClassesCollection.ContextMenu.Show(new Point(senderBounds.X, senderBounds.Y + senderBounds.Height), panel);
        }

        private void Btn_Clicked1(ContextButton sender)
        {
            int side = (int)sender.Tag;
            if (side < 0)
                workplace.CurrentWindow.CutOffScheme();
            else
            {
                ExpandForm form = new ExpandForm(workplace, side);
            }
        }

        private void Form_Closed(Form sender, bool result)
        {
            NumericInputMenuPanel input = (NumericInputMenuPanel)sender.Get_ContentMenuPanlel();
            object o = input.GetValue();
        }

        private void CreateExitBtn()
        {
            MenuPanel btn = new MenuPanel(GetTopMenuPanel_ButtonSettings());
            btn.Hovered += Button_Hovered;
            btn.Text = "Main menu";
            btn.Hovered += Button_Hovered;
            btn.ToolTipText = "Saves scheme and returns to main menu.";
            btn.Clicked += MainMenu_Clicked1;
            topMenu.Children.Add(btn);
        }
        
        private void CreateCodeButn()
        {
            MenuPanel btn = new MenuPanel(GetTopMenuPanel_ButtonSettings(true));
            btn.Hovered += Button_Hovered;
            btn.Text = "Code modify";
            btn.Hovered += Button_Hovered;
            btn.ToolTipText = "Change code.";
            btn.Clicked += Code_Clicked;
            topMenu.Children.Add(btn);
        }

        private void Code_Clicked(MenuPanel sender)
        {
            this.engine.CurrentScreen = new SelectCodeScreen(this.engine, this);
        }

        private void MainMenu_Clicked1(MenuPanel sender)
        {
            Close();
            GameSelectScreen screen = new GameSelectScreen(engine);
            engine.CurrentScreen = screen;
        }

        internal void Load(string fullFileName)
        {
            workplace.Project.Load(fullFileName);
            if (workplace.Simulation.IsRunning == false)
                sim_start.SimulateClick();
        }

        private void CreateSchemeBugConvertor()
        {
            ContextPanel panel = new ContextPanel(engine.TextureLoader.CreateSimpleTexture(Color.Gray), engine.TextureLoader.GetFont("f1"));
            ContextButton button = new ContextButton("Create new main scheme");
            panel.Buttons.Add(button);
            button.Hovered += Button_Hovered1;
            button.Clicked += expand_Clicked;
            button.ToolTipText = "Creates new main scheme. It is impossible to insert main scheme to another scheme. There is only one physical representation of main scheme in project. (You cant undo/redo this action.)";
            button.Tag = true;

            button = new ContextButton("Create new scheme");
            button.Hovered += Button_Hovered1;
            panel.Buttons.Add(button);
            button.Clicked += expand_Clicked;
            button.ToolTipText = "Creates new scheme. This scheme can be inserted to other schemes. (You cant undo/redo this action.)";
            button.Tag = false;
            panel.Changed();

            MenuPanel btn = new MenuPanel(GetTopMenuPanel_ButtonSettings(true));
            btn.Tag = panel;
            btn.Text = "New scheme";
            btn.Hovered += Button_Hovered;
            btn.ToolTipText = "Creates new main or regular scheme. You cant undo/redo this action.";
            EnableButton(btn, true);
            btn.Clicked += click_SchemeToBug; 
            topMenu.Children.Add(btn);
        }

        private void click_SchemeToBug(MenuPanel sender)
        {
            ContextPanel panel = (ContextPanel)sender.Tag;
            Rectangle senderBounds = sender.GetBounds();
            ImportantClassesCollection.ContextMenu.Show(new Point(senderBounds.X, senderBounds.Y + senderBounds.Height), panel);
        }

        private void expand_Clicked(ContextButton sender)
        {
            workplace.CreateNewTopBug((bool)sender.Tag);
        }

        private void CreateSimulationButtons()
        {
            sim_step = new MenuPanel(GetTopMenuPanel_ButtonSettings(true));
            sim_step.Text = "Step";
            sim_step.Hovered += Button_Hovered;
            sim_step.ToolTipText = "Do one step of simulation.";
            sim_step.Clicked += Step_Clicked;
            topMenu.Children.Add(sim_step);

            sim_start = new MenuPanel(GetTopMenuPanel_ButtonSettings());
            sim_start.Text = "Start";
            sim_start.Hovered += Button_Hovered;
            sim_start.ToolTipText = "Start or stop simulation";
            sim_start.Clicked += Sim_start_Clicked;
            topMenu.Children.Add(sim_start);
        }

        private void UpdateSimButtons()
        {
            if (workplace.Simulation.IsRunning)
            {
                sim_start.Text = "Stop";
                sim_start.ToolTipText = "Stop simulation. Simulation is currently running";
            }
            else
            {
                sim_start.Text = "Start";
                sim_start.ToolTipText = "Start simulation. Simulation is currenlty stoped.";
            }
            sim_start.TextChanged();
            EnableButton(sim_step, !workplace.Simulation.IsRunning);
        }

        private void Sim_start_Clicked(MenuPanel sender)
        {
            if (workplace.Simulation.IsRunning)
                workplace.Simulation.Stop();
            else
                workplace.Simulation.Start();
        }

        private void Step_Clicked(MenuPanel sender)
        {
            workplace.Simulation.DoStep();
        }

        private void CreateUndoRedo()
        {
            undo = new MenuPanel(GetTopMenuPanel_ButtonSettings());
            undo.Text = "<-";
            undo.Hovered += Button_Hovered;
            undo.ToolTipText = "Undo.";
            undo.Tag = true;
            undo.Clicked += Button_Clicked1;
            topMenu.Children.Add(undo);

            redo = new MenuPanel(GetTopMenuPanel_ButtonSettings());
            redo.Text = "->";
            redo.Hovered += Button_Hovered;
            redo.ToolTipText = "Redo.";
            redo.Tag = false;
            redo.Clicked += Button_Clicked1;
            topMenu.Children.Add(redo);
        }

        private void Button_Clicked1(MenuPanel sender)
        {
            bool undo = (bool)sender.Tag;
            if (undo)
                workplace.SchemeEventHistory.Undo();
            else
                workplace.SchemeEventHistory.Redo();
        }

        private void CreateActionsButton()
        {
            ContextPanel panel = new ContextPanel(engine.TextureLoader.CreateSimpleTexture(Color.Gray), engine.TextureLoader.GetFont("f1"));
            var values = Enum.GetValues(typeof(Actions)).Cast<Actions>();
            foreach (Actions action in values)
            {
                if (action != Actions.Inserting_Bug)
                {
                    ContextCheckButton btn = new ContextCheckButton(GlobalSettings.GetActionText(action)+ GlobalSettings.GetActionShortcut(action));
                    panel.Buttons.Add(btn);
                    btn.Clicked += Btn_Clicked;
                    btn.Tag = action;
                }
            }
            panel.Changed();

            MenuPanel button = new MenuPanel(GetTopMenuPanel_ButtonSettings());
            button.Text = "Actions";
            button.Hovered += Button_Hovered;
            button.ToolTipText = "Select type of action.";
            button.Tag = panel;
            button.Clicked += Button_Clicked;
            topMenu.Children.Add(button);
        }

        private void Button_Hovered1(ContextButton sender, bool isHovered)
        {
            if (isHovered)
                workplace.StatusText.SetToolTipText(sender.ToolTipText);
            else
                workplace.StatusText.SetToolTipText("");
        }

        private void Button_Hovered(MenuPanel sender, bool isHovered)
        {
            if (isHovered)
                workplace.StatusText.SetToolTipText(sender.ToolTipText);
            else
                workplace.StatusText.SetToolTipText("");
        }

        private void Btn_Clicked(ContextButton sender)
        {
            workplace.Set_CurrentAction((Actions)sender.Tag);
        }

        private void Button_Clicked(MenuPanel sender)
        {
            ContextPanel panel = (ContextPanel)sender.Tag;
            Rectangle senderBounds = sender.GetBounds();

            foreach (ContextCheckButton btn in panel.Buttons)
                btn.Set_Checked(((Actions)btn.Tag == workplace.CurrentAction), false);
            ImportantClassesCollection.ContextMenu.Show(new Point(senderBounds.X, senderBounds.Y+senderBounds.Height), panel);
        }

        private MenuPanelSettings GetTopMenuPanel_ButtonSettings(bool createGap= false)
        {
            MenuPanelSettings toReturn = new MenuPanelSettings();
            if(createGap)
                toReturn.Margin = new Point(16, 0);
            else
                toReturn.Margin = new Point(0, 0);
            toReturn.Size = new Point(0, Default.TopMenu_Height);
            toReturn.TextHalign = HorizontalAligment.Center;
            toReturn.TextValign = VerticalAligment.Center;
            toReturn.TextMargin = new Point(Default.TopMenu_TextMargin, 0);
            toReturn.BackGroundTexture = this.enabledTexture;
            toReturn.Font = engine.TextureLoader.GetFont("f1");
            toReturn.AdjustWidthToContent = true;
            toReturn.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            toReturn.BorderWidth = 1;
            return toReturn;
        }
        
        public override bool KeyPress(Keys key)
        {
            switch (key)
            {
                case Keys.D:
                    workplace.Set_CurrentAction(Actions.DrawingLines);
                    break;
                case Keys.S:
                    workplace.Set_CurrentAction(Actions.Selecting);
                    break;
                case Keys.X:
                    workplace.Set_CurrentAction(Actions.Inserting_Diode);
                    break;
                case Keys.C:
                    workplace.Set_CurrentAction(Actions.Inserting_Konveror);
                    break;
                case Keys.I:
                    workplace.Set_CurrentAction(Actions.Inserting_Input);
                    break;
                case Keys.O:
                    workplace.Set_CurrentAction(Actions.Inserting_Output);
                    break;
                case Keys.R:
                    workplace.Set_CurrentAction(Actions.Removing);
                    break;
                case Keys.B:
                    workplace.Set_CurrentAction(Actions.BreakPoint);
                    break;
                case Keys.Delete:
                    workplace.CoppyAssistant.Delete();
                    break;
                default:
                    return false;
            }
            return true;
        }

        internal override void Draw(SpriteBatch sb)
        {
            workplace.Draw(sb);
            topMenu.ControlerDraw(sb);
        }
    }
}
