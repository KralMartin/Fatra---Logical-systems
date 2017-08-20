using System;
using ContextMenu_Mono;
using ContextMenu_Mono.Advanced;
using ContextMenu_Mono.ContextMenu;
using ContextMenu_Mono.Menu;
using Microsoft.Xna.Framework;
using CP_Engine.MapItems;
using CP_Engine.SchemeItems;
using Utilties_Mono;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CP_Engine.cs.ProjectItems.CodeItems;
using CP_Engine.cs.ApplicationControls.Forms;

namespace CP_Engine
{
    class PopupMenuGenerator
    {
        ContextMenuClass contextMenuLayer;
        WorkPlace workplace;
        ContextPanel mainPanel;

        PlacedBug pBug;
        PhysScheme pScheme;
        Point coords;
        Tile tile;
        TileInfoItem info;

        internal PopupMenuGenerator(WorkPlace workplace, ContextMenuClass contextMenuLayer)
        {
            this.workplace = workplace;
            this.contextMenuLayer = contextMenuLayer;
            mainPanel = CreateDefaultPanel();
        }

        private ContextPanel CreateDefaultPanel()
        {
            return new ContextPanel(ImportantClassesCollection.TextureLoader.CreateSimpleTexture(Color.Gray), ImportantClassesCollection.TextureLoader.GetFont("f1"));
        }

        internal void ShowContextMenu(Point position, Selection selection)
        {
            AnalyzeSelection(position, selection);
            mainPanel.Buttons.Clear();
            Point tileID = workplace.CurrentWindow.GetTileAt(position);
            ChangeWidth();
            OpenInnerScheme();
            ChangePlacedBugOrder();
            CreateBugMenu();
            CreateCoppyPaste();
            CreateInOutUpdate();
            ChangeColors();
            InsertBits();

            mainPanel.Changed();
            contextMenuLayer.Show(position, mainPanel);
        }

        private void ChangeWidth()
        {
            if (pBug != null)
                return;
            ContextButton btn = new ContextButton("Change Width");
            btn.Clicked += Btn_Clicked;
            mainPanel.Buttons.Add(btn);
        }

        private void InsertBits()
        {
            if (pBug == null || pBug.Bug.IsUserCreated() == false)
                return;
            if (workplace.Project.Programmability.CodeItems.Count <= 0)
                return;

            ContextPanel panel = CreateDefaultPanel();
            ContextButton btn;
            foreach (Code code in workplace.Project.Programmability.CodeItems)
            {
                btn = new ContextButton(code.Title);
                btn.Clicked += Code_Clicked;
                btn.Tag = code;
                panel.Buttons.Add(btn);
            }
            btn = new ContextButton("<New value>");
            btn.Clicked += Code_Clicked;
            panel.Buttons.Add(btn);

            panel.Changed();
            ContextLinkButton link = new ContextLinkButton(panel, "bits");
            this.mainPanel.Buttons.Add(link);
        }

        private void Code_Clicked(ContextButton sender)
        {
            //Insert bits.
            if (sender.Tag != null)
            {
                Code code = (Code)sender.Tag;
                CodeForm form = new CodeForm(code, workplace, this.pBug, this.pScheme);
            }
            else
            {
                InsertBitsForm form = new InsertBitsForm(workplace, pScheme);
            }            
        }

        private void ChangeColors()
        {
            if (pBug != null)
                return;
            ContextPanel panel = CreateDefaultPanel();
            foreach (NamedColor current in GlobalSettings.TileColors)
            {
                ContextButton btn = new ContextButton(current.Text);
                btn.Clicked += Btn_ColorChange;
                btn.Tag = current;
                panel.Buttons.Add(btn);
            }
            panel.Changed();
            ContextLinkButton link = new ContextLinkButton(panel, "Color");
            this.mainPanel.Buttons.Add(link);
        }

