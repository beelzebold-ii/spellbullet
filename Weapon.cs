using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// base class for weapons
//~=======================~
abstract class Weapon:invObj{
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
	
	public Weapon(float pox,float poy,int cnt = 1) : base(pox,poy,cnt){
		
	}
	
	const float HITSCAN_MAXRANGE = 800.0f;
	
	protected void Hitscan(double angleofs,int damage,int wounds){
		//TraceLog(TraceLogLevel.Debug,"WEAPON: Fired Hitscan at angle offset " + angleofs + " with dmg/wound of " + damage + "/" + wounds);
		
		double lineangle = owner.angle + angleofs;
		lineangle *= 3.14159d/180.0d; //I fucking hate radians.
		Vector2 LineVec = new Vector2((float)System.Math.Sin(lineangle), -(float)System.Math.Cos(lineangle));
		Vector2 p1 = owner.pos;
		Vector2 p2 = p1 + (LineVec * HITSCAN_MAXRANGE);
		
		new Screen.HitscanLine(p1,p2);
	}
	protected void FireBullets(double basespread,int basedmg,int wounding = -1,float variation = 2.0f,int shotcount = 1){
		if(wounding == -1)
			wounding = (int)(basedmg / 2.0f);
		
		//increase spread with recoil
		if(owner is SB_Player){
			SB_Player pla = (SB_Player)owner;
			TraceLog(TraceLogLevel.Debug,"WEAPON: Attack with recoil " + pla.recoil);
			basespread *= 1.0d + pla.recoil;
		}
		
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
}

// and a test weapon
class TestGun:Weapon{
	protected override string tag => "Test";
	public override int maxCount => 75;
	public override double spread => 1.5f;
	public override float Recoil => 1.5f;
	public override float MaxRecoil => 50.0f;
	
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