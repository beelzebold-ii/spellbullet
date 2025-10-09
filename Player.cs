using System.Numerics;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// player eObj class
//~==================~
class SB_Player:eObj{
	public override int spawnHealth => 100;
	
	public const float PickupRange = 32.0f;
	public const int MaxInventory = 6;
	
	//camera position (absolute)
	protected Vector2 camera = Vector2.Zero;
	public Vector2 Camera{ get => camera; }
	
	//apparently empty constructors are still needed because the base constructor takes required arguments.
	public SB_Player(float pox,float poy) : base(pox,poy){
		SetSprite("swordforniko");
	}
	
	public override void Tick(){
		camera = pos - new Vector2(400,225);
		
		Vector2 MoveVec = Input.GetMovementVector();
		vel += MoveVec;
		
		base.Tick();
		
		
		//ACTION INPUT HANDLING
		
		//PICKUP FROM GROUND
		if(IsKeyPressed(Input.PkupKey) || IsGamepadButtonPressed(0,Input.PkupBtn)){
			foreach(gObj obj in Program.gameObject){
				if(!(obj is invObj))
					continue;
				if(CheckCollisionPointCircle(obj.pos,pos,PickupRange) && Inventory.Count < MaxInventory){
					TraceLog(TraceLogLevel.Debug,"PLAYER: Picked up invObj");
					((invObj)obj).AttachTo(this);
					break;
				}
			}
		}
		//OPEN INVENTORY
		if(IsKeyPressed(Input.InvKey) || IsGamepadButtonPressed(0,Input.InvBtn)){
			if(Program.Menu == Program.MenuState.None){
				Program.Menu = Program.MenuState.Inventory;
			}else if(Program.Menu == Program.MenuState.Inventory){
				Program.Menu = Program.MenuState.None;
			}
		}
	}
}