        private void Btn_ColorChange(ContextButton sender)
        {
            workplace.SchemeEventHistory.StartEvent(workplace.CurrentWindow.Scheme, true);
            NamedColor obj = (NamedColor)sender.Tag;
            foreach (Point current in workplace.CurrentWindow.Selection.Items)
                workplace.CurrentWindow.Scheme.SetTileColor(current, obj.ID);
            workplace.SchemeEventHistory.FinalizeEvent();
        }

        private void ChangePlacedBugOrder()
        {
            if (pBug == null)
                return;
            ContextButton btn = new ContextButton("Settings");
            btn.Clicked += OnPlaceBugSettings;
            mainPanel.Buttons.Add(btn);
        }

        private void OnPlaceBugSettings(ContextButton sender)
        {
            PlacedBugSettings form = new PlacedBugSettings(workplace, this.pBug);
        }

        private void CreateInOutUpdate()
        {
            if (info == null)
                return;
            if (info.TileType != TileTypes.Input && info.TileType != TileTypes.Output)
                return;
            ContextButton btn = new ContextButton("Settings");
            btn.Clicked += inOutSettings;
            mainPanel.Buttons.Add(btn);
        }

        private void inOutSettings(ContextButton sender)
        {
            InOutForm form = new InOutForm(workplace, coords);
        }

        private void CreateCoppyPaste()
        {
            ContextButton btn = new ContextButton("Coppy");
            btn.Clicked += Btn_Coppy;
            mainPanel.Buttons.Add(btn);
            btn = new ContextButton("Paste");
            btn.Clicked += Btn_Paste;
            mainPanel.Buttons.Add(btn);
        }

        private void Btn_Paste(ContextButton sender)
        {
            workplace.CoppyAssistant.Paste();
        }

        private void Btn_Coppy(ContextButton sender)
        {
            workplace.CoppyAssistant.Coppy();
        }

        private void AnalyzeSelection(Point position, Selection selection)
        {
            this.pBug = null;
            this.pScheme = null;
            this.tile = null;
            this.info = null;
            coords = workplace.CurrentWindow.GetTileAt(position);
            if (workplace.CurrentWindow.Scheme.ValidateCoords(coords))
            {
                tile = workplace.CurrentWindow.Scheme.Get_Tile(coords);
                if (TilesInfo.IsBugType(tile.Data.Type))
                {
                    pBug = workplace.CurrentWindow.Scheme.PlacedBugs.Get(tile.Data.HorzWidth);
                    if (pBug.Bug.Scheme != null)
                        this.pScheme = workplace.CurrentWindow.PhysScheme.Children[tile.Data.HorzWidth];
                }
                else
                    info = TilesInfo.GetItem(tile.Data.Type);
            }
        }

        private void OpenInnerScheme()
        {
            if (this.pScheme == null)
                return;
            ContextButton btn = new ContextButton("Go in");
            btn.Clicked += btn_GoToScheme;
            mainPanel.Buttons.Add(btn);
        }

        private void btn_GoToScheme(ContextButton sender)
        {
            workplace.OpenWindow(this.pScheme);
        }

        private void CreateBugMenu()
        {
            ContextPanel panel = CreateDefaultPanel();

            List<Bug> list = workplace.Project.SchemeStructure.GetAllowedSchemesIn(workplace.CurrentWindow.Scheme).OrderBy(x => x.Title).ToList();
            foreach (Bug bug in list)
            {
                ContextButton btn = new ContextButton(bug.Title);
                btn.Clicked += insertButtonClicked;
                btn.Tag = bug;
                panel.Buttons.Add(btn);
            }
            panel.Changed();
            ContextLinkButton link = new ContextLinkButton(panel, "Insert");
            this.mainPanel.Buttons.Add(link);
           
        }

        private void insertButtonClicked(ContextButton sender)
        {
            Bug bug = (Bug)sender.Tag;
            workplace.StartInserting(bug);
        }

        private void Btn_Clicked(ContextButton sender)
        {
            ChangeWidthForm form = new ChangeWidthForm(workplace);
        }
    }
}
