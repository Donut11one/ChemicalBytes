using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoleculeLibrary
{
    public class PubChemAPI
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetMoleculeNameAsync(string smiles)
        {
            string url = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/smiles/{Uri.EscapeDataString(smiles)}/JSON";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception("PubChem request failed.");
        }

        public static (string? Name, string? Smiles) ExtractMoleculeData(string jsonResponse)
        {
            try
            {
                JsonDocument doc = JsonDocument.Parse(jsonResponse);
                JsonElement root = doc.RootElement;

                // Navigate to the "PC_Compounds" array
                if (root.TryGetProperty("PC_Compounds", out JsonElement compoundsArray) && compoundsArray.GetArrayLength() > 0)
                {
                    JsonElement compound = compoundsArray[0];

                    // Extract molecule name (IUPAC Preferred Name)
                    string? moleculeName = null;
                    string? smilesFormat = null;
                    //extract props array
                    if (compound.TryGetProperty("props", out JsonElement propsArray))
                    {
                        //Setting each element in json to props
                        foreach (JsonElement prop in propsArray.EnumerateArray())
                        {
                            //Extracting the urn label name and value
                            if (prop.TryGetProperty("urn", out JsonElement urn) &&
                                urn.TryGetProperty("label", out JsonElement label) &&
                                urn.TryGetProperty("name", out JsonElement name) &&
                                prop.TryGetProperty("value", out JsonElement value))
                            {
                                //Initializing the name of the label and name extracted
                                //The "??" is to set the string equal to "" if GetString returns null
                                string labelText = label.GetString() ?? "";
                                string nameText = name.GetString() ?? "";

                                //Check if the LabelText is the IUPAC name which is the offical name of each chemical compound
                                // using Systematic name as it is the way a compound is named from tis strucutre
                                if (labelText == "IUPAC Name" && nameText == "Systematic")
                                {
                                    //Set the molecule Name to the string given in JSON
                                    moleculeName = value.GetProperty("sval").GetString();
                                }
                                //Check if labelText is equal to SMILES as the smiles format is stored in this part of the JSON
                                //Check if the nameText is canonical becuase it is the most accurate
                                else if (labelText == "SMILES" && nameText == "Canonical")
                                {
                                    //Set the smilesformat to the String given in the JSON
                                    smilesFormat = value.GetProperty("sval").GetString();
                                }
                            }
                        }
                    }

                    return (moleculeName, smilesFormat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }

            return (null, null);
        }
    }
}
