-How to use to translate the {My Supported Game Here}? 
R: Read the "How To.txt".
N: Read the game before publish, i don't have sure if the Strings.srl have all dialogs included.


-Wow, this SRLx32 works with the other game?
R: Yes and Nop, To works is required a low level modification in the main game exe.
N: Read more in the end of this FAQ


-Fuck, the user can dump the translation using the command -dump???
R: Yes, and you can use my tool without say Thx to me.
N: If you really wanna prevent the string dump, modify the srlx32.dll, is a managed dll, not obfuscated.

-How to publish?
R: Well, Disable the Debug, LogInput, LogOutput, LogFile, Rebuild in the SRL.ini
Then remove the WorkingDir and move the Strings.lst to the same directory of the SRL.ini
After this you just need include in your patch the String.srl, SRLx32.dll, SRL.ini and the Game.exe
N: If you rename the SRLx32.dll to dinput8.dll or d3d9.dll the SRL will initialize very early, then
if you have a problem of the game freezing after the new game/load game, try this.

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
-"Debug" Enable the Debug Output Window
-"Delay" Log the Delay to reload very string 
-"Dump" Dump the Strings.srl including the translation
-"DumpRetail" Dump the Strings.srl but don't export the translation
-"DetectText" When the Debug mode is enable, the dumpper will append to the Strings.lst only lines that as recognized as dialogue
-"LogAll" Verbose mode, log all string reloads
-"LogInput" Log all strings that the game requested the SRL translation
-"LogOutput" Log all strings that the SRL return to the game
-"LogFile" Save the Log window output to the a file (SRL.log)
-"Unsafe" When false the SRL don't initialize while the game don't try load a string that is recognized as dialogue.
-"Rebuild" Rebuild the Strings.srl every time that the game starts
-"AntiCrash" True or False; Is the setting to games who crash on close, this can fix some cases terminating the process on you request to close it.
-"FreeOnExit" A alternative to the AntiCrash
-"TrimRangeMismatch" When true trim from the start and end all chars from input strings that isn't in the "AcceptableRange" list
-"AcceptableRanges" Acceptable range of chars, use the char: - to specify a range, like 0-9, A-z, you can put a single char too, just don't put the - before the end
-"NoTrim" Disable all trim algorithms of the SRL
-"BreakLine" Set if the game have their own breakline flag, (the default is the 0x0A byte)
-"ReloadedPrefix" Put a prefix in all reloaded lines
-"ReloadedSufix" Put a sufix in all realoaded lines
-"Multithread" Make the SRL optimized for Multithreaded games, when false the SRL create a new process to store their database, if unsure keep true
-"WindowHook" Try hook all windows user interface strings and allow translate it
-"Invalidate" If the WindowHook only translate when you put the cursor over the text, enable this
-"CachePointer" Cache the reload and speed up if the game request it again
-"NoDiagCheck" Allways think in a string as a Dialogue, Don't enable with Unsafe=false
-"LiteralMask" Reload strings Mask literally
-"DisableMask" When true disables the Mask Reloader Feature
-"LiveSettings" Reload the SRL.ini without restart the game (Some features require restart)
-"DecodeInputRemap" Restore any char reload in the input string before process
-"MultiDatabase" Create multiple databases to every .lst file, Increase memory usage but speed up the SRL, if unsure keep true
-"NoReload" Disable the SRL
-"WorkingDir" Set a custom directory as workspace of the SRL, keep empty to use the SRL dll directory
-"ReloadMaskArgs" If a mask contains a string that needs be translated, keep true
-"CustomCredits" Set a custom message during the SRL loading screen
-"LiteMode" Disable many features of the SRL to speed up
-"RemoveViolations" If you put a char in the .lst that is being used by the Char Reloader feature, when true, the SRL will cutoff all 'ilegal' chars
-"AsianInput" Hint the Dialogue Detection algorithm saying if the game is a japanese game or not.
-"AutoUnks" Automatically create the unknow char reload list based on your reloads.
-"CaseSensitive" The SRL database match can match with case senstive or not, just change this
-"NotCachedOnly" Use the pointer cache to don't allow the SRL process again the same string, usefull when the SRL injection point is inside a for each char loop of the string
-"SetOutEncoding" If the Debug Low Window (aka console) display invalid chars, set true
-"AllowEmptyReloads" Keep in the database Reloads that don't change nothing
-"AllowDuplicates" Allow a single database contains more than one reload for the same match line

About the AllowDuplicates
Well, After basically 2 years devlopling the SRL, I added support to the SRL can match
different text for a same line, This feature is disabled by default because the old method
to match the string is 15x faster than the new method, in others words enabling this feature
make the SRL match the game text more slow, and of course, will increase the cpu usage too.
Wow shit! 15x?! Yes, but you don't need worry much since the SRL is very, but very fast
to match a string, in a small database in the old method he can match like 500000 times 
the reloads using only 0.01 ms, and when using the new method he will use 0.18 ms.
Then it's really more slow but isn't something that you will notice when use...
Oh Good! But if we have more gains than lost with this new method, why is disable by default?
Well, Well... Basically the SRL already can match a duplicate line in the old method,
You just need split the database in parts and the SRL will give priority to the last database
that he found a translation, and you can need use the ::SETDB-??:: sometimes too, like when
the duplicate is just after the last, But Enabling this new feature you don't need 
split the database in many .lst's to match a duplicate, and don't need use the ::SETDB-??::,
The SRL will give priority to the closest line of the lastest matched line in the same database,
Keep in mind, when the SRL change to other database he don't reset the 'last match' position
of the related database, then if the game return to a .lst that has already matched something
can give unexpected results, then keep sure to test everything before you relase your patch.

