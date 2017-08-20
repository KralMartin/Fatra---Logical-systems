using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using ContextMenu_Mono.ScrollWindowItems;
using CP_Engine;
using Microsoft.Xna.Framework;
using Utilties_Mono;

namespace CP_v1
{
    class FunctionSelectHalfScreen : HalfScreen
    {
        ScrollWindow scrollWindow;

        public FunctionSelectHalfScreen(SelectCodeScreen screen, Function selectedFun) : base(screen, HorizontalAligment.Left, "Functions")
        {
            MenuPanelSettings s = this.ContentPanel.Settings;
            s.Margin = new Point();
            scrollWindow = new ScrollWindow(s, itemSize);
            this.ContentPanel.Children.Add(scrollWindow.GetMainPanel());


            CreateButtons("New function", "Delete function", "Modify funcion", null, 3);
            UpdateCodes(selectedFun);
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
                Function fun = new Function();
                fun.Name = "fun title";
                fun.FunctionCode = "";
                screen.Workplace.Project.Programmability.FunctionItems.Add(fun);
                this.screen.ChangeHalfScreenLeft(new FunctionWriteHalfScree(screen, fun));
            }
            else if (buttonIndex == 1)
            {
                //delete
                CheckMenuPanel panel = this.checkGroup.GetChecked();
                if (panel!=null && panel.Tag != null)
                {
                    screen.Workplace.Project.Programmability.FunctionItems.Remove((Function)panel.Tag);
                    UpdateCodes(null);
                }
            }
            else
            {
                //Modify
                CheckMenuPanel panel = this.checkGroup.GetChecked();
                if (panel != null && panel.Tag != null)
                    this.screen.ChangeHalfScreenLeft(new FunctionWriteHalfScree(screen, (Function)panel.Tag));
            }
        }

        private void UpdateCodes(Function selectedFun)
        {
            if (selectedFun == null && screen.Workplace.Project.Programmability.FunctionItems.Count > 0)
                selectedFun = screen.Workplace.Project.Programmability.FunctionItems[0];

            checkGroup.Clear();
            scrollWindow.MenuPanelItems.Clear();

            foreach (Function fun in screen.Workplace.Project.Programmability.FunctionItems)
            {
                CheckMenuPanel btn = DefaultCheckBox();
                if (fun.Name == selectedFun.Name)
                    btn.Set_Checked(true, false);

                btn.Text = fun.Name;
                btn.DoubleClicked += Check_DoubleClicked;
                btn.Tag = fun;
                scrollWindow.MenuPanelItems.Add(btn);
            }
            scrollWindow.Changed();
        }
        
        private void Check_DoubleClicked(MenuPanel sender)
        {
            Function fun = (Function)sender.Tag;
            this.screen.ChangeHalfScreenLeft(new FunctionWriteHalfScree(screen, fun));
        }
    }
}
