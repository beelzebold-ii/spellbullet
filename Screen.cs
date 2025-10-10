using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

//class which contains many functions for drawing thingies
class Screen{
	public static RenderTexture2D[] ScreenCanvas = new RenderTexture2D[] { new RenderTexture2D(), new RenderTexture2D() };
	
	public static void Init(){
		TraceLog(TraceLogLevel.Debug,"SCREEN: Init RenderTextures");
		ScreenCanvas[0] = LoadRenderTexture(800,450);
		ScreenCanvas[1] = LoadRenderTexture(800,450);
	}
	public static void DeInit(){
		TraceLog(TraceLogLevel.Debug,"SCREEN: De-Init RenderTextures");
		UnloadRenderTexture(ScreenCanvas[0]);
		UnloadRenderTexture(ScreenCanvas[1]);
	}
	
	public static void DrawObject(gObj o){
		if(o.Sprite == "TNT1")
			return;
		
		Texture2D tex;
		if(!AssetManager.Textures.TryGetValue(o.Sprite,out tex)){
			TraceLog(TraceLogLevel.Error,"SCREEN: Texture \"" + o.Sprite + "\" not found");
			o.SetSprite("TNT1");
			return;
		}
		Vector2 offset = new Vector2(tex.Width/2,tex.Height/2);
		//this doesn't seem to be correctly rotating. so I am going to cry until it fixes itself
		DrawTextureEx(tex, Vector2.Round(o.pos - Program.playerObject.Camera - offset), (float)o.angle, 1.0f, Color.White);
		
		if(o is invObj && CheckCollisionPointCircle(o.pos,Program.playerObject.pos,SB_Player.PickupRange)){
			Vector2 txtpos = new Vector2(o.pos.X,o.pos.Y + offset.Y);
			txtpos = Vector2.Round(txtpos - Program.playerObject.Camera);
			
			DrawText("" + ((invObj)o).Tag,(int)txtpos.X,(int)txtpos.Y,10,Color.RayWhite);
		}
	}
	
	//present what's on the rendertexture to the window
	public static void Flip(int i = 1,bool doscale = true){
		Vector2 position = Vector2.Zero;
		float scale = 1.0f;
		if(doscale == true){
			float wh = GetScreenHeight() / 450.0f;
			float ww = GetScreenWidth() / 800.0f;
			scale = System.Math.Min(ww,wh);
			
			position.X = (ww - scale) * 400;
		}
		DrawTextureEx(ScreenCanvas[i].Texture,position,0.0f,scale,Color.White);
	}
}