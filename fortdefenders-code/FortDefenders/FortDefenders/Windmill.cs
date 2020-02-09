using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using FortDefenders.AnimationHelperClasses;


namespace FortDefenders
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public sealed class Windmill : BuildingBase
    {

        private Int16 AMOUNT_AT_TIME = 1;
        private Single currentDelayBeforeExtraction;

        private const Single NEXT_STAGE_TIME = 5.0f;     //5 seconds for testing;
        private const Int16 MAX_NUMBER_OF_BUILDINGS = 1;
        public const Int16 MAX_NUMBER_OF_PEOPLE_INSIDE = 3;
        private const Single TIME_TEXTURE = 1;
        private const Single TIME_PRODUCE = 1;
        public const Int16 WIDTH = 12;
        public const Int16 HEIGHT = 12;
        public const Int16 GOLD = 0;
        public const Int16 WOOD = 1;
        public const Int16 STONE = 1;
        public const Int16 NEEDS_HUMANS_TO_BUILD = 2;
        
        private Single currentTexTime;
        private WindmillStatesAnim textureCurState;
        private Texture2D texFirstAnim;
        private Texture2D texSecondAnim;
        private Texture2D texThirdAnim;

        //Number of farmers inside
        public Int16 FarmersCounter { get; set; }

        public Windmill(Game game) : base(NEXT_STAGE_TIME, game, CellState.Windmill, 0, BuildingState.Begin)
        {
            this.FarmersCounter = 0;
            this.textureCurState = WindmillStatesAnim.state_first;
        }

        /// <summary>
        /// Must be implemented. Otherwise there are no textures available to draw them on the plane
        /// </summary>
        public sealed override void LoadTextures()
        {
            this.beginTex = this.game.Content.Load<Texture2D>("Textures/Windmill/WindmillTest");
            this.halfTex = this.game.Content.Load<Texture2D>("Textures/house_halfway");
            this.readyTex = this.game.Content.Load<Texture2D>("Textures/Windmill/Mill1finished");
            this.curTexture = this.beginTex;

            this.currentSound = this.game.Content.Load<SoundEffect>("audio/FarmCow");

            //Animation
            this.LoadAnimTextures();

            //In your own class do the same thing, as it's done below. Use the same order, otherwise some funny bugs shall appear
            WindmillMenu windmillMenu = null;
            this.game.RegisterGameComponent<WindmillMenu>(out windmillMenu, false, false);
            this.menu = windmillMenu;
            this.menu.SetParent(this);
            windmillMenu.ConvertParent();

            base.LoadTextures();
        }

        public sealed override void Update(GameTime gameTime)
        {
            this.GenerateFood(gameTime);
            this.SwitchTextures();
            base.Update(gameTime);
        }

        private void GenerateFood(GameTime gameTime)
        {
            this.currentDelayBeforeExtraction -= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            if (this.currentDelayBeforeExtraction <= 0)
            {
                this.currentDelayBeforeExtraction = TIME_PRODUCE;
                if (currentHealth >= MAX_HEALTH)
                {
                    //This is only an example, how the number of workers could be used. 
                    //Fill free to suggest and implement your own method.
                    this.resourcesHUD.SetCurrentResources(0, 0, 0, Convert.ToInt16(AMOUNT_AT_TIME * this.FarmersCounter));
  
                }
            }
        }

        public sealed override void Draw()
        {
            base.Draw();
        }

        public sealed override Int16 GetMaxNumberOfBuildings()
        {
            return MAX_NUMBER_OF_BUILDINGS;
        }

        public sealed override Int16 GetWorkersNeeded()
        {
            return NEEDS_HUMANS_TO_BUILD;
        }

        private void SwitchTextures()
        {
            if (currentHealth >= MAX_HEALTH)
            {
                this.currentTexTime += (this.game.TargetElapsedTime.Milliseconds / 1000.0f);
                if (this.currentTexTime >= TIME_TEXTURE)
                {
                    this.currentTexTime = 0;
                    this.SwitchTextureHelper();
                }


            }
        }

        private void SwitchTextureHelper()
        {
            switch (this.textureCurState)
            { 
                case WindmillStatesAnim.state_first:

                    this.textureCurState = WindmillStatesAnim.state_second;
                    this.curTexture = this.texSecondAnim;

                    break;
                case WindmillStatesAnim.state_second:

                    this.textureCurState = WindmillStatesAnim.state_third;
                    this.curTexture = this.texThirdAnim;

                    break;
                case WindmillStatesAnim.state_third:
                                        
                    this.textureCurState = WindmillStatesAnim.state_first;
                    this.PlaySound();
                    this.curTexture = this.texFirstAnim;

                    break;
                default:
                    this.curTexture = this.beginTex;
                    break;
            }
        }

        private void LoadAnimTextures()
        {
            this.texFirstAnim = this.game.Content.Load<Texture2D>("Textures/Windmill/Mill1finished");
            this.texSecondAnim = this.game.Content.Load<Texture2D>("Textures/Windmill/Mill2finished");
            this.texThirdAnim = this.game.Content.Load<Texture2D>("Textures/Windmill/Mill3finished");
        }
    }
}

