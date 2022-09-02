using FargowiltasSouls.Projectiles.Souls;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class ShellHide : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shell Hide");
            Description.SetDefault("Projectiles are being blocked");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "缩壳");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "阻挡抛射物,但受到双倍接触伤害");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            player.noKnockback = true;
            player.endurance = .9f;
            player.thorns *= 10;

            modPlayer.ShellHide = true;

            float distance = 3f * 16;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<TurtleShield>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TurtleShield>(), 0, 0, player.whoAmI);
            }

            if (modPlayer.TurtleCounter > 80)
            {
                Main.projectile.Where(x => x.active && x.hostile && x.damage > 0 && Vector2.Distance(x.Center, player.Center) <= distance && ProjectileLoader.CanDamage(x) != false && ProjectileLoader.CanHitPlayer(x, player) && FargoSoulsUtil.CanDeleteProjectile(x)).ToList().ForEach(x =>
                {
                    int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.GoldFlame, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100,
                        default(Color), 2f);
                    Main.dust[dustId].noGravity = true;
                    int dustId3 = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, DustID.GoldFlame, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100,
                        default(Color), 2f);
                    Main.dust[dustId3].noGravity = true;

                    // Turn around
                    x.velocity *= -1f;

                    // Flip sprite
                    if (x.Center.X > player.Center.X)
                    {
                        x.direction = 1;
                        x.spriteDirection = 1;
                    }
                    else
                    {
                        x.direction = -1;
                        x.spriteDirection = -1;
                    }

                    x.hostile = false;
                    x.friendly = true;

                    modPlayer.TurtleShellHP--;
                });
            }

            if (modPlayer.TurtleShellHP <= 0)
            {
                player.AddBuff(ModContent.BuffType<BrokenShell>(), 1800);
                modPlayer.TurtleShellHP = 19;

                //some funny dust
                const int max = 30;
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = Vector2.UnitY * 5f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                    Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.GreenBlood, 0f, 0f, 0, default(Color), 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }
            }
        }
    }
}