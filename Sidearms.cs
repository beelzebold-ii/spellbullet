using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// PISTOL
//~==============~
class Pistol9mm:Weapon{
	protected override string tag => "Pistol";
	public override int maxCount => 15;
	public override float Recoil => 1.75f;
	public override float Recovery => 0.1f;
	public override double spread => 3.0f;
	public override bool cyclic => false;
	
	public Pistol9mm(float pox,float poy,int cnt = -1) : base(pox,poy,cnt){
		//SetSprite("smsnb0");
	}
	
	public override int Attack(){
		if(count <= 0){
			return 10;
		}
		
		count--;
		PlaySound(AssetManager.Sounds["pistol"]);
		FireBullets(spread,15,10);
		
		ApplyRecoil();
		
		return 4;//800r/m;20r/s - if only it were cyclic
	}
}

// REVOLVER
//~==============~
class Revolver:Weapon{
	protected override string tag => "Revolver";
	public override int maxCount => 6;
	public override float Recoil => 3.0f;
	public override float Recovery => 0.16f;
	public override double spread => 2.0f;
	public override bool cyclic => false;
	
	public Revolver(float pox,float poy,int cnt = -1) : base(pox,poy,cnt){
		//SetSprite("smsnb0");
	}
	
	public override int Attack(){
		if(count <= 0){
			return 10;
		}
		
		count--;
		PlaySound(AssetManager.Sounds["pistol"]);
		FireBullets(spread,25,20,1.5f);
		
		ApplyRecoil();
		
		return 10;//535r/m;6r/s
	}
}