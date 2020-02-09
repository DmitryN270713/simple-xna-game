using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders
{
    public sealed class Gates : BuildingBase
    {
        private const Single NEXT_STAGE_TIME = 5.0f;     //5 seconds for testing;
        private const Int16 MAX_NUMBER_OF_BUILDINGS = 0;    //Means as many as needed
        public const Int16 GOLD = 0;
        public const Int16 WOOD = 10;
        public const Int16 STONE = 10;
        public const Int16 NEEDS_HUMANS_TO_BUILD = 0;       //Doesn't need any.

        private Texture2D openGateTex;
        private Texture2D closedGateTex;
        private GateState gateState;

        public GateState OpenCloseState 
        {
            get 
            { 
                return this.gateState; 
            }
            set 
            { 
                this.gateState = (this.gateState == GateState.Opened ? GateState.Closed : GateState.Opened);
                this.OpenCloseGates();
            }
        }

        public Gates(Game game) : base(NEXT_STAGE_TIME, game, CellState.Gate, 100, BuildingState.Ready)
        {
            this.gateState = GateState.Closed;
        }

        public sealed override void LoadTextures()
        {
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/gate/gate_start");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/gate/gate_halfway");
            this.closedGateTex = this.game.Content.Load<Texture2D>("Textures/gate/gate_closed");
            this.openGateTex = this.game.Content.Load<Texture2D>("Textures/gate/gate_opened");
            this.curTexture = this.readyTex = this.closedGateTex;

            //In your own class do the same thing, as it's done below. Use the same order, otherwise some funny bugs shall appear
            GatesMenu gatesMenu = null;
            this.game.RegisterGameComponent<GatesMenu>(out gatesMenu, false, false);
            this.menu = gatesMenu;
            this.menu.SetParent(this);

            base.LoadTextures();
        }

        public sealed override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public sealed override void Draw()
        {
            base.Draw();
        }

        public override short GetMaxNumberOfBuildings()
        {
            return MAX_NUMBER_OF_BUILDINGS;
        }

        private void OpenCloseGates()
        {
            this.curTexture = (this.gateState == GateState.Closed ? this.closedGateTex : this.openGateTex);
        }

        public sealed override short GetWorkersNeeded()
        {
            return NEEDS_HUMANS_TO_BUILD;
        }
    }
}
