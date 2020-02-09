using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace FortDefenders
{
    public abstract class BuildingBase
    {
        public delegate void BuildingConstructed(Object o, CellState type);
        public event BuildingConstructed OnBuildingConstructed;

        protected readonly Single MAX_HEALTH = 100.0f;
        private readonly Single NEXT_STAGE_TIME;

        protected PickingService pickingService;
        protected Texture2D beginTex;
        protected Texture2D halfTex;
        protected Texture2D readyTex;
        protected Single currentHealth;
        protected GameStateForBuilding gameStateBuilding;
        protected Single timeToNextState;
        protected Boolean canStartConstruction;
        protected Texture2D curTexture;
        protected ResourcesHUD resourcesHUD;
        protected SoundEffect currentSound;

        protected AlphaTestEffect effect;
        protected Game game;
        protected Matrix world;
        protected Matrix terrainWorld;
        protected Matrix view;
        protected Matrix projection;

        protected VertexPositionNormalTexture[] vertices;
        protected ConstructionMenu menu;
        private BoundingBox box;
        private Boolean isBoxConstructed;
        private BoundingFrustum frustum;
        protected Boolean canDraw;
        private MenusCaller menusCaller;
        private Boolean paused;

        #region Properties

        public CellState buildingType { get; protected set; }
        public Point Position { get; set; }
        public Int16 NumberOfBuildings { get; set; }
        public BuildingState state { get; set; }
        public Boolean ConstructionPaused
        {
            get
            {
                return this.paused;
            }
            set
            {
                if (value)
                {
                    this.canStartConstruction = false;
                    this.paused = value;
                }
                else
                {
                    this.canStartConstruction = true;
                    this.paused = value;
                }
            }
        }

        #endregion

        public abstract Int16 GetMaxNumberOfBuildings();
        public abstract Int16 GetWorkersNeeded();

        public BuildingBase(Single next_stage_time, Game game, CellState type, Single currentHealth, BuildingState state)
        {
            this.pickingService = game.GetService<PickingService>();
            NEXT_STAGE_TIME = next_stage_time;
            this.timeToNextState = NEXT_STAGE_TIME;
            this.buildingType = type;
            this.game = game;
            this.effect = new AlphaTestEffect(this.game.GraphicsDevice);
            this.state = state;
            this.gameStateBuilding = GameStateForBuilding.Normal;
            this.canStartConstruction = false;
            this.currentHealth = currentHealth;
            this.isBoxConstructed = false;
            this.canDraw = false;
        }

        public virtual void SetResourcesHUDObject(ref ResourcesHUD resourcesHUD)
        {
            this.resourcesHUD = resourcesHUD;
        }

        public virtual void SetVertices(VertexPositionNormalTexture[] vertices)
        {
            this.vertices = vertices;
            Int16 length = Convert.ToInt16(this.vertices.Length);
            //It appears that the manipulation with the DethStencilState class will have a negative effect on the drawing process. 
            //As result, the texture will be drawn below on the same level(Y -coordinate) as the terrain. This will provoke so-called Z -fighting.
            //Thus, the plane with a texture will be drawn a bit above the terrain - Y = 0.01f. Please remember it when overriding this method
            for (Int16 i = 0; i < length; i++)
            {
                this.vertices[i].Position.Y += 0.01f;        
            }
        }

        /// <summary>
        /// This method must be called after menu-object creating. Otherwise, the menu will not work correctly.
        /// </summary>
        public virtual void LoadTextures()
        {
            this.menusCaller = this.game.Components.OfType<MenusCaller>().ElementAt(0);
            this.menu.EnabledChanged += new EventHandler<EventArgs>(menu_EnabledChanged);
            this.frustum = new BoundingFrustum(this.view * this.projection);       
        }

        private void menu_EnabledChanged(Object sender, EventArgs e)
        {
            if (this.menu.Enabled)
            {
                this.pickingService.Enabled = false;
                this.menusCaller.Enabled = false;
                this.menusCaller.Visible = false;
            }
            else
            {
                this.pickingService.Enabled = true;
                this.menusCaller.Enabled = true;
                this.menusCaller.Visible = true;
            }
        }

        protected virtual void SwitchStateNormal()
        {
            switch (this.state)
            {
                case BuildingState.Begin:
                    this.state = BuildingState.Half;
                    this.curTexture = this.halfTex;

                    break;
                case BuildingState.Half:
                    this.state = BuildingState.Ready;
                    this.curTexture = this.readyTex;
                    this.currentHealth = MAX_HEALTH;
                    this.canStartConstruction = false;
                    this.OnBuildingConstructedEvent();

                    break;
                default:
                    this.curTexture = this.readyTex;
                    this.canStartConstruction = false;

                    break;
            }
        }

        public virtual void SetStartConstruction(Boolean canStartConstruct)
        {
            this.canStartConstruction = canStartConstruct;
        }

        public virtual void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection, ref Matrix terrainWorld)
        {
            this.world = world;
            this.view = view;
            this.projection = projection;
            this.terrainWorld = terrainWorld;

            if (!this.isBoxConstructed)
            {
                Vector3 transformedVert = Vector3.Zero;
                Vector3 min = Vector3.Transform(this.vertices[0].Position, this.terrainWorld * this.world);
                Vector3 max = Vector3.Transform(this.vertices[0].Position, this.terrainWorld * this.world);

                for (Int16 i = 0; i < this.vertices.Length; i++)
                {
                    transformedVert = Vector3.Transform(this.vertices[i].Position, this.terrainWorld * this.world);
                    min = Vector3.Min(min, transformedVert);
                    max = Vector3.Max(max, transformedVert);
                }
                this.box = new BoundingBox(min, max);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            this.frustum.Matrix = this.view * this.projection;
            this.canDraw = this.frustum.Intersects(this.box);

            if (this.canStartConstruction)
            {
                this.timeToNextState -= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

                this.currentHealth += (MAX_HEALTH / (2.0f * NEXT_STAGE_TIME * 1000.0f / gameTime.ElapsedGameTime.Milliseconds));

                if (this.timeToNextState <= 0)
                {
                    this.SwitchStateNormal();
                    this.timeToNextState = NEXT_STAGE_TIME;
                }
            }
        }

        public virtual void Draw()
        {
            if (this.canDraw)
            {
                this.effect.World = this.terrainWorld * this.world;
                this.effect.View = this.view;
                this.effect.Projection = this.projection;
              //this.effect.TextureEnabled = true;
                this.effect.Texture = this.curTexture;

                foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertices, 0, 2);
                }
            }
        }

        public virtual void ShowConstructionMenu()
        {
            this.menu.Enabled = true;
            this.menu.Visible = true;
        }

        public virtual Texture2D GetCurrentTexture()
        {
            return this.curTexture;
        }

        public virtual Single GetCurrentHealth()
        {
            return this.currentHealth;
        }

        private void OnBuildingConstructedEvent()
        {
            if (this.OnBuildingConstructed != null)
            {
                this.OnBuildingConstructed(this, this.buildingType);
            }
        }

        protected virtual void PlaySound()
        {
            if (this.canDraw)
            {
                this.currentSound.Play();
            }
        }
    }
}
