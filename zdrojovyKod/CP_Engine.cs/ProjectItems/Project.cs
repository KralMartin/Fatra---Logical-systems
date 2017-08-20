using CP_Engine.LoadSaveItems;
using CP_Engine.MapItems;
using CP_Engine.ProjectItems;
using CP_Engine.SchemeItems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace CP_Engine
{
    /// <summary>
    /// Keeps items related to each project. 
    /// </summary>
    public class Project
    {
        /// <summary>
        /// File name without ext.
        /// </summary>
        internal string ProjectName { get; private set; }

        /// <summary>
        /// Full path and file name.
        /// </summary>
        internal string FullProjectName { get; private set; }

        public Programmability Programmability { get; private set; }
        public BugCollection Bugs { get; private set; }
        internal SchemeStructure SchemeStructure { get; private set; }
        internal PhysScheme TopPScheme { get; private set; }
        
        WorkPlace workplace;

        internal Project(WorkPlace workplace)
        {
            this.workplace = workplace;
        }

        /// <summary>
        /// Creates new project. 
        /// 
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="fullProjectName"></param>
        public void New(string projectName, string fullProjectName, string defaultFile)
        {
            //Reset this structure.      
            Reset();
            this.ProjectName = projectName;
            this.FullProjectName = fullProjectName;

            //Create default bug.
            Bug bug = new Bug(this.Bugs.GetBugID());
            bug.Title = this.Bugs.DefaultBugName();
            bug.Description = "";
            //Make it top bug.
            CreateBasicSchemeToBug(bug);
            //Set as top bug.
            SetAsTopBug(bug.Scheme);

            LoadSave ls = new LoadSave();
            ls.Load(workplace, null,defaultFile);
        }

        /// <summary>
        /// Loads project on provided path.
        /// </summary>
        /// <param name="projectName"></param>
        public void Load(string projectName)
        {
            Reset();
            LoadSave ls = new LoadSave();
            ls.Load(workplace, projectName, null);
            this.ProjectName = ls.ProjectName;
            this.FullProjectName = ls.FullProjectName;
        }

        /// <summary>
        /// Resets this structure. 
        /// </summary>
        private void Reset()
        {
            workplace.SchemeEventHistory.Reset();
            this.SchemeStructure = new SchemeStructure(workplace);
            this.Bugs = new BugCollection();
            this.Programmability = new Programmability();
        }

        /// <summary>
        /// Saves currenly opened project.
        /// </summary>
        public void Save()
        {
            LoadSave ls = new LoadSave();
            ls.Save(workplace);
        }

        /// <summary>
        /// Inserts one input and one output into scheme.
        /// </summary>
        /// <param name="scheme"></param>
        private void InsertBase(Scheme scheme)
        {
            TileData data = new TileData();
            data.HorzWidth = 1;
            data.Type = 21;
            scheme.Set_TileData(new Point(1, scheme.TilesCountY / 2), data);
            data.Type = 22;
            scheme.Set_TileData(new Point(scheme.TilesCountX - 2, scheme.TilesCountY / 2), data);
        }

        /// <summary>
        /// Creates Scheme and merges it with provided bug.
        /// </summary>
        /// <param name="bug"></param>
        internal void CreateBasicSchemeToBug(Bug bug)
        {
            //Create new scheme.
            Scheme scheme = new Scheme(workplace, 32, 16, bug);
            workplace.SchemeEventHistory.StartEvent(scheme, false);
            this.SchemeStructure.SchemeCreated(scheme);
            this.Bugs.Add(scheme.Bug);
            //Insert ioputs.
            InsertBase(scheme);
            workplace.SchemeEventHistory.FinalizeEvent();
            workplace.SchemeEventHistory.Reset();
        }

        /// <summary>
        /// Sets provided scheme as top bug.
        /// </summary>
        /// <param name="scheme"></param>
        internal void SetAsTopBug(Scheme scheme)
        {
            //Remove phys representation of current TopScheme.
            if (this.TopPScheme != null)
                this.TopPScheme.PlacedBug.RemoveAllPhysRepres(workplace);

            PlacedBug pBug = new PlacedBug(0, new Point(), scheme.Bug, null);
            pBug.CreatePhysRepre(workplace, null);
            this.TopPScheme = this.SchemeStructure.Get_PhysSchemes(scheme)[0];

            //Open window with top scheme.
            workplace.OpenWindow(this.TopPScheme);
            //Close windows that shos outdated pSchemes.
            workplace.CloseWindows();
        }
    }
}
