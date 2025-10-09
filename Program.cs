using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

class Program{
	//global variables will be listed as public static in the Program class
	public static List<gObj> gameObject = new List<gObj>() { };
	public static SB_Player playerObject = new SB_Player(0.0f,0.0f);
	
	static void Main(){
		//init the window
		InitWindow(800,450,"Spellbullet");
		SetTargetFPS(60);
		
		//init game variables
		SetTraceLogLevel(TraceLogLevel.Debug);
		
		AssetManager.Init();
		
		//new SubMachineGun(60.0f,-20.0f);
		
		//main game loop
		while(!WindowShouldClose()){
			foreach(gObj obj in gameObject){
				obj.Tick();
			}
			
			BeginDrawing();
			
			ClearBackground(Color.Black);
			
			foreach(gObj obj in gameObject){
				Screen.DrawObject(obj);
			}
			
			DrawText("" + GetGamepadAxisMovement(0,GamepadAxis.LeftX),0,0,20,Color.RayWhite);
			
			EndDrawing();
		}
		
		//program deinitialization
		AssetManager.DeInit();
		
		CloseWindow();
	}
}