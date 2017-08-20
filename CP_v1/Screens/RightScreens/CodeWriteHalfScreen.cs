using System;
using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using CP_Engine;
using Microsoft.Xna.Framework;
using Utilties_Mono;
using System.Collections.Generic;

namespace CP_v1
{
    class CodeWriteHalfScreen : HalfScreen
    {
        const int textPanelMargin = 10;

        TextInputMenuPanel titlePanel, codePanel;
        Code code;

        public CodeWriteHalfScreen(SelectCodeScreen screen, Code code) : base(screen, HorizontalAligment.Right, "Modify Source Codes")
        {
            this.code = code;
            CreateButtons("Save", "Save & Back", "Compile", "Back", 4);

            MenuPanelSettings s = this.ContentPanel.Settings;
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;
            this.ContentPanel.Settings = s;

            MenuPanelSettings inputSettings = DefaultUI.DefaultMultilineInput(1);
            MenuPanelSettings lblSettings = DefaultUI.DefaultLabelSettings();
            inputSettings.Size = new Point(this.ContentPanel.Settings.Size.X - textPanelMargin * 2, inputSettings.Size.Y);

            //Label
            MenuPanel lbl = new MenuPanel(lblSettings);
            lbl.Text = "Title";
            ContentPanel.Children.Add(lbl);
            //Input
            titlePanel = new TextInputMenuPanel(inputSettings, false);
            titlePanel.Text = code.Title;
            this.ContentPanel.Children.Add(titlePanel);

            //Label
            lbl = new MenuPanel(lblSettings);
            lbl.Text = "Code";
            ContentPanel.Children.Add(lbl);
            //Input
            inputSettings.Size = new Point(inputSettings.Size.X,
                this.ContentPanel.Settings.Size.Y - (lblSettings.Margin.Y + lblSettings.Size.Y + inputSettings.Margin.Y) * 2 - inputSettings.Size.Y - 8);
            inputSettings.TextValign = VerticalAligment.Top;
            codePanel = new TextInputMenuPanel(inputSettings, true);
            codePanel.Text = code.CodeText;
            this.ContentPanel.Children.Add(codePanel);            
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
                Compile();
            }
            else
            {
                //Exit
                Exit();
            }
        }

        private void Compile()
        {
            if (screen.Workplace.Project.Programmability.RecompileFunctions() == false)
            {
                Form form = DefaultUI.CreateFormText("Error", "Compiler functions contain errors.");
            }
            else
            {
                Code codeToCheck = new Code();
                codeToCheck.CodeText = codePanel.Text;
                List<bool> bits = this.screen.Workplace.Project.Programmability.Translate(codeToCheck);
                string text;
                if (bits != null)
                {
                    text = this.screen.Workplace.Project.Programmability.GetNoteText();
                    text += "\n\n" + BinaryMath.ToBinarry(bits.ToArray(), screen.Workplace.Project.Programmability.InstructionWidth);
                }
                else
                    text = "This source code can not be compiled!";
                Form form = DefaultUI.CreateFormText("Compilation", text);
            }
        }

        private void Exit()
        {
            this.screen.ChangeHalfScreenRight(new CodeSelectHalfScreen(this.screen, code));
        }

        private void Save()
        {
            code.Title = titlePanel.Text;
            code.CodeText = codePanel.Text;
        }
    }
}
