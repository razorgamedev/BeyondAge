using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BeyondAge.Graphics;
using Microsoft.Xna.Framework.Input;
using BeyondAge.Managers;
using NLua;

namespace BeyondAge.Entities
{
    class Character : Component
    {
        public string Name { get; set; } = "Anon";
        public int Age     { get; set; }
    }

    class CharacterController : Filter
    {
        private Entity player;
        private Primitives primitives;

        public int TimesTalkedToByPlayer { get; private set; } = 0;

        private float coolDown = 0;
        private float maxCoolDown { get => 1; }

        public CharacterController(Primitives primitives) : base(typeof(Body), typeof(Character), typeof(Sprite), typeof(PhysicsBody))
        {
            this.primitives = primitives;
        }

        public override void PreUpdate(GameTime time)
        {
            player = WorldRef.GetFirstWithComponent(typeof(Player));
        }

        public override void Update(Entity ent, GameTime time)
        {
            if (player != null)
            {
                var body        = ent.Get<Body>();
                var sprite      = ent.Get<Sprite>();
                var physics     = ent.Get<PhysicsBody>();
                var pbody       = player.Get<Body>();
                var pphysics    = player.Get<PhysicsBody>();
                var character   = ent.Get<Character>();

                if (Vector2.Distance(body.Center, pbody.Center) < Constants.InteractionDistance)
                {
                    // Get the tangent
                    var facing = Math.Atan2(pbody.Center.Y - body.Center.Y, pbody.Center.X - body.Center.X);
                    var val = Math.Abs(pphysics.Direction - facing);

                    if (Math.Floor(val) == 3)
                    {
                        if (GameInput.Self.KeyPressed(Keys.Enter) && coolDown <= 0)
                        {
                            var table = BeyondAge.Assets.GetDialogTable("npc");
                            var dialogTable = table[character.Name] as LuaTable;
                            
                            BeyondAge.TheGame.DoDialog(dialogTable, (TimesTalkedToByPlayer == 0) ? 1 : 2);

                            TimesTalkedToByPlayer++;
                            coolDown = maxCoolDown;
                        }
                    }

                    if (coolDown > 0)
                        coolDown -= (float)(time.ElapsedGameTime.TotalSeconds);
                }
            }
        }

        public override void Draw(Entity ent, SpriteBatch batch)
        {
            if (player != null)
            {
                var body = ent.Get<Body>();
                var sprite = ent.Get<Sprite>();
                var physics = ent.Get<PhysicsBody>();
                var pbody = player.Get<Body>();
                var pphysics = player.Get<PhysicsBody>();

                if (Vector2.Distance(body.Center, pbody.Center) < Constants.InteractionDistance)
                {
                    // Get the tangent
                    var facing = Math.Atan2(pbody.Center.Y - body.Center.Y, pbody.Center.X - body.Center.X);
                    var val = Math.Abs(pphysics.Direction - facing);

                    if (Math.Floor(val) == 3)
                    {
                        primitives.DrawRect(new Rectangle((body.Position - new Vector2(0, sprite.Region.Height * Constants.SCALE + 16)).ToPoint(), body.Size.ToPoint()), Color.SlateGray);
                    }
                }
            }
        }
    }
}
