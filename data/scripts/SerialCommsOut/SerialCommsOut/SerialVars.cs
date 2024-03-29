﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRage;
using VRage.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRage.Voxels;
using VRageMath;


/*   
 * To execute this script, run the following command from the chat window:
        //call SerialCommsOut_SerialCommsOut SerialCommsOut.Script ShowSerialVars
 */


//Project Goal: Try to do all math here before sending data to Hardware to mitigate slower external HW clocks.

namespace SerialCommsOut 
{
    //We need to create a class object that contains all of the output properties we want to send to the Serial Port 
    //We are creating these as strings so that the Hardware script can grab the data from the Serial buffer without 
    //having to know the type ahead of time. This allows the Hardware to continue to recieve data while it is being read.
    //Getting Typed data from the serial buffer silently blocks new data until the read is complete, which could lead to lost data.
    public class SerialVars
    {
        public int Priority { get; set; }
        public string DataType { get; set; } // Type of data. Float, Int, string, etc. Different Hardware components may need to math some stuff to display data properly. 
        public string ItemName { get; set; } //friendly name i.e. "health"
        public string CurrentValue { get; set; } //health percentage etc.
        public string MaxValue { get; set; } // Maximum range for determining what to set the Analog pin value to in Hardware
        
    }

    [Sandbox.Common.MySessionComponentDescriptor(Sandbox.Common.MyUpdateOrder.AfterSimulation)]
     class Script : MySessionComponentBase
    {
        public static int CurrentPlayerState{get; set;}
        public static IMyEntity EntityStash { get; set; }
        public static void ColllectDataForSerial()
        {


        }

        #region Get Player Data
        //Add switch case for CurrentPlayerState for each method below in this region only. 
        //If CurrentPlayerState !=0 pass the stashed Player Entity - Needs to be written.

        static public string GetPlayerHealth(MyObjectBuilder_Character CharacterInfo)
        {
            float? percentFloat = null;
            if(CurrentPlayerState == 0)
            {
                if (!CharacterInfo.Health.HasValue)
                {
                    percentFloat = 100.00f;
                }
                else
                {
                    percentFloat = CharacterInfo.Health.Value;
                }
            }
            else
            {
                percentFloat = 666666;
            }

           return percentFloat.ToString();   
        } 
        static public string GetPlayerEnergy(MyObjectBuilder_Character CharacterInfo)
        {
            double percentDouble = 0;
            if (CurrentPlayerState == 0)
            {
                percentDouble = CharacterInfo.Battery.CurrentCapacity * 10000000f;
            }
            return percentDouble.ToString();
         }
        static public string GetPlayerOxygen(MyObjectBuilder_Character CharacterInfo)
        {
            double percentDouble = 0;
            if (CurrentPlayerState == 0)
            {
                 percentDouble = CharacterInfo.OxygenLevel * 10000f / 100;
            }
            return percentDouble.ToString();            
        }
        static public string GetPlayerHydrogen(MyObjectBuilder_Character CharacterInfo)
        {
            double percentDouble = 0;
            string currentamount = string.Empty;
            string id = string.Empty;
            if (CurrentPlayerState == 0)
            {
                //percentDouble = CharacterInfo.StoredGases..Capacity (* 10000f / 100);
                List<MyObjectBuilder_Character.StoredGas> StoredGases = CharacterInfo.StoredGases;
                foreach (MyObjectBuilder_Character.StoredGas gas in StoredGases)
                {
                     id = gas.Id.ToString();
                     if (id == "MyObjectBuilder_GasProperties/Hydrogen")
                     { percentDouble = gas.FillLevel *100; }
                     else { percentDouble = 0; }

                }
            }
            return percentDouble.ToString();
        }
        static public string GetPlayerDamperStatus(MyObjectBuilder_Character CharacterInfo)
        {
            bool BitVal = false;
            if (CurrentPlayerState == 0)
            {
                BitVal = CharacterInfo.DampenersEnabled;
            }
            return BitVal.ToString();
        }  
        static public string GetPlayerJetPackStatus(MyObjectBuilder_Character CharacterInfo)
        {
            bool BitVal = false;
            if (CurrentPlayerState == 0)
            {
                BitVal = CharacterInfo.JetpackEnabled;
            }
            return BitVal.ToString();
        }
        static public string GetPlayerSuitLightStatus(MyObjectBuilder_Character CharacterInfo)
        {
            bool BitVal = false;
            if (CurrentPlayerState == 0)
            {
                BitVal = CharacterInfo.LightEnabled;
            }
            return BitVal.ToString();
        }
        static public string GetPlayerSpeed(MyObjectBuilder_Character CharacterInfo)
        {
            string StringVal = string.Empty;
            if (CurrentPlayerState == 0)
            { 
            float Xvalue = CharacterInfo.LinearVelocity.X;
            float Yvalue = CharacterInfo.LinearVelocity.Y;
            float Zvalue = CharacterInfo.LinearVelocity.Z;
            Vector3 testVector = new Vector3(Xvalue, Yvalue, Zvalue);
            StringVal = testVector.Length().ToString() + " m/s";
            }
            return StringVal;
        }
        static public string GetPlayerMaxInventory()
        {
            var characterEntity = (MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity);
            var invOwner = (characterEntity as Sandbox.ModAPI.Interfaces.IMyInventoryOwner);
            float InvVol = 0;   
            if (invOwner != null && invOwner.InventoryCount > 0)
            {
                var inv = invOwner.GetInventory(0);

                if (CurrentPlayerState == 0)
                {
                    InvVol = (float)inv.MaxVolume;
                    //values[Icons.INVENTORY] = ((float)inv.CurrentVolume / (float)inv.MaxVolume) * 100;
                }
                else
                {
                    InvVol = 4000F;
                }
                    
            }                               
            return InvVol.ToString();
        }
        static public string GetPlayerCurrentInventory()
        {
            var characterEntity = (MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity);
            var invOwner = (characterEntity as Sandbox.ModAPI.Interfaces.IMyInventoryOwner);
            float InvVol = 0;
            if (invOwner != null && invOwner.InventoryCount > 0)
            {
                var inv = invOwner.GetInventory(0);

                if (CurrentPlayerState == 0)
                {
                    InvVol = (float)inv.CurrentVolume;
                    //values[Icons.INVENTORY] = ((float)inv.CurrentVolume / (float)inv.MaxVolume) * 100;
                }
                else
                {
                    InvVol = 4000F;
                }

            }
            return InvVol.ToString();
        }
        static public string GetPlayerAntennaStatus(MyObjectBuilder_Character CharacterInfo)
        {
            bool BitVal = false;
            if (CurrentPlayerState == 0)
            {
                BitVal = CharacterInfo.EnableBroadcasting;
            }
            return BitVal.ToString();
        }
    #endregion

