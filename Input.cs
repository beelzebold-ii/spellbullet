using System.Numerics;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

//input handling
class Input{
	public const float AxisEpsilon = 0.07f;
	
	//KEYBIND STUFF
	public class ActionBind{
		public KeyboardKey key;
		public GamepadButton btn;
		public MouseButton mou;
		public ActionBind(KeyboardKey k = KeyboardKey.Null,GamepadButton b = GamepadButton.Unknown,int m = -1){
			key = k;
			btn = b;
			if(m != -1)
				mou = (MouseButton)m;
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
	
	public static bool CheckActionBind(ActionBind bind,bool hold = false){
		if(hold)
			return (IsKeyDown(bind.key) || IsGamepadButtonDown(0,bind.btn) || IsMouseButtonDown(bind.mou));
		else
			return (IsKeyPressed(bind.key) || IsGamepadButtonPressed(0,bind.btn) || IsMouseButtonPressed(bind.mou));
	}
	
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
		if(AxisVector.Length() > Input.AxisEpsilon)
			MoveVec += AxisVector;
		
		//if vector length is zero normalizing will have weird results so just return as is
		if(MoveVec.Length() < Input.AxisEpsilon)
			return Vector2.Zero;
		
		//normalize the vector to a unit vector and return
		if(MoveVec.Length() > 0.9)
			MoveVec = Vector2.Normalize(MoveVec);
		return MoveVec;
	}
}