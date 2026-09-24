using BD2Equipment.Live;
static class TableReaderChecks {
 public enum Database { DB_COMMON=7, DB_CLIENT_LOCAL=9 }
 public enum Ordering { DESC=3, ASC=5 }
 public enum AlternateDatabase { DB_COMMON=42 }
 public sealed class Manager {
  public int SqlCalls,TableCalls;
  public List<T> GetTableList<T>(string sql){SqlCalls++;return new();}
  public List<T> GetTableList<T>(Database database,string table,string order,Ordering direction){
   if(database!=Database.DB_COMMON||table!="EquipmentTable"||order!=""||direction!=Ordering.ASC)throw new Exception("Wrong native table arguments");TableCalls++;return new(){(T)(object)"loaded"};
  }
 }
 public sealed class OnlySql {public List<T> GetTableList<T>(string sql)=>new();}
 public sealed class Ambiguous {
  public List<T> GetTableList<T>(Database db,string table,string order,Ordering direction)=>new();
  public List<T> GetTableList<T>(AlternateDatabase db,string table,string order,Ordering direction)=>new();
 }
 public static int Run(){int count=0;void Check(bool value){count++;if(!value)throw new Exception("Table reader check "+count);}
  var method=NativeTableReader.Resolve(typeof(Manager));var manager=new Manager();var result=(List<string>)method.MakeGenericMethod(typeof(string)).Invoke(manager,NativeTableReader.Arguments(method,"EquipmentTable"))!;
  Check(result.SequenceEqual(new[]{"loaded"}));Check(manager.SqlCalls==0&&manager.TableCalls==1);
  foreach(var type in new[]{typeof(OnlySql),typeof(Ambiguous)}){bool rejected=false;try{NativeTableReader.Resolve(type);}catch(InvalidOperationException){rejected=true;}Check(rejected);}
  bool invalid=false;try{NativeTableReader.Arguments(method,"EquipmentTable; DELETE");}catch(ArgumentException){invalid=true;}Check(invalid);
  Console.WriteLine("Native table reader checks: "+count);return count;
 }
}
