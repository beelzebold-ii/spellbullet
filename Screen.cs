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
		//apparently the rotation origin parameter also offsets so we don't need to apply that offset here
		Vector2 drawpos = Vector2.Round(o.pos - Program.playerObject.Camera/* - offset*/);
		Rectangle srect = new Rectangle(0, 0, tex.Width, tex.Height);
		Rectangle drect = new Rectangle(drawpos.X, drawpos.Y, tex.Width, tex.Height);
		//here offset, being the vector from corner to center, is exactly our rotation origin, so that's why that's there
		DrawTexturePro(tex, srect, drect, offset, (float)o.angle, Color.White);
		
		//draw nametag if o is invObj and near player pickuprange
		if(o is invObj && CheckCollisionPointCircle(o.pos,Program.playerObject.pos,SB_Player.PickupRange)){
			Vector2 txtpos = new Vector2(o.pos.X,o.pos.Y + offset.Y);
			//txtpos = o.pos; //for debugging
			txtpos = Vector2.Round(txtpos - Program.playerObject.Camera);
			
			DrawText("" + ((invObj)o).Tag,(int)txtpos.X,(int)txtpos.Y,10,Color.RayWhite);
		}
		
		//draw collision radius if o is eObj
		if(o is eObj){
			DrawCircleLines((int)(drawpos.X),(int)(drawpos.Y),((eObj)o).radius,new Color(0x00,0x55,0x00,0xff));
		}
	}
	
	//hitscan lines
	public class HitscanLine{
		public int tics;
		public Vector2 p1;
		public Vector2 p2;
		
		public HitscanLine(Vector2 a,Vector2 b){
			p1 = a;
			p2 = b;
			tics = 15;
			
			Program.hscanLines.Add(this);
		}
	}
	public static void DrawHitscanLine(HitscanLine todraw){
		if(todraw.tics <= 0)
			return;
		DrawLineV(todraw.p1 - Program.playerObject.Camera,todraw.p2 - Program.playerObject.Camera,
			new Color(0xff,0xaa,0x33,(int)(System.Math.Clamp(todraw.tics / 9.0f * 255.0f,0.0f,200.0f))));
	}
	
	//draw menu stuff
	public static void DrawGameMenu(MenuScreen gmenu){
		foreach(MenuScreen.Button btn in gmenu.buttons){
			//draw texture
			if(btn.texture != "TNT1"){
				Texture2D tex;
				if(!AssetManager.Textures.TryGetValue(btn.texture,out tex)){
					TraceLog(TraceLogLevel.Error,"SCREEN: Texture \"" + btn.texture + "\" not found");
					btn.texture = "TNT1";
					continue;
				}
				DrawTextureEx(tex, (btn.position + (btn.size/2.0f)) - (new Vector2(tex.Width / 2,tex.Height / 2) * btn.texscale), 0.0f, btn.texscale, btn.col);
			}
			//draw text
			if(btn.txtlabel != ""){
				DrawText(btn.txtlabel,(int)btn.position.X,(int)btn.position.Y,btn.txtsize,btn.col);
			}
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