// SPELLBULLET
// Spellweaving and gunslinging topdown shooter!
// By yours truly, "Hellcat" Niko Chevrier
// Future graphics hopefully by Minerva "YourLocalCreechur"/"Maladjusted"

using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

class Program{
	//global variables will be listed as public static in the Program class
	//general game state stuff
	public enum State{
		Title,
		Menu,
		Game
	}
	public static State Gamestate = State.Game;
	public enum MenuState{
		None,
		Inventory,
		Pause
	}
	public static MenuState Menu = MenuState.None;
	
	//objects
	public static List<gObj> gameObject = new List<gObj>() { };
	public static SB_Player playerObject = new SB_Player(0.0f,0.0f);
	
	//objects slated for removal from the gameObject list
	private static List<gObj> unlinkedObject = new List<gObj>() { };
	
	//"public static void main deez nuts" - siveine
	public static void Main(){
		//init the window
		InitWindow(800,450,"Spellbullet");
		SetWindowMinSize(800,450);
		SetWindowState(ConfigFlags.ResizableWindow);
		SetTargetFPS(60);
		SetExitKey(KeyboardKey.F11);
		
		//init game variables
		SetTraceLogLevel(TraceLogLevel.Debug);
		
		AssetManager.Init();
		Screen.Init();
		
		new SubMachineGun(60.0f,-20.0f);
		
		//main game loop
		while(!WindowShouldClose()){
			switch(Gamestate){
			default:
			case State.Game:
			// GAME UPDATE
				foreach(gObj obj in gameObject){
					obj.Tick();
				}
				foreach(gObj obj in unlinkedObject){
					gameObject.Remove(obj);
				}
				unlinkedObject = new List<gObj>() { };
				break;
			}
			
			BeginDrawing();
			ClearBackground(new Color(0x11,0x11,0x11,0xff));
			BeginTextureMode(Screen.ScreenCanvas[0]);
			
			ClearBackground(Color.Black);
			
			switch(Gamestate){
			default:
			case State.Game:
			// GAME DRAW
				//draw OBJECTS
				foreach(gObj obj in gameObject){
					if(obj != playerObject)
						Screen.DrawObject(obj);
				}
				Screen.DrawObject(playerObject);
				
				//draw MENUS
				switch(Menu){
				default:
				case MenuState.None:
					break;
				case MenuState.Inventory:
				// INVENTORY DRAW
					int i = 0;
					DrawText("INVENTORY",0,0,20,Color.RayWhite);
					foreach(invObj item in playerObject.Inventory){
						i++;
						DrawText("" + item.Tag + " (" + item.count + ")",10,15 * i + 5,15,Color.RayWhite);
					}
					break;
				}
				break;
			}
			BeginTextureMode(Screen.ScreenCanvas[1]);
			Screen.Flip(0,false);
			EndTextureMode();
			Screen.Flip(1);
			EndDrawing();
		}
		
		//program deinitialization
		Screen.DeInit();
		AssetManager.DeInit();
		
		CloseWindow();
	}
	
	//method for slating any object for removal from the gameObject list
	public static void UnlinkObject(gObj toUnlink){
		unlinkedObject.Add(toUnlink);
	}
	public static void LinkObject(gObj toLink){
		gameObject.Add(toLink);
	}
}