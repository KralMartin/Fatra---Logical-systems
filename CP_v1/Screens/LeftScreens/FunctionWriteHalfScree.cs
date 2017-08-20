using System;
using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using CP_Engine;
using Microsoft.Xna.Framework;
using Utilties_Mono;


namespace CP_v1
{
    class FunctionWriteHalfScree : HalfScreen
    {
        const int textPanelMargin = 10;

        TextInputMenuPanel titlePanel, codePanel;
        Function fun;

        public FunctionWriteHalfScree(SelectCodeScreen screen, Function fun) : base(screen, HorizontalAligment.Left, "Modify Function")
        {
            this.fun = fun;
            CreateButtons("Save", "Save & Back", "Check", "Back", 4);

            MenuPanelSettings s = this.ContentPanel.Settings;
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;
            this.ContentPanel.Settings = s;

            MenuPanelSettings inputSettings = DefaultUI.DefaultMultilineInput(1);
            MenuPanelSettings lblSettings = DefaultUI.DefaultLabelSettings();
            inputSettings.Size = new Point(this.ContentPanel.Settings.Size.X - textPanelMargin * 2, inputSettings.Size.Y);

            //Label
            MenuPanel lbl = new MenuPanel(lblSettings);
            lbl.Text = "Function text definition:";
            ContentPanel.Children.Add(lbl);
            //Input
            titlePanel = new TextInputMenuPanel(inputSettings, false);
            titlePanel.Text = fun.Name;
            this.ContentPanel.Children.Add(titlePanel);

            //Label
            lbl = new MenuPanel(lblSettings);
            lbl.Text = "Function binary definition:";
            ContentPanel.Children.Add(lbl);
            //Input
            codePanel = new TextInputMenuPanel(inputSettings, true);
            codePanel.Text = fun.FunctionCode;
            this.ContentPanel.Children.Add(codePanel);

            lblSettings.Size = new Point(this.ContentPanel.Settings.Size.X - textPanelMargin * 2, 200);
            lbl = new MenuPanel(lblSettings);
            lbl.Text = "Function text definition:\nThis text is compared to lines in source code. Function text is composed of two parts. "
                + "First part is name of function, second part are parameters. These two parts are divided by space character and parameters are divided by comma.\n\n"+
                "Function binary definition:\nWhen matching line is found in source code, it will be transformed to binnary code using binary definition of function.\n\n"+
                "Example:\n" +
                "Text definition:(SUM a, b)\nBinary definition: (1000aabb)\n" +
                "Then we have a line (SUM 10,11) in source code. This line will be translated to binnary code (10001011)";
            this.ContentPanel.Children.Add(lbl);
        }

        protected override void ButtonClicked(int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                //Save
                Save();
            }
            else if (buttonIndex == 1)
            {
                //Save & Exit
                Save();
                Exit();
            }
            else if (buttonIndex == 2)
            {
                //Check
                CheckFunction();
            }
            else
            {
                //Exit
                Exit();
            }
        }

        private void CheckFunction()
        {
            Function funToCheck = new Function();
            funToCheck.Name = titlePanel.Text;
            funToCheck.FunctionCode = codePanel.Text;
            string text;
            try
            {
                funToCheck.UpdateParameters();
                text = "Function is valid.";
            }
            catch (Exception e)
            {
                text = "Function is not valid.\n"+e.Message;
            }
            DefaultUI.CreateFormText("Function check", text);
        }

        private void Exit()
        {
            try
            {
                fun.UpdateParameters();
            }
            catch
            {
                Form form = DefaultUI.CreateFormQuestion("Error", "Function is not valid, do you still want to exit?");
                form.AfterClose += Form_AfterClose;
                return;
            }
            this.screen.ChangeHalfScreenLeft(new FunctionSelectHalfScreen(this.screen, fun));
        }

        private void Form_AfterClose(Form sender, bool result)
        {
            if(result)
                this.screen.ChangeHalfScreenLeft(new FunctionSelectHalfScreen(this.screen, fun));
        }

        private void Save()
        {
            fun.Name = titlePanel.Text;
            fun.FunctionCode = codePanel.Text;
        }
    }
}
