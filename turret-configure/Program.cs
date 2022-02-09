using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        


        

        public class Turret
        {
            public IMyMotorAdvancedStator first_stator;
            public IMyCubeGrid middle_subgrid;
            public IMyMotorAdvancedStator second_stator;
            public IMyCubeGrid final_subgrid;
            public IMyCameraBlock camera;
            public List<IMyFunctionalBlock> guns;

            public IMyTurretControlBlock controller;

            public Turret()
            {
                guns = new List<IMyFunctionalBlock>();
                return;
            }
        }

        public Program()
        {
                        
            

        }

        public void Save()
        {
            
        }

        public void Main(string argument, UpdateType updateSource)
        {
            List<IMyMotorAdvancedStator> stator = new List<IMyMotorAdvancedStator>();
            List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
            List<IMyFunctionalBlock> allFunctionalBlocks = new List<IMyFunctionalBlock>();
            List<IMyUserControllableGun> guns = new List<IMyUserControllableGun>();
            List<IMyTurretControlBlock> controllers = new List<IMyTurretControlBlock>();
            List<Turret> turrets = new List<Turret>();

            GridTerminalSystem.GetBlocksOfType(stator);
            GridTerminalSystem.GetBlocksOfType(cameras);
            GridTerminalSystem.GetBlocksOfType(allFunctionalBlocks);
            GridTerminalSystem.GetBlocksOfType(controllers);
            GridTerminalSystem.GetBlocksOfType(guns);
            List<IMyFunctionalBlock> weapons = guns.Cast<IMyFunctionalBlock>().ToList();


            foreach (IMyMotorAdvancedStator stat in stator)
            {
                if (stat.CubeGrid.EntityId == Me.CubeGrid.EntityId)
                {
                    turrets.Add(new Turret() { first_stator = stat });
                }
            }
            for (int i = 0; i < turrets.Count; i++)
            {
                Turret tur = turrets[i];
                tur.middle_subgrid = tur.first_stator.TopGrid;
            }


            for (int i1 = 0; i1 < turrets.Count; i1++)
            {
                Turret tur = turrets[i1];
                foreach (IMyMotorAdvancedStator stat in stator)
                {
                    if (stat.CubeGrid.EntityId == tur.middle_subgrid.EntityId)
                    {
                        tur.second_stator = stat;
                        tur.final_subgrid = tur.second_stator.TopGrid;
                    }
                }


                try
                {
                    foreach (IMyCameraBlock cam in cameras)
                    {
                        if (cam.CubeGrid.EntityId == tur.final_subgrid.EntityId)
                        {
                            tur.camera = cam;
                        }
                    }
                }
                catch
                {

                }
                try
                {
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        IMyFunctionalBlock gun = weapons[i];
                        if (gun.CubeGrid.EntityId == tur.final_subgrid.EntityId)
                        {
                            tur.guns.Add(gun);
                        }
                    }
                }
                catch
                {

                }

            }

            for (int i = 0; i < turrets.Count; i++)
            {
                try
                {
                    turrets[i].controller = controllers[i];
                }
                catch
                {
                    Echo("Probably need to add more turret controller blocks");
                }


            }

            //configure
            for (int i = 0; i < turrets.Count; i++)
            {

                try
                {


                    Turret tur = turrets[i];
                    tur.controller.RemoveTools(allFunctionalBlocks);



                    tur.controller.AddTools(tur.guns);

                    tur.controller.AzimuthRotor = tur.first_stator;

                    tur.controller.ElevationRotor = tur.second_stator;
                    if (tur.camera != null)
                    {
                        tur.controller.Camera = tur.camera;
                    }

                    //final options
                    tur.controller.AIEnabled = true;
                    tur.controller.Enabled = true;
                    tur.controller.Range = 800;
                    tur.camera.Enabled = true;
                }
                catch (Exception e)
                {
                    Echo(e.Message + "\n" + e.Source);
                }
            }
            Echo("Turrets Ready");


        }
    }
}
