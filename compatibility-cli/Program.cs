using BD2Equipment.Compatibility;
using Mono.Cecil;
using System.Text.Json;
if(args[0]=="cleanup-result"){
 var frame=BD2Equipment.Core.LiveEquipment.Snapshot();
 if(!BD2Equipment.Core.EquipmentUiFlow.Has(frame,"EquipmentBatchUpgradeResultPopupUI"))throw new InvalidOperationException("Expected existing equipment batch result");
 using var game=new BD2Equipment.Core.LiveEquipment();game.Cleanup("EquipmentMakingUI");
 Console.WriteLine("Existing result closed; crafting page ready; no resource action submitted.");return;
}
if(args[0]=="read-only-capture"){
 using var game=new BD2Equipment.Core.LiveEquipment(true);var captured=game.Capture();
 Console.WriteLine(JsonSerializer.Serialize(new{status="passed",equipment=captured["gear"]!.AsArray().Count,recipes=BD2Equipment.Core.EquipmentPlanner.Catalog["recipes"]!.AsArray().Count,resourcesSpent=false}));return;
}
if(args[0]=="catalog"){
 var tables=new System.Text.Json.Nodes.JsonObject();foreach(var dir in args.Skip(1))foreach(var file in Directory.GetFiles(dir,"*Table.json"))tables[Path.GetFileNameWithoutExtension(file)]=System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(file));
 var built=BD2Equipment.Core.CurrentCatalog.Build(tables,"offline-current-tables");
 BD2Equipment.Core.J.Write(Path.Combine(args[1],"derived-catalog.json"),built.Catalog);
 BD2Equipment.Core.J.Write(Path.Combine(args[1],"derived-display.json"),built.Display);
 Console.WriteLine($"Current tables: {built.Catalog["equipment"]!.AsObject().Count} equipment; {built.Catalog["recipes"]!.AsArray().Count} normal recipes");return;
}
if(args[0] is "prepare" or "taps" or "attach" or "upgrade" or "locate" or "self-test"){await BD2Equipment.Live.LiveEntry.RunAsync(args);return;}
using var index=new MetadataIndex(args[1]);
if(args[0]=="inspect"){
 foreach(var t in index.Types.Where(t=>t.FullName==args[2]||t.Name==args[2])){
 Console.WriteLine(t.FullName);foreach(var m in MetadataIndex.Members(t).Where(m=>args.Length<4||m.Name.Contains(args[3]))){Console.WriteLine(m.FullName + (m is MethodDefinition mm ? " static="+mm.IsStatic : ""));if(m is MethodDefinition f){foreach(var x in f.Parameters)Console.WriteLine($"  {x.Name}: {x.ParameterType} = {x.Constant}");if(args.Length>4&&f.HasBody)foreach(var i in f.Body.Instructions)Console.WriteLine(i);}}
 }
} else if(args[0]=="generate") {
 var c=ContractGenerator.Generate(index,args[2]);File.WriteAllText(args[3],JsonSerializer.Serialize(c,new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"{c.Types.Length} types");
} else if(args[0]=="check"){
 var c=JsonSerializer.Deserialize<BindingContract>(File.ReadAllText(args[2]))!;Console.WriteLine(JsonSerializer.Serialize(BindingResolver.Resolve(index,c).Report));
}