At WordWrap
-"Enable" Disable or Enable the SRL Auto Wordwrap engine
-"Monospaced" When false the FontName, FontSize, Bold are required
-"FakeBreakLine" If the engine don't have any breakline char, the fakebreakline full the line with space to automatically break the line where needed
-"FontName" The font face name to be used in the string measure
-"FontSize" The font size to be used in the string measure (decimal)
-"Bold" If true, the string will measure using bold font
-"MaxWidth" When Monospaced is true, put the max count of characters per line, when false, the max line width in pixels

At MTL
-"MassiveMode" When true will use the array translation method of the TLIB instead the single line method.
-Self-Explanatory entries... But to enable the TLib.dll (A private library) is required in the game directory, contact-me

At Overlay (Required the Overlay.dll in the directory)
-"Enabled" Enable or Disable the Overlay
-"Padding" Set a default overlay padding

At Filter (Dialogue Detection algorithm)
-"DenyList" List of all characters that never will appear in a dialogue (split with ,)
-"QuoteList" List of all quotes that appears in the game (split with ,)
-"IgnoreList" List of things to ignore when try detect if is a dialogue or match a line, If starts with >> you will append the default list
-"TrimList" List of characters that can appears in the begin or end of a string that needs be removed before process the dialogue
-"Sensitivity" The dialogue sensitivity level, (can use negative values), bigger values are more 'permessive' than smaller values (5 is recommended)
-"UseDB" If the string is present in the database is automatically accepted as dialogue
-"ForceTrim" If you enable the "NoTrim" feature, can enable this to reenable the trim only to verify if is a dialogue
-"TagCleaner" Remove all tags from the input and output text
-"IgnoreTag" Ignore all tags when match a string in the database but don't remove it.
-"TagChars" A sequence of 2 characters, the first one to 'open' and the second to 'close' the tag to be used in the "IgnoreTag" and "TagCleaner"

At Intro
-"MinSize" Minimal Window Width and Height to show the introduction
-"Seconds" The seconds to show the introduction screen
-"CheckProportion" A alternative to the MinSize, Only show the introduction when the Width is bigger than the Height
-"CreateWindowEx" Hook the CreateWindowEx to show the Intro
-"ShowWindow" Hook the ShowWindow to show the intro (Recommended)
-"SetWindowPos" Hook the SetWindowPost to show the Intro
-"MoveWindow" Hook the MoveWindow to show the Intro

At Hook
-"AutoEngineHook" Try Detect the game engine and Auto-Install the SRL Engine in the game (Supported Engines: SoftPal; AdvHD)
-"LoadLibraryFix" Hook the LoadLibrary to redirect the wrapper dll to the retail dll (Use if the game crash when using the SRL as wrapper)
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
N: If the SRL have a delayed initialization try rename the SRLx32.dll to d3d9.dll or dinput8.dll (recommended), and modify the game exe to load the d3d9.dll and not the old SRLx32.dll

-I have 2 identic lines that but I don't want the same translation to both
R: Split the .LST in 2 files, the .lst format don't support duplicated lines, then put one of those lines in a new .lst file
N: If you put ::SETDB-Sample:: in the first of the line translation, the SRL will change to the "Strings-Sample.lst"
after match the current string, with this you can translate a duplicate that appears one after other

-About the .lst format
The .lst format is a simple plain text format pattern created by me that can be used for any game,
A .lst file is like a 'match and replace' list to the SRL, for each dialogue of the game, we have
a pair of the same line, where the first line is the "Match Line" and the secondi s the "Reload Line"
The Match Line is what the SRL will use to know when he need return the translation (the Reload Line)
Don't change the match line or can break things.

We have the "Mask Lines" too, the Mask line is any line that use {0}, {1}, {2} and etc...
You can think in the mask as a wildcard but with a small cool features, for example:

You used {0} coins, you have {1} coins left.
Shit, I lost {0} coins with this shit.... Now I have only {1} coins...

As allways, the first line is the match line, and the second the reload line.
This Reload is literally a String.Format, then you can do thing like {0:N5} or {0:D5} to modify the result string.

And of course the .lst accept some tags, all tags is prefixed and sufixed with "::" (Without quote), for example:

::SETDB-??:: (Where ?? is your database name or id (see in the console))
This tag force the SRL change to another .lst file (aka database) when the SRL reload the target line.
This tag can be used only in the reload line, don't use in the match line.

::BREAKLINE::
This tag represent the 0x0A character, in other words the break line.
This tag can be usen in the match and reload lines.

::RETURNLINE::
This tag represent the 0x0D character, in other words the carriage return.
This tag can be used in hte match and reload lines.

::NOWORDWRAP::
This tag force the SRL don't wordwrap the target line.
This tag can be used only in the reload line, and in the begin of the line.

::NOMASK::
This tag tells to the SRL the current line isn't a mask match line, don't forget to enable the LiteralMask to use the line as Reload
This tag can be used only in the reload line, and in the begin of the line.

::NOPREFIX::
This tag force the SRL don't append the "ReloadedPrefix" to the target line.
This tag can be used only in the reload line.

::NOSUFIX::
Like the ::NOPREFIX::, but don't append the "ReloadedSufix"

::NOTRIM::
Disable the Automatic Trim Restoration in the Translation Line.
This tag can be used only in the reload line, and in the begin of the line.

::FULLWORDWRAP::
This tag force the SRL do wordwrap in the sub-strings of all masks reloaded.
This tag can be used only in the reload line, and in the begin of the line.

::MAXWIDTH[??]:: (Where ?? is your width)
This tag force the SRL use the especified width to the wordwrap the target line.
This tag can be used only in the reload line, and in the begin of the line.

::EVENT??:: (Where the ?? is the event id)
This tag trigger a overlay event of the SRL with the specified ID
This tag can be used only in the reload line, and in the begin of the line.