using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class OceanicSeal : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Oceanic Seal");
            //Description.SetDefault("No dodging, no lifesteal, no supersonic, no escape");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
            //DisplayName.AddTranslation(GameCulture.Chinese, "海洋印记");
            //Description.AddTranslation(GameCulture.Chinese, "无法躲避,无法进行生命偷取,无法快速移动,无法逃脱");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().OceanicMaul = true;
            player.GetModPlayer<FargoPlayer>().eternityDamage = 0; //fuck it

            //player.GetModPlayer<FargoPlayer>().MutantPresence = true; //LUL

            player.GetModPlayer<FargoPlayer>().noDodge = true;
            player.GetModPlayer<FargoPlayer>().noSupersonic = true;
            player.moonLeech = true;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBoss, NPCID.DukeFishron))
            {
                player.buffTime[buffIndex] = 2;
                if (player.whoAmI == Main.npc[EModeGlobalNPC.fishBoss].target
                    && player.whoAmI == Main.myPlayer
                    && player.ownedProjectileCounts[mod.ProjectileType("FishronRitual2")] < 1)
                {
                    Projectile.NewProjectile(Main.npc[EModeGlobalNPC.fishBoss].Center, Vector2.Zero,
                        mod.ProjectileType("FishronRitual2"), 0, 0f, player.whoAmI, 0f, EModeGlobalNPC.fishBoss);
                }
            }
            else
            {
                return;
            }

            /*float distance = player.Distance(Main.npc[FargoSoulsGlobalNPC.fishBoss].Center);
            const float threshold = 1200f;
            if (distance > threshold)
            {
                if (distance > threshold * 1.5f)
                {
                    if (distance > threshold * 2f)
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " tried to escape."), 7777, 0);
                        return;
                    }

                    player.frozen = true;
                    player.controlHook = false;
                    player.controlUseItem = false;
                    if (player.mount.Active)
                        player.mount.Dismount(player);
                    player.velocity.X = 0f;
                    player.velocity.Y = -0.4f;
                }

                Vector2 movement = Main.npc[FargoSoulsGlobalNPC.fishBoss].Center - player.Center;
                float difference = movement.Length() - 1200f;
                movement.Normalize();
                movement *= difference < 17f ? difference : 17f;
                player.position += movement;

                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, 135, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 5f;
                }
            }*/
        }
    }
}