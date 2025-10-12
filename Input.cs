using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

//input handling
abstract class Input{
	public const float AxisEpsilon = 0.07f;
	public const float AimDeadzone = 0.6f;
	
	//KEYBIND STUFF
	public class ActionBind{
		public KeyboardKey key;
		public GamepadButton btn;
		public MouseButton mou;
		public bool hasmou = false;
		public ActionBind(KeyboardKey k = KeyboardKey.Null,GamepadButton b = GamepadButton.Unknown,int m = -1){
			key = k;
			btn = b;
			if(m != -1){
				mou = (MouseButton)m;
				hasmou = true;
			}
		}
	}
	
	//keys and buttons for movement
	public static KeyboardKey[] MoveKeys = new KeyboardKey[] { KeyboardKey.W, KeyboardKey.S, KeyboardKey.A, KeyboardKey.D };
	public static GamepadButton[] MoveBtn = new GamepadButton[] { GamepadButton.LeftFaceUp, GamepadButton.LeftFaceDown, GamepadButton.LeftFaceLeft, GamepadButton.LeftFaceRight };
	//bind for picking up items from the floor
	public static ActionBind Grab = new ActionBind(KeyboardKey.Q,GamepadButton.RightFaceUp);
	//bind for exiting menus
	public static ActionBind Back = new ActionBind(KeyboardKey.Escape,GamepadButton.RightFaceRight);
	//bind for opening inventory screen
	public static ActionBind Inv = new ActionBind(KeyboardKey.E,GamepadButton.MiddleLeft);
	//bind for firing weapon
	public static ActionBind Fire = new ActionBind(KeyboardKey.Null,GamepadButton.RightTrigger2,(int)MouseButton.Left);
	
	//action binds
	public static bool CheckActionBind(ActionBind bind,bool hold = false){
		if(hold)
			return (IsKeyDown(bind.key) || IsGamepadButtonDown(0,bind.btn) || (IsMouseButtonDown(bind.mou) && bind.hasmou && !Program.MScreen.useMouse));
		else
			return (IsKeyPressed(bind.key) || IsGamepadButtonPressed(0,bind.btn) || (IsMouseButtonPressed(bind.mou) && bind.hasmou && !Program.MScreen.useMouse));
	}
	
	//movement
	public static Vector2 GetMovementVector(){
		Vector2[] movevecs = new Vector2[] { new Vector2(0.0f,-1.0f), new Vector2(0.0f,1.0f), new Vector2(-1.0f,0.0f), new Vector2(1.0f,0.0f) };
		
		Vector2 MoveVec = Vector2.Zero;
		
		//iterate keys and apply their vectors
		for(int k = 0;k < 4;k++){
			if(IsKeyDown(Input.MoveKeys[k]) || IsGamepadButtonDown(0,Input.MoveBtn[k])){
				MoveVec += movevecs[k];
			}
		}
		
		//apply gamepad analog axes
		//need to check against some small epsilon for deadzone
		Vector2 AxisVector = new Vector2(GetGamepadAxisMovement(0,GamepadAxis.LeftX), GetGamepadAxisMovement(0,GamepadAxis.LeftY));
		if(AxisVector.Length() > AxisEpsilon)
			MoveVec += AxisVector;
		
		//if vector length is zero normalizing will have weird results so just return as is
		if(MoveVec.Length() < Input.AxisEpsilon)
			return Vector2.Zero;
		
		//normalize the vector to a unit vector and return
		if(MoveVec.Length() > 0.9)
			MoveVec = Vector2.Normalize(MoveVec);
		return MoveVec;
	}
	
	//aiming
	public static bool AnalogAim = false;
	public static double GetAimAngle(){
		Vector2 AxisVector = new Vector2(GetGamepadAxisMovement(0,GamepadAxis.RightX), GetGamepadAxisMovement(0,GamepadAxis.RightY));
		if(AxisVector.Length() > AxisEpsilon)
			AnalogAim = true;
		if(GetMouseDelta().Length() > 1.0f)
			AnalogAim = false;
		if(!AnalogAim){
			double aimangle = System.Math.Atan2(GetMouseX() - (GetScreenWidth()/2),(GetScreenHeight()/2) - GetMouseY());
			return aimangle * 180.0d / 3.14159d;// I FUCKING HATE RADIANS
		}else{
			double aimangle = System.Math.Atan2(AxisVector.X,-AxisVector.Y);
			if(AxisVector.Length() > AimDeadzone)
				return aimangle * 180.0d / 3.14159d;
			else
				return Program.playerObject.angle;
		}
	}
	
	//menu clicks
	public static void ProcessMenuClick(ButtonPress menubtn){
		if(menubtn.Index != -1)
			TraceLog(TraceLogLevel.Debug,"INPUT: ProcessMenuClick Ident: \"" + menubtn.Ident + "\" Index: " + menubtn.Index);
		switch(menubtn.Ident){
		default:
			TraceLog(TraceLogLevel.Debug,"INPUT: Unknown/invalid Ident");
			break;
		case "none":
			break;
		// INVENTORY MENU ITEM SLOTS
		case "InventoryItemSlot":
			int i = menubtn.Index;
			
			TraceLog(TraceLogLevel.Debug,"INPUT: Selected Inventory item slot " + i);
			
			if(Program.playerObject.Inventory.Count <= i){
				TraceLog(TraceLogLevel.Debug,"INPUT: Slot empty");
				break;
			}
			invObj item = Program.playerObject.Inventory[i];
			
			if(item is Weapon){
				TraceLog(TraceLogLevel.Debug,"INPUT: Slot contains Weapon");
				if(Program.playerObject.ReadyWeapon == item){
					Program.playerObject.ReadyWeapon = null;
				}else{
					Program.playerObject.ReadyWeapon = (Weapon)item;
				}
			}
			break;
		}
	}
}