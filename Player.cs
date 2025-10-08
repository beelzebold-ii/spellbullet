using System.Numerics;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// player eObj class
//~==================~
class SB_Player:eObj{
	public override int spawnHealth => 100;
	
	//camera position (absolute)
	protected Vector2 camera = Vector2.Zero;
	public Vector2 Camera{ get => camera; }
	
	//apparently empty constructors are still needed because the base constructor takes required arguments.
	public SB_Player(float pox,float poy) : base(pox,poy){
		SetSprite("swordforniko");
	}
	
	//input handling
	class Input{
		public static Vector2 GetMovementVector(){
			KeyboardKey[] movekeys = new KeyboardKey[] { KeyboardKey.W, KeyboardKey.S, KeyboardKey.A, KeyboardKey.D };
			Vector2[] movevecs = new Vector2[] { new Vector2(0.0f,-1.0f), new Vector2(0.0f,1.0f), new Vector2(-1.0f,0.0f), new Vector2(1.0f,0.0f) };
			
			Vector2 MoveVec = Vector2.Zero;
			
			//iterate keys and apply their vectors
			for(int k = 0;k < 4;k++){
				if(IsKeyDown(movekeys[k])){
					MoveVec += movevecs[k];
				}
			}
			
			//if vector length is zero normalizing will have weird results so just return as is
			if(MoveVec.Length() == 0.0)
				return MoveVec;
			
			//normalize the vector to a unit vector and return
			return Vector2.Normalize(MoveVec);
		}
	}
	
	public override void Tick(){
		camera = pos - new Vector2(GetScreenWidth() / 2,GetScreenHeight() / 2);
		
		Vector2 MoveVec = Input.GetMovementVector();
		vel += MoveVec;
		
		base.Tick();
	}
}