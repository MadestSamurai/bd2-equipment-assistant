using System.Text.Json.Nodes;
using BD2Equipment.Core;
using static BD2Equipment.Core.J;
static class Oracle {
 public static void Check(JsonObject test,JsonObject got){string kind=S(test["kind"]);var want=test["expected"]!;void Require(bool ok,string reason){if(!ok)throw new Exception(reason);}
  if(kind=="reject"){Require(got["error"]!=null,"Expected rejection");return;}Require(got["result"]!=null,S(got["error"]));var value=got["result"]!;
  if(kind=="powder"){
   foreach(string k in new[]{"count","gold","potions","powder"})Require(N(value[k])==N(want[k]),k+" differs from independent oracle");EquipmentPlanner.Validate(value.AsObject());
   var job=test["job"]!;double Scarce(JsonNode plan)=>O(plan["materials"]).Sum(p=>(double)N(p.Value)/Math.Max(1,N(job["stock"]!["materials"]?[p.Key])-N(job["settings"]?["material_reserves"]?[p.Key])));
   Require(Math.Abs(Scarce(value)-Scarce(want))<1e-5,"Scarcity objective differs");
  }else if(kind=="execution"){
   Require(S(value["state"])==S(want["state"]),"Execution state");Require(A(value["actions"]).Count()==N(want["count"]),"Batch count");Require(A(value["actions"]).All(a=>N(a["items"]![1])==5000&&N(a["value"])==N(want["native"])),"Native batch parameters");
   if(S(want["state"])=="budget_exhausted")Require(N(value["progress"]!["powder"])==300000,"Budget accounting");if(S(want["state"])=="completed")Require(S(value["cleanup"])=="EquipmentUpgradeUI","Cleanup destination");
  }else Require(Equal(value,want),"Exact oracle mismatch");
 }
}