using System.Text.Json.Nodes;
using BD2Equipment.Core;
using static BD2Equipment.Core.J;
static class PresentationExecutionChecks {
 public static void Run(){
  string old=Environment.GetEnvironmentVariable("BD2_EQUIPMENT_DATA_ROOT")!;int checks=0;
  void Check(bool value){checks++;if(!value)throw new Exception("Presentation execution check "+checks);}
  try{
   Environment.SetEnvironmentVariable("BD2_EQUIPMENT_DATA_ROOT",Path.Combine(old,"presentation-execution"));Directory.CreateDirectory(Data);
   var stock=new JsonObject{["account"]="fixture",["player"]="fixture",["gold"]=100000,["potions"]=300,["materials"]=new JsonObject{["201"]=300,["204"]=300},["protected"]=new JsonObject(),["equipment"]=new JsonArray()};
   var plan=EquipmentPlanner.Powder(stock,new(){["count_limit"]=25});string id=S(plan["id"]);
   using(var game=new PowderGame(stock,plan,true)){
    bool failed=false;try{EquipmentExecution.Execute(plan,id,game);}catch(StepRejected){failed=true;}
    Check(failed&&game.Submissions==1&&game.Count==10);
    var saved=Read(Path.Combine(Data,id+".json"));Check(A(saved["operations"]).Count()==1&&saved["pending"]==null&&S(saved["presentation"]?["state"])=="pending");
   }
   using(var game=new PowderGame(stock,plan,false)){
    var result=EquipmentExecution.Execute(plan,id,game);
    Check(game.ResumedPresentation&&game.Submissions==1&&game.Count==15);
    Check(S(result["state"])=="completed"&&A(result["operations"]).Sum(o=>N(o["result"]?["count"]))==25);
    Check(S(result["presentation"]?["state"])=="completed");
   }
  }finally{Environment.SetEnvironmentVariable("BD2_EQUIPMENT_DATA_ROOT",old);}
  Console.WriteLine("Presentation execution checks: "+checks);
 }
 sealed class PowderGame(JsonObject stock,JsonObject plan,bool fail):IEquipmentGame {
  public int Submissions,Count;int next;bool pending;public bool ResumedPresentation;
  public JsonObject Stock()=>Copy(stock);public void RefinePreview(string id)=>throw new NotSupportedException();
  public int PowderPreview(JsonNode recipe,int count,int level){if(pending)throw new Exception("Next batch was reached before presentation completed");next=count;return count;}
  public JsonObject Transaction(JsonObject journal,string role,JsonObject action,Func<JsonObject,JsonArray,JsonObject,JsonObject> verify){
   Submissions++;Count+=next;pending=true;
   var r=new JsonObject{["recipe"]=Clone(A(plan["rows"]).First()["recipe"]),["count"]=next,["gold"]=0,["potions"]=0,["powder"]=0};
   journal["operations"]!.AsArray().Add(new JsonObject{["result"]=Copy(r)});journal["expected"]=Copy(stock);journal["pending"]=null;return r;
  }
  public void CompletePresentation(string role,bool waitForResult=true){if(!waitForResult)ResumedPresentation=true;if(fail)throw new StepRejected("Result UI incomplete; receipt retained");pending=false;}
  public void Cleanup(string destination){}public IDisposable EnableActions()=>this;public void Dispose(){}
 }
}
