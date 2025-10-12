//raylib cs appears to have disabled controller vibration.
//I want controller vibration.
//so, I have to do it myself.
//this should hopefully work exactly the same as raylib's internal set up.
using System;
using System.Runtime.InteropServices;
using System.Threading;

using Raylib_cs;
using static Raylib_cs.Raylib;

public static class SDL{
	private const string SDL_LIB = "interop\\SDL2.dll";//"SDL2.dll" on windows, "libSDL2.so" on linux
	
	[DllImport(SDL_LIB, CallingConvention = CallingConvention.Cdecl)]
	public static extern int SDL_Init(uint flags);
	
	[DllImport(SDL_LIB, CallingConvention = CallingConvention.Cdecl)]
	public static extern void SDL_Quit();
	
	[DllImport(SDL_LIB, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr SDL_GameControllerOpen(int index);
	
	[DllImport(SDL_LIB, CallingConvention = CallingConvention.Cdecl)]
	public static extern void SDL_GameControllerClose(IntPtr controller);
	
	[DllImport(SDL_LIB, CallingConvention = CallingConvention.Cdecl)]
	public static extern bool SDL_GameControllerHasRumble(IntPtr controller);
	
	[DllImport(SDL_LIB, CallingConvention = CallingConvention.Cdecl)]
	public static extern int SDL_GameControllerRumble(IntPtr controller,
		ushort lowFreqRumble, ushort highFreqRumble, uint durationMs);
	
	[DllImport(SDL_LIB, CallingConvention = CallingConvention.Cdecl)]
	public static extern void SDL_PumpEvents();
}

class Haptic{
	private static IntPtr controller = IntPtr.Zero;
	private static bool sdlInitialized = false;
	private const uint SDL_INIT_GAMECONTROLLER = 0x00002000;
	
	public static IntPtr GetController(int gamepadIndex = 0){
		TraceLog(TraceLogLevel.Info,"HAPTIC: Begin SDL controller init");
		if(!sdlInitialized){
			if(SDL.SDL_Init(SDL_INIT_GAMECONTROLLER) < 0){
				TraceLog(TraceLogLevel.Error,"HAPTIC: Failed to initialize SDL2 GameController subsystem");
				throw new Exception("Failed to initialize SDL2 GameController subsystem.");
			}
			sdlInitialized = true;
		}
	
		controller = SDL.SDL_GameControllerOpen(gamepadIndex);
		if(controller == IntPtr.Zero)
			TraceLog(TraceLogLevel.Error,"HAPTIC: No SDL controller " + gamepadIndex);
		else
			TraceLog(TraceLogLevel.Debug,"HAPTIC: Opened SDL controller " + gamepadIndex);
		if(SDL.SDL_GameControllerHasRumble(controller))
			TraceLog(TraceLogLevel.Debug,"HAPTIC: Has rumble");
		else
			TraceLog(TraceLogLevel.Error,"HAPTIC: Does not have rumble");
		return controller;
	}
	
	public static void Rumble(float lowIntensity, float highIntensity, float duration){
		ushort low = (ushort)(Math.Clamp(lowIntensity, 0f, 1f) * 65535);
		ushort high = (ushort)(Math.Clamp(highIntensity, 0f, 1f) * 65535);
		uint durationMs = (uint)(duration * 1000);
	
		int result = SDL.SDL_GameControllerRumble(controller, low, high, durationMs);
		if(result != 0)
			TraceLog(TraceLogLevel.Error,"HAPTIC: Controller rumble failed with error " + result);
	}
	
	public static void ShutdownSDL(){
		if(sdlInitialized){
			SDL.SDL_GameControllerClose(controller);
			SDL.SDL_Quit();
			sdlInitialized = false;
		}
	}
}