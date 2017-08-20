using CP_Engine.PathFinderItems;
using CP_Engine.SchemeItems;
using CP_Engine.SimulationItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace CP_Engine
{
    public class Simulation
    {
        public delegate void ClickEventHandler(Simulation sender);
        public event ClickEventHandler Clicked;

        /// <summary>
        /// Determines whether simulation is running or not.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Determines whether simulation is really running.
        /// Simulation can be running from user perspective,
        /// but simulation was paused due to scheme reorganization.
        /// </summary>
        internal bool IsThreadRunning { get; private set; }

        /// <summary>
        /// Events of simulation.
        /// </summary>
        internal SimEventCollection Events { get; private set; }

        /// <summary>
        /// Current step of simulation.
        /// </summary>
        internal int Step { get; private set; }

        internal List<BreakPointResult> BreakPointResults { get; private set; }

        WorkPlace workplace;
        Thread thread;                  //Thead, in which simulation is running
        long stepWillEndAt;             //Real time, when one simulation step can end (step can end later, but not sooner).
        long stepLenght;                //Real lenght of simulation step in miliseconds.
        Stopwatch watch;                //Time measurment.
        private Object simLock;
        private bool ignoreBreakPoints; //Ignore breakpoints when simulation is heating up

        internal Simulation(WorkPlace workplace)
        {
            this.workplace = workplace;
            this.Events = new SimEventCollection(this);
            simLock = new object();
            IsThreadRunning = false;
            IsRunning = false;
            this.Step = 0;
            stepLenght = 0;
            this.BreakPointResults = new List<BreakPointResult>();
            watch = new Stopwatch();
            watch.Start();
        }

        internal void Heat()
        {
            UpdateSchemes();
            ignoreBreakPoints = true;
            for (int i = 0; i < 100; i++)
                DoStep();
            ignoreBreakPoints = false;
        }

        /// <summary>
        /// Executes one step of simulation.
        /// </summary>
        public void DoStep()
        {
            lock (simLock)
            {
                if (IsRunning == false)
                {
                    ExecuteStep();
                }
            }
        }

        /// <summary>
        /// Start simulate, if it is not running.
        /// </summary>
        public void Start()
        {
            stepLenght = 0;
            if (IsRunning == false)
            {
                IsRunning = true;
                StartThread();
                if (Clicked != null)
                    Clicked(this);
            }
        }

        /// <summary>
        /// End simulate, if it is running.
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                StopThread();
                if (Clicked != null)
                    Clicked(this);
            }
        }

        /// <summary>
        /// Really starts simulation. 
        /// This function affects IsThreadRunning property.
        /// </summary>
        internal void StartThread()
        {
            lock (simLock)
            {
                //Find paths in each uninitialized scheme.
                UpdateSchemes();
                //Start simulation, if simulation is running from user's perspective.
                if (IsRunning)
                {
                    if (IsThreadRunning == false)
                    {
                        IsThreadRunning = true;
                        thread = new Thread(Run);
                        thread.Start();
                    }
                    else
                    {
                        throw new Exception("nemozem pustut ked ide");
                    }
                }
            }
        }

        /// <summary>
        /// Really stop simulation
        /// This function affects IsThreadRunning property.
        /// </summary>
        internal void StopThread()
        {
            lock (simLock)
            {
                if (IsThreadRunning)
                {
                    IsThreadRunning = false;
                    thread.Join();
                }
            }
        }

        /// <summary>
        /// Finds paths in all schemes.
        /// </summary>
        private void UpdateSchemes()
        {
            foreach (Bug bug in this.workplace.Project.Bugs.GetItems())
            {
                if (bug.IsUserCreated())
                {
                    PathFinder finder = new PathFinder(workplace);
                    finder.FindPaths(bug.Scheme.Sources.UninitializedSources);
                    bug.Scheme.Sources.UninitializedSources.Clear();
                }
            }
        }

        /// <summary>
        /// Function executed by simulation thread.
        /// </summary>
        private void Run()
        {
            while (IsThreadRunning)
            {
                //Calculate when can simulation step end.
                stepWillEndAt = watch.ElapsedMilliseconds + stepLenght;
                //Execute step.
                ExecuteStep();
                //If there is still remaining time, wait.
                long ellapsedMilliseconds = watch.ElapsedMilliseconds;
                while (ellapsedMilliseconds < stepWillEndAt)
                {
                    if (IsThreadRunning == false)
                        return;
                    var willsleep = (int)(stepWillEndAt - ellapsedMilliseconds);
                    Thread.Sleep(Math.Min(100, (int)(stepWillEndAt - ellapsedMilliseconds)));
                    ellapsedMilliseconds = watch.ElapsedMilliseconds;
                }
            }
        }

        /// <summary>
        /// Executes one simulation step.
        /// </summary>
        private void ExecuteStep()
        {
            //Simulation step is divided to 2 substeps.
            //SubStep 1
            //Actually change value of SSources and acknowledge paths, that value has been changed.
            //Paths(SSource.IsInputIn) will be inserted to simulation and will be executed in next simulation step.
            List<EventChangeValue> ecvs = this.Events.GetEcvs();
            foreach (EventChangeValue ecv in ecvs)
                ecv.Execute(this);

            //SubStep 2
            //Change value of Paths and acknowledge Path.Outputs, that value has been changed.
            //SSources(Path.Outputs) will create Event, that will change their value in next step.
            List<PhysPath> paths = this.Events.GetPats();
            foreach (PhysPath pPath in paths)
                pPath.Execute(this);

            //Check breakpoints
            CheckBreakPoints();
            this.Step++;
        }

        /// <summary>
        /// Checks whether values in tiles, that contain BreakPoint, were changed.
        /// </summary>
        private void CheckBreakPoints()
        {
            this.BreakPointResults.Clear();
            foreach (Bug bug in workplace.Project.Bugs.GetItems())
            {
                if (bug.IsUserCreated())
                {
                    bug.Scheme.BreakPoints.CheckBreakPoints(this.BreakPointResults);
                }
            }
            if (ignoreBreakPoints==false && BreakPointResults.Count > 0)
            {
                lock (simLock)
                {
                    IsRunning = false;
                    IsThreadRunning = false;
                    if (Clicked != null)
                        Clicked(this);
                }
            }
        }

    }
}
