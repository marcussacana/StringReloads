## My Ko-fi
<a href='https://ko-fi.com/Z8Z231I4Z' target='_blank'><img height='40' style='border:0px;height:40px;' src='https://cdn.ko-fi.com/cdn/kofi1.png?v=2' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>

# StringsReloads v7.6
[![Build Status](https://ci.appveyor.com/api/projects/status/github/marcussacana/StringReloads?branch=master&retina=true)](https://ci.appveyor.com/project/marcussacana/StringReloads)


This is a tool created to inject strings using low level code

* Help translate a game without discovery about he package/encryption.
* Help use Non-ASCII Characters.
* Help with a customizable Auto-WordWrap.
* Help change the game font size (when possible)
* Help fix non-monospaced char spacing (when possible)
* Can use with just a small assembly knowledge


---
Our strategy to inject the string is create a new section using tools like the Stud_PE or CFF Explorer to have space in the executable to append the new code, a commom method is replace a original instruction and jump to your code, generally a jump have 5 bytes of length, sometimes you need replace more than one instruction and don't forget to place original instructions like CMP and TEST after the execution of your code, of course, if have one.

### Sample Call Algorithm:
```Assembly
@StrInject:			;Declare StrInject Label
	push EDX		;Backup EDX, ECX, EBX Registers
	push ECX
	push EBX
	push EAX		;The EAX is the string pointer or a unicode character
	call GetProc		;Here the Srl.Process Pointer is catched
	call EAX
	pop EBX			;Restore EDX, ECX, EBX Registers from the "Backup"
	pop ECX
	pop EDX
	jmp RetPnt		;Continue the Execution
```

We have various method to catch the Srl.Process Pointer, this one is to games who have a dynamic main module alocation, is possible use the sample bellow if you have injected the export in the game executable, you can use Stud_PE or CFF Explorer to do this, You need too create a new executable section to append our new code. 

In the case of this Sample the Srl.Process address in the import table is allways relative to the our new executable section.

In the code bellow the catched EIP is to the "pop EAX" and the Srl.Process in the import table is for example 0x02B9C400 and the "pop EAX" is at 0x02B9E44C, so...
0x02B9E44C - 0x02B9C400 = 0x204C, now if we subtract this value of the EIP we can get the position of the Srl.Process pointer.

If you wanna use the SRL to put Non-ASCII Characters you need call the Srl.Process and give the char of a GetGlyph function to he.

### Sample Catch Srl.Process:
```Assembly
@GetProc:
	call @Nxt	    	;Call the label Nxt
@Nxt:				    ;Declare the Nxt label
	pop EAX 		    ;Catch the EIP :)
	sub EAX, 0x204C		;Subtract the Difference from the EIP and Import Address
	mov EAX, [EAX]		;Read the import table
	ret
```
### Fastest Reload Method:
After some problems with games that reload string inside a loop I created the GetDirectProcess, to import this function use the same method of the SRL.Process above, But you will change the GetProc, here a example code:

```Assembly
@GetProc:
	call @Nxt	    	   ;Call the label Nxt
@Nxt:			    	   ;Declare the Nxt label
	pop EAX 		       ;Catch the EIP :)
	cmp dword [EAX+0x21], 0;Verify if already have the Address
	jne @Finish
	push EAX              ;Backup NXT Address
	sub EAX, 0x2050		  ;Subtract the Difference from the EIP and Import Address
	mov EAX, [EAX]		  ;Read the import table
	call EAX              ;Calls the GetDirectProcess
	pop EBX               ;Recovery NXT Address
	mov [EBX+0x21], EAX   ;Save the Process Address
	mov EAX, EBX          ;Prepare to Finish
@Finish:
	add EAX, 0x21         ;Ajust Pointer
	mov EAX, [EAX]		  ;Read the Process Address
	ret

@Ptr:
	dd 0                ;Here is the @Nxt: + 0x21

```
This method don't have paramters, just call and catch the EAX, the EAX is a pointer to the Process function

---

### Auto-Install Feature:
The SRL have a feature to automatically install the SRL in the game engine without you need know how to patch the game.  
First, dowload the SRL [HERE](https://github.com/marcussacana/StringReloads/releases/latest), extract to the game directory, then rename the SRLWrapper.dll to d3d9.dll; dinput8.dll or any other supported wrapper [See Here what is supported](https://github.com/marcussacana/StringReloads/tree/master/SRLWrapper/Wrapper), then in the SRL.ini set the AutoInstall to true.    

#### Supported By:
- AdvHD
- SoftPal
- CMVS32 and CMVS64
- ExHIBIT
- EntisGLS

Some SoftPal games needs a manual setup with the help of the Auto-Installer, click below to see the example:
[![SRL SoftPal Auto-Install Feature](http://img.youtube.com/vi/RAgZQBWqiJQ/0.jpg)](http://www.youtube.com/watch?v=RAgZQBWqiJQ "SRL SoftPal Auto-Install Feature")  

The EntisGLS AutoInstall only allow you modify the internal engine XML config, and with that, you will be able to set custom fonts and load extracted files.  
Just add the `<file path="$(CURRENT)\Patch"/>` in the generated `EntisGLSConfig.xml` file and the SRL will automatically force the game use your custom settings.  
Note: The priority are in the upper lines, add the line in the top of others `<file/>` or `<archive/>` blocks.

---

### Font Modifier
A common feature that the people want from the String Reloads is the Font Modifier feature, this feature works with major part of games that don't have a pre-rendered font and is easy to setup it.  
First, dowload the SRL [HERE](https://github.com/marcussacana/StringReloads/releases/latest), extract to the game directory, then rename the SRLWrapper.dll to d3d9.dll; dinput8.dll or any other supported wrapper [See Here what is supported](https://github.com/marcussacana/StringReloads/tree/master/SRLWrapper/Wrapper).  
Now, you need to enable the CreateFont hook in the SRL.ini, we have 4 variants of the CreateFont, the CreateFontA, CreateFontW, CreateFontIndirectA and CreateFontIndirectW.  
Usually a game use only one of those 4 options, the correct way to know what hook you must enable is find in the game executable if he have the string "CreateFontA", "CreateFontW", "CreateFontIndirectA" or "CreateFontIndirectW"... Or, if you're lazy or you're a bit crazy, you can just enable everything, but isn't recommended.  
When you enable the CreateFont hook, the SRL will be able to modify some few things in the game font, like the font size (Not works with all engines) and the font itself.  
In the end of the SRL.ini you can found a default 'sample' font remap entry, the `[Font.0]`, you can add more font reload parameters adding a new block with a incremented number in the block name, like `[Font.1]`, `[Font.2]`, `[Font.3]`...  
- In the `From` parameter you can set the 'origin' font face name to be modified, where * means all fonts.
- In the `FromWidth` or `FromHeight` you can match the font to be modified by specifying his original size. (By default commented)
- In the `FromCharset` you can match the font to be modified by specifying the font charset. (By default commented)
- In the `To` parameter you can set your 'target' font face name.
- In the `Charset` parameter you can force the font charset to be used, where 0 = auto, commented = don't modify, 932 = SJIS, see more values [here](https://docs.microsoft.com/en-us/windows/win32/intl/code-page-identifiers)
- In the `Width`, `Height` parameters, you can modify the font size, the numbers can be a absolute size or relative size, `+5` or `-1` are relative values that are calculed with  the original font size value. the `.` prefix in the value specify that value is a absolute value, then `.-1` will really became -1 regradless of the original font size.  

- The `Width`, `Height` parameters can be even more dynamic if you want, it's possible to set the value as a expression to be evaulated during the runtime, for example, if you set the `Width` to `if([Width] < 0, [Width] * -1, [Width])`, this expression will check if the Width is negative, and make him positive, otherwise, does nothing.  
Expressions can have the parameters: `[Width]`, `[Height]`, `[Charset]`, `[Facename]`, you can check details of the expression usage [here](https://github.com/ncalc/ncalc/wiki)

Also, in the `[StringReloads]` block, you can find the `LoadLocalFonts` parameter, with this parameter set to true, the SRL will make the game be able to load fonts from the game directory (or subdirectory) without need install the font in the host system.

### Dependencies:
*	[DllExport](https://github.com/3F/DllExport) (Included)
---
### Useful:
* [Multiline Assembly](http://rammichael.com/multimate-assembler) (Allow put a block of code in a specified position)
	*  (Use <0x02B9E44A> for example to set the position to write your code)
* Auto-Builds: [AppVeyor](https://ci.appveyor.com/project/marcussacana/StringReloads/build/artifacts)
* Lastest Build: [Download](https://ci.appveyor.com/api/projects/marcussacana/StringReloads/artifacts/SRL/bin/SRLEngine.zip)
* Auto-Releases: [Visit](https://github.com/marcussacana/StringReloads/releases/latest)
