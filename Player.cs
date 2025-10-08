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
	
	public override void Tick(){
		camera = pos - new Vector2(GetScreenWidth() / 2,GetScreenHeight() / 2);
		
		Vector2 MoveVec = Input.GetMovementVector();
		vel += MoveVec;
		
		base.Tick();
	}
}