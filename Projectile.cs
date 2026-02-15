using System.Numerics;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// class for projectiles of any kind
//~==================================~
class Projectile:gObj{
	public override bool doFriction => true;
	
	//don't collide with the owner (still apply explode effects to them tho)
	protected gObj owner;
	
	//self explanatory stats describing the damage, stun, wounding, and fire applied to hit targets
	protected virtual int damage => 0;
	protected virtual int stun => 0;
	protected virtual int wound => 0;
	protected virtual int fire => 0;
	protected virtual int exploderad => -1;
	protected virtual int explodedmg => 0;
	
	//color for the inside of the explosion fx
	protected virtual Color explodecol => Color.Orange;
	
	//multiplier for knockback applied to the target
	protected virtual float knockback => 1.0f;
	protected virtual float explodekb => 1.0f;
	
	//projectile's radius
	protected virtual float radius => 6.0f;
	
	//the sprite name for the projectile. statenumber (+1) is appended to this
	protected virtual string projsprite => "TNT";
	//the number of states to cycle through
	protected virtual int spritecount => 1;
	//how many tics to spend on each frame of animation
	protected virtual int stateduration => 8;
	
	//very kind and nice constructor now that I'm not being STUPID
	public Projectile(gObj caller,float pox,float poy,float pvel, double angle) : base(pox,poy){
		owner = caller;
		vel = new Vector2((float)System.Math.Sin(angle) * pvel, -(float)System.Math.Cos(angle) * pvel);
	}
	
	public override void Tick(){
		//TODO: now to make it collide with things lol
	}
	
	//collision checking, called once per tick
	protected gObj DoObjectCollision(){
		//if our radius is above zero, check collision with every other eObj
		if(radius > 0.0f){
			foreach(gObj obj in Program.gameObject){
				if(obj == this)
					continue;//no self collision
				if(!(obj is eObj))
					continue;//can't collide with non eObjs
				eObj coll = (eObj)obj;
				if(coll.radius <= 0.0f)
					continue;//can't collide if radius zero
				if(!CheckCollisionCircles(pos,radius,coll.pos,coll.radius))
					continue;//obviously can't collide if we aren't touching
				return coll;
			}
		}
		
		return null;
	}
}