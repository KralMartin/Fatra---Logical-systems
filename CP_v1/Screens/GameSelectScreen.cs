using ContextMenu_Mono;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Utilties_Mono;

namespace CP_v1
{
    class GameSelectScreen : Screen
    {
        CheckMenuPanelGroup checkGroup;
        MenuPanel stackPanel;
        MenuPanel bottomPanel;
        int chceckBtnHeight;
        int checkBtnSpacing;

        public GameSelectScreen(Engine engine) : base(engine)
        {
            chceckBtnHeight = 60;
            checkBtnSpacing = 10;
            checkGroup = new CheckMenuPanelGroup();

            MenuPanelSettings s = new MenuPanelSettings();
            s.Valign = VerticalAligment.Center;
            s.Halign = HorizontalAligment.Center;
            s.Size = new Point(Math.Min(600, ImportantClassesCollection.ScreenSize.X), (chceckBtnHeight+checkBtnSpacing)*5);
            s.IgnoreEffects = true;
            s.ChildrenLayout = ChildrenLayouts.VerticalStack;

            stackPanel = new MenuPanel(s);
            ImportantClassesCollection.MainControler.Push(stackPanel);

            RefreshFiles();

            s = new MenuPanelSettings();
            s.IgnoreEffects = true;
            s.ChildrenLayout = ChildrenLayouts.Normal;
            s.Halign = HorizontalAligment.Center;
            s.Valign = VerticalAligment.Bottom;
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.DarkGray);
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            s.BorderWidth = 2;
            s.Size = new Point(Math.Min(800, ImportantClassesCollection.ScreenSize.X), 128);

            bottomPanel = new MenuPanel(s);
            ImportantClassesCollection.MainControler.Push(bottomPanel);


            MenuPanel btn = DefaultButton(0, 0);
            btn.Text = "New project";
            btn.Clicked += New_Clicked;
            bottomPanel.Children.Add(btn);

            btn = DefaultButton(0,1);
            btn.Text = "Load project";
            btn.Clicked += Load_Clicked;
            bottomPanel.Children.Add(btn);

            btn = DefaultButton(0,2);
            btn.Text = "Delete project";
            btn.Clicked += Delete_Clicked;
            bottomPanel.Children.Add(btn);

            bottomPanel.Changed(new Rectangle(new Point(), ImportantClassesCollection.ScreenSize));
        }
        
        internal override void Resize()
        {
            stackPanel.Changed(new Rectangle(new Point(), ImportantClassesCollection.ScreenSize));
            bottomPanel.Changed(new Rectangle(new Point(), ImportantClassesCollection.ScreenSize));
        }

        private void Load_Clicked(MenuPanel sender)
        {
            MenuPanel pnl = checkGroup.GetChecked();
            if (pnl != null)
            {
                MyFile file = (MyFile)checkGroup.GetChecked().Tag;
                Close();
                GameScreen screen = new GameScreen(engine);
                engine.CurrentScreen = screen;
                screen.Load(file.FullFileName);
            }
        }

        internal Engine Get_Engine()
        {
            return engine;
        }

        private void RefreshFiles()
        {
            checkGroup = new CheckMenuPanelGroup();
            stackPanel.Children.Clear();
            MyFile[] files = GetFileNames();
            for (int i = 0; i < Math.Min(files.Length, 5); i++)
            {
                CheckMenuPanel check = DefaultCheckBox();
                if (i == 0)
                    check.Set_Checked(true, true);
                check.Text = files[i].FileName;
                check.DoubleClicked += Load_Clicked;
                check.Tag = files[i];
                stackPanel.Children.Add(check);
            }
            stackPanel.Changed(new Rectangle(new Point(), ImportantClassesCollection.ScreenSize));
        }
        
        private MyFile[] GetFileNames()
        {
            if (Directory.Exists(engine.Settings.ProjectsPath) == false)
                return new MyFile[0];
            string[] fileNames =  Directory.GetFiles(engine.Settings.ProjectsPath, "*.xml", SearchOption.TopDirectoryOnly);
            MyFile[] files = new MyFile[fileNames.Length];
            for (int i = 0; i < fileNames.Length; i++)
                files[i] = new MyFile(fileNames[i]);
            return files.OrderByDescending(x => x.LastTimeAccessed).ToArray();
        }

        private void Delete_Clicked(MenuPanel sender)
        {
            MenuPanel pnl = checkGroup.GetChecked();
            if (pnl != null)
            {
                MyFile file = (MyFile)pnl.Tag;
                File.Delete(file.FullFileName);
                RefreshFiles();
            }
        }

        private void New_Clicked(MenuPanel sender)
        {
            NewProjectForm form = new NewProjectForm(this.engine);
        }

        internal override void Close()
        {
            this.stackPanel.Pop();
            this.bottomPanel.Pop();
        }

        internal override void Draw(SpriteBatch sb)
        {
            stackPanel.ControlerDraw(sb);
            bottomPanel.ControlerDraw(sb);
        }

        private CheckMenuPanel DefaultCheckBox()
        {
            //General settings.
            MenuPanelSettings s = new MenuPanelSettings();
            s.Size = new Point(Math.Min(600, ImportantClassesCollection.ScreenSize.X), chceckBtnHeight);
            s.Margin = new Point(0, checkBtnSpacing);
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1");
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Gray);
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            s.BorderWidth = 2;

            CheckMenuPanel check = new CheckMenuPanel(s, checkGroup, CheckTypes.Border);
            
            //Checked settings.
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Yellow);
            s.BorderWidth = 4;
            check.CheckedSettings = s;

            return check;
        }

        private MenuPanel DefaultButton(int row, int col)
        {
            int btnSpacing = 10;
            int btnWidth = 120;
            int btnHeight = 40;

            int x = 64;
            if (col > 0)
                x += (btnSpacing + btnWidth) * col;
            int y = 16;
            if (row > 0)
                y += (btnSpacing + btnHeight) * row;

            MenuPanelSettings s = new MenuPanelSettings();
            s.Size = new Point(btnWidth, btnHeight);
            s.Margin = new Point(x, y);
            s.TextHalign = HorizontalAligment.Center;
            s.TextValign = VerticalAligment.Center;
            s.Valign = VerticalAligment.Top;
            s.Halign = HorizontalAligment.Left;
            s.Font = ImportantClassesCollection.TextureLoader.GetFont("f1"); 
            s.BackGroundTexture = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Gray);
            s.BorderColor = ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Black);
            s.BorderWidth = 1;

            MenuPanel panel = new MenuPanel(s);
            return panel;
        }
    }
}
