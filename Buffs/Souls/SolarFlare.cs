using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class SolarFlare : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Flare");
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "太阳耀斑");
        }

        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderDebuff";


        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SolarFlare = true;

            if (npc.buffTime[buffIndex] < 2)
            {
                int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(), 1000, 0f, Main.myPlayer);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            }
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            return true;
        }
    }
}