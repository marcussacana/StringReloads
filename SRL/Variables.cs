using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace SRL
{
    partial class StringReloader
    {

        internal const string BreakLineFlag = "::BREAKLINE::";
        internal const string ReturnLineFlag = "::RETURNLINE::";
        internal const string AntiWordWrapFlag = "::NOWORDWRAP::";
        internal const string AntiMaskParser = "::NOMASK::";
        internal const string AntiPrefixFlag = "::NOPREFIX::";
        internal const string AntiSufixFlag = "::NOSUFIX::";
        internal const string MaskWordWrap = "::FULLWORDWRAP::";

        internal const string CfgName = "StringReloader";
        internal const string ServiceMask = "StringReloaderPipeID-{0}";
        internal const string ServiceDuplicateFlag = "|Duplicate";

        internal const string EncodingModifierFlag = "EncodingModifier";
        internal const string StringModifierFlag = "StringModifier";

        const int CacheLength = 200;

        static int GamePID = System.Diagnostics.Process.GetCurrentProcess().Id;

        static Dictionary<ushort, char> CharRld;
        static Dictionary<ushort, char> UnkRld;
        static IDictionary<string, string> MskRld = null;
        static Dictionary<long, string> DBNames = null;
        static Dictionary<string, FontRedirect> FontReplaces = new Dictionary<string, FontRedirect>();


        static List<IntPtr> PtrCacheIn = new List<IntPtr>();
        static List<IntPtr> PtrCacheOut = new List<IntPtr>();
        static List<string> Missed = new List<string>();
        static List<string> Replys = new List<string>();
        static List<long> Ptrs = new List<long>();
        static List<Range> Ranges = null;

        static Engine.IEngine AutoEngine = new Engine.Wrapper();

        static NamedPipeClientStream PipeClient = null;

        static bool Initialized = false;
        static bool LoadShowed = false;
        static bool ConsoleShowed = false;
        static bool DelayTest = false;
        static bool CmdLineChecked = false;
        static bool LogInput = false;
        static bool LogOutput = false;
        static bool DumpStrOnly = false;
        static bool DialogFound = false;
        static bool CloseEventAdded = false;
        static bool Unicode = false;
        static bool LogFile = false;
        static bool TrimRangeMismatch = false;
        static bool SpecialLineBreaker = false;
        static bool EnableWordWrap = false;
        static bool Multithread = false;
        static bool AntiCrash = false;
        static bool InvalidateWindow = false;
        static bool CachePointers = false;
        static bool FreeOnExit = false;
        static bool DialogCheck = true;
        static bool LiteralMaskMatch = false;
        static bool DisableMasks = false;
        static bool DecodeCharactersFromInput = false;
        static bool WindowHookRunning = false;
        static bool MultipleDatabases = false;
        static bool Managed = false;
        static bool NoReload = false;
        static bool NoTrim = false;
        static bool ReloadMaskParameters = false;
        static bool LiteMode = false;
        static bool RemoveIlegals = false;
        static bool AsianInput = false;
        static bool AutoUnks = false;
        static bool CaseSensitive = false;
        static bool NotCachedOnly = false;
        static bool AllowEmpty = false;
        static bool AllowDuplicates = false;
        static bool SetConsoleEncoding = false;

        static bool OverlayEnabled = false;
        static bool OverlayInitialized = false;
        static bool ShowNonReloads = false;
        static bool PaddingSeted = false;
        static int OPaddingTop;
        static int OPaddinLeft;
        static int OPaddinBottom;
        static int OPaddingRigth;

        static int ReplyPtr = 0;
        static int CacheArrPtr = 0;

        static int LogStack = 0;
        static int CursorX, CursorY;

        static string RldPrefix;
        static string RldSufix;

        static string[] DenyList;
        static string[] IgnoreList;
        static Quote[] QuoteList;
        static string TagChars;
        static int Sensitivity;
        static bool UseDatabase;
        static bool ForceTrim;
        static bool TagCleaner;
        static bool IgnoreTag;

#if DEBUG
        static bool HookCreateWindow;
        static bool HookSendMessage;
#endif
        static bool ImportHook;
        static bool LoadLibraryFix;
        static bool HookCreateFile;
        static bool HookMultiByteToWideChar;
        static bool HookSetWindowText;
        static bool HookGlyphOutline;
        static bool HookTextOut;
        static bool HookExtTextOut;
        static bool HookCreateFont;
        static bool HookCreateFontIndirect;
        static bool HookCoInitialize;
        static byte FontCharset;
        static string FontFaceName;
        static bool UndoChars;

#if DEBUG
        static float LastDPI;
#endif

        static bool HookCreateWindowEx;
        static bool HookShowWindow;
        static bool HookSetWindowPos;
        static bool HookMoveWindow;
        static bool CheckProportion;
        static int Seconds;
        static int MinSize;

        static bool NoDatabase;


        static string SourceLang = string.Empty;
        static string TargetLang = string.Empty;
        static bool MassiveMode;

        static string StrLstSufix = string.Empty;
        static string LastInput = string.Empty;
        static string LastOutput = string.Empty;
        static string CustomDir = string.Empty;
        static string CustomCredits = string.Empty;
        internal static string GameLineBreaker = "\n";

        static System.Drawing.Font Font;
        static bool Monospaced;
        static bool FakeBreakLine;
        static uint MaxWidth;

        static DotNetVM TLIB = null;
        static DotNetVM EncodingModifier = null;
        static DotNetVM StringModifier = null;
        static DotNetVM Overlay = null;

        static bool? ModifierRewriteMode = null;

        static bool DirectRequested = false;

        static string[] Replaces = new string[0];
        static string TLMap => BaseDir + "Strings.srl";
        static string TLMapSrc => BaseDir + "Strings.lst";
        static string TLMapSrcMsk => BaseDir + "Strings-{0}.lst";
        static string CharMapSrc => BaseDir + "Chars.lst";
        static string IntroMsk => BaseDir + "Intro{0}.{1}";
        static string TLDP => BaseDir + "TLIB.dll";
        static string OEDP => BaseDir + "Overlay.dll";
        static string MTLCache => BaseDir + "MTL.lst";
        static string ReplLst => BaseDir + "Replaces.lst";
        static string SrlDll => System.Reflection.Assembly.GetCallingAssembly().Location;
        internal static string IniPath => AppDomain.CurrentDomain.BaseDirectory + "Srl.ini";

        internal static bool LiveSettings = false;

        static string BaseDir => AppDomain.CurrentDomain.BaseDirectory + CustomDir;

        internal static Encoding ReadEncoding = Encoding.Default;
        internal static Encoding WriteEncoding = Encoding.Default;

        static BinaryReader PipeReader = null;
        static BinaryWriter PipeWriter = null;

        static int LastDBID = 0;
        static int DBID = 0;
        static List<IDictionary<string, string>> Databases = null;
        static IDictionary<string, string> StrRld {
            get {
                if (Databases == null)
                    Databases = new List<IDictionary<string, string>>() { null };

                if (DBID >= Databases.Count)
                    Databases.Add(CreateDictionary());

                if (DBID >= Databases.Count)
                    throw new Exception("GET - Invalid Database ID");

                return Databases[DBID];
            }
            set {
                if (Databases == null)
                    Databases = new List<IDictionary<string, string>>() { null };

                if (DBID >= Databases.Count)
                    Databases.Add(CreateDictionary());

                if (DBID >= Databases.Count)
                    throw new Exception("SET - Invalid Database ID");

                Databases[DBID] = value;
            }
        }

        static bool? DbgFlg = null;
        static bool _FrcDbg = false;
        public static bool Debugging {
            get {
                if (_FrcDbg)
                    return true;

                if (DbgFlg.HasValue)
                    return DbgFlg.Value;

                DbgFlg = File.Exists(BaseDir + "DEBUG");

                return DbgFlg.Value;
            }
            set {
                _FrcDbg = value;
            }
        }
        public static bool Verbose { get; private set; } = false;


        internal static uint GameBaseAddress => System.Diagnostics.Process.GetCurrentProcess().MainModule.BaseAddress.ToUInt32();
        private static IntPtr hConsole = IntPtr.Zero;
        private static bool _hdlFail = false;
        private static IntPtr _hdl = IntPtr.Zero;
        static IntPtr GameHandler {
            get {
                if (_hdl != IntPtr.Zero || _hdlFail)
                    return _hdl;

                Thread Work = new Thread(() =>
                {
                    _hdl = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                    string title = WindowTitle;
                });
                Work.Start();

                try
                {
                    DateTime Begin = DateTime.Now;
                    while ((Work.IsAlive || Work.IsBackground) && (DateTime.Now - Begin).TotalSeconds <= 2)
                        continue;
                    Work?.Abort();
                }
                catch { }

                //Optimization
                if (_hdl == IntPtr.Zero)
                    _hdlFail = true;

                return _hdl;
            }
        }

        private static float DPI {
            get {
                var g = System.Drawing.Graphics.FromHwnd(GameHandler);
                return g.DpiX;
            }
        }


        private static string _tlt = string.Empty;
        private static string WindowTitle {
            get {
                if (_tlt == string.Empty)
                {
                    _tlt = System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle;
                }
                return _tlt;
            }
        }


        private static TextWriter _LogWriter = null;
        private static TextWriter LogWriter {
            get {
                if (_LogWriter == null)
                {
                    _LogWriter = File.AppendText(BaseDir + "SRL.log");
                }
                return _LogWriter;
            }
        }

        private static int _netstas = -1;
        private static DateTime LastTry = DateTime.Now;
        private static bool Online {
            get {
                if (_netstas == 1 || IsWine)
                    return true;
                if (_netstas == -1 || (DateTime.Now - LastTry).TotalMinutes > 10)
                {
                    Ping myPing = new Ping();
                    string host = "google.com";
                    byte[] buffer = new byte[32];
                    int timeout = 1000;
                    PingOptions pingOptions = new PingOptions();
                    PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                    _netstas = (reply.Status == IPStatus.Success ? 1 : 0);
                }
                LastTry = DateTime.Now;
                return _netstas == 1;
            }
            set {
                _netstas = value ? 1 : 0;
            }
        }

        static bool? isWine;
        internal static bool IsWine {
            get {
                if (isWine.HasValue) return isWine.Value;

                IntPtr hModule = GetModuleHandleW(@"ntdll.dll");
                if (hModule == IntPtr.Zero)
                    isWine = false;
                else
                {
                    IntPtr fptr = GetProcAddress(hModule, @"wine_get_version");
                    isWine = fptr != IntPtr.Zero;
                }

                return isWine.Value;
            }
        }
        internal static string SRLVersion {
            get {
                var Version = System.Diagnostics.FileVersionInfo.GetVersionInfo(SrlDll);
                return Version.FileMajorPart + "." + Version.FileMinorPart;
            }
        }
        private static bool GameStarted()
        {
            try
            {
                return !string.IsNullOrWhiteSpace(System.Diagnostics.Process.GetCurrentProcess().MainWindowTitle);
            }
            catch
            {
                return false;
            }
        }

        static Thread SettingsWatcher = null;

        #region DEFAULTS

        static string[] MatchDel = new string[] {
            "\r", "\\r", "\n", "\\n", " ", "_r", "―", "-", "*", "♥", "①", "♪"
        };

        const string DefaultLatRange = "0-9A-Za-zÀ-ÃÇ-ÎÓ-ÕÚ-Ûà-ãç-îó-õú-û｡-ﾟ !?~.,"; 
        const string DefaultAsiRange = "０-９Ａ-ｚぁ-んァ-ヶ万-下且-丙乍-乏乕-乙九-也予-二亞-亢交-亦享-亮什-仂仍-仏仔-仙代-以伍-休伸-伺位-佑何-佗余-佞侭-侯係-俄俣-俥倡-倦倨-倭偆-偉側-偶傘-傚傲-債儀-儂儔-儖儺-儼儿-允元-兎全-兮兵-典冏-冓冕-冗冤-冦冨-冬冰-冷凄-准凋-凍凛-凝凸-出分-刈制-刻削-前剣-剥副-創劇-劉助-劭勗-務勝-勠勢-勤勸-勺勾-匂包-匈匕-北匸-医匿-十卅-半卑-協卮-卵又-収叡-句叨-右叶-司合-向含-吮吻-吾呈-告呵-呷呻-命咊-和咎-咐咼-咾哀-哂哇-哉啄-商喉-喋喘-喚喜-喟喧-喬嘖-嘘噪-噬囀-囃圦-在坎-坑坤-坦垢-垤埆-埈埒-埔堯-報塗-塚墸-墻壗-壙壮-売壹-壽変-夋夘-夜天-央奇-奉奎-契奓-奕奧-奪妁-妄妣-妥妹-妻媼-媾孔-存孚-孝季-学宇-安宋-宍宗-宝客-宦害-家寂-寇寒-寔寝-察寤-審射-尋對-小尸-屁屍-屑岶-岸岺-岼峨-峪崔-崛嶷-嶺工-巨己-巵帙-帛帶-常幃-幅幡-幤干-并幸-广底-店庵-庸廁-廃廈-廊廟-廣延-廸建-廼弉-弍式-弑弓-弘弥-弧彩-彭彿-待徊-後徐-従得-徙徨-循忖-忙応-忞怎-怐怛-思急-怫恁-恃恢-恥恨-恭悃-悅悉-悍悲-悶情-惇惞-惡惰-惴惹-惻愍-意感-愡愼-愿慊-慎慘-慚慮-慱慳-慷憘-憚懆-懍懶-懸懼-戀戈-戊戌-戎成-戔戝-戟戮-戰戲-戴房-扁扇-扉找-技抂-抄抑-折抻-抽担-拊拏-拔拗-拙招-拝括-拯拵-拷指-按挽-挿捧-捩掟-掣控-掬援-揶擠-擣攪-攬攴-改敍-敏敕-教文-斉斫-断旃-旆无-既日-早易-昕昞-映昭-是昴-昶晁-晄晝-晟晤-晩普-晰暇-暉暖-暙曲-曵曷-最服-朏朔-朗朝-期朦-木未-朮朶-朸李-村杞-杦杯-杳枠-枢架-枹柎-柑柳-柵栁-栄栩-栫桀-桄桐-桔梦-梨梯-梱棈-棋棟-棡椋-椏椙-検椡-椣楓-楕楜-楞楠-楢業-楯楳-極楷-楹楼-楾槊-槎樊-樌樒-樔模-樣権-樫樸-樺橆-橈橾-檀櫁-櫃欟-次欷-欺歡-此殉-残殱-段殺-殼毒-比氾-求汜-池沁-沃沙-沛沸-沿泙-泛泡-泣泯-泱洩-洫海-浹淅-淇淪-淬深-淳混-淼渇-渋渙-減渟-渡渣-渧渫-港游-渺湾-満滓-滕漓-漕潭-潰澀-澂激-濃濟-濡瀝-瀟瀦-瀨炫-炯炸-炻煤-照燥-燧燬-燮爺-牀牆-版犀-犂狂-狄狠-狢狷-狹猛-猝猩-猫珈-珊琅-琇琲-琶瑙-瑜瑞-瑠瑢-瑤瓰-瓲瓶-瓸甃-甅甌-甎甯-申町-甼畉-畍留-畝畤-畧畩-畫當-畸疉-疋疱-疳疼-疾痲-痴瘟-瘢癆-癈癧-癪発-百皂-的皆-皈皙-皜皷-皺監-盥眇-眉眞-眠眤-眦瞻-瞽矚-矜短-矯砥-砧碌-碎礪-礬祇-祉祕-祗祝-祠祿-禁禍-福禽-私稗-稚稻-穀積-穐穡-穣窕-窘窮-窰竃-竅竑-竓竟-竣童-竧竸-竺筋-筍筏-筒算-箚箜-箟篤-篦簑-簔簽-籀粁-粃粱-粳糞-糠約-紆紗-紜素-索紮-細終-絆絎-結絡-絣統-絳継-綜綫-網綽-綿緜-緞縉-縋縡-縣縹-縻織-繖繻-繽纈-纊纎-纐纒-纔罌-罎罧-罫罷-罹羅-羈考-耆耗-耙聲-聴聽-聿肄-肇胙-胛脅-脊腓-腕腸-腺膽-臀臘-臚臺-臼與-舊舌-舎舖-舘舩-般舵-船艘-艚艶-艸芫-芭苑-苔若-苧苹-苻茁-茆茖-茘茴-茶莞-莠華-菲萋-萎葡-董葫-葯蓉-蓋蔓-蔕蕈-蕋蕗-蕚蕨-蕫薨-薬薮-薰藹-藻蚊-蚌蚩-蚫蛬-蛯蜈-蜊蠍-蠏蠡-蠣衡-衣衽-衿裁-装裲-裵裼-裾褜-褞襞-襠覆-覈視-覘訖-記診-証詐-詒詫-詮詰-詳誣-誨請-諍諚-諜諞-諢諶-諸謀-謂謙-講譚-譜譯-譲豪-豬豸-豺貌-貎負-貢貧-貰貲-貴貶-貸費-貽貿-賄賚-賜賢-賤賺-賽贒-贔踈-踊踝-踟蹇-蹊躪-躬躯-躱車-軍軻-軾輒-輕輛-輝轄-轆轌-轎轡-轤辭-農辺-込迩-迫迷-迺逍-逑逓-逗逝-連遉-運遍-遖遧-適遭-遯遵-選避-還酉-酎釆-釉釋-金釖-釘釚-釟釣-釧鉙-鉛銚-銜鋸-鋼錘-錚錠-錣鎬-鎮鏖-鏘鐘-鐚鑑-鑓鑼-鑿閉-開閑-閔関-閥闔-闖陋-降陛-陟院-陦陵-陸隍-随隗-隙際-隝隯-隲隶-隹雄-雇雋-雎霆-霈霍-霏靂-靄靠-面鞄-鞆頁-頃頏-頓頻-頽顋-顏顯-顱飭-飯飼-飾餒-餔饐-饒首-香馬-馮駄-駆駐-駒驩-驫髭-髯鬧-鬪魁-魅魍-魏鮑-鮓鯡-鯤鯰-鯲鰈-鰊鰒-鰕鰭-鰰鰹-鰻鱆-鱈鴆-鴉鴪-鴬鵜-鵞鷸-鷺鸙-鸛麑-麓麸-麼黌-黒黛-點黻-黽齟-齣一丁七不与丐丑丞両並丨个中丱串丶丸丹主丼丿乂乃久之乢乱乳乾亀亂亅了于云互五井亘亙些亜亨亰亳亶人仄仆仇今介仞仟仡仭仮仰仲件价任仼伀企伃伉伊会伜伝伯估伴伶似伽佃但佇体佩佯佰佳併佶佻佼使侃來侈侊例侍侏侑侒侔侖侘侚供依侠価侫侵侶便俉俊俍俎俐俑俔俗俘俚俛保俟信修俯俳俵俶俸俺俾俿倅倆倉個倍倏們倒倔倖候倚倞借倶倹偀偂偃偏偐偕偖做停健偬偰偲偸偽傀傅傍傑傔催傭傷傾僂僅僉僊働像僑僕僖僘僚僞僣僥僧僭僮僴僵價僻億儉儒儘儚償儡優儲儷児兒兔党兜兢兤入共兼冀冂内円冉冊册再写冝冠冢冽冾凉凖几凡処凧凩凪凬凭凰凱凵凶函凾刀刃刄刊刋刎刑刔刕列初判別刧利刪刮到刳剃剄則剏剔剖剛剞剩剪剽剿劃劍劑劒劔力劜功加劣劦劯励労劵効劼劾勀勁勃勅勇勉勍勒動勛勦勧勲勳勵匍匏匐匙匚匝匠匡匣匤匪匯匱匳千卍南単博卜卞占卦卩卷卸卻卿厂厄厓厖厘厚原厠厥厦厨厩厭厮厰厲厳厶去参參叔取受叙叛叝叟叺吁吃各君吝吟吠否吩吶吸吹呀呂呆呎呑呟周呪呰呱味咀咄咆咒咜咢咤咥咨咩咫咬咯咲咳咸哄哘員哢哥哦哨哩哭哮哲哺哽哿唄唆唇唏唐唔唖售唯唱唳唸唹唾啀啌問啓啖啗啜啝啣啻啼啾喀喃善喆喇單喰営嗄嗅嗇嗔嗚嗜嗟嗣嗤嗷嗹嗽嗾嘆嘉嘔嘛嘩嘯嘱嘲嘴嘶嘸噂噌噎噐噛噤器噴噸噺嚀嚆嚇嚊嚏嚔嚠嚢嚥嚮嚴嚶嚼囈囎囑囓囗囘囚四回因団囮困囲図囹固国囿圀圃圄圈圉國圍圏園圓圖團圜土圭地圷圸圻址坂均坊坙坡坩坪坿垂垈垉型垓垠垪垬垰垳埀埃埋城埖埜域埠埣埴執培基埼堀堂堅堆堊堋堕堙堝堡堤堪場堵堺堽塀塁塊塋塑塒塔塞塢塩填塰塲塵塹塾境墅墓増墜增墟墨墫墮墲墳墾壁壅壇壊壌壑壓壕壜壞壟壤壥士壬壷夂夏夐夕外夢夥大失夲夷夸夾奄套奘奚奛奝奠奢奣奥奬奮女奴奸好妊妍妓妖妙妛妝妨妬妲妾姆姉始姐姑姓委姙姚姜姥姦姨姪姫姶姻姿威娃娉娑娘娚娜娟娠娥娩娯娵娶娼婀婁婆婉婚婢婦婪婬婿媒媚媛嫁嫂嫉嫋嫌嫐嫖嫗嫡嫣嫦嫩嫺嫻嬉嬋嬌嬖嬢嬪嬬嬰嬲嬶嬾孀孃孅子孑孟孩孫孰孱孳孵學孺宀它宅宏宕実宮宰宸容宿寀寉富寐寘寛寡寢寫寬寮寰寳寵寶寸寺対寿封専少尓尖尚尞尠尢尤尨尭就居屆屈届屋屓展属屠屡層履屬屮屯山屶屹岌岐岑岔岡岦岨岩岫岬岱岳岾峅峇峙峠峡峭峯峰峵島峺峻峽崇崋崎崑崟崢崧崩嵂嵋嵌嵎嵐嵒嵓嵜嵩嵬嵭嵯嵳嵶嶂嶄嶇嶋嶌嶐嶝嶢嶬嶮嶼嶽巉巌巍巐巒巓巖巛川州巡巣巫差巷巻巽巾市布帆帋希帑帖帝帥師席帯帰帳帽幀幇幌幎幔幕幗幟庁広庄庇床序庚府庠度座庫庭廏廐廓廖廚廛廝廨廩廬廰廱廳廴廾廿弁弃弄弛弟弡弩弭弯弱弴張強弸弼弾彁彅彈彊彌彎彑当彖彗彙彜彝彡形彦彧彰影彳彷役彼徇很從徠御徭微徳徴德徹徼徽心必忌忍忠忤快忰忱念忸忻忽忿怒怕怖怙怠怡怯怱怺恆恊恋恍恐恒恕恙恚恝恟恠息恰恵恷悁悒悔悖悗悚悛悟悠患悦悧悩悪悸悼悽惑惓惕惘惚惜惣惧惨惶惷愀愁愃愆愈愉愑愕愚愛愧愨愬愰愴愷慂慄慇慈慓慕慝慟慢慣慥慧慨慫慾憂憇憊憎憐憑憔憖憤憧憩憫憬憮憲憶憺憾懃懐懣懦懲懴懺或戚戛戡戦截戸戻手才扎打払托扛扞扠扣扨扮扱扶批扼抉把抛抜択抦披抬抱抵抹拂拌拍拠拡拱拳拾拿持挂挌挑挙挟挧挨挫振挺捉捌捍捏捐捕捗捜捫据捲捶捷捺捻掀掃授掉掌掎掏排掖掘掛接掲掴掵掻掾揀揃揄揆揉描提插揖揚換握揣揩揮揺搆損搏搓搖搗搜搦搨搬搭搴搶携搾摂摎摘摠摧摩摯摶摸摺撃撈撒撓撕撚撝撞撤撥撩撫播撮撰撲撹撻撼擁擂擅擇操擎擒擔擘據擦擧擬擯擱擲擴擶擺擽擾攀攅攘攜攝攣攤支攻放政故效救敝敞敢散敦敬数敲整敵敷數斂斃斌斎斐斑斗料斛斜斟斡斤斥斧斯新斷方於施旁旋旌族旒旗旙旛旬旭旱旺旻昀昂昃昆昇昉昊昌明昏昜昤春昧昨昱昻昼昿晉晋晏晒晗晙晢晳晴晶智暁暃暄暎暑暝暠暢暦暫暮暲暴暸暹暼暾暿曁曄曇曉曖曙曚曜曝曠曦曩曰會月有朋望朱朴机朽朿杁杆杉杓杖杙杜杪杭杵杷杼松板枅枇枉枋枌析枕林枚果枝枦枩枯枳枴枻柀柁柄柆柊染柔柘柚柝柞柢柤柧柩柬柮柯柱査柾柿栓栖栗栞校栢栲栴核根格栽框案桍桎桙桜桝桟档桧桴桶桷桾桿梁梃梅梍梏梓梔梗梛條梟梠梢梭梳梵梶梹梺梼棄棆棍棏棒棔棕棗棘棚棣棧森棯棲棹棺椀椁椄椅椈椒椥椦椨椪椰椴椶椹椽椿楊楙楚楨楪楫榁概榊榎榑榔榕榘榛榜榠榧榮榱榲榴榻榾榿槁槃槇槐槓様槙槝槞槢槧槨槫槭槲槹槻槽槿樂樅樗標樛樞樟樮樰樵樶樽橄橋橘橙機橡橢橦橫橲橳橸檄檍檎檐檗檜檠檢檣檪檬檮檳檸檻櫑櫓櫚櫛櫞櫟櫢櫤櫨櫪櫺櫻欄欅權欒欖欝欣欧欲欽款歃歇歉歌歎歐歓歔歙歛歟武歩歪歯歳歴歸歹死歿殀殃殄殆殍殕殖殘殞殤殪殫殯殷殿毀毅毆毋母毎毖毘毛毟毫毬毯毳氈氏民氓气気氛氣氤水氷永汎汐汕汗汚汢汨汪汯汰汲汳決汽汾沆沈沌沍沐沒沓沖没沢沫沮沱河況泄泅泉泊泌泓法泗泝泥注泪泳洄洋洌洒洗洙洛洞洟津洲洳洵洶洸活洽派流浄浅浙浚浜浣浤浦浩浪浬浮浯浴涅涇消涌涎涓涕涖涙涛涜涬涯液涵涸涼淀淋淌淏淑淒淕淘淙淞淡淤淦淨淮淵清渓渕渝温渼渾湃湊湍湎湖湘湛湜湟湧湫湮湯湲湶溂溌溏源準溘溜溝溟溢溥溪溯溲溶溷溺溽溿滂滄滅滉滋滌滑滝滞滬滯滲滴滷滸滾滿漁漂漆漉漏漑漠漢漣漫漬漱漲漸漾漿潁潅潔潘潛潜潟潤潦潴潸潺潼澄澆澈澎澑澗澡澣澤澪澱澳澵澹濆濔濕濘濛濤濫濬濮濯濱濳濵濶濺濾瀁瀅瀇瀉瀋瀏瀑瀕瀘瀚瀛瀬瀰瀲瀾灌灑灘灣火灯灰灸灼災炅炉炊炎炒炙炳烈烋烏烙烝烟烱烹烽焄焉焏焔焙焚焜無焦然焼煆煇煉煌煎煕煖煙煜煢煩煬煮煽熄熈熊熏熔熕熙熟熨熬熱熹熾燁燃燈燉燎燐燒燔燕燗營燠燵燹燻燼燾燿爆爍爐爛爨爪爬爭爰爲爵父牋牌牒牘牙牛牝牟牡牢牧物牲牴特牽牾犇犒犖犠犢犧犬犯犱犲状犹犾狆狎狐狒狗狙狛狩独狭狼狽猊猖猗猟猤猥献猯猴猶猷猾猿獄獅獎獏獗獣獨獪獰獲獵獷獸獺獻玄率玉王玖玩玲玳玻玽珀珂珍珎珒珖珞珠珣珥珪班珮珱珵珸現球琉琢琥琦琩琪琮琺琿瑁瑕瑩瑪瑯瑰瑳瑶瑾璃璉璋璞璟璢璧環璽瓊瓏瓔瓜瓠瓢瓣瓦瓧瓩瓮甁甑甓甕甘甚甜甞生産甥甦用甫甬男甸畄畆畏畑畔畠畢畭畯異畳畴畿疂疆疇疎疏疑疔疚疝疣疥疫疵疸疹痂痃病症痊痍痒痔痕痘痙痛痞痢痣痩痰痺痼痾痿瘁瘉瘋瘍瘤瘧瘰瘴瘻療癌癒癖癘癜癡癢癬癰癲癶癸皀皋皎皐皓皖皞皦皮皰皴皿盂盃盆盈益盍盒盖盗盛盜盞盟盡盧盪目盲直相盻盾省眄看県眛眩眷眸眺眼着睆睇睚睛睡督睥睦睨睫睹睾睿瞋瞎瞑瞞瞠瞥瞬瞭瞰瞳瞶瞹瞿矇矍矗矢矣知矧矩石矼砂砌砒研砕砠砡砲破砺砿硅硎硝硤硫硬硯硲硴硺硼碁碆碇碑碓碕碗碚碣碧碩碪碯碵確碼碾磁磅磆磊磋磐磑磔磚磧磨磬磯磴磽礁礇礎礑礒礙礦礰示礼社祀祁祐祓祚祢祥票祭祷祺禄禅禊禔禛禝禦禧禪禮禰禳禹禺秉秋科秒秕秘租秡秣秤秦秧秩秬称移稀稈程稍税稔稜稟稠種稱稲稷穂穃穆穉穗穩穫穰穴究穹空穽穿突窃窄窈窒窓窟窩窪窶窺窿竇竈竊立竍竏竕站竚竜竝竪竫竭端竰競竿笂笄笆笈笊笋笏笑笘笙笛笞笠笥符笨第笳笵笶笹筅筆筈等答策筝筥筧筬筮筰筱筴筵筺箆箇箋箍箏箒箔箕管箪箭箱箴箸節篁範篆篇築篋篌篏篝篠篩篭篳篶篷簀簇簍簗簟簡簣簧簪簫簷簸籃籌籍籏籐籔籖籘籟籠籤籥籬米籵籾粉粋粍粐粒粕粗粘粛粟粡粢粤粥粧粨粫粭粮粹粽精糀糂糅糊糎糒糖糘糜糢糧糯糲糴糶糸糺系糾紀紂紊紋納紐純紕紫紬紲紳紵紹紺紿絈絋経絖絛絜絞給絨絮絵絶絹絽綉綏經綟綠綢綣綴綵綷綸綺綻緇緊緋総緑緒緕緖緘線締緡緤編緩緬緯緲練緻縁縄縅縒縛縞縟縦縫縮縱縲縵縷總績繁繃繆繊繋繍繒繙繚繝繞繦繧繩繪繭繰繹繿纂纃續纖纛纜缶缸缺罅罇罐网罔罕罘罟罠置罰署罵羂羃羊羌美羔羚羝羞羡羣群羨義羮羯羲羶羸羹羽翁翅翆翊翌習翔翕翠翡翦翩翫翰翳翹翻翼耀老耋而耐耒耕耜耡耨耳耶耻耽耿聆聊聒聖聘聚聞聟聡聢聨聯聰聶職聹肉肋肌肓肖肘肚肛肝股肢肥肩肪肬肭肯肱育肴肺胃胄胆背胎胖胝胞胡胤胥胯胱胴胸胼能脂脚脛脣脩脯脱脳脹脾腆腋腎腐腑腟腥腦腫腮腰腱腴腿膀膂膃膈膊膏膓膕膚膜膝膠膣膤膨膩膰膳膵膸膺臂臆臈臉臍臑臓臟臠臣臥臧臨自臭至致臾舁舂舅舐舒舛舜舞舟舮舳艀艇艝艟艢艤艦艨艪艫艮良艱色艾芋芍芒芙芝芟芥芦芯花芳芸芹芻芽苅苗苙苛苜苞苟苡苣苫英苳苴茉茎茜茣茨茫茯茱茲茸茹荀荅草荊荏荐荒荘荢荳荵荷荻荼荿莅莇莉莊莎莓莖莚莢莨莪莫莱莵莽菁菅菇菊菌菎菓菖菘菜菟菠菩菫菴菶菷菻菽萃萄萇萓萠萢萩萪萬萱萵萸萼落葆葈葉葎著葛葦葩葱葵葷葹葺蒂蒄蒋蒐蒔蒙蒜蒟蒡蒭蒲蒴蒸蒹蒻蒼蒿蓁蓄蓆蓍蓐蓑蓖蓙蓚蓜蓬蓮蓴蓼蓿蔀蔆蔑蔗蔘蔚蔟蔡蔦蔬蔭蔵蔽蕀蕁蕃蕎蕓蕕蕣蕭蕷蕾薀薄薇薈薊薐薑薔薗薙薛薜薤薦薹薺藁藉藍藏藐藕藜藝藤藥藩藪藷藾蘂蘆蘇蘊蘋蘓蘖蘗蘚蘢蘭蘯蘰蘿虍虎虐虔處虚虜虞號虧虫虱虹虻蚓蚕蚣蚤蚯蚰蚶蛄蛆蛇蛉蛋蛍蛎蛔蛙蛛蛞蛟蛤蛩蛸蛹蛻蛾蜀蜂蜃蜆蜍蜑蜒蜘蜚蜜蜥蜩蜴蜷蜻蜿蝉蝋蝌蝎蝓蝕蝗蝙蝟蝠蝣蝦蝨蝪蝮蝴蝶蝸蝿螂融螟螢螫螯螳螺螻螽蟀蟄蟆蟇蟋蟐蟒蟠蟯蟲蟶蟷蟹蟻蟾蠅蠇蠑蠕蠖蠧蠱蠶蠹蠻血衂衄衆行衍衒術街衙衛衝衞表衫衰衲衵衷袁袂袈袋袍袒袖袗袙袞袢袤被袮袰袱袴袵袷袿裏裔裕裘裙補裝裟裡裨裸裹褂褄複褊褌褐褒褓褥褪褫褶褸褻襁襃襄襌襍襖襤襦襪襭襯襲襴襷襾西要覃覊見規覓覚覡覦覧覩親覬覯覲観覺覽覿觀角觚觜觝解触觧觴觸言訂訃計訊訌討訐訒訓訛訝訟訣訥訪設許訳訴訶訷詁詆詈詔評詛詞詠詢詣試詩詹詼誂誄誅誇誉誌認誑誓誕誘誚語誠誡説読誰課誹誼誾調諂諄談諏諒論諤諦諧諫諭諮諱諳諺諾謄謇謌謎謐謔謖謗謝謠謡謦謨謫謬謳謹謾譁證譌譎譏譓譖識譟警譫譬譴護譽譿讀讃變讌讎讐讒讓讖讙讚谷谺谿豁豆豈豊豌豎豐豕豚象豢豼貂貅貉貊貔貘貝貞資賈賊賍賎賑賓賞賠賦質賭賰賴贄贅贇贈贊贋贍贏贐贖赤赦赧赫赭走赱赳赴赶起趁超越趙趣趨足趺趾跂跋跌跏跖跚跛距跟跡跣跨跪跫路跳践跼跿踏踐踪踰踴踵蹂蹄蹌蹐蹕蹙蹟蹠蹣蹤蹲蹴蹶蹼躁躄躅躇躊躋躍躑躓躔躙躡躾軅軆軈軏軒軛軟転軣軫軸較輅載輊輌輙輟輦輩輪輯輳輸輹輻輾輿轂轉轗轜轟辛辜辞辟辣辧辨辷辿迂迄迅迎近返迚迢迥迦迭迯述迴追退送逃逅逆逋這通逧逮週進逵逶逸逹逼逾遁遂遅遇遘遙遜遞遠遡遣遥遲遺遼遽邇邉邊邏邑那邦邨邪邯邱邵邸郁郊郎郛郞郡郢郤部郭郵郷都鄂鄒鄕鄙鄧鄭鄰鄲酒酔酖酘酢酣酥酩酪酬酲酳酵酷酸醂醇醉醋醍醐醒醗醜醢醤醪醫醯醴醵醸醺釀釁釡釭釮釵釶釼釿鈆鈊鈍鈎鈐鈑鈔鈕鈞鈩鈬鈴鈷鈹鈺鈼鈿鉀鉄鉅鉈鉉鉋鉎鉐鉑鉗鉞鉢鉤鉦鉧鉱鉷鉸鉾銀銃銅銈銑銓銕銖銘銧銭銷銹鋏鋐鋒鋓鋕鋗鋙鋠鋤鋧鋩鋪鋭鋲鋳鋿錂錆錏錐錝錞錥錦錨錫錬錮錯録錵錺錻鍄鍈鍋鍍鍔鍖鍗鍛鍜鍠鍬鍮鍰鍵鍼鍾鎌鎔鎖鎗鎚鎤鎧鎰鎹鏃鏆鏈鏐鏑鏝鏞鏡鏤鏥鏨鏸鐃鐇鐐鐓鐔鐡鐫鐱鐵鐶鐸鐺鑁鑄鑅鑈鑚鑛鑞鑠鑢鑪鑰鑵鑷钁長門閂閃閇閏閖閘閙閠閧閨閭閲閹閻閼閾闃闇闊闌闍闘關闡闢闥阜阡阨阪阮阯防阻阿陀陂附陏限陪陬陰陲陳険陽隅隆隈隊隋隔隕隠隣隧隨險隴隻隼雀雁雉雑雕雖雙雛雜離難雨雪雫雰雲零雷雹電需霄霊霑霓霖霙霜霞霤霧霪霰露霳霸霹霻霽霾靆靈靉靍靏靑青靕靖静靜非靤靦靨革靫靭靱靴靹靺靼鞁鞋鞍鞏鞐鞘鞜鞠鞣鞦鞨鞫鞭鞳鞴韃韆韈韋韓韜韭韮韲音韵韶韻響項順須頌頗領頚頡頤頬頭頴頷頸顆顔顕顗願顛類顥顧顫顳顴風颪颯颱颶飃飄飆飛飜食飢飩飫飲飴餃餅餉養餌餐餘餝餞餠餡餤餧館餬餮餽餾饂饅饉饋饌饕饗馞馥馨馳馴馼駁駈駕駘駛駝駟駢駭駮駱駲駸駻駿騁騅騎騏騒験騙騨騫騰騷騾驀驂驃驅驍驎驕驗驚驛驟驢驤驥骨骭骰骸骼髀髄髏髑髓體高髙髜髞髟髢髣髦髪髫髱髴髷髻鬆鬘鬚鬟鬢鬣鬥鬮鬯鬱鬲鬻鬼魑魔魘魚魯魲魴魵鮃鮎鮏鮖鮗鮟鮠鮨鮪鮫鮭鮮鮱鮴鮹鮻鯀鯆鯉鯊鯏鯑鯒鯔鯖鯛鯨鯵鰀鰄鰆鰌鰍鰐鰛鰡鰤鰥鰲鰾鱒鱗鱚鱠鱧鱶鱸鳥鳧鳩鳫鳬鳰鳳鳴鳶鴃鴎鴒鴕鴛鴟鴣鴦鴨鴻鴾鴿鵁鵄鵆鵈鵐鵑鵙鵠鵡鵤鵫鵬鵯鵰鵲鵺鶇鶉鶏鶚鶤鶩鶫鶯鶲鶴鶸鶺鶻鷁鷂鷄鷆鷏鷓鷙鷦鷭鷯鷲鷽鸞鹵鹸鹹鹽鹿麁麈麋麌麕麗麝麟麥麦麩麪麭麾麿黄黔默黙黠黥黨黯黴黶黷黹鼇鼈鼎鼓鼕鼠鼡鼬鼻鼾齊齋齎齏齒齔齦齧齪齬齲齶齷龍龕龜龝龠朗隆、。，．：；？！＼～…‥‘’“”（）〔〕［］｛｝〈〉《》「」『』【】＊＠※";

        static string[] TrimChars = new string[] { "%K", "%LC", "♪", "%P" };
        
        #endregion

        static IntroContainer[] Introduction;
    }
}
