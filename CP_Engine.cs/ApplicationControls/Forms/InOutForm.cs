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
    class InOutForm
    {
        Point coords;
        IODescription desc;
        WorkPlace workplace;
        NumericInputMenuPanel orderInput;
        TextInputMenuPanel descriptionInput;
        TextInputMenuPanel titleInput;
        List<IODescription> items;

        internal InOutForm(WorkPlace workplace, Point coords)
        {
            this.workplace = workplace;
            this.coords = coords;
            desc = workplace.CurrentWindow.Scheme.Bug.IODecription.Get_Description(coords);
            if (desc.IsInput)
                items = workplace.CurrentWindow.Scheme.Bug.IODecription.Get_Inputs();
            else
                items = workplace.CurrentWindow.Scheme.Bug.IODecription.Get_Outputs();
            int maxOrder = items.Count;

            MenuPanelSettings s = new MenuPanelSettings();
            s.ChildrenLayout = ContextMenu_Mono.ChildrenLayouts.Normal;
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.Size = new Point(300, 200);
            s.IgnoreEffects = true;
            MenuPanel content = new MenuPanel(s);

            MenuPanelSettings inputSettings = DefaultUI.DefaultNumericInput();
            MenuPanelSettings lblSettings = DefaultUI.DefaultLabelSettings();

            
            //Order 
            //+40
            lblSettings.Margin = new Point(10, 0);
            MenuPanel lbl = new MenuPanel(lblSettings);
            lbl.Text = "Order";
            content.Children.Add(lbl);
            //+20
            inputSettings.Margin = new Point(10, 25);
            orderInput = new NumericInputMenuPanel(inputSettings, 1, maxOrder, 1);
            orderInput.Text = (desc.Order+1).ToString();
            content.Children.Add(orderInput);

            //DispayText
            //+40
            lblSettings.Margin = new Point(10, 65);
            lbl = new MenuPanel(lblSettings);
            lbl.Text = "DisplayText";
            content.Children.Add(lbl);
            //+25
            inputSettings.Margin = new Point(10, 90);
            titleInput = new TextInputMenuPanel(inputSettings);
            titleInput.Text = desc.DisplayText;
            content.Children.Add(titleInput);
            
            //Desc
            //+40
            lblSettings.Margin = new Point(10, 130);
            lbl = new MenuPanel(lblSettings);
            lbl.Text = "Description";
            content.Children.Add(lbl);
            //+25
            inputSettings.Margin = new Point(10, 155);
            descriptionInput = new TextInputMenuPanel(inputSettings);
            descriptionInput.Text = desc.Description;
            content.Children.Add(descriptionInput);

            Form form = DefaultUI.CreateDefaultForm("Input Output settings", content);
            form.AfterClose += Form_Closed;
        }

        private void Form_Closed(Form sender, bool result)
        {
            if (result == false)
                return;

            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            workplace.SchemeEventHistory.EventRequiresBugReplace();
            //Save settings                
            //Order        
            int newOrder = (int)this.orderInput.GetValue()-1;
            if (newOrder != this.desc.Order)
            {
                this.items.Remove(this.desc);
                this.items.Insert(newOrder , this.desc);
                int index = 0;
                foreach (IODescription item in this.items)
                    item.Order = index++;
            }
            //Title
            desc.DisplayText = this.titleInput.Text;
            //Description
            desc.Description = this.descriptionInput.Text;

            workplace.SchemeEventHistory.FinalizeEvent();
        }
    }
}
