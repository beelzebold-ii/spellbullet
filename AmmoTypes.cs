namespace Spellbullet;

class Ammo9mm:Ammo{
	public override int maxCount => 90;
	protected override string tag => "9mm ammo";
	
	public Ammo9mm(float pox,float poy,int cnt = -1):base(pox,poy,cnt){
		SetSprite("ammoa0");
	}
}
class Ammo35:Ammo{
	public override int maxCount => 60;
	protected override string tag => ".35 ammo";
	
	public Ammo35(float pox,float poy,int cnt = -1):base(pox,poy,cnt){
		SetSprite("ammoa0");
	}
}
class Ammo12g:Ammo{
	public override int maxCount => 15;
	protected override string tag => "12 gauge ammo";
	
	public Ammo12g(float pox,float poy,int cnt = -1):base(pox,poy,cnt){
		SetSprite("ammoa0");
	}
}