        #region Get Ship Data
        static public string GetShipSomething()
        {
            string SomeShipData = string.Empty;
            if (CurrentPlayerState > 0)
            {


            }
            return SomeShipData;
        }
        #endregion


        static public int GetPlayerState()
        {
            int PlayerState = 1;
            //check to see if player is solo = 0, in a cockpit = 1, chair = 2 or Cryopod = 3
            //doing this in case we want a different output set depending on the situation (ex: we won't allow block/ship control for player in cryopod)   
            IMyEntity controlled = null;
            string state = string.Empty;
            try
            {
                 controlled = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity;
            }
            catch
            {
                if (EntityStash != null)
                {
                     controlled = EntityStash;
                }
            }
           // var controlled = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity;

            if (controlled is IMyCharacter)
            {
                state = "character";
                PlayerState = 0;
            }
            else if (controlled is Sandbox.ModAPI.Ingame.IMyCockpit)
            {
                state = "cockpit";
                PlayerState = 1;
            }
            else if (!(controlled is IMyCharacter) && (!(controlled is Sandbox.ModAPI.Ingame.IMyCockpit)))
            {
                state = controlled.ToString();
                PlayerState = 3;
            }
            return PlayerState;
        }

      // ShowHelloWorld must be public static, you can define your own methods,
      // but to be able to call them from chat they must be public static 
        public static void ShowSerialVars()
        {
            CurrentPlayerState = GetPlayerState();
            try
            {
                EntityStash = MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity;
            }
            catch
            {
                EntityStash = null;
            }
            MyAPIGateway.Utilities.ShowNotification("Data Collection Started", 10000, MyFontEnum.Red);            
            if(CurrentPlayerState == 0 || EntityStash != null)
            {
                
                ShowPlayerData();
            }
            
            //MyAPIGateway.Utilities.ShowNotification(state, 10000, MyFontEnum.Green);
            //MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.ToString(), 10000, MyFontEnum.Green);
            //MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.Client.ToString(), 10000, MyFontEnum.Green);
            //MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.IdentityId.ToString(), 10000, MyFontEnum.Green);
            //MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.Controller.ToString(), 10000, MyFontEnum.Green);
            //MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.Controller.ControlledEntity.ToString(), 10000, MyFontEnum.Green);
            //MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.ToString(), 10000, MyFontEnum.Green);
            MyAPIGateway.Utilities.ShowNotification("Data Collection Completed", 10000, MyFontEnum.Red);
            
        }

