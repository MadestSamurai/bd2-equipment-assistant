using BD2Equipment.Live;
internal static class CatalogTransferChecks {
 public static void Run(){int checks=0;void C(bool b){checks++;if(!b)throw new Exception("Catalog segmented transfer regression");}void Reject(Action a){try{a();}catch(InvalidDataException){checks++;return;}throw new Exception("Invalid catalog transfer accepted");}
 var data=new byte[17*1024*1024+31];new Random(1234).NextBytes(data);string id="request",hash=CatalogTransfer.Hash(data);int n=CatalogTransfer.Parts(data.Length);var d=new CatalogDownload(id,data.Length,n,CatalogTransfer.PartBytes,hash);
 Reject(()=>d.Complete());var first=CatalogTransfer.Part(data,0);Reject(()=>d.Add("old",0,first,CatalogTransfer.Hash(first)));Reject(()=>d.Add(id,1,first,CatalogTransfer.Hash(first)));Reject(()=>d.Add(id,0,first,new string('0',64)));
 for(int i=0;i<n;i++){var part=CatalogTransfer.Part(data,i);C(part.Length<=1024*1024);d.Add(id,i,part,CatalogTransfer.Hash(part));}
 C(d.Complete().SequenceEqual(data));Reject(()=>d.Add(id,0,first,CatalogTransfer.Hash(first)));Reject(()=>new CatalogDownload(id,data.Length,n-1,CatalogTransfer.PartBytes,hash));
 Console.WriteLine("Catalog transfer >16 MiB: "+checks+" checks passed");}
}
