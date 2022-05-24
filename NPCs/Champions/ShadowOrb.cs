using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    public class ShadowOrb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Orb");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗影珠");
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(
                   ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<ShadowChampion>()],
                   quickUnlock: true
               );
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 9999;
            NPC.lifeMax = 9999;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.chaseable = false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = 9999;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.buffType[0] != 0)
                NPC.DelBuff(0);

            NPC host = FargoSoulsUtil.NPCExists(NPC.ai[0], ModContent.NPCType<ShadowChampion>());
            if (host == null)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            NPC.scale = (Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.95f;
            NPC.life = NPC.lifeMax;

            NPC.damage = 0;
            NPC.defDamage = 0;

            NPC.position = host.Center + new Vector2(NPC.ai[1], 0f).RotatedBy(NPC.ai[3]);
            NPC.position.X -= NPC.width / 2;
            NPC.position.Y -= NPC.height / 2;
            float rotation = 0.07f; //NPC.ai[1] == 125f ? 0.03f : -0.015f;
            if (NPC.ai[1] != 110)
                rotation = 0.03f;
            NPC.ai[3] += rotation;
            if (NPC.ai[3] > (float)Math.PI)
            {
                NPC.ai[3] -= 2f * (float)Math.PI;
                NPC.netUpdate = true;
            }
            NPC.rotation = NPC.ai[3] + (float)Math.PI / 2f;

            if (NPC.ai[1] != 110 && NPC.ai[1] != 700)
            {
                NPC.ai[2] += 2 * (float)Math.PI / 69;
                if (NPC.ai[2] > (float)Math.PI)
                    NPC.ai[2] -= 2 * (float)Math.PI;
                NPC.ai[1] += (float)Math.Sin(NPC.ai[2]) * 30;
            }

            NPC.alpha = NPC.localAI[3] == 1 ? 150 : 0;

            if ((NPC.ai[1] == 110 && host.life < host.lifeMax * .66)
                || (NPC.ai[1] == 700 && host.life < host.lifeMax * .33))
                NPC.active = false;

            NPC.dontTakeDamage = host.ai[0] == -1;

            if (NPC.localAI[3] == 1)
                NPC.dontTakeDamage = true;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            NPC.dontTakeDamage = true;
            NPC.localAI[3] = 1;
            NPC.netUpdate = true;
            damage = 0;

            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

            const int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.UnitX * 10f;
                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + NPC.Center;
                Vector2 vector7 = vector6 - NPC.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 27, 0f, 0f, 0, default(Color), 3f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }

            return false;
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage = 0;
            NPC.life++;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (FargoSoulsUtil.CanDeleteProjectile(projectile))
            {
                projectile.penetrate = 0;
                projectile.timeLeft = 0;
            }
            damage = 0;
            NPC.life++;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreKill()
        {
            return false;
        }

        public override bool? DrawHealthBar(byte hbPos, ref float scale, ref Vector2 Pos)
        {
            return false;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White * NPC.Opacity;
        }
    }
}
