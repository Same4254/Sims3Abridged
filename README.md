# Sims3Abridged
A memory hack into the game Sims 3 to allow native operations such as File I/O, Sockets, etc... On top of existing libraries supported by the game

## The Problem

Sims 3 is one of my faveorite games, and as a programmer I find it interesting to mess with games with modding. Thankfully, the Sims contains a massive library of DLLs that makes this incredibly easy.

However, the problem was that the mod I wanted to create was impossible to accomplish with these libraries. This is because the .NET runtime that these mods run in lack a significant portion of the Sytem implementation. Thus, Threading, Sockets, Pipeing, File I/O, etc... Are all impossible with just these provided libraries.

## The Solution

The idea of this mod is to essentially have two programs running: The External program and the Internal Mod.

This is done so that the external program can modify the memory in the internal mod. This external program can be any language that can handle making system calls to modify memory of the Sims 3 process.

The point here is to have the external program modify and read memory of the internal mod. This regoin of memory is simply a byte array (reffered to as the buffer) in the mod that the external program can locate.

With the ability for both the external program and internal mod to read and write this regoin of memory, we can set up a protocol to formalize communication in this channel.

Now, the external program can communicate with the internal program to tell the internal mod to do certain actions, while the external program can perform Socketing and other native actions.

## How To Use

Update the refrecenes of the project! Make sure to refrence the Sims3Dlls!

As stated, this is a two part solution. There is an external program and an internal mod. This project makes use of macros to control what code goes on what side. As such, there is a custom MSBuild target for the internal mod. Use the following command in the development terminal in visual studio to compile the mod dll. Currently this dumps the DLL into the bin directory of the project named "Target.dll". This can be changed in the MSBuild target. At the bottom of the project file, there is the CSC tag which is what dictates how to compile the internal script target. This also specifies the output directory.

```
MSBuild.exe Sims3Abridged.csproj -t:InternalScript
```

For the external script, you just need to hit the run button (make sure that it is for x64 for Jupiter to work)
