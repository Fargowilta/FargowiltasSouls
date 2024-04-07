
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class TimeFrozenBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Time Frozen");
            // Description.SetDefault("You are stopped in time");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;

            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "时间冻结");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你停止了时间");
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

            player.FargoSouls().MutantNibble = true; //no heal
            player.FargoSouls().NoUsingItems = 2;

            FargowiltasSouls.ManageMusicTimestop(player.buffTime[buffIndex] < 5);

            if (!Main.dedServ && player.whoAmI == Main.myPlayer)
            {
                ManagedScreenFilter filter = ShaderManager.GetFilter("FargowiltasSouls.Invert");
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<CosmosChampion>())
                        && Main.npc[EModeGlobalNPC.championBoss].ai[0] == 15)
                {
                    filter.SetFocusPosition(Main.npc[EModeGlobalNPC.championBoss].Center);
                }

                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>())
                    && WorldSavingSystem.MasochistModeReal && Main.npc[EModeGlobalNPC.mutantBoss].ai[0] == -5)
                {
                    filter.SetFocusPosition(Main.npc[EModeGlobalNPC.mutantBoss].Center);
                }

                if (player.buffTime[buffIndex] > 60)
                    filter.Activate();

                if (player.buffTime[buffIndex] == 90)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ZaWarudoResume"), player.Center);

                if (SoulConfig.Instance.ForcedFilters && Main.WaveQuality == 0)
                    Main.WaveQuality = 1;
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().TimeFrozen = true;
        }
    }
}