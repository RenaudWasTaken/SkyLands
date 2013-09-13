﻿using System.Collections.Generic;
using API.Generic;
using Game.CharacSystem;
using Game.World.Generator;
using Mogre;

using API.Geo.Cuboid;
using Game.States;

namespace Game.RTS
{
    public class RobotFactory : Building
    {
        public const int ROBOT_COST = 30;
        
        private const int sizeX = 5, sizeY = 6, sizeZ = 5;

        private Vector3 mSpawnPoint;  // Absolute coord
        private Timer mTimeSinceLastPop;

        public bool CanCreateUnit { get { return this.mTimeSinceLastPop.Milliseconds >= 8000; } }

        public RobotFactory(StateManager stateMgr, Island island, VanillaRTS rts) : base(stateMgr, island, rts, "RF") { }
        public RobotFactory(StateManager stateMgr, Island island, VanillaRTS rts, Vector3 position)
            : base(stateMgr, island, rts, "RF", position)
        {
        }
        protected override void Init()
        {
            this.Size = new Vector3(sizeX, sizeY, sizeZ);
            this.mBuilding = new byte[sizeX, sizeY, sizeZ];
            this.mYDiff = 1;
        }

        protected override void Create()
        {
            base.Create();
            const byte smoothStone = 20;

            for (int y = 0; y < 2; y++)
                for (int x = 0; x < sizeX; x++)
                    for (int z = 0; z < sizeZ; z++)
                        this.mBuilding[x, y, z] = smoothStone;

            for (int y = 2; y < sizeY; y++)
                for (int x = 1; x < sizeX - 1; x++)
                    for (int z = 1; z < sizeZ - 1; z++)
                        this.mBuilding[x, y, z] = smoothStone;

            for (int y = 2; y < 5; y++)
                this.mBuilding[2, y, 2] = 0;

            for (int y = 2; y < 5; y++)
                this.mBuilding[2, y, 1] = this.mColoredBlock;

            this.mClearZone = new List<Vector3>();
            for (int y = 2; y < 5; y++)
                for (int x = 1; x < sizeX - 1; x++)
                    this.mClearZone.Add(new Vector3(x, y, 0));
        }

        protected override void OnBuild()
        {
            base.OnBuild();
            this.mSpawnPoint = (this.Position + new Vector3(2, 1, 2)) * Cst.CUBE_SIDE;
            this.mTimeSinceLastPop = new Timer();
            this.RTS.NbFactory++;
        }

        public void CreateUnit()    // Called by VanillaRTS only
        {
            if (this.CanCreateUnit)
            {
                this.RTS.Crystals -= ROBOT_COST;
                this.RTS.AmountUnits++;
                this.mStateMgr.MainState.CharacMgr.AddCharacter(new CharacterInfo("Robot", this.RTS.Faction)
                {
                    SpawnPoint = this.mSpawnPoint
                });
                this.mTimeSinceLastPop.Reset();
            }
        }
    }
}
