using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class UniversalCollapseProj : ModProjectile
    {
        public int countdown = 4;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuke");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = 16; //explosives AI
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2400;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 600 == 0)
            {
                CombatText.NewText(Projectile.Hitbox, new Color(51, 102, 0), countdown, true);
                countdown--;
            }

            Projectile.scale += .01f;

            Projectile.frameCounter++;   //Making the timer go up.
            if (Projectile.frameCounter >= 600)  //how fast animation is
            {
                Projectile.frame++; //Making the frame go up...
                Projectile.frameCounter = 0; //Resetting the timer.
                if (Projectile.frame > 3) //amt of frames - 1
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Main.tile[i, j].ClearEverything();

                    if (WorldGen.InWorld(i, j))
                        Main.Map.Update(i, j, 255);
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.boss && !npc.dontTakeDamage)
                {
                    npc.StrikeNPC(npc.lifeMax + npc.defense, 0, 0, true);
                }
            }

            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
            {
                int def = Main.LocalPlayer.statDefense;
                float dr = Main.LocalPlayer.endurance;
                Main.LocalPlayer.statDefense = 0;
                Main.LocalPlayer.endurance = 0f;

                int damage = Math.Max(9999, Main.LocalPlayer.statLifeMax2 * 2);
                Main.LocalPlayer.Hurt(PlayerDeathReason.ByProjectile(Main.LocalPlayer.whoAmI, Projectile.whoAmI), damage, 0);

                Main.LocalPlayer.statDefense = def;
                Main.LocalPlayer.endurance = dr;
            }

            Main.refreshMap = true;

            if (!Main.dedServ)
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Thunder") { Volume = 1.5f });
        }
    }
}