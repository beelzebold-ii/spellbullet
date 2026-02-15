using System.Numerics;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// player eObj class
//~==================~
class SB_Player:eObj{
	public override int spawnHealth => 100;
	public override float radius => 22;
	public override float mass => 50;
	public override float speed => 0.66f;
	
	public const float PickupRange = 32.0f;
	public const int MaxInventory = 6;
	
	//weapon stuff
	public Weapon ReadyWeapon = null;
	public int fireDelay = 0;
	public float recoil = 0.0f;
	
	//camera position (absolute)
	protected Vector2 camera = Vector2.Zero;
	public Vector2 Camera{ get => camera; }
	
	//apparently empty constructors are still needed because the base constructor takes required arguments.
	public SB_Player(float pox,float poy) : base(pox,poy){
		SetSprite("player");
		ReadyWeapon = new Pistol9mm(0.0f,0.0f);
		ReadyWeapon.AttachTo(this);
	}
	
	public override void Tick(){
		camera = pos - new Vector2(400,225);
		
		Vector2 MoveVec = Input.GetMovementVector();
		vel += MoveVec * speed;
		
		//was just for testing rotation
		//angle += 1.0d;
		//NormalizeAngle();
		
		//TURN TOWARDS MOUSE
		angle = Input.GetAimAngle();
		NormalizeAngle();
		
		//HANDLE WEAPON DELAY AND RECOIL
		if(fireDelay > 0)
			fireDelay--;
		if(ReadyWeapon != null){
			if(recoil > 0.0f){
				recoil -= ReadyWeapon.Recovery;
			}else{
				if(recoil < 0.0f)
					recoil = 0.0f;
			}
		}else{
			recoil = 0.0f;
		}
		
		base.Tick();
		
		
		//ACTION INPUT HANDLING
		
		//FIRE WEAPON
		if(ReadyWeapon != null && fireDelay <= 0){
			if(Input.CheckActionBind(Input.Fire,ReadyWeapon.cyclic)){
				fireDelay = ReadyWeapon.Attack();
			}
		}
		//RELOAD WEAPON
		if(ReadyWeapon != null && fireDelay <= 0){
			if(Input.CheckActionBind(Input.Reload)){
				fireDelay = ReadyWeapon.Reload();
			}
		}
		
		//PICKUP FROM GROUND
		if(Input.CheckActionBind(Input.Grab)){
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
		if(Input.CheckActionBind(Input.Inv)){
			if(Program.Menu == Program.MenuState.None){
				Program.Menu = Program.MenuState.Inventory;
				Program.MScreen = new InventoryMenu();
			}else if(Program.Menu == Program.MenuState.Inventory){
				Program.Menu = Program.MenuState.None;
				Program.MScreen = new MenuScreen();
			}
		}
		//EXIT MENUS
		if(Input.CheckActionBind(Input.Back)){
			if(Program.Menu != Program.MenuState.None){
				Program.Menu = Program.MenuState.None;
				Program.MScreen = new MenuScreen();
			}
		}
	}
}