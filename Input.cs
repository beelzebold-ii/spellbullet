using System.Numerics;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

//input handling
class Input{
	public const float AxisEpsilon = 0.07f;
	public static Vector2 GetMovementVector(){
		KeyboardKey[] movekeys = new KeyboardKey[] { KeyboardKey.W, KeyboardKey.S, KeyboardKey.A, KeyboardKey.D };
		GamepadButton[] padkeys = new GamepadButton[] { GamepadButton.LeftFaceUp, GamepadButton.LeftFaceDown, GamepadButton.LeftFaceLeft, GamepadButton.LeftFaceRight };
		Vector2[] movevecs = new Vector2[] { new Vector2(0.0f,-1.0f), new Vector2(0.0f,1.0f), new Vector2(-1.0f,0.0f), new Vector2(1.0f,0.0f) };
		
		Vector2 MoveVec = Vector2.Zero;
		
		//iterate keys and apply their vectors
		for(int k = 0;k < 4;k++){
			if(IsKeyDown(movekeys[k]) || IsGamepadButtonDown(0,padkeys[k])){
				MoveVec += movevecs[k];
			}
		}
		
		//apply gamepad analog axes
		//need to check against some small epsilon for deadzone
		Vector2 AxisVector = new Vector2(GetGamepadAxisMovement(0,GamepadAxis.LeftX), GetGamepadAxisMovement(0,GamepadAxis.LeftY));
		if(AxisVector.Length() > Input.AxisEpsilon)
			MoveVec += AxisVector;
		
		//if vector length is zero normalizing will have weird results so just return as is
		if(MoveVec.Length() == 0.0)
			return MoveVec;
		
		//normalize the vector to a unit vector and return
		if(MoveVec.Length() > 0.9)
			MoveVec = Vector2.Normalize(MoveVec);
		return MoveVec;
	}
}