using ContextMenu_Mono;
using ContextMenu_Mono.Menu;
using ContextMenu_Mono.ScrollWindowItems;
using CP_Engine;
using Microsoft.Xna.Framework;
using Utilties_Mono;

namespace CP_v1
{
    class CodeSelectHalfScreen : HalfScreen
    {
        ScrollWindow scrollWindow;

        public CodeSelectHalfScreen(SelectCodeScreen screen, Code selectedCode) : base(screen, HorizontalAligment.Right, "Source Codes")
        {
            MenuPanelSettings s = this.ContentPanel.Settings;
            s.Margin = new Point();
            scrollWindow = new ScrollWindow(s, itemSize);
            this.ContentPanel.Children.Add(scrollWindow.GetMainPanel());
            CreateButtons("New code", "Delete code", "Modify code", null, 3);

            UpdateCodes(selectedCode);
        }

        internal override void Resize()
        {
            base.Resize();
            MenuPanelSettings s = this.ContentPanel.Settings;
            s.Margin = new Point();
            scrollWindow.Resize(s, itemSize);
        }

        protected override void ButtonClicked(int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                //New
                Code code = new Code();
                code.Title = "code title";
                code.CodeText = "code";
                screen.Workplace.Project.Programmability.CodeItems.Add(code);
                this.screen.ChangeHalfScreenRight(new CodeWriteHalfScreen(screen, code));
            }
            else if (buttonIndex == 1)
            {
                //delete
                CheckMenuPanel panel = this.checkGroup.GetChecked();
                if (panel != null && panel.Tag != null)
                {
                    screen.Workplace.Project.Programmability.CodeItems.Remove((Code)panel.Tag);
                    UpdateCodes(null);
                }
            }
            else
            {
                //Modify
                CheckMenuPanel panel = this.checkGroup.GetChecked();
                if (panel != null && panel.Tag != null)
                    this.screen.ChangeHalfScreenRight(new CodeWriteHalfScreen(screen, (Code)panel.Tag));
            }
        }

        private void UpdateCodes(Code selectedCode)
        {
            if (selectedCode == null && screen.Workplace.Project.Programmability.CodeItems.Count > 0)
                selectedCode = screen.Workplace.Project.Programmability.CodeItems[0];

            checkGroup = new CheckMenuPanelGroup();
            scrollWindow.MenuPanelItems.Clear();

            foreach (Code code in  screen.Workplace.Project.Programmability.CodeItems)
            {
                CheckMenuPanel btn = DefaultCheckBox();
                if (code.Title == selectedCode.Title)
                    btn.Set_Checked(true, false);

                btn.Text = code.Title;
                btn.DoubleClicked += Check_DoubleClicked;
                btn.Tag = code;
                scrollWindow.MenuPanelItems.Add(btn);
            }
            scrollWindow.Changed();
        }
        
        private void Check_DoubleClicked(MenuPanel sender)
        {
            Code code = (Code)sender.Tag;
            this.screen.ChangeHalfScreenRight(new CodeWriteHalfScreen(screen, code));
        }
    }
}
