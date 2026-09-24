using BD2Equipment.Compatibility;
using Mono.Cecil;
using Mono.Cecil.Cil;
static class CompatibilityChecks
{
 public static int Run(){int count=0;void Check(bool pass){count++;if(!pass)throw new Exception("Compatibility check "+count);}
  string root=Path.Combine(Path.GetTempPath(),"bd2-equipment-compat-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
  string Make(string name,bool rename=false,bool missing=false,bool ambiguous=false){
   string path=Path.Combine(root,name+".dll");using var module=ModuleDefinition.CreateModule(name,ModuleKind.Dll);
   var t=new TypeDefinition("","UIFixture",TypeAttributes.Public|TypeAttributes.Class,module.TypeSystem.Object);module.Types.Add(t);
   if(!missing){foreach(var n in ambiguous?new[]{"ὡὡ","ὡὢ"}:new[]{rename?"ὡὢ":"ὡὡ"})t.Fields.Add(new FieldDefinition(n,FieldAttributes.Public,module.TypeSystem.Int32));}
   module.Write(path);return path;
  }
  using var before=new MetadataIndex(Make("before"));var type=before.Types.Single(t=>t.Name=="UIFixture");var f=type.Fields[0];
  var contract=new BindingContract(1,new[]{new TypeContract(type.FullName,before.Shape(type),Array.Empty<string>(),new[]{new MemberContract(f.Name,MetadataIndex.Signature(f),"",before.Uses(f))})},Array.Empty<ApiContract>(),new());
  Check(BindingResolver.Resolve(before,contract).Report.Status=="compatible");
  using var renamed=new MetadataIndex(Make("renamed",true));var result=BindingResolver.Resolve(renamed,contract);Check(result.Report.Status=="compatible"&&result.Report.RenamedMembers==1);
  using var absent=new MetadataIndex(Make("absent",missing:true));Check(BindingResolver.Resolve(absent,contract).Report.Status=="unsupported");
  using var duplicate=new MetadataIndex(Make("duplicate",ambiguous:true));Check(BindingResolver.Resolve(duplicate,contract).Report.Status=="unsupported");
  Console.WriteLine("Compatibility checks: "+count);return count;
 }
}
