using System.Text.Json.Nodes;
using BD2Equipment.Core;
using BD2.LocalIpc;
static class ConnectionRecoveryChecks
{
 public static void Run()
 {
  int count=0; void Check(bool ok){count++;if(!ok)throw new Exception("Connection recovery check "+count);}
  var location=new JsonObject{["processId"]=10,["startTicks"]=20};int reads=0;
  JsonObject Missing(){reads++;throw new TimeoutException("The operation has timed out.");}
  Check(LiveEquipment.ExistingSnapshot(null,Missing,location)==null&&reads==0);
  Check(LiveEquipment.ExistingSnapshot("known",Missing,location)==null&&reads==1);
  JsonObject Frame(long pid)=>new(){["ProcessId"]=pid,["ProcessStartTicks"]=20};
  Check(LiveEquipment.ExistingSnapshot("known",()=>Frame(10),location)!=null);
  Check(LiveEquipment.ExistingSnapshot("known",()=>Frame(11),location)==null);
  int attempts=0,delays=0;
  Check(EquipmentReadRetry.Read(()=>++attempts<3?throw new TimeoutException():17,_=>delays++)==17&&attempts==3&&delays==2);
  attempts=0;try{EquipmentReadRetry.Read<int>(()=>{attempts++;throw new TimeoutException();},_=>{});throw new Exception("unbounded retry");}catch(TimeoutException){Check(attempts==3);}
  attempts=0;try{EquipmentReadRetry.Read<int>(()=>{attempts++;throw new LeaseRevokedException();},_=>{});throw new Exception("lease swallowed");}catch(LeaseRevokedException){Check(attempts==1);}
  attempts=0;try{EquipmentReadRetry.Read<int>(()=>{attempts++;throw new InvalidDataException("identity mismatch");},_=>{});throw new Exception("identity swallowed");}catch(InvalidDataException){Check(attempts==1);}
  Check(!EquipmentReadRetry.Transient(new IOException("IPC: protocol")));
  Console.WriteLine("Connection recovery checks: "+count);
 }
}
