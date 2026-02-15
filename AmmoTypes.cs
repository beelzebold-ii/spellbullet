using System.Numerics;

namespace Spellbullet;

class Ammo9mm:Ammo{
	public override int maxCount => 90;
	
	public Ammo9mm(float pox,float poy,int cnt = -1):base(pox,poy,cnt){
		
	}
}
class Ammo35:Ammo{
	public override int maxCount => 60;
	
	public Ammo35(float pox,float poy,int cnt = -1):base(pox,poy,cnt){
		
	}
}
class Ammo12g:Ammo{
	public override int maxCount => 15;
	
	public Ammo12g(float pox,float poy,int cnt = -1):base(pox,poy,cnt){
		
	}
}