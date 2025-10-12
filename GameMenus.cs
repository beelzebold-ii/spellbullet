using System.Numerics;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

// BIG TODO:
// MenuScreen gamepad support!
// with an index for what button is selected
// and virtual methods for changing that
// based on dpad or left analog stick input

// struct containing button identifier and idnumber
//~=================================================~
struct ButtonPress{
	public string Ident = "none";
	public int Index = -1;
	public ButtonPress(string ide = "none",int ind = -1){
		Ident = ide;
		Index = ind;
	}
}

// base class for menus that can have buttons and images/text
//~===========================================================~
class MenuScreen{
	public class Button{
		public Vector2 position = Vector2.Zero;
		public Vector2 size = Vector2.Zero;
		public string texture = "TNT1";
		public float texscale = 1.0f;
		public string txtlabel = "";
		public int txtsize = 20;
		public string identifier = "none";
		public Color col = Color.White;
		public Button(Vector2 pos,Vector2 siz = default,string ident = "none",string tex = "TNT1",float scale = 1.0f,string txt = "",int txtsiz = 20,Color? btncol = null){
			position = pos;
			size = siz;
			texture = tex;
			texscale = scale;
			txtlabel = txt;
			txtsize = txtsiz;
			identifier = ident;
			
			if(btncol == null){
				col = Color.White;
			}else{
				col = (Color)btncol;
			}
		}
	}
	public List<Button> buttons = new List<Button>() { };
	
	public ButtonPress CheckButtonClick(){
		if(IsMouseButtonPressed(MouseButton.Left)){
			float wh = GetScreenHeight() / 450.0f;
			float ww = GetScreenWidth() / 800.0f;
			float scale = System.Math.Min(ww,wh);
			Vector2 mou = new Vector2(GetMouseX() / scale, GetMouseY() / scale);
			mou.X -= ((ww - scale) * 400.0f) / scale;
			System.Console.WriteLine("Mouse click " + mou.X + "," + mou.Y);
			int i = 0;
			foreach(Button btn in buttons){
				if(btn.identifier == "none")
					continue;
				if(mou.X > btn.position.X && mou.Y > btn.position.Y && mou.X < btn.position.X + btn.size.X && mou.Y < btn.position.Y + btn.size.Y){
					TraceLog(TraceLogLevel.Debug,"GAMEMENU: Button click of identifier \"" + btn.identifier + "\" id " + i);
					return new ButtonPress(btn.identifier, i);
				}
				i++;
			}
		}
		return new ButtonPress("none",-1);
	}
	
	//if this is true then disable all mouse keybinds
	public bool useMouse = false;
	//ditto but for dpad and left analog stick
	public bool useDPad = false;
	
	public MenuScreen(){
		/*
		useMouse = true;
		useDPad = true;
		buttons.Add(new Button( new Vector2(60.0f,40.0f), new Vector2(20.0f,40.0f), "test", "mButton50x30", 1.0f, "" ));
		buttons.Add(new Button( new Vector2(60.0f,40.0f), Vector2.Zero, "none", "smsnb0", 2.0f, "Submachinegun", 20, Color.RayWhite ));
		*/
	}
}

// INVENTORY MENU SCREEN
//~======================~
class InventoryMenu:MenuScreen{
	protected void DrawInvSlot(Vector2 ButtonPos, invObj? item){
		Vector2 ButtonSize = new Vector2(120.0f,70.0f);
		Color? BtnCol = null;
		if(Program.playerObject.ReadyWeapon == item && item != null)
			BtnCol = new Color(0x66,0x66,0xdd,0xff);
		buttons.Add(new Button( ButtonPos, ButtonSize, "InventoryItemSlot", "mButton60x35", 2.0f, btncol:BtnCol));
		if(item != null){
			buttons.Add(new Button( ButtonPos + new Vector2(0.0f,-5.0f), ButtonSize, "none", item.Sprite, 2.0f));
			buttons.Add(new Button( ButtonPos + new Vector2(5.0f,45.0f), Vector2.Zero, "none", txt:item.Tag,txtsiz:10,btncol:Color.RayWhite));
			buttons.Add(new Button( ButtonPos + new Vector2(10.0f,55.0f), Vector2.Zero, "none", txt:item.count + "/" + item.maxCount,txtsiz:10,btncol:Color.RayWhite));
		}
	}
	
	public InventoryMenu(){
		useMouse = true;
		useDPad = true;
		buttons.Add(new Button( new Vector2(20.0f,15.0f), Vector2.Zero, "none", txt:"INVENTORY",txtsiz:20,btncol:Color.RayWhite));
		Vector2 ButtonPos;
		int i;
		invObj? item;
		
		//row 1
		ButtonPos = new Vector2(60.0f,60.0f);
		i = 0;
		item = null;
		if(Program.playerObject.Inventory.Count>i)
			item = Program.playerObject.Inventory[i];
		DrawInvSlot(ButtonPos,item);
		ButtonPos = new Vector2(185.0f,60.0f);
		i = 1;
		item = null;
		if(Program.playerObject.Inventory.Count>i)
			item = Program.playerObject.Inventory[i];
		DrawInvSlot(ButtonPos,item);
		ButtonPos = new Vector2(310.0f,60.0f);
		i = 2;
		item = null;
		if(Program.playerObject.Inventory.Count>i)
			item = Program.playerObject.Inventory[i];
		DrawInvSlot(ButtonPos,item);
		//row 2
		ButtonPos = new Vector2(60.0f,135.0f);
		i = 3;
		item = null;
		if(Program.playerObject.Inventory.Count>i)
			item = Program.playerObject.Inventory[i];
		DrawInvSlot(ButtonPos,item);
		ButtonPos = new Vector2(185.0f,135.0f);
		i = 4;
		item = null;
		if(Program.playerObject.Inventory.Count>i)
			item = Program.playerObject.Inventory[i];
		DrawInvSlot(ButtonPos,item);
		ButtonPos = new Vector2(310.0f,135.0f);
		i = 5;
		item = null;
		if(Program.playerObject.Inventory.Count>i)
			item = Program.playerObject.Inventory[i];
		DrawInvSlot(ButtonPos,item);
	}
}