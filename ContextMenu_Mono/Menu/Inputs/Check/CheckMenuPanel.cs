using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities_Mono;

namespace ContextMenu_Mono.Menu
{
    public enum CheckTypes { None = 0, Background = 1, Border = 2, ColoredBackGroundTexture = 4 }

    public class CheckMenuPanel : MenuPanel
    {
        public delegate void CheckedChangedEventHandler(CheckMenuPanel sender);
        public event CheckedChangedEventHandler CheckedChanged;

        public bool Checked { get; private set; }

        public CheckTypes CheckType { get; set; }
        public MenuPanelSettings CheckedSettings { get; set; }
        public Color CheckedBackgroundColor { get;  set; }

        private CheckMenuPanelGroup group;  //When group is set, group manages button's checked state.
        private bool toogling;
        private RectBorder checkedBorder;

        /// <summary>
        /// CheckMenuPanel is in no group => Checking this CheckMenuPanel wont affect any other CheckMenuPanel.
        /// </summary>
        /// <param name="settings"></param>
        public CheckMenuPanel(MenuPanelSettings settings, bool toogling, CheckTypes checkType) : base(settings)
        {
            this.toogling = toogling;
        }

        /// <summary>
        /// CheckMenuPanel is in group. 
        /// Only one CheckMenuPanel can be checked at time in group.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="group"></param>
        public CheckMenuPanel(MenuPanelSettings settings, CheckMenuPanelGroup group, CheckTypes checkType) : base(settings)
        {
            this.group = group;
            this.CheckType = checkType;
            group.Panels.Add(this);
        }

        /// <summary>
        /// Set Checked property.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="fireEvent">TRUE: CheckedChanged event will be fired.</param>
        public void Set_Checked(bool value, bool fireEvent)
        {
            if (Checked!= value)
            {
                if (group != null)
                    group.CheckedChanged(this, fireEvent);
                else
                {
                    ChangeCheckedAndFireEvent(fireEvent);
                }
            }
            
        }

        internal override void ExecuteAction()
        {
            if (toogling)
                this.Set_Checked(!this.Checked, true);
            else
                this.Set_Checked(true, true);
        }

        internal void ChangeCheckedAndFireEvent(bool fireEvent)
        {
            this.Checked = !this.Checked;
            if (fireEvent && CheckedChanged != null)
                CheckedChanged(this);
        }

        protected override void Changed_Protected()
        {
            base.Changed_Protected();
            
            //Set Checked Border.
            if (Settings.BorderWidth > 0)
                checkedBorder = new RectBorder(CheckedSettings.BorderWidth, new Rectangle(0, 0, relativeBounds.Width, relativeBounds.Height), CheckedSettings.BorderColor);
            else
                checkedBorder = null;
        }

        /// <summary>
        /// Same function as parent, but only adds Checked effect
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="adjustment"></param>
        internal override void Draw(SpriteBatch sb, Point adjustment)
        {
            Rectangle rect = relativeBounds;
            adjustment.X += rect.X;
            adjustment.Y += rect.Y;
            rect.X = adjustment.X;
            rect.Y = adjustment.Y;

            //Select background texture.
            Texture2D bcgTex;
            if (Checked && (this.CheckType & CheckTypes.Background) == CheckTypes.Background)
                bcgTex = this.CheckedSettings.BackGroundTexture;
            else
                bcgTex = this.Settings.BackGroundTexture;

            if (bcgTex != null)
            {
                if (Settings.IgnoreEffects)
                    sb.Draw(bcgTex, rect, Color.White);
                else if (Checked && (this.CheckType & CheckTypes.ColoredBackGroundTexture) == CheckTypes.ColoredBackGroundTexture)
                    sb.Draw(bcgTex, rect, CheckedBackgroundColor);
                else if (isPressed)
                    sb.Draw(bcgTex, rect, Color.Red);
                else if (isHovered)
                    sb.Draw(bcgTex, rect, Color.LightBlue);
                else
                    sb.Draw(bcgTex, rect, Color.White);
            }
            //Draw text.
            if (string.IsNullOrEmpty(this.Text) == false)
                DrawnText.DrawTopLeftCorner(sb, adjustment);

            //Select border.
            RectBorder curBorder;
            if (Checked && (this.CheckType & CheckTypes.Border) == CheckTypes.Border)
                curBorder = checkedBorder;
            else
                curBorder = border;
            //Draw border.
            if (curBorder != null)
                curBorder.DrawDynamic(sb, adjustment);

            foreach (MenuPanel current in this.Children)
            {
                current.Draw(sb, adjustment);
            }
        }

    }
}
