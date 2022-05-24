using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class LivingWasteland : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Wasteland");
            Description.SetDefault("Everyone around you turns to rot");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "人形废土");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你周围的每个人都开始腐烂");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            const float distance = 300f;
            for (int i = 0; i < Main.maxNPCs; i++)
                if (Main.npc[i].active && Main.npc[i].Distance(player.Center) < distance)
                    Main.npc[i].AddBuff(ModContent.BuffType<Rotting>(), 2);
            for (int i = 0; i < Main.maxPlayers; i++)
                if (Main.player[i].active && !Main.player[i].dead && i != player.whoAmI && Main.player[i].Distance(player.Center) < distance)
                    Main.player[i].AddBuff(ModContent.BuffType<Rotting>(), 2);

            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * distance);
                offset.Y += (float)(Math.Cos(angle) * distance);
                Dust dust = Main.dust[Dust.NewDust(player.Center + offset - new Vector2(4, 4), 0, 0, 119, 0, 0, 100, Color.White, 1f)];
                dust.velocity = player.velocity;
                if (Main.rand.NextBool(3))
                    dust.velocity += Vector2.Normalize(offset) * -5f;
                dust.noGravity = true;
            }

            player.GetModPlayer<FargoSoulsPlayer>().Rotting = true;
            /*player.GetModPlayer<FargoSoulsPlayer>().AttackSpeed -= .1f;
            player.statLifeMax2 -= player.statLifeMax / 5;
            player.statDefense -= 10;
            player.GetDamage(DamageClass.Melee) -= 0.1f;
            player.GetDamage(DamageClass.Magic) -= 0.1f;
            player.GetDamage(DamageClass.Ranged) -= 0.1f;
            player.GetDamage(DamageClass.Summon) -= 0.1f;*/
        }
    }
}