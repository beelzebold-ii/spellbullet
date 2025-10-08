using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

//class which contains many functions for drawing thingies
class Screen{
	public static void DrawObject(gObj o){
		if(o.Sprite == "TNT1")
			return;
		Texture2D tex;
		if(!AssetManager.Textures.TryGetValue(o.Sprite,out tex)){
			TraceLog(TraceLogLevel.Error,"SCREEN: Texture \"" + o.Sprite + "\" not found");
			return;
		}
		Vector2 offset = new Vector2(tex.Width/2,tex.Height/2);
		DrawTextureEx(tex,o.pos - Program.playerObject.Camera - offset,0.0f,1.0f,Color.White);
	}
}