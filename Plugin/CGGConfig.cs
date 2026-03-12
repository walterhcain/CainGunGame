using Rocket.API;

namespace walterhcain.GunGame
{
    public class CGGConfig : IRocketPluginConfiguration
    {
        //Levels of guns
        //ammo for each gun
        //Melee Weapon of choice
        //PunchBack?
        //MeleeBack?
        //Safezone coords x,y,z
        //Kills them or look at the arena code
        //Safezone timer

        public ushort Level1Gun;
        public ushort Level2Gun;
        public ushort Level3Gun;
        public ushort Level4Gun;
        public ushort Level5Gun;
        public ushort Level6Gun;
        public ushort Level7Gun;
        public ushort Level8Gun;
        public ushort Level9Gun;
        public ushort Level10Gun;
        public ushort Level11Gun;
        public ushort Level12Gun;
        public ushort Level13Gun;
        public ushort Level14Gun;
        public ushort Level15Gun;
        public ushort Level16Gun;
        public ushort Level17Gun;

        public ushort Level1Ammo;
        public ushort Level2Ammo;
        public ushort Level3Ammo;
        public ushort Level4Ammo;
        public ushort Level5Ammo;
        public ushort Level6Ammo;
        public ushort Level7Ammo;
        public ushort Level8Ammo;
        public ushort Level9Ammo;
        public ushort Level10Ammo;
        public ushort Level11Ammo;
        public ushort Level12Ammo;
        public ushort Level13Ammo;
        public ushort Level14Ammo;
        public ushort Level15Ammo;
        public ushort Level16Ammo;
        public ushort Level17Ammo;

        public ushort MeleeWeapon;

        public bool PunchBack;
        public bool MeleeBack;

        public float SafeZonex;
        public float SafeZoney;
        public float SafeZonez;

        public int SafeZoneTimer;



        public void LoadDefaults()
        {
            Level1Gun = 97;  //Default: Colt
            Level2Gun = 107; //Default: Ace
            Level3Gun = 488;  //Default: Desert Falcon
            Level4Gun = 99; //Default: Cobra
            Level5Gun = 484; //Default: Sportshot
            Level6Gun = 479; //Default: Birch rifle
            Level7Gun = 1143; //Default: Sawed-Off
            Level8Gun = 112; //Default: Bluntforce
            Level9Gun = 1041; //Default: Yuri
            Level10Gun = 1024; //Default: Peacemaker
            Level11Gun = 1037; //Default: Heartbreaker
            Level12Gun = 1362; //Default: Aug
            Level13Gun = 132; //Default: Dragonfang
            Level14Gun = 18; //Default: Timberwolf
            Level15Gun = 1382; //Default: Ekho
            Level16Gun = 300; //Default: ShadowStalker
            Level17Gun = 346; //Default: Crossbow

            Level1Ammo = 98;
            Level2Ammo = 108;
            Level3Ammo = 489;
            Level4Ammo = 1006;
            Level5Ammo = 485;
            Level6Ammo = 478;
            Level7Ammo = 381;
            Level8Ammo = 113;
            Level9Ammo = 1042;
            Level10Ammo = 1026;
            Level11Ammo = 6;
            Level12Ammo = 123;
            Level13Ammo = 133;
            Level14Ammo = 20;
            Level15Ammo = 1384;
            Level16Ammo = 301;
            Level17Ammo = 347;

            MeleeWeapon = 121;  //Default: Military Knife

            PunchBack = true;
            MeleeBack = false;

            SafeZonex = 0;
            SafeZoney = 0;
            SafeZonez = 0;

            SafeZoneTimer = 60;
        }
    }
}
