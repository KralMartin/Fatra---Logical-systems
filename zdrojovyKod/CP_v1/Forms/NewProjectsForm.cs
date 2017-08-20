using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
namespace CP_v1
{
    class NewProjectForm
    {
        Engine engine;
        TextInputMenuPanel titleInput;

        internal NewProjectForm(Engine engine)
        {
            this.engine = engine;

            MenuPanelSettings s = new MenuPanelSettings();
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.Size = new Point(4, 4);
            s.AdjustHeightToContent = true;
            s.AdjustWidthToContent = true;
            s.IgnoreEffects = true;
            MenuPanel content = new MenuPanel(s);

            MenuPanelSettings lblSettings = DefaultUI.DefaultLabelSettings();
            MenuPanelSettings inputSettings = DefaultUI.DefaultMultilineInput(1);
            
            //Label
            MenuPanel lbl = new MenuPanel(lblSettings);
            lbl.Text = "Bug title:";
            content.Children.Add(lbl);
            //Input
            titleInput = new TextInputMenuPanel(inputSettings, false);
            content.Children.Add(titleInput);

            Form form = DefaultUI.CreateDefaultForm("Placed Bug settings", content);
            form.BeforeClose += Form_BeforeClose;
            form.AfterClose += Form_Closed;

            //Focus input
            titleInput.SimulateClick();
        }

        private void Form_BeforeClose(Form sender, bool result, ref bool closeForm)
        {
            if (result == false)
                return;
            if (titleInput.Text != "")
            {
                closeForm = true;
            }
            else
            {
                closeForm = false;
                DefaultUI.CreateFormText("Error", "Project name cant be empty!");
            }
        }

        private void Form_Closed(Form sender, bool result)
        {
            if (result == false)
                return;
            engine.CurrentScreen.Close();
            GameScreen screen = new GameScreen(engine);
            engine.CurrentScreen = screen;
            screen.New(titleInput.Text, engine.Settings.ProjectsPath + "\\" + titleInput.Text + ".xml");
        }
    }
}
