using System;
using System.Collections.Generic;

using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.Common.Events;
using Miyagi.UI.Controls.Styles;

namespace Game.GUICreator
{
	public class MenuGUI : GUIFactory
    {
        public enum Buttons { Play, Options, Exit }
        private Dictionary<string, Button> mButtons;

        public MenuGUI(MiyagiMgr miyagiMgr, string name) : base(miyagiMgr, name) {}

        protected override void CreateGUI()
        {
            /* Backgroung image */
            PictureBox backgroung = new PictureBox();
            backgroung.AlwaysOnBottom = true;
            backgroung.Bitmap = new System.Drawing.Bitmap(@"../../src/Media/skins/background.bmp");
            backgroung.Size = this.WndSize;
            this.mGUI.Controls.Add(backgroung);
            
            /* Buttons */
            //Size originalImgSize = new Size(1600, 900);
            Point[] pos = new Point[] { new Point(688, 372), new Point(685, 491), new Point(686, 626) };
            //Point relativePos = new Point(originalPos.X / originalImgSize.Width * backgroung.Size.Width, originalPos.Y / originalImgSize.Height * backgroung.Size.Height);
            Size buttonSize = new Size(188, 76);
            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleCenter;

            this.mButtons = new Dictionary<string, Button>();
            for (int i = 0; i < Enum.GetValues(typeof(Buttons)).Length; i++)
            {
                Button button = new Button();
                button.Size = buttonSize;
                button.Text = Enum.GetName(typeof(Buttons), (Buttons)i);
                button.TextStyle = style;
                button.Location = pos[i];
                button.Skin = this.mMiyagiMgr.Skins["Button"];
                this.mGUI.Controls.Add(button);
                this.mButtons.Add(button.Text, button);
            }
        }

        public void SetListener(Buttons button, EventHandler<MouseButtonEventArgs> del)
        {
            this.mButtons[Enum.GetName(typeof(Buttons), button)].MouseClick += new EventHandler<Miyagi.Common.Events.MouseButtonEventArgs>(del);
        }
	}
}