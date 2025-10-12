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
	public static MenuScreen MScreen = new MenuScreen();
	
	//if the audio device is ready or not
	public static bool AudioReady = false;
	
	//objects
	public static List<gObj> gameObject = new List<gObj>() { };
	public static SB_Player playerObject = null;
	
	//objects slated for removal from the gameObject list
	private static List<gObj> unlinkedObject = new List<gObj>() { };
	
	//hitscan lines to be drawn
	public static List<Screen.HitscanLine> hscanLines = new List<Screen.HitscanLine>() { };
	//to be unlinked
	private static List<Screen.HitscanLine> unlinkedLines = new List<Screen.HitscanLine>() { };
	
	//"public static void main deez nuts" - siveine
	public static void Main(){
		//init the window
		SetTraceLogLevel(TraceLogLevel.Debug);
		InitWindow(800,450,"Spellbullet");
		SetWindowMinSize(800,450);
		SetWindowState(ConfigFlags.ResizableWindow);
		SetTargetFPS(60);
		SetExitKey(KeyboardKey.Null);
		
		InitAudioDevice();
		AudioReady = IsAudioDeviceReady();
		
		Haptic.GetController();
		
		//init game variables
		AssetManager.Init();
		Screen.Init();
		
		playerObject = new SB_Player(0.0f,0.0f);
		
		new TestGun(120.0f,-20.0f);
		new SB_TestEnemy(260.0f,0.0f);
		
		//main game loop
		while(!WindowShouldClose()){
			//for haptics to update
			SDL.SDL_PumpEvents();
			
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
				//check menu button input
				ButtonPress menubtn = MScreen.CheckButtonClick();
				Input.ProcessMenuClick(menubtn);
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
				//draw HITSCANS
				foreach(Screen.HitscanLine todraw in hscanLines){
					if(todraw.tics <= 0){
						unlinkedLines.Add(todraw);
						continue;
					}
					Screen.DrawHitscanLine(todraw);
					todraw.tics--;
				}
				//unlink HITSCANS
				foreach(Screen.HitscanLine toremove in unlinkedLines){
					hscanLines.Remove(toremove);
				}
				unlinkedLines = new List<Screen.HitscanLine>() { };
				//draw OBJECTS
				foreach(gObj obj in gameObject){
					if(obj != playerObject)
						Screen.DrawObject(obj);
				}
				//draw PLAYER CONE OF FIRE
				if(playerObject.ReadyWeapon != null){
					double spread = playerObject.ReadyWeapon.spread * (1.0d + playerObject.recoil);
					DrawCircleSector(playerObject.pos - playerObject.Camera,600.0f,
						(float)(playerObject.angle - spread) - 90.0f,(float)(playerObject.angle + spread) - 90.0f,
						2,new Color(0x66,0x33,0x33,0x44));
				}
				//draw PLAYER
				Screen.DrawObject(playerObject);
				
				// THE HEADS UP DISPLAY
				//~=====================~
				//draw WEAPON
				if(playerObject.ReadyWeapon != null){
					DrawText("WEAPON",0,380,10,Color.RayWhite);
					DrawText("" + playerObject.ReadyWeapon.Tag,10,390,20,Color.RayWhite);
					Color ammocolor = Color.Orange;
					if(playerObject.ReadyWeapon.count <= playerObject.ReadyWeapon.maxCount / 3)
						ammocolor = new Color(0xff,0x00,0x00,0xff);
					if(playerObject.ReadyWeapon.count == 0)
						ammocolor = Color.Gray;
					DrawText("" + playerObject.ReadyWeapon.count + "/" +  playerObject.ReadyWeapon.maxCount,15,410,20,ammocolor);
				}
				
				//draw MENUS
				if(Menu == MenuState.Inventory)
					MScreen = new InventoryMenu();
				Screen.DrawGameMenu(MScreen);
				
				break;
			}
			BeginTextureMode(Screen.ScreenCanvas[1]);
			Screen.Flip(0,false);
			EndTextureMode();
			Screen.Flip(1);
			EndDrawing();
		}
		
		//program deinitialization
		Haptic.ShutdownSDL();
		
		Screen.DeInit();
		AssetManager.DeInit();
		
		CloseAudioDevice();
		
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