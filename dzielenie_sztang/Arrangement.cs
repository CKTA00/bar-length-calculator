using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bar_length_calculator
{
    class Arrangement
    {
        // TODO: Not sure if its needed for something. Probably just remove it.
        /// <summary>
        /// Global total count of objects of type Arrangement created in one runtime.
        /// </summary>
        public static int _objectCount;

        /// <summary>
        /// Ordered arrangement of elements.
        /// </summary>
        List<Element> layout;

        /// <summary>
        /// Waste in cm that is unused by layout.
        /// </summary>
        private float remnant;
        /// <summary>
        /// Waste in cm that is unused by layout.
        /// </summary>
        public float Remnant() { return remnant; }

        // TODO: Clarify what it is
        /// <summary> 
        /// Amount of (main?) bars. Helper variable
        /// </summary>
        private int barAmount = 0;

        // TODO: Clarify what it is
        /// <summary>
        /// Amount of bars.
        /// </summary>
        public int BarAmount { get => barAmount; set => barAmount = value; }

        public string ToString(ShowMode sm)
        {
            string s=""; 
            //TODO: Replace if statement and ShowMode with strategy pattern
            if(sm==ShowMode.Old)
            {
                s += "[";
                foreach (var el in layout)
                {
                    s += el.quantity + ", ";
                }
                s += "r = " + remnant + "] x " + barAmount;
            }
            else if(sm==ShowMode.Default || sm==ShowMode.All)
            {
                foreach (var e in layout)
                {
                    if (e.quantity > 0 || sm == ShowMode.All)
                    {
                        if(e.quantity==1)
                            s += "    " + e.quantity + " element o długości "; // TODO: Make this string language dependent
                        else if(e.quantity<5)
                            s += "    " + e.quantity + " elementy o długości "; // TODO: Make this string language dependent
                        else
                            s += "    " + e.quantity + " elementów o długości "; // TODO: Make this string language dependent

                        if (e.length == Math.Floor(e.length))
                        {
                            int temp = (int)e.length;
                            s += temp.ToString();
                        }
                        else
                        {
                            s += e.length.ToString();
                        }
                        s += " cm\n";
                    }
                   
                }
            }
            else
            {
                foreach (var e in layout)
                {
                    if (e.quantity > 0 || sm == ShowMode.AllShort)
                    {
                        s += "    " + e.quantity + " x "; 
                        if (e.length == Math.Floor(e.length))
                        {
                            int temp = (int)e.length;
                            s += temp.ToString();
                        }
                        else
                        {
                            s += e.length.ToString();
                        }
                        s += " cm\n";
                    }

                       
                }
            }
            
            return s; // TODO: Make this string language dependent (all cases)
        }

        public override string ToString()
        {
            return ToString(ShowMode.Old);
        }

        public Arrangement(float main_bar_length, List<Element> typesOfElements)
        {
            // the rewriting of list of elemnts from argument to layout, but it sets quantities to 0
            layout = new List<Element>();
            layout.AddRange(typesOfElements);

            Element e = new Element();
            e.quantity = 0;
            for (int i = 0; i < layout.Count(); i++)
            {
                e.length = layout[i].length;
                layout[i] = e;
            }

            remnant = -1;
            _objectCount++;
        }

        public Arrangement(float main_bar_length, List<Element> typesOfElements, float rest)
        {
            // the rewriting of list of elemnts from argument to layout, but it sets quantities to 1 (different from above)
            layout = new List<Element>();
            layout.AddRange(typesOfElements);

            Element e = new Element();
            e.quantity = 0;
            for (int i = 0; i < layout.Count(); i++)
            {
                e.length = layout[i].length;
                layout[i] = e;
            }

            this.remnant = rest;
            _objectCount++;
        }

        /// <summary>
        /// Calculates and returns remnant of arrangement (also saves it to its state).
        /// </summary>
        /// <param name="mainBarLength">Length of the main bar in cm</param>
        /// <returns></returns>
        public float CalculateRemnants(float mainBarLength)
        {
            float s = 0;
            foreach (var el in layout)
            {
                s += el.quantity * el.length;
            }
            remnant = mainBarLength - s;
            return remnant;
        }

        public List<Element> GetLayout()
        {
            return layout;
        }

        /// <summary>
        /// Sprawdza czy podany element da się odciąć od pozostałej sztabki. Jeśli tak to odcina, w przeciwnym razie sztabka zostaje nienaruszona
        /// Checks if it is possible to cut the element from ramaining bar. If yes, cuts the element form bar.
        /// </summary>
        /// <param name="el">type of element</param>
        /// <returns>True if cutting was successful, False if cutting is impossible.</returns>
        public bool TryCutOff(Element el)
        {
            if (remnant - el.length > 0)
            {
                ++el.quantity;
                remnant -= el.length;
                return true;
            }
            else return false;
        }

        // TODO: This function feels unprofesional. Think of alternative solution
        /// <summary>
        /// Increments by 1 element in layout with specific index
        /// </summary>
        /// <param name="elementIndex">Index of element in the list</param>
        public void AddElement(int elementIndex)
        {
            Element e = new Element();
            e.quantity = layout[elementIndex].quantity + 1;
            e.length = layout[elementIndex].length;
            layout[elementIndex] = e;
        }
    }
}
