using System.Numerics;
using System.IO;
using System.Collections.Generic;

using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Spellbullet;

//class that contains access to any assets we may need
class AssetManager{
	public static Dictionary<string,Texture2D> Textures = new Dictionary<string,Texture2D>();
	
	public static void Init(){
		//init textures
		TraceLog(TraceLogLevel.Info,"ASSETS: Init Textures");
		string sourcedir = Directory.GetCurrentDirectory() + "\\textures";
		var files = Directory.EnumerateFiles(sourcedir);
		foreach(string thisfile in files){
			string fName = thisfile.Substring(sourcedir.Length + 1);
			fName = fName.Substring(0,fName.Length - 4);
			AssetManager.Textures.Add(fName,LoadTexture("textures\\" + fName + ".png"));
			TraceLog(TraceLogLevel.Debug,"ASSETS: Loaded Texture " + fName);
		}
	}
	
	public static void DeInit(){
		//deinit textures
		TraceLog(TraceLogLevel.Info,"ASSETS: De-Init Textures");
		foreach(string k in Textures.Keys){
			UnloadTexture(Textures[k]);
			if(Textures.Remove(k))
				TraceLog(TraceLogLevel.Debug,"ASSETS: Unloaded Texture " + k);
			else
				TraceLog(TraceLogLevel.Warning,"ASSETS: Failed to unload Texture" + k);
		}
	}
}