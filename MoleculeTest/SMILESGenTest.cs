using MoleculeLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace MoleculeTest
{
    [TestClass]
    public class SMILESGenTest
    {
        [TestMethod]
        public async Task TestSMILESGeneratorAsync()
        {
            Atom carbon1 = new Atom("C");
            Atom carbon2 = new Atom("C");
            Atom Oxygen = new Atom("O");
            new Bond(carbon1, carbon2, 1);
            new Bond(carbon2, Oxygen, 2);

            SMILESGenerator generator = new SMILESGenerator();
            string smiles = generator.GenerateSMILES(carbon1);
            PubChemAPI api = new PubChemAPI();
            string response = await api.GetMoleculeNameAsync(smiles); //Smiles for ethanol
            var (name, APIsmiles) = PubChemAPI.ExtractMoleculeData(response);
            Console.WriteLine(smiles);
            Console.WriteLine(APIsmiles);    
            Assert.AreEqual("CC=O", smiles);
        }
        [TestMethod]
        public void TestSMILESGenerator2() {
            Atom carbon1 = new Atom("C");
            Atom carbon2 = new Atom("C");
            Atom Oxygen = new Atom("O");
            new Bond(carbon1, carbon2, 1);
            new Bond(carbon2, Oxygen, 1);

            SMILESGenerator generator = new SMILESGenerator();
            string smiles = generator.GenerateSMILES(carbon2);
            Console.WriteLine(smiles);
            Assert.AreEqual("CCO", smiles);
        }       
        [TestMethod]
        public void BenzeneTest() {
            Atom carbon1 = new Atom("C");
            Atom carbon2 = new Atom("C");
            Atom carbon3 = new Atom("C");
            Atom carbon4 = new Atom("C");
            Atom carbon5 = new Atom("C");
            Atom carbon6 = new Atom("C");

            new Bond(carbon1, carbon2, 1);
            new Bond(carbon2, carbon3, 2);
            new Bond(carbon3, carbon4, 1);
            new Bond(carbon4, carbon5, 2);
            new Bond(carbon5, carbon6, 1);
            new Bond(carbon6, carbon1, 2);

            SMILESGenerator generator = new SMILESGenerator();
            string smiles = generator.GenerateSMILES(carbon1);
            Console.WriteLine(smiles);
            Assert.AreEqual("C1=CC=CC=C1", smiles);
        }
    }   
}
