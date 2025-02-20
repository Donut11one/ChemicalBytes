using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleculeLibrary;
using System;

namespace MoleculeTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestValidStructureforCarbon()
        {
            Atom carbon = new Atom("C");
            Atom hydrogen1 = new Atom("H");
            Atom hydrogen2 = new Atom("H");
            Atom hydrogen3 = new Atom("H");
            Atom hydrogen4 = new Atom("H");

            new Bond(carbon, hydrogen1, 1);
            new Bond(carbon, hydrogen2, 1);
            new Bond(carbon, hydrogen3, 1);
            new Bond(carbon, hydrogen4, 1);

            Assert.AreEqual(4, carbon.TotalBondCount());
        }        [TestMethod]
        public void TestValidStructureforOxygen()
        {
            Atom carbon = new Atom("C");
            Atom Oxygen = new Atom("O");

            new Bond(carbon, Oxygen, 2);

            Assert.AreEqual(2, carbon.TotalBondCount());
        }
        [TestMethod]
        [ExpectedException(typeof(Exception))] //checks for exceptions
        public void TestInvalidBond()
        {
            Atom carbon = new Atom("C");
            Atom oxygen1 = new Atom("O");
            Atom oxygen2 = new Atom("O");

            new Bond(carbon, oxygen1, 2);
            Assert.IsFalse(oxygen1.CanFormBond(1)); // this should be false

            new Bond(carbon, oxygen2, 2); //Should fail oxygen cant have more than 2 bonds


            new Bond(carbon, new Atom("H"), 1); // should throw an exception

        }        
        [TestMethod]
        [ExpectedException(typeof(Exception))] //checks for exceptions
        public void TestInvalidBonds2()
        {
            Atom carbon = new Atom("C");
            Atom oxygen1 = new Atom("O");
            Atom oxygen2 = new Atom("O");
            Atom Hydrogen1 = new Atom("H");
            Atom Hydrogen2 = new Atom("H");

            new Bond(carbon, oxygen1, 1);
            Assert.IsTrue(oxygen1.CanFormBond(1)); // this should be True
            new Bond(carbon, oxygen2, 2);  //+2 bonds to carbon
            new Bond(carbon, Hydrogen1, 1); //Last bond taken by hydrogen1
            new Bond(carbon, Hydrogen2 , 1); //Should throw error as the carbon should only have up to 4 bonds and this is a 5th bond



        }
        
    }
}
