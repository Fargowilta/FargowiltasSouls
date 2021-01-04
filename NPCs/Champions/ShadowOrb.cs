using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.IO;

namespace FargowiltasSouls.NPCs.Champions
{
    public class ShadowOrb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Orb");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影珠");
        }

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 32;
            npc.defense = 9999;
            npc.lifeMax = 9999;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            npc.aiStyle = -1;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;
            npc.chaseable = false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 9999;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[3] = reader.ReadSingle();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            if (npc.buffType[0] != 0)
                npc.DelBuff(0);

            if (npc.ai[0] < 0f || npc.ai[0] >= Main.maxNPCs)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            NPC host = Main.npc[(int)npc.ai[0]];
            if (!host.active || host.type != ModContent.NPCType<ShadowChampion>())
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            
            npc.scale = (Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.95f;
            npc.life = npc.lifeMax;

            npc.damage = 0;
            npc.defDamage = 0;

            npc.position = host.Center + new Vector2(npc.ai[1], 0f).RotatedBy(npc.ai[3]);
            npc.position.X -= npc.width / 2;
            npc.position.Y -= npc.height / 2;
            float rotation = 0.07f; //npc.ai[1] == 125f ? 0.03f : -0.015f;
            if (npc.ai[1] != 110)
                rotation = 0.03f;
            npc.ai[3] += rotation;
            if (npc.ai[3] > (float)Math.PI)
            {
                npc.ai[3] -= 2f * (float)Math.PI;
                npc.netUpdate = true;
            }
            npc.rotation = npc.ai[3] + (float)Math.PI / 2f;

            if (npc.ai[1] != 110 && npc.ai[1] != 700)
            {
                npc.ai[2] += 2 * (float)Math.PI / 69;
                if (npc.ai[2] > (float)Math.PI)
                    npc.ai[2] -= 2 * (float)Math.PI;
                npc.ai[1] += (float)Math.Sin(npc.ai[2]) * 30;
            }

            npc.alpha = npc.localAI[3] == 1 ? 150 : 0;

            if ((npc.ai[1] == 110 && host.life < host.lifeMax * .66)
                || (npc.ai[1] == 700 && host.life < host.lifeMax * .33))
                npc.active = false;

            npc.dontTakeDamage = host.ai[0] == -1;

            if (npc.localAI[3] == 1)
                npc.dontTakeDamage = true;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            npc.dontTakeDamage = true;
            npc.localAI[3] = 1;
            npc.netUpdate = true;
            damage = 0;

            Main.PlaySound(SoundID.Item, npc.Center, 14);

            const int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.UnitX * 10f;
                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + npc.Center;
                Vector2 vector7 = vector6 - npc.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 27, 0f, 0f, 0, default(Color), 3f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }

            return false;
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage = 0;
            npc.life++;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!projectile.minion)
            {
                projectile.penetrate = 0;
                projectile.timeLeft = 0;
            }
            damage = 0;
            npc.life++;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override bool? DrawHealthBar(byte hbPos, ref float scale, ref Vector2 Pos)
        {
            return false;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White * npc.Opacity;
        }
    }
}
