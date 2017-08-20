using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Utilities_Mono;
using System;

namespace Utilties_Mono
{
    public class Controler
    {
        public string DebugValue { get; set; }
        public UserInput UserInput { get; private set; }

        private Controler parent;
        private List<Controler> children;

        public Controler()
        {
            children = new List<Controler>();
        }

        internal Controler(UserInput userInput)
        {
            this.UserInput = userInput;
            children = new List<Controler>();
        }

        /// <summary>
        /// Push object to this Controler's children.
        /// Lastly pushed object will recieve user-input with highest priority.
        /// </summary>
        /// <param name="controler"></param>
        public void Push(Controler controler)
        {
            controler.parent = this;
            children.Insert(0, controler);
            if (this.UserInput != null)
                controler.ForwardUserInputToChildren(this.UserInput);
        }

        /// <summary>
        /// Remove object from this Parent Controler.
        /// Removed object will no longer recieve user-input.
        /// </summary>
        /// <param name="controler"></param>
        public void Pop()
        {
            if (UserInput != null)
                UserInput.ToBeRemoved.Add(this);
        }

        /// <summary>
        /// Removes all children Controlers.
        /// </summary>
        public void ClearChildren()
        {
            this.children.Clear();
        }

        /// <summary>
        /// Parent loses pointer to this object.
        /// </summary>
        internal void RemoveFromParent()
        {
            if (parent != null)
            {
                parent.children.Remove(this);
                parent = null;
            }
        }

        /// <summary>
        /// Whem UserInput is set to this object, forward it to all it's children.
        /// </summary>
        /// <param name="userInput"></param>
        private void ForwardUserInputToChildren(UserInput userInput)
        {
            this.UserInput = userInput;
            foreach (Controler controler in children)
                controler.ForwardUserInputToChildren(userInput);
        }

        #region Overidable functions

        /// <summary>
        /// This function draws all children in inverted order as they recieve user-input.
        /// </summary>
        /// <param name="sb"></param>
        public virtual void ControlerDraw(SpriteBatch sb)
        {
            for (int i = children.Count - 1; i >= 0; i--)
                children[i].ControlerDraw(sb);
        }
        
        /// <summary>
        /// If TRUE is returned, user-input will be catched and this recieves next MouseRelaseLeft.
        /// If FALSE is returned, user-input will be forwarder to next Controler.
        /// </summary>
        /// <returns></returns>
        public virtual bool MousePressLeft()
        {
            return false;
        }

        /// <summary>
        /// If TRUE is returned, user-input will be catched and this recieves next MouseRelaseRight.
        /// If FALSE is returned, user-input will be forwarder to next Controler.
        /// </summary>
        /// <returns></returns>
        public virtual bool MousePressRight()
        {
            return false;
        }

        /// <summary>
        /// To get called this function, this instance have to catch previous MousePressLeft.
        /// </summary>
        /// <param name="userInput"></param>
        public virtual void MouseReleaseLeft(UserInput userInput) { }

        /// <summary>
        /// To get called this function, this instance have to catch previous MousePressRight.
        /// </summary>
        /// <param name="userInput"></param>
        public virtual void MouseReleaseRight(UserInput userInput) { }
        
        public virtual void DoubleClick(UserInput userInput) { }

        /// <summary>
        /// If TRUE is returned, user-input will be catched and this LostMouseMove function will be called over this object.
        /// If FALSE is returned, user-input will be forwarder to next Controler.
        /// </summary>
        /// <returns></returns>
        public virtual bool MouseMove()
        {
            return false;
        }

        /// <summary>
        /// This function is called when this object catched (last - 1) mouse-move, but another object catched last mouse-move.
        /// </summary>
        public virtual void LostMouseMove() { }

        /// <summary>
        /// If TRUE is returned, user-input will be catched.
        /// If FALSE is returned, user-input will be forwarder to next Controler.
        /// </summary>
        /// <returns></returns>
        public virtual bool MouseWheel(int delta)
        {
            return false;
        }

        /// <summary>
        /// If TRUE is returned, user-input will be catched.
        /// If FALSE is returned, user-input will be forwarder to next Controler.
        /// </summary>
        /// <returns></returns>
        public virtual bool KeyPress(Keys key)
        {
            return false;
        }

        public virtual void KeyPressAgain(Keys key)
        {
        }

        /// <summary>
        /// If TRUE is returned, user-input will be catched.
        /// If FALSE is returned, user-input will be forwarder to next Controler.
        /// </summary>
        /// <returns></returns>
        public virtual void KeyRelease(Keys key)
        {
        }

        /// <summary>
        /// If key remains pressed longer than return value of this function, 
        /// KeyPress function will be executed again. 
        /// Return -1 when you do not care about button.
        /// </summary>
        /// <param name="key">Current key.</param>
        /// <returns>Key reset time.</returns>
        public virtual long GetResetTime(Keys key)
        {
            return -1;
        }
        #endregion

        #region Functions, that hanle user input

        /// <summary>
        /// Forwards user-input to all children.
        /// If none of children catches user-input,
        /// then this Controler will handle user-input.
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        internal bool MousePressLeft_Internal()
        {
            foreach (Controler current in children)
            {
                if (current.MousePressLeft_Internal())
                    return true;
            }
            if (MousePressLeft())
            {
                UserInput.LeftPressedControler = this;
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Forwards user-input to all children.
        /// If none of children catches user-input,
        /// then this Controler will handle user-input.
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        internal bool MousePressRight_Internal()
        {
            foreach (Controler current in children)
            {
                if (current.MousePressRight_Internal())
                    return true;
            }
            if (MousePressRight())
            {
                UserInput.RightPressedControler = this;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Forwards user-input to all children.
        /// If none of children catches user-input,
        /// then this Controler will handle user-input.
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        internal bool MouseMove_Internal()
        {
            foreach (Controler current in children)
            {
                if (current.MouseMove_Internal())
                    return true;
            }
            if (MouseMove())
            {
                UserInput.HoveredControler = this;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Forwards user-input to all children.
        /// If none of children catches user-input,
        /// then this Controler will handle user-input.
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        internal bool MouseWheel_Internal(int delta)
        {
            foreach (Controler current in children)
            {
                if (current.MouseWheel_Internal(delta))
                    return true;
            }
            return MouseWheel(delta);
        }

        internal bool KeyPress_Internal(Keys key)
        {
            foreach (Controler current in children)
            {
                if (current.KeyPress_Internal(key))
                    return true;
            }
            if (KeyPress(key))
            {
                UserInput.KeyPressedControler[key].Controler = this;
                return true;
            }
            return false;
        }
        
        #endregion

        public override string ToString()
        {
            if (DebugValue == null)
                return base.ToString();
            return DebugValue;
        }
    }
}
