# StringsReloads v7.4
[![Build Status](https://ci.appveyor.com/api/projects/status/github/marcussacana/StringReloads?branch=master&retina=true)](https://ci.appveyor.com/project/marcussacana/StringReloads)


This is a tool created to inject strings using low level code

* Help translate a game without discovery about he packget/encryption.
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
The SRL have a feature to automatically install the SRL in the game engine without you need know how to patch the game, just rename the SRLx32.dll to d3d9.dll; dinput8.dll or any other supported wrapper [See Here what is supported](https://github.com/marcussacana/StringReloads/tree/master/SRLWrapper/Wrapper), then in the SRL.ini set the AutoInstall to true.    

#### Supported By:
- AdvHD
- SoftPal
- CMVS32 and CMVS64
- ExHIBIT

Some SoftPal games needs a manual setup with the help of the Auto-Installer, click below to see the example:
[![SRL SoftPal Auto-Install Feature](http://img.youtube.com/vi/RAgZQBWqiJQ/0.jpg)](http://www.youtube.com/watch?v=RAgZQBWqiJQ "SRL SoftPal Auto-Install Feature")

### Dependencies:
*	[DllExport](https://github.com/3F/DllExport) (Included)
---
### Useful:
* [Multiline Assembly](http://rammichael.com/multimate-assembler) (Allow put a block of code in a specified position)
	*  (Use <0x02B9E44A> for example to set the position to write your code)
* Auto-Builds: [AppVeyor](https://ci.appveyor.com/project/marcussacana/StringReloads/build/artifacts)
* Lastest Build: [Download](https://ci.appveyor.com/api/projects/marcussacana/StringReloads/artifacts/SRL/bin/SRLEngine.zip)
* Auto-Releases: [Visit](https://github.com/marcussacana/StringReloads/releases/latest)
