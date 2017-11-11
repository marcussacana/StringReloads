-How to use to translate the {My Supported Game Here}? 
R: Read the "How To.txt".
N: Read the game before publish, i don't have sure if the Strings.srl have all dialogs included.


-Wow, this SRLx32 works with the other game?
R: Yes and Nop, To works is required a low level modification in the main game exe.
N: Read more in the end of this FAQ


-Fuck, the user can dump the translation using the command -dump???
R: Yes, and you can use my tool without say Thx to me.
N: If you really wanna prevent the string dump, modify the srlx32.dll, is a managed dll, not obfuscated.


-A BIG DELAY EVERY FUCKING LINE
R: Ehh... hm... Maybe your cpu is trash?
N: Launch the game with "-debug -delay" to see the string search delay, if is bigger than 300ms, feel free to contact-me

-I Know Assembly and I like use your tool to other game, how?
R: This SRLx32.dll have a export called "Process", use the Stud_PE to modify the executable imports and create a section to write your code.
This Process export can be used with 2 ways, first if you push a pointer and call, the EAX return the translation string Pointer (Null terminated)
If you push to the Process a value small than 0xFFFF he process as Char and not a string, the EAX will return the replace char too.
This char replace i use before call the GetGlyphOutlineA.
N: Remeber to POP the pushed value, the program don't change the stack.

-Okay, injected in my game, but the encoding isn't right!
R: The SRLx32.dll use the Default windows Encoding, At [StringReloader] set InEncoding and OutEncoding with your target encoding name or id,

-Allways show a "Ops, a Bug" message...
R: SRLx32.dll is corrupted, maybe...

-I Have Injected the SRLx32.dll in my game, but the Char reloads shows wrong char...
R: Maybe you need modify the CreateFont function, break the CreateFont Calls and see the right place to change the font encoding
N: puting a "mov byte ptr [eax+0x17], 0x00" before the CreateFontCall where EAX = the push pointer to the CreateFontIndirect,
N: and 0x00 the Charset, read more here: https://msdn.microsoft.com/en-us/library/cc194829.aspx

-About the SRL.ini
I will explain only what looks needed, run the game with "-help" to se the others value...
At StringsReloader
-"InEncoding" The Input Encoding; you can use 932/Unicode/utf8/ISO-8859-1 or any shit, google your encoding name...
-"OutEncoding" The same; but used to return a matched string...
-"Wide" True or False; Is the setting to modify the read/write method to Encoding with 16-bits
-"AntiCrash" True or False; Is the setting to games who crash on close, this can fix some cases terminating the process on you request to close it.
-"MatchIgnore" Txt1,Txt2,Txt3; Is a setting to append at the default Match ignore list, every string in the list is ignored by the string match.
-"TrimChars" (,),*; Is a setting to append strings to trim from end and begin of every line, this don't affect the return string.

At MTL
-Self-Explanatory entries... But to enable the TLib.dll is required in the game directory, you can get it at my github.