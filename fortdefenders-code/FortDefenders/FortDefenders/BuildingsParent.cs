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
using System.Reflection;
using System.Linq.Expressions;
using FortDefenders.PersonHelperClasses;


namespace FortDefenders
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BuildingsParent : DrawableGameComponent
    {
        public const Int16 WALL_WIDTH = 50;
        public const Int16 WALL_HEIGHT = 50;
        private const Int16 WALL_PART_WIDTH = 10;
        public const Int16 WALL_PART_HEIGHT = 2;

        private readonly Point wallStartBottomPos;
        private readonly Point wallStartTopPos;

        private Matrix terrainWorld;
        private CellState buildingTypeToBuild;
        private GameField gameField;
        private List<BuildingBase> listOfBuildings;
        private VertexPositionNormalTexture[] currentBuildingVertices;
        private Dictionary<CellState, TypesOfBuildings> listOfTypes;
        private Int16 neededWorkers;
        private Boolean isAboutToPauseConstruction;
        private Boolean isAboutToProceed;
        private Texture2D msgBoxTex;

        private ResourcesHUD resourcesHUD;
        private Mayor mayorObject;


        public BuildingsParent(Game game) : base(game)
        {
            this.listOfBuildings = new List<BuildingBase>();
            this.currentBuildingVertices = new VertexPositionNormalTexture[6];
            this.wallStartBottomPos = new Point(10, 10);
            this.wallStartTopPos = new Point(10, 58);

            this.CheckConstants();

            this.listOfTypes = BuildingsParentHelper.GetListOfTypesOfBuildings(this.ConstructBuilding, 
                               this.CheckAvailability, this.IsEnoughWorkers, this.FreeWorkers);
            this.isAboutToPauseConstruction = false;
            this.isAboutToProceed = false;
        }

        private void CheckConstants()
        {
            if (WALL_HEIGHT != WALL_WIDTH || (WALL_HEIGHT % 10) != 0 || (WALL_WIDTH % 10) != 0)
            {
                throw new Exception("The wall's width and height must be equal. Also the height and the width must be devided by 10 without a remainder");
            }

            if ((WALL_WIDTH % WALL_PART_WIDTH) != 0)
            {
                throw new Exception("The width of the wall must be devided by the width of the wall's part without a remainder");
            }
        }

        public void SetParent(Object parent, ref ResourcesHUD resourcesHUD)
        {
            this.gameField = parent as GameField;   //Yeah, I know, we can pass GameField -object directly, but it's not fun at all
            this.gameField.OnNotEmptyClicked += new GameField.NotEmptyClicked(gameField_OnNotEmptyClicked);
            this.resourcesHUD = resourcesHUD;
        }

        private void gameField_OnNotEmptyClicked(Object o, NotEmptyArgs e)
        {
            if (e.State == CellState.House || e.State == CellState.Wall)
            {
                this.ParseHouseWallType(e.Position, e.State);
            }
            else
            {
                this.ParseOtherTypes(e.State, e.Position);
            }
        }

        private void ParseHouseWallType(Point pos, CellState type)
        {
            if (!this.isAboutToPauseConstruction && !this.isAboutToProceed)
            {
                var result = from target in this.listOfBuildings
                             where target.buildingType == type
                             select target;

                foreach (BuildingBase b in result)
                {
                    if (b.Position == pos)
                    {
                        b.ShowConstructionMenu();
                        break;
                    }
                }
            }
            else if (this.isAboutToPauseConstruction && !this.isAboutToProceed)
            {
                this.PauseConstruction(pos);
            }
            else if (!this.isAboutToPauseConstruction && this.isAboutToProceed)
            {
                this.ProceedConstruction(pos);
            }
        }

        /// <summary>
        /// Every other construction could be built once.
        /// </summary>
        /// <param name="type">type of the construction</param>
        private void ParseOtherTypes(CellState type, Point pos)
        {
            if (!this.isAboutToPauseConstruction && !this.isAboutToProceed)
            {
                var result = from target in this.listOfBuildings
                             where target.buildingType == type
                             select target;

                foreach (BuildingBase b in result)
                {
                    b.ShowConstructionMenu();
                }
            }
            else if (this.isAboutToPauseConstruction && !this.isAboutToProceed)
            {
                this.PauseConstruction(pos);
            }
            else if (!this.isAboutToPauseConstruction && this.isAboutToProceed)
            {
                this.ProceedConstruction(pos);
            }
        }

        private void PauseConstruction(Point pos)
        {
            var result = from target in this.listOfBuildings
                         where (target.Position == pos && target.state != BuildingState.Ready)
                         select target;
            if (result.Count() > 0)
            {
                BuildingBase b = result.ElementAt(0);
                b.ConstructionPaused = true;
                this.mayorObject.ReleaseBuilders(b.GetWorkersNeeded());
            }
            else
            {
                MessageBox msgBox = null;
                MessageBox.Show(this.Game, "The construction process cannot be\n interrupted",
                                "Mayor said: ", MessageBoxButton.OK, this.mayorObject.FaceTex,
                                out msgBox);
            }
            this.isAboutToPauseConstruction = false;
        }

        private void ProceedConstruction(Point pos)
        {
            var result = from target in this.listOfBuildings
                         where (target.Position == pos && target.state != BuildingState.Ready)
                         select target;
            if (result.Count() > 0)
            {
                BuildingBase b = result.ElementAt(0);
                if (!this.mayorObject.GetFreeUnits(b.GetWorkersNeeded(), PrivilegesCosts.PrivilegedWorker))
                {
                    this.mayorObject.ShowMessageBox();
                    this.isAboutToProceed = false;
                    return;
                }
                this.mayorObject.SendBuilders(b.GetWorkersNeeded());
                b.ConstructionPaused = false;
            }
            else
            {
                MessageBox msgBox = null;
                MessageBox.Show(this.Game, "Building is ready",
                                "Mayor said: ", MessageBoxButton.OK, this.mayorObject.FaceTex,
                                out msgBox);
            }
            this.isAboutToProceed = false;
        }

        public void ResetAddWorkersRecall()
        {
            this.isAboutToPauseConstruction = false;
            this.isAboutToProceed = false;
        }

        /// <summary>
        /// Castel is 50 x 50 units. Thus to decrease memory usage, every wall will be 1 unit wide and 10 units long.
        /// In other words there shall be only 20 objects to draw and update
        /// </summary>
        public void InitializeWalls()
        {
            Int16 count = WALL_WIDTH / WALL_PART_WIDTH;
            //Constructing horizontal walls
            this.InitializeHorizontalWall(count);
            //Constructing vertical walls
            this.InitializeVerticalWall(count);
        }

        private void InitializeVerticalWall(Int16 count)
        {
            for (Int16 i = 0; i < count; i++)
            {
                Int16 lastX = Convert.ToInt16(this.wallStartBottomPos.X + WALL_PART_WIDTH * (i + 1) - 1);
                Int16 lastZ = Convert.ToInt16(this.wallStartBottomPos.Y + 5 * WALL_PART_WIDTH + WALL_PART_HEIGHT - 1);
                Int16 X = Convert.ToInt16(this.wallStartBottomPos.X + WALL_PART_WIDTH * i);
                Int16 Z = Convert.ToInt16(this.wallStartBottomPos.Y + 5 * WALL_PART_WIDTH);
                this.gameField.CheckCoordinates(X, Z, CellState.Wall, lastX, lastZ, out this.currentBuildingVertices);
                this.InstantiateWallPart(X, Z);

                Z = Convert.ToInt16(this.wallStartBottomPos.Y - 2);
                lastZ = Convert.ToInt16(this.wallStartBottomPos.Y + WALL_PART_HEIGHT - 3);

                if (i == 2)
                {
                    this.InstantiateGates(X, Z, lastX, lastZ);
                    continue;
                }

                this.gameField.CheckCoordinates(X, Z, CellState.Wall, lastX, lastZ, out this.currentBuildingVertices);
                this.InstantiateWallPart(X, Z);
            }
        }

        private void InitializeHorizontalWall(Int16 count)
        {
            for (Int16 i = 0; i < count; i++)
            {
                Int16 lastX = Convert.ToInt16(this.wallStartBottomPos.Y + WALL_PART_HEIGHT - 1);
                Int16 lastZ = Convert.ToInt16(this.wallStartBottomPos.X + WALL_PART_WIDTH * (i + 1) - 1);
                Int16 X = Convert.ToInt16(this.wallStartBottomPos.Y);
                Int16 Z = Convert.ToInt16(this.wallStartBottomPos.X + WALL_PART_WIDTH * i);
                this.gameField.CheckCoordinates(X, Z, CellState.Wall, lastX, lastZ, out this.currentBuildingVertices);
                this.InstantiateWallPart(X, Z);

                X = Convert.ToInt16(this.wallStartTopPos.Y);
                lastX = Convert.ToInt16(this.wallStartTopPos.Y + WALL_PART_HEIGHT - 1);
                this.gameField.CheckCoordinates(X, Z, CellState.Wall, lastX, lastZ, out this.currentBuildingVertices);
                this.InstantiateWallPart(X, Z);
            }
        }

        private void InstantiateWallPart(Int16 X, Int16 Z)
        {
            Wall wall = new Wall(this.Game);
            wall.LoadTextures();
            wall.SetVertices(this.currentBuildingVertices);
            wall.Position = new Point(X, Z);
            this.listOfBuildings.Add(wall);
        }

        private void InstantiateGates(Int16 X, Int16 Z, Int16 lastX, Int16 lastZ)
        {
            Gates gates = new Gates(this.Game);
            this.gameField.CheckCoordinates(X, Z, CellState.Gate, lastX, lastZ, out this.currentBuildingVertices);
            gates.LoadTextures();
            gates.SetVertices(this.currentBuildingVertices);
            gates.Position = new Point(X, Z);
            this.listOfBuildings.Add(gates);
        }

        public void SetMayorObject(ref Mayor mayorObject)
        {
            this.mayorObject = mayorObject;
            this.mayorObject.OnRecallBuilders += new Mayor.RecallBuilders(mayorObject_OnRecallBuilders);
            this.mayorObject.OnAddBuilders += new Mayor.AddBuilders(mayorObject_OnAddBuilders);
        }

        private void mayorObject_OnAddBuilders(Object sender)
        {
            this.isAboutToProceed = true;
        }

        private void mayorObject_OnRecallBuilders(Object sender)
        {
            this.isAboutToPauseConstruction = true;
        }

        //X is height and Z is width: arr[X][Z]
        public void SetBuildingToBuild(CellState type, Int16 X, Int16 Z)
        {
            this.buildingTypeToBuild = type;
            this.BuildIfPossible(X, Z);
        }

        private void BuildIfPossible(Int16 X, Int16 Z)
        {
            if(this.listOfTypes.ContainsKey(this.buildingTypeToBuild))
            {
                if (!this.listOfTypes[this.buildingTypeToBuild].IsEnoughWorkersInvokation())
                {
                    this.mayorObject.ShowMessageBox();
                    return;
                }

                if (this.listOfTypes[this.buildingTypeToBuild].StartCheckerInvokation())
                {
                    this.listOfTypes[this.buildingTypeToBuild].StartConstructBuildingInvokation(X, Z);
                }
                else
                {
                    MessageBox msgBox;
                    MessageBox.Show(this.Game, "The maximum number of buildings is reached\nfor this type", "Cannot construct \na building",
                                    MessageBoxButton.OK, this.msgBoxTex, out msgBox);
                }
            }
        }

        private void FreeWorkers(Int16 neededWorkers)
        {
            this.mayorObject.ReleaseBuilders(neededWorkers);
        }

        private Boolean IsEnoughWorkers(Int16 neededWorkers)
        {
            this.neededWorkers = neededWorkers;
            return this.mayorObject.GetFreeUnits(neededWorkers, PrivilegesCosts.PrivilegedWorker);
        }

        private Boolean CheckAvailability()
        {
            Boolean isAny = false;
            var result = from target in this.listOfBuildings
                         where target.buildingType == this.buildingTypeToBuild
                         select target;

            foreach (BuildingBase b in result)
            {
                isAny = true;
                if (b.NumberOfBuildings < b.GetMaxNumberOfBuildings())
                {
                    return true;
                }
            }

            //The building could be possibly constructed due to the reason that there is no at all such constructions in the list
            if (!isAny)
            {
                return true;
            }

            return false;
        }

        private void ConstructBuilding(Type type, params Int16[] list)
        {
            Int16 lastX = Convert.ToInt16(list[0] + list[3] - 1);
            Int16 lastZ = Convert.ToInt16(list[1] + list[2] - 1);
            if (this.gameField.CheckCoordinates(list[0], list[1], this.buildingTypeToBuild, lastX, lastZ, out this.currentBuildingVertices))
            {
                ConstructorInfo ctor = type.GetConstructors().First(); 
                BuildingBase obj = ctor.Invoke(new Object[]{this.Game}) as BuildingBase;
                MethodInfo methodLoadTexture = type.GetMethod("LoadTextures");
                methodLoadTexture.Invoke(obj, null);
                MethodInfo methodSetVertices = type.GetMethod("SetVertices");
                methodSetVertices.Invoke(obj, new Object[] { this.currentBuildingVertices });
                PropertyInfo propertyPoint = type.GetProperty("Position");
                propertyPoint.SetValue(obj, new Point(list[0], list[1]), null);
                MethodInfo methodSetStartConstruction = type.GetMethod("SetStartConstruction");
                methodSetStartConstruction.Invoke(obj, new Object[] { true });
                MethodInfo methodSetResourcesHUDObject = type.GetMethod("SetResourcesHUDObject");
                methodSetResourcesHUDObject.Invoke(obj, new Object[] {this.resourcesHUD});
                this.listOfBuildings.Add(obj);
                this.IncreaseNumberOfConstructions(this.buildingTypeToBuild);
                this.mayorObject.SendBuilders(this.neededWorkers);
                this.neededWorkers = 0;
                obj.OnBuildingConstructed += new BuildingBase.BuildingConstructed(obj_OnBuildingConstructed);
            }
        }

        private void obj_OnBuildingConstructed(Object o, CellState type)
        {
            BuildingBase buildingBase = (o as BuildingBase);
            this.listOfTypes[type].FreeWorkersInvocation();
            if (type == CellState.CityHall)
            {
                this.mayorObject.LegalizeAuthority();
            }
            buildingBase.OnBuildingConstructed -= new BuildingBase.BuildingConstructed(obj_OnBuildingConstructed);
        }

        private void IncreaseNumberOfConstructions(CellState type)
        {
            var result = from target in this.listOfBuildings
                         where target.buildingType == type
                         select target;

            foreach (BuildingBase b in result)
            {
                b.NumberOfBuildings++;
                b.NumberOfBuildings = result.ElementAt(0).NumberOfBuildings;
            }
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

        protected override void LoadContent()
        {
            this.msgBoxTex = this.Game.Content.Load<Texture2D>("Textures/MessageBox/warning_buildings_parent");
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (BuildingBase b in this.listOfBuildings)
            {
                b.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.listOfBuildings.Count > 0)
            {
                this.Game.GraphicsDevice.RasterizerState = new RasterizerState()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.CullClockwiseFace
                };

                this.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                foreach (BuildingBase b in this.listOfBuildings)
                {
                    b.Draw();
                }
            }

            base.Draw(gameTime);
        }

        public void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection)
        {
            if (this.listOfBuildings.Count > 0)
            {
                foreach (BuildingBase b in this.listOfBuildings)
                {
                    b.SetMatrices(ref world, ref view, ref projection, ref this.terrainWorld);
                }
            }
        }

        public void GetMayorObject(ref Mayor mayorObject)
        {
            mayorObject = this.mayorObject;
        }
    }
}
