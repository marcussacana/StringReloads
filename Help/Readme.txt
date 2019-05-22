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
N: Try enalbed the CreateFont Hook and set the Charset to 0x00 in the SRL.ini

-About the SRL.ini
I will explain only what looks needed, run the game with "-help" to se the others value...
At StringsReloader
-"InEncoding" The Input Encoding; you can use 932/Unicode/utf8/ISO-8859-1 or any shit, google your encoding name...
-"OutEncoding" The same; but used to return a matched string...
-"Encoding" Set the InEncoding and OutEncoding the same encoding.
-"Wide" True or False; Is the setting to modify the read/write method to Encoding with 16-bits
-"AntiCrash" True or False; Is the setting to games who crash on close, this can fix some cases terminating the process on you request to close it.
-"FreeOnExit" A alternative to the AntiCrash
-"MatchIgnore" Txt1,Txt2,Txt3; Is a setting to append at the default Match ignore list, every string in the list is ignored by the string match.
-"TrimChars" (,),*; Is a setting to append strings to trim from end and begin of every line, this don't affect the return string.
-"Delay" Log the Delay to reload very string 
-"Dump" Dump the Strings.srl including the translation
-"DumpRetail" Dump the Strings.srl but don't export the translation
-"Debug" Enable the Debug Output Window
-"LogAll" Verbose mode, log all string reloads
-"LogInput" Log all strings that the game requested the SRL translation
-"LogOutput" Log all strings that the SRL return to the game
-"LogFile" Save the Log window output to the a file (SRL.log)
-"Unsafe" When false the SRL don't initialize while the game don't try load a string that is recognized game as dialogue.
-"Rebuild" Rebuild the Strings.srl every time that the game starts
-"TrimRangeMismatch" Trim from the begin and end all chars from input strings that isn't in the "AcceptableRange" list
-"AcceptableRanges" Acceptable range of chars, use the char: - to specify a range, like 0-9, A-z, you can put a single char too, just don't put the -
-"NoTrim" Disable all trim algorithms of the SRL
-"BreakLine" Set if the game have their own breakline flag, (the default is the 0x0A byte)
-"ReloadedPrefix" Put a prefix in all reloaded lines
-"ReloadedSufix" Put a sufix in all realoaded lines
-"Multithread" Make the SRL optimized for Multithreaded games, when false the SRL create a new process to store their database, keep true
-"WindowHook" Try hook all windows user interface strings and allow translate it
-"Invalidate" If the WindowHook only translate when you put the cursor over the text, enable this
-"CachePointer" Cache the reload and speed up if the game request it again
-"NoDiagCheck" Allways think in a string as a Dialogue, Don't enable with Unsafe=false
-"LiteralMask" Reload strings Mask literally
-"LiveSettings" Reload the SRL.ini without restart the game (Some features require restart)
-"DecodeInputRemap" Restore any char reload in the input string before process
-"MultiDatabase" Create multiple databases to every .lst file, Increase memory usage but speed up the SRL
-"NoReload" Disable the SRL
-"WorkingDir" Set a custom directory as workspace of the SRL, keep empty to use the SRL dll directory
-"ReloadMaskArgs" If a mask contains a string that needs be translated, keep true
-"CustomCredits" Set a custom message during the SRL loading screen
-"LiteMode" Disable many features of the SRL to speed up
-"RemoveViolations" If you put a char in the .lst that is being used by the Char Reloader feature, when true, the SRL will cutoff all 'ilegal' chars
-"AsianInput" Hint the Dialogue Detection algorithm saying if the game is a japanese game or not.
-"CaseSensitive" The SRL database match can match with case senstive or not, just change this
-"NotCachedOnly" Use the pointer cache to don't allow the SRL process again the same string
-"SetOutEncoding" If the Debug Low Window (aka console) display invalid chars, set true
-"AllowEmptyReloads" Keep in the database Reloads that don't change nothing

