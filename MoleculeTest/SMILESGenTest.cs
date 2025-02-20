using MoleculeLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleculeTest
{
    [TestClass]
    public class SMILESGenTest
    {
        [TestMethod]
        public void TestSMILESGenerator()
        {
            Atom carbon1 = new Atom("C");
            Atom carbon2 = new Atom("C");
            Atom Oxygen = new Atom("O");
            new Bond(carbon1, carbon2, 1);
            new Bond(carbon2, Oxygen, 2);

            SMILESGenerator generator = new SMILESGenerator();
            string smiles = generator.GenerateSMILES(carbon1);
            Console.WriteLine(smiles);
            Assert.AreEqual("C(C(=O))", smiles);
        }
        [TestMethod]
        public void TestSMILESGenerator2() {
            Atom carbon1 = new Atom("C");
            Atom carbon2 = new Atom("C");
            Atom Oxygen = new Atom("O");
            new Bond(carbon1, carbon2, 1);
            new Bond(carbon2, Oxygen, 1);

            SMILESGenerator generator = new SMILESGenerator();
            string smiles = generator.GenerateSMILES(carbon1);
            Console.WriteLine(smiles);
            Assert.AreEqual("C(C(O))", smiles);
        }
    }
}
