using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BBEEGInteger.EEG
{
    public class EEGInfos
    {
        // Liste des informations liées aux signaux
        private List<Signal> _signals;
        public List<Signal> signals
        {
            get { return _signals; }
            set { _signals = value; }
        }

        // Redondant avec signals.length, peut-être voué à disparaître...
        private int _numberOfSignals;
        public int numberOfSignals
        {
          get { return _numberOfSignals; }
          set { _numberOfSignals = value; }
        }

        // Nombre de valeurs, par signal, dans 1000ms
        private int _frequency;
        public int frequency
        {
          get { return _frequency; }
          set { _frequency = value; }
        }

        // Durée des enregistrements, en ms
        private int _duration;
        public int duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        private string _date;
        public string date
        {
            get { return _date; }
            set { _date = value; }
        }
    }

    public class Signal
    {
        private string _label;
        public string label
        {
            get
            {
                string result = string.Empty;

                string type = this._label.Substring(0, 1);
                string name = this._label.Substring(2, this._label.Length - 2);

                switch (type)
                {
                    case "0":
                        result = "EEG";
                        break;
                    case "1":
                        result = "polygraphic AC_channel";
                        break;
                    case "2":
                        result = "C channel";
                        break;
                    case "3":
                        result = "photic";
                        break;
                    case "4":
                        result = "depth electrode";
                        break;
                }
                return string.Format("{0} {1}",result,name);
            }
            set { _label = value; }
        }

        private int _theta;
        public int theta
        {
            get { return _theta; }
            set { _theta = value; }
        }

        private int _phi;
        public int phi
        {
            get { return _phi; }
            set { _phi = value; }
        }

        private int _r;
        public int r
        {
            get { return _r; }
            set { _r = value; }
        }

        private int _minanal;
        public int minanal
        {
            get { return _minanal; }
            set { _minanal = value; }
        }

        private int _maxanal;
        public int maxanal
        {
            get { return _maxanal; }
            set { _maxanal = value; }
        }

        private int _minconv;
        public int minconv
        {
            get { return _minconv; }
            set { _minconv = value; }
        }

        private int _maxconv;
        public int maxconv
        {
            get { return _maxconv; }
            set { _maxconv = value; }
        }

        private string _unit;
        public string unit
        {
            get { return _unit; }
            set { _unit = value; }
        }


        //private int _value;
        //public int value
        //{
        //    get { return this._value; }
        //    set { this._value = value; }
        //}



    }


}
