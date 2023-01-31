using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Deathrays
{
    public abstract class MutantSpecialDeathray : BaseDeathray
    {

        public override string Texture => "FargowiltasSouls/Projectiles/Deathrays/MutantSpecialDeathray";
        public MutantSpecialDeathray(int maxTime) : base(maxTime, sheeting: TextureSheeting.Horizontal) { }
        public MutantSpecialDeathray(int maxTime, float hitboxModifier) : base(maxTime, hitboxModifier: hitboxModifier, sheeting: TextureSheeting.Horizontal) { }

        bool spawned;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 16;
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                Projectile.frame = (int)Math.Abs(Main.GameUpdateCount % Main.projFrames[Projectile.type]);
            }

            Projectile.frameCounter += 1;
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            if (Main.rand.NextBool(10))
                Projectile.spriteDirection *= -1;
        }
    }
}
