using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Magmaw
{
    public partial class Magmaw : ModNPC
    {
        public enum State //ALL States
        {
            Idle,
            SwordClap,
            SwordCharge,
            SwordChargePredictive
        }
        #region AI
        public override void AI()
        {
            if (!AliveCheck(player))
                return;

            //Reset properties
            HitPlayer = true;

            // Spawn jaw if none exists
            if (!Main.projectile.Any(p => p.TypeAlive<MagmawJaw>() && p.As<MagmawJaw>().ParentID == NPC.whoAmI))
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), JawCenter, Vector2.Zero, ModContent.ProjectileType<MagmawJaw>(), NPC.damage, 2f, Main.myPlayer, NPC.whoAmI);

            // Spawn hands if they don't exist
            for (int side = -1; side < 2; side += 2)
            {
                if (!Main.projectile.Any(p => p.TypeAlive<MagmawHand>() && p.As<MagmawHand>().ParentID == NPC.whoAmI && p.As<MagmawHand>().Side == side))
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitX * side * 400, Vector2.Zero, ModContent.ProjectileType<MagmawHand>(), NPC.damage, 2f, Main.myPlayer, NPC.whoAmI, side);
                    }
            }

            JawOffset = Vector2.UnitY * NPC.height * 0.675f;

            //Phase 1/2 specific passive attributes
            switch (Phase)
            {
                case 1:

                    break;
                case 2:

                    break;
            }
            HandleState();
            Timer++;
        }
        public void HandleState()
        {
            switch ((State)CurrentState)
            {
                case State.Idle:
                    Idle();
                    break;
                case State.SwordClap:
                    SwordClap();
                    break;
                default:
                    FindAttackFromIdle();
                    break;
            }
        }
        public void HandleHandState(MagmawHand hand)
        {
            switch ((State)CurrentState)
            {
                case State.Idle:
                    Idle_Hand(hand);
                    break;
                case State.SwordClap:
                    SwordClap_Hand(hand);
                    break;
            }
        }
        #endregion
        #region Help Methods
        void Reset()
        {
            LockVector1 = Vector2.Zero;
            LockVector2 = Vector2.Zero;
            Timer = 0;
            AI2 = 0;
            AI3 = 0;
        }
        void FindAttackFromIdle()
        {
            Reset();
            ChainDepth = 1;
        }
        void Followup()
        {
            Reset();
            ChainDepth++;

        }
        void AttackThenIdle(State newState)
        {
            Reset();
            ChainDepth = MaxChainDepth;
        }
        void GoIdle(int idleTime = 60, bool reposition = false)
        {
            Reset();
            ChainDepth = 0;
            IdleReposition = reposition;
            IdleTime = idleTime;
            CurrentState = (float)State.Idle;
        }
        #endregion
        #region States
        public void Idle()
        {
            HitPlayer = false;
            float idleMaxDistance = 400;

            Vector2 idlePos = NPC.Center;
            if (NPC.Distance(player.Center) > idleMaxDistance)
                idlePos = player.Center + player.SafeDirectionTo(NPC.Center) * idleMaxDistance;
            if (IdleReposition)
                idlePos = player.Center - Vector2.UnitY * 400;
            NPC.velocity = (idlePos - NPC.Center) * 0.05f;
        }
        public void Idle_Hand(MagmawHand hand)
        {
            Vector2 offset = (Vector2.UnitX * hand.Side * 220 + Vector2.UnitY * 80).SetMagnitude(MagmawHand.DefaultDistance);
            Vector2 desiredPos = NPC.Center + offset;
            hand.Projectile.velocity = (desiredPos - hand.Projectile.Center) * 0.15f;

            //Vector2 rotDir = Projectile.SafeDirectionTo(Main.player[parent.target].Center);
            Vector2 rotDir = (Vector2.UnitX * hand.Side * 0.7f) - Vector2.UnitY;
            RotateTowards(rotDir, 0.15f);
        }
        public void SwordClap()
        {

        }
        public void SwordClap_Hand(MagmawHand hand)
        {

        }
        #endregion
    }
}
