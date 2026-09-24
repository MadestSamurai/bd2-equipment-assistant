using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using static BD2Equipment.Core.J;
namespace BD2Equipment.Core;
public sealed class StepRejected(string message):InvalidOperationException(message);
public interface IEquipmentGame:IDisposable
{
 JsonObject Stock();
 int PowderPreview(JsonNode recipe,int count,int level);
 void RefinePreview(string instance);
 JsonObject Transaction(JsonObject journal,string role,JsonObject action,Func<JsonObject,JsonArray,JsonObject,JsonObject> verify);
 IDisposable EnableActions();
 void Cleanup(string destination);
}
public sealed class LiveEquipment:IEquipmentGame
{
 public static string Root=>Path.Combine(Data,"live");
 static string P(string name)=>Path.Combine(Root,name);
 FileStream? controller;JsonObject? original,request;bool configWritten;
 static readonly JsonObject Policy=Resource("ui-policy.json");
 const string Level="ὬὠὬὡὫὤὮὨὦὤὥ",Auto="ὬὭὪὣὣὨὭὭὩὯὨ",Count="ὧὤὯὭὨὨὦὥὡὦὫ",Price="ὩὥὦὦὭὨὢὩὧὫὦ",Recipe="ὤὭὠὥὤὠὡὣὣὠὣ.ὫὮὩὬὤὫὮὨὢὯὬ.Id",RefineGear="ὢὠὡὡὫὤὮὫὢὢὪ.InvenIndex",RefineMode="ὯὢὪὬὠὧὥὠὤὩὦ",RefineReady="ὠὠὡὭὧὫὫὭὥὣὯ";
 public static void Prepare(Func<string[],string> invoke){
  Directory.CreateDirectory(Data);var location=JsonNode.Parse(invoke(["locate"]))!;string managed=S(location["managed"]);var source=EquipmentPlanner.Catalog["source"]!;
  bool Match(string path,string expected){if(!File.Exists(path))return false;using var file=File.OpenRead(path);return Convert.ToHexString(SHA256.HashData(file)).Equals(expected,StringComparison.OrdinalIgnoreCase);}
  if(!Match(Path.Combine(managed,"Assembly-CSharp.dll"),S(source["assemblySha256"])))throw new InvalidOperationException("游戏版本已变化，请更新工具后重新计算");
  string data=Environment.GetEnvironmentVariable("BD2_DATA_ROOT")??Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"..","LocalLow","Gamfs","BrownDust II","Data"));string db=S(source["databaseFile"]);
  if(!new[]{Path.Combine(data,"t",db),Path.Combine(data,db)}.Any(p=>Match(p,S(source["databaseSha256"]))))throw new InvalidOperationException("游戏数据表已更新，请更新工具后重新计算，避免使用旧费用");
  JsonObject? frame=null;try{frame=Snapshot();if(!Equal(frame["ProcessId"],location["processId"])||!Equal(frame["ProcessStartTicks"],location["startTicks"]))frame=null;}catch(IOException){}catch(InvalidOperationException){}
  string prepared=Path.Combine(Data,"prepared");Directory.CreateDirectory(prepared);
  if(frame==null||N(frame["BridgeVersion"])!=61){invoke(["prepare",managed,prepared]);invoke([frame==null?"attach":"upgrade",Path.Combine(prepared,"bridge.dll")]);}
  string spec=Path.Combine(prepared,"evidence-spec.json");Write(spec,Resource("evidence-spec.json"));invoke(["taps",managed,spec,Path.Combine(Data,"evidence-config.json")]);Snapshot();
 }
 public LiveEquipment(bool details=false){
  Directory.CreateDirectory(Root);
  try{
   controller=new FileStream(P("controller.lock"),FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite);if(controller.Length==0){controller.WriteByte(48);controller.Flush(true);}controller.Lock(0,1);
   original=File.Exists(P("evidence-config.json"))?Read(P("evidence-config.json")):null;var config=Read(Path.Combine(Data,"evidence-config.json"));if(!details)config["Reads"]=Array(A(config["Reads"]).Where(r=>S(r["Id"])!="equipment.details"));Write(P("evidence-config.json"),config);configWritten=true;Evidence(Snapshot());
  }catch{Dispose();throw;}
 }
 public void Dispose(){try{Release();if(configWritten){if(original!=null)Write(P("evidence-config.json"),original);else File.Delete(P("evidence-config.json"));}}finally{controller?.Dispose();controller=null;}}
 void Release(){if(request==null)return;try{if(File.Exists(P("observation-request.json"))&&S(Read(P("observation-request.json"))["Id"])==S(request["Id"]))File.Delete(P("observation-request.json"));}catch(IOException){}finally{request=null;}}
 string Ensure(){if(request==null||N(request["ExpiresUtcTicks"])-DateTime.UtcNow.Ticks<TimeSpan.FromSeconds(5).Ticks){request=new(){["Id"]=Guid.NewGuid().ToString("N"),["Prefixes"]=new JsonArray("equipment","policy.equipment","powder","reward.presentation","trade.characters","trade.currency","trade.inventory"),["ExpiresUtcTicks"]=DateTime.UtcNow.AddSeconds(15).Ticks};Write(P("observation-request.json"),request);}return S(request["Id"]);}
 public static JsonObject Snapshot(){var f=Read(P("snapshot.json"));long age=DateTime.UtcNow.Ticks-N(f["AtUtcTicks"]);if(S(f["Error"])!=""||age<0||age>TimeSpan.FromSeconds(3).Ticks)throw new InvalidOperationException("游戏观察连接未就绪："+S(f["Error"]));return f;}
 JsonObject Evidence(JsonObject? before=null){string id=Ensure();var timer=Stopwatch.StartNew();while(true){var e=Read(P("evidence.json"));if(S(e["ObservationRequest"])!=id||S(e["Error"]).StartsWith("System.IO.IOException:")){if(timer.Elapsed.TotalSeconds>=3)throw new InvalidOperationException("库存观察尚未就绪");Thread.Sleep(100);continue;}long age=DateTime.UtcNow.Ticks-N(e["AtUtcTicks"]);if(S(e["Error"])!=""||age<0||age>TimeSpan.FromSeconds(3).Ticks)throw new InvalidOperationException("库存观察未就绪："+S(e["Error"]));if(before!=null&&LiveEvidence.Actor(before)!=LiveEvidence.Actor(e["Frame"]!))throw new InvalidOperationException("观察账号或游戏进程改变");return e;}}
 public JsonObject Capture(){var e=Evidence(Snapshot());Write(Path.Combine(Data,"capture-evidence.json"),e);var stock=EquipmentPlanner.Normalize(e);var details=new JsonObject{["equipment"]=LiveEvidence.Reading(e,"equipment.details","$items"),["characters"]=LiveEvidence.Reading(e,"trade.characters","$items")};if(A(details["equipment"]).Count()!=A(stock["equipment"]).Count())throw new InvalidOperationException("装备详情读取不完整");Write(Path.Combine(Data,"inventory.json"),stock);Write(Path.Combine(Data,"display-inventory.json"),details);return new(){["stock"]=stock,["gear"]=GearView.Build(stock,details),["resume"]=EquipmentExecution.Resumable(stock)};}
 public JsonObject Stock()=>EquipmentPlanner.Normalize(Evidence(Snapshot()));
 public static void Stopped(){if(File.Exists(Stop))throw new StepRejected("已停止；当前已提交的批次仍会完成核账");}
 static void PauseGuard(){Stopped();if(File.Exists(P("pause")))throw new StepRejected("已暂停，未提交下一步操作");}
 public static string[] Blockers(JsonObject f,string type){var background=A(Policy["background_surfaces"]).Select(v=>S(v)).ToHashSet();var matches=A(f["Surfaces"]).Where(u=>S(u["Type"])==type).ToArray();JsonNode? target=matches.Length==1?matches[0]:null;
  return A(f["Surfaces"]).Where(u=>B(u["Popup"])&&S(u["Type"])!=type&&!background.Contains(S(u["Type"]))&&!(target!=null&&S(target["Path"]).StartsWith(S(u["Path"],"\0")+"/"))&&!(target!=null&&B(target["Popup"])&&target["Order"]!=null&&u["Order"]!=null&&N(u["Order"])<N(target["Order"]))).Select(u=>S(u["Type"])).ToArray();
 }
 static HashSet<string> Types(JsonObject f)=>A(f["Surfaces"]).Select(u=>S(u["Type"])).ToHashSet();
 public void Step(string ui,string? field=null,bool back=false,string? expect=null,string? absent=null,string? operation=null,JsonArray? items=null,long value=0,string reason="装备助手：用户已确认的操作"){
  string Context(JsonObject f)=>Canonical(Select(f,"ProcessId","ProcessStartTicks","Instance","AccountKey","PlayerKey","Scene"));
  var initial=Snapshot();string bound=Context(initial);var timer=Stopwatch.StartNew();
  for(int attempt=0;attempt<6;attempt++){
   while(true){PauseGuard();var f=Snapshot();if(Context(f)!=bound)throw new StepRejected("账号或场景改变，未发送操作");var found=A(f["Surfaces"]).Where(u=>S(u["Type"])==ui).ToArray();if(found.Length!=1||found[0]["InputReady"]==null||B(found[0]["InputReady"]))break;if(timer.Elapsed.TotalSeconds>=20)throw new StepRejected("界面尚未就绪，未发送操作");Thread.Sleep(200);}
   try{StepOnce(ui,field,back,expect,absent,operation,items,value,reason);return;}
   catch(StepRejected ex)when(attempt<5&&ex.Message is "rejected: screen_changed" or "rejected: ui_not_ready"){Thread.Sleep(600);}
   catch(StepRejected ex)when(attempt<5&&ui!="NewsPopupEventUI"&&ex.Message=="Foreground popup needs handling: NewsPopupEventUI"){
    var f=Snapshot();if(Context(f)!=bound||!Types(f).Contains("MenuUI")||!Blockers(f,ui).SequenceEqual(new[]{"NewsPopupEventUI"}))throw;Step("NewsPopupEventUI",back:true,absent:"NewsPopupEventUI");
   }
  }
 }
 void StepOnce(string ui,string? field,bool back,string? expect,string? absent,string? operation,JsonArray? items,long value,string reason){
  PauseGuard();var before=Snapshot();var candidates=A(before["Surfaces"]).Where(u=>S(u["Type"])==ui).ToArray();if(candidates.Length!=1)throw new StepRejected("找不到唯一界面："+ui);var surface=candidates[0];var blockers=Blockers(before,ui);if(blockers.Length>0)throw new StepRejected("Foreground popup needs handling: "+string.Join(", ",blockers));
  long target=0;string kind=operation??(back?"back":"click");if(!back&&operation==null){var targets=A(surface["Targets"]).Where(t=>S(t["Field"])==field&&B(t["Enabled"])).ToArray();if(targets.Length!=1)throw new StepRejected("找不到唯一可用按钮："+ui+"."+field);target=N(targets[0]["Id"]);if(S(targets[0]["Route"])=="pointer")kind="pointer";}
  var command=Select(before,"ProcessId","ProcessStartTicks","Instance","AccountKey","PlayerKey","Scene","UiToken");string id=Guid.NewGuid().ToString("N");command["Scope"]="gameplay";command["Id"]=id;command["Kind"]=kind;command["SurfaceId"]=Clone(surface["Id"]);command["TargetId"]=target;command["ObservedUtcTicks"]=Clone(before["AtUtcTicks"]);command["ExpiresUtcTicks"]=DateTime.UtcNow.AddSeconds(10).Ticks;command["Reason"]=reason;command["Items"]=items?.DeepClone()??new JsonArray();command["Value"]=value;
  if(File.Exists(P("command.json")))throw new StepRejected("另一操作正在等待执行");string dir=P("steps/"+id);Write(Path.Combine(dir,"before.json"),before);Write(Path.Combine(dir,"intent.json"),command);Write(P("command.json"),command);var timer=Stopwatch.StartNew();JsonObject? receipt=null;
  while(timer.Elapsed.TotalSeconds<20){string path=P("receipts/"+id+".json");if(File.Exists(path)){receipt=Read(path);string state=S(receipt["State"]);if(state is "rejected" or "unknown"){Write(Path.Combine(dir,"result.json"),receipt);if(state=="rejected"&&!B(receipt["MayHaveDispatched"]))throw new StepRejected(state+": "+S(receipt["Error"]));throw new InvalidOperationException(state+": "+S(receipt["Error"]));}if(state=="observed_after_dispatch"){JsonObject? after=null;try{after=Snapshot();}catch(InvalidOperationException){}if(after!=null){var types=Types(after);if((expect==null||types.Contains(expect))&&(absent==null||!types.Contains(absent))){Write(Path.Combine(dir,"result.json"),new JsonObject{["state"]="observed_expected_ui",["receipt"]=receipt,["after"]=after});return;}}}}Thread.Sleep(200);}
  Write(Path.Combine(dir,"result.json"),new JsonObject{["state"]="unknown_timeout",["receipt"]=receipt});throw new InvalidOperationException("操作结果未确认；已保存证据，不会自动重发");
 }
 JsonObject WaitRead(string name,string path,Func<JsonNode?,bool> predicate){string actor=LiveEvidence.Actor(Snapshot());var timer=Stopwatch.StartNew();while(timer.Elapsed.TotalSeconds<25){PauseGuard();var frame=Snapshot();if(LiveEvidence.Actor(frame)!=actor)throw new InvalidOperationException("账号已切换");try{var e=Evidence(frame);if(predicate(LiveEvidence.Reading(e,name,path)))return e;}catch(InvalidOperationException){}Thread.Sleep(250);}throw new InvalidOperationException("原生界面状态未就绪："+name+"."+path);}
 public void Cleanup(string destination="MenuUI"){
  string actor=LiveEvidence.Actor(Snapshot());var timer=Stopwatch.StartNew();var allowed=new[]{"EquipmentBatchUpgradeResultPopupUI","EquipmentUpgradeResultPopupUI","EquipmentUpgradeUI","EquipmentMakingUI","EquipmentMakingSelectUI","InventoryManageUI","MailUI"}.ToHashSet();
  while(timer.Elapsed.TotalSeconds<45){PauseGuard();var f=Snapshot();if(LiveEvidence.Actor(f)!=actor)throw new InvalidOperationException("收尾期间账号改变");var types=Types(f);
   var popup=new[]{"EquipmentBatchUpgradeResultPopupUI","EquipmentUpgradeResultPopupUI","EquipmentUpgradePopupUI","NewsPopupEventUI"}.FirstOrDefault(t=>types.Contains(t)&&Blockers(f,t).Length==0);
   if(popup!=null){Step(popup,back:true,absent:popup);continue;}
   if(A(f["Surfaces"]).Any(u=>S(u["Type"])==destination&&B(u["InputReady"]))&&Blockers(f,destination).Length==0)return;
   if(destination=="MenuUI"&&types.Contains("GameFieldDefaultUI")&&!types.Contains("MenuUI")){Step("GameFieldDefaultUI","_buttonMenu",expect:"MenuUI");continue;}
   if(destination=="MenuUI"&&A(f["Surfaces"]).Any(u=>S(u["Type"])=="ItemGetPopupUI"&&B(u["InputReady"]))&&Blockers(f,"ItemGetPopupUI").Length==0){Step("ItemGetPopupUI","_objBackButton",absent:"ItemGetPopupUI");continue;}
   var next=A(f["Surfaces"]).Where(u=>allowed.Contains(S(u["Type"]))&&B(u["InputReady"])&&Blockers(f,S(u["Type"])).Length==0).OrderByDescending(u=>N(u["Order"])).FirstOrDefault();if(next!=null)Step(S(next["Type"]),back:true,absent:S(next["Type"]));Thread.Sleep(250);
  }throw new InvalidOperationException("界面尚未稳定，已保留确认结果，不会重做消费");
 }
 public int PowderPreview(JsonNode recipe,int count,int level){var types=Types(Snapshot());long recipeId=N(recipe["id"]);
  if(types.Contains("EquipmentMakingUI")){Cleanup("EquipmentMakingUI");var e=Evidence(Snapshot());if(N(LiveEvidence.Reading(e,"powder.craft",Recipe))!=recipeId){Cleanup("EquipmentMakingSelectUI");Step("EquipmentMakingSelectUI",operation:"powder_recipe",value:recipeId,expect:"EquipmentMakingUI");}}
  else{if(types.Contains("EquipmentMakingSelectUI"))Cleanup("EquipmentMakingSelectUI");else{Cleanup();Step("MenuUI",operation:"equipment_craft_menu",expect:"EquipmentMakingSelectUI");}Step("EquipmentMakingSelectUI",operation:"powder_recipe",value:recipeId,expect:"EquipmentMakingUI");}
  var craft=WaitRead("powder.craft",Recipe,n=>N(n)==recipeId);long quantity=N(LiveEvidence.Reading(craft,"powder.craft","_craftMaterial.craftCount"));if(quantity<1||N(LiveEvidence.Reading(craft,"powder.craft","_craftButton.UseResourceCount"))!=N(recipe["potions"])*quantity)throw new InvalidOperationException("原生制作药耗与计划不一致");
  Step("EquipmentMakingUI","_oneTimeCraftButton",expect:"EquipmentUpgradePopupUI");Step("EquipmentUpgradePopupUI",operation:"powder_options",value:level,items:new JsonArray(count));
  var options=WaitRead("powder.options",Count,n=>N(n)>=1&&N(n)<=count);int actual=(int)N(LiveEvidence.Reading(options,"powder.options",Count));int capacity=(int)double.Parse(S(LiveEvidence.Reading(options,"powder.options","_goldSlider_auto.maxValue")),System.Globalization.CultureInfo.InvariantCulture);
  if(actual!=Math.Min(count,capacity)||N(LiveEvidence.Reading(options,"powder.options",Level))!=level||!B(LiveEvidence.Reading(options,"powder.options",Auto))||N(LiveEvidence.Reading(options,"powder.options",Price))!=N(recipe["gold"]![level.ToString()])*actual)throw new InvalidOperationException("游戏预览的数量、强化等级或报价与计划不符");return actual;
 }
 public void RefinePreview(string instance){var f=Snapshot();var types=Types(f);bool same=types.Contains("EquipmentUpgradeUI")&&S(LiveEvidence.Reading(Evidence(f),"equipment.refine_ui",RefineGear))==instance;
  if(!same){if(!types.Contains("InventoryManageUI")&&!types.Contains("EquipmentUpgradeUI")){Cleanup();Step("MenuUI","_buttonInventory",expect:"InventoryManageUI");}Cleanup("InventoryManageUI");Step("InventoryManageUI",operation:"equipment_refine_preview",items:new JsonArray(long.Parse(instance)),expect:"EquipmentUpgradeUI");}
  Cleanup("EquipmentUpgradeUI");var e=WaitRead("equipment.refine_ui",LiveEvidence.RefineAuto,n=>n?.GetValue<bool>()==false);if(S(LiveEvidence.Reading(e,"equipment.refine_ui",RefineGear))!=instance||S(LiveEvidence.Reading(e,"equipment.refine_ui",RefineMode))!="SMELT"||!B(LiveEvidence.Reading(e,"equipment.refine_ui",RefineReady)))throw new InvalidOperationException("批量精炼界面尚未就绪或选中装备已变化");
 }
 JsonArray Events(string role,long since){var result=new List<JsonNode>();string dir=P("events");if(Directory.Exists(dir))foreach(var path in Directory.EnumerateFiles(dir,"*.json")){if(long.TryParse(Path.GetFileName(path).Split('-')[0],out long stamp)&&stamp>=since){var e=Read(path);if(S(e["Role"])==role)result.Add(e);}}return Array(result.OrderBy(e=>N(e["Sequence"])));}
 public JsonObject Transaction(JsonObject journal,string role,JsonObject action,Func<JsonObject,JsonArray,JsonObject,JsonObject> verify){
  Stopped();var before=Evidence(Snapshot());long stamp=DateTime.UtcNow.Ticks;var entry=new JsonObject{["id"]=Guid.NewGuid().ToString("N"),["role"]=role,["state"]="dispatching",["at"]=stamp,["before"]=before,["action"]=action};journal["pending"]=entry;string path=Path.Combine(Data,S(journal["id"])+".json");Write(path,journal);
  try{
   try{Step(S(action["ui"]),S(action["field"],null!),operation:S(action["operation"],null!),items:action["items"]?.AsArray(),value:N(action["value"]),reason:S(action["reason"]));}catch(StepRejected){entry["state"]="rejected";throw;}catch(InvalidOperationException ex){entry["ui_error"]=ex.Message;}
   var timer=Stopwatch.StartNew();while(timer.Elapsed.TotalSeconds<70){var events=Events(role,stamp);var after=Evidence(before["Frame"]!.AsObject());JsonObject? result=null;
    try{entry["events"]=events;var packets=LiveEvidence.Responses(role,events,before,after,role=="equipment.refine_batch"?(int)N(action["items"]![1]):1);result=verify(EquipmentPlanner.Normalize(before),packets,EquipmentPlanner.Normalize(after));}catch(InvalidOperationException ex){entry["waiting"]=ex.Message;}
    if(result!=null){entry["state"]="completed";entry["after"]=after;entry["result"]=result;Write(Path.Combine(Data,"receipts",S(journal["id"]),S(entry["id"])+".json"),entry);journal["operations"]!.AsArray().Add(Select(entry,"id","role","state","at","result"));journal["expected"]=EquipmentPlanner.Normalize(after);journal["pending"]=null;Write(path,journal);return result;}
    Thread.Sleep(200);
   }throw new InvalidOperationException(S(entry["waiting"],"操作结果未确认；不会重发"));
  }catch{if(S(entry["state"])!="rejected")entry["state"]="unknown";Write(path,journal);throw;}
 }
 sealed class RestorePause(bool previous):IDisposable{public void Dispose(){if(previous)Write(P("pause"),new JsonObject());}}
 public IDisposable EnableActions(){bool wasPaused=File.Exists(P("pause"));File.Delete(P("pause"));return new RestorePause(wasPaused);}
}