At WordWrap
-"Enable" Disable or Enable the SRL Auto Wordwrap engine
-"Monospaced" When false the FontName, FontSize, Bold are required
-"FakeBreakLine" If the engine don't have any breakline char, the fakebreakline full the line with space to automatically break the line where needed
-"FontName" The font face name to be used in the string measure
-"FontSize" The font size to be used in hte string measure (decimal)
-"Bold" If true, the string will measure using bold font
-"MaxWidth" When Monospaced is true, put the max count of characters per line, when false, the max line width in pixels

At MTL
-Self-Explanatory entries... But to enable the TLib.dll is required in the game directory, contact-me

At Overlay (Required the Overlay.dll in the directory)
-"Enabled" Enable or Disable the Overlay
-"Padding" Set a default overlay padding

At Filter (Dialogue Detection algorithm)
-"DenyList" List of all characters that never will appear in a dialogue (split with ,)
-"QuoteList" List of all quotes that appears in the game (split with ,)
-"IgnoreList" List of things to ignore when try detect if is a dialogue
-"TrimList" List of characters that can appears in the begin or end of a string that needs be removed before process the dialogue
-"Sensitivity" The dialogue sensitivity level, (can use negative values), bigger values are more 'permessive' than smaller values (5 is recommended)
-"UseDB" If the string is present in the database is automatically accepted as dialogue
-"ForceTrim" If you enable the "NoTrim" feature, can enable this to reenable the trim only to verify if is a dialogue

At Intro
-"MinSize" Minimal Window Width and Height to show the introduction
-"Seconds" The seconds to show the introduction screen
-"CheckProportion" A alternative to the MinSize, Only show the introduction when the Width is bigger than the Height
-"CreateWindowEx" Hook the CreateWindowEx to show the Intro
-"ShowWindow" Hook the ShowWindow to show the intro (Recommended)
-"SetWindowPos" Hook the SetWindowPost to show the Intro
-"MoveWindow" Hook the MoveWindow to show the Intro

At Hook
-"CreateFile" Hook the game file reading and allow he read files from the SRL worspace or a directory called "Patch"
-"UndoChars" Undo the char reload in the MultiByteToWideChar, TextOut and ExtTextOut hooks
-"MultibyteToWideChar" Hook the MultibyteToWideChar and reload the string
-"SetWindowText" Hook the SetWindowText and reload the string
-"GetGlyphOutline" Hook the "GetGlyphOutline" and reload the character
-"TextOut" Hook the TextOut and reload the string
-"ExtTextOut" Hook the ExtTextOut and reload the string
-"CreateFont" Hook the CreateFontA and CreateFontW and allow modify the game font
-"CreateFontIndirect" Hook the CreateFontIndirectA and the CreateFontIndirectW and allow modify the game font
-"FontCharset" Modify the Font Charset
-"FaceName" Modify all fonts facename 

At [Hook.Font.?] (You can create any amount of font remap, just put Hook.Font.0, Hook.Font.1, Hook.Font.2, Hook.Font.3... and etc)
-"FromFont" The original font facename to replace (keep * to all fonts)
-"ToFont" The modified font facename to force the game load
-"Size" Try modify the font size (accept relative values, +10, -10... and absolute values 32, 20, 24)

-How to use the Intro
R: To the intro hook catch the game initilization it's obvious, the SRL needs initialize before the game,
N: If the SRL have a delayed initialization try rename the SRLx32.dll to d3d9.dll, and modify the game exe to load the d3d9.dll and not the old SRLx32.dll

-I have 2 identic lines that but I don't want the same translation to both
R: Split the .LST in 2 files, the .lst format don't support duplicated lines, then put one of those lines in a new .lst file
N: If you put ::SETDB-Sample:: in the first of the line translation, the SRL will change to the "Strings-Sample.lst" after match the current string, with this you can translate a duplicate that appears one after other