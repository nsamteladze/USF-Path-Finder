using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Path_Finder.DataTypes;
using System.Threading;
using System.Windows.Threading;

namespace Path_Finder.Source.Algorithms
{
    /// <summary>
    /// This part of the Algorithm class contains methods
    /// which are used to run the algorithm in a separate thread
    /// and communicate with the UI thread
    /// </summary>
    public partial class Algorithm
    {
        #region Constants

        // Sleep time at Slow speed
        private const int SLEEP_TIME = 500;

        #endregion Constants

        #region Data

        // Flag that determines can we continue or we need
        // to wait for some input (e.g. for user to press Start)
        public bool CanContinue
        {
            get;
            set;
        }

        // Mutex to acces CanContinue property
        public Mutex WaitMutex
        {
            get;
            set;
        }

        #endregion Data

        #region Public methods

        /// <summary>
        /// Starts the algorithmic thread
        /// </summary>
        public void RunAlgThread()
        {
            // Block the UI, IsRunning = true
            BlockUI();
            IsRunning = true;

            // Find the path, displaying how the algorithm proceeds
            List<City> optimalPath = this.FindOptimalPath();

            // IsRunning = false, Unblock the UI
            IsRunning = false;
            UnblockUI();         
        }

        #endregion Public methods

        #region Private methods

        /// <summary>
        /// Block buttons in the UI.
        /// Uses dispatcher on the main window for it.
        /// </summary>
        private void BlockUI()
        {
            // If windows is set
            if (Window != null)
            {
                // Call the invoke methods on the object's dispatcher
                Window.graphLayout.Dispatcher.Invoke(
                    // Queue the action on the object
                    new Action(delegate()
                        {
                            Window.graphLayout.IsEnabled = false;
                        }),
                    DispatcherPriority.Normal, null);

                Window.ButtonOpen.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.ButtonOpen.IsEnabled = false;
                    }),
                    DispatcherPriority.Normal, null);

                Window.ButtonReset.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.ButtonReset.IsEnabled = false;
                    }),
                    DispatcherPriority.Normal, null);

                Window.ButtonDefault.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.ButtonDefault.IsEnabled = false;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonDistance.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonDistance.IsEnabled = false;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonHops.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonHops.IsEnabled = false;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonStep.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonStep.IsEnabled = false;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonNormal.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonNormal.IsEnabled = false;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonFast.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonFast.IsEnabled = false;
                    }),
                    DispatcherPriority.Normal, null);
                Window.ButtonClean.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.ButtonClean.IsEnabled = false;
                    }),
                    DispatcherPriority.Normal, null);
            }
        }

        /// <summary>
        /// Unlock buttons in the UI.
        /// Uses dispatcher on the main window for it.
        /// </summary>
        private void UnblockUI()
        {
            if (Window != null)
            {
                Window.graphLayout.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.graphLayout.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);

                Window.ButtonOpen.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.ButtonOpen.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);

                Window.ButtonReset.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.ButtonReset.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);

                Window.ButtonDefault.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.ButtonDefault.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonDistance.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonDistance.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonHops.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonHops.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonStep.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonStep.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonNormal.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonNormal.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);

                Window.RadioButtonFast.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.RadioButtonFast.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);
                Window.ButtonClean.Dispatcher.Invoke(
                    new Action(delegate()
                    {
                        Window.ButtonClean.IsEnabled = true;
                    }),
                    DispatcherPriority.Normal, null);
            }

        }

        /// <summary>
        /// Wait the desired amount of time depending on the chosen speed.
        /// Steps - wait for the user to press Start.
        /// Slow - sleep for SLEEP_TIME milliseconds.
        /// Fast - do not wait.
        /// </summary>
        private void Wait()
        {
            if (AlgSpeed == Alg_Speed.Steps)
            {
                // We get CanContinue property into this variable
                bool waitFlag = true;

                // Busy waiting while we can't continue
                while (waitFlag)
                {
                    WaitMutex.WaitOne();
                    waitFlag = !CanContinue;
                    WaitMutex.ReleaseMutex();
                }

                // We can do only one more step
                WaitMutex.WaitOne();
                CanContinue = false;
                WaitMutex.ReleaseMutex();
            }
            else if (AlgSpeed == Alg_Speed.Slow)
            {
                Thread.Sleep(SLEEP_TIME);
            }
            else if (AlgSpeed == Alg_Speed.Fast)
            {

            }
        }

        #endregion Private methods
    }


}
