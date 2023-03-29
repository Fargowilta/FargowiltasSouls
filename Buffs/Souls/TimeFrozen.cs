using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class TimeFrozen : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Time Frozen");
            Description.SetDefault("You are stopped in time");
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

            player.GetModPlayer<FargoSoulsPlayer>().MutantNibble = true; //no heal
            player.GetModPlayer<FargoSoulsPlayer>().NoUsingItems = 2;

            FargowiltasSouls.Instance.ManageMusicTimestop(player.buffTime[buffIndex] < 5);

            if (!Main.dedServ && player.whoAmI == Main.myPlayer)
            {
                if (Filters.Scene["FargowiltasSouls:Invert"].IsActive())
                {
                    if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.CosmosChampion>())
                    && Main.npc[EModeGlobalNPC.championBoss].ai[0] == 15)
                    {
                        Filters.Scene["FargowiltasSouls:Invert"].GetShader().UseTargetPosition(Main.npc[EModeGlobalNPC.championBoss].Center);
                    }

                    if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
                        && FargoSoulsWorld.MasochistModeReal && Main.npc[EModeGlobalNPC.mutantBoss].ai[0] == -5)
                    {
                        Filters.Scene["FargowiltasSouls:Invert"].GetShader().UseTargetPosition(Main.npc[EModeGlobalNPC.mutantBoss].Center);
                    }
                }
                else if (player.buffTime[buffIndex] > 60)
                {
                    Filters.Scene.Activate("FargowiltasSouls:Invert").GetShader().UseTargetPosition(player.Center);
                }

                if (player.buffTime[buffIndex] == 90)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/ZaWarudoResume"), player.Center);
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().TimeFrozen = true;
        }
    }
}