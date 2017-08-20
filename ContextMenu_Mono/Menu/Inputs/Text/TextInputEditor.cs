using ContextMenu_Mono.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilties_Mono;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Utilities_Mono;
using Microsoft.Xna.Framework.Graphics;

namespace ContextMenu_Mono
{
    public class TextInputEditor : Controler
    {
        protected TextInputMenuPanel panel;

        SpriteFont font;

        Rectangle bounds;
        string textBak;        
        bool allowEnter;
        HashSet<Keys> keyHashSet;
        
        internal TextInputEditor(TextInputMenuPanel panel, SpriteFont font, bool allowEnter)
        {
            this.panel = panel;
            this.font = font;
            this.allowEnter = allowEnter;
            this.bounds = panel.GetBounds();
            keyHashSet = new HashSet<Keys>();
            ImportantClassesCollection.MenuLayer.Push(this);
            if (panel.Text == null)
            {
                panel.Text = "";
                panel.TextChanged();
            }
            textBak = panel.Text;
            panel.DrawnText.CursorPosition = textBak.Length;
            panel.TextChanged();
        }

        #region Controler overrides

        public override bool MousePressLeft()
        {
            if (bounds.Contains(UserInput.MouseState.Position))
            {
                return true;
            }
            panel.SetEditorNull();
            panel.TextChanged();
            this.Pop();
            return false;
        }

        public override bool KeyPress(Keys key)
        {
            keyHashSet.Add(key);
            ExecuteKey(key);
            return true;
        }

        public override void KeyPressAgain(Keys key)
        {
            keyHashSet.Remove(key);
            ExecuteKey(key);
        }

        private void ExecuteKey(Keys key)
        {
            switch (key)
            {
                case Keys.Escape:
                    panel.Text = textBak;
                    panel.SetEditorNull();
                    this.Pop();
                    break;
                case Keys.Enter:
                    if (allowEnter)
                    {
                        panel.DrawnText.AddText("\n");
                    }
                    else
                    {
                        panel.SetEditorNull();
                        this.Pop();
                    }
                    break;
                case Keys.Back:
                    panel.DrawnText.BackSpace();
                    break;
                case Keys.Delete:
                    panel.DrawnText.DeleteChar();
                    break;
                case Keys.Up:
                    panel.DrawnText.GoUp();
                    break;
                case Keys.Down:
                    panel.DrawnText.GoDown();
                    break;
                case Keys.Left:
                    panel.DrawnText.GoLeft();
                    break;
                case Keys.Right:
                    panel.DrawnText.GoRight();
                    break;
                case Keys.OemOpenBrackets:
                    if (UserInput.IsShiftDown())
                        ExecuteString('{');
                    else
                        ExecuteString('[');
                    break;
                case Keys.OemCloseBrackets:
                    if (UserInput.IsShiftDown())
                        ExecuteString('}');
                    else
                        ExecuteString(']');
                    break;
                case Keys.OemSemicolon:
                    if (UserInput.IsShiftDown())
                        ExecuteString(':');
                    else
                        ExecuteString(';');
                    break;
                case Keys.OemComma:
                    if (UserInput.IsShiftDown())
                        ExecuteString('<');
                    else
                        ExecuteString(',');
                    break;
                case Keys.OemPeriod:
                    if (UserInput.IsShiftDown())
                        ExecuteString('>');
                    else
                        ExecuteString('.');
                    break;
                case Keys.OemPlus:
                    ExecuteString('+');
                    break;
                case Keys.OemMinus:
                    ExecuteString('-');
                    break;
                default:
                    int n = (int)key;
                    char c = (char)n;
                    if (UserInput.IsShiftDown())
                        ExecuteString(c);
                    else
                    {
                        string sc = c.ToString();
                        sc = sc.ToLower();
                        ExecuteString(sc[0]);
                    }
                    break;
            }
            panel.TextChanged();
        }

        private void ExecuteString(char c)
        {
            if(ValidateAddedChar(c))
                panel.DrawnText.AddText(c.ToString());
        }
        
        public override long GetResetTime(Keys key)
        {
            if (keyHashSet.Contains(key))
            {
                return 500;
            }
            return 30;
        }

        #endregion

        protected virtual bool ValidateAddedChar(char character)
        {
            if (character == ',' || character == '.' || character == '<' || character == '>' || character == '(' || character == ')')
                return true;
            if (character == '[' || character == ']' || character == '{' || character == '}' || character == ':' || character == ';')
                return true;
            int decimalKeyRepresentation = (int)character;
            if (decimalKeyRepresentation >= 32 && decimalKeyRepresentation <= 126)
                return true;
            return false;
        }
        
    }
}
