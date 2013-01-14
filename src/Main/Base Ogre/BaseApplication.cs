using System;
using System.Collections.Generic;
using Mogre;

using Game.GUICreator;

namespace Game.BaseApp
{
    class ShutdownException : Exception {}
    
    public abstract class BaseApplication
    {
        protected Root          mRoot;
        protected SceneManager  mSceneMgr;
        protected RenderWindow  mWindow;
        protected MoisManager   mInput;
        protected Camera        mCam;
        protected Viewport      mViewport;
        protected MiyagiMgr mMiyagiMgr;
        protected bool          mIsShutDownRequested = false;
        private string          mPluginsCfg          = "plugins.cfg";
        private string          mResourcesCfg        = "resources.cfg";
        private int             mTextureMode         = 0;
        private int             mRenderMode          = 0;
        private Overlay         mDebugOverlay;

        public void Go()
        {
            //try
            {
                if (!this.Setup()) { return; }
                this.mRoot.StartRendering();
                this.Shutdown();
            }
            /*catch (System.Runtime.InteropServices.SEHException e)
            {
                Console.WriteLine(e);

                System.Windows.Forms.MessageBox.Show(
                    "An Ogre error has occurred. Check the Ogre.log file for details", "Exception",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                System.Windows.Forms.MessageBox.Show(
                    e.Message, "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }*/
        }

        private bool Setup()
        {
            this.mRoot = new Root(mPluginsCfg);

            if (!this.Configure())
                return false;

            this.ChooseSceneManager();
            this.CreateCamera();
            this.CreateViewports();

            TextureManager.Singleton.DefaultNumMipmaps = 5;

            this.LoadResources();

            mInput = new MoisManager();
            int windowHnd;
            mWindow.GetCustomAttribute("WINDOW", out windowHnd);
            mInput.Startup(windowHnd, mWindow.Width, mWindow.Height);

            this.mMiyagiMgr = new MiyagiMgr(this.mInput, new Mogre.Vector2(this.mWindow.Width, this.mWindow.Height));
            this.mDebugOverlay = new Overlay(mWindow);
            this.mDebugOverlay.AdditionalInfo = "Bilinear";

            this.Create();
            this.AddFrameLstn(new RootLstn(RootLstn.TypeLstn.FrameRendering, this.OnFrameRendering));

            return true;
        }

        private bool Configure()
        {
            if (this.mRoot.ShowConfigDialog()) { this.mWindow = this.mRoot.Initialise(true, "SkyLands"); return true; }
            else { return false; }
        }

        private void ChooseSceneManager() { this.mSceneMgr = this.mRoot.CreateSceneManager(SceneType.ST_GENERIC); }

        private void CreateCamera()
        {
            this.mCam = this.mSceneMgr.CreateCamera("MainCamera");
            this.mCam.NearClipDistance = 5;
            this.mCam.FarClipDistance = 3000;
        }

        private void CreateViewports()
        {
            this.mViewport = this.mWindow.AddViewport(this.mCam);
            this.mViewport.BackgroundColour = ColourValue.Black;
            this.mCam.AspectRatio = (this.mViewport.ActualWidth / this.mViewport.ActualHeight);
        }

        protected abstract void Create();

        protected abstract void Update(FrameEvent evt);

        private void LoadResources()
        {
            // Load resource paths from config file
            var cf = new ConfigFile();
            cf.Load(mResourcesCfg, "\t:=", true);

            // Go through all sections & settings in the file
            var seci = cf.GetSectionIterator();
            while (seci.MoveNext())
            {
                foreach (var pair in seci.Current)
                {
                    ResourceGroupManager.Singleton.AddResourceLocation(
                        pair.Value, pair.Key, seci.CurrentKey);
                }
            }

            ResourceGroupManager.Singleton.InitialiseAllResourceGroups();
        }

        private void ReloadAllTextures() { TextureManager.Singleton.ReloadAll(); }

        private void ProcessInput()
        {
            this.mInput.Update();

            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_R)) { this.CycleTextureFilteringMode(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_F5)) { this.ReloadAllTextures(); }
            if (mInput.WasKeyPressed(MOIS.KeyCode.KC_SYSRQ)) { this.TakeScreenshot(); }
        }

        private void CycleTextureFilteringMode()
        {
            this.mTextureMode = (this.mTextureMode + 1) % 4;
            switch (this.mTextureMode)
            {
                case 0:
                    MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_BILINEAR);
                    this.mDebugOverlay.AdditionalInfo = "BiLinear";
                    break;

                case 1:
                    MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_TRILINEAR);
                    this.mDebugOverlay.AdditionalInfo = "TriLinear";
                    break;

                case 2:
                    MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_ANISOTROPIC);
                    MaterialManager.Singleton.DefaultAnisotropy = 8;
                    this.mDebugOverlay.AdditionalInfo = "Anisotropic";
                    break;

                case 3:
                    MaterialManager.Singleton.SetDefaultTextureFiltering(TextureFilterOptions.TFO_NONE);
                    MaterialManager.Singleton.DefaultAnisotropy = 1;
                    this.mDebugOverlay.AdditionalInfo = "None";
                    break;
            }
        }

        private void CyclePolygonMode()
        {
            this.mRenderMode = (this.mRenderMode + 1) % 3;
            switch (mRenderMode)
            {
                case 0: mCam.PolygonMode = PolygonMode.PM_SOLID;     break;
                case 1: mCam.PolygonMode = PolygonMode.PM_WIREFRAME; break;
                case 2: mCam.PolygonMode = PolygonMode.PM_POINTS;    break;
            }
        }

        private void TakeScreenshot() { mWindow.WriteContentsToTimestampedFile("screenshot", ".png"); }

        public void AddFrameLstn(RootLstn listener)    { listener.AddListener(this.mRoot); }
        public void RemoveFrameLstn(RootLstn listener) { listener.RemoveListener(this.mRoot); }

        private bool OnFrameRendering(FrameEvent evt)
        {
            if (this.mWindow.IsClosed || this.mIsShutDownRequested) { return false; }
            try
            {
                this.ProcessInput();
                this.mMiyagiMgr.Update();
                this.Update(evt);

                this.mDebugOverlay.Update(evt.timeSinceLastFrame);
                return true;
            }
            catch (ShutdownException)
            { 
                this.mIsShutDownRequested = true;
                return false;
            }
        }

        protected virtual void Shutdown()
        {
            this.mMiyagiMgr.ShutDown();
            this.mInput.Shutdown();
            this.mRoot.Dispose();
        }
    }
}