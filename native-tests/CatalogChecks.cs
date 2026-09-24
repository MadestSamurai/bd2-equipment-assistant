using System.Text.Json.Nodes;
using BD2Equipment.Core;
using static BD2Equipment.Core.J;
static class CatalogChecks
{
 public static int Run(){int count=0;void Check(bool pass){count++;if(!pass)throw new Exception("Current catalog check "+count);}
  var tables=JsonNode.Parse("""
  {"EquipmentTable":[{"id":10,"grade":1,"rankGroupId":10,"growthGroupId":10,"expect":1,"itemNameTextId":10}],
   "EquipmentMakingTable":[{"id":123,"resultItemType":9,"resultItemCount":1,"resultItemId":1,"talentLevel":1,"materialItemId":[201],"materialItemType":[8],"materialItemCount":[3],"itemNameLocalTextId":10}],
   "EquipmentGradeTable":[{"id":1,"rankSmeltItemType":[4,8],"rankSmeltItemId":[0,10],"rankSmeltItemCount":[20,1]}],
   "EquipmentRankTable":[],"EquipmentGrowthTable":[],"EquipmentExpectTable":[{"groupId":1}],
   "RandomBoxTable":[{"id":1,"rewardGroupId":1}],"RewardGroupTable":[{"id":1,"dropCount":1,"itemId":[10],"itemType":[10]}],
   "TalentSkillTable":[{"classType":9,"id":1,"catalystValue":3}],
   "ResourceTable":[{"id":201,"itemNameTextId":10}],"CharTable":[{"id":1000,"uniqueCharId":100,"charNameTextId":10}],
   "NameTextTable":[{"id":10,"textCn":"测试装备","textEn":"Test equipment"}],"LocalTextTable":[{"id":10,"textCn":"测试配方"}],
   "EquipmentOptionTable":[{"groupId":1,"id":1,"defaultValue":2}]
  }
  """)!.AsObject();
  for(int i=1;i<=3;i++)tables["EquipmentRankTable"]!.AsArray().Add(new JsonObject{["groupId"]=10,["id"]=i,["rankValue"]=new JsonArray(1,2,3,4)});
  for(int i=1;i<=9;i++){tables["EquipmentExpectTable"]![0]!["level"+i]=i*10;tables["EquipmentGrowthTable"]!.AsArray().Add(new JsonObject{["groupId"]=10,["id"]=i,["breakResultItemType"]=new JsonArray(8),["breakResultItemId"]=new JsonArray(10),["breakResultItemCount"]=new JsonArray(i+1)});}
  var baseline=CurrentCatalog.Build(tables,"client-a");Check(N(baseline.Catalog["recipes"]![0]!["id"])==123);Check(N(baseline.Catalog["recipes"]![0]!["gold"]!["7"])==70);Check(N(baseline.Catalog["recipes"]![0]!["powder"]!["7"])==8);Check(S(baseline.Display["equipment"]!["10"]!["names"]!["en-US"])=="Test equipment");
  var changed=Copy(tables);changed["EquipmentExpectTable"]![0]!["level7"]=90;changed["TalentSkillTable"]![0]!["catalystValue"]=5;changed["EquipmentGradeTable"]![0]!["rankSmeltItemCount"]![1]=2;
  var revised=CurrentCatalog.Build(changed,"client-a");Check(Hash(baseline.Catalog)!=Hash(revised.Catalog));Check(N(revised.Catalog["recipes"]![0]!["gold"]!["7"])==90);Check(N(revised.Catalog["recipes"]![0]!["potions"])==5);Check(N(revised.Catalog["refine_cost"]!["1"]!["powder"])==2);
  var added=Copy(tables);var gear=Copy(added["EquipmentTable"]![0]);gear["id"]=9999;added["EquipmentTable"]!.AsArray().Add(gear);Check(CurrentCatalog.Build(added,"client-b").Catalog["equipment"]!["9999"]!=null);Check(Hash(CurrentCatalog.Build(tables,"client-b").Catalog)!=Hash(baseline.Catalog));
  void Rejected(Action<JsonObject> change){var data=Copy(tables);change(data);bool rejected=false;try{CurrentCatalog.Build(data,"client-a");}catch(InvalidDataException){rejected=true;}Check(rejected);}
  Rejected(t=>t.Remove("EquipmentTable"));Rejected(t=>t["EquipmentGradeTable"]![0]!["rankSmeltItemType"]![1]=99);Rejected(t=>t["EquipmentMakingTable"]![0]!["resultItemCount"]=2);Rejected(t=>t["EquipmentMakingTable"]![0]!["materialItemCount"]![0]=-1);Rejected(t=>t["EquipmentRankTable"]!.AsArray().RemoveAt(0));
  Console.WriteLine("Current catalog checks: "+count);return count;
 }
}
