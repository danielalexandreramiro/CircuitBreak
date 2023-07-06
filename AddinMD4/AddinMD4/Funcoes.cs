using ExcelDna.Integration;

namespace AddinMD4
{

    public class Funcoes
    {

        [ExcelFunction(Name = "MD4.Ola", Description = "Retorna ola")]
        public static string Ola()
        {
            //return new Funcoes2().SubscribeMD4();
            return new Funcoes2().UsoCircuitBreakAntes();
            
            
        }
    }
}
