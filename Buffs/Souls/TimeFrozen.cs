using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class TimeFrozen : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Time Frozen");
            //Description.SetDefault("You are stopped in time");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
            //DisplayName.AddTranslation(GameCulture.Chinese, "时间冻结");
            //Description.AddTranslation(GameCulture.Chinese, "你停止了时间");
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";

            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.mount.Active)
                player.mount.Dismount(player);

            player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlDown = false;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            player.controlMount = false;
            player.velocity = player.oldVelocity;
            player.position = player.oldPosition;

            player.GetModPlayer<FargoPlayer>().MutantNibble = true; //no heal

            Fargowiltas.Instance.ManageMusicTimestop(player.buffTime[buffIndex] < 5);

            if (Main.netMode != NetmodeID.Server)
            {
                if (!Filters.Scene["FargowiltasSouls:Invert"].IsActive() && player.buffTime[buffIndex] > 60)
                {
                    Filters.Scene.Activate("FargowiltasSouls:Invert").GetShader().UseTargetPosition(FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.CosmosChampion>()) ? Main.npc[EModeGlobalNPC.championBoss].Center : player.Center);
                }
            }

            if (player.buffTime[buffIndex] == 90)
            {
                if (!Main.dedServ)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/ZaWarudoResume").WithVolume(1f).WithPitchVariance(.5f), player.Center);
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().TimeFrozen = true;
        }
    }
}