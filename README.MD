# StringsReloads v1.0
[![Build Status](https://ci.appveyor.com/api/projects/status/github/marcussacana/StringReloads?branch=master&retina=true)](https://ci.appveyor.com/project/marcussacana/StringReloads)


This is a tool created to inject strings using low level code

* Accept strings and characters as input
* Can use with just a small assembly knowledge


---
Our strategy to inject the string is create a new section using tools like the Stud_PE or CFF Explorer to have space in the executable to append the new code, a commom method is replace a original command and jump to your code, generally a jump have 5 bytes of length, sometimes you need replace more than one command and don't forgive to place original commands like CMP and TEST after the execution of your code, of course, if have one.

### Sample Call Algorithm
```Assembly
@StrInject:			;Declare StrInject Label
	push EDX		;Backup EDX, ECX, EBX Registers
	push ECX
	push EBX
	push EAX		;The EAX is the string pointer or a unicode character
	call GetProc	;Here the Srl.Process Pointer is catched
	call EAX
	add ESP, 0x4	;Discard the last "Push EAX" (Is Cdecl)
	pop EBX			;Restore EDX, ECX, EBX Registers from the "Backup"
	pop ECX
	pop EDX
	jmp RetPnt		;Continue the Execution
```

---

We have various method to catch the Srl.Process Pointer, this one is to games who have a dynamic main module alocation, is possible use the sample bellow if you have injected the export in the game executable, you can use Stud_PE or CFF Explorer to do this, You need too create a new executable section in to append our new code. 

In the case of this Sample the Srl.Process address in the import table is allways relative to the our new executable section.

In the code bellow the catched EIP is to the "pop EAX" and the Srl.Process in the import table is for example 0x02B9C400 and the "pop EAX" is at 0x02B9E44C, so...
0x02B9E44C - 0x02B9C400 = 0x204C, now if we subtract this value of the EIP we can get the position of the Srl.Process pointer.

### Sample Catch Srl.Process
```Assembly
@GetProc:
	call Nxt		;Call the label Nxt
@Nxt:				;Declare the Nxt label
	pop EAX 		;Catch the EIP :)
	sub EAX, 0x204C	;Subtract the Difference from the EIP and Import Address
	mov EAX, [EAX]	;Read the import table
	ret
```

---

### Build Dependencies:
*	[vsSoluctionBuildEvents](https://github.com/3F/vsSolutionBuildEvent)
*	[DllExport](https://github.com/3F/DllExport) (Included)
---
### Useful
* [Multiline Assembly](http://rammichael.com/multimate-assembler) (Allow put a block of code in a specified position)
	*  (Use <0x02B9E44A> for example to set the position to write your code)
* Auto-Builds: [AppVeyor](https://ci.appveyor.com/project/marcussacana/StringReloads/build/artifacts)