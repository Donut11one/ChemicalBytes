using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoleculeLibrary;
using System;

namespace MoleculeTest
{
    [TestClass]
    public class PubChemAPITest
    {
        [TestMethod]
        public async Task EthanalTest()
        {
            PubChemAPI api = new PubChemAPI();
            string response = await api.GetMoleculeNameAsync("CC=O"); //Smiles for ethanol
            var (name, smiles) = PubChemAPI.ExtractMoleculeData(response);//parse out data for JSON as the api returns a JSON
            //check if the returned string from the api and JSON parser is equal to ethanol which is the respective name for CCO
            Assert.IsTrue(name == "ethanal", "Expected response to be Ethanol"); 
            //check if the returned string from the api is CCO for smiles format
            Assert.IsTrue(smiles == "CC=O", "Expected response to be CCO");
        }
        [TestMethod]
        public async Task EthanoicAcidTest()
        {
            PubChemAPI api = new PubChemAPI();
            string response = await api.GetMoleculeNameAsync("CC(=O)O"); //Smiles for ethanol
            var (name, smiles) = PubChemAPI.ExtractMoleculeData(response);//parse out data for JSON as the api returns a JSON
            Console.WriteLine(response);
            //check if the returned string from the api and JSON parser is equal to ethanol which is the respective name for CCO
            Assert.IsTrue(name == "ethanoic acid", "Expected response to be ethanoic acid");
            //check if the returned string from the api is CCO for smiles format
            Assert.IsTrue(smiles == "CC(=O)O", "Expected response to be CCO");
        }
    }
}
