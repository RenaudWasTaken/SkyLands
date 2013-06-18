﻿using System.Linq;
using System.Collections.Generic;
using Awesomium.Core;
using Game.BaseApp;
using Game.States;
using Game.World.Generator;
using Mogre;

using Game.World.Blocks;
using API.Geo.Cuboid;

namespace Game.RTS
{
    public abstract class Building
    {
        private const string ghostBlock = "GhostBlock";

        protected StateManager mStateMgr;
        protected Island mIsland;
        protected readonly ConstructionBlock mConstrBlock;
        protected Vector3 mConsBlockPos;
        protected List<Vector3> mClearZone;
        protected byte mColoredBlock;
        protected byte[, ,] mBuilding;
        protected int mYDiff;

        public Faction Faction { get; private set; }
        public Vector3 Position { get; protected set; }
        public Vector3 Size { get; protected set; }
        private Vector3 RealPos { get { return this.Position - Vector3.UNIT_Y * this.mYDiff; } }
        private bool mIsCreated;

        protected Building(StateManager stateMgr, Island island, Vector3 pos)
        {
            this.mStateMgr = stateMgr;
            this.mIsland = island;
            this.mConstrBlock = User.ActConstrBlock;
            this.Faction = User.ActConstrBlock.Faction;
            this.mColoredBlock = (byte) ((this.Faction == Faction.Blue) ? 32 : 31);
            this.Position = pos;
            this.mClearZone = new List<Vector3>();
            this.Init();
            this.mConstrBlock.DrawRemainingRessource();
        }

        protected abstract void Init();
        protected virtual void Create() { this.mIsCreated = true; }

        public void ConfirmBuilding()
        {
            this.Create();
            this.BuildGhost();
        }

        private void ClearScene()
        {
            for (int x = 0; x < this.Size.x; x++)
            {
                for (int y = 0; y < this.Size.y; y++)
                {
                    for (int z = 0; z < this.Size.z; z++)
                    {
                        if (x == 0 && y == 0 && z == 0) { continue; }
                        Vector3 pos = new Vector3(x, y, z);
                        if (this.mBuilding[x, y, z] != 0 || this.mClearZone.Contains(pos) && !(this.mIsland.getBlock(this.RealPos + pos, false) is ConstructionBlock))
                            this.mIsland.removeFromScene(this.RealPos + pos);
                    }
                }
            }
        }

        private void BuildGhost()
        {
            /*for (int x = 0; x < this.Size.x; x++)
            {
                for (int y = 0; y < this.Size.y; y++)
                {
                    for (int z = 0; z < this.Size.z; z++)
                    {
                        Vector3 pos = this.RealPos + new Vector3(x, y, z);
                        if (this.mBuilding[x, y, z] != 0 && (this.mIsland.getBlock(this.RealPos + pos, false) is Air))
                            this.mIsland.addBlockToScene(pos, ghostBlock);
                    }
                }
            }*/
        }

        public void Build()
        {
            if (!this.mIsCreated) { this.Create(); }
            this.ClearScene();

            /*for (int x = 0; x < this.Size.x; x++)
            {
                for (int y = 0; y < this.Size.y; y++)
                {
                    for (int z = 0; z < this.Size.z; z++)
                    {
                        Vector3 pos = this.RealPos + new Vector3(x, y, z);
                        if (this.mBuilding[x, y, z] != 0 && !(this.mIsland.getBlock(pos, false) is ConstructionBlock))
                        {
                            string name = VanillaChunk.staticBlock[VanillaChunk.byteToString[this.mBuilding[x, y, z]]].getName();
                            this.mIsland.addBlockToScene(pos, name);
                        }
                    }
                }
            }*/
            this.OnBuild();
        }

        protected virtual void OnBuild()
        {
            //this.mIsland.removeFromScene(this.Position + this.mConsBlockPos);
            //this.mIsland.addBlockToScene(this.Position + this.mConsBlockPos, "Construction");
            User.ActConstrBlock = null;
            User.RequestBuilderClose = true;
        }

        public void OnDrop(int pos, int newAmount)
        {
            string imgName = GUI.GetImageAt(pos);
            byte b = VanillaChunk.textureToBlock[imgName].getId();
            this.mConstrBlock.RemainingRessources[b] = newAmount;

            if (this.mConstrBlock.RemainingRessources.All(keyValPair => keyValPair.Value <= 0))
                this.mStateMgr.MainState.BuildingMgr.AddBuilding(this.mConstrBlock);
        }
    }
}