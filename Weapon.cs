using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// base class for weapons
//~=======================~
abstract class Weapon:invObj{
	//weapons won't consolidate into each other
	protected override bool CanConsolidate => false;
	
	//how much recoil this weapon incurs per shot
	//recoil is essentially just a multiplier for spread
	public virtual float Recoil => 2.25f;
	//how much recoil the player recovers from per tic with this weapon
	//if this makes up for all of recoil/shot within the weapon's firing delay then there's effectively zero recoil/shot
	public virtual float Recovery => 0.35f;
	//the highest amount of recoil the weapon is allowed to reach
	public virtual float MaxRecoil => 10.5f;
	//whether or not the weapon can be fired full auto
	public virtual bool cyclic => true;
	//supposed base spread used only for drawing the spread cone in front of the player
	public virtual double spread => 6.0f;
	//what ammo the weapon reloads with/gives on unload
	public abstract string AmmoClass{ get; }
	
	public Weapon(float pox,float poy,int cnt = 1) : base(pox,poy,cnt){
		
	}
	
	const float HITSCAN_MAXRANGE = 800.0f;
	const double WEAPON_PAINACC = 12.0d;
	
	protected void Hitscan(double angleofs,int damage,int wounding){
		//TraceLog(TraceLogLevel.Debug,"WEAPON: Fired Hitscan at angle offset " + angleofs + " with dmg/wound of " + damage + "/" + wounds);
		
		double lineangle = owner.angle + angleofs;
		lineangle *= 3.14159d/180.0d; //I fucking hate radians.
		Vector2 LineVec = new Vector2((float)System.Math.Sin(lineangle), -(float)System.Math.Cos(lineangle));
		Vector2 p1 = owner.pos;
		Vector2 p2 = p1 + (LineVec * HITSCAN_MAXRANGE);
		
		new Screen.HitscanLine(p1,p2,new Color(0xff,0xaa,0x33,0xff));//default orange color
		//new Screen.HitscanLine(p1,p2,new Color(0x44,0x44,0xaa,0xff));//weird fucked up blue color for fuckin around
		//new Screen.HitscanLine(p1,p2,new Color(0x67,0x3a,0xb7,0xff));//undead zeratul's personal color???
	}
	protected void FireBullets(double basespread,int basedmg,int wounding = 0,float variation = 1.5f,int shotcount = 1){
		//increase spread with recoil
		if(owner is SB_Player){
			SB_Player pla = (SB_Player)owner;
			TraceLog(TraceLogLevel.Debug,"WEAPON: Attack with recoil " + pla.recoil);
			basespread *= 1.0d + pla.recoil;
		}
		//increase spread with pain
		float painRatio = owner.pain / (float)owner.spawnHealth;
		basespread += painRatio * WEAPON_PAINACC;
		
		int roundedspread = (int)(basespread * 10);
		int maxdmg = (int)(basedmg * variation);
		for(int i = 0;i < shotcount;i++){
			Hitscan(GetRandomValue(-roundedspread,roundedspread) / 10.0d,GetRandomValue(basedmg, maxdmg),wounding);
		}
	}
	
	protected void ApplyRecoil(){
		if(!(owner is SB_Player))
			return;
		((SB_Player)owner).recoil += Recoil;
		((SB_Player)owner).recoil = System.Math.Min(((SB_Player)owner).recoil,MaxRecoil);
		if(IsGamepadAvailable(0)){
			Haptic.Rumble(Recoil/16.0f,Recoil/8.0f,Recoil/20.0f);
		}
	}
	
	//returns the delay until the next attack will occur
	public abstract int Attack();
	
	//actually reload the gun if possible; returns one of a few statuses - 0 (failed), 1 (from partial), 2 (from empty)
	protected int DoReload(){
		if(count >= maxCount){
			return 0;
		}
		int oldcount = count;
		
		//array of empty invObjs to remove
		List<invObj> emptyObjs = new List<invObj>() { };
		//start from the end of the list to maintain consolidation
		for(int i=owner.Inventory.Count-1;i>=0;i--){
			invObj thisitem = owner.Inventory[i];
			if(thisitem.GetType().Name == AmmoClass){
				TraceLog(TraceLogLevel.Debug,"WEAPON: Reload found valid item");
				int diff = System.Math.Min(maxCount - count,thisitem.count);
				thisitem.count -= diff;
				count += diff;
				
				if(thisitem.count <= 0)
					emptyObjs.Add(thisitem);
				
				if(count >= maxCount)
					break;
			}
		}
		
		foreach(invObj iii in emptyObjs){
			owner.Inventory.Remove(iii);
		}
		
		if(oldcount == count)
			return 0;
		if(oldcount == 0)
			return 2;
		return 1;
	}
	//attempt to reload; returns the time it takes to reload
	//this default just returns tics based on the status result of DoReload
	public virtual int Reload(){
		return DoReload();
	}
}

// and a test weapon
class TestGun:Weapon{
	protected override string tag => "Test";
	public override int maxCount => 75;
	public override double spread => 1.5f;
	public override float Recoil => 1.5f;
	public override float MaxRecoil => 50.0f;
	public override string AmmoClass => "Ammo9mm";
	
	public TestGun(float pox,float poy,int cnt = -1) : base(pox,poy,cnt){
		SetSprite("smsnb0");
	}
	
	public override int Attack(){
		if(count <= 0){
			return 10;
		}
		
		//count--;
		PlaySound(AssetManager.Sounds["pistol"]);
		FireBullets(spread,5);
		
		ApplyRecoil();
		
		return 1;//who fuckin knows man, 60r/s
	}
}

// base class for ammo items
//~==========================~
abstract class Ammo:invObj{
	public Ammo(float pox,float poy,int cnt = -1):base(pox,poy,cnt){
		
	}
}