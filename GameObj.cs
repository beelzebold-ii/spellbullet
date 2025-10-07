using System.Numerics;

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
	
	//whether or not to apply friction every tick
	public virtual bool doFriction => true;
	
	//constructor
	public gObj(float pox,float poy){
		TraceLog(TraceLogLevel.Debug,"GAMEOBJ: Creating gObj of type: " + this.GetType().Name);
		Program.gameObject.Add(this);
		
		pos = new Vector2(pox,poy);
	}
	
	//and our ticker function
	public virtual void Tick(){
		//increment object's age
		age++;
		
		//apply velocity and friction
		pos += vel;
		if(doFriction)
			vel *= GLOBAL_FRICTION;
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
}