using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class FuseBomb : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fuse Bomb");
        }

        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 1;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (NPC.golemBoss != -1 && Main.npc[NPC.golemBoss].active && Main.npc[NPC.golemBoss].type == NPCID.Golem) //during golem fight
            {
                target.AddBuff(BuffID.OnFire, 600);
                target.AddBuff(BuffID.BrokenArmor, 600);
                target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 600);
                target.AddBuff(BuffID.WitheredArmor, 600);

                if (Framing.GetTileSafely(Main.npc[NPC.golemBoss].Center).WallType != WallID.LihzahrdBrickUnsafe) //outside temple
                    target.AddBuff(BuffID.Burning, 120);
            }

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<EarthChampion>()))
            {
                target.AddBuff(BuffID.OnFire, 300);
                target.AddBuff(BuffID.Burning, 300);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int num615 = 0; num615 < 45; num615++)
            {
                int num616 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                Main.dust[num616].velocity *= 1.4f;
            }

            for (int num617 = 0; num617 < 30; num617++)
            {
                int num618 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[num618].noGravity = true;
                Main.dust[num618].velocity *= 7f;
                num618 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[num618].velocity *= 3f;
            }

            for (int num619 = 0; num619 < 3; num619++)
            {
                float scaleFactor9 = 0.4f;
                if (num619 == 1) scaleFactor9 = 0.8f;
                int num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore97 = Main.gore[num620];
                gore97.velocity.X++;
                Gore gore98 = Main.gore[num620];
                gore98.velocity.Y++;
                num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore99 = Main.gore[num620];
                gore99.velocity.X--;
                Gore gore100 = Main.gore[num620];
                gore100.velocity.Y++;
                num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore101 = Main.gore[num620];
                gore101.velocity.X++;
                Gore gore102 = Main.gore[num620];
                gore102.velocity.Y--;
                num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore103 = Main.gore[num620];
                gore103.velocity.X--;
                Gore gore104 = Main.gore[num620];
                gore104.velocity.Y--;
            }
        }
    }
}