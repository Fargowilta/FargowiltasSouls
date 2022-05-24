using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class CreeperGutted : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gutted Creeper");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "爬行者");
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(
                    ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCID.BrainofCthulhu],
                    quickUnlock: true
                );
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Mods.FargowiltasSouls.Bestiary.GuttedCreeper")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.lifeMax = 30;
            NPC.friendly = true;
            NPC.dontCountMe = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.8f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
        }

        public override void AI()
        {
            if (NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                NPC.lifeMax *= (int)NPC.ai[2];
                NPC.defDamage *= (int)NPC.ai[2];
                NPC.defDefense *= (int)NPC.ai[2];
                NPC.life = NPC.lifeMax;
            }

            NPC.damage = NPC.defDamage;
            NPC.defense = NPC.defDefense;

            Player player = Main.player[(int)NPC.ai[0]];
            if (!player.active || player.dead || !player.GetModPlayer<FargoSoulsPlayer>().GuttedHeart)
            {
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
                return;
            }

            Vector2 distance = (player.Center - NPC.Center).RotatedBy(NPC.ai[3]);
            float length = distance.Length();
            if (length > 1000f)
            {
                NPC.Center = player.Center;
                NPC.velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 8;
            }
            else if (length > 40f)
            {
                distance /= 10f;
                NPC.velocity = (NPC.velocity * 15f + distance) / 16f;
            }
            else
            {
                if (NPC.velocity.Length() < 8)
                    NPC.velocity *= 1.05f;
            }

            if (NPC.ai[1]++ > 52f)
            {
                NPC.ai[1] = 0f;
                NPC.ai[3] = MathHelper.ToRadians(Main.rand.NextFloat(-15, 15));
                //NPC.velocity = NPC.velocity.RotatedByRandom(2 * Math.PI);

                if (player.whoAmI == Main.myPlayer && !player.GetToggleValue("MasoBrain"))
                {
                    int n = NPC.whoAmI;
                    NPC.StrikeNPCNoInteraction(9999, 0f, 0);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n, 9999f);
                    return;
                }
            }

            NPC.position += player.position - player.oldPosition;

            /*const float IdleAccel = 0.05f;
            foreach (NPC n in Main.npc.Where(n => n.active && n.ai[0] == NPC.ai[0] && n.type == NPC.type && n.whoAmI != NPC.whoAmI && NPC.Distance(n.Center) < NPC.width))
            {
                NPC.velocity.X += IdleAccel * (NPC.Center.X < n.Center.X ? -1 : 1);
                NPC.velocity.Y += IdleAccel * (NPC.Center.Y < n.Center.Y ? -1 : 1);
                n.velocity.X += IdleAccel * (n.Center.X < NPC.Center.X ? -1 : 1);
                n.velocity.Y += IdleAccel * (n.Center.Y < NPC.Center.Y ? -1 : 1);
            }*/
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[2] <= 1)
                NPC.frame.Y = 0;
            else if (NPC.ai[2] <= 2)
                NPC.frame.Y = frameHeight;
            else
                NPC.frame.Y = frameHeight * 2;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage *= 4 * 2; //comepnsate below strikenpc below
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.RottenEgg:
                    return false;

                case ProjectileID.AshBallFalling:
                case ProjectileID.CrimsandBallFalling:
                case ProjectileID.DirtBall:
                case ProjectileID.EbonsandBallFalling:
                case ProjectileID.MudBall:
                case ProjectileID.PearlSandBallFalling:
                case ProjectileID.SandBallFalling:
                case ProjectileID.SiltBall:
                case ProjectileID.SlushBall:
                    if (projectile.velocity.X == 0)
                        return false;
                    break;

                default:
                    break;
            }

            return null;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage /= 2; //take less damage from hostile contact damage too
            return true;
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (FargoSoulsUtil.CanDeleteProjectile(projectile))
            {
                projectile.timeLeft = 0;
                projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().canHurt = false; //so ml projs, etc. splash damage wont hrut
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.CreeperHitbox>(), NPC.damage, 6f, (int)NPC.ai[0]);

            if (NPC.life <= 0)
            {
                //SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 5);
                    Main.dust[d].velocity *= 2.5f;
                    Main.dust[d].scale += 0.5f;
                }
            }
        }

        public override bool CheckActive() => false;

        public override bool PreKill() => false;
    }
}