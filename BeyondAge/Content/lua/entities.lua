--[[
var player = world.Create("player");
player.Add<Body>(new Body { X = 512, Y = 512, Width = 8 * Constants.SCALE, Height = 6 * Constants.SCALE });
player.Add<Sprite>(new Sprite(BeyondAge.Assets.GetTexture("character_sheet"), new Rectangle(0, 0, 8, 16)));
player.Add<PhysicsBody>(new PhysicsBody { });
player.Add<Player>(new Player());
player.Add<Illuminate>(new Illuminate(new PointLight
{
    Color = Color.White,
    Intensity = 1,
    Radius = 1500,
    Scale = new Vector2(1500)
}));
]]

local scaleFactor = 2

return {
    ["Player"] = {
        tags = {"player"},
		components = {
			["Body"] = {X = 512 + 200, Y = 512, Width = 8 * Constants.SCALE * scaleFactor, Height = 6 * Constants.SCALE * scaleFactor},
			--["Sprite"] = {Texture = "character_sheet", Region = {0, 0, 16, 32}, Scale = {1, 1}},
			["PhysicsBody"] = {},
			["Player"] = {},
			["Illuminate"] = {
				Color = {1, 1, 1, 1},
				Intensity = 1,
				Radius = 1500,
				Scale = 1500
			},
			["Health"] = {}
		}
    },

	["Npc1"] = {
		tags = {"npc", "friendly"},
		components = {
			["Body"] = {X = 512 + 200, Y = 512, Width = 8 * Constants.SCALE, Height = 6 * Constants.SCALE},
			["Sprite"] = {Texture = "character_sheet", Region = {0, 0, 8, 16}, Color = {0, 0, 1, 1}},
			["PhysicsBody"] = {},
			["Character"] = {Name = "Bilmith"}
		}
	}
}
