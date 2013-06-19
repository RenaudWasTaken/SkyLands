﻿using System.Media;
using Game.GUIs;
using Game.csharp.Main.RTS.Buildings;
using Mogre;

using Game.BaseApp;
using Game.Input;
using Game.CharacSystem;
using Game.Shoot;
using Game.RTS;
using Game.World.Display;
using Game.World;

namespace Game.States
{
    public class GameState : MainState
    {
        private BulletManager mBulletMgr;
        private SoundPlayer mSoundPlayer;

        public RTSManager RTSManager { get; private set; }
        public GameState(StateManager stateMgr) : base(stateMgr, "Game") { }

        protected override void Startup()
        {
            if(this.mStateMgr.GameInfo.Type == TypeWorld.Plains)
                this.mSoundPlayer = new SoundPlayer("Media/sounds/dark casle.wav");
            else
                this.mSoundPlayer = new SoundPlayer("Media/sounds/Desert Theme.wav");

            this.World = new MainWorld(this.mStateMgr);
            this.World.setSafeSpawnPoint();

            base.Startup();

            CharacterInfo playerInfo = new CharacterInfo("Sinbad", Faction.Blue, true) { SpawnPoint = this.World.getSpawnPoint() };
            this.CharacMgr.AddCharacter(playerInfo);
            this.User.SwitchFreeCamMode();

            this.CharacMgr.AddCharacter(new CharacterInfo("Robot-01", Faction.Red)
            {
                SpawnPoint = playerInfo.SpawnPoint + new Vector3(800, 500, 200)
            });

            //ParticleGenerator.mkParticle(this.mStateMgr.SceneMgr, this.World.getSpawnPoint(), "MultiEmitters");
        }

        protected override void AfterWorldCreation()
        {
            this.mBulletMgr = new BulletManager(this.mStateMgr.SceneMgr, this.World);
            this.CharacMgr = new CharacMgr(this.mStateMgr, this.World, this.User, this.mBulletMgr);
            this.mBulletMgr.AttachCharacMgr(this.CharacMgr);
            this.RTSManager = new RTSManager(this.mStateMgr);
            this.BuildingMgr = new BuildingManager(this.mStateMgr, this.World.getIsland(), this.RTSManager);
            this.User.BuildingMgr = this.BuildingMgr;
        }

        public override void Show()
        {
            base.Show();
            this.mSoundPlayer.PlayLooping();

        }

        public override void Hide()
        {
            base.Hide();
            this.mSoundPlayer.Stop();
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            this.RTSManager.Update(frameTime);
            this.mBulletMgr.Update(frameTime);

            User user = this.mStateMgr.MainState.User;
            VanillaPlayer mainPlayer = this.mStateMgr.MainState.CharacMgr.MainPlayer;
            if (user.IsFreeCamMode && mainPlayer != null && !GUI.Visible)
            {
                bool ctrlPressed = this.mStateMgr.Controller.IsKeyDown(MOIS.KeyCode.KC_LCONTROL);
                mainPlayer.SetIsAllowedToMove(ctrlPressed, false);
                user.IsAllowedToMoveCam = !ctrlPressed;
            }

            if (this.mStateMgr.Controller.WasKeyPressed(MOIS.KeyCode.KC_F1) && !GUI.Visible)
                user.SwitchFreeCamMode();
        }

        public override void Save() { this.World.save(this.CharacMgr.MainPlayer); }

        public override void OpenMainGUI()
        {
            InGameMenu.Save = this.Save;
            new InGameMenu(this.mStateMgr);
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            this.mSoundPlayer.Stop();
        }
    }
}