        private static void ShowPlayerData()
        {
            MyObjectBuilder_Character CharacterInfo = (MyObjectBuilder_Character)MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.GetObjectBuilder();
            
           
            Dictionary<int, SerialVars> SerialOutValues = new Dictionary<int, SerialVars>();

            #region object gathering

            SerialVars PlayerHealth = new SerialVars()
            {
                Priority = 0,
                ItemName = "PlayerHealth",
                CurrentValue = SerialCommsOut.Script.GetPlayerHealth(CharacterInfo),
                MaxValue = "100",
                DataType = "Float"
            };
            SerialVars PlayerEnergy = new SerialVars()
            {
                Priority = 1,
                ItemName = "PlayerEnergy",
                CurrentValue = SerialCommsOut.Script.GetPlayerEnergy(CharacterInfo),
                MaxValue = "100",
                DataType = "Double"
            };
            SerialVars PlayerOxygen = new SerialVars()
            {
                Priority = 2,
                ItemName = "PlayerOxygen",
                CurrentValue = SerialCommsOut.Script.GetPlayerOxygen(CharacterInfo),
                MaxValue = "100",
                DataType = "Double"
            };
            SerialVars PlayerHydrogen = new SerialVars()
            {
                Priority = 3,
                ItemName = "PlayerHydrogen",
                CurrentValue = SerialCommsOut.Script.GetPlayerHydrogen(CharacterInfo),
                MaxValue = "100",
                DataType = "Double"
            };
            SerialVars PlayerDampers = new SerialVars()
            {
                Priority = 4,
                ItemName = "PlayerDampers",
                CurrentValue = SerialCommsOut.Script.GetPlayerDamperStatus(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerJetPack = new SerialVars()
            {
                Priority = 5,
                ItemName = "PlayerJetPack",
                CurrentValue = SerialCommsOut.Script.GetPlayerJetPackStatus(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerSuitLight = new SerialVars()
            {
                Priority = 6,
                ItemName = "PlayerSuitLight",
                CurrentValue = SerialCommsOut.Script.GetPlayerSuitLightStatus(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerSpeed = new SerialVars()
            {
                Priority = 7,
                ItemName = "PlayerSpeed",
                CurrentValue = SerialCommsOut.Script.GetPlayerSpeed(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerCurrentInventory = new SerialVars()
            {
                Priority = 8,
                ItemName = "PlayerCurrentInventory",
                CurrentValue = SerialCommsOut.Script.GetPlayerCurrentInventory(),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerMaxInventory = new SerialVars()
            {
                Priority = 9,
                ItemName = "PlayerMaxInventory",
                CurrentValue = SerialCommsOut.Script.GetPlayerMaxInventory(),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerAntenna = new SerialVars()
            {
                Priority = 10,
                ItemName = "PlayerAntenna",
                CurrentValue = SerialCommsOut.Script.GetPlayerAntennaStatus(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            //Should Ship data priority always come after player data?
            //Should we break this API script into 2 Scripts one for Player and one for Ship?
            //A ship only script would be easier to attach to a programmable block until Ondrej enables local scripts

            /*SerialVars ShipDamage = new SerialVars()
            {
                Priority = 100,
                ItemName = "ShipDamage",
                CurrentValue = "50",
                MaxValue = "100",
                DataType = "Double"
            };*/
            #endregion

            //Add all Desired Objects here with "SerialOutValues.Add"
         
                SerialOutValues.Clear();
                SerialOutValues.Add(PlayerHealth.Priority, PlayerHealth);
                SerialOutValues.Add(PlayerEnergy.Priority, PlayerEnergy);
                SerialOutValues.Add(PlayerOxygen.Priority, PlayerOxygen);
                SerialOutValues.Add(PlayerHydrogen.Priority, PlayerHydrogen);
                SerialOutValues.Add(PlayerDampers.Priority, PlayerDampers);
                SerialOutValues.Add(PlayerJetPack.Priority, PlayerJetPack);
                SerialOutValues.Add(PlayerSuitLight.Priority, PlayerSuitLight);
                SerialOutValues.Add(PlayerSpeed.Priority, PlayerSpeed);
                SerialOutValues.Add(PlayerCurrentInventory.Priority, PlayerCurrentInventory);
                SerialOutValues.Add(PlayerMaxInventory.Priority, PlayerMaxInventory);
                SerialOutValues.Add(PlayerAntenna.Priority, PlayerAntenna);
                string ScreenData = string.Empty;
                foreach (SerialVars serialVars in SerialOutValues.Values)  //.OrderBy()
                {
                    ScreenData += (String.Format("Priority = {0}, ItemName = {1}, CurrentValue = {2}, MaxValue = {3}, DataType = {4}", serialVars.Priority, serialVars.ItemName, serialVars.CurrentValue, serialVars.MaxValue, serialVars.DataType)) + "/\n";
                }
            
            MyAPIGateway.Utilities.ShowMissionScreen("Serial Out Data", DateTime.Now.ToString(), "Values:", ScreenData, null, "Hide Screen");
            ScreenData = String.Empty;
       
            /*if (CurrentPlayerState != 0)
            {
                SerialOutValues.Add(ShipDamage.Priority, ShipDamage);
            }*/
            //call SerialCommsOut_SerialCommsOut SerialCommsOut.Script ShowSerialVars

        }

   }
}
