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
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics;
using FortDefenders.PersonHelperClasses;


namespace FortDefenders
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ResourcesParent : DrawableGameComponent
    {
        private delegate T CreateObject<T>(Game game);

        //for the sake of simplicity, both stonemines and groves shall have a permanent location on the map. 
        //Remember, this is only a demo-version of the game. Moreover, I want to have a nap sometimes and may be eat something.
        private const Int16 MAX_MINE_NUMBER_Z = 5;
        private const Int16 MAX_MINE_NUMBER_X = 7;
        private const Int16 MAX_GROVE_NUMBER_Z = 15;
        private const Int16 MAX_GROVE_NUMBER_X = 4;
        private const Int16 EMPTY_SPOTS_NUMBER_STONES = 4;
        private const Int16 EMPTY_SPOTS_NUMBER_TREES = 5;
        private readonly Point startStonePoint1;
        private readonly Point startStonePoint2;
        private readonly Point startTreesPoint1;
        private readonly Point startTreesPoint2;
        private readonly Point startTreesPoint3;

        private Matrix terrainWorld;
        private ContentManager Content;
        private GameField gameField;
        private List<ResourceBase> listOfResources;
        private VertexPositionNormalTexture[] currentResourceVertices;
        private Random rndRes;
        private ResourcesHUD resourcesHUD;

        private Mayor mayorObject;

        
        
        public ResourcesParent(Game game)
            : base(game)
        {
            this.Content = this.Game.Content;
            this.listOfResources = new List<ResourceBase>();
            this.currentResourceVertices = new VertexPositionNormalTexture[6];
            this.startStonePoint1 = new Point(5, 1);
            this.startStonePoint2 = new Point(50, 2);
            this.startTreesPoint1 = new Point(61, 20);
            this.startTreesPoint2 = new Point(1, 25);
            this.startTreesPoint3 = new Point(15, 60);
            this.rndRes = new Random();
        }

        public void SetParent(Object parent, ref ResourcesHUD resourcesHUD)
        {
            this.gameField = parent as GameField;  
            this.gameField.OnResourceClicked += new GameField.ResourceClicked(gameField_OnResourceClicked);

            this.resourcesHUD = resourcesHUD;
        }

        private void gameField_OnResourceClicked(Object o, NotEmptyArgs e)
        {
            var result = from target in this.listOfResources
                         where (target.resourceType == e.State && !target.CanExtractResources)
                         select target;

            foreach (ResourceBase r in result)
            {
                if (r.Position == e.Position)
                {
                    if (!this.CheckWorkers(r.GetNeededWorkers(), e.State, false))
                        break;

                    r.CanExtractResources = true;
                    break;
                }
            }
        }

        private Boolean CheckWorkers(Int16 neededWorkers, CellState state, Boolean isMenuShown)
        {
            Boolean result = this.mayorObject.GetFreeUnits(neededWorkers, PrivilegesCosts.CommonWorker);
            if (result)
            {
                if (state == CellState.Tree)
                {
                    this.mayorObject.SendLumberjacks(neededWorkers);
                }
                else if (state == CellState.Stone)
                {
                    this.mayorObject.SendMiners(neededWorkers);
                }
                return result;
            }
            else
            {
                if (!isMenuShown)
                {
                    this.mayorObject.ShowMessageBox();
                }
            } 
            return result;
        }

        public void SetTerrainWorld(ref Matrix terrainWorld)
        {
            this.terrainWorld = terrainWorld;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        public void AddResources()
        {
            this.GenerateResources<Stone>(this.startStonePoint1, MAX_MINE_NUMBER_X, MAX_MINE_NUMBER_Z, Stone.STONE_WIDTH, Stone.STONE_HEIGHT, EMPTY_SPOTS_NUMBER_STONES, CellState.Stone);
            this.GenerateResources<Stone>(this.startStonePoint2, MAX_MINE_NUMBER_X, MAX_MINE_NUMBER_Z, Stone.STONE_WIDTH, Stone.STONE_HEIGHT, EMPTY_SPOTS_NUMBER_STONES, CellState.Stone);
            this.GenerateResources<Tree>(this.startTreesPoint1, MAX_GROVE_NUMBER_X, MAX_GROVE_NUMBER_Z, Tree.TREE_WIDTH, Tree.TREE_HEIGHT, EMPTY_SPOTS_NUMBER_TREES, CellState.Tree);
            this.GenerateResources<Tree>(this.startTreesPoint2, MAX_GROVE_NUMBER_X, (MAX_GROVE_NUMBER_Z - 7), Tree.TREE_WIDTH, Tree.TREE_HEIGHT, EMPTY_SPOTS_NUMBER_TREES, CellState.Tree);
            this.GenerateResources<Tree>(this.startTreesPoint3, (MAX_GROVE_NUMBER_Z - 5), MAX_GROVE_NUMBER_X, Tree.TREE_WIDTH, Tree.TREE_HEIGHT, EMPTY_SPOTS_NUMBER_TREES, CellState.Tree);

            //Changing celss status to forbidden
            Int16 verticalLength = (GameField.MAP_WIDTH - BuildingsParent.WALL_WIDTH) / 2;
            Int16 backZ = GameField.MAP_WIDTH + BuildingsParent.WALL_PART_HEIGHT - (GameField.MAP_WIDTH - BuildingsParent.WALL_WIDTH) / 2;
            Int16 bottomHeight = (GameField.MAP_HEIGHT - BuildingsParent.WALL_HEIGHT) / 2;
            Int16 topX = GameField.MAP_HEIGHT - (GameField.MAP_HEIGHT - BuildingsParent.WALL_HEIGHT) / 2;
            this.AddForbidden(0, 0, (GameField.MAP_HEIGHT - 1), verticalLength);
            this.AddForbidden(0, backZ, (GameField.MAP_HEIGHT - 1), (GameField.MAP_WIDTH - 1));
            this.AddForbidden(0, 0, bottomHeight, (GameField.MAP_WIDTH - 1));
            this.AddForbidden(topX, 0, (GameField.MAP_HEIGHT - 1), (GameField.MAP_WIDTH - 1));
        }

        /// <summary>
        /// Faster alternative to Activator.CreateInstance()
        /// </summary>
        /// <typeparam name="T">ResourceBase</typeparam>
        /// <param name="startPoint">start position</param>
        /// <param name="maxX">max amount along virtual X</param>
        /// <param name="maxZ">max amount along virtual Z</param>
        private void GenerateResources<T>(Point startPoint, Int16 maxX, Int16 maxZ, Int16 objectWidth, 
                                          Int16 objectHeight, Int16 emptySpotsAmount, CellState type) where T : ResourceBase
        {
            Int16 emptySpot = Convert.ToInt16(this.rndRes.Next(MAX_MINE_NUMBER_X - 1));
            Int16 count = 0;

            ConstructorInfo ctor = typeof(T).GetConstructors().First();
            ParameterExpression arg = Expression.Parameter(typeof(Game), "game");
            NewExpression expression = Expression.New(ctor, arg);
            LambdaExpression lambda = Expression.Lambda(typeof(CreateObject<T>), expression, arg);
            CreateObject<T> instantiate = (CreateObject<T>)lambda.Compile();

            for (Int16 i = 0; i < maxZ; i++)
            {
                for (Int16 j = 0; j < maxX; j++)
                {
                    Int16 X = Convert.ToInt16(startPoint.X + j * objectHeight);
                    Int16 Z = Convert.ToInt16(startPoint.Y + i * objectWidth);
                    Int16 lastX = Convert.ToInt16(X + objectHeight - 1);
                    Int16 lastZ = Convert.ToInt16(Z + objectWidth - 1);

                    if (count < emptySpotsAmount && j == emptySpot)
                    {
                        count++;
                        emptySpot = Convert.ToInt16(this.rndRes.Next(maxX - 1));
                        continue;
                    }

                    this.gameField.MarkAddResources(X, Z, type, lastX, lastZ, out this.currentResourceVertices);
                    this.InstantiateResourceObject(X, Z, instantiate);
                }
            } 

        }

        private void InstantiateResourceObject<T>(Int16 X, Int16 Z, CreateObject<T> instantiate) where T : ResourceBase
        {
            T resource = instantiate(this.Game);
            resource.LoadTextures();
            resource.SetResourcesHUDObject(ref this.resourcesHUD);
            resource.SetVertices(this.currentResourceVertices);
            resource.Position = new Point(X, Z);
            resource.OnDepositsWorkedOut += new ResourceBase.DepositsWorkedOut(resource_OnDepositsWorkedOut);
            this.listOfResources.Add(resource);
        }

        private void resource_OnDepositsWorkedOut(Object o)
        {
            ResourceBase resource = o as ResourceBase;
            this.listOfResources.Remove(resource);
            if (resource.resourceType == CellState.Tree)
            {
                this.mayorObject.ReleaseLumberjacks(resource.GetNeededWorkers());
            }
            else if (resource.resourceType == CellState.Stone)
            {
                this.mayorObject.ReleaseMiners(resource.GetNeededWorkers());
            }
        }

        private void AddForbidden(Int16 X, Int16 Z, Int16 maxX, Int16 maxZ)
        {
            for (Int16 i = Z; i < maxZ; i++)
            {
                for (Int16 j = X; j < maxX; j++)
                {
                    this.gameField.AddForbidden(j, i);
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            for (Int16 i = 0; i < this.listOfResources.Count; i++)
            {
                this.listOfResources[i].Update(gameTime);
            }

                base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.listOfResources.Count > 0)
            {
                this.Game.GraphicsDevice.RasterizerState = new RasterizerState()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.CullClockwiseFace
                };

                this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                for (Int16 i = 0; i < this.listOfResources.Count; i++)
                {
                    this.listOfResources[i].Draw();
                }
            }
            base.Draw(gameTime);
        }

        public void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection)
        {
            if (this.listOfResources.Count > 0)
            {
                foreach (ResourceBase r in this.listOfResources)
                {
                    r.SetMatrices(ref world, ref view, ref projection, ref this.terrainWorld);
                }
            }
        }

        public void SetMayorObject(ref Mayor mayorObject)
        {
            this.mayorObject = mayorObject;
            this.mayorObject.OnRecallLumberjacks += new Mayor.RecallLumberjacks(mayorObject_OnRecallLumberjacks);
            this.mayorObject.OnAddLumberjacks += new Mayor.AddLumberjacks(mayorObject_OnAddLumberjacks);
            this.mayorObject.OnRecallMiners += new Mayor.RecallMiners(mayorObject_OnRecallMiners);
            this.mayorObject.OnAddMiners += new Mayor.AddMiners(mayorObject_OnAddMiners);
        }

        private void mayorObject_OnAddMiners(Object sender)
        {
            var result = from target in this.listOfResources
                         where (!target.CanExtractResources && target.IsDepreserved()
                                && target.resourceType == CellState.Stone)
                         select target;
            if (result.Count() > 0)
            {
                ResourceBase r = result.ElementAt(0);
                Int16 workers = r.GetNeededWorkers();
                if (!this.CheckWorkers(workers, CellState.Stone, true))
                    return;
                r.CanExtractResources = true;
            }
        }

        private void mayorObject_OnRecallMiners(Object sender)
        {
            var result = from target in this.listOfResources
                         where (target.CanExtractResources && target.resourceType == CellState.Stone)
                         select target;
            if (result.Count() > 0)
            {
                ResourceBase r = result.ElementAt(0);
                r.CanExtractResources = false;
                this.mayorObject.ReleaseMiners(r.GetNeededWorkers());
            }
        }

        private void mayorObject_OnAddLumberjacks(Object sender)
        {
            var result = from target in this.listOfResources
                         where (!target.CanExtractResources && target.IsDepreserved()
                                && target.resourceType == CellState.Tree)
                         select target;
            if (result.Count() > 0)
            {
                ResourceBase r = result.ElementAt(0);
                Int16 workers = r.GetNeededWorkers();
                if (!this.CheckWorkers(workers, CellState.Tree, true))
                    return;
                r.CanExtractResources = true;
            }
        }

        private void mayorObject_OnRecallLumberjacks(Object sender)
        {
            var result = from target in this.listOfResources
                         where (target.CanExtractResources && target.resourceType == CellState.Tree)
                         select target;
            if (result.Count() > 0)
            {
                ResourceBase r = result.ElementAt(0);
                r.CanExtractResources = false;
                this.mayorObject.ReleaseLumberjacks(r.GetNeededWorkers());
            }
        }
    }
}
