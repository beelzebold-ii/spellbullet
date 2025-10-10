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
	
	//returns the delay until the next attack will occur
	public abstract int Attack();
}

// and a test weapon
class TestGun:Weapon{
	protected override string tag => "Submachinegun";
	public override int maxCount => 30;
	public TestGun(float pox,float poy,int cnt = -1) : base(pox,poy,cnt){
		SetSprite("smsnb0");
	}
	
	public override int Attack(){
		
		return 4;//900r/m;15r/s
	}
}