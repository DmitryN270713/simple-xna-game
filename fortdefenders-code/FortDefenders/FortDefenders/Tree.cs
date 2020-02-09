using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FortDefenders
{
    public class Tree : ResourceBase
    {
        public const Int16 TREE_WIDTH = 2;
        public const Int16 TREE_HEIGHT = 2;
        public const Int16 START_WOOD = 30;
        private const Int16 NEEDED_WORKERS = 1;

        private const Int16 MAX_AMOUNT = 300;
        private const Int16 AMOUNT_AT_TIME = 3;
        private const Single DELAY_EXTRACTION = 0.5f;
        private readonly TreeType treeType;


        public Tree(Game game) : base(MAX_AMOUNT, AMOUNT_AT_TIME, DELAY_EXTRACTION, game, CellState.Tree)
        {
            Random rnd = new Random();
            Int16 tmp = Convert.ToInt16(rnd.Next(0, 10));
            if (tmp >= 5)
            {
                this.treeType = TreeType.Conifer;
            }
            else
            {
                this.treeType = TreeType.Deciduous;
            }
        }

        public override void LoadTextures()
        {
            switch (this.treeType)
            { 
                case TreeType.Conifer:
                    this.beginTex = this.game.Content.Load<Texture2D>("Textures/resources/BirchTreeFinished");
                    break;
                case TreeType.Deciduous:
                    this.beginTex = this.game.Content.Load<Texture2D>("Textures/resources/tree_normal_2");
                    break;
                default:
                    break;
            }
            this.endTex = this.game.Content.Load<Texture2D>("Textures/resources/tree_end");

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

        public override short GetNeededWorkers()
        {
            return NEEDED_WORKERS;
        }
    }
}
