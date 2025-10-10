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
	public virtual float Recoil => 1.5f;
	//how much recoil the player recovers from per tic with this weapon
	//if this makes up for all of recoil/shot within the weapon's firing delay then there's effectively zero recoil/shot
	public virtual float Recovery => 0.2f;
	//the highest amount of recoil the weapon is allowed to reach
	public virtual float MaxRecoil => 5.5f;
	//whether or not the weapon can be fired full auto
	public virtual bool cyclic => true;
	
	public Weapon(float pox,float poy,int cnt = 1) : base(pox,poy,cnt){
		
	}
	
	protected void Hitscan(double angleofs,int damage,int wounds){
		TraceLog(TraceLogLevel.Debug,"WEAPON: Fired Hitscan at angle offset " + angleofs + " with dmg/wound of " + damage + "/" + wounds);
	}
	protected void FireBullets(double basespread,int basedmg,int wounding = -1,float variation = 2.0f,int shotcount = 1){
		if(wounding == -1)
			wounding = (int)(basedmg * 1.5f);
		
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
		
		ApplyRecoil();
	}
	
	protected void ApplyRecoil(){
		if(!(owner is SB_Player))
			return;
		((SB_Player)owner).recoil += Recoil;
		((SB_Player)owner).recoil = System.Math.Min(((SB_Player)owner).recoil,MaxRecoil);
	}
	
	//returns the delay until the next attack will occur
	public abstract int Attack();
}

// and a test weapon
class TestGun:Weapon{
	protected override string tag => "Submachinegun";
	public override int maxCount => 30;
	public override bool cyclic => false;
	
	public TestGun(float pox,float poy,int cnt = -1) : base(pox,poy,cnt){
		SetSprite("smsnb0");
	}
	
	public override int Attack(){
		if(count <= 0){
			return 10;
		}
		
		//count--;
		PlaySound(AssetManager.Sounds["pistol"]);
		FireBullets(3.0d,20);
		
		return 5;//720r/m;12r/s
	}
}