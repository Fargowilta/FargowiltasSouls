using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Shadow
{
    public class ShadowGuardian : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_68";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dungeon Guardian");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.DungeonGuardian];
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            CooldownSlot = 1;

            Projectile.timeLeft = 600;
            Projectile.hide = true;
            Projectile.light = 0.5f;

            Projectile.FargoSouls().DeletionImmuneRank = 1;
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
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                }
            }

            Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.rotation += Projectile.direction * .3f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 100, default, 2f);
                Main.dust[dust].noGravity = true;
            }

            if (!Main.dedServ)
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity / 3, ModContent.Find<ModGore>(Mod.Name,
                Main.rand.NextBool() ? "Gore_54" : "Gore_55").Type, Projectile.scale);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Darkness, 300);
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 300);
                target.AddBuff(BuffID.Blackout, 300);
                target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
                target.AddBuff(ModContent.BuffType<LethargicBuff>(), 300);
            }
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.guardBoss, NPCID.DungeonGuardian))
                target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Npc[NPCID.DungeonGuardian].Value;

            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, texture2D13);
            return false;
        }
    }
}