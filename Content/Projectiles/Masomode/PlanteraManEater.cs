using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class PlanteraManEater : PlanteraSnatchers
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureNPC(NPCID.ManEater);
        public override string VineTexture => "Terraria/Images/Chain4";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Planty Tentacle");
            Main.projFrames[Type] = Main.npcFrameCount[NPCID.ManEater];
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

    }
    public class PlanteraSnatcher : PlanteraSnatchers
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureNPC(NPCID.Snatcher);
        public override string VineTexture => "Terraria/Images/Chain5";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Planty Tentacle");
            Main.projFrames[Type] = Main.npcFrameCount[NPCID.ManEater];
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

    }
    public class PlanteraTrapper : PlanteraSnatchers
    {
        public override string Texture => FargoSoulsUtil.VanillaTextureNPC(NPCID.AngryTrapper);
        public override string VineTexture => "Terraria/Images/Chains_1";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Planty Tentacle");
            Main.projFrames[Type] = Main.npcFrameCount[NPCID.ManEater];
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

    }
    public abstract class PlanteraSnatchers : ModProjectile
    {
        public abstract string VineTexture { get; }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            //CooldownSlot = 1;

            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 60 * 60 * 60;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        int arenaDistance = Main.rand.Next(980, 1050);
        const int defaultDistance = 2000;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.Plantera);
            if (npc == null || !npc.HasValidTarget)
            {
                Projectile.Kill();
                return;
            }

            Vector2 originPos = npc.Center + Projectile.ai[1].ToRotationVector2() * defaultDistance;

            Projectile.rotation = Projectile.DirectionFrom(originPos).ToRotation() + MathHelper.Pi;
            Projectile.localAI[0] = originPos.X;
            Projectile.localAI[1] = originPos.Y;

            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.frame = 0;
                //Projectile.timeLeft--;
            }
            else
            {
                //if (counter == 0)
                //    SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);

                if (true)
                {
                    Projectile.position -= npc.velocity * 2 / 3;
                    int playerIndex = Player.FindClosest(npc.Center, 1, 1);
                    if (!playerIndex.IsWithinBounds(Main.maxPlayers))
                        return;
                    Player player = Main.player[playerIndex];
                    if (player == null || !player.active)
                        return;
                    float extension = Math.Min(npc.Distance(player.Center), defaultDistance - arenaDistance);
                    Vector2 target = originPos + Projectile.SafeDirectionTo(Main.player[npc.target].Center) * extension;

                    Projectile.rotation = Projectile.SafeDirectionTo(originPos).ToRotation();

                    Vector2 distance = target - Projectile.Center;
                    float length = distance.Length();
                    distance /= 8f;
                    Projectile.velocity = (Projectile.velocity * 9f + distance) / 10f;
                    if (distance.LengthSquared() < 100 * 100)
                        Projectile.velocity /= 2;
                }
                /*
                else if (counter == attackTime)
                {
                    Projectile.velocity = -12f * Projectile.rotation.ToRotationVector2();
                    SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
                }
                */
                /*
                else
                {
                    if (npc.HasPlayerTarget && Projectile.Distance(npc.Center) > npc.Distance(Main.player[npc.target].Center))
                    {
                        Tile tile = Framing.GetTileSafely(Projectile.Center);
                        if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ChlorophyteWeapon, -Projectile.velocity.X * 0.1f, -Projectile.velocity.Y * 0.1f, Scale: 2.5f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].velocity *= 4f;
                            }

                            Projectile.velocity = Vector2.Zero;
                        }
                    }
                }
                */

                if (Projectile.velocity.Length() > 8)
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 8;

                if (++Projectile.frameCounter > 3 * (Projectile.extraUpdates + 1))
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = 0;
                }
            }
        }
        /*
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
            
            if (Projectile.localAI[0] != 0 && Projectile.localAI[1] != 0 && counter <= attackTime)
            {
                Vector2 originPos = Vector2.UnitX * Projectile.localAI[0] + Vector2.UnitY * Projectile.localAI[1];

                int length = (int)Projectile.Distance(originPos);
                const int increment = 512;
                for (int i = 0; i < length; i += increment)
                {
                    if (!Main.dedServ)
                        Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.SafeDirectionTo(originPos) * (i + Main.rand.NextFloat(increment)), Vector2.Zero,
                        ModContent.Find<ModGore>(Mod.Name, Main.rand.NextBool() ? "Gore_386" : "Gore_387").Type, Projectile.scale);
                }
            }
            
            //Gore.NewGore(Projectile.Center, Vector2.Zero, mod.GetGoreSlot("Assets/Gores/Plantera/Gore_" + (Main.rand.NextBool() ? "388" : "389")), Projectile.scale);
        }
        */
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<IvyVenomBuff>(), 240);
        }
        /*
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                    new Vector2(Projectile.localAI[0], Projectile.localAI[1]), Projectile.Center);
        }
        */
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] != 0 && Projectile.localAI[1] != 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>(VineTexture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Vector2 position = Projectile.Center;
                Vector2 mountedCenter = Vector2.UnitX * Projectile.localAI[0] + Vector2.UnitY * Projectile.localAI[1];
                Rectangle? sourceRectangle = new Rectangle?();
                Vector2 origin = Vector2.UnitX * texture.Width * 0.5f + Vector2.UnitY * texture.Height * 0.5f;
                float num1 = texture.Height;
                Vector2 vector24 = mountedCenter - position;
                float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                bool flag = true;
                if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                    flag = false;
                if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                    flag = false;
                while (flag)
                    if (vector24.Length() < num1 + 1.0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Vector2 vector21 = vector24;
                        vector21.Normalize();
                        position += vector21 * num1;
                        vector24 = mountedCenter - position;
                        Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                        color2 = Projectile.GetAlpha(color2);
                        Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0);
                    }
            }

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }

}