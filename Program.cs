using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

class Program{
	public static List<gObj> gameObject = new List<gObj>() { };
	public static SB_Player playerObject = new SB_Player(0.0f,0.0f);
	
	static void Main(){
		//init the window
		InitWindow(800,450,"Spellbullet");
		SetTargetFPS(60);
		
		//init game variables
		SetTraceLogLevel(TraceLogLevel.Debug);
		
		
		
		//main game loop
		while(!WindowShouldClose()){
			foreach(gObj obj in gameObject){
				obj.Tick();
			}
			
			BeginDrawing();
			
			ClearBackground(Color.RayWhite);
			
			EndDrawing();
		}
		
		//program deinitialization
		CloseWindow();
	}
}