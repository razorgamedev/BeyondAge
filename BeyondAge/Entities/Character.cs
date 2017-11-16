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

        public enum Type
        {
            Npc, 
            Player
        }

        public Type CharacterType { get; set; } = Type.Npc;

        public List<Clothing> Clothing { get; set; } = new List<Entities.Clothing>();
    }

    class CharacterController : Filter
    {
        private Entity player;
        private Primitives primitives;

        public int TimesTalkedToByPlayer { get; private set; } = 0;

        private float coolDown = 0;
        private float maxCoolDown { get => 1; }

        public CharacterController(Primitives primitives) : base(typeof(Body), typeof(Character), typeof(PhysicsBody))
        {
            this.primitives = primitives;
            this.optional.Add(typeof(Sprite));
            this.optional.Add(typeof(Animation));
        }

        public override void PreUpdate(GameTime time)
        {
            player = WorldRef.GetFirstWithComponent(typeof(Player));
        }

        public override void Update(Entity ent, GameTime time)
        {
            var character = ent.Get<Character>();
            if (player != null && character.CharacterType == Character.Type.Npc)
            {
                var body        = ent.Get<Body>();
                var sprite      = ent.Get<Sprite>();
                var physics     = ent.Get<PhysicsBody>();
                var pbody       = player.Get<Body>();
                var pphysics    = player.Get<PhysicsBody>();
                

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
            var character = ent.Get<Character>();
            if (player != null && character.CharacterType == Character.Type.Npc)
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
            
            // Drawing the clothing
            if (ent.Has(typeof(Animation)))
            {
                var sprite = ent.Get<Animation>();
                var body = ent.Get<Body>();
                var sheet = BeyondAge.Assets.GetTexture("character_sheet");
                foreach (var clothing in character.Clothing)
                {
                    int sector = ent.Has(typeof(PhysicsBody)) ? ent.Get<PhysicsBody>().Sector : 0;
                    if (clothing.ClothingType == Clothing.Type.Hair)
                    {
                        var region = new Rectangle(clothing.StartPos + new Point((int)(Clothing.HairSize.X * (sector - 1)), 0), Clothing.HairSize.ToPoint());
                        var ypos = body.Y + sprite.OffsetY - (region.Height * Constants.SCALE * sprite.ScaleY) + body.Height - (sprite.CurrentFrame.Rect.Height / 2 * Constants.SCALE) - Clothing.HairSize.Y / 2 - Constants.SCALE;

                        batch.Draw(
                           sprite.Texture,
                           new Rectangle(
                               (int)(body.X + sprite.OffsetX),
                               (int)(ypos),
                               (int)(region.Width * Constants.SCALE * sprite.ScaleX),
                               (int)(region.Height * Constants.SCALE * sprite.ScaleY)),
                           region,
                           sprite.Color,
                           0,
                           Vector2.Zero,
                           SpriteEffects.None,
                           sprite.DrawLayer + 0.011f
                        );
                    }

                    if (clothing.ClothingType == Clothing.Type.Shirt)
                    {
                        var region = new Rectangle(clothing.StartPos + new Point((int)(Clothing.HairSize.X * (sector - 1)), 0), Clothing.HairSize.ToPoint());
                        var ypos = body.Y + sprite.OffsetY - (region.Height * Constants.SCALE * sprite.ScaleY) + body.Height - 5 * Constants.SCALE;

                        batch.Draw(
                           sprite.Texture,
                           new Rectangle(
                               (int)(body.X + sprite.OffsetX),
                               (int)(ypos),
                               (int)(region.Width * Constants.SCALE * sprite.ScaleX),
                               (int)(region.Height * Constants.SCALE * sprite.ScaleY)),
                           region,
                           sprite.Color,
                           0,
                           Vector2.Zero,
                           SpriteEffects.None,
                           sprite.DrawLayer + 0.01f
                        );
                    }
                }
            }
        }
    }
}
