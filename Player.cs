using System.Numerics;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// player eObj class
//~==================~
class SB_Player:eObj{
	public override int spawnHealth => 100;
	
	public SB_Player(float pox,float poy) : base(pox,poy){}
	
	public override void Tick(){
		
		
		base.Tick();
	}
}