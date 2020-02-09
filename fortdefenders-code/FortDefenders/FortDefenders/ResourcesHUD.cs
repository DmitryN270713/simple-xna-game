using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FortDefenders
{
    public sealed class ResourcesHUD : HUD
    {
        public delegate void PopulationReachesMaximum(Object o);
        public event PopulationReachesMaximum OnPopulationReachedMaximum;

        private const Int16 START_MAX_POPULATION = 50;
        public const Int16 START_POPULATION = 4;
        private const Int16 START_FOOD = 400;
        private const Int16 START_GOLD = 50;
        private const Int16 ICON_WIDTH = 20;
        private const Int16 ICON_HEIGHT = 20;
        private const Int16 LEFT_OFFSET = 10;
        private const Int16 RIGHT_OFFSET = 5;
        private const Int16 TOP_OFFSET = 5;

        private Int16 currentPopulation;
        private Int16 currentGold;
        private Int16 currentStone;
        private Int16 currentWood;          
        private Int16 currentFood;
        private Int16 currentMaxPopulation;

        private Texture2D backgroundTex;

        private Rectangle drawingRectangle;
        private List<ResourceHUDUnit> lsUnit;
        private String[] values;

        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private Object lockObject;

        public ResourcesHUD(Game game) : base(game)
        {
            this.currentPopulation = START_POPULATION;
            this.currentGold = START_GOLD;
            this.currentStone = Stone.START_STONE;
            this.currentWood = Tree.START_WOOD;
            this.currentFood = START_FOOD;
            this.currentMaxPopulation = START_MAX_POPULATION;

            this.drawingRectangle = new Rectangle();
            this.lsUnit = new List<ResourceHUDUnit>();
            this.lockObject = new Object();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            List<Texture2D> tmp = new List<Texture2D>();
            this.values = new String[5];
            this.font = this.Game.Content.Load<SpriteFont>("Fonts/Castellar_HUD");

            this.backgroundTex = this.Game.Content.Load<Texture2D>("Textures/ResourcesHUD/HUD_background");
            Texture2D woodTex = this.Game.Content.Load<Texture2D>("Textures/ResourcesHUD/wood");
            tmp.Add(woodTex);
            values[0] = this.currentWood.ToString();
            Texture2D stoneTex = this.Game.Content.Load<Texture2D>("Textures/ResourcesHUD/stone");
            tmp.Add(stoneTex);
            values[1] = this.currentStone.ToString();
            Texture2D goldTex = this.Game.Content.Load<Texture2D>("Textures/ResourcesHUD/gold");
            tmp.Add(goldTex);
            values[2] = this.currentGold.ToString();
            Texture2D foodTex = this.Game.Content.Load<Texture2D>("Textures/ResourcesHUD/food");
            tmp.Add(foodTex);
            values[3] = this.currentFood.ToString();
            Texture2D populationTex = this.Game.Content.Load<Texture2D>("Textures/ResourcesHUD/population");
            tmp.Add(populationTex);
            values[4] = this.currentPopulation + "/" + this.currentMaxPopulation;
            this.PopulateUnitsListHelper(ref tmp, ref values);

            base.LoadContent();
        }

        private void PopulateUnitsListHelper(ref List<Texture2D> textures, ref String[] texts)
        {
            Single textLength = 0;
            String txt = String.Empty;
            Int16 length = Convert.ToInt16(textures.Count);
            for (Int16 i = 0; i < length; i++)
            { 
                Rectangle rect = this.SetRectangle(Convert.ToInt16(LEFT_OFFSET + textLength + i * ICON_WIDTH + 2 * i * RIGHT_OFFSET),
                                                   TOP_OFFSET, ICON_WIDTH, ICON_HEIGHT);
                txt = texts[i];
                this.PopulateUnitsList(textures[i], rect, txt);
                textLength += this.font.MeasureString(txt).X;
            }
        }

        private void PopulateUnitsList(Texture2D texture, Rectangle rectangle, String text)
        {
            ResourceHUDUnit unit = new ResourceHUDUnit(texture)
            {
                Rect = rectangle,
                Text = text
            };

            this.lsUnit.Add(unit);
        }

        private Rectangle SetRectangle(Int16 X, Int16 Y, Int16 width, Int16 height)
        {
            this.drawingRectangle.X = X;
            this.drawingRectangle.Y = Y;
            this.drawingRectangle.Width = width;
            this.drawingRectangle.Height = height;

            return this.drawingRectangle;
        }

        public override void Update(GameTime gameTime)
        {
            this.values[0] = this.currentWood.ToString();
            this.values[1] = this.currentStone.ToString();
            this.values[2] = this.currentGold.ToString();
            this.values[3] = this.currentFood.ToString();
            this.values[4] = this.currentPopulation + "/" + this.currentMaxPopulation;

            this.UpdateUnitsValues(ref this.values);

            base.Update(gameTime);
        }

        private void UpdateUnitsValues(ref String[] texts)
        {
            Int16 length = Convert.ToInt16(this.lsUnit.Count);
            String txt = String.Empty;
            Single txtLength = 0;
            for (Int16 i = 0; i < length; i++)
            {
                Rectangle rect = this.SetRectangle(Convert.ToInt16(LEFT_OFFSET + txtLength + i * ICON_WIDTH + 2 * i * RIGHT_OFFSET),
                                                   TOP_OFFSET, ICON_WIDTH, ICON_HEIGHT);
                txt = texts[i];
                this.lsUnit[i].Rect = rect;
                this.lsUnit[i].Text = txt;
                txtLength += this.font.MeasureString(txt).X;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.Game.GraphicsDevice.RasterizerState = new RasterizerState()
            {
                FillMode = FillMode.Solid
            };

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, this.Game.GraphicsDevice.RasterizerState);

            this.spriteBatch.Draw(this.backgroundTex, this.SetRectangle(0, 0, Convert.ToInt16(this.Game.GraphicsDevice.Viewport.Width), ICON_HEIGHT + ICON_HEIGHT / 2), Color.White);
            this.DrawIconText();

            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawIconText()
        {
            Int16 length = Convert.ToInt16(this.lsUnit.Count);
            Vector2 pos = Vector2.Zero;
            pos.Y = TOP_OFFSET;
            for (Int16 i = 0; i < length; i++)
            {
                pos.X = lsUnit[i].Rect.X + ICON_WIDTH + RIGHT_OFFSET;
                this.spriteBatch.Draw(this.lsUnit[i].texture, this.lsUnit[i].Rect, Color.White);
                this.spriteBatch.DrawString(this.font, this.lsUnit[i].Text, pos, Color.DarkBlue);
            }
        }

        /// <summary>
        /// Negative value should be sent to decrease resources
        /// </summary>
        /// <param name="val">resources to add/subtract</param>
        /// <param name="type">type of resources</param>
        public Boolean SetCurrentResources(Int16 stone, Int16 wood, Int16 gold, Int16 food)
        {
            lock (this.lockObject)
            {
                if ((this.currentStone + stone) < 0 || (this.currentWood + wood) < 0
                    || (this.currentGold + gold) < 0 || (this.currentFood + food) < 0)
                {
                    return false;
                }

                this.currentStone += stone;
                this.currentWood += wood;
                this.currentGold += gold;
                this.currentFood += food;
            }

            return true;
        }

        /// <summary>
        /// Negative value should be sent to decrease resources
        /// </summary>
        /// <param name="val">resources to add/subtract</param>
        public Boolean SetCurrentPopulation(Int16 val)
        {
            lock (this.lockObject)
            {
                if (this.currentPopulation >= this.currentMaxPopulation)
                {
                    this.OnPopulationReachedMaximumEvent();
                    return false;
                }
                this.currentPopulation += val;
            }
            return true;
        }

        public void SetCurrentMaxPopulation()
        {
            this.currentMaxPopulation += House.ADDED_POPULATION; 
        }

        private void OnPopulationReachedMaximumEvent()
        {
            if (this.OnPopulationReachedMaximum != null)
            {
                this.OnPopulationReachedMaximum(this);
            }
        }

        public Boolean CheckResources(Int16 stone, Int16 wood, Int16 gold, Int16 food)
        {
            if ((this.currentStone + stone) < 0 || (this.currentWood + wood) < 0
                    || (this.currentGold + gold) < 0 || (this.currentFood + food) < 0)
            {
                return false;
            }
            return true;
        }

        public void GetResources(out Int16 wood, out Int16 stone, out Int16 gold, out Int16 food)
        {
            wood = this.currentWood;
            stone = this.currentStone;
            gold = this.currentGold;
            food = this.currentFood;
        }

        public void GetPopulationInfo(out Int16 max, out Int16 current)
        {
            max = this.currentMaxPopulation;
            current = this.currentPopulation;
        }
    }
}
