using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

class eObjHumanoid:eObj{
	public override int spawnHealth => 100;
	public override float radius => 22;
	public override float mass => 50;
	public override float speed => 0.2f;
	
	public eObjHumanoid(float pox,float poy) : base(pox,poy){
		if(!isPlayer)
			bloodVol = spawnHealth/40.0f;//100hp = 2.5l of blood
		else
			bloodVol = spawnHealth/20.0f;//100hp = 5.0l of blood
	}
}

class eObjBeast:eObj{
	public override int spawnHealth => 500;
	public override float radius => 34;
	public override float mass => 120;
	public override float speed => 0.1f;
	
	public eObjBeast(float pox,float poy) : base(pox,poy){
		if(!isPlayer)
			bloodVol = (float)System.Math.Sqrt(spawnHealth)/2.0f;//500hp = ~11.15l of blood
		else
			bloodVol = (float)System.Math.Sqrt(spawnHealth);//500hp = ~22.3l of blood
	}
}

class eObjConstruct:eObj{
	public override int spawnHealth => 120;
	public override float radius => 22;
	public override float mass => 60;
	public override float speed => 0.15f;
	
	public eObjConstruct(float pox,float poy) : base(pox,poy){
			bloodVol = 5.0f;//constructs don't bleed or have blood, so it's irrelevant
	}
}

class eObjElemental:eObj{
	public override int spawnHealth => 100;
	public override float radius => 22;
	public override float mass => 25;
	public override float speed => 0.3f;
	
	public eObjElemental(float pox,float poy) : base(pox,poy){
			bloodVol = 5.0f;//elementals don't bleed or have blood, so it's irrelevant
	}
}