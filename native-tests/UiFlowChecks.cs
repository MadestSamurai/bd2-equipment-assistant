using System.Text.Json.Nodes;
using BD2Equipment.Core;
using static BD2Equipment.Core.J;
static class UiFlowChecks {
 static JsonObject Ui(string name,bool popup=false,int order=0,bool ready=true,string? path=null)=>new(){["Type"]=name,["Id"]=order+1,["Popup"]=popup,["Order"]=order,["InputReady"]=ready,["Path"]=path??"root/"+name};
 static JsonObject Frame(params JsonObject[] rows)=>new(){["ProcessId"]=1,["ProcessStartTicks"]=2,["Instance"]="i",["AccountKey"]="a",["PlayerKey"]="p",["Scene"]="s",["Surfaces"]=new JsonArray(rows.Select(x=>(JsonNode)x).ToArray())};
 public static void Run(){int checks=0;void Check(bool ok){checks++;if(!ok)throw new Exception("Result UI check "+checks);}
  double now=0;bool closed=false;var sent=new List<EquipmentUiAction>();
  JsonObject Late()=>!closed&&now>=8?Frame(Ui("EquipmentMakingUI"),Ui("EquipmentBatchUpgradeResultPopupUI",true,1000)):Frame(Ui("EquipmentMakingUI"));
  EquipmentUiFlow.Cleanup("EquipmentMakingUI","EquipmentBatchUpgradeResultPopupUI",Late,a=>{sent.Add(a);closed=true;},()=>{},()=>now+=.1,()=>now);
  Check(closed&&sent.Count==1&&now>=8&&now<8.2);Check(sent[0].Absent=="EquipmentBatchUpgradeResultPopupUI");
  now=0;sent.Clear();bool batch=true,preview=true;
  JsonObject Stack(){var rows=new List<JsonObject>{Ui("EquipmentMakingUI")};if(preview)rows.Add(Ui("EquipmentUpgradePopupUI",true,900));if(batch)rows.Add(Ui("EquipmentBatchUpgradeResultPopupUI",true,1000));return Frame(rows.ToArray());}
  EquipmentUiFlow.Cleanup("EquipmentMakingUI",null,Stack,a=>{sent.Add(a);if(a.Ui=="EquipmentBatchUpgradeResultPopupUI")batch=false;else preview=false;},()=>{},()=>now+=.1,()=>now);
  Check(sent.Select(a=>a.Ui).SequenceEqual(new[]{"EquipmentBatchUpgradeResultPopupUI","EquipmentUpgradePopupUI"}));
  now=0;sent.Clear();closed=false;
  JsonObject Animated()=>closed?Frame(Ui("EquipmentMakingUI")):Frame(Ui("EquipmentMakingUI"),Ui("EquipmentBatchUpgradeResultPopupUI",true,1000,now>=2));
  EquipmentUiFlow.Cleanup("EquipmentMakingUI",null,Animated,a=>{sent.Add(a);Check(now>=2);closed=true;},()=>{},()=>now+=.1,()=>now);
  Check(sent.Count==1);
  now=0;sent.Clear();
  EquipmentUiFlow.Cleanup("EquipmentMakingUI",null,()=>Frame(Ui("EquipmentMakingUI",ready:now>=1)),sent.Add,()=>{},()=>now+=.1,()=>now);
  Check(sent.Count==0&&now>=1);
  now=0;bool rejected=false;
  try{EquipmentUiFlow.Cleanup("EquipmentMakingUI",null,()=>Frame(Ui("EquipmentMakingUI"),Ui("MessagePopupUI",true,1100)),sent.Add,()=>{},()=>now+=.1,()=>now);}catch(StepRejected){rejected=true;}
  Check(rejected&&sent.Count==0);
  now=0;rejected=false;
  try{EquipmentUiFlow.Cleanup("EquipmentMakingUI","EquipmentBatchUpgradeResultPopupUI",()=>Frame(Ui("EquipmentMakingUI")),sent.Add,()=>{},()=>now+=.1,()=>now,1);}catch(StepRejected){rejected=true;}
  Check(rejected&&sent.Count==0);
  now=0;rejected=false;
  try{EquipmentUiFlow.Cleanup("EquipmentMakingUI",null,()=>Frame(Ui("EquipmentMakingUI")),sent.Add,()=>throw new StepRejected("paused"),()=>now+=.1,()=>now);}catch(StepRejected){rejected=true;}
  Check(rejected&&sent.Count==0);
  Check(EquipmentUiFlow.DismissibleBlocker(Frame(Ui("EquipmentMakingUI"),Ui("EquipmentBatchUpgradeResultPopupUI",true,1000)),"EquipmentMakingUI")=="EquipmentBatchUpgradeResultPopupUI");
  Check(EquipmentUiFlow.DismissibleBlocker(Frame(Ui("EquipmentMakingUI"),Ui("EquipmentUpgradePopupUI",true,1000)),"EquipmentMakingUI")==null);
  Check(LiveEquipment.Blockers(Frame(Ui("ItemGetPopupUI",true,1000,path:"root/item"),Ui("EquipmentInfoPopupUI",true,1000,path:"root/item/card")),"ItemGetPopupUI").Length==0);
  Check(LiveEquipment.Blockers(Frame(Ui("ItemGetPopupUI",true,1000,path:"root/item"),Ui("EquipmentInfoPopupUI",true,1000,path:"root/other")),"ItemGetPopupUI").Length==1);
  Console.WriteLine("Result presentation checks: "+checks);
 }
}
