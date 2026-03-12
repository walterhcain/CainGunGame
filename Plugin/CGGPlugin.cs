using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace walterhcain.GunGame
{
    public class CGGPlugin : RocketPlugin<CGGConfig>
    {
        public static CGGPlugin Instance;
        public string version = "Version 1.0.0";
        protected Dictionary<CSteamID, int> kills = new Dictionary<CSteamID, int>();
        protected Dictionary<CSteamID, int> deaths = new Dictionary<CSteamID, int>();
        protected Dictionary<CSteamID, int> levels = new Dictionary<CSteamID, int>();
        protected CSteamID[] thirdPlace = new CSteamID[1];
        protected CSteamID[] secondPlace = new CSteamID[1];
        protected CSteamID[] firstPlace = new CSteamID[1];

        protected DateTime timer;
        public bool GameOver = false;

        

        protected override void Load()
        {
            Instance = this;
            U.Events.OnPlayerConnected += GG_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += GG_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDeath += GG_OnPlayerKilled;
            PlayerLife.onPlayerLifeUpdated -= GG_OnPlayerLifeUpdated;
            //UnturnedPlayerEvents.OnPlayerRevive += GG_OnPlayerRevive;
            Rocket.Core.Logging.Logger.Log("Cain's Gun Game has been successfully loaded!");
            Rocket.Core.Logging.Logger.Log("---------------------------------------------");
            Rocket.Core.Logging.Logger.Log(version);
            Rocket.Core.Logging.Logger.Log("---------------------------------------------");
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= GG_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= GG_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDeath -= GG_OnPlayerKilled;
            PlayerLife.onPlayerLifeUpdated -= GG_OnPlayerLifeUpdated;
            //UnturnedPlayerEvents.OnPlayerRevive -= GG_OnPlayerRevive;
            Rocket.Core.Logging.Logger.Log("Cain's Gun Game has been successfully unloaded!");
        }


        private void GG_OnPlayerConnected(UnturnedPlayer player)
        {
            kills.Add(player.CSteamID, 0);
            deaths.Add(player.CSteamID, 0);
            levels.Add(player.CSteamID, 1);
            ClearInv(player);
            //giveGun(player);
        }

        private void GG_OnPlayerKilled(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            ClearInv(player);
            deaths[player.CSteamID] = deaths[player.CSteamID] + 1;
            if (cause == EDeathCause.MELEE && Instance.Configuration.Instance.MeleeBack == true)
            {
                LevelDown(player);
            }
            else if (cause == EDeathCause.PUNCH && Instance.Configuration.Instance.PunchBack == true)
            {
                LevelDown(player);
            }
            else if (cause == EDeathCause.GUN)
            {
                kills[murderer] = kills[murderer] + 1;
                if (!checkWin(UnturnedPlayer.FromCSteamID(murderer)))
                {
                    LevelUp(UnturnedPlayer.FromCSteamID(murderer));
                }
            }
            else if(cause == EDeathCause.MISSILE || cause == EDeathCause.GRENADE || cause == EDeathCause.LANDMINE || cause == EDeathCause.SENTRY)
            {
                kills[murderer] = kills[murderer] + 1;
            }

        }

        private void GG_OnPlayerLifeUpdated(Player player)
        {
            if (!player.life.isDead)
            {
                giveGun(UnturnedPlayer.FromPlayer(player));
            }
            
        }

/*
        private void GG_OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle)
        {
            //GiveGun(player)
        }
        */
        private void GG_OnPlayerDisconnected(UnturnedPlayer player)
        {
            ClearInv(player);
            kills.Remove(player.CSteamID);
            deaths.Remove(player.CSteamID);
            levels.Remove(player.CSteamID);
            if (firstPlace[0] == player.CSteamID)
            {
                firstPlace[0] = (CSteamID)0;
            }
            else if (secondPlace[0] == player.CSteamID)
            {
                secondPlace[0] = (CSteamID)0;
            }
            else if (thirdPlace[0] == player.CSteamID)
            {
                thirdPlace[0] = (CSteamID)0;
            }
        }

        private void LevelUp(UnturnedPlayer player)
        {
            CSteamID cid = player.CSteamID;
            levels[player.CSteamID] = levels[player.CSteamID] + 1;
            rankUp(player);
            checkWin(player);
        }

        private void LevelDown(UnturnedPlayer player)
        {
            if (levels[player.CSteamID] != 1)
            {
                levels[player.CSteamID] = levels[player.CSteamID] - 1;
            }
        }

        private bool checkWin(UnturnedPlayer player)
        {
         
            if(levels[player.CSteamID] == 18)
            {
                GameOver = true;
                timer = DateTime.Now.AddSeconds(Instance.Configuration.Instance.SafeZoneTimer);
                announceWinner(UnturnedPlayer.FromCSteamID(firstPlace[0]), UnturnedPlayer.FromCSteamID(secondPlace[0]), UnturnedPlayer.FromCSteamID(thirdPlace[0]));
                resetGame();
                foreach (SteamPlayer sp in Provider.clients)
                {
                    UnturnedPlayer up = UnturnedPlayer.FromSteamPlayer(sp);
                    ClearInv(up);
                    up.Teleport(new Vector3(CGGPlugin.Instance.Configuration.Instance.SafeZonex, CGGPlugin.Instance.Configuration.Instance.SafeZoney, CGGPlugin.Instance.Configuration.Instance.SafeZonez), 0);
                    kills[up.CSteamID] = 0;
                    deaths[up.CSteamID] = 0;
                    levels[up.CSteamID] = 1;

                }

                UnturnedChat.Say("Game Over! New Match will start in " + (timer - DateTime.Now).Seconds + " seconds!");
                return true;
            }
            else
            {
                
                return false;
            }
        }


        private void resetGame()
        {
            firstPlace[0] = (CSteamID)0;
            secondPlace[0] = (CSteamID)0;
            thirdPlace[0] = (CSteamID)0;
        }


        private void rankUp(UnturnedPlayer player)
        {
            CSteamID cid = player.CSteamID;
            if (firstPlace[0] == (CSteamID)0)
            {
                firstPlace[0] = cid;
            }
            else if (secondPlace[0] == (CSteamID)0)
            {
                secondPlace[0] = cid;
            }
            else if (thirdPlace[0] == (CSteamID)0)
            {
                thirdPlace[0] = cid;
            }
            else if (firstPlace[0] != cid && secondPlace[0] != cid && thirdPlace[0] != cid)
            {
                if (levels[cid] > levels[thirdPlace[0]])
                {
                    thirdPlace[0] = cid;
                }
            }
            else if (thirdPlace[0] == cid)
            {
                if (levels[cid] > levels[secondPlace[0]])
                {
                    thirdPlace[0] = secondPlace[0];
                    secondPlace[0] = cid;
                }
            }
            else if (secondPlace[0] == cid)
            {
                if (levels[cid] > levels[firstPlace[0]])
                {
                    secondPlace[0] = firstPlace[0];
                    firstPlace[0] = cid;
                }
            }
        }

        private void giveGun(UnturnedPlayer player)
        {
            int caseSwitch = levels[player.CSteamID];
            switch(caseSwitch){
                case 1:
                    player.GiveItem(Instance.Configuration.Instance.Level1Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level1Ammo, 3);
                    break;
                case 2:
                    player.GiveItem(Instance.Configuration.Instance.Level2Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level2Ammo, 3);
                    break;
                case 3:
                    player.GiveItem(Instance.Configuration.Instance.Level3Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level3Ammo, 3);
                    break;
                case 4:
                    player.GiveItem(Instance.Configuration.Instance.Level4Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level4Ammo, 3);
                    break;
                case 5:
                    player.GiveItem(Instance.Configuration.Instance.Level5Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level5Ammo, 3);
                    break;
                case 6:
                    player.GiveItem(Instance.Configuration.Instance.Level6Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level6Ammo, 3);
                    break;
                case 7:
                    player.GiveItem(Instance.Configuration.Instance.Level7Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level7Ammo, 3);
                    break;
                case 8:
                    player.GiveItem(Instance.Configuration.Instance.Level8Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level8Ammo, 3);
                    break;
                case 9:
                    player.GiveItem(Instance.Configuration.Instance.Level9Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level9Ammo, 3);
                    break;
                case 10:
                    player.GiveItem(Instance.Configuration.Instance.Level10Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level10Ammo, 3);
                    break;
                case 11:
                    player.GiveItem(Instance.Configuration.Instance.Level11Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level11Ammo, 3);
                    break;
                case 12:
                    player.GiveItem(Instance.Configuration.Instance.Level12Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level12Ammo, 3);
                    break;
                case 13:
                    player.GiveItem(Instance.Configuration.Instance.Level13Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level13Ammo, 3);
                    break;
                case 14:
                    player.GiveItem(Instance.Configuration.Instance.Level14Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level14Ammo, 3);
                    break;
                case 15:
                    player.GiveItem(Instance.Configuration.Instance.Level15Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level15Ammo, 3);
                    break;
                case 16:
                    player.GiveItem(Instance.Configuration.Instance.Level16Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level16Ammo, 1);
                    break;
                case 17:
                    player.GiveItem(Instance.Configuration.Instance.Level17Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level17Ammo, 4);
                    break;
                default:
                    player.GiveItem(Instance.Configuration.Instance.Level1Gun, 1);
                    player.GiveItem(Instance.Configuration.Instance.MeleeWeapon, 1);
                    player.GiveItem(Instance.Configuration.Instance.Level1Ammo, 4);
                    break;
            }
        }

        private void announceWinner(UnturnedPlayer player1, UnturnedPlayer player2, UnturnedPlayer player3)
        {
            if(firstPlace[0] != (CSteamID)0)
            {
                UnturnedChat.Say("1st Place: " + player1.CharacterName + " with " + kills[player1.CSteamID] + "kills and " + deaths[player1.CSteamID] + " deaths!", Color.yellow);
            }
            else
            {
                UnturnedChat.Say("The first place player left the game!");
            }
            if(secondPlace[0] != (CSteamID)0)
            {
                UnturnedChat.Say("2nd Place: " + player2.CharacterName + " with " + kills[player2.CSteamID] + "kills and " + deaths[player2.CSteamID] + " deaths!", Color.yellow);
            }
            else
            {
                UnturnedChat.Say("The second place player left the game!");
            }
            if(thirdPlace[0] != (CSteamID)0)
            {
                UnturnedChat.Say("3rd Place: " + player3.CharacterName + " with " + kills[player3.CSteamID] + "kills and " + deaths[player3.CSteamID] + " deaths!", Color.yellow);
            }
            else
            {
                UnturnedChat.Say("The third place player left the game!");
            }

        }

        public bool ClearInv(UnturnedPlayer player)
        {
            bool returnv = false;
            try
            {
                player.Player.equipment.dequip();
                for (byte p = 0; p < (PlayerInventory.PAGES - 1); p++)
                {
                    byte itemc = player.Player.inventory.getItemCount(p);
                    if (itemc > 0)
                    {
                        for (byte p1 = 0; p1 < itemc; p1++)
                        {
                            player.Player.inventory.removeItem(p, 0);
                        }
                    }
                }
                player.Player.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                {
                    (byte)0,
                    (byte)0,
                    new byte[0]
                });
                player.Player.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                {
                    (byte)1,
                    (byte)0,
                    new byte[0]
                });
                returnv = true;
            }
            catch (Exception e)
            {
                Rocket.Core.Logging.Logger.Log("There was an error clearing " + player.CharacterName + "'s inventory.  Here is the error.");
                Console.Write(e);
            }
            return returnv;
        }

        void FixedUpdate()
        {
            if (GameOver)
            {
                if (DateTime.Now >= timer)
                {
                   
                    foreach (SteamPlayer sp in Provider.clients)
                    {
                        UnturnedPlayer ut = UnturnedPlayer.FromSteamPlayer(sp);
                        ut.Suicide();
                    }
                    GameOver = false;
                    UnturnedChat.Say("Match Begin!");
                }
                if (timer < DateTime.Now.AddSeconds(6))
                {
                    UnturnedChat.Say("Match Begins in " + (timer - DateTime.Now).Seconds + " Seconds!");
                }
            }
        }
    }
}
//Find a way to autogive weapons and other items