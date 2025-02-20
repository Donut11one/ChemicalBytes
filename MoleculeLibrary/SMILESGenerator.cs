using System.Collections.Generic;

namespace MoleculeLibrary
{
    public class SMILESGenerator
    {
        private HashSet<Atom> visited = new HashSet<Atom>();
        private HashSet<Bond> visitedBonds = new HashSet<Bond>();

        public string GenerateSMILES(Atom startAtom)
        {
            visited.Clear();
            return DFS(startAtom);
        }

        private string DFS(Atom atom)
        {
            if (visited.Contains(atom))
            {
                return "";
            }
            visited.Add(atom);

            string smiles = atom.Element;

            foreach (var bond in atom.Bonds)
            {
                if (!visitedBonds.Contains(bond))
                {
                    visitedBonds.Add(bond);
                    Atom nextAtom = bond.Atom1 == atom ? bond.Atom2 : bond.Atom1;
                    if(bond.BondType == 2)
                    {
                        smiles += "(=";
                        smiles += DFS(nextAtom) + ")";
                    }
                    else
                    {
                        smiles += "(" + DFS(nextAtom) + ")";

                    }
                }
            }

            return smiles;
        }
    }
}
