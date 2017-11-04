using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeyondAge.Graphics
{
    

    class BgClouds : BgGraphics
    {       

        private class Cloud
        {
            public Vector2 Position;
            public float Scale;
            public float Speed;
        }

        Texture2D cloudTexture;
        private List<Cloud> clouds = new List<Cloud>();

        public BgClouds()
        {
            this.clouds = new List<Cloud>();
        }

        public override void Load()
        {
            this.cloudTexture = BeyondAge.Assets.GetTexture("cloud_1");
            var rnd = new Random();
            for (int i = 0; i < 750; i++) {
                //TODO(Dustin): Remove magic numbers
                clouds.Add(
                    new Cloud
                    {
                        Position = new Vector2(-1280 + ((float)(rnd.NextDouble()) * 1280f * 2), -1280 + ((float)(rnd.NextDouble()) * 1280f * 2)),
                        Scale = Constants.SCALE + (-4 + (float)rnd.NextDouble() * 8),
                        Speed = 1 + (float)rnd.NextDouble() * 2
                    });
            }
        }

        public override void Update(GameTime time)
        {
            for (int i = 0; i < clouds.Count; i++)
            {
                var cloud = clouds[i];
                cloud.Position += new Vector2((float)time.ElapsedGameTime.TotalSeconds * 3f * cloud.Speed, 0);
                cloud.Position.Y += (float)(Math.Sin((time.TotalGameTime.TotalSeconds + i) / 25f) / 25f);

                if (cloud.Position.X - cloudTexture.Width > 1280)
                {
                    cloud.Position.X = -cloudTexture.Width;
                    if ((cloud.Position.Y / cloudTexture.Height) % 2 == 0)
                        cloud.Position.X += cloudTexture.Width / 2;
                }
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            foreach(var cloud in clouds)
            {
                batch.Draw(
                    cloudTexture,
                    cloud.Position * Constants.SCALE,
                    new Rectangle(0, 0, cloudTexture.Width, cloudTexture.Height),
                    new Color(1, 1, 1, 0.8f),   
                    0,
                    Vector2.Zero,
                    cloud.Scale,
                    SpriteEffects.None,
                    0.01f);
            }
        }
        
        public override void Destroy()
        {
            clouds = new List<Cloud>();
        }

    }
}
