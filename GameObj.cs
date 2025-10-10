using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// base class for ALL ingame objects
//~==================================~
abstract class gObj{
	//age of the object
	private int age = 0;
	public int Age{ get; }
	
	//friction coefficient
	const float GLOBAL_FRICTION = 0.85f;
	
	//position and velocity
	public Vector2 pos = Vector2.Zero;
	public Vector2 vel = Vector2.Zero;
	//yaw angle
	public double angle = 0.0;
	
	//whether or not to apply friction every tick
	public virtual bool doFriction => true;
	
	//constructor
	public gObj(float pox,float poy){
		TraceLog(TraceLogLevel.Debug,"GAMEOBJ: Creating gObj of type: " + this.GetType().Name);
		Program.gameObject.Add(this);
		
		pos = new Vector2(pox,poy);
	}
	
	//state can be used by any object logic
	protected int state = 0;
	//when state tics == zero, the object will be notified with a virtual function.
	protected int st_tics = 1;
	//this is said function
	protected virtual void NextState(int prevstate){}
	
	//and our ticker function
	public virtual void Tick(){
		//increment object's age
		age++;
		
		//apply velocity and friction
		pos += vel;
		if(doFriction)
			vel *= GLOBAL_FRICTION;
		
		//decrement state tics
		if(st_tics==0){
			st_tics--;
			NextState(state);
		}else{
			st_tics--;
		}
	}
	
	//what Texture to use? "TNT1" will skip drawing the object
	private string sprite = "TNT1";
	public string Sprite{ get => sprite; }
	public void SetSprite(string spr){
		if(!AssetManager.Textures.ContainsKey(spr)){
			TraceLog(TraceLogLevel.Error,"GAMEOBJ: Attempted to SetSprite to invalid Texture " + spr);
			return;
		}
		sprite = spr;
	}
	
	//normalize angle to -180..180
	public void NormalizeAngle(){
		angle = ((angle + 180.0d) % 360.0d) - 180.0d;
	}
}

// base class for "living" entities of any kind
//~=============================================~
abstract class eObj:gObj{
	//objhp is private, health is a property that will kill the eObj if it drops to zero
	private int objhp;
	public int health{
		get => objhp;
		set{
			if(value < objhp){
				OnDamage(objhp - value);
			}
			objhp = value;
			if(objhp <= 0){
				Die();
			}else{
				isdead = false;
			}
		}
	}
	//the health that the object should spawn with; read only and overridden in subclasses
	public abstract int spawnHealth{ get; }
	//whether or not the object is dead or not (rather self explanatory)
	private bool isdead = false;
	public bool isDead{ get => isdead; }
	
	//constructor for eObjs; hopefully constructors for subclasses are not necessary, but if they are, all should be well
	public eObj(float pox,float poy) : base(pox,poy){
		objhp = spawnHealth;
	}
	
	//and our ticker function
	public override void Tick(){
		base.Tick();
	}
	
	//method for killing the object
	public virtual void Die(){
		if(isDead)
			return;
		objhp = 0;
		isdead = true;
	}
	//method for UnKilling the object
	public void Resurrect(int reshealth = -1){
		if(!isDead)
			return;
		if(reshealth==-1)
			reshealth = spawnHealth;
		isdead = false;
		objhp = reshealth;
	}
	
	//virtual method called to notify the object when it takes damage
	//calling this does NOT damage the object, only makes it pretend it was damaged
	protected virtual void OnDamage(int dmg){}
	
	//this object's inventory
	public List<invObj> Inventory = new List<invObj>() { };
	public invObj GiveInventory(string classname,int count = 1){
		//ensure classname is of a valid invObj child class
		classname = "Spellbullet." + classname;
		Type invtype = null;
		try{
			invtype = Type.GetType(classname);
		}catch(Exception e){
			TraceLog(TraceLogLevel.Error,"GAMEOBJ: Attempted to give invalid invObj " + classname);
			return null;
		}
		if(!invtype.IsSubclassOf(typeof(invObj)))
			return null;
		
		//create and attach the item
		invObj item = (invObj) Activator.CreateInstance(invtype,new Object[] {0.0f,0.0f,count});
		item.AttachTo(this);
		
		return item;
	}
}

// base class for inventory items
//~===============================~
abstract class invObj:gObj{
	//the displayed name of the item
	protected virtual string tag => "";
	public string Tag{ get => tag; }
	
	//whether or not the item is attached to an object's inventory
	protected bool attached = false;
	public bool Attached{ get => attached; }
	protected eObj owner = null;
	//method for attaching the item to an object which also unlinks them from the main game object list
	public void AttachTo(eObj who){
		owner = who;
		attached = true;
		
		owner.Inventory.Add(this);
		
		Program.UnlinkObject(this);
	}
	
	//how many of the item?
	public int count;
	//how many of the item *can* there be?
	public virtual int maxCount => 1;
	
	//constructor
	public invObj(float pox,float poy,int num = -1) : base(pox,poy){
		count = num;
		if(count == -1)
			count = maxCount;
		
		//angle = GetRandomValue(-31415,31415)/10000.0d;
		//fuck radians man
		//angle *= 180.0d/3.14159d;
		angle = GetRandomValue(-1800,1800)/10.0d;
		NormalizeAngle();
		System.Console.WriteLine("invObj angle set to " + angle);
	}
	
	public override void Tick(){
		base.Tick();
	}
}