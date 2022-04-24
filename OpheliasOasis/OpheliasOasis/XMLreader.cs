using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace OpheliasOasis
{
    [DataContract(Name = "XMLStr", Namespace = "OpheliasOasis")]
    public struct XMLformat
    {
        public XMLformat(ReservationDB db, Hotel hot, Calendar calend, String managerPass) 
        {
            R = db;
            H = hot;
            C = calend;
            M = managerPass;
        }

        [DataMember(Name = "XMLStrRDB")]
        public ReservationDB R { get; set;  }
        [DataMember(Name = "XMLStrH")]
        public Hotel H { get; set;  }
        [DataMember(Name = "XMLStrC")]
        public Calendar C { get; set; }
        [DataMember(Name = "XMLStrMP")]
        public String M { get; set; }

    }




    static class XMLreader
    {
        static private String filePath = "C:\\OpheliasOasis\\Archive\\";
        static public void XMLout(ReservationDB ResDB, Hotel hotel, Calendar cal, String ManPass) 
        {
            if (File.Exists(filePath + DateTime.Today.ToString("D"))) 
            {
                File.Delete(filePath + DateTime.Today.ToString("D"));
            }
            Console.WriteLine(filePath + DateTime.Today.ToString("D"));
            
            FileStream fs = new FileStream(filePath + DateTime.Today.ToString("D"), FileMode.Create);

            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
            DataContractSerializer ser = new DataContractSerializer(typeof(XMLformat));
            ser.WriteObject(writer, new XMLformat(ResDB, hotel, cal, ManPass));
            writer.Close();
            fs.Close();
        }


        static public XMLformat XMLin(DateTime day)
        {
            if (! File.Exists(filePath + day.ToString("D")))
            {
                throw new FileNotFoundException();
            }

            FileStream fs = new FileStream(filePath + day.ToString("D"), FileMode.OpenOrCreate);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

            // Create the DataContractSerializer instance.
            DataContractSerializer ser = new DataContractSerializer(typeof(XMLformat));

            // Deserialize the data and read it from the instance.
            XMLformat returnStruct = (XMLformat)ser.ReadObject(reader);
            returnStruct.R.reorganize();
            return returnStruct;


        }


    }
}
