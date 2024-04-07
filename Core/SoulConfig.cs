using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasSouls.Core
{
    class SoulConfig : ModConfig
    {
        public static SoulConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ServerSide;

        private const string ModName = "FargowiltasSouls";

        [DefaultValue(true)]
        public bool HideTogglerWhenInventoryIsClosed;

        [DefaultValue(true)]
        public bool ItemDisabledTooltip;

        [DefaultValue(true)]
        public bool DeviChatter;

        [DefaultValue(false)]
        public bool ToggleSearchReset;

        [DefaultValue(false)]
        public bool BigTossMode;

        [DefaultValue(true)]
        public bool ForcedFilters;

        #region maso

        [Header("Maso")]

        [DefaultValue(true)]
        [ReloadRequired]
        public bool BossRecolors;

        [DefaultValue(true)]
        public bool PrecisionSealIsHold;

        [DefaultValue(true)]
        public bool PreBossNightGlow;

        private const float max4kX = 3840f;
        [Increment(1f)]
        [Range(0f, max4kX)]
        [DefaultValue(610f)]
        public float OncomingMutantX;

        private const float max4kY = 2160f;
        [Increment(1f)]
        [Range(0f, max4kY)]
        [DefaultValue(250f)]
        public float OncomingMutantY;

        #endregion

        #region patreon

        [Header("Patreon")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonRoomba;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonOrb;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonFishingRod;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDoor;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonWolf;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDove;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonKingSlime;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonFishron;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonPlant;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDevious;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonVortex;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonPrime;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonCrimetroid;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonNanoCore;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonROB;

        #endregion

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            OncomingMutantX = Utils.Clamp(OncomingMutantX, 0, max4kX);
            OncomingMutantY = Utils.Clamp(OncomingMutantY, 0, max4kY);
        }
    }
}
