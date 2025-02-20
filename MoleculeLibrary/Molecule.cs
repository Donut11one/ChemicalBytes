using System.Collections.Generic;

namespace MoleculeLibrary
{
    public class Atom
    {
        public string Element { get; } // "C", "O", "H", etc.
        public List<Bond> Bonds { get; } = new List<Bond>();

        private static readonly Dictionary<string, int> MaxBondCounts = new()
        {
            { "H", 1 }, { "O", 2 }, { "N", 3 }, { "C", 4 } // Define valencies
        };

        public Atom(string element)
        {
            Element = element;
        }

        public int TotalBondCount() => Bonds.Sum(b => b.BondType);

        public bool CanFormBond(int bondType)
        {
            if (!MaxBondCounts.ContainsKey(Element)) return false;
            return (TotalBondCount() + bondType) <= MaxBondCounts[Element];
        }
    }

    public class Bond
    {
        public Atom Atom1 {
            get; 
        }
        public Atom Atom2 { 
            get;
        }
        public int BondType { get; } // 1 = single, 2 = double, etc.
        public Bond(Atom atom1, Atom atom2, int bondType)
        {
            if (!atom1.CanFormBond(bondType) || !atom2.CanFormBond(bondType))
            {
                throw new System.Exception("Bond exceeds atom valency");
            }
            Atom1 = atom1;
            Atom2 = atom2;
            BondType = bondType
            atom1.Bonds.Add(this);
            atom2.Bonds.Add(this);
        }
    }
}
