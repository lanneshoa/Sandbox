using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBEEGInteger.Wrapper;

namespace BBEEGInteger.EEG
{
    public class EEGMetadata
    {
        private EEGInfos _eegInfos;
        public EEGInfos eegInfos
        {
            get { return _eegInfos; }
            set { _eegInfos = value; }
        } 

        // Il est possible qu'il n'y ait aucune information d'affichage au sein des fichiers DeltaMed : dans ce cas, ne pas tenir compte de cet objet.
        // Il est également possible que dans un fichier DeltaMed, il y ait plusieurs configurations d'affichage pour un EEG, auquel cas, il faudra transformer cet objet en un tableau
        // => A voir en fonction de ce qu'on aura dans le fichier DeltaMed
        private DisplayConfiguration _displayConfiguration;
        public DisplayConfiguration displayConfiguration
        {
            get { return _displayConfiguration; }
            set { _displayConfiguration = value; }
        }

        // Là-dedans, il faudra mettre toutes les infos présentes dans le fichier DeltaMed, et pouvant potentiellement être utiles pour l'affichage du document et sa description (informations patient ? A voir avec les docteurs...)
        private AdditionnalInfos _additionnalInfos;
        public AdditionnalInfos additionnalInfos
        {
            get { return _additionnalInfos; }
            set { _additionnalInfos = value; }
        }

        // Pour l'instant, ne rien mettre là-dedans, à terme, il faudra y mettre les informations sur les vidéos DeltaMed, et notamment les offsets de départ/fin de chaque vidéo... A creuser lorsqu'on aura davantage d'infos sur la structure du fichier DeltaMed.
        private Videos _videos;
        public Videos videos
        {
            get { return _videos; }
            set { _videos = value; }
        }

        public EEGMetadata Parse(TCoh3 recordInformation)
        {
            this.eegInfos = new EEGInfos();
            this.eegInfos.duration = recordInformation.duration;
            this.eegInfos.frequency = recordInformation.frequency;
            this.eegInfos.numberOfSignals = recordInformation.electrodes;
            this.eegInfos.signals = new List<Signal>();

            for (int i = 0; i < recordInformation.electrodes; i++)
            {
                string name=string.Empty;
                for(int j=0;j<8;j++)
                {
                    name += recordInformation.name[i].name[j].ToString();
                }

                string unit = string.Empty;
                for (int j = 0; j < 4; j++)
                {
                    unit += recordInformation.unit[i].unit[j].ToString();
                }

                this.eegInfos.signals.Add(
                    new Signal { 
                        label = string.Format("{0}_{1}", Convert.ToInt32(recordInformation.type[i]).ToString(), name.Replace("\0", "")),
                        theta= recordInformation.theta[i],
                        phi = recordInformation.phi[i],
                        r= recordInformation.r[i],
                        minanal = recordInformation.minanal[i],
                        maxanal = recordInformation.maxanal[i],
                        minconv = recordInformation.minconv[i],
                        maxconv = recordInformation.maxconv[i],
                        unit = unit.Replace("\0", "")
                });
            }

            string date = string.Empty;
            for (int j = 0; j < 20; j++)
            {
                date += recordInformation.date[j].ToString();
            }
            this.eegInfos.date = date.Replace("\0", "");

            return this;
        }
        
    }

    public class DisplayConfiguration
    {
        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

    public class AdditionnalInfos
    {
        
    }

    public class Videos
    {

    }
}
