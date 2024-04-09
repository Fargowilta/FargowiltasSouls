using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Vortex
{
    public class VortexVortexProjectile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Vortex");
            Main.projFrames[Projectile.type] = 9;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 60 * 60;

            Projectile.scale = 2f;
        }
        public enum Biomes
        {
            Purity,
            Corruption,
            Crimson,
            Hallow,
            Snow,
            Desert,
            Jungle,
            Dungeon,
            Cloud
        }
        private int GetBiome() //purposefully not networked
        {
            ref float Biome = ref Projectile.localAI[0];
            ref float ParentID = ref Projectile.ai[0];
            Player player = Main.LocalPlayer;
            if (Main.projectile[(int)ParentID].active && Main.projectile[(int)ParentID].Center.Y - Projectile.Center.Y > 0) //if coming from up angle, use cloud texture
            {
                return (int)Biomes.Cloud;
            }
            if (player.ZoneCorrupt)
            {
                return (int)Biomes.Corruption;
            }
            if (player.ZoneCrimson)
            {
                return (int)Biomes.Crimson;
            }
            if (player.ZoneHallow)
            {
                return (int)Biomes.Hallow;
            }
            if (player.ZoneSnow)
            {
                return (int)Biomes.Snow;
            }
            if (player.ZoneDesert)
            {
                return (int)Biomes.Desert;
            }
            if (player.ZoneJungle)
            {
                return (int)Biomes.Jungle;
            }
            if (player.ZoneBeach)
            {
                return (int)Biomes.Desert;
            }
            if (player.ZoneDungeon)
            {
                return (int)Biomes.Dungeon;
            }
            return (int)Biomes.Purity;
        }

        public override void AI()
        {
            ref float ParentID = ref Projectile.ai[0];
            ref float Timer = ref Projectile.ai[1];
            ref float Biome = ref Projectile.localAI[0];
            ref float Rotate = ref Projectile.ai[1];
            if (Timer < 60)
            {
                if (Biome <= 0)
                {
                    Biome = GetBiome() + 1;
                    Projectile.frame = (int)Biome - 1;
                    Rotate = Main.rand.NextFloat((float)(-Math.PI / 15), (float)(Math.PI / 15));
                }
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= (int)(255f / 20);
                }
            }
            else
            {
                Projectile parent = Main.projectile[(int)ParentID];
                if (parent.TypeAlive<VortexVortex>())
                {
                    if ((Projectile.Center - parent.Center).LengthSquared() < 16 * 16)
                    {
                        Projectile.Kill();
                    }
                    int speed = 12;
                    Vector2 diff = Vector2.Normalize(parent.Center - Projectile.Center);
                    Projectile.velocity = diff * speed;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Projectile.rotation += Rotate;
            Timer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
            return false;
        }
    }
}