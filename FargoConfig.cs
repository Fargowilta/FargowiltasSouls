using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasSouls
{
    class SoulConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static SoulConfig Instance => ModContent.GetInstance<SoulConfig>();

        private const string ModName = "FargowiltasSouls";

        [Label($"$Mods.{ModName}.Config.HideToggler")]
        [Tooltip($"$Mods.{ModName}.Config.HideTogglerDescription")]
        [DefaultValue(true)]
        public bool HideTogglerWhenInventoryIsClosed;

        [Label($"$Mods.{ModName}.Config.DeviChatter")]
        [DefaultValue(true)]
        public bool DeviChatter;

        [Label($"$Mods.{ModName}.Config.ToggleSearchReset")]
        [DefaultValue(false)]
        public bool ToggleSearchReset;

        [Label($"$Mods.{ModName}.Config.BigTossMode")]
        [DefaultValue(false)]
        public bool BigTossMode;

        #region maso

        [Header($"$Mods.{ModName}.Config.MasoHeader")]

        [Label($"$Mods.{ModName}.Config.MasoBossRecolors")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool BossRecolors;

        [Label($"$Mods.{ModName}.Config.Precision")]
        [Tooltip($"$Mods.{ModName}.Config.PrecisionTooltip")]
        [DefaultValue(true)]
        public bool PrecisionSealIsHold;

        private const float max4kX = 3840f;
        [Label($"$Mods.{ModName}.Config.OncomingMutantX")]
        [Increment(1f)]
        [Range(0f, max4kX)]
        [DefaultValue(610f)]
        public float OncomingMutantX;

        private const float max4kY = 2160f;
        [Label($"$Mods.{ModName}.Config.OncomingMutantY")]
        [Increment(1f)]
        [Range(0f, max4kY)]
        [DefaultValue(250f)]
        public float OncomingMutantY;

        #endregion

        #region patreon

        [Header($"$Mods.{ModName}.PatreonHeader")]
        [Label($"$Mods.{ModName}.PatreonRoomba")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonRoomba;

        [Label($"$Mods.{ModName}.PatreonOrb")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonOrb;

        [Label($"$Mods.{ModName}.PatreonFishingRod")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonFishingRod;

        [Label($"$Mods.{ModName}.PatreonDoor")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDoor;

        [Label($"$Mods.{ModName}.PatreonWolf")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonWolf;

        [Label($"$Mods.{ModName}.PatreonDove")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDove;

        [Label($"$Mods.{ModName}.PatreonKingSlime")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonKingSlime;

        [Label($"$Mods.{ModName}.PatreonFishron")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonFishron;

        [Label($"$Mods.{ModName}.PatreonPlant")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonPlant;

        [Label($"$Mods.{ModName}.PatreonDevious")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonDevious;

        [Label($"$Mods.{ModName}.PatreonVortex")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonVortex;

        [Label($"$Mods.{ModName}.PatreonPrime")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonPrime;

        [Label($"$Mods.{ModName}.PatreonCrimetroid")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonCrimetroid;

        [Label($"$Mods.{ModName}.PatreonNanoCore")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PatreonNanoCore;

        #endregion

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            OncomingMutantX = Utils.Clamp(OncomingMutantX, 0, max4kX);
            OncomingMutantY = Utils.Clamp(OncomingMutantY, 0, max4kY);
        }
    }
}
