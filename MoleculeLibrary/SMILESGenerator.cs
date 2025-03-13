using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoleculeLibrary
{
    public class SMILESGenerator
    {
        private Dictionary<Atom, int> cycleLabels = new Dictionary<Atom, int>();
        private HashSet<Bond> visitedBonds = new HashSet<Bond>();
        private HashSet<Atom> visitedAtoms = new HashSet<Atom>();
        private int cycleIndex = 1;

        public string GenerateSMILES(Atom startAtom)
        {
            cycleLabels.Clear();
            visitedBonds.Clear();
            visitedAtoms.Clear();
            cycleIndex = 1;
            return DFS(startAtom, null, new StringBuilder());
        }

        private string DFS(Atom atom, Atom? parent, StringBuilder smiles)
        {
            visitedAtoms.Add(atom);
            smiles.Append(atom.Element);

            foreach (var bond in atom.Bonds.OrderBy(b => b.Atom1 == atom ? b.Atom2.Element : b.Atom1.Element)) //Order atoms to improve SMILES consistency
            {
                Atom nextAtom = bond.Atom1 == atom ? bond.Atom2 : bond.Atom1;

                if (nextAtom == parent) 
                {
                    continue;
                }

                if (visitedBonds.Contains(bond))
                {
                    continue;
                }
                visitedBonds.Add(bond);
                smiles.Append(GetBondSymbol(bond.BondType));

                if (visitedAtoms.Contains(nextAtom))
                {
                    if (!cycleLabels.ContainsKey(nextAtom))
                    {
                        cycleLabels[nextAtom] = cycleIndex;
                        cycleIndex++;
                    }
                    smiles.Append(cycleLabels[nextAtom]);
                }
                else
                {
                    DFS(nextAtom, atom, smiles);
                }
            }

            return smiles.ToString();
        }

        private string GetBondSymbol(int bondType)
        {
            return bondType switch
            {
                1 => "",  // Single bonds are implied in SMILES
                2 => "=", // Double bond
                3 => "#", // Triple bond
                _ => ""  // Default case (shouldn't happen)
            };
        }
    }
}