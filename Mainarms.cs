using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// SUBMACHINEGUN
//~==============~
class SubMachinegun:Weapon{
	protected override string tag => "Submachinegun";
	public override int maxCount => 30;
	public override float Recoil => 2.5f;
	public override float Recovery => 0.36f;
	public override double spread => 1.5f;
	public override string AmmoClass => "Ammo9mm";
	
	public SubMachinegun(float pox,float poy,int cnt = -1) : base(pox,poy,cnt){
		SetSprite("smsnb0");
	}
	
	public override int Attack(){
		if(count <= 0){
			return 10;
		}
		
		count--;
		PlaySound(AssetManager.Sounds["pistol"]);
		FireBullets(spread,15,5);
		
		ApplyRecoil();
		
		return 5;//720r/m;12r/s
	}
}