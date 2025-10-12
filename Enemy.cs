using System.Numerics;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// test enemy
//~===========~
class SB_TestEnemy:eObj{
	public override int spawnHealth => 100;
	public override float radius => 22;
	public override float mass => 50;
	public override float speed => 0.2f;
	public SB_TestEnemy(float pox,float poy) : base(pox,poy){
		SetSprite("enemy");
	}
	
	public override void Tick(){
		Vector2 MoveVec = Vector2.Normalize(Program.playerObject.pos - pos);
		//vel += MoveVec * speed;
		
		base.Tick();
	}
}