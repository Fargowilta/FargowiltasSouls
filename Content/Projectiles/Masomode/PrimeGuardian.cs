using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class PrimeGuardian : MutantGuardian
    {
        public override string Texture => "Terraria/Images/NPC_127";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dungeon Guardian Prime");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.SkeletronPrime];
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 600;
            CooldownSlot = -1;
        }

        public override bool CanHitPlayer(Player target)
        {
            return true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Main.rand.NextFloat(0, 2 * (float)Math.PI);
                Projectile.hide = false;

                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                }
            }

            Projectile.frame = 2;
            Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.rotation += Projectile.direction * .3f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<NanoInjectionBuff>(), 480);
            target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 480);
            target.AddBuff(ModContent.BuffType<LethargicBuff>(), 480);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 100, default, 2f);
                Main.dust[dust].noGravity = true;
            }

            if (!Main.dedServ)
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity / 3, ModContent.Find<ModGore>(Mod.Name, Main.rand.NextBool() ? "Gore_149" : "Gore_150").Type, Projectile.scale);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Npc[NPCID.SkeletronPrime].Value;
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, tex);
            return false;
        }
    }
}

