using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// base class for weapons
//~=======================~
abstract class Weapon:invObj{
	public Weapon(float pox,float poy,int cnt = 1) : base(pox,poy,cnt){
		
	}
}

// and a test weapon
class SubMachineGun:Weapon{
	protected override string tag => "Submachinegun";
	public override int maxCount => 30;
	public SubMachineGun(float pox,float poy,int cnt = -1) : base(pox,poy,cnt){
		SetSprite("smsnb0");
	}
}