using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class TimeStopCDBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Time Stop Cooldown");
            // Description.SetDefault("You cannot stop time yet");
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (proj.active && proj.type == ProjectileID.StardustGuardian && proj.owner == player.whoAmI)
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            Vector2 vector6 = Vector2.UnitY * 5f;
                            vector6 = vector6.RotatedBy((j - (20 / 2 - 1)) * 6.28318548f / 20) + proj.Center;
                            Vector2 vector7 = vector6 - proj.Center;
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.PurificationPowder);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = vector7;
                            Main.dust[d].scale = 1.5f;
                        }

                        break;
                    }
                }
            }
        }
    }
}