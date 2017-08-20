using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilties_Mono;

namespace CP_Engine
{
    class BreakPointForm
    {
        WorkPlace workplace;
        List<BreakPointResult> results;
        Form form;
        MenuPanel content;
        int breakPointIndex;
        int occouranceIndex;

        internal BreakPointForm(WorkPlace workplace)
        {
            this.workplace = workplace;
            results = workplace.Simulation.BreakPointResults.ToList();

            MenuPanelSettings s = new MenuPanelSettings();
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.TextMargin = new Point(20, 20);
            s.TextValign = VerticalAligment.Center;
            s.Size = new Point(300, 200);

            content = new MenuPanel(s);
            content.Text = "tesdt t";

            FormSettings fs = DefaultUI.DefaultFormSettings("Break Point");
            fs.ButtonOKText = "Go to next";
            fs.ButtonCancelText = "Exit";
            form = new Form(fs, ImportantClassesCollection.MenuLayer, content);
            form.BeforeClose += Form_BeforeClose;

            breakPointIndex = 0;
            occouranceIndex = 0;
            Update();
        }

        private void Form_BeforeClose(Form sender, bool result, ref bool closeForm)
        {
            if (result)
            {
                closeForm = Update();
            }
        }
        private bool Update()
        {
            if (breakPointIndex >= results.Count)
                return true;
            BreakPointResult result = results[breakPointIndex];
            BreakPointOccourance occourance = result.BreakOccourances[occouranceIndex];
            workplace.OpenWindow(occourance.PhysScheme);

            content.Text = "Path:" + occourance.PhysScheme.GetPath() + "\n" +
                "Old value: " + BinaryMath.ToBinarry(occourance.OldValues) + "\n" +
                "New value: " + BinaryMath.ToBinarry(occourance.NewValues);
            content.TextChanged();

            occouranceIndex++;
            if (result.BreakOccourances.Count <= occouranceIndex)
            {
                occouranceIndex = 0;
                breakPointIndex++;
            }
            if (breakPointIndex >= results.Count)
            {
                MenuPanel btn = form.Get_ButtonOK();
                MenuPanelSettings s = btn.Settings;
                s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.DarkGray);
                btn.Settings = s;
            }
            return false;
        }
    }
}
