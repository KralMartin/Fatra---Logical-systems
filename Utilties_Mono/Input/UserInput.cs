using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Utilties_Mono;
using System;
using System.Linq;

namespace Utilities_Mono
{
    /// <summary>
    /// Catches user-input and forwards it to Controlers.
    /// </summary>
    public class UserInput
    {
        /// <summary>
        /// Speed of DoubleClick in miliseconds.
        /// </summary>
        internal long DoubleClickExpire { get; set; }

        /// <summary>
        /// Controler with highest priority in recieving user-input.
        /// </summary>
        public Controler FirstControler { get; private set; }

        public MouseState MouseState { get; private set; }
        public MouseState OldMouse { get; private set; }
        public Keys[] PressedKeys { get; private set; }
        public GameTime GameTime { get; private set; }
        internal Controler LeftPressedControler { get; set; }                       //When Controler catches user-input, it also registers itself to get next left mouse button release.
        internal Controler RightPressedControler { get; set; }                      //When Controler catches user-input, it also registers itself to get next right mouse button release.
        internal Controler HoveredControler { get; set; }                           //When Controler catches user-input, it also registers itself to get next mouse move action.
        internal Dictionary<Keys, TimeAndControler> KeyPressedControler { get; set; }      //When Controler catches user-input, it also registers itself to get next mouse move action.
        internal List<Controler> ToBeRemoved { get; private set; }  //List of Controlers that want no longer recieve user-input .
                
        int oldWheelValue;
        double leftButtonReleased;
        Keys[] oldKeys;

        public UserInput()
        {
            DoubleClickExpire = 300;
            ToBeRemoved = new List<Controler>();
            FirstControler = new Controler(this);
            KeyPressedControler = new Dictionary<Keys, TimeAndControler>();
            oldKeys = new Keys[0];
            oldWheelValue = 0;
        }

        public void Update(GameTime gameTime)
        {
            this.GameTime = gameTime;
            UpdateMouse();
            UpdateKeyBoard(gameTime);
            foreach (Controler toRemove in ToBeRemoved)
                toRemove.RemoveFromParent();
            ToBeRemoved.Clear();
        }

        public bool IsShiftDown()
        {
            return PressedKeys.Contains(Keys.LeftShift);
        }

        private void UpdateKeyBoard(GameTime gameTime)
        {
            TimeAndControler tac;

            //Get keyboard-state and pressed-keys array.
            KeyboardState keyboardState = Keyboard.GetState();
            PressedKeys = keyboardState.GetPressedKeys();
            //Loop trough currently pressed keys.
            for (int i = 0; i < PressedKeys.Length; i++)
            {
                if (KeyPressedControler.ContainsKey(PressedKeys[i]))
                {
                    //Key was pressed in previous update too.   
                    tac = KeyPressedControler[PressedKeys[i]];                                                                       
                    if (tac.Controler != null)
                    {
                        long resetTime = tac.Controler.GetResetTime(PressedKeys[i]);
                        if ((gameTime.TotalGameTime.TotalMilliseconds - tac.TimeStamp) >= resetTime)
                        {
                            //Key was pressed long enough to fire KeyPress function again.
                            tac.Controler.KeyPressAgain(PressedKeys[i]);
                            tac.TimeStamp += resetTime;
                        }
                    }
                }
                else
                {
                    //Key was NOT pressed in previous update.
                    //Store time, when the key was pressed
                    tac = new TimeAndControler();
                    tac.TimeStamp = gameTime.TotalGameTime.TotalMilliseconds;
                    KeyPressedControler.Add(PressedKeys[i], tac);

                    //Forward user-input to Controlers.
                    //If any of controlers catches input, it sets 
                    FirstControler.KeyPress_Internal(PressedKeys[i]);
                }
            }
            //Loop trough keys, that were pressed last update.
            for (int i = 0; i < oldKeys.Length; i++)
            {
                //If key is released now, remove information about when it was pressed.
                if (keyboardState.IsKeyUp(oldKeys[i]))
                {
                    //Forward user-input to Controlers.
                    if(KeyPressedControler[oldKeys[i]].Controler!=null)
                        KeyPressedControler[oldKeys[i]].Controler.KeyRelease(oldKeys[i]);
                    KeyPressedControler.Remove(oldKeys[i]);
                }
            }
            oldKeys = PressedKeys;
        }

        private void UpdateMouse()
        {
            MouseState = Mouse.GetState();

            //Mouse move.
            Controler oldHoveredControler = HoveredControler;
            FirstControler.MouseMove_Internal();
            if (oldHoveredControler != null && object.ReferenceEquals(oldHoveredControler, HoveredControler) == false)
                oldHoveredControler.LostMouseMove();

            if (oldWheelValue != MouseState.ScrollWheelValue)
            {
                int deltaWheel = MouseState.ScrollWheelValue - oldWheelValue;
                oldWheelValue = MouseState.ScrollWheelValue;
                FirstControler.MouseWheel_Internal(deltaWheel);
            }

            if (MouseState.LeftButton != OldMouse.LeftButton)
            {
                if (MouseState.LeftButton == ButtonState.Pressed)
                {
                    FirstControler.MousePressLeft_Internal();
                }
                else if (LeftPressedControler != null)
                {
                    LeftPressedControler.MouseReleaseLeft(this);
                    if (GameTime.TotalGameTime.TotalMilliseconds - leftButtonReleased < DoubleClickExpire)
                        LeftPressedControler.DoubleClick(this);
                    LeftPressedControler = null;
                    leftButtonReleased = GameTime.TotalGameTime.TotalMilliseconds;
                }
            }
            if (MouseState.RightButton != OldMouse.RightButton)
            {
                if (MouseState.RightButton == ButtonState.Pressed)
                {
                    FirstControler.MousePressRight_Internal();
                }
                else if (RightPressedControler != null)
                {
                    RightPressedControler.MouseReleaseRight(this);
                    RightPressedControler = null;
                }
            }
            OldMouse = MouseState;
        }
    }
}
