using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Dusts
{
    internal class BloodDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.4f;
            dust.noGravity = true;
            dust.scale *= 0.5f;
            dust.frame = new Rectangle(0, 0, 10, 10);
        }

        public override bool Update(Dust dust)
        {
            float light = dust.scale * 0.6f;
            Lighting.AddLight(dust.position, light * 1.68f, light * 0.08f, light * 0.67f);
            return true;
        }
    }
}
