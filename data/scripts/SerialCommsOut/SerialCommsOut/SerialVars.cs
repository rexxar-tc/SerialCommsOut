using System;
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
  Welcome to Modding API. This is one of two sample scripts that you can modify for your needs, 
  in this case simple script is prepared that will show Hello world message in chat. 
  You need to run this script manually from chat to see it. To run it you first need to enable this in game 
  (press new World, than Custom World and Mods , you should see Script1 at the top), when world with mod loads, 
  please press F11 to see if there was any loading error during loading of the mod. When there is no mod loading errors  
  you can activate mod by opening chat window (by pressing Enter key). Than you need to call Main method of script class. 
   
  To do that you need to write this command : //call SerialCommsOut SerialCommsOut.Script ShowSerialVars         //call SerialCommsOut_SerialCommsOut SerialCommsOut.Script ShowSerialVars

  //call means that you want to call script
  Script1_TestScript is name of directory (if you have more script directories e.g. Script1, Script2 ... you need to change Script1 to your actual directory)
  TestScript.Script is name of tthe class with namespace , if you define new class you need to use new name e.g. when you create class Test in TestScript namespace
  you need to write : TestScript.Test 
  ShowHelloWorld is name of method, you can call only public static methods from chat window. 
   
   You can define your own namespaces / classes / methods to call 
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
        public int Priority { get; set; }   // This needs to be a unique number to set the priority of which messages are sent.
        public string ItemName { get; set; } //friendly name i.e. "health"
        public string CurrentValue { get; set; } //health percentage etc.
        public string MaxValue { get; set; } // Maximum range for determining what to set the Analog pin value to in Hardware
        public string DataType { get; set; } // Type of data. Float, Int, string, etc. Different Hardware components may need to math some stuff to display data properly. 
    }

    [Sandbox.Common.MySessionComponentDescriptor(Sandbox.Common.MyUpdateOrder.AfterSimulation)]
    class Script : MySessionComponentBase
    {
        static int CurrentPlayerState = GetPlayerState(); //see Method GetPlayerState()        
        
        public static void ColllectDataForSerial()
        {


        }

        #region Get Player Data
        //Add switch case for CurrentPlayerState for each method below in this region only. 
        //If CurrentPlayerState !=0 playerfunctions have to be obtained a different way:
        //Call GetPlayerState and use Global Variable for switch
        //Switch Not needed for Ship Functions, we only need to get ship functions if CurrentPlayerState !=0

        static public string GetPlayerHealth(MyObjectBuilder_Character CharacterInfo)
        {
            float? percentFloat = null;
            if(GetPlayerState() == 0)
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
            if (GetPlayerState() == 0)
            {
                percentDouble = CharacterInfo.Battery.CurrentCapacity * 10000000f;
            }
            return percentDouble.ToString();
         }
        static public string GetPlayerOxygen(MyObjectBuilder_Character CharacterInfo)
        {
            double percentDouble = 0;
            if (GetPlayerState() == 0)
            {
                 percentDouble = CharacterInfo.OxygenLevel * 10000f / 100;
            }
            return percentDouble.ToString();            
        }
        static public string GetPlayerDamperStatus(MyObjectBuilder_Character CharacterInfo)
        {
            bool BitVal = false;
            if (GetPlayerState() == 0)
            {
                BitVal = CharacterInfo.DampenersEnabled;
            }
            return BitVal.ToString();
        }  
        static public string GetPlayerJetPackStatus(MyObjectBuilder_Character CharacterInfo)
        {
            bool BitVal = false;
            if (GetPlayerState() == 0)
            {
                BitVal = CharacterInfo.JetpackEnabled;
            }
            return BitVal.ToString();
        }
        static public string GetPlayerSuitLightStatus(MyObjectBuilder_Character CharacterInfo)
        {
            bool BitVal = false;
            if (GetPlayerState() == 0)
            {
                BitVal = CharacterInfo.LightEnabled;
            }
            return BitVal.ToString();
        }
        static public string GetPlayerSpeed(MyObjectBuilder_Character CharacterInfo)
        {
            string StringVal = string.Empty;
            if (GetPlayerState() == 0)
            { 
            float Xvalue = CharacterInfo.LinearVelocity.X;
            float Yvalue = CharacterInfo.LinearVelocity.Y;
            float Zvalue = CharacterInfo.LinearVelocity.Z;
            Vector3 testVector = new Vector3(Xvalue, Yvalue, Zvalue);
            StringVal = testVector.Length().ToString() + " m/s";
            }
            return StringVal;
        }
        static public string GetPlayerCurrentInventory(Sandbox.ModAPI.IMyInventory CharacterInventory)
        {
            string InvVol = string.Empty;            
            if (GetPlayerState() == 0)
            {
                InvVol = (CharacterInventory.CurrentVolume.RawValue /100).ToString();
            }
            else
            {
                InvVol = "4000";
            }
            return InvVol;
        }
        static public string GetPlayerMaxInventory(Sandbox.ModAPI.IMyInventory CharacterInventory)
        {
            string InvVol = string.Empty;         
                if (GetPlayerState() == 0)
                {
                    InvVol = (CharacterInventory.MaxVolume.RawValue /100).ToString();
                }
                else
                {
                    InvVol = "4000";
                }   
            
            return InvVol;
        }
        static public string GetPlayerAntennaStatus(MyObjectBuilder_Character CharacterInfo)
        {
            bool BitVal = false;
            if (GetPlayerState() == 0)
            {
                BitVal = CharacterInfo.EnableBroadcasting;
            }
            return BitVal.ToString();
        }
    #endregion

        #region Get Ship Data

        #endregion


        static public int GetPlayerState()
        {
            int PlayerState = 1;
            //check to see if player is solo = 0, in a cockpit = 1, chair = 2 or Cryopod = 3
            //Use GetObjectBuilder() to get the MyObjectBuilder_EntityBase of any entity. If your entity is a cockpit you should be able to cast it to MyObjectBuilder_Cockpit.          
           

            return PlayerState;
        }

      // ShowHelloWorld must be public static, you can define your own methods,
      // but to be able to call them from chat they must be public static 
        public static void ShowSerialVars()
        {
            MyObjectBuilder_Character CharacterInfo = (MyObjectBuilder_Character)MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.GetObjectBuilder();
            var characterEntity = (MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity);            
            var CharacterInventory = (characterEntity as Sandbox.ModAPI.Interfaces.IMyInventoryOwner).GetInventory(0) as Sandbox.ModAPI.IMyInventory;            
            Dictionary<int, SerialVars> SerialOutValues = new Dictionary<int,SerialVars>();

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
            SerialVars PlayerDampers = new SerialVars()
            {
                Priority = 3,
                ItemName = "PlayerDampers",
                CurrentValue = SerialCommsOut.Script.GetPlayerDamperStatus(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerJetPack = new SerialVars()
            {
                Priority = 4,
                ItemName = "PlayerJetPack",
                CurrentValue = SerialCommsOut.Script.GetPlayerJetPackStatus(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerSuitLight = new SerialVars()
            {
                Priority = 5,
                ItemName = "PlayerSuitLight",
                CurrentValue = SerialCommsOut.Script.GetPlayerSuitLightStatus(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerSpeed = new SerialVars()
            {
                Priority = 6,
                ItemName = "PlayerSpeed",
                CurrentValue = SerialCommsOut.Script.GetPlayerSpeed(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerCurrentInventory = new SerialVars()
            {
                Priority = 7,
                ItemName = "PlayerCurrentInventory",
                CurrentValue = SerialCommsOut.Script.GetPlayerCurrentInventory(CharacterInventory),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerMaxInventory = new SerialVars()
            {
                Priority = 8,
                ItemName = "PlayerMaxInventory",
                CurrentValue = SerialCommsOut.Script.GetPlayerMaxInventory(CharacterInventory),
                MaxValue = "true",
                DataType = "Bit"
            };
            SerialVars PlayerAntenna = new SerialVars()
            {
                Priority = 9,
                ItemName = "PlayerAntenna",
                CurrentValue = SerialCommsOut.Script.GetPlayerAntennaStatus(CharacterInfo),
                MaxValue = "true",
                DataType = "Bit"
            };
            //Should Ship data priority always come after player data?
            //Should we break this API script into 2 Scripts one for Player and one for Ship?
            //A ship only script would be easier to attach to a programmable block until Ondrej enables local scripts

            SerialVars ShipDamage = new SerialVars()
            {
                Priority = 100,
                ItemName = "ShipDamage",
                CurrentValue = "50",
                MaxValue = "100",
                DataType = "Double"
            };
            #endregion
            
            

            //Add all Desired Objects here with "SerialOutValues.Add"
            if (GetPlayerState() == 0)
            {
                SerialOutValues.Clear();
                SerialOutValues.Add(PlayerHealth.Priority, PlayerHealth);
                SerialOutValues.Add(PlayerEnergy.Priority, PlayerEnergy);
                SerialOutValues.Add(PlayerOxygen.Priority, PlayerOxygen);
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
            }
            if (GetPlayerState() != 0)
            {
                SerialOutValues.Add(ShipDamage.Priority, ShipDamage);
            }   
            MyAPIGateway.Utilities.ShowNotification("Data Collection Started", 10000, MyFontEnum.Red);
            
            //MyAPIGateway.Utilities.ShowMissionScreen("Serial Out Data", DateTime.Now.ToString(), "Values:", ScreenData, null, "Hide Screen");
            //ScreenData = String.Empty;

            MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.ToString(), 10000, MyFontEnum.Green);
            MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.Controller.ToString(), 10000, MyFontEnum.Green);
            MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.Controller.ControlledEntity.ToString(), 10000, MyFontEnum.Green);
            MyAPIGateway.Utilities.ShowNotification(MyAPIGateway.Session.Player.Controller.ControlledEntity.Entity.ToString(), 10000, MyFontEnum.Green);

            MyAPIGateway.Utilities.ShowNotification("Data Collection Completed",  10000, MyFontEnum.Red);
            //call SerialCommsOut_SerialCommsOut SerialCommsOut.Script ShowSerialVars
        }

   }
}
