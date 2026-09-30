using BD2Equipment.Live;
internal static class CatalogProjectionChecks {
 public static void Run(){var p=new CatalogProjection();void C(bool v){if(!v)throw new Exception("Catalog dependency projection failed");}
 p.Remember("EquipmentTable",new {ItemNameTextId=10});p.Remember("ResourceTable",new{ItemNameTextId=11});p.Remember("CharTable",new{CharNameTextId=12});
 p.Remember("EquipmentMakingTable",new{ItemNameLocalTextId=20,ResultItemType=9,ResultItemId=30});
 C(p.Include("RandomBoxTable",new{Id=30}));C(!p.Include("RandomBoxTable",new{Id=99}));p.Remember("RandomBoxTable",new{RewardGroupId=40});
 C(p.Include("RewardGroupTable",new{Id=40}));C(!p.Include("RewardGroupTable",new{Id=41}));
 foreach(var id in new[]{10,11,12})C(p.Include("NameTextTable",new{Id=id}));C(!p.Include("NameTextTable",new{Id=13}));
 C(p.Include("LocalTextTable",new{Id=20}));C(!p.Include("LocalTextTable",new{Id=21}));C(p.Include("EquipmentOptionTable",new{Id=999}));
 Console.WriteLine("Catalog dependency projection: 11 checks passed");}
}
