using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace FortDefenders
{
    public abstract class ResourceBase
    {
        public delegate void DepositsWorkedOut(Object o);
        public event DepositsWorkedOut OnDepositsWorkedOut;

        private const Single TIME_BEFORE_DESTROYING = 10.0f;
        private readonly Int16 MAX_AMOUNT;
        private readonly Int16 AMOUNT_AT_TIME;
        private readonly Single DELAY_EXTRACTION;

        private ResourcesHUD resourcesHUD;

        private Texture2D curTexture;
        protected Texture2D beginTex;
        protected Texture2D endTex;
        private Single currentDestroyingTime;
        private Single currentDelayBeforeExtraction;
        protected Int16 currentAmount;

        protected BasicEffect effect;
        protected Game game;
        protected Matrix world;
        protected Matrix terrainWorld;
        protected Matrix view;
        protected Matrix projection;

        protected VertexPositionNormalTexture[] vertices;
        private BoundingBox box;
        private Boolean isBoxConstructed;
        private BoundingFrustum frustum;
        protected Boolean canDraw;

        public CellState resourceType { get; protected set; }
        public Point Position { get; set; }
        public Boolean CanExtractResources { get; set; }

        public abstract Int16 GetNeededWorkers();

        public ResourceBase(Int16 maxAmount, Int16 amountAtTime, Single delayBeforeExtraction, Game game, CellState type)
        {
            this.MAX_AMOUNT = maxAmount;
            this.currentAmount = this.MAX_AMOUNT;
            this.currentDestroyingTime = TIME_BEFORE_DESTROYING;
            this.CanExtractResources = false;
            this.AMOUNT_AT_TIME = amountAtTime;
            this.DELAY_EXTRACTION = delayBeforeExtraction;
            this.currentDelayBeforeExtraction = this.DELAY_EXTRACTION;
            this.game = game;
            this.resourceType = type;
            this.effect = new BasicEffect(this.game.GraphicsDevice);
            this.isBoxConstructed = false;
            this.canDraw = false;
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

        public virtual void LoadTextures()
        {
            if (this.beginTex == null)
            {
                throw new Exception("Call the method of the base class after textures loading");
            }

            this.curTexture = this.beginTex;
            this.frustum = new BoundingFrustum(this.view * this.projection);
        }

        public virtual void SetResourcesHUDObject(ref ResourcesHUD resourcesHUD)
        {
            this.resourcesHUD = resourcesHUD;
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

            if (this.currentAmount <= 0)
            {
                this.curTexture = this.endTex;
                this.CanExtractResources = false;
                this.currentDestroyingTime -= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                if (this.currentDestroyingTime <= 0)
                {
                    this.OnDepositsWorkedOutEvent();
                }
            }

            this.ExtractResources(gameTime);
        }

        private void ExtractResources(GameTime gameTime)
        {
            if (this.CanExtractResources)
            {
                this.currentDelayBeforeExtraction -= (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                if (this.currentDelayBeforeExtraction <= 0)
                {
                    this.currentAmount -= this.AMOUNT_AT_TIME;
                    this.currentDelayBeforeExtraction = this.DELAY_EXTRACTION;
                    if (this.resourceType == CellState.Tree)
                    {
                        this.resourcesHUD.SetCurrentResources(0, AMOUNT_AT_TIME, 0, 0);
                    }
                    else
                    {
                        this.resourcesHUD.SetCurrentResources(AMOUNT_AT_TIME, 0, 0, 0);
                    }
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
                this.effect.TextureEnabled = true;
                this.effect.Texture = this.curTexture;

                foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    this.game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertices, 0, 2);
                }
            }
        }

        private void OnDepositsWorkedOutEvent()
        {
            if (this.OnDepositsWorkedOut != null)
            {
                this.OnDepositsWorkedOut(this);
            }
        }

        public virtual Boolean IsDepreserved()
        {
            return ((this.currentAmount == this.MAX_AMOUNT && this.currentAmount > 0) ? false : true);
        }
    }
}
