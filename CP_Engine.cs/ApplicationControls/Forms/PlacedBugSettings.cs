using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using CP_Engine.BugItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    class PlacedBugSettings
    {
        NumericInputMenuPanel orderInput;
        TextInputMenuPanel descInput;
        CheckMenuPanel checkOrder;

        PlacedBug pBug;
        WorkPlace workplace;
        int defaultOrder;

        internal PlacedBugSettings(WorkPlace workplace, PlacedBug pBug)
        {
            this.workplace = workplace;
            this.pBug = pBug;
            
            MenuPanelSettings s = new MenuPanelSettings();
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.Size = new Point(300, 400);
            s.IgnoreEffects = true;
            MenuPanel content = new MenuPanel(s);

            MenuPanelSettings lblSettings = DefaultUI.DefaultLabelSettings();

            //Title
            MenuPanel lbl = new MenuPanel(lblSettings);
            lbl.Text = "Bug title";
            content.Children.Add(lbl);
            content.Changed();

            MenuPanelSettings inputSettings = DefaultUI.DefaultNumericInput();
            TextInputMenuPanel input = new TextInputMenuPanel(inputSettings);
            input.Text = pBug.Bug.Title;
            content.Children.Add(input);
            Enable(input, false);

            //Bug Description===========================
            lbl = new MenuPanel(lblSettings);
            lbl.Text = "Bug description:";
            content.Children.Add(lbl);
            content.Changed();

            inputSettings = DefaultUI.DefaultMultilineInput(3);
            input = new TextInputMenuPanel(inputSettings);
            input.Text = pBug.Bug.Description;
            content.Children.Add(input);
            Enable(input, false);

            //Order============================================
            if (pBug.Bug.IsUserCreated() || pBug.Bug is MemmoryBug)
            {
                checkOrder = new CheckMenuPanel(lblSettings, true, CheckTypes.None);
                checkOrder.Text = "In Order:";
                checkOrder.CheckedChanged += CheckOrder_CheckedChanged;
                content.Children.Add(checkOrder);

                //Calculate max order
                List<PlacedBug> pBugs = workplace.CurrentWindow.Scheme.PlacedBugs.GetItems();
                int maxOrder = pBugs.Max(x => x.Order);
                if (pBug.Order < 0)
                    maxOrder++;
                maxOrder++;
                if (pBug.Order >= 0)
                    defaultOrder = pBug.Order + 1;
                else
                    defaultOrder = maxOrder;

                inputSettings = DefaultUI.DefaultNumericInput();
                orderInput = new NumericInputMenuPanel(inputSettings, 1, maxOrder, 1);
                content.Children.Add(orderInput);
            }
            else if (pBug.Bug is ClockBug)
            {
                lbl = new MenuPanel(lblSettings);
                lbl.Text = "Simulation steps:";
                content.Children.Add(lbl);

                inputSettings = DefaultUI.DefaultNumericInput();
                orderInput = new NumericInputMenuPanel(inputSettings, 1, 1000000, 1);
                orderInput.Text = pBug.Number.ToString();
                content.Children.Add(orderInput);
            }
            //PlacedBug Description
            lbl = new MenuPanel(lblSettings);
            lbl.Text = "Local description:";
            content.Children.Add(lbl);
            content.Changed();

            inputSettings = DefaultUI.DefaultMultilineInput(3);
            descInput = new TextInputMenuPanel(inputSettings, true);
            descInput.Text = pBug.Description;
            content.Children.Add(descInput);

            //Global settings button
            if (pBug.Bug.IsUserCreated() || pBug.Bug is MemmoryBug)
            {
                MenuPanel btn = new MenuPanel(DefaultUI.DefaultButtonSettings());
                btn.Clicked += Btn_Clicked;
                btn.Text = "Global settings";
                content.Children.Add(btn);
            }
            //Check checked btn
            if (checkOrder != null)
            {
                checkOrder.Set_Checked((pBug.Order >= 0), false);
                CheckOrder_CheckedChanged(checkOrder);
            }
            Form form = DefaultUI.CreateDefaultForm("Local scheme settings (" + pBug.Bug.Title + ")", content);
            form.AfterClose += Form_Closed;
        }

        private void Btn_Clicked(MenuPanel sender)
        {
            NewSchemeForm schemeForm = new NewSchemeForm(this.workplace, this.pBug.Bug, false);
        }

        private void CheckOrder_CheckedChanged(CheckMenuPanel sender)
        {
            Enable(orderInput, sender.Checked);
            if (sender.Checked)
            {
                checkOrder.Text = "(x) In Order:";
                orderInput.Text = defaultOrder.ToString();
            }
            else
            {
                checkOrder.Text = "( ) In Order:";
                orderInput.Text = "";
            }
            orderInput.TextChanged();
            checkOrder.TextChanged();
        }

        private void Enable(MenuPanel panel, bool enable)
        {
            MenuPanelSettings s = panel.Settings;
            s.IgnoreEffects = !enable;
            if(enable)
                s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.LightGray);
            else
                s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.DarkGray);
            panel.Settings = s;

        }

        private void Form_Closed(Form sender, bool result)
        {
            if (result == false)
                return;
            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            if (checkOrder != null)
            {
                int newOrder = -1;
                if (checkOrder.Checked)
                    newOrder = (int)orderInput.GetValue() - 1;
                if (newOrder != pBug.Order)
                    workplace.CurrentWindow.Scheme.PlacedBugs.ChangeOrder(pBug, newOrder);
            }
            else if (pBug.Bug is ClockBug)
            {
                pBug.Number = (int)orderInput.GetValue();
            }
            pBug.Description = descInput.Text;
            workplace.SchemeEventHistory.FinalizeEvent();
        }
    }
}
