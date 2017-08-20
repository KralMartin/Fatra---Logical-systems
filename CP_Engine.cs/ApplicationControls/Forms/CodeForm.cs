using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.Menu;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CP_Engine.cs.ApplicationControls.Forms
{
    class CodeForm
    {
        private Code code;
        WorkPlace workplace;
        List<bool> bits;
        PlacedBug pBug;
        PhysScheme pScheme;

        public CodeForm(Code code, WorkPlace workplace, PlacedBug pBug, PhysScheme pScheme)
        {
            this.code = code;
            this.workplace = workplace;
            this.pBug = pBug;
            this.pScheme = pScheme;

            if (workplace.Project.Programmability.RecompileFunctions() == false)
            {
                Form form = DefaultUI.CreateFormText("Error", "Compiler functions contains error!");
                return;
            }

            bits = workplace.Project.Programmability.Translate(code);
            string text;
            if (bits != null)
            {
                Form form = DefaultUI.CreateFormQuestion("Binary code preview.", workplace.Project.Programmability.GetNoteText() +
                    "\n\n" + BinaryMath.ToBinarry(bits.ToArray(), workplace.Project.Programmability.InstructionWidth));
                form.BeforeClose += Form_BeforeClose;
            }
            else
            {
                Form form = DefaultUI.CreateFormText("Error", "This source code can not be compiled!");
            }
        }

        private void Form_BeforeClose(Form sender, bool result, ref bool closeForm)
        {            
            if (result)
            {
                Queue<bool> bitQue = workplace.Project.Programmability.Rotate(bits);
                workplace.Simulation.StopThread();
                pBug.Bug.SetMemmoryValue(bitQue, pScheme, workplace.Simulation);
                workplace.Simulation.StartThread();
            }
        }        
    }
}
