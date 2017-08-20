using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.ContextMenu;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;

namespace CP_Engine
{
    public class NewSchemeForm
    {
        WorkPlace workplace;
        Bug bug;
        TextInputMenuPanel titleInput, descInput, displayTextInput;
        bool createNew;
        bool createNewTopScheme;
        MenuPanel colorInput;

        /// <summary>
        /// Shows Form to create new or modify settings of Bug.
        /// </summary>
        /// <param name="workplace">Workplace.</param>
        /// <param name="bug">Bug, that will be modified. If NULL new bug will be created.</param>
        /// <param name="createNewTopScheme">Relevant only when bug parameter is NULL. TRUE: Created bug will be set as top bug.</param>
        public NewSchemeForm(WorkPlace workplace, Bug bug, bool createNewTopScheme = false)
        {
            this.workplace = workplace;
            this.createNewTopScheme = createNewTopScheme;
            if (bug == null)
            {
                createNew = true;
                bug = new Bug(workplace.Project.Bugs.GetBugID());
                bug.Title = workplace.Project.Bugs.DefaultBugName();
            }
            else
                createNew = false;
            this.bug = bug;

            MenuPanelSettings s = new MenuPanelSettings();
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.Size = new Point(600, 500);
            s.IgnoreEffects = true;
            MenuPanel content = new MenuPanel(s);

            MenuPanelSettings inputSettings = DefaultUI.DefaultNumericInput();
            MenuPanelSettings lblSettings = DefaultUI.DefaultLabelSettings();

            //Title
            MenuPanel lbl = new MenuPanel(lblSettings);
            lbl.Text = "Title";
            content.Children.Add(lbl);

            titleInput = new TextInputMenuPanel(inputSettings);
            titleInput.Text = bug.Title;
            content.Children.Add(titleInput);

            //Bug Display text===========================
            lbl = new MenuPanel(lblSettings);
            lbl.Text = "Display text";
            content.Children.Add(lbl);

            displayTextInput = new TextInputMenuPanel(inputSettings);
            displayTextInput.Text = bug.DisplayTextContertor.Text;
            content.Children.Add(displayTextInput);

            //Bug Description===========================
            lbl = new MenuPanel(lblSettings);
            lbl.Text = "Description:";
            content.Children.Add(lbl);

            inputSettings = DefaultUI.DefaultMultilineInput(3);
            descInput = new TextInputMenuPanel(inputSettings);
            descInput.Text = bug.Description;
            content.Children.Add(descInput);

            lbl = new MenuPanel(lblSettings);
            lbl.Text = "Color:";
            content.Children.Add(lbl);

            colorInput = new MenuPanel(DefaultUI.DefaultComboBoxSettings());
            colorInput.Clicked += ColorInput_Clicked;
            content.Children.Add(colorInput);
            
            content.Changed();
            string formTitle;
            if (bug != null)
                formTitle = "Global scheme settings (" + bug.Title + ")";
            else
                formTitle = "New scheme";

            //Select right color
            colorInput.Tag = GlobalSettings.BugColors[bug.ColorID];
            colorInput.Text = GlobalSettings.BugColors[bug.ColorID].Text;
            colorInput.TextChanged();

            Form form = DefaultUI.CreateDefaultForm(formTitle, content);
            form.AfterClose += Form_Closed;
        }

        private void ColorInput_Clicked(MenuPanel sender)
        {
            ContextPanel panel = new ContextPanel(colorInput.Settings.BackGroundTexture, colorInput.Settings.Font);
            panel.MinimalWidth = sender.GetBounds().Width;
            foreach (NamedColor current in GlobalSettings.BugColors)
            {
                ContextButton btn = new ContextButton(current.Text);
                btn.Clicked += Btn_Clicked;
                btn.Tag = current;
                panel.Buttons.Add(btn);
            }
            panel.Changed();
            ImportantClassesCollection.ContextMenu.Show(sender, panel);
        }

        private void Btn_Clicked(ContextButton sender)
        {
            NamedColor nc = (NamedColor)sender.Tag;
            colorInput.Tag = nc;
            colorInput.Text = nc.Text;
            colorInput.TextChanged();
        }

        private void Form_Closed(Form sender, bool result)
        {
            if (result == false)
                return;
            this.bug.Title = titleInput.Text;
            this.bug.Description = descInput.Text;
            this.bug.DisplayTextContertor.Text = displayTextInput.Text;
            this.bug.DisplayTextContertor.Changed();
            this.bug.ColorID = ((NamedColor)colorInput.Tag).ID;
            if (createNew)
            {
                workplace.Project.CreateBasicSchemeToBug(bug);
                //Set as top bug and change action to inserting bug.
                if (createNewTopScheme)
                {
                    Bug topBug = workplace.Project.TopPScheme.PlacedBug.Bug;
                    workplace.Project.SetAsTopBug(bug.Scheme);
                    workplace.StartInserting(topBug);
                }
                else
                    workplace.StartInserting(bug);
            }
        }
    }
}
