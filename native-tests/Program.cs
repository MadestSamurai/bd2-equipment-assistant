using System.Text;
using System.Text.Json.Nodes;
using BD2Equipment.Core;
using static BD2Equipment.Core.J;
Console.OutputEncoding=new UTF8Encoding(false);
bool regression=args.Length==0;JsonArray? cases=null;if(regression){using var stream=typeof(FakeGame).Assembly.GetManifestResourceStream("Tests.Cases.json")!;cases=JsonNode.Parse(stream)!.AsArray();Environment.SetEnvironmentVariable("BD2_EQUIPMENT_DATA_ROOT",Path.Combine(AppContext.BaseDirectory,"test-data",Guid.NewGuid().ToString("N")));Console.WriteLine(BD2Equipment.Live.PolicyChecks.Run());}
string testData=Data;int testIndex=0;var failures=new List<string>();
void Record(JsonObject result){if(!regression){Console.WriteLine(Canonical(result));return;}try{Oracle.Check(cases![testIndex-1]!.AsObject(),result);}catch(Exception ex){failures.Add((testIndex-1)+": "+ex.Message);}}

foreach(var line in (regression?cases!.Select(c=>c!["job"]!.ToJsonString()):File.ReadLines(args[0]))){
 try{
  Environment.SetEnvironmentVariable("BD2_EQUIPMENT_DATA_ROOT",Path.Combine(testData,(testIndex++).ToString()));
  var job=JsonNode.Parse(line)!.AsObject();JsonNode result;
  switch(S(job["operation"])){
   case "catalog_hash":result=JsonValue.Create(Hash(EquipmentPlanner.Catalog))!;break;
   case "verify_refine":result=LiveEvidence.VerifyRefine(S(job["instance"]),job["cost"]!, (int)N(job["requested"]),(int)N(job["native_target"]),job["old"]!.AsObject(),job["packets"]!.AsArray(),job["new"]!.AsObject());break;
   case "verify_powder":result=LiveEvidence.VerifyPowder(job["recipe"]!, (int)N(job["count"]),(int)N(job["level"]),job["old"]!.AsObject(),job["data"]!.AsObject(),job["new"]!.AsObject());break;
   case "responses":result=LiveEvidence.Responses(S(job["role"]),job["events"]!.AsArray(),job["before"]!.AsObject(),job["after"]!.AsObject(),(int)N(job["requested"]));break;
   case "resumable":result=EquipmentExecution.Resumable(job["stock"]!.AsObject())??new JsonObject();break;
   case "normalize":result=EquipmentPlanner.Normalize(job["evidence"]!.AsObject());break;
   case "execution":
    var stock=job["stock"]!.AsObject();var plan=EquipmentPlanner.Refine(stock,job["settings"]!.AsObject());using(var game=new FakeGame(stock,job["results"]!.AsArray(),B(job["stop_after"]))){try{var journal=EquipmentExecution.Execute(plan,S(plan["id"]),game);result=new JsonObject{["state"]=Clone(journal["state"]),["progress"]=LiveEvidence.Progress(journal),["actions"]=game.Actions.DeepClone(),["cleanup"]=game.Destination};}catch(StepRejected){result=new JsonObject{["state"]="stopped",["actions"]=game.Actions.DeepClone(),["stock"]=game.Stock()};}}break;
   default:result=EquipmentService.Handle(job);break;
  }
  Record(new JsonObject{["result"]=result});
 }catch(Exception ex){Record(new JsonObject{["error"]=ex.Message,["type"]=ex.GetType().Name});}
}
if(regression){CatalogChecks.Run();CompatibilityChecks.Run();TableReaderChecks.Run();Console.WriteLine(new JsonObject{["checks"]=testIndex,["status"]=failures.Count==0?"passed":"failed",["failures"]=new JsonArray(failures.Select(x=>(JsonNode?)JsonValue.Create(x)).ToArray())}.ToJsonString());if(failures.Count>0)Environment.ExitCode=1;}
sealed class FakeGame(JsonObject initial,JsonArray results,bool stop):IEquipmentGame{
 JsonObject current=Copy(initial);int index;public readonly JsonArray Actions=new();public string Destination="";
 public JsonObject Stock()=>Copy(current);public void RefinePreview(string instance){}public int PowderPreview(JsonNode recipe,int count,int level)=>throw new NotImplementedException();public void Cleanup(string destination)=>Destination=destination;public IDisposable EnableActions()=>this;public void Dispose(){}
 public JsonObject Transaction(JsonObject journal,string role,JsonObject action,Func<JsonObject,JsonArray,JsonObject,JsonObject> verify){
  Actions.Add(Copy(action));var item=results[index++]!;int requested=(int)N(action["items"]![1]);int count=(int)N(item["attempts"]);string instance=S(action["items"]![0]);var old=Copy(current);var packets=new JsonArray();int remaining=count;
  do{int n=Math.Min(1000,remaining);packets.Add(new JsonObject{["TryCount"]=n,["ConsumeItemInfo"]=new JsonArray(new JsonObject{["type"]=4,["id"]=0,["count"]=80*n},new JsonObject{["type"]=1,["id"]=10,["count"]=30*n}),["EquipInfo"]=n>0?new JsonObject{["invenIndex"]=long.Parse(instance),["baseInfo"]=new JsonObject{["rank"]=Clone(item["ranks"])}}:null,["ResultType"]="UpgradeSuccess",["LackItemInfo"]=new JsonArray()});remaining-=n;}while(remaining>0);
  current["gold"]=N(current["gold"])-80*count;current["materials"]!["10"]=N(current["materials"]!["10"])-30*count;if(count>0)A(current["equipment"]).Single(g=>S(g["instance"])==instance)["ranks"]=Clone(item["ranks"]);var r=verify(old,packets,current);journal["expected"]=Copy(current);journal["operations"]!.AsArray().Add(new JsonObject{["result"]=Copy(r)});Write(Path.Combine(Data,S(journal["id"])+".json"),journal);if(stop)File.WriteAllText(Stop,"stop");return r;
 }
}
