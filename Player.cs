using System.Numerics;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// player eObj class
//~==================~
class SB_Player:eObj{
	//go figure
	public override bool isPlayer => true;
	
	public override int spawnHealth => 100;
	public override float radius => 22;
	public override float mass => 50;
	public override float speed => 0.66f;
	
	public float stamina = 100.0f;
	
	//item stuff
	public const float PickupRange = 32.0f;
	public const int MaxInventory = 9;
	
	//weapon stuff
	public Weapon ReadyWeapon = null;
	public int fireDelay = 0;
	public float recoil = 0.0f;
	
	//action timer is used by reloading or using other items for the timer animation
	public int maxActionTimer = 0;
	public int actionTimer = 0;
	
	//camera position (absolute)
	protected Vector2 camera = Vector2.Zero;
	public Vector2 Camera{ get => camera; }
	
	//apparently empty constructors are still needed because the base constructor takes required arguments.
	public SB_Player(float pox,float poy) : base(pox,poy){
		SetSprite("player");
		//ReadyWeapon = new Pistol9mm(0.0f,0.0f);
		//ReadyWeapon.AttachTo(this);
		ReadyWeapon = (Weapon)GiveInventory("Pistol9mm",15);
		GiveInventory("Ammo9mm",30);
	}
	
	public override void Tick(){
		camera = pos - new Vector2(400,225);
		
		float stunRatio = stun/(float)spawnHealth;
		float painRatio = pain/(float)spawnHealth;
		float damageRatio = (spawnHealth - health)/(float)spawnHealth;
		float bloodlossRatio = 5.0f - bloodVol / (spawnHealth / 40.0f); // 100/40=2.5
		
		int totalwound = 0;
		foreach(int w in BleedingWounds){
			totalwound += w;
		}
		foreach(int w in PatchedWounds){
			totalwound += w;
		}
		
		int minStun = (int)(bloodlossRatio * 100 + damageRatio * 20);
		int minPain = (int)(totalwound + damageRatio * 30);
		
		float maxStamina = painRatio * 100.0f;
		//drops below zero at 60% stun or 1.5 liters of blood lost
		//at which point this multiplies by ninety, so passive stamina degeneration is 90 times faster than regeneration
		float regenStamina = 0.15f - stunRatio * 0.25f;
		//at 65% stun you'd have about 88 seconds of consciousness from full stamina which is hopefully plenty to stop yourself from actively dying
		//at ~75% stun you'd have only about 29 seconds of consciousness from full which is still a lot
		if(regenStamina < 0.0f)
			regenStamina *= 90.0f;
		//past 75% stun degeneration is doubled.
		//this means at 75% stun you only get 14 seconds of consciousness from full
		//this means if you hit 75% base stun (~1.8 liters of bloodloss) you are most likely going to die
		if(stunRatio >= 0.75f)
			regenStamina *= 2.0f;
		stamina += regenStamina;
		stamina = System.Math.Clamp(stamina,-5.0f,maxStamina);
		
		Vector2 MoveVec = Input.GetMovementVector();
		if(stun > 20)
			vel += MoveVec * speed * (0.6f - stunRatio / 2.0f);
		else
			vel += MoveVec * speed * (1.0f - stunRatio / 2.5f);
		
		//was just for testing rotation
		//angle += 1.0d;
		//NormalizeAngle();
		
		//TURN TOWARDS MOUSE
		angle = Input.GetAimAngle();
		NormalizeAngle();
		
		//REDUCE ACTION TIMER
		if(actionTimer > 0)
			actionTimer--;
		
		//HANDLE WEAPON DELAY AND RECOIL
		if(fireDelay > 0)
			fireDelay--;
		if(ReadyWeapon != null){
			if(recoil > 0.0f){
				if(stun > 20)
					recoil -= ReadyWeapon.Recovery * (1.0f - stunRatio / 2.0f);
				else
					recoil -= ReadyWeapon.Recovery * (1.0f - stunRatio / 4.0f);
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
				fireDelay = (int)(ReadyWeapon.Reload() * (1.0f + painRatio * 1.5f));
				actionTimer = fireDelay;
				maxActionTimer = actionTimer;